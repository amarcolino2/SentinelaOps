# SentinelaOps - Video Monitoring Decision Support Platform

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Overview

**SentinelaOps** is an open-source platform for video monitoring decision support that leverages AI-driven analysis to reduce false positives by 70% and accelerate decision times to <2.2 seconds.

The platform serves as an architectural reference implementation for:
- **Domain-Driven Design (DDD)** with bounded contexts and aggregate patterns
- **Event-Driven Architecture** with RabbitMQ pub/sub messaging
- **Clean Architecture** with acyclic dependencies and layered design
- **Composable AI Skills** with extensible analysis pipelines
- **Distributed Systems** with OpenTelemetry observability

## Quick Start

### Prerequisites

- **.NET 8 SDK** ([download](https://dotnet.microsoft.com/download))
- **Docker** and **Docker Compose** ([download](https://www.docker.com/products/docker-desktop))
- **Git**

### Local Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/SentinelaOps/SentinelaOps.git
   cd SentinelaOps
   ```

2. **Start infrastructure services**
   ```bash
   docker-compose up -d
   ```
   
   This starts:
   - **Ollama** (Gemma 3 4B model) on port 11434
   - **RabbitMQ** on ports 5672 (AMQP) and 15672 (UI)
   - **Jaeger** on port 16686 (tracing UI)
   - **Prometheus** on port 9090 (metrics UI)

3. **Download Ollama model** (first time only)
   ```bash
   curl http://localhost:11434/api/pull -d '{"name": "gemma3:4b"}'
   ```

4. **Build the solution**
   ```bash
   dotnet build
   ```

5. **Run all tests**
   ```bash
   dotnet test --logger:"console;verbosity=normal" --collect:"XPlat Code Coverage"
   ```

6. **Start API server** (in terminal)
   ```bash
   cd src/SentinelaOps.Api
   dotnet run
   ```

   API available at `http://localhost:5000` (see Swagger UI at `/swagger`)

7. **Start Worker service** (in another terminal)
   ```bash
   cd src/SentinelaOps.Worker
   dotnet run
   ```

### Observability

Once services are running:

- **Jaeger UI**: [http://localhost:16686](http://localhost:16686) - Trace events across services
- **Prometheus**: [http://localhost:9090](http://localhost:9090) - Query metrics
- **RabbitMQ UI**: [http://localhost:15672](http://localhost:15672) - Monitor message queues
- **API Swagger**: [http://localhost:5000/swagger](http://localhost:5000/swagger) - Explore endpoints

## Project Structure

```
SentinelaOps/
├── src/
│   ├── SentinelaOps.Domain/                    # DDD domain layer (zero external deps)
│   ├── SentinelaOps.Application/               # Use cases & orchestration
│   ├── SentinelaOps.Infrastructure/            # Data, messaging, external services
│   ├── SentinelaOps.Api/                       # ASP.NET Core REST API
│   ├── SentinelaOps.Worker/                    # Background event processor
│   ├── SentinelaOps.Harness/                   # ML model comparison & prompt testing
│   └── SentinelaOps.Skills.*/                  # Composable analysis skills
├── tests/
│   ├── SentinelaOps.Domain.Tests/              # Domain unit tests (>90% coverage)
│   ├── SentinelaOps.Application.Tests/         # Application workflow tests
│   ├── SentinelaOps.Infrastructure.Tests/      # Integration tests
│   ├── SentinelaOps.Api.Tests/                 # API contract tests
│   ├── SentinelaOps.Architecture.Tests/        # Dependency & layering rules
│   └── SentinelaOps.EndToEnd.Tests/            # Behavioral flow tests
├── docs/                                       # Architecture, design, specifications
├── monitoring/                                 # Prometheus config
└── docker-compose.yml                          # Local infrastructure

```

## Architecture

### Bounded Contexts

| Context | Purpose | Ports |
|---------|---------|-------|
| **Event Receiver** | Ingest and validate monitoring events | ONVIF, RTSP, HTTP adapters |
| **Skill Orchestrator** | Route events through analysis pipeline | Pipeline coordination |
| **Inference Provider** | AI model abstraction (Ollama, Claude, GPT-4V) | `IInferenceProvider` interface |
| **Skills** | Composable analysis modules (5 initial) | Perimeter, Intrusion, FalsePositive, Severity, Summary |
| **Persistence** | Data storage (SQLite→PostgreSQL roadmap) | Repository interfaces |
| **API** | REST endpoints for clients | ASP.NET Core |
| **Worker** | Async event processing | RabbitMQ consumer |
| **Harness** | Model & prompt testing/comparison | Data-driven decisions |

### Key Design Patterns

- **Domain-Driven Design**: Aggregates, value objects, domain events, ubiquitous language
- **Event-Driven**: RabbitMQ pub/sub for context decoupling
- **CQRS**: Separated command and query responsibilities
- **Repository Pattern**: Data access abstraction
- **Ports & Adapters**: External system integration contracts
- **Dependency Injection**: Constructor injection with DI container

## API Examples

### Health Check
```bash
curl http://localhost:5000/health
```

### Submit Event for Analysis
```bash
curl -X POST http://localhost:5000/api/v1/events \
  -H "Content-Type: application/json" \
  -d '{
    "cameraId": "cam-001",
    "timestamp": "2024-01-15T10:30:00Z",
    "videoUrl": "rtsp://camera.local/stream",
    "metadata": {"zone": "perimeter", "sensitivity": "high"}
  }'
```

### Get Analysis Results
```bash
curl http://localhost:5000/api/v1/events/{eventId}/analysis
```

### Get Metrics
```bash
curl http://localhost:9090/api/v1/query?query=sentinelaops_event_processing_duration_seconds
```

## Testing

### Run all tests
```bash
dotnet test
```

### Run tests with coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura
```

### Run specific test project
```bash
dotnet test tests/SentinelaOps.Domain.Tests
```

### Run architecture tests (validate layering rules)
```bash
dotnet test tests/SentinelaOps.Architecture.Tests
```

## Documentation

- [Vision & Strategy](docs/vision/) - Strategic positioning and objectives
- [Requirements](docs/requirements/) - Functional and non-functional requirements
- [Domain Model](docs/domain-model/) - DDD aggregates and value objects
- [Architecture](docs/architecture/) - Decision records, AI strategy, solution structure
- [Roadmap](docs/roadmap/) - 3-phase implementation plan

**[Full Documentation Index](docs/README.md)**

## Development Workflow

1. **Create feature branch**
   ```bash
   git checkout -b feature/add-xyz-skill
   ```

2. **Write tests first** (TDD)
   - Domain tests in `Domain.Tests/`
   - Application tests in `Application.Tests/`

3. **Implement feature** following ADRs
   - Reference ADRs in commit messages
   - Maintain rastreability: code ↔ design ↔ requirements

4. **Run full test suite**
   ```bash
   dotnet test --logger:"console;verbosity=normal"
   ```

5. **Create pull request**
   - Link to issue/requirement
   - Reference architecture decision record (ADR)

## Performance Targets

| Metric | Target | Status |
|--------|--------|--------|
| Event processing latency | <2.2s (p99) | 🎯 Target |
| False positive reduction | 70% | 🎯 Target |
| Model inference latency | <2s @ 4B params | 🎯 Ollama baseline |
| Throughput | 100 events/sec | 🎯 Phase 2 |
| Availability | 99.9% uptime | 🎯 Phase 2 |
| Test coverage | >80% domain layer | 🎯 Phase 1 |

## Roadmap

### Phase 1: MVP (Weeks 1-16)
- ✅ Specification & architecture (complete)
- 🔄 **Infrastructure setup** (current)
- ⬜ Domain layer implementation
- ⬜ Event-driven worker
- ⬜ 5 initial skills
- ⬜ Harness platform
- ⬜ Full test coverage
- ⬜ Docker containerization

### Phase 2: Production Ready (Weeks 17-28)
- PostgreSQL migration
- Kubernetes deployment
- Distributed tracing at scale
- Performance optimization

### Phase 3: Evolution (Ongoing)
- Additional AI models (Qwen, Llama, Claude)
- Advanced skill composition
- Mobile dashboard

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

MIT License - see [LICENSE](LICENSE) file

## Contact

**Team**: SentinelaOps Development Team  
**Issues**: [GitHub Issues](https://github.com/SentinelaOps/SentinelaOps/issues)

---

**Last Updated**: January 2024  
**Version**: 0.1.0 (MVP Phase)
