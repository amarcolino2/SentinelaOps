# Roadmap Evolutivo

## Visão Estratégica do Roadmap

O Roadmap apresenta evolução do projeto em 3 Fases, alinhadas com entrega de valor progressivo:

- **Fase 1 (MVP)**: Valor imediato, prototipagem, validação
- **Fase 2**: Escala horizontal, múltiplos modelos, produção
- **Fase 3**: Evolução contínua, ML, referência arquitetural

---

## Fase 1: MVP (Semanas 1-16)

### Objetivo
Entregar sistema funcional capaz de receber eventos, analisar e reduzir falsos positivos com Ollama + Gemma 3.

### Etapas

#### Etapa 1.1: Especificação (Semanas 1-4) ✅ CONCLUÍDA
- ✅ Visão do Produto
- ✅ Problema de Negócio
- ✅ Objetivos
- ✅ Requisitos Funcionais (23 REQ)
- ✅ Requisitos Não-Funcionais (25 NFRE)
- ✅ Casos de Uso (5 UC)
- ✅ Domain Model
- ✅ Event Storming
- ✅ Context Map (8 Bounded Contexts)
- ✅ ADRs (13 decisões)
- ✅ Estratégia de IA
- ✅ Estratégia de Skills
- ✅ Estratégia de Harness
- ✅ Estrutura da Solução
- ✅ Roadmap

**Saídas**: 
- Documentação completa em `/docs`
- Arquitetura clara
- Alinhamento stakeholder
- Zero ambiguidades

**Responsável**: Arquiteto Principal
**Status**: ✅ COMPLETADO

---

#### Etapa 1.2: Infraestrutura & Setup (Semanas 5-6)

**Tarefas**:
1. Criar solução .NET 8 (SentinelaOps.sln)
2. Setup Docker Compose (Ollama, RabbitMQ, SQLite, Jaeger)
3. Configurar CI/CD (GitHub Actions)
4. Configurar test coverage tools
5. Configurar linting (StyleCop)
6. Documentação de desenvolvimento

**Entregáveis**:
- Solution file pronto
- Docker Compose funcionando
- `docker-compose up` traz ambiente completo
- README atualizado
- Developers conseguem rodar localmente em < 5 minutos

**Tempo Estimado**: 2 semanas
**Responsável**: DevOps Engineer + Backend Lead

---

#### Etapa 1.3: Domain Layer (Semanas 7-8)

**Tarefas**:
1. Implementar Entities (MonitoringEvent, InferenceExecution, Prompt)
2. Implementar Value Objects (EventId, CorrelationId, Classification, Confidence)
3. Implementar Domain Events (EventReceived, AnalysisCompleted, ActionRecorded)
4. Implementar Repository Interfaces
5. Testes unitários (> 90% cobertura)
6. Documentação de domínio

**Entregáveis**:
- `SentinelaOps.Domain` projeto
- ~500 linhas de código de domínio
- ~1000 linhas de testes
- Zero dependências externas no domínio

**Tempo Estimado**: 2 semanas
**Responsável**: Senior Backend Engineer

---

#### Etapa 1.4: Application Layer (Semanas 9-10)

**Tarefas**:
1. Setup MediatR para CQRS
2. Implementar Command Handlers:
   - ReceiveEventHandler
   - ExecutePipelineHandler
   - RecordActionHandler
3. Implementar Query Handlers
4. Implementar Application Services
5. Mappers (Domain → DTO)
6. Validação de Commands
7. Testes de integração

**Entregáveis**:
- `SentinelaOps.Application` projeto
- CQRS handlers orquestrando domínio
- Validação automática
- ~1500 linhas de código
- ~800 linhas de testes

**Tempo Estimado**: 2 semanas
**Responsável**: Senior Backend Engineer

---

#### Etapa 1.5: Infrastructure Layer (Semanas 11-12)

**Tarefas**:
1. EF Core setup com SQLite
2. Implementar Repositories
3. Unit of Work pattern
4. Database migrations
5. RabbitMQ publisher/subscriber
6. OpenTelemetry integration
7. Structured logging com Serilog

**Entregáveis**:
- `SentinelaOps.Infrastructure` projeto
- Migrations automáticas (v001, v002, etc)
- Persistência agnóstica (SQLite agora, fácil trocar depois)
- Messaging assíncrono funcional
- Observabilidade completa (logs, traces, métricas)

**Tempo Estimado**: 2 semanas
**Responsável**: Backend Engineer + DevOps

---

#### Etapa 1.6: API & Controllers (Semanas 13)

**Tarefas**:
1. ASP.NET Core controllers
2. Endpoints conforme REQ (POST /api/v1/events, GET /api/{id}, etc)
3. OpenAPI/Swagger documentação
4. Middleware (CorrelationId, error handling, auth)
5. JWT autenticação
6. RBAC autorização

**Entregáveis**:
- `SentinelaOps.Api` projeto roddando
- 5 endpoints principais
- Swagger docs acessível
- Autenticação funcional

**Tempo Estimado**: 1 semana
**Responsável**: Backend Engineer

---

#### Etapa 1.7: Skills Implementation (Semanas 13-14)

**Tarefas**:
1. Implementar ISkill interface
2. Implementar 5 skills iniciais:
   - PerimeterAnalysisSkill
   - FalsePositiveAnalysisSkill
   - SeverityClassificationSkill
   - IncidentSummarySkill
   - (bonus: IntrusionAnalysisSkill)
3. Skill Registry & Orchestrator
4. Pipeline configuration
5. Testes de skills

**Entregáveis**:
- 5 módulos de Skill
- Pipeline orquestração funcional
- Timeout handling
- Error recovery

**Tempo Estimado**: 2 semanas
**Responsável**: ML Engineer + Backend

---

#### Etapa 1.8: Inference Provider (Semanas 14-15)

**Tarefas**:
1. Implementar IInferenceProvider interface
2. Implementar OllamaInferenceProvider
3. Prompt building e template engine
4. Result parsing (JSON extraction)
5. Circuit breaker & retry policy
6. Timeouts and error handling
7. Métricas de inferência

**Entregáveis**:
- OllamaInferenceProvider funcional
- Integração com Ollama container
- Agnósticismo garantido
- Fallback gracefully

**Tempo Estimado**: 2 semanas
**Responsável**: ML Engineer

---

#### Etapa 1.9: Worker Background Service (Semana 15)

**Tarefas**:
1. Hosted Service para processar fila
2. RabbitMQ consumer
3. Orchestração de análise
4. Error handling e retry
5. Graceful shutdown

**Entregáveis**:
- `SentinelaOps.Worker` projeto
- Processamento end-to-end funcional
- Heartbeat monitoring

**Tempo Estimado**: 1 semana
**Responsável**: Backend Engineer

---

#### Etapa 1.10: Harness (Semana 16)

**Tarefas**:
1. Harness orchestrator
2. Model comparison endpoint
3. Prompt versioning
4. Basic benchmarking
5. Results export (JSON/CSV)

**Entregáveis**:
- `/harness` endpoints
- Capacidade de comparar modelos
- Versioning de prompts

**Tempo Estimado**: 1 semana
**Responsável**: ML Engineer

---

#### Etapa 1.11: Testing & QA (Semana 16+)

**Tarefas**:
1. Testes unitários: > 80%
2. Testes de integração: fluxo crítico
3. Testes E2E: evento real
4. Load testing: 10 evt/s
5. Security testing básico
6. Performance profiling

**Entregáveis**:
- Cobertura > 80%
- Relatórios de teste
- Documentação de teste

**Responsável**: QA Engineer

---

### Milestone: MVP ENTREGUE

**Data Esperada**: Final Semana 16 (≈ 4 meses)

**Capacidades**:
- ✅ Receber eventos de qualquer fonte
- ✅ Validar eventos
- ✅ Executar pipeline de skills
- ✅ Analisar com Ollama + Gemma 3
- ✅ Persistir resultados
- ✅ Consultar via API
- ✅ Registrar ações operacionais
- ✅ Harness comparar modelos
- ✅ Observabilidade (logs, traces, métricas)
- ✅ Auditoria completa

**Métricas de Sucesso**:
- ✅ Latência p95 < 2s
- ✅ Throughput 10+ evt/s
- ✅ Taxa de falsos positivos 70% → 20-30%
- ✅ Cobertura de teste > 80%
- ✅ Zero production incidents por design

---

## Fase 2: Escalação & Produção (Semanas 17-28)

### Objetivo
Escalar sistema para produção, adicionar suporte a múltiplos modelos, otimizar performance.

### Etapas

#### Etapa 2.1: Database Evolution (Semanas 17-18)
- ✅ Implementar PostgreSQL adapter
- ✅ Testar migrations
- ✅ Performance profiling
- ✅ Backup strategy

#### Etapa 2.2: Multiple Inference Providers (Semanas 19-20)
- ✅ Azure OpenAI provider
- ✅ Anthropic Claude provider
- ✅ Model registry
- ✅ Dynamic provider loading

#### Etapa 2.3: Advanced Harness (Semanas 20-21)
- ✅ Synthetic dataset generation
- ✅ A/B testing automation
- ✅ Cost analysis
- ✅ Performance monitoring
- ✅ Anomaly detection

#### Etapa 2.4: Native ONVIF/RTSP Support (Semanas 22-23)
- ✅ ONVIF client
- ✅ RTSP stream integration
- ✅ Event detection from streams
- ✅ Image extraction

#### Etapa 2.5: Additional Skills (Semanas 24-25)
- ✅ MotionAnalysisSkill
- ✅ HumanActivityAnalysisSkill
- ✅ VehicleAnalysisSkill

#### Etapa 2.6: Production Hardening (Semanas 26-27)
- ✅ Security audit
- ✅ Performance optimization
- ✅ Kubernetes readiness
- ✅ HA/DR strategy

#### Etapa 2.7: Enterprise Features (Semana 28)
- ✅ Multi-tenancy
- ✅ Advanced RBAC
- ✅ Audit log export
- ✅ Compliance reporting

### Milestone: PRODUCTION READY

**Data Esperada**: Final Semana 28 (≈ 7 meses total)

---

## Fase 3: Evolução & Referência (Semanas 29+)

### Objetivo
Manter sistema atualizado, adicionar funcionalidades avançadas, estabelecer como referência arquitetural.

### Etapas

#### Etapa 3.1: ML-Based Confidence Calibration (Semanas 29-31)
- Treinar modelo para calibração de confiança
- Fine-tuning baseado em feedback real
- Confidence curve per modelo

#### Etapa 3.2: Community & Marketplace (Semanas 32-33)
- Plugin marketplace para skills
- Contribuições da comunidade
- Versioning de skills
- Rating de skills

#### Etapa 3.3: Advanced Analytics (Semanas 34-35)
- Dashboard Grafana avançada
- Análise de operador effectiveness
- Trend analysis
- Anomaly detection

#### Etapa 3.4: Mobile & Modern UI (Semanas 36-37)
- React/Vue web app moderna
- Mobile app (iOS/Android)
- Real-time notifications
- Live video integration

#### Etapa 3.5: Continuous Improvement (Ongoing)
- Feedback loop from production
- Prompt optimization
- Model evaluation
- Documentation updates

---

## Dependências Entre Fases

```
Fase 1: MVP (16 semanas)
  └─ Fase 2: Production (12 semanas)
      └─ Fase 3: Evolution (ongoing)
```

Cada fase depende da anterior estar completa e funcional.

---

## Alocação de Recursos

### Fase 1
```
Team:
├─ 1x Arquiteto Principal (4 semanas spec, então advisory)
├─ 2x Senior Backend Engineers (16 semanas)
├─ 1x ML Engineer (14 semanas)
├─ 1x DevOps Engineer (10 semanas)
├─ 1x QA Engineer (8 semanas)
└─ 1x Scrum Master (16 semanas)

Total: ~6 FTE
```

### Fase 2
```
Team:
├─ 1x Senior Backend Engineer (12 semanas)
├─ 1x ML Engineer (12 semanas)
├─ 1x DevOps Engineer (12 semanas)
├─ 1x Security Engineer (6 semanas)
└─ 1x QA Engineer (12 semanas)

Total: ~4.5 FTE
```

### Fase 3
```
Team:
├─ 1x Backend Engineer (ongoing)
├─ 1x ML Engineer (ongoing)
├─ 1x DevOps Engineer (ongoing)
├─ 1x Community Manager (ongoing)
└─ Contributors (open source)

Total: ~3 FTE + community
```

---

## Entregáveis por Fase

### Fase 1
- ✅ Documentação de especificação (CURRENT)
- ✅ Código MVP funcional
- ✅ Docker Compose local dev
- ✅ API OpenAPI docs
- ✅ README e guides de setup
- ✅ Test suite > 80% cobertura

### Fase 2
- ✅ Production docker images
- ✅ Kubernetes manifests
- ✅ Multiple database support
- ✅ Multiple model support
- ✅ Advanced monitoring
- ✅ Security documentation

### Fase 3
- ✅ Modern web UI
- ✅ Community marketplace
- ✅ Advanced analytics
- ✅ ML optimization tools
- ✅ Integration ecosystem

---

## Timeline Resumido

| Fase | Duração | Start | End | Status |
|------|---------|-------|-----|--------|
| Especificação | 4 sem | Semana 1 | Semana 4 | ✅ Concluída |
| MVP | 12 sem | Semana 5 | Semana 16 | 📅 Planejada |
| Produção | 12 sem | Semana 17 | Semana 28 | 📅 Planejada |
| Evolução | Ongoing | Semana 29+ | ∞ | 📅 Planejada |

**Total até Produção**: 28 semanas (≈ 7 meses)
**Total MVP**: 16 semanas (≈ 4 meses)

---

## Critérios de Sucesso por Fase

### Fase 1
- ✅ Latência p95 < 2s ✓
- ✅ Redução de 50% em falsos positivos ✓
- ✅ Operadores conseguem usar em < 5 min de treinamento ✓
- ✅ Auditoria completa rastreável ✓
- ✅ Cobertura de teste > 80% ✓
- ✅ Zero production downtime
- ✅ API documentada e testável

### Fase 2
- ✅ Suporta múltiplos modelos simultaneamente
- ✅ Deploy em Kubernetes
- ✅ Multi-region capability
- ✅ Compliance (LGPD/GDPR) validado
- ✅ Enterprise SLA (99.5% uptime)

### Fase 3
- ✅ 500+ GitHub stars
- ✅ 50+ contribuições externas
- ✅ Referência arquitetural reconhecida
- ✅ Adotado por 5+ organizações

---

## Próximos Passos Imediatos

1. **AGORA**: Validar especificação com stakeholders
2. **Semana 1**: Kickoff do projeto
3. **Semana 2**: Setup infraestrutura
4. **Semana 3**: Iniciar implementação Domain layer
5. **Semana 4**: Especificação 100% validada

---

## Documento de Rastreamento

Este roadmap será atualizado:
- Semanalmente: Progress updates
- Bi-semanalmente: Retrospective & adjustments
- Mensalmente: Stakeholder review

Link: [Acompanhar Roadmap](roadmap/PROGRESS.md) (será criado)
