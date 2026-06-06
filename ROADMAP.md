# 🗺️ Roadmap SentinelaOps

Visão de longo prazo e próximas etapas do projeto.

---

## 📊 Status Atual (Junho 2026)

```
Conclusão: ███████░░░ 70%

✅ Etapa 1.1 - 1.3: Domain Core + Testes
🔄 Etapa 1.4 - 1.5: Application Services (EM PROGRESSO)
⏳ Etapa 2.0 - 3.0: Worker + API + Dashboard
```

---

## 🎯 Q2 2026 (Junho - Julho)

### Etapa 1.4: Agregados Adicionais ⏳
```
[ ] PromptVersion Aggregate
    ├─ Versionamento SemVer
    ├─ Métricas (precision, recall, confidence, latency)
    ├─ Histórico de ativações/rollback
    └─ Repository Interface + Testes

[ ] InferenceExecution Aggregate
    ├─ Rastreamento de execução de modelo
    ├─ Input/Output com latency
    ├─ Tokens usados
    └─ Repository Interface + Testes
```

**ETA**: 1 semana | **Contribuidores**: 1-2

---

### Etapa 1.5: Application Services ⏳
```
[ ] ProcessEventCommand + Handler
    ├─ Orquestração de análise
    ├─ Chamada de skills
    └─ Agregação de resultados

[ ] 8 Skills como Application Services
    ├─ PerimeterAnalysisService
    ├─ IntrusionAnalysisService
    ├─ MotionAnalysisService
    ├─ HumanActivityAnalysisService
    ├─ VehicleAnalysisService
    ├─ FalsePositiveAnalysisService
    ├─ SeverityClassificationService
    └─ IncidentSummaryService

[ ] SkillOrchestrator
    ├─ Execução paralela de skills
    ├─ Agregação de resultados
    └─ Timeout handling
```

**ETA**: 2 semanas | **Contribuidores**: 2-3

---

## 🎯 Q3 2026 (Julho - Setembro)

### Etapa 2.0: Worker Service
```
[ ] SentinelaOps.Worker
    ├─ Background Processing Host
    ├─ RabbitMQ Consumer
    ├─ Event Processing Loop
    └─ Error Handling + Retry

[ ] Infrastructure Layer
    ├─ MonitoringEventRepository (SQL)
    ├─ PromptVersionRepository (SQL)
    ├─ InferenceExecutionRepository (SQL)
    ├─ UnitOfWork Pattern
    └─ Database Migrations (EF Core)

[ ] RabbitMQ Integration
    ├─ Event Publisher
    ├─ Event Subscriber
    └─ Dead Letter Queue
```

**ETA**: 3 semanas | **Contribuidores**: 2-3

---

### Etapa 2.1: REST API
```
[ ] SentinelaOps.Api
    ├─ EventController (POST /events)
    ├─ AnalysisController (GET /analysis/:id)
    ├─ HealthController
    └─ Swagger/OpenAPI

[ ] Authentication & Authorization
    ├─ JWT Token Handler
    ├─ RBAC Policies
    └─ API Key Support

[ ] Error Handling & Logging
    ├─ Global Exception Handler
    ├─ Structured Logging (Serilog)
    └─ Correlation ID Propagation
```

**ETA**: 2 semanas | **Contribuidores**: 2

---

## 🎯 Q4 2026 (Outubro - Dezembro)

### Etapa 3.0: Real-Time Notifications
```
[ ] WebSocket Support
    ├─ SignalR Integration
    ├─ Real-time Event Streaming
    └─ Operator Notifications

[ ] Admin Dashboard (Web)
    ├─ React SPA
    ├─ Event Monitoring
    ├─ Analysis History
    └─ Settings Management

[ ] Metrics & Observability
    ├─ Prometheus Metrics
    ├─ Jaeger Tracing
    ├─ Health Checks
    └─ Dashboards (Grafana)
```

**ETA**: 4 semanas | **Contribuidores**: 3-4

---

### Etapa 3.1: Harness Engineering
```
[ ] SentinelaOps.Harness
    ├─ PromptCatalog Management
    ├─ EvaluationScenarios
    ├─ InferenceRun Tracking
    └─ BenchmarkResults

[ ] Model Comparison
    ├─ Ollama vs OpenAI vs Azure
    ├─ Latency Comparison
    ├─ Cost Analysis
    └─ Accuracy Metrics

[ ] Harness Dashboard
    ├─ Results Visualization
    ├─ Trend Analysis
    └─ Model Recommendations
```

**ETA**: 3 semanas | **Contribuidores**: 2

---

## 2027: Expansão

### Múltiplas Integrações
```
[ ] Monitoring Systems
    ├─ ONVIF Integration
    ├─ RTSP Streaming
    ├─ Hikvision API
    └─ Milestone Integration

[ ] AI Providers
    ├─ Azure OpenAI
    ├─ Anthropic Claude
    ├─ Mistral AI
    └─ Groq (Latency optimized)

[ ] Databases
    ├─ SQL Server
    ├─ PostgreSQL
    ├─ MongoDB
    └─ Cosmos DB (Azure)

[ ] Message Brokers
    ├─ Kafka
    ├─ Azure Service Bus
    └─ AWS SQS
```

---

## 🎓 Learning Goals

Este roadmap é desenhado para oferecer **experiência prática** em:

- ✅ Domain-Driven Design (DDD) em escala
- ✅ Microservices Architecture
- ✅ Event-Driven Systems
- ✅ AI/ML Integration
- ✅ Production-Grade .NET
- ✅ Distributed Systems
- ✅ Cloud-Native Architecture

---

## 🤝 Como Contribuir ao Roadmap

### Você quer...

**Aprender DDD?** → Implemente um novo Aggregate (Etapa 1.4)

**Trabalhar com APIs?** → Implemente REST endpoints (Etapa 2.1)

**Full-Stack?** → Trabalhe no Dashboard (Etapa 3.0)

**DevOps/Infrastructure?** → Worker Service + Deploy (Etapa 2.0)

**Ficar expert em .NET?** → Application Services (Etapa 1.5)

---

## 📅 Contribuintes Esperados

| Etapa | Complexidade | Contribuidores | Tempo |
|-------|-------------|-----------------|-------|
| 1.4 | ⭐⭐ | 1-2 | 1 semana |
| 1.5 | ⭐⭐⭐ | 2-3 | 2 semanas |
| 2.0 | ⭐⭐⭐ | 2-3 | 3 semanas |
| 2.1 | ⭐⭐ | 2 | 2 semanas |
| 3.0 | ⭐⭐⭐⭐ | 3-4 | 4 semanas |
| 3.1 | ⭐⭐⭐ | 2 | 3 semanas |

---

## 💼 Oportunidades de Carreira

### Para Recrutadores 👔
Este projeto atrai engenheiros que conhecem:
- Modern .NET (8.0+)
- Clean Architecture & DDD
- Event-Driven Systems
- AI/ML Integration
- Cloud-Native Development

**Estes são exatamente os perfis que empresas tech procuram!**

---

## 📊 Success Metrics

```
✅ Q2: Domain Core + Tests (Concluído)
⏳ Q3: Production-Ready Worker + API (Target)
✅ Q4: Real-time Features + Harness
🎯 2027: Multi-model, Multi-provider platform
```

---

## 🙋 Interesse em Contribuir?

**Veja as próximas tarefas em**: [NEXT_STEPS.md](NEXT_STEPS.md)

**Entenda a arquitetura**: [ARCHITECTURE.md](ARCHITECTURE.md)

**Siga as convenções**: [CONTRIBUTING.md](CONTRIBUTING.md)

---

**Última atualização**: Junho 6, 2026

Roadmap sujeito a mudanças. Feedback bem-vindo! 🚀
