# Architecture Decision Records (ADRs)

ADRs documentam decisões arquiteturais importantes, seu contexto, alternativas consideradas e consequências.

Formato: [ADR-NNN: Title](status) - date

---

## ADR-001: DDD como Padrão Arquitetural Primário

**Status**: ACCEPTED (2024-01-15)
**Context**: Projeto requer agnósticismo de modelo de IA, persistência flexível e skills compostas
**Decision**: Aplicar Domain-Driven Design em toda arquitetura

### Alternativas Consideradas
1. CRUD simples sem domain model
2. Data-Driven Architecture (tabelas como fonte de verdade)
3. Microservices isolados sem domínio compartilhado

### Decisão e Racional
Escolhemos DDD porque:
- ✅ Domínio não conhece tecnologia (Ollama, SQLite, RabbitMQ)
- ✅ Entidades isoladas facilitam substituição (ex: SQLite → PostgreSQL)
- ✅ Domain Events permitem comunicação entre contexts sem acoplamento
- ✅ Ubiquitous Language alinha dev com domain experts (operadores)

### Consequências
- ✅ Código mais complexo inicialmente (aprendizado de DDD necessário)
- ✅ Melhor manutenibilidade a longo prazo
- ✅ Facilita testes unitários de lógica de domínio
- ❌ Overhead de abstrações se projeto ficar muito pequeno
- ❌ Curva de aprendizado para novo developers

### Implementação
- Entities, Value Objects, Aggregates explícitos em `SentinelaOps.Domain`
- Repository Interfaces em Domain, implementações em Infrastructure
- Domain Events publicados e subscritos através de contextos

---

## ADR-002: Event-Driven Architecture para Comunicação Entre Contexts

**Status**: ACCEPTED (2024-01-15)
**Context**: Múltiplos Bounded Contexts precisam se comunicar sem acoplamento
**Decision**: Usar Domain Events + Message Queue (RabbitMQ) para comunicação assíncrona

### Alternativas Consideradas
1. Direct REST calls entre contexts
2. Shared Database (antipattern)
3. gRPC síncrono
4. Apache Kafka

### Decisão e Racional
Domain Events + RabbitMQ porque:
- ✅ Desacoplamento completo entre contexts
- ✅ Observabilidade (cada evento é auditado)
- ✅ Replayability (histórico de eventos)
- ✅ RabbitMQ é simples de operar (vs Kafka)
- ✅ Escala horizontal (múltiplos workers consomem fila)

### Consequências
- ✅ Eventual consistency (não problema para use case)
- ✅ Resiliência automática (circuit breaker, retry)
- ❌ Debugging distribuído mais complexo
- ❌ Network latency (< 10ms aceitável)

### Implementação
- Domain Events publicados em Application Service Handlers
- RabbitMQ como message broker (Docker Compose)
- Event handlers assíncronos com retry policy

---

## ADR-003: Agnósticismo Total de Modelo de IA

**Status**: ACCEPTED (2024-01-15)
**Context**: Projeto deve suportar múltiplos modelos (Ollama, Azure OpenAI, Anthropic, etc) sem mudança de código
**Decision**: Criar camada abstrata `IInferenceProvider` implementada por provedores específicos

### Alternativas Consideradas
1. Hardcoding de Ollama direto no domínio (Bad)
2. Factory pattern simples
3. Strategy pattern por modelo
4. Plugin architecture

### Decisão e Racional
IInferenceProvider interface + implementations porque:
- ✅ Domain não conhece Ollama, OpenAI, etc
- ✅ Nova implementação não requer mudança em código existente (Open/Closed)
- ✅ Testes podem usar Mock InferenceProvider
- ✅ Múltiplos provedores simultâneos (para Harness)

### Consequências
- ✅ Fácil adicionar novo modelo
- ✅ Testabilidade melhorada
- ❌ Pequeno overhead de abstração
- ❌ Nem todos modelos tem exatamente mesma interface (adaptar)

### Implementação
```csharp
public interface IInferenceProvider
{
    Task<InferenceResult> InferAsync(InferenceRequest request);
}

public class OllamaInferenceProvider : IInferenceProvider { }
public class AzureOpenAIInferenceProvider : IInferenceProvider { }
```

### Migração para Novo Modelo
Trocar de Ollama para Azure OpenAI:
```yaml
# appsettings.json antes:
Inference:
  Provider: Ollama
  Model: gemma3:4b

# appsettings.json depois:
Inference:
  Provider: AzureOpenAI
  Model: gpt-4-vision
  ApiKey: ${AZURE_OPENAI_KEY}
  Endpoint: https://...
```
Zero mudança em código de domínio.

---

## ADR-004: Skills como Unidades Independentes Compostas

**Status**: ACCEPTED (2024-01-15)
**Context**: Diferentes tipos de análise (perímetro, intrusão, falso positivo, etc) precisam ser independentes e compostas
**Decision**: Criar interface `ISkill` com pipeline configurável

### Alternativas Consideradas
1. Single monolithic function
2. Hardcoded sequence
3. Chain of Responsibility pattern
4. Plugin architecture

### Decisão e Racional
ISkill + SkillPipeline porque:
- ✅ Nova skill sem modificar código existente
- ✅ Pipeline reconfigurável por tipo de evento
- ✅ Cada skill testável independentemente
- ✅ Composição clara do fluxo de análise

### Consequências
- ✅ Extensibilidade garantida
- ✅ Reutilização de skills
- ❌ Latência é soma das skills (mitigado por paralelismo no Harness)

### Implementação
```csharp
public interface ISkill
{
    string Name { get; }
    Task<SkillResult> ExecuteAsync(SkillContext context);
}

public class SkillPipeline
{
    private List<ISkill> _skills;
    public async Task<PipelineResult> ExecuteAsync(MonitoringEvent evt) { }
}
```

### Exemplo de Pipeline
```yaml
Pipelines:
  Default:
    - PerimeterAnalysisSkill
    - FalsePositiveAnalysisSkill
    - SeverityClassificationSkill
    - IncidentSummarySkill
  Detailed:
    - PerimeterAnalysisSkill
    - IntrusionAnalysisSkill
    - MotionAnalysisSkill
    - FalsePositiveAnalysisSkill
    - SeverityClassificationSkill
    - IncidentSummarySkill
```

---

## ADR-005: Harness Engineering como Componente Obrigatório

**Status**: ACCEPTED (2024-01-15)
**Context**: Necessidade de experimentação, benchmark e comparação de modelos sem afetar produção
**Decision**: Criar Bounded Context dedicado ao Harness com capacidade de executar em paralelo

### Alternativas Consideradas
1. Sem harness (apenas produção)
2. Harness como feature da aplicação principal
3. Sistema completamente separado
4. Harness como notebook (Jupyter)

### Decisão e Racional
Harness como Bounded Context porque:
- ✅ Isolado de produção (sem impacto em performance)
- ✅ Executa múltiplos modelos em paralelo
- ✅ Versiona e testa prompts
- ✅ Gera métricas comparativas (precision, recall, latência)
- ✅ Permite rollback de prompt

### Consequências
- ✅ Experimentação contínua possível
- ✅ Data-driven decisions sobre modelos
- ❌ Complexidade adicional
- ❌ Uso de recursos (múltiplos modelos em paralelo)

### Implementação
- Endpoints: `/harness/benchmarks`, `/harness/comparisons`
- Database: mesma persistência (transações isoladas)
- Modelos: múltiplas instâncias de IInferenceProvider

---

## ADR-006: CorrelationId para Rastreabilidade Completa

**Status**: ACCEPTED (2024-01-15)
**Context**: Auditoria, compliance (LGPD/GDPR) e debugging requerem rastreabilidade de cada evento
**Decision**: Gerar CorrelationId na entrada (Event Receiver) e propagar através de toda cadeia

### Alternativas Consideradas
1. Sem rastreamento
2. TraceId implícito do .NET
3. Múltiplos IDs (não consolidado)
4. Log-based search

### Decisão e Racional
CorrelationId único porque:
- ✅ Rastreabilidade ponta-a-ponta (recebimento → decisão)
- ✅ Compliance: auditoria completa
- ✅ Debugging: busca rápida em logs
- ✅ Observabilidade: correlação de spans em Jaeger/Zipkin

### Consequências
- ✅ Auditoria melhorada
- ✅ Debugging facilitado
- ❌ Overhead mínimo de propagação

### Implementação
```
Event Receiver:
  CorrelationId = UUID gerado
    ↓
Domain Event: CorrelationId included
    ↓
Application Log: { "correlationId": "...", ... }
    ↓
OpenTelemetry Span: span.SetAttribute("correlation_id", "...")
    ↓
HTTP Header: X-Correlation-ID response header
```

### Rastreamento de Um Evento
```json
{
  "correlationId": "550e8400-e29b-41d4-a716-446655440000",
  "logs": [
    {"ts": "10:30:00", "event": "EventReceived", "service": "EventReceiver"},
    {"ts": "10:30:01", "event": "AnalysisStarted", "service": "SkillOrchestrator"},
    {"ts": "10:30:02", "event": "AnalysisCompleted", "service": "SkillOrchestrator"},
    {"ts": "10:30:03", "event": "EventPersisted", "service": "Persistence"},
    {"ts": "10:32:05", "event": "ActionRecorded", "service": "API"}
  ]
}
```

---

## ADR-007: Repository Pattern com Unit of Work para Persistência

**Status**: ACCEPTED (2024-01-15)
**Context**: Independência de banco de dados (SQLite → PostgreSQL → SQL Server)
**Decision**: Implementar Repository Pattern com Unit of Work abstração

### Alternativas Consideradas
1. Direct EF Core na aplicação
2. Apenas LINQ queries
3. Dapper (low-level)
4. Custom data access

### Decisão e Racional
Repository + Unit of Work porque:
- ✅ Domain não conhece EF Core
- ✅ Trocar banco sem mudança de domínio
- ✅ Testabilidade (mock repositories)
- ✅ Transaction boundaries claros

### Consequências
- ✅ Independência de banco
- ✅ Testes rápidos (in-memory repos)
- ❌ Abstração adicional
- ❌ Query performance (vs direto EF)

### Implementação
```csharp
public interface IEventRepository
{
    Task<Event> GetByIdAsync(EventId id);
    Task SaveAsync(Event evt);
}

public interface IUnitOfWork
{
    IEventRepository Events { get; }
    IInferenceRunRepository InferenceRuns { get; }
    Task CommitAsync();
}
```

---

## ADR-008: OpenTelemetry para Observabilidade Distribuída

**Status**: ACCEPTED (2024-01-15)
**Context**: Sistema distribuído (RabbitMQ, múltiplos workers) requer observabilidade ponta-a-ponta
**Decision**: Integrar OpenTelemetry para traces, metrics e logging estruturado

### Alternativas Consideradas
1. Apenas logging (insuficiente)
2. Application Insights (vendor lock)
3. DataDog (custo alto)
4. Jaeger open-source (self-hosted)

### Decisão e Racional
OpenTelemetry + Jaeger porque:
- ✅ Open standard (não vendor lock)
- ✅ Traces distribuídos
- ✅ Métricas Prometheus
- ✅ Logging estruturado
- ✅ Jaeger pode rodar local (Docker)

### Consequências
- ✅ Visibilidade completa
- ✅ Debugging facilitado
- ❌ Overhead telemetria (~5-10%)
- ❌ Storage de traces (Jaeger DB cresce)

### Implementação
```csharp
var tracerProvider = new TracerProviderBuilder()
    .AddAspNetCoreInstrumentation()
    .AddOtlpExporter(options => options.Endpoint = new Uri("http://jaeger:4317"))
    .Build();
```

---

## ADR-009: JWT + RBAC para Segurança da API

**Status**: ACCEPTED (2024-01-15)
**Context**: API requer autenticação e autorização baseada em papéis
**Decision**: JWT para autenticação, RBAC (Role-Based Access Control) para autorização

### Alternativas Consideradas
1. Basic Auth (inseguro)
2. OAuth2 (overhead)
3. API Keys (sem fine-grained control)
4. mTLS (complex)

### Decisão e Racional
JWT + RBAC porque:
- ✅ JWT stateless (escalável)
- ✅ RBAC simples (Operator, Admin, Analyst)
- ✅ Seguro (assinatura HMAC/RSA)
- ✅ Refresh token para long-lived sessions

### Consequências
- ✅ Segurança padrão
- ✅ Sem estado no servidor
- ❌ Revocation é lenta (até expiração)
- ❌ Token size (se muitos claims)

### Implementação
```csharp
[Authorize(Roles = "Operator")]
[HttpPost("api/v1/events/{eventId}/action")]
public async Task RecordAction(EventId eventId, [FromBody] ActionRequest action) { }
```

---

## ADR-010: Versionamento de API (v1) desde Início

**Status**: ACCEPTED (2024-01-15)
**Context**: API evoluirá, precisa compatibilidade
**Decision**: URL versioning (/api/v1/) desde início

### Alternativas Consideradas
1. Sem versionamento
2. Header versioning
3. Query param versioning

### Decisão e Racional
URL versioning porque:
- ✅ Explícito (fácil ver versão na URL)
- ✅ Cache-friendly (CDN distingue versões)
- ✅ Padrão REST

### Consequências
- ✅ Compatibilidade para sempre
- ✅ Múltiplas versões rodando
- ❌ URL mais longa

---

## ADR-011: SQLite como Banco Inicial (Fase 1)

**Status**: ACCEPTED (2024-01-15)
**Context**: MVP requer simplicidade, nenhum infrastructure operacional
**Decision**: SQLite como banco de dados para Fase 1

### Alternativas Consideradas
1. PostgreSQL (overkill para MVP)
2. MongoDB (sem need para NoSQL)
3. H2 (outro SQLite)
4. In-memory (perda de dados)

### Decisão e Racional
SQLite porque:
- ✅ Zero infrastructure (arquivo local)
- ✅ Suficiente para MVP
- ✅ Suporta transações ACID
- ✅ Fácil backup (copiar arquivo)

### Consequências
- ✅ MVP rápido
- ✅ Nenhuma dependência operacional
- ❌ Não escalável para múltiplas instâncias
- ❌ Upgrade para PostgreSQL necessário em Fase 2

### Roadmap
```
Fase 1 (MVP): SQLite
Fase 2: PostgreSQL (múltiplas instâncias)
Fase 3: MongoDB (escala horizontal)
```

### Implementação
Via Repository Pattern, Switch é trivial:
```csharp
// appsettings.json Fase 1
Database:
  Type: SQLite
  ConnectionString: Data Source=sentinela.db

// appsettings.json Fase 2
Database:
  Type: PostgreSQL
  ConnectionString: Host=postgres;Database=sentinela;...
```

---

## ADR-012: Prompts Versionados no Código (v1)

**Status**: ACCEPTED (2024-01-15)
**Context**: Prompts são artefatos críticos, requerem versionamento
**Decision**: Prompts armazenados em database com versionamento SemVer

### Alternativas Consideradas
1. Hardcoded em código
2. Arquivos de configuração
3. Database (escolhido)
4. Embedding model (premature)

### Decisão e Racional
Database com versionamento porque:
- ✅ Histórico completo de prompt
- ✅ Ativar/desativar sem redeploy
- ✅ Rastreabilidade de qual prompt foi usado
- ✅ Rollback automático se problema

### Consequências
- ✅ Agilidade (trocar prompt sem deploy)
- ✅ Auditoria (qual prompt em cada inferência)
- ❌ Não testável com versão control tradicional
- ❌ Sincronização dev/staging/prod manual

### Implementação
```
PromptTable:
├─ id: UUID
├─ name: string (ex: "PerimeterAnalysis")
├─ version: string (SemVer, ex: "1.0.0")
├─ content: string (template)
├─ createdAt: DateTime
├─ createdBy: UserId
├─ isActive: bool
└─ metrics: JSON (precision, recall, etc)
```

---

## ADR-013: Docker + Docker Compose para Local Dev & Deployment

**Status**: ACCEPTED (2024-01-15)
**Context**: Desenvolvimento local precisa replicar produção (Ollama, RabbitMQ, Database)
**Decision**: Docker Compose para orquestração

### Alternativas Consideradas
1. Manual setup (não reproduzível)
2. Kubernetes (overkill para MVP)
3. Docker Compose (escolhido)
4. Vagrant VMs

### Decisão e Racional
Docker Compose porque:
- ✅ Reproduzível (mesma configuração dev/prod)
- ✅ Fácil (docker-compose up)
- ✅ Single file de configuração
- ✅ Escalável para Kubernetes depois

### Consequências
- ✅ Onboarding rápido
- ✅ Nenhuma divergência dev/prod
- ❌ Docker necessário
- ❌ Upgrade para Kubernetes manual

---

## Resumo de Decisões Arquiteturais

| ADR | Título | Status | Impacto |
|-----|--------|--------|---------|
| ADR-001 | DDD | ACCEPTED | Alto (design geral) |
| ADR-002 | Event-Driven | ACCEPTED | Alto (comunicação) |
| ADR-003 | Agnósticismo IA | ACCEPTED | Alto (flexibility) |
| ADR-004 | Skills Compostas | ACCEPTED | Médio (extensibilidade) |
| ADR-005 | Harness | ACCEPTED | Médio (experimentação) |
| ADR-006 | CorrelationId | ACCEPTED | Médio (observabilidade) |
| ADR-007 | Repository Pattern | ACCEPTED | Médio (testabilidade) |
| ADR-008 | OpenTelemetry | ACCEPTED | Médio (observabilidade) |
| ADR-009 | JWT + RBAC | ACCEPTED | Médio (segurança) |
| ADR-010 | API v1 | ACCEPTED | Baixo (compatibilidade) |
| ADR-011 | SQLite Fase 1 | ACCEPTED | Médio (operabilidade) |
| ADR-012 | Prompts Versionados | ACCEPTED | Médio (auditoria) |
| ADR-013 | Docker Compose | ACCEPTED | Médio (desenvolvimento) |

---

## Template para Novos ADRs

```markdown
## ADR-XXX: [Title]

**Status**: PROPOSED | ACCEPTED | REJECTED | SUPERSEDED BY ADR-YYY
**Date**: YYYY-MM-DD
**Author**: [Name]

### Context
[Contexto do problema/decisão]

### Decision
[Decisão tomada]

### Rationale
[Por que essa decisão]

### Alternatives Considered
1. [Alternativa 1]
2. [Alternativa 2]

### Consequences
- ✅ [Consequência positiva]
- ❌ [Consequência negativa]

### Implementation
[Como será implementado]
```
