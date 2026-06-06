# Estrutura da Solução

## Overview

A estrutura de solução segue Clean Architecture + DDD, organizando código em camadas e contextos bem definidos.

---

## Estrutura de Diretórios

```
SentinelaOps/
├── src/
│   ├── SentinelaOps.Domain/                    # Camada de Domínio (zero dependências)
│   │   ├── Entities/
│   │   │   ├── MonitoringEvent.cs
│   │   │   ├── InferenceExecution.cs
│   │   │   └── Prompt.cs
│   │   ├── ValueObjects/
│   │   │   ├── EventId.cs
│   │   │   ├── CorrelationId.cs
│   │   │   ├── Classification.cs
│   │   │   ├── Confidence.cs
│   │   │   └── EventMetadata.cs
│   │   ├── Events/
│   │   │   ├── EventReceived.cs
│   │   │   ├── EventValidated.cs
│   │   │   ├── AnalysisStarted.cs
│   │   │   ├── AnalysisCompleted.cs
│   │   │   ├── SkillExecutionCompleted.cs
│   │   │   ├── InferenceRunCompleted.cs
│   │   │   ├── ActionRecorded.cs
│   │   │   └── ...
│   │   ├── Services/
│   │   │   ├── IInferenceProvider.cs
│   │   │   ├── InferenceRequest.cs
│   │   │   ├── InferenceResult.cs
│   │   │   ├── ISkill.cs
│   │   │   ├── SkillContext.cs
│   │   │   └── SkillResult.cs
│   │   ├── Repositories/
│   │   │   ├── IEventRepository.cs
│   │   │   ├── IInferenceRunRepository.cs
│   │   │   ├── IPromptRepository.cs
│   │   │   ├── IAuditLogRepository.cs
│   │   │   └── IUnitOfWork.cs
│   │   ├── Specifications/
│   │   │   └── EventSpecification.cs
│   │   └── Exceptions/
│   │       ├── DomainException.cs
│   │       ├── EventValidationException.cs
│   │       └── InferenceException.cs
│   │
│   ├── SentinelaOps.Application/               # Camada de Aplicação
│   │   ├── Handlers/                          # Command/Query Handlers
│   │   │   ├── Events/
│   │   │   │   ├── ReceiveEventHandler.cs
│   │   │   │   └── ValidateEventHandler.cs
│   │   │   ├── Orchestration/
│   │   │   │   ├── ExecutePipelineHandler.cs
│   │   │   │   └── OrchestrateAnalysisHandler.cs
│   │   │   ├── Actions/
│   │   │   │   └── RecordActionHandler.cs
│   │   │   └── ...
│   │   ├── Services/                          # Application Services
│   │   │   ├── EventService.cs
│   │   │   ├── AnalysisService.cs
│   │   │   ├── HarnessService.cs
│   │   │   └── ...
│   │   ├── DTOs/
│   │   │   ├── EventDto.cs
│   │   │   ├── AnalysisResultDto.cs
│   │   │   └── ...
│   │   ├── Queries/
│   │   │   ├── GetEventByIdQuery.cs
│   │   │   └── GetEventsQuery.cs
│   │   ├── Commands/
│   │   │   ├── ReceiveEventCommand.cs
│   │   │   ├── ExecutePipelineCommand.cs
│   │   │   └── RecordActionCommand.cs
│   │   └── Mappers/
│   │       ├── EventMapper.cs
│   │       └── AnalysisResultMapper.cs
│   │
│   ├── SentinelaOps.Infrastructure/            # Camada de Infraestrutura
│   │   ├── Persistence/
│   │   │   ├── DatabaseContext.cs
│   │   │   ├── Repositories/
│   │   │   │   ├── EventRepository.cs
│   │   │   │   ├── InferenceRunRepository.cs
│   │   │   │   ├── PromptRepository.cs
│   │   │   │   ├── AuditLogRepository.cs
│   │   │   │   └── UnitOfWork.cs
│   │   │   ├── Migrations/
│   │   │   │   ├── 001_InitialCreate.cs
│   │   │   │   └── ...
│   │   │   └── Mappings/
│   │   │       ├── EventMapping.cs
│   │   │       ├── InferenceExecutionMapping.cs
│   │   │       └── ...
│   │   ├── Inference/
│   │   │   ├── OllamaInferenceProvider.cs
│   │   │   ├── OllamaClient.cs
│   │   │   └── PromptBuilder.cs
│   │   ├── Skills/
│   │   │   ├── SkillRegistry.cs
│   │   │   ├── SkillOrchestrator.cs
│   │   │   ├── Implementations/
│   │   │   │   ├── PerimeterAnalysisSkill.cs
│   │   │   │   ├── FalsePositiveAnalysisSkill.cs
│   │   │   │   ├── SeverityClassificationSkill.cs
│   │   │   │   ├── IncidentSummarySkill.cs
│   │   │   │   └── ...
│   │   │   └── Pipelines/
│   │   │       ├── PipelineConfiguration.cs
│   │   │       └── DefaultPipeline.cs
│   │   ├── Messaging/
│   │   │   ├── RabbitMQPublisher.cs
│   │   │   ├── RabbitMQSubscriber.cs
│   │   │   ├── EventSubscriber.cs
│   │   │   └── DomainEventPublisher.cs
│   │   ├── Observability/
│   │   │   ├── StructuredLogger.cs
│   │   │   ├── TelemetryService.cs
│   │   │   ├── MetricsCollector.cs
│   │   │   └── CorrelationContext.cs
│   │   ├── Cache/
│   │   │   ├── PromptCache.cs
│   │   │   └── CacheService.cs
│   │   ├── Http/
│   │   │   ├── OllamaHttpClient.cs
│   │   │   └── HttpClientFactory.cs
│   │   └── Configuration/
│   │       ├── InferenceOptions.cs
│   │       ├── PersistenceOptions.cs
│   │       ├── MessagingOptions.cs
│   │       └── ObservabilityOptions.cs
│   │
│   ├── SentinelaOps.Api/                      # Camada de API/Presentation
│   │   ├── Controllers/
│   │   │   ├── EventsController.cs
│   │   │   ├── AnalysisController.cs
│   │   │   ├── ActionsController.cs
│   │   │   ├── HealthController.cs
│   │   │   └── HarnessController.cs
│   │   ├── Middleware/
│   │   │   ├── CorrelationIdMiddleware.cs
│   │   │   ├── ErrorHandlingMiddleware.cs
│   │   │   ├── LoggingMiddleware.cs
│   │   │   └── AuthenticationMiddleware.cs
│   │   ├── Filters/
│   │   │   ├── ExceptionFilter.cs
│   │   │   ├── ValidationFilter.cs
│   │   │   └── AuthorizationFilter.cs
│   │   ├── Hubs/
│   │   │   └── EventNotificationHub.cs     # WebSocket
│   │   ├── Validators/
│   │   │   ├── EventRequestValidator.cs
│   │   │   └── ActionRequestValidator.cs
│   │   ├── Extensions/
│   │   │   ├── ServiceCollectionExtensions.cs
│   │   │   └── ApplicationBuilderExtensions.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── SentinelaOps.Worker/                   # Worker Background Service
│   │   ├── EventProcessingWorker.cs
│   │   ├── MessageHandlers/
│   │   │   ├── EventReceivedHandler.cs
│   │   │   └── AnalysisCompletedHandler.cs
│   │   ├── HostedServices/
│   │   │   ├── EventProcessingService.cs
│   │   │   ├── MetricsReportingService.cs
│   │   │   └── HealthCheckService.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── SentinelaOps.Harness/                  # Harness (Bounded Context)
│   │   ├── Domain/
│   │   │   ├── Entities/
│   │   │   │   ├── BenchmarkRun.cs
│   │   │   │   └── ComparisonResult.cs
│   │   │   ├── ValueObjects/
│   │   │   │   ├── ModelComparison.cs
│   │   │   │   └── PromptEvaluation.cs
│   │   │   └── Services/
│   │   │       └── IHarnessCoordinator.cs
│   │   ├── Application/
│   │   │   ├── Handlers/
│   │   │   │   ├── StartBenchmarkHandler.cs
│   │   │   │   ├── GenerateComparisonHandler.cs
│   │   │   │   └── EvaluatePromptHandler.cs
│   │   │   └── Services/
│   │   │       └── HarnessService.cs
│   │   └── Infrastructure/
│   │       ├── HarnessOrchestrator.cs
│   │       └── Repositories/
│   │           └── BenchmarkRepository.cs
│   │
│   ├── SentinelaOps.Skills.Abstractions/      # Skills Abstractions
│   │   ├── ISkill.cs
│   │   ├── SkillContext.cs
│   │   ├── SkillResult.cs
│   │   ├── SkillException.cs
│   │   └── SkillRegistry.cs
│   │
│   ├── SentinelaOps.Skills.Perimeter/         # Skill Implementation 1
│   │   ├── PerimeterAnalysisSkill.cs
│   │   ├── PromptTemplate.cs
│   │   └── ResultParser.cs
│   │
│   ├── SentinelaOps.Skills.Intrusion/         # Skill Implementation 2
│   │   ├── IntrusionAnalysisSkill.cs
│   │   └── ResultParser.cs
│   │
│   ├── SentinelaOps.Skills.FalsePositive/     # Skill Implementation 3
│   │   ├── FalsePositiveAnalysisSkill.cs
│   │   └── ResultParser.cs
│   │
│   ├── SentinelaOps.Skills.Severity/          # Skill Implementation 4
│   │   ├── SeverityClassificationSkill.cs
│   │   └── ResultParser.cs
│   │
│   └── SentinelaOps.Skills.Summary/            # Skill Implementation 5
│       ├── IncidentSummarySkill.cs
│       └── ResultParser.cs
│
├── tests/
│   ├── SentinelaOps.Domain.Tests/              # Domain Logic Tests
│   │   ├── Entities/
│   │   │   ├── MonitoringEventTests.cs
│   │   │   └── PromptTests.cs
│   │   ├── ValueObjects/
│   │   │   ├── ConfidenceTests.cs
│   │   │   └── ClassificationTests.cs
│   │   ├── Services/
│   │   │   └── InferenceRequestTests.cs
│   │   └── Specifications/
│   │       └── EventSpecificationTests.cs
│   │
│   ├── SentinelaOps.Application.Tests/        # Application Logic Tests
│   │   ├── Handlers/
│   │   │   ├── ReceiveEventHandlerTests.cs
│   │   │   ├── ExecutePipelineHandlerTests.cs
│   │   │   └── RecordActionHandlerTests.cs
│   │   ├── Services/
│   │   │   ├── EventServiceTests.cs
│   │   │   └── AnalysisServiceTests.cs
│   │   └── Mappers/
│   │       └── EventMapperTests.cs
│   │
│   ├── SentinelaOps.Infrastructure.Tests/     # Infrastructure Tests
│   │   ├── Persistence/
│   │   │   ├── EventRepositoryTests.cs
│   │   │   └── UnitOfWorkTests.cs
│   │   ├── Inference/
│   │   │   └── OllamaInferenceProviderTests.cs
│   │   └── Skills/
│   │       ├── SkillOrchestratorTests.cs
│   │       └── SkillTests.cs
│   │
│   ├── SentinelaOps.Api.Tests/                 # API Integration Tests
│   │   ├── Controllers/
│   │   │   ├── EventsControllerTests.cs
│   │   │   └── AnalysisControllerTests.cs
│   │   ├── Middleware/
│   │   │   └── CorrelationIdMiddlewareTests.cs
│   │   └── ApiTestFixture.cs
│   │
│   ├── SentinelaOps.Architecture.Tests/        # Architecture Tests
│   │   ├── LayerDependencyTests.cs
│   │   ├── DddPatternTests.cs
│   │   ├── NamingConventionTests.cs
│   │   └── CircularDependencyTests.cs
│   │
│   └── SentinelaOps.EndToEnd.Tests/            # E2E Tests
│       ├── EventProcessingE2ETests.cs
│       ├── HarnessE2ETests.cs
│       ├── ApiE2ETests.cs
│       └── Fixtures/
│           ├── DatabaseFixture.cs
│           ├── OllamaFixture.cs
│           └── RabbitMQFixture.cs
│
├── docker-compose.yml                          # Infrastructure (local dev)
├── Dockerfile                                   # API Docker image
├── docker-compose.prod.yml                      # Production config
│
├── docs/                                        # Toda documentação (já criada)
│   ├── vision/
│   ├── requirements/
│   ├── use-cases/
│   ├── domain-model/
│   ├── event-storming/
│   ├── context-map/
│   ├── bounded-contexts/
│   ├── adr/
│   ├── prompts/
│   ├── skills/
│   ├── architecture/
│   ├── roadmap/
│   └── decisions/
│
├── .github/
│   ├── workflows/
│   │   ├── ci.yml                              # Build, test, coverage
│   │   ├── deploy.yml                          # Deploy to staging/prod
│   │   └── release.yml                         # Release management
│   └── CONTRIBUTING.md
│
├── .editorconfig
├── .gitignore
├── .gitattributes
├── Directory.Build.props                       # Propriedades compartilhadas
├── SentinelaOps.sln                            # Solution file
├── README.md
├── LICENSE
└── CHANGELOG.md
```

---

## Convenções de Nomenclatura

### Projects
- `SentinelaOps.[Feature].Domain` (zero dependências)
- `SentinelaOps.[Feature].Application` (depends: Domain)
- `SentinelaOps.[Feature].Infrastructure` (depends: Domain, Application)
- `SentinelaOps.Api` (depends: todos)
- `SentinelaOps.Worker` (depends: todos)

### Namespaces
```
SentinelaOps.Domain
  └─ Entities
  └─ ValueObjects
  └─ Events
  └─ Services
  └─ Repositories
  └─ Specifications
  └─ Exceptions

SentinelaOps.Application
  └─ Handlers
  └─ Services
  └─ DTOs
  └─ Queries
  └─ Commands
  └─ Mappers

SentinelaOps.Infrastructure
  └─ Persistence
  └─ Inference
  └─ Skills
  └─ Messaging
  └─ Observability
```

### Classes
- Handlers: `[Command/Query]Handler`
- Services: `[Name]Service`
- Repositories: `[Entity]Repository`
- Skills: `[Name]Skill`
- Providers: `[Name]Provider`
- Exceptions: `[Name]Exception`

### Interfaces
- `I[Service]` (ex: IInferenceProvider, IEventRepository)

---

## Dependências Between Projects

```
SentinelaOps.Domain
  ↑ (referenced by)
SentinelaOps.Application
  ↑ (referenced by)
SentinelaOps.Infrastructure
  ↑ (referenced by)
SentinelaOps.Api
SentinelaOps.Worker

SentinelaOps.Skills.Abstractions
  ↑ (referenced by)
SentinelaOps.Skills.[Feature]
  ↑ (referenced by)
SentinelaOps.Infrastructure
```

---

## Configuração de Build

### Propriedades Compartilhadas
`Directory.Build.props`:
```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>latest</LangVersion>
    <WarningLevel>4</WarningLevel>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
</Project>
```

### NuGet Packages Principais

**Domain**:
- nenhum (zero dependências!)

**Application**:
- FluentValidation
- MediatR
- AutoMapper

**Infrastructure**:
- EntityFrameworkCore
- EntityFrameworkCore.Sqlite
- RabbitMQ.Client
- OpenTelemetry
- Serilog

**API**:
- AspNetCore
- Swagger
- IdentityModel.Tokens.Jwt

---

## Inicialização do Projeto

```bash
# 1. Clone
git clone <repo>
cd SentinelaOps

# 2. Build
dotnet build

# 3. Run tests
dotnet test

# 4. Start infrastructure
docker-compose up -d

# 5. Run migrations
dotnet run --project src/SentinelaOps.Api -- --migrate

# 6. Start application
dotnet run --project src/SentinelaOps.Api
dotnet run --project src/SentinelaOps.Worker

# 7. Access
API: http://localhost:5000
Swagger: http://localhost:5000/swagger
Jaeger: http://localhost:16686
```

---

## Próximos Passos

1. ✅ Criar slurção
2. ➕ Implementar Domain layer
3. ➕ Implementar Application layer com MediatR
4. ➕ Implementar Infrastructure (EF Core, Repositories)
5. ➕ Implementar API (Controllers, Middleware)
6. ➕ Implementar Worker (background service)
7. ➕ Implementar Skills
8. ➕ Implementar Harness
9. ➕ Testes em todas camadas
10. ➕ Docker Compose para local dev
