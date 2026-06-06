# 🏗️ Arquitetura SentinelaOps

Este documento descreve a arquitetura de alto nível, padrões e decisões do projeto.

---

## 📐 Visão Geral

```
┌─────────────────────────────────────────────────────────────┐
│                     MONITORING SYSTEM                       │
│  (ONVIF, RTSP, APIs, Custom Events)                         │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                   API / REST / WebSocket                    │
│  (Ingest events, subscribe to alerts)                       │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│              MESSAGING (RabbitMQ/Kafka)                     │
│  (Event distribution across system)                         │
└──┬────────────────┬────────────────┬─────────────────┬──────┘
   │                │                │                 │
   ▼                ▼                ▼                 ▼
┌─────────┐   ┌──────────┐   ┌────────────┐   ┌─────────────┐
│ WORKER  │   │ HARNESS  │   │ API        │   │ OTHER WORKERS
│ SERVICE │   │ SERVICE  │   │ (Polling)  │   │             
└────┬────┘   └────┬─────┘   └────┬───────┘   └─────────────┘
     │              │              │
     ▼              ▼              ▼
┌─────────────────────────────────────────────────────────────┐
│           APPLICATION LAYER (Commands/Queries)             │
│  (Orchestration, Business Logic)                            │
└────────────────────┬────────────────────────────────────────┘
                     │
     ┌───────────────┼───────────────┐
     ▼               ▼               ▼
┌──────────┐   ┌──────────┐   ┌────────────┐
│  SKILLS  │   │ INFERENCE│   │  DOMAIN    │
│(Analysis)│   │ PROVIDER │   │   EVENTS   │
└──────────┘   └──────────┘   └────────────┘
     │               │               │
     └───────────────┼───────────────┘
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                 DOMAIN LAYER (DDD)                          │
│  (Aggregates, Value Objects, Domain Events)                │
└────────────────────┬────────────────────────────────────────┘
                     │
     ┌───────────────┼───────────────┐
     ▼               ▼               ▼
┌──────────┐   ┌──────────┐   ┌────────────┐
│ PERSISTENCE
│ (SQL)     │   │  MESSAGING
│ (Events)  │   │ INFERENCE
│           │   │ (AI Models) 
└──────────┘   └──────────┘   └────────────┘
```

---

## 🎯 Pilares Arquiteturais

### 1️⃣ Domain-Driven Design (DDD)

**Foco**: Modelo de domínio como centro da arquitetura

```
Value Objects       → Imutáveis, sem ID (Confidence, EventId, Classification)
Aggregate Roots     → Entidade raiz com ciclo de vida (MonitoringEvent, PromptVersion)
Domain Events       → Fatos que ocorreram (EventReceived, AnalysisCompleted)
Repositories        → Abstração de persistência (IMonitoringEventRepository)
Bounded Contexts    → Domínios separados (Skills, Inference, Events)
```

**Localização**:
```
src/SentinelaOps.Domain/Core/
├── EventId.cs                          ← Value Object
├── CorrelationId.cs                    ← Value Object
├── MonitoringEvent.cs                  ← Aggregate Root
├── DomainEvents.cs                     ← Domain Events
└── IMonitoringEventRepository.cs       ← Repository Interface
```

---

### 2️⃣ Clean Architecture (Layered)

**Estrutura em Camadas**:

```
┌──────────────────────────────────┐
│   PRESENTATION LAYER             │  ← API / UI / CLI
│   (Controllers, ViewModels)      │
└─────────────┬────────────────────┘
              │
┌─────────────▼────────────────────┐
│   APPLICATION LAYER              │  ← Commands, Handlers, Services
│   (Use Cases, Business Logic)    │
└─────────────┬────────────────────┘
              │
┌─────────────▼────────────────────┐
│   DOMAIN LAYER                   │  ← Entities, Value Objects, Aggregates
│   (Business Rules, DDD)          │
└─────────────┬────────────────────┘
              │
┌─────────────▼────────────────────┐
│   INFRASTRUCTURE LAYER           │  ← Databases, APIs, Messaging
│   (Implementation Details)       │
└──────────────────────────────────┘
```

**Regra de Dependência**: Código em camadas internas ❌ NUNCA depende de camadas externas

---

### 3️⃣ Skills-Based Architecture

Análise desacoplada em skills independentes:

```
┌─────────────────────────────────────┐
│   SKILL ORCHESTRATOR                │  ← Coordena execução
├─────────────────────────────────────┤
│  Perimeter │ Intrusion │ Motion │... │  ← 8 Skills paralelos
├─────────────────────────────────────┤
│  Inference Provider (Ollama/OpenAI) │  ← Abstração de IA
├─────────────────────────────────────┤
│  Domain: MonitoringEvent             │  ← Resultado: Classification
└─────────────────────────────────────┘
```

**Localização**:
```
src/SentinelaOps.Skills.Abstractions/
├── ISkill.cs
├── SkillRequest.cs
├── SkillResult.cs
└── SkillRegistry.cs

src/SentinelaOps.Skills.Perimeter/
├── PerimeterAnalysisSkill.cs
└── PerimeterAnalysisTests.cs
```

---

### 4️⃣ Event-Driven Architecture

Comunicação via eventos de domínio:

```
MonitoringEvent Created
        │
        ▼
EventReceivedDomainEvent
        │
        ▼
Worker processa análise
        │
        ▼
AnalysisCompletedDomainEvent
        │
        ▼
API notifica operator (WebSocket/SignalR)
```

**Localização**:
```
src/SentinelaOps.Domain/Core/DomainEvents.cs
├── EventReceivedDomainEvent
├── AnalysisStartedDomainEvent
└── AnalysisCompletedDomainEvent
```

---

### 5️⃣ Specification-Driven Development (SDD)

**Documento PRIMEIRO, código DEPOIS**:

```
docs/                               ← Especificações
├── domain-model/DOMAIN_MODEL.md   ← Modelo de domínio
├── event-storming/                ← Event storming
├── context-map/                   ← Bounded contexts
├── skills/                        ← Specs de cada skill
└── architecture/                  ← Decisões arquiteturais

Implementação segue specs ao pé da letra
```

---

## 🔀 Fluxo de Dados

### Processamento de Evento

```
1. INGESTION
   ├─ REST API recebe MonitoringEvent
   ├─ Valida schema
   └─ Publica em RabbitMQ

2. QUEUING
   ├─ Evento aguarda processamento
   └─ Worker dequeue event

3. ANALYSIS (Paralelo - 8 Skills)
   ├─ PerimeterAnalysisSkill → Classification
   ├─ IntrusionAnalysisSkill → Classification
   ├─ MotionAnalysisSkill    → Classification
   └─ ... (5 skills mais)

4. AGGREGATION
   ├─ Combina resultados
   ├─ Calcula confiança final
   └─ Gera justificativa

5. PERSISTENCE
   ├─ Salva resultado em BD
   ├─ Publica AnalysisCompletedDomainEvent
   └─ Publica em RabbitMQ

6. NOTIFICATION
   ├─ API notifica via WebSocket
   └─ Dashboard atualiza em tempo real
```

---

## 🧠 As 8 Skills de Análise

Cada skill é uma **IA especializada independente** que executa em paralelo, analisando o evento sob uma perspectiva diferente:

### 1️⃣ **Perimeter Analysis** 🔵
- Detecta intrusões e violações de zona de proteção
- Analisa comportamentos suspeitos em áreas restritas
- Verifica cruzamentos de fronteira
- **Entrada**: Imagem, metadata de zona, histórico
- **Saída**: `{isPerimeterViolation, violationType, confidence, riskLevel}`
- **Local**: `.github/skills/perimeter-analysis/SKILL.md`

### 2️⃣ **Intrusion Analysis** 🔓
- Identifica arrombamento, escalada, destruição de obstáculos
- Detecta comportamento evasivo
- Análise de ferramentas/armas
- **Entrada**: Imagem, contexto de segurança
- **Saída**: `{isIntrusion, intrusionType, confidence, severity}`
- **Local**: `.github/skills/intrusion-analysis/SKILL.md`

### 3️⃣ **Motion Analysis** 🚶
- Detecta padrões anormais de movimento
- Identifica rotas incomuns
- Análise de velocidade/aceleração
- **Entrada**: Sequência de frames, trajetória
- **Saída**: `{isAnomalous, anomalyType, confidence, pattern}`
- **Local**: `.github/skills/motion-analysis/SKILL.md`

### 4️⃣ **Human Activity Analysis** 👤
- Classifica atividades humanas (trabalho, locomoção, descanso)
- Identifica comportamento suspeito
- Análise de interações humanas
- **Entrada**: Imagem, contexto temporal
- **Saída**: `{activityType, isSuspicious, confidence, behavior}`
- **Local**: `.github/skills/human-activity-analysis/SKILL.md`

### 5️⃣ **Vehicle Analysis** 🚗
- Detecção e tipo de veículo (carro, moto, caminhão, etc)
- Extração de placa
- Análise de velocidade e comportamento
- Integridade estrutural (danos, modificações)
- **Entrada**: Imagem, contexto de via
- **Saída**: `{vehicleType, plate, speed, behavior, integrity}`
- **Local**: `.github/skills/vehicle-analysis/SKILL.md`

### 6️⃣ **False Positive Analysis** 🎯
- Reduz falsos positivos através de análise contextual
- Verifica padrões históricos
- Validação de confiabilidade
- **Entrada**: Resultado de outras skills, histórico
- **Saída**: `{isFalsePositive, confidence, reason}`
- **Local**: `.github/skills/false-positive-analysis/SKILL.md`

### 7️⃣ **Severity Classification** 📊
- Classifica severidade e urgência do incidente
- Matriz de risco (probabilidade × impacto)
- Priorização de resposta operacional
- **Entrada**: Resultados de todas skills, contexto
- **Saída**: `{severity, urgency, priority, riskScore}`
- **Local**: `.github/skills/severity-classification/SKILL.md`

### 8️⃣ **Incident Summary** 📋
- Consolida análises em relatório executivo estruturado
- Classificação final para operador
- Evidências e justificativa
- Próximos passos recomendados
- **Entrada**: Resultados de todas skills
- **Saída**: `{classification, confidence, justification, evidence, nextSteps}`
- **Local**: `.github/skills/incident-summary/SKILL.md`

**Como as Skills Funcionam**:

```csharp
// SkillOrchestrator executa todos em paralelo
var tasks = new[]
{
    perimeterSkill.AnalyzeAsync(request),
    intrusionSkill.AnalyzeAsync(request),
    motionSkill.AnalyzeAsync(request),
    humanActivitySkill.AnalyzeAsync(request),
    vehicleSkill.AnalyzeAsync(request),
    falsePositiveSkill.AnalyzeAsync(previousResults),
    severitySkill.AnalyzeAsync(allResults),
    summarySkill.AnalyzeAsync(finalResults)
};

await Task.WhenAll(tasks);

// Agregação final: Severity + Summary = Classification Final
```

**Benefícios da Abordagem**:
- ✅ **Paralelo**: Todas skills executam simultaneamente (5-10x mais rápido)
- ✅ **Desacoplado**: Novos skills sem alterar existentes (Open/Closed Principle)
- ✅ **Testável**: Cada skill tem testes unitários isolados
- ✅ **Escalável**: Trocar modelo IA de uma skill sem afetar outras
- ✅ **Rastreável**: Cada skill contribui explicitamente para decisão final

**Onde Estão Definidas**:
```
.github/skills/
├── perimeter-analysis/SKILL.md
├── intrusion-analysis/SKILL.md
├── motion-analysis/SKILL.md
├── human-activity-analysis/SKILL.md
├── vehicle-analysis/SKILL.md
├── false-positive-analysis/SKILL.md
├── severity-classification/SKILL.md
├── incident-summary/SKILL.md
└── SKILLS_STRATEGY.md              ← Visão geral
```

---

## 💾 Padrões de Persistência

### Repository Pattern

```csharp
// Abstração
public interface IMonitoringEventRepository
{
    Task AddAsync(MonitoringEvent @event, CancellationToken cancellationToken);
    Task<MonitoringEvent?> GetByIdAsync(EventId id, CancellationToken cancellationToken);
    // ...
}

// Implementação (Infrastructure)
public class MonitoringEventRepository : IMonitoringEventRepository
{
    // Usa SQL Server, SQLite, ou outro DB
}
```

**Localizações**:
```
src/SentinelaOps.Domain/Core/
└── IMonitoringEventRepository.cs     ← Contrato

src/SentinelaOps.Infrastructure/Persistence/
└── MonitoringEventRepository.cs      ← Implementação
```

---

## 🤖 Abstração de IA

O domínio ❌ **NUNCA** conhece Ollama, OpenAI, etc

```csharp
// Domain não sabe que é Ollama
var request = new InferenceRequest { Prompt = "..." };
var result = await inferenceProvider.InferenceAsync(request);
// result: Classification, Confidence, Justification
```

**Localização**:
```
src/SentinelaOps.Application/
└── Inference/
    ├── IInferenceProvider.cs         ← Contrato
    ├── InferenceRequest.cs
    └── InferenceResult.cs

src/SentinelaOps.Infrastructure/Inference/
├── OllamaInferenceProvider.cs        ← Ollama
└── OpenAiInferenceProvider.cs        ← OpenAI (futuro)
```

---

## 🧪 Estratégia de Testes

| Tipo | Localização | Cobertura |
|------|------------|-----------|
| **Unit** | `Domain.Tests/` | 100% logica pura |
| **Integration** | `Integration.Tests/` | API + DB + Messaging |
| **E2E** | `E2E.Tests/` | Fluxo completo com containers |
| **Architecture** | `Architecture.Tests/` | Validar dependências |
| **Harness** | `Harness.Tests/` | Comparação de modelos |

**Exemplo Unit Test**:
```csharp
[Fact]
public void Create_WithValidConfidence_Succeeds()
{
    var confidence = Confidence.Create(0.85);
    Assert.Equal(0.85, confidence.Value);
}
```

---

## 📦 Dependências de Projeto

```
SentinelaOps.Domain
    └─ (nenhuma externa!)
    
SentinelaOps.Application
    └─ SentinelaOps.Domain
    
SentinelaOps.Infrastructure
    ├─ SentinelaOps.Application
    ├─ EntityFrameworkCore (SQL)
    └─ RabbitMQ.Client
    
SentinelaOps.Api
    ├─ SentinelaOps.Application
    ├─ SentinelaOps.Infrastructure
    └─ ASP.NET Core
    
SentinelaOps.Worker
    ├─ SentinelaOps.Application
    ├─ SentinelaOps.Infrastructure
    └─ Microsoft.Extensions.Hosting
```

---

## 🎓 Decisões Arquiteturais (ADRs)

**Registradas em**: `docs/adr/`

**Exemplos**:
- ADR-001: Por que usar DDD em vez de CRUD
- ADR-002: Por que Skills são independentes
- ADR-003: Por que Domain não conhece Infrastructure
- ADR-004: Por que usar Event Sourcing

---

## 🚀 Escalabilidade Futura

### Horizontal Scaling
```
┌──────────────┐      ┌──────────────┐
│   Worker 1   │      │   Worker 2   │
└──────┬───────┘      └────┬─────────┘
       │                   │
       └─────────┬─────────┘
                 │
        ┌────────▼─────────┐
        │   RabbitMQ       │  ← Message Bus
        │  (Load Balance)  │
        └──────────────────┘
```

### Multi-Model Inference
```
┌────────────────────────────┐
│   Inference Provider       │
├────────────────────────────┤
│ Ollama │ OpenAI │ Azure    │
└────────────────────────────┘
```

---

## 📊 Métricas e Observabilidade

```
OpenTelemetry
    ├─ Distributed Tracing (Jaeger)
    ├─ Metrics (Prometheus)
    ├─ Logs (Structured)
    └─ Correlation ID (End-to-end)
```

---

## 🔐 Segurança

- JWT para autenticação
- RBAC para autorização
- Auditoria de todas inferências
- Versionamento de prompts
- Rastreabilidade completa

---

## 📚 Próximos Passos

1. **Etapa 1.4**: PromptVersion + InferenceExecution aggregates
2. **Etapa 1.5**: Application Services + Skills Integration
3. **Etapa 2.0**: Worker Service
4. **Etapa 2.1**: Infrastructure Persistence
5. **Etapa 3.0**: API + WebSocket Notifications

---

**Para contribuir com base nesta arquitetura**, leia [CONTRIBUTING.md](CONTRIBUTING.md)
