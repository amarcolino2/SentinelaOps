Você atuará como Arquiteto de Software Principal, Especialista em .NET 8, Inteligência Artificial, Sistemas Distribuídos, Domain-Driven Design (DDD), Clean Architecture, Event-Driven Architecture, Spec-Driven Development (SDD) e Harness Engineering.

# Projeto

Nome: Sentinela Ops

Objetivo:

Desenvolver uma plataforma Open Source de apoio à decisão operacional para ambientes de videomonitoramento.

A plataforma deverá reduzir falsos positivos através da análise inteligente de eventos utilizando Inteligência Artificial multimodal.

O foco não é substituir operadores humanos, mas fornecer contexto, classificação, justificativas, recomendações e confiança da análise para acelerar a tomada de decisão.

O projeto deverá servir como referência pública de arquitetura moderna, IA aplicada, sistemas distribuídos e desenvolvimento orientado por especificação.

---

# Conceito Central

Receber eventos provenientes de qualquer sistema de monitoramento capaz de fornecer:

* Imagens JPEG
* Metadados
* Eventos analíticos
* Eventos de perímetro
* Eventos de intrusão
* Eventos personalizados

A plataforma deverá analisar o contexto completo do evento e produzir:

* Classificação
* Nível de confiança
* Justificativa
* Resumo operacional
* Recomendação
* Evidências utilizadas

---

# Pilares Fundamentais

O projeto deverá ser construído sobre quatro pilares obrigatórios:

1. Domain-Driven Design (DDD)
2. Spec-Driven Development (SDD)
3. Harness Engineering
4. Skills-Based AI Architecture

Esses pilares devem orientar todas as decisões arquiteturais.

---

# Spec-Driven Development (SDD)

Nenhuma implementação deverá ser iniciada antes da produção da especificação.

A especificação deverá ser considerada a fonte primária do projeto.

Criar a seguinte estrutura:

docs/

├── vision
├── requirements
├── use-cases
├── domain-model
├── event-storming
├── context-map
├── bounded-contexts
├── adr
├── prompts
├── skills
├── architecture
├── roadmap
└── decisions

Antes de gerar qualquer código produzir:

1. Visão do Produto
2. Problema de Negócio
3. Objetivos
4. Requisitos Funcionais
5. Requisitos Não Funcionais
6. Casos de Uso
7. Event Storming
8. Context Map
9. Bounded Contexts
10. Modelo de Domínio
11. ADRs
12. Diagramas Mermaid
13. Estratégia de IA
14. Estratégia de Skills
15. Estratégia de Harness
16. Estrutura da Solução
17. Roadmap Evolutivo

Somente após a conclusão da especificação iniciar a implementação.

---

# Harness Engineering

A plataforma deverá possuir um Harness dedicado para experimentação, benchmark, validação e comparação de modelos.

O Harness é parte obrigatória da arquitetura.

Objetivos:

* Comparar modelos
* Comparar prompts
* Comparar versões de prompts
* Comparar Skills
* Avaliar precisão
* Avaliar confiança
* Avaliar latência
* Avaliar custo computacional
* Avaliar consistência das respostas

Criar o Bounded Context:

Inference Harness

Criar:

* PromptCatalog
* PromptVersion
* PromptTemplate
* EvaluationScenario
* EvaluationDataset
* InferenceRun
* InferenceResult
* InferenceMetrics
* BenchmarkResult
* ModelRegistry

O Harness deverá permitir executar um mesmo evento contra múltiplos modelos.

Exemplo:

Evento

→ Gemma 3
→ Qwen VL
→ Llama Vision
→ OpenAI

Comparar:

* Resultado
* Confiança
* Tempo de resposta
* Tokens
* Consumo de recursos
* Precisão

Toda inferência deverá ser auditável.

---

# Skills-Based AI Architecture

A análise deverá ser organizada através de Skills independentes.

Criar abstrações:

ISkill

SkillRequest

SkillResult

SkillContext

SkillRegistry

SkillPipeline

SkillOrchestrator

As Skills não poderão depender de modelos específicos.

Implementar inicialmente:

* PerimeterAnalysisSkill
* IntrusionAnalysisSkill
* MotionAnalysisSkill
* HumanActivityAnalysisSkill
* VehicleAnalysisSkill
* FalsePositiveAnalysisSkill
* SeverityClassificationSkill
* IncidentSummarySkill

Permitir composição de Skills.

Exemplo:

Evento

↓

PerimeterAnalysisSkill

↓

FalsePositiveAnalysisSkill

↓

SeverityClassificationSkill

↓

IncidentSummarySkill

↓

Resultado Final

Aplicar Open/Closed Principle.

Novas Skills deverão ser adicionadas sem modificar as existentes.

---

# Tecnologias Iniciais

Backend:

* .NET 8
* ASP.NET Core
* Worker Services

Mensageria:

* RabbitMQ

Persistência:

* SQLite

IA:

* Ollama
* Gemma 3 4B

Containerização:

* Docker
* Docker Compose

Observabilidade:

* OpenTelemetry

---

# Evolução Futura

A arquitetura deverá permitir substituição sem impacto no domínio.

IA:

* Gemma
* Qwen VL
* Llama Vision
* OpenAI
* Azure OpenAI
* Anthropic
* Mistral
* Provedores futuros

Mensageria:

* RabbitMQ
* Kafka
* Azure Service Bus
* AWS SQS

Persistência:

* SQLite
* SQL Server
* PostgreSQL
* Oracle
* MongoDB

Monitoramento:

* ONVIF
* RTSP
* APIs Proprietárias
* Sistemas Corporativos
* Integrações futuras

---

# Arquitetura

Aplicar rigorosamente:

* DDD
* Clean Architecture
* Hexagonal Architecture
* SOLID
* Clean Code
* CQRS quando aplicável
* Event-Driven Architecture
* Dependency Inversion Principle
* Ports and Adapters

---

# Abstração de IA

O domínio não poderá conhecer:

* Ollama
* Gemma
* Qwen
* OpenAI
* Azure OpenAI
* Qualquer modelo específico

Criar:

IInferenceProvider

InferenceRequest

InferenceResult

ModelConfiguration

PromptTemplate

PromptVersion

InferenceContext

InferenceMetadata

Implementação inicial:

Ollama

Modelo inicial:

Gemma 3 4B

Configuração:

{
"Inference": {
"Provider": "Ollama",
"Model": "gemma3:4b"
}
}

A troca para:

{
"Inference": {
"Provider": "Ollama",
"Model": "qwen2.5-vl:7b"
}
}

não deverá exigir alterações no domínio nem na aplicação.

---

# Abstração de Monitoramento

Criar:

IMonitoringEventProvider

MonitoringEvent

EventMetadata

EventImage

EventSource

EventType

EventContext

Todos os sistemas externos deverão ser adaptados para um modelo canônico interno.

Nenhuma dependência direta de fabricante será permitida.

---

# Abstração de Persistência

O domínio não poderá conhecer:

* SQLite
* SQL Server
* PostgreSQL
* Oracle
* MongoDB

Criar:

Repositories

Unit Of Work

Specifications

Implementação inicial:

SQLite

Implementações futuras deverão ocorrer sem alteração no domínio.

---

# Fluxo Esperado

Evento

↓

Monitoring Provider

↓

Mensageria

↓

Worker

↓

Skill Orchestrator

↓

Inference Provider

↓

Classificação

↓

Persistência

↓

API

↓

Operador

---

# Classificações

A IA deverá classificar eventos em:

* Valid
* PossibleFalsePositive
* Suspicious
* HumanReviewRequired
* Inconclusive

---

# Observabilidade

Implementar:

* OpenTelemetry
* Structured Logging
* Correlation Id
* Distributed Tracing
* Metrics
* Health Checks
* Dashboards

---

# Segurança

Implementar:

* JWT
* RBAC
* Auditoria
* Histórico de inferências
* Histórico de prompts
* Versionamento de prompts
* Rastreabilidade completa

---

# Qualidade

Implementar:

* Testes Unitários
* Testes de Integração
* Testes de Contrato
* Testes de Arquitetura
* Testes End-to-End
* Testes de Harness
* Benchmark de modelos

---

# Estrutura da Solução

src/

├── SentinelaOps.Domain
├── SentinelaOps.Application
├── SentinelaOps.Infrastructure
├── SentinelaOps.Api
├── SentinelaOps.Worker

├── SentinelaOps.Harness
├── SentinelaOps.Harness.Domain
├── SentinelaOps.Harness.Application

├── SentinelaOps.Skills
├── SentinelaOps.Skills.Abstractions
├── SentinelaOps.Skills.Perimeter
├── SentinelaOps.Skills.Intrusion
├── SentinelaOps.Skills.Motion
├── SentinelaOps.Skills.Vehicle
├── SentinelaOps.Skills.FalsePositive
├── SentinelaOps.Skills.Severity
├── SentinelaOps.Skills.Summary

tests/

├── UnitTests
├── IntegrationTests
├── ArchitectureTests
├── HarnessTests
├── EndToEndTests

Produza inicialmente toda a especificação arquitetural, documentação técnica e modelagem do domínio.

Não gere código até concluir completamente a etapa de Spec-Driven Development.

Após a conclusão da especificação, implemente a solução incrementalmente, justificando cada decisão arquitetural adotada.
