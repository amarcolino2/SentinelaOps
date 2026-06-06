# Event Storming

## Visão Geral

Event Storming é uma técnica de modelagem colaborativa que:
1. Identifica Domain Events (coisas que acontecem no domínio)
2. Agrupa eventos em fluxos
3. Identifica Aggregates que causam eventos
4. Revela Bounded Contexts

---

## Timeline de Events

### Fase 1: Evento Chegada

**Timeline**: T=0ms a T=100ms

```
T=0ms
├─ MonitoringSystemEventDetected (Sistema Externo)
│  └─ Payload: {image, metadata, eventType}
│
├─ EventReceived (Domain Event)
│  ├─ EventId: UUID
│  ├─ CorrelationId: UUID
│  ├─ Timestamp: T=0
│  ├─ Published by: Event Receiver Bounded Context
│  └─ Subscribers: API, Logging, Metrics
│
├─ EventValidated (Domain Event) [T=50ms]
│  ├─ Validation: OK
│  ├─ Published by: Event Receiver
│  └─ Subscribers: Queue
│
└─ EventEnqueued (Domain Event) [T=100ms]
   ├─ Queue: RabbitMQ
   ├─ Published by: Event Receiver
   └─ Subscribers: Skill Orchestrator
```

### Fase 2: Análise / Orquestração

**Timeline**: T=100ms a T=2000ms

```
T=100ms
├─ AnalysisStarted (Domain Event)
│  ├─ CorrelationId: (propagated)
│  ├─ Pipeline: PerimeterAnalysis→FalsePositive→Severity→Summary
│  ├─ Published by: Skill Orchestrator
│  └─ Subscribers: Inference Harness, Observability
│
├─ PerimeterAnalysisSkillExecutionStarted [T=100ms]
│  └─ Skill Request: {image, zone_config, metadata}
│
├─ InferenceRunStarted [T=110ms]
│  ├─ RunId: UUID
│  ├─ Model: ollama/gemma3:4b
│  ├─ PromptVersion: PerimeterAnalysis v1.1.0
│  ├─ Published by: Inference Provider
│  └─ Subscribers: Harness, Metrics
│
├─ InferenceExecuted [T=600ms]
│  ├─ Result: {classification: "inside", confidence: 0.92, evidence: [...]}
│  ├─ Tokens: 245
│  ├─ Duration: 490ms
│  ├─ Published by: Ollama (via Inference Provider)
│  └─ Subscribers: Skill Orchestrator, Harness
│
├─ PerimeterAnalysisSkillCompleted [T=620ms]
│  ├─ Output: {classification, confidence, justification, evidence}
│  ├─ Published by: Skill Orchestrator
│  └─ Subscribers: Next Skill
│
├─ FalsePositiveAnalysisSkillExecutionStarted [T=620ms]
│  └─ Input: (output from previous skill)
│
├─ InferenceRunStarted [T=630ms]
│  └─ ... (repeat for FalsePositiveAnalysis)
│
├─ InferenceExecuted [T=1200ms]
│  ├─ Result: {probabilityFalsePositive: 0.15, reason: "..."}
│  └─ Duration: 570ms
│
├─ FalsePositiveAnalysisSkillCompleted [T=1220ms]
│
├─ SeverityClassificationSkillExecutionStarted [T=1220ms]
├─ InferenceRunStarted [T=1230ms]
├─ InferenceExecuted [T=1750ms]
├─ SeverityClassificationSkillCompleted [T=1770ms]
│
├─ IncidentSummarySkillExecutionStarted [T=1770ms]
├─ InferenceRunStarted [T=1780ms]
├─ InferenceExecuted [T=1950ms]
├─ IncidentSummarySkillCompleted [T=1970ms]
│
└─ AnalysisCompleted [T=2000ms]
   ├─ FinalResult: {classification, confidence, summary, recommendation}
   ├─ TotalDuration: 1900ms
   ├─ Published by: Skill Orchestrator
   └─ Subscribers: Persistence, API, Harness, Notifications
```

### Fase 3: Persistência

**Timeline**: T=2000ms a T=2200ms

```
T=2000ms
├─ AnalysisResultPersistenceStarted
│  └─ CorrelationId: (propagated)
│
├─ EventPersisted [T=2050ms]
│  ├─ Location: SQLite
│  ├─ EventId: indexed
│  └─ Published by: Persistence Repository
│
├─ InferenceResultPersisted [T=2100ms]
│  ├─ Includes: all skill results, metrics, metadata
│  └─ Published by: Persistence Repository
│
└─ AnalysisResultAvailable [T=2200ms]
   ├─ EventId: resolvível via API
   ├─ Status: ready
   ├─ Published by: Persistence Layer
   └─ Subscribers: API, Notifications
```

### Fase 4: Operação Humana

**Timeline**: T=2200ms a T=120000ms (var)

```
T=2200ms
├─ OperatorNotified
│  ├─ Channel: Push notification / WebSocket
│  ├─ Payload: EventId + summary + confidence
│  └─ Published by: API Notification Service
│
T=2300ms
├─ OperatorConsultedEvent [Human Actor]
│  ├─ Method: GET /api/v1/events/{eventId}
│  ├─ Response: Complete analysis
│  └─ Subscriber action: Review
│
T=120000ms (2 minutes later)
├─ ActionRecorded (Domain Event)
│  ├─ EventId: (same)
│  ├─ Action: dismiss | escalate | investigate
│  ├─ Reason: "Reflexo detectado"
│  ├─ UserId: operator@domain
│  ├─ Timestamp: T=120000ms
│  ├─ Published by: API (from Operator)
│  └─ Subscribers: Persistence, Audit Log, Analytics
│
├─ AuditLogAppended [T=120010ms]
│  ├─ Type: ACTION_RECORDED
│  ├─ Actor: operator@domain
│  ├─ Details: action, reason, eventId
│  ├─ Immutable: true
│  └─ Timestamp: T=120010ms
│
└─ AnalyticsEventRecorded [T=120020ms]
   ├─ Decision time: 120s
   ├─ Action effectiveness: (TBD)
   └─ Feedback loop
```

---

## Agregação por Contexto

### Event Receiver Context
```
Events Gerados:
├─ EventReceived
├─ EventValidated
├─ EventEnqueued
└─ ValidationFailed

Responsáveis:
├─ Aggregate: MonitoringEvent
├─ Service: EventValidationService
└─ Command Handler: ReceiveEventHandler
```

### Skill Orchestration Context
```
Events Gerados:
├─ AnalysisStarted
├─ SkillExecutionStarted
├─ SkillExecutionCompleted
├─ SkillExecutionFailed
├─ AnalysisCompleted
└─ AnalysisFailed

Responsáveis:
├─ Aggregate: (no aggregate, pure orchestration)
├─ Service: SkillOrchestrator
└─ Command Handler: ExecutePipelineHandler
```

### Inference Provider Context
```
Events Gerados:
├─ InferenceRunStarted
├─ InferenceExecuted
├─ InferenceRunCompleted
├─ InferenceRunFailed
└─ MetricsRecorded

Responsáveis:
├─ Aggregate: InferenceExecution
├─ Service: OllamaInferenceService (implementação de IInferenceProvider)
└─ Command Handler: ExecuteInferenceHandler
```

### Persistence Context
```
Events Gerados:
├─ EventPersisted
├─ InferenceResultPersisted
├─ ActionPersisted
├─ AuditLogAppended
└─ PersistenceFailed

Responsáveis:
├─ Aggregate: (repository pattern)
├─ Service: EventRepository, InferenceRunRepository
└─ Command Handler: PersistAnalysisHandler
```

### Inference Harness Context
```
Events Gerados:
├─ BenchmarkStarted
├─ InferenceRunRecorded
├─ BenchmarkCompleted
├─ ComparisonGenerated
├─ ResultsExported
└─ PromptVersionEvaluated

Responsáveis:
├─ Aggregate: BenchmarkRun, PromptVersion
├─ Service: HarnessCoordinator
└─ Command Handlers: StartBenchmarkHandler, GenerateComparisonHandler
```

---

## Hotspots (Questões Importantes)

### HS-001: Quando Confiança é Calculada?
**Questão**: Confidence score é calculado por Skill ou agregado no final?

**Evento Relacionado**: InferenceExecuted vs AnalysisCompleted

**Decisão (ADR-003)**:
- Cada Skill retorna sua própria confidence
- Confidence final é agregada (média ponderada) no IncidentSummarySkill
- Se um Skill falha, confidence final baixa

### HS-002: Como Lidar com Skill Timeout?
**Questão**: Se Skill demora > 5s, o que acontece?

**Evento Relacionado**: SkillExecutionFailed (timeout)

**Decisão (ADR-004)**:
- Timeout depois 5s
- Publicar SkillExecutionFailed
- Próxima Skill continua com resultado parcial
- Confidence final reduzida

### HS-003: Quando Persistir Resultado Intermediário vs Final?
**Questão**: Armazenar cada SkillResult ou só resultado final?

**Evento Relacionado**: SkillExecutionCompleted vs AnalysisCompleted

**Decisão (ADR-005)**:
- Armazenar resultado final + todos SkillResults como JSON
- Permite auditoria completa
- Replayabilidade com novo prompt

### HS-004: Propagação de CorrelationId
**Questão**: Como garantir CorrelationId em toda cadeia?

**Decisão (ADR-006)**:
- Gerar CorrelationId no Event Receiver
- Propagar em todos Domain Events
- Incluir em logs estruturados
- Incluir em headers HTTP (X-Correlation-ID)

---

## Regras de Fluxo

### Fluxo Bem-Sucedido Esperado
```
EventReceived
  → EventValidated
    → EventEnqueued
      → AnalysisStarted
        → PerimeterAnalysisSkillCompleted
          → FalsePositiveAnalysisSkillCompleted
            → SeverityClassificationSkillCompleted
              → IncidentSummarySkillCompleted
                → AnalysisCompleted
                  → EventPersisted
                    → AnalysisResultAvailable
                      → OperatorNotified
                        → ActionRecorded
                          → AuditLogAppended
```

### Caminhos de Falha
```
Path 1: Validação Falha
EventReceived → ValidationFailed → AuditLogAppended

Path 2: Skill Falha
AnalysisStarted → SkillExecutionFailed → AnalysisCompleted (partial) → EventPersisted

Path 3: Persistência Falha
AnalysisCompleted → PersistenceFailed → Retry (exponential backoff) → DeadLetterQueue
```

---

## Métricas Derivadas de Events

Do fluxo de events, podemos derivar:

1. **Latência Total**: EventReceived → ActionRecorded
2. **Latência Análise**: AnalysisStarted → AnalysisCompleted
3. **Latência por Skill**: SkillExecutionStarted → SkillExecutionCompleted
4. **Throughput**: EventReceived count por segundo
5. **Taxa de Falha**: (InferenceRunFailed + SkillExecutionFailed) / AnalysisStarted
6. **Taxa de Timeout**: SkillExecutionFailed (timeout) / SkillExecutionStarted
7. **Confiança Média**: Aggregação de todos AnalysisCompleted confidence
8. **Tempo para Decisão**: ActionRecorded timestamp - AnalysisResultAvailable timestamp
9. **Taxa de False Positive**: (ActionRecorded action=dismiss) / AnalysisCompleted
10. **Taxa de Escalação**: (ActionRecorded action=escalate) / AnalysisCompleted

---

## Mapeamento para Implementation

### Application Layer Commands
Do Event Storming, derivamos Commands:

```
ReceiveEventCommand
  → Handler: ReceiveEventHandler
  → Publishes: EventReceived

ExecutePipelineCommand
  → Handler: ExecutePipelineHandler
  → Publishes: AnalysisStarted, SkillExecutionStarted, ..., AnalysisCompleted

ExecuteInferenceCommand
  → Handler: ExecuteInferenceHandler
  → Publishes: InferenceRunStarted, InferenceExecuted

PersistAnalysisCommand
  → Handler: PersistAnalysisHandler
  → Publishes: EventPersisted, InferenceResultPersisted

RecordActionCommand
  → Handler: RecordActionHandler
  → Publishes: ActionRecorded, AuditLogAppended
```

### Domain Events Subscription
Subscribers escutam eventos para Side Effects:

```
EventReceived
  ├─ → LoggingService (estruturado)
  ├─ → MetricsService (counter events_received)
  └─ → AnalyticsService (armazenar timestamp)

AnalysisCompleted
  ├─ → NotificationService (notificar operador)
  ├─ → MetricsService (gauge confidence_score)
  └─ → PersistenceService (salvar resultado)

ActionRecorded
  ├─ → AuditLog (imutável)
  ├─ → AnalyticsService (análise efetividade)
  └─ → MetricsService (gauge decision_time)
```

Isso garante que domínio fica desacoplado de side effects.
