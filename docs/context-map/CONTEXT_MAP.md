# Context Map & Bounded Contexts

## Visão Geral

O Context Map mostra como diferentes Bounded Contexts se relacionam e comunicam. O projeto está organizado em 7 Bounded Contexts principais.

---

## Bounded Contexts Identificados

### 1. Monitoring Provider Adapter (Adapter)

**Responsabilidade**: Abstrair sistemas de monitoramento externos (câmeras, sensores)

**Domínio**: Adaptação/Integração

**Entidades Principais**:
- MonitoringEventProvider (interface)
- EventPayload
- MonitoringSystemConfig

**Saídas**:
- Publica: `EventReceived` (Domain Event)

**Linguagem Ubíqua**:
- Event Source, Camera, Sensor, Event Payload, EventType (PerimeterViolation, Intrusion, Motion, Custom)

**Dependências**:
- Event Receiver Context (downstream)

**Exemplos Futuros**:
- ONVIF Provider
- RTSP Provider
- Axis Communications API
- Hikvision API

---

### 2. Event Receiver (Core)

**Responsabilidade**: Receber, validar e enfileirar eventos para análise

**Domínio**: Recebimento de Eventos

**Entidades Principais**:
- MonitoringEvent (Aggregate Root)
- EventId, CorrelationId (Value Objects)
- EventMetadata (Value Object)

**Saídas**:
- Publica: `EventReceived`, `EventValidated`, `EventEnqueued`
- HTTP API: POST `/api/v1/events`

**Linguagem Ubíqua**:
- Event, Validation, Enqueue, CorrelationId, EventId

**Dependências**:
- Messaging Queue (RabbitMQ)
- Skill Orchestrator Context (downstream)
- Persistence Context (para armazenar evento original)

**Responsabilidades**:
- Aceitar POST request
- Validar formato JPEG (magic bytes)
- Validar tamanho imagem (max 5MB)
- Validar campos obrigatórios
- Gerar EventId + CorrelationId
- Enfileirar para processamento
- Retornar 202 Accepted

---

### 3. Skill Orchestrator (Core)

**Responsabilidade**: Orquestrar execução sequencial de Skills

**Domínio**: Orquestração de Análise

**Entidades Principais**:
- SkillPipeline (configuração)
- SkillRegistry (registro de Skills)

**Saídas**:
- Publica: `AnalysisStarted`, `SkillExecutionStarted`, `SkillExecutionCompleted`, `AnalysisCompleted`, `AnalysisFailed`
- Chama: ISkill.ExecuteAsync()

**Linguagem Ubíqua**:
- Skill, Pipeline, Execution, Timeout, Orchestration

**Dependências**:
- Event Receiver (recebe eventos)
- Skills Context (múltiplos skills)
- Inference Provider Context (para executar análise de IA)
- Persistence Context (salvar resultado)

**Responsabilidades**:
- Ler configuração de pipeline
- Executar skills sequencialmente
- Passar output como input para próximo
- Implementar timeout (5s por skill)
- Coletar resultados
- Publicar eventos
- Tratar falhas gracefully

---

### 4. Skills (Core - Multi-module)

Cada Skill é um módulo separado implementando ISkill.

#### 4.1 PerimeterAnalysisSkill
- **Responsabilidade**: Classificar se evento está dentro/fora do perímetro
- **Entrada**: Image, ZoneConfig, Metadata
- **Saída**: Classification (inside|outside|uncertain), Confidence, Evidence

#### 4.2 IntrusionAnalysisSkill
- **Responsabilidade**: Classificar se evento é intrusão
- **Entrada**: Image, HistoricalContext, ZoneConfig
- **Saída**: Classification (intrusion|authorized|suspicious), Confidence

#### 4.3 FalsePositiveAnalysisSkill
- **Responsabilidade**: Analisar probabilidade de falso positivo
- **Entrada**: Resultados de skills anteriores
- **Saída**: ProbabilityFalsePositive (0.0-1.0), Reason

#### 4.4 SeverityClassificationSkill
- **Responsabilidade**: Classificar severidade do evento
- **Entrada**: Análise completa até esse ponto
- **Saída**: SeverityLevel (critical|high|medium|low|informational), Score

#### 4.5 IncidentSummarySkill
- **Responsabilidade**: Gerar resumo operacional
- **Entrada**: Resultado de todas skills anteriores
- **Saída**: Summary (string natural language), RecommendedAction

#### 4.6 Skills Futuras (Fase 2+)
- MotionAnalysisSkill
- HumanActivityAnalysisSkill
- VehicleAnalysisSkill

**Linguagem Ubíqua por Skill**:
- Cada skill tem seu domínio mini com termos específicos

**Dependências**:
- Inference Provider Context (para executar inferência de IA)
- Skill Orchestrator (orquestra execução)

---

### 5. Inference Provider (Core)

**Responsabilidade**: Abstrair modelo de IA (agnóstico)

**Domínio**: Execução de Inferência

**Entidades Principais**:
- InferenceExecution (Aggregate)
- InferenceRequest (Value Object)
- InferenceResult (Value Object)
- ModelConfiguration (Value Object)

**Saídas**:
- Publica: `InferenceRunStarted`, `InferenceExecuted`, `InferenceRunCompleted`, `InferenceRunFailed`
- Implementação: OllamaInferenceProvider (atual)

**Linguagem Ubíqua**:
- Inference, Model, Prompt, Request, Result, Tokens, Latency

**Dependências**:
- Ollama (serviço externo)
- Prompt Context (obter versão ativa de prompt)
- Persistence Context (armazenar resultado de inferência)

**Responsabilidades**:
- Receber InferenceRequest
- Construir prompt final (combinar template + dados)
- Chamar modelo de IA (Ollama)
- Parsear resultado
- Calcular métricas (tokens, latência)
- Tratar timeout/erro
- Publicar eventos

**Implementações Futuras**:
- AzureOpenAIInferenceProvider
- AnthropicInferenceProvider
- MistralInferenceProvider
- LocalLlamaInferenceProvider

---

### 6. Inference Harness (Core)

**Responsabilidade**: Experimentação, benchmark e comparação de modelos/prompts

**Domínio**: Experimentação de IA

**Entidades Principais**:
- BenchmarkRun (Aggregate)
- Prompt (Aggregate) - compartilhado com Inference Provider
- PromptVersion (Aggregate) - compartilhado
- EvaluationDataset (Value Object)
- ModelRegistry (registry)

**Saídas**:
- Publica: `BenchmarkStarted`, `BenchmarkCompleted`, `ComparisonGenerated`
- Endpoints: `/harness/benchmarks`, `/harness/comparisons`, `/harness/prompts`

**Linguagem Ubíqua**:
- Benchmark, Comparison, Model, Prompt Version, Metrics, Dataset, Evaluation

**Dependências**:
- Inference Provider Context (executar múltiplas inferências)
- Persistence Context (armazenar resultados)
- Prompt Context (obter versões de prompt)

**Responsabilidades**:
- Registrar múltiplos modelos
- Executar benchmark em paralelo
- Coletar métricas de cada modelo
- Gerar relatório comparativo
- Versionar prompts
- Rastrear desempenho por versão
- Exportar resultados

---

### 7. Persistence (Support/Infrastructure)

**Responsabilidade**: Abstrair armazenamento de dados

**Domínio**: Persistência

**Entidades Principais**:
- Repository Interfaces (definidas em Domain)
- Database Context (EF Core)
- Schema definitions

**Saídas**:
- Publica: `EventPersisted`, `InferenceResultPersisted`, `ActionRecorded`, `AuditLogAppended`
- Implements: IEventRepository, IInferenceRunRepository, IAuditLogRepository

**Linguagem Ubíqua**:
- Repository, Query, Persistence, Transaction

**Dependências**:
- SQLite (implementação atual)
- Domain Aggregates (que precisa persistir)

**Responsabilidades**:
- Implementar Repository Pattern
- Gerenciar transações
- Aplicar migrations
- Implementar Unit of Work (se necessário)
- Criar índices para performance
- Suportar múltiplos bancos (SQLite → PostgreSQL → SQL Server)

**Implementações Futuras**:
- PostgreSQL Repository
- SQL Server Repository
- MongoDB Repository (para scale horizontal)

---

### 8. API (Support)

**Responsabilidade**: Exposição HTTP REST do sistema

**Domínio**: API Gateway

**Endpoints Principais**:
- POST `/api/v1/events` → Event Receiver
- GET `/api/v1/events/{eventId}` → Persistence Query
- POST `/api/v1/events/{eventId}/action` → Record Action
- GET `/api/v1/health` → Health Check
- WebSocket `/ws/events` → Real-time notifications

**Saídas**:
- HTTP REST API
- OpenAPI 3.0 specification

**Linguagem Ubíqua**:
- Endpoint, Resource, Request, Response, Status Code

**Dependências**:
- Application Services (orquestração)
- Domain Models (retornar DTOs)

---

## Context Map Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                      SENTINELA OPS - CONTEXT MAP                        │
└─────────────────────────────────────────────────────────────────────────┘

┌────────────────────┐
│  Monitoring        │  (Adapter Context)
│  Provider Adapter  │─────────┐
│  (ONVIF, RTSP)     │         │ publishes EventReceived
└────────────────────┘         │
                               ↓
                        ┌──────────────────────┐
                        │   Event Receiver     │ (Core Context)
                        │   • Validate         │
                        │   • Enqueue          │────┐
                        └──────────────────────┘    │
                                                    │ publishes AnalysisStarted
                                                    ↓
┌──────────────────────────────────────────────────────────────────┐
│                    Skill Orchestrator (Core)                     │ ←── orchestrates
│ • PerimeterAnalysisSkill                                         │
│ • IntrusionAnalysisSkill                                         │
│ • FalsePositiveAnalysisSkill                                     │
│ • SeverityClassificationSkill                                    │
│ • IncidentSummarySkill                                           │
└──────────────────────────────────────────────────────────────────┘
         │                                              │
         │ calls ExecuteAsync()                         │ publishes AnalysisCompleted
         ↓                                              ↓
┌─────────────────────────────┐              ┌──────────────────────┐
│  Inference Provider (Core)  │              │  Persistence (Supp)  │
│  • OllamaProvider           │              │  • Repositories      │
│  • (Future: Azure OpenAI)   │              │  • Unit of Work      │
│                             │              │  • Audit Log         │
│ calls: Ollama Inference     │              │  • Queries           │
└─────────────────────────────┘              └──────────────────────┘
         ↑
         │ uses Prompt
         │
    ┌────────────────────────────┐
    │ Inference Harness (Core)   │
    │ • Model Comparison         │
    │ • Prompt Versioning        │
    │ • Benchmark Execution      │
    │ • Results Export           │
    └────────────────────────────┘
         │
         │ publishes ComparisonGenerated
         ↓
    ┌────────────────────────────┐
    │      API (Support)         │
    │ • POST /api/events         │
    │ • GET /api/events/{id}     │
    │ • WebSocket notifications  │
    │ • OpenAPI 3.0 docs         │
    └────────────────────────────┘
```

---

## Relacionamentos Entre Contexts

### Event Receiver → Skill Orchestrator
**Tipo**: Publish-Subscribe
**Mecanismo**: Domain Events (EventEnqueued) + RabbitMQ
**Propriedade**: Assincronia (Event Receiver não espera)
**Descrição**: Event Receiver publica EventEnqueued, Skill Orchestrator subscreve e processa

---

### Skill Orchestrator → Inference Provider
**Tipo**: Synchronous Call
**Mecanismo**: ISkill.ExecuteAsync() → chama IInferenceProvider
**Propriedade**: Sincronismo (Skill aguarda resposta)
**Descrição**: Cada Skill chama InferenceProvider para executar análise de IA

---

### Inference Provider ← Prompt Context
**Tipo**: Query
**Mecanismo**: IPromptRepository.GetActiveVersionAsync()
**Propriedade**: Pull (Inference Provider busca)
**Descrição**: InferenceProvider obtém versão ativa de prompt antes de inferir

---

### Skills → Inference Provider
**Tipo**: Hierarchical
**Mecanismo**: Inheritance/Interface (ISkill → IInferenceProvider)
**Descrição**: Todas Skills usam mesma interface de Inference Provider (agnóstico)

---

### Skill Orchestrator → Persistence
**Tipo**: Publish-Subscribe
**Mecanismo**: Domain Events (AnalysisCompleted) + async handler
**Propriedade**: Assincronia, eventual consistency
**Descrição**: Skill Orchestrator publica AnalysisCompleted, Persistence subscreve e salva

---

### Inference Harness → Inference Provider
**Tipo**: Synchronous Call (paralelo)
**Mecanismo**: Execute múltiplos InferenceRunAsync() em paralelo
**Propriedade**: Sincronismo com paralelismo
**Descrição**: Harness executa múltiplos modelos em paralelo para comparação

---

### Inference Harness → Prompt
**Tipo**: Query & Mutation
**Mecanismo**: IPromptRepository.GetAsync(), CreateVersionAsync(), ActivateAsync()
**Propriedade**: Full control
**Descrição**: Harness versiona prompts e ativa novas versões

---

### API → Application Services
**Tipo**: Synchronous Call
**Mecanismo**: Dependency Injection
**Propriedade**: Sincronismo
**Descrição**: Controllers chamam Application Service Handlers

---

### API → Persistence
**Tipo**: Query
**Mecanismo**: IEventRepository.GetByIdAsync()
**Propriedade**: Sincronismo (read)
**Descrição**: API consulta resultados persistidos

---

## Anti-Corruption Layers (ACL)

### Monitoring Provider Adapter ↔ External Systems
```
External System (proprietário)
    ↓
ACL: MonitoringEventProvider (interface)
    ↓
Event Receiver (domínio)
```
Garante que sistema externo não afeta domínio interno.

### Ollama ↔ Inference Provider
```
Ollama (API REST)
    ↓
ACL: OllamaInferenceProvider (adapter)
    ↓
IInferenceProvider (interface de domínio)
```
Garante agnósticismo de modelo.

---

## Shared Kernel

### Prompt Context
**Compartilhado por**:
- Inference Provider (lê versão ativa)
- Inference Harness (versiona e testa)

**Linguagem Ubíqua Compartilhada**:
- Prompt, Version, SemVer, Active, Metrics

**Agregates Compartilhadas**:
- Prompt
- PromptVersion
- PromptMetrics

---

## Padrão de Comunicação

### Síncrona (Skill → Inference)
```
Skill.Execute()
  ↓
calls InferenceProvider.InferAsync()
  ↓
blocks until result
  ↓
returns SkillResult
```

### Assíncrona (Orchestrator → Persistence)
```
SkillOrchestrator.Complete()
  ↓
publishes AnalysisCompleted event
  ↓
returns immediately
  ↓
PersistenceEventHandler.Handle()
  ↓ (later, async)
persists result
```

---

## Escalabilidade de Contexts

### Horizontal Scaling
```
Multiple instances of:
├─ Event Receiver (RabbitMQ distribui)
├─ Skill Orchestrator (RabbitMQ distribui)
├─ Inference Provider (sem estado, stateless)
└─ Persistence (conexão compartilhada ao banco)
```

### Vertical Scaling
```
Inference Harness:
├─ Executa Skills paralelos
└─ Executa múltiplos modelos paralelos
```

---

## Matriz de Dependências

| Context | Depende De | Dependências |
|---|---|---|
| Monitoring Provider | External Systems | (none) |
| Event Receiver | RabbitMQ, Persistence | Monitoring Provider |
| Skill Orchestrator | Event Receiver, Skills, Inference | Persistence, Messaging |
| Skills | Inference Provider | Skill Orchestrator |
| Inference Provider | Ollama, Prompt | Skills |
| Persistence | Database | Event Receiver, Skill Orchestrator, Harness |
| Harness | Inference Provider, Prompt, Persistence | (none) |
| API | Application Services, Persistence | All contexts |

---

## Roadmap de Evolução de Contexts

### Fase 1 (MVP)
- ✅ Monitoring Provider Adapter (básico)
- ✅ Event Receiver
- ✅ Skill Orchestrator
- ✅ 5 Skills iniciais
- ✅ Inference Provider (Ollama only)
- ✅ Persistence (SQLite)
- ✅ Harness (comparação modelos)
- ✅ API REST

### Fase 2
- ➕ Multiple Inference Providers (Azure OpenAI, Anthropic)
- ➕ Multiple Persistence Implementations (PostgreSQL, SQL Server)
- ➕ Advanced Harness (synthetic dataset generation)
- ➕ Native ONVIF/RTSP Support
- ➕ Video Feed Integration

### Fase 3+
- ➕ Mobile API
- ➕ Dashboards (Grafana integration)
- ➕ Machine Learning for Confidence Calibration
- ➕ Real-time Alerting System
- ➕ Community Marketplace for Skills
