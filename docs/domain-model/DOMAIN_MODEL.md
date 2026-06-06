# Domain Model

## Overview

O Domain Model captura o entendimento compartilhado do domínio de videomonitoramento com apoio de IA.

Segue estrutura DDD:
- **Ubiquitous Language**: Linguagem compartilhada entre desenvolvedores, domain experts e operadores
- **Entities**: Objetos com identidade única (Event, InferenceRun, Prompt)
- **Value Objects**: Sem identidade, imutáveis (Confidence, Classification, EventMetadata)
- **Aggregates**: Clusters de entidades com raiz (EventAggregate, PromptVersionAggregate)
- **Domain Events**: Fatos importantes (EventReceived, AnalysisCompleted, ActionRecorded)
- **Repositories**: Interface de persistência por Aggregate
- **Domain Services**: Lógica que não cabe em uma entidade (SkillOrchestrator, InferenceService)

---

## Linguagem Ubíqua (Ubiquitous Language)

### Termos Críticos

| Termo | Significado | Contexto |
|---|---|---|
| **Event** | Detecção de algo pelo sistema de monitoramento | Domínio |
| **Classification** | Resultado de análise: Valid, PossibleFalsePositive, Suspicious, HumanReviewRequired, Inconclusive | Domínio |
| **Confidence** | Score 0.0-1.0 indicando confiança da análise | Domínio |
| **Justification** | Razão textual para classificação | Domínio |
| **Evidence** | Dados que suportam classificação (ex: "posição humana detectada em 0.7") | Domínio |
| **Skill** | Unidade independente de análise (ex: PerimeterAnalysisSkill) | Domínio |
| **Pipeline** | Sequência ordenada de Skills | Domínio |
| **Prompt** | Instrução de IA para executar análise | Domínio |
| **Inference** | Execução de modelo IA com prompt e entrada | Domínio |
| **Action** | Decisão operacional em resposta a evento (dismiss, escalate, investigate) | Domínio |
| **Harness** | Sistema para experimentar, comparar e benchmark modelos/prompts | Domínio |
| **Correlation ID** | Identificador único para rastrear evento através de toda cadeia | Cross-cutting |

---

## Aggregates

### 1. Event Aggregate (Raiz: MonitoringEvent)

**Responsabilidade**: Encapsular evento recebido e seu ciclo de vida.

**Estrutura**:
```
Event Aggregate
├─ MonitoringEvent (Raiz)
│  ├─ EventId (ValueObject)
│  ├─ CorrelationId (ValueObject)
│  ├─ Source (ValueObject)
│  ├─ EventType (Enum: PerimeterViolation, Intrusion, Movement, Custom)
│  ├─ OccurredAt (DateTime)
│  ├─ Image (Blob)
│  ├─ Metadata (ValueObject)
│  │  ├─ Zone (string)
│  │  ├─ SensorId (string)
│  │  ├─ Sensitivity (Enum: Low, Medium, High)
│  │  └─ CustomAttributes (Dictionary<string, object>)
│  └─ State (Enum: Received, Processing, Analyzed, Archived)
└─ Events (List of Domain Events)
   ├─ EventReceived
   ├─ EventValidated
   ├─ AnalysisStarted
   ├─ AnalysisCompleted
   └─ ActionRecorded
```

**Invariantes**:
- ✅ EventId é único dentro do sistema
- ✅ OccurredAt ≤ ReceivedAt
- ✅ Image não é null e é JPEG válida
- ✅ Metadata contém campos obrigatórios (Zone, SensorId)

**Comportamentos**:
```csharp
public class MonitoringEvent
{
    public void ReceiveAndValidate(EventPayload payload)
    {
        // Validar
        // Publicar EventReceived
    }

    public void StartAnalysis(InferenceContext context)
    {
        // Marcar estado como Processing
        // Publicar AnalysisStarted
    }

    public void CompleteAnalysis(InferenceResult result)
    {
        // Armazenar resultado
        // Marcar estado como Analyzed
        // Publicar AnalysisCompleted
    }

    public void RecordAction(OperationalAction action)
    {
        // Validar ação
        // Armazenar ação
        // Publicar ActionRecorded
    }
}
```

---

### 2. PromptVersion Aggregate (Raiz: Prompt)

**Responsabilidade**: Gerenciar versões de prompts com histórico e métricas.

**Estrutura**:
```
PromptVersion Aggregate
├─ Prompt (Raiz)
│  ├─ PromptId (ValueObject)
│  ├─ Name (string, ex: "PerimeterAnalysis")
│  ├─ Description (string)
│  ├─ Versions (List<PromptVersion>)
│  │  ├─ Version (SemVer, ex: 1.0.0)
│  │  ├─ Content (string, template)
│  │  ├─ CreatedAt (DateTime)
│  │  ├─ CreatedBy (UserId)
│  │  ├─ IsActive (bool)
│  │  ├─ Metrics (PromptMetrics)
│  │  │  ├─ Precision (double 0.0-1.0)
│  │  │  ├─ Recall (double 0.0-1.0)
│  │  │  ├─ AverageConfidence (double)
│  │  │  ├─ AverageLatency (TimeSpan)
│  │  │  └─ SampleSize (int)
│  │  └─ DatasetResults (List<InferenceResult>)
│  └─ RollbackHistory (List<Version>)
└─ Events
   ├─ PromptVersionCreated
   ├─ PromptVersionActivated
   ├─ PromptVersionRolledBack
   └─ PromptMetricsUpdated
```

**Invariantes**:
- ✅ Pelo menos uma versão sempre existe
- ✅ Exatamente uma versão é ativa
- ✅ Versões seguem SemVer
- ✅ Métricas baseadas em >= 100 amostras para relevância

**Comportamentos**:
```csharp
public class Prompt
{
    public void CreateVersion(string content, UserId author)
    {
        // Validar content não vazio
        // Calcular próxima SemVer
        // Criar PromptVersion
        // Publicar PromptVersionCreated
    }

    public void Activate(Version version)
    {
        // Desativar versão anterior
        // Ativar nova versão
        // Publicar PromptVersionActivated
    }

    public void UpdateMetrics(Version version, PromptMetrics metrics)
    {
        // Validar métricas
        // Armazenar
        // Publicar PromptMetricsUpdated
    }

    public void Rollback(Version fromVersion)
    {
        // Validar versão anterior existe
        // Ativar versão anterior
        // Registrar rollback
        // Publicar PromptVersionRolledBack
    }
}
```

---

### 3. InferenceRun Aggregate (Raiz: InferenceExecution)

**Responsabilidade**: Rastrear uma execução de análise completa.

**Estrutura**:
```
InferenceRun Aggregate
├─ InferenceExecution (Raiz)
│  ├─ InferenceRunId (ValueObject)
│  ├─ CorrelationId (ValueObject)
│  ├─ EventId (ValueObject referência)
│  ├─ ModelName (string, ex: "ollama/gemma3:4b")
│  ├─ PromptVersion (PromptVersion ValueObject)
│  ├─ StartedAt (DateTime)
│  ├─ CompletedAt (DateTime nullable)
│  ├─ Status (Enum: Pending, InProgress, Completed, Failed)
│  ├─ SkillResults (List<SkillExecutionResult>)
│  │  ├─ SkillName (string)
│  │  ├─ Input (object)
│  │  ├─ Output (object)
│  │  ├─ Confidence (double 0.0-1.0)
│  │  ├─ Justification (string)
│  │  ├─ Evidence (List<string>)
│  │  ├─ ExecutionTime (TimeSpan)
│  │  └─ Status (Enum: Success, Failed, Timeout)
│  ├─ FinalResult (ValueObject)
│  │  ├─ Classification (Enum)
│  │  ├─ OverallConfidence (double 0.0-1.0)
│  │  ├─ OperationalSummary (string)
│  │  ├─ RecommendedAction (Enum)
│  │  └─ Evidence (List<string>)
│  ├─ Metrics (ExecutionMetrics)
│  │  ├─ TotalDuration (TimeSpan)
│  │  ├─ TokensUsed (int, para modelos que rastreiam)
│  │  ├─ CpuUsagePercent (double)
│  │  └─ MemoryUsageMB (double)
│  └─ Metadata (Dictionary<string, object>)
└─ Events
   ├─ InferenceRunStarted
   ├─ SkillExecuted
   ├─ InferenceRunCompleted
   ├─ InferenceRunFailed
   └─ MetricsRecorded
```

**Invariantes**:
- ✅ InferenceRunId é único
- ✅ CompletedAt ≥ StartedAt (quando completado)
- ✅ SkillResults ordenados por execução
- ✅ FinalResult não é null quando Status = Completed
- ✅ Métricas não são negativas

**Comportamentos**:
```csharp
public class InferenceExecution
{
    public void Start(EventAggregate evt, ModelConfig model, Prompt prompt)
    {
        // Validar
        // Inicializar
        // Publicar InferenceRunStarted
    }

    public void RecordSkillExecution(SkillExecutionResult result)
    {
        // Adicionar a SkillResults
        // Validar invariantes
        // Publicar SkillExecuted
    }

    public void Complete(FinalResult result, ExecutionMetrics metrics)
    {
        // Armazenar resultado
        // Armazenar métricas
        // Marcar como Completed
        // Publicar InferenceRunCompleted
    }

    public void Fail(Exception error)
    {
        // Registrar erro
        // Marcar como Failed
        // Publicar InferenceRunFailed
    }
}
```

---

## Value Objects

### EventMetadata
```csharp
public class EventMetadata
{
    public string Zone { get; }
    public string SensorId { get; }
    public Sensitivity Sensitivity { get; }
    public Dictionary<string, object> CustomAttributes { get; }
    // ValueObject behavior: immutable, equality by value
}
```

### Classification
```csharp
public enum Classification
{
    Valid,
    PossibleFalsePositive,
    Suspicious,
    HumanReviewRequired,
    Inconclusive
}
```

### Confidence
```csharp
public class Confidence
{
    public double Score { get; } // 0.0-1.0
    public string Justification { get; }
    public List<string> Evidence { get; }
    
    // Validar 0.0 <= Score <= 1.0
}
```

### CorrelationId
```csharp
public class CorrelationId
{
    public Guid Value { get; }
    // Imutável, único
}
```

---

## Domain Events

### Catálogo Completo de Events

```
EventDomain
├─ EventReceived (quando evento chega)
├─ EventValidated (quando passa validação)
└─ ActionRecorded (quando operador decide)

SkillOrchestrationDomain
├─ AnalysisStarted
├─ SkillExecutionStarted
├─ SkillExecutionCompleted
├─ SkillExecutionFailed
└─ AnalysisCompleted

InferenceDomain
├─ InferenceRunStarted
├─ SkillInferenceExecuted
├─ InferenceRunCompleted
├─ InferenceRunFailed
└─ MetricsRecorded

PromptDomain
├─ PromptVersionCreated
├─ PromptVersionActivated
├─ PromptVersionRolledBack
└─ PromptMetricsUpdated

HarnessDomain
├─ BenchmarkStarted
├─ BenchmarkCompleted
├─ ComparisonGenerated
└─ ResultsExported
```

---

## Repositories (Interfaces - Domínio não conhece implementação)

```csharp
public interface IEventRepository
{
    Task<MonitoringEvent> GetByIdAsync(EventId eventId);
    Task<List<MonitoringEvent>> GetByZoneAsync(string zone, DateRange dateRange);
    Task SaveAsync(MonitoringEvent evt);
    Task<bool> ExistsAsync(EventId eventId);
}

public interface IPromptRepository
{
    Task<Prompt> GetByIdAsync(PromptId promptId);
    Task<Prompt> GetActiveVersionAsync(string promptName);
    Task SaveAsync(Prompt prompt);
    Task<List<Prompt>> GetAllAsync();
}

public interface IInferenceRunRepository
{
    Task<InferenceExecution> GetByIdAsync(InferenceRunId runId);
    Task<List<InferenceExecution>> GetByEventIdAsync(EventId eventId);
    Task SaveAsync(InferenceExecution execution);
}
```

---

## Domain Services

### SkillOrchestrator
```csharp
public interface ISkillOrchestrator
{
    Task<OrchestrationResult> ExecutePipelineAsync(
        MonitoringEvent evt,
        string pipelineConfigName,
        CancellationToken cancellation);
}
```

### InferenceService
```csharp
public interface IInferenceProvider
{
    Task<InferenceResult> InferAsync(
        InferenceRequest request,
        CancellationToken cancellation);
}
```

---

## Constraints & Business Rules

1. **Uniqueness**: Cada EventId é único globalmente
2. **Ordering**: SkillResults devem estar ordenados por execução
3. **Atomicity**: Análise completa (todos skills) ou nenhuma é persistida
4. **Immutability**: Uma vez finalizado, resultado não muda
5. **Auditability**: Toda mudança de prompt é registrada com versão
6. **Confidence**: Score 0.0-1.0, requerido para classificação válida

---

## Diagramas

### Class Diagram (Simplified)

```
┌─────────────────────────────────────┐
│    MonitoringEvent (Entity)         │
├─────────────────────────────────────┤
│ EventId: ValueObject                │
│ CorrelationId: ValueObject          │
│ Source: string                      │
│ EventType: enum                     │
│ Image: Blob                         │
│ Metadata: ValueObject               │
│ State: enum                         │
└─────────────────────────────────────┘
            │ Analyzed By
            ↓
┌─────────────────────────────────────┐
│  InferenceExecution (Entity)        │
├─────────────────────────────────────┤
│ InferenceRunId: ValueObject         │
│ ModelName: string                   │
│ PromptVersion: ValueObject          │
│ SkillResults: List<SkillResult>     │
│ FinalResult: ValueObject            │
│ Metrics: ValueObject                │
└─────────────────────────────────────┘
            │ Uses
            ↓
┌─────────────────────────────────────┐
│    Prompt (Aggregate)               │
├─────────────────────────────────────┤
│ PromptId: ValueObject               │
│ Versions: List<PromptVersion>       │
│ CreateVersion()                     │
│ Activate()                          │
│ Rollback()                          │
└─────────────────────────────────────┘
```

---

## Transformação do Domain Model em Código

O Domain Model será implementado respeitando:
- ✅ Entities com identidade única
- ✅ Value Objects imutáveis
- ✅ Aggregates com raiz clara
- ✅ Domain Events para comunicação
- ✅ Repositories como interfaces
- ✅ Domain Logic isolado de Infrastructure

**Mapeamento para Projeto**:
- `SentinelaOps.Domain/Entities/` → MonitoringEvent, InferenceExecution, Prompt
- `SentinelaOps.Domain/ValueObjects/` → EventId, Confidence, Classification, etc.
- `SentinelaOps.Domain/Events/` → Domain Events
- `SentinelaOps.Domain/Repositories/` → Repository Interfaces (não implementação)
- `SentinelaOps.Domain/Services/` → Domain Services (ISkillOrchestrator, IInferenceProvider)
