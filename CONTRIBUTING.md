# 🤝 Contribuindo para SentinelaOps

Obrigado por se interessar em contribuir! Este documento descreve como começar e as convenções do projeto.

---

## 🚀 Começando Rápido

### 1. Faça um Fork + Clone

```bash
git clone https://github.com/seu-usuario/SentinelaOps.git
cd SentinelaOps
```

### 2. Configure o Ambiente

```bash
# Requisitos
- .NET 8.0 SDK
- Visual Studio Code (recomendado)
- Docker Desktop (para Ollama, RabbitMQ, etc)

# Setup
dotnet build --no-restore
dotnet test --no-build
```

### 3. Inicie os Serviços (Opcional)

```bash
docker-compose up -d
# Aguarde:
# - Ollama: http://localhost:11434
# - RabbitMQ: http://localhost:15672 (guest/guest)
# - Jaeger: http://localhost:16686
# - Prometheus: http://localhost:9090
```

---

## 📋 Entender o Projeto

**OBRIGATÓRIO antes de contribuir** (leia NESTA ORDEM):

1. `README.md` (5 min) - Visão geral
2. `docs/architecture/SOLUTION_STRUCTURE.md` (10 min) - Arquitetura
3. `HOW_TO_RESUME.md` (2 min) - Estado atual
4. `NEXT_STEPS.md` (5 min) - Próximos passos
5. `docs/SPECIFICATION_CONSOLIDATED.md` (opcional, para specs completas)

---

## 🎯 Tipos de Contribuição

### Tier 1: Bug Fixes (Iniciantes)

**Issues marcados com `good-first-issue`**

```
1. Abra issue descrevendo o bug
2. Crie branch: git checkout -b fix/seu-bug
3. Faça o fix com teste
4. Abra PR
```

**Exemplos**:
- Corrigir validações
- Melhorar mensagens de erro
- Adicionar missing null checks

---

### Tier 2: Features Pequenas (Intermediário)

**Issues marcados com `help-wanted`**

```
1. Leia ARCHITECTURE.md para entender fluxo
2. Crie branch: git checkout -b feature/sua-feature
3. Implemente com TDD (testes primeiro)
4. Mínimo 90% cobertura
5. Abra PR com descrição de teste
```

**Exemplos**:
- Novos value objects
- Validações de domínio
- Melhorias em testes

---

### Tier 3: Grandes Features (Avançado)

**Issues marcados com `epic` ou fora da Roadmap**

```
1. Abra DISCUSSION antes de começar
2. Concordar com arquiteto
3. Siga patterns estabelecidos
4. Build + Tests devem passar 100%
```

**Exemplos**:
- Novos aggregates (PromptVersion, InferenceExecution)
- Application Services
- Skills integration

---

## 💻 Convenções de Código

### C# Style Guide

```csharp
// ✅ BOM: PascalCase para classes, interfaces, métodos
public class MonitoringEvent { }
public interface IRepository { }
public void ProcessEvent() { }

// ✅ BOM: camelCase para parâmetros e variáveis
public void Create(string eventName, int priority) { }

// ✅ BOM: CONSTANT_CASE para constantes
private const int MAX_RETRIES = 3;

// ✅ BOM: _camelCase para campos privados
private string _internalState;

// ❌ RUIM: Abreviações
public class Evt { } // ❌
public class Event { } // ✅

// ✅ BOM: Documentação em tudo
/// <summary>
/// Processa evento de monitoramento
/// </summary>
/// <param name="eventId">ID do evento</param>
public void ProcessMonitoringEvent(string eventId) { }
```

### DDD Patterns (OBRIGATÓRIO)

**Value Objects**:
```csharp
// ✅ BOM: Imutável, factory pattern, IEquatable
public class Confidence
{
    public double Value { get; }
    
    public static Confidence Create(double value)
    {
        if (value < 0 || value > 1)
            throw new ArgumentException("Deve estar entre 0 e 1");
        return new Confidence(value);
    }
    
    private Confidence(double value) => Value = value;
    
    public override bool Equals(object? obj) => /* implementar */;
}
```

**Aggregate Roots**:
```csharp
// ✅ BOM: Private constructor, static factory, domain events
public class MonitoringEvent
{
    public EventId Id { get; }
    public EventStatus Status { get; private set; }
    
    public static MonitoringEvent Create(EventId id, CorrelationId correlationId)
    {
        var entity = new MonitoringEvent(id, correlationId);
        entity.RaiseDomainEvent(new EventReceivedDomainEvent(...));
        return entity;
    }
    
    private MonitoringEvent(EventId id, CorrelationId correlationId) { /* ... */ }
}
```

### XML Documentation

```csharp
// OBRIGATÓRIO em todo método/classe público
/// <summary>
/// Valida e cria um novo evento de monitoramento
/// </summary>
/// <param name="eventId">Identificador único do evento</param>
/// <param name="correlationId">ID para rastreamento distribuído</param>
/// <returns>Novo MonitoringEvent com status Received</returns>
/// <exception cref="ArgumentNullException">Se eventId ou correlationId for null</exception>
public static MonitoringEvent Create(EventId eventId, CorrelationId correlationId)
{
    ArgumentNullException.ThrowIfNull(eventId);
    ArgumentNullException.ThrowIfNull(correlationId);
    
    var entity = new MonitoringEvent(eventId, correlationId);
    entity.RaiseDomainEvent(new EventReceivedDomainEvent(eventId, correlationId));
    return entity;
}
```

---

## ✅ Processo de Pull Request

### 1. Preparar Branch

```bash
git checkout main
git pull origin main
git checkout -b feature/sua-feature
```

### 2. Implementar com TDD

```
1. Escrever testes (RED)
2. Implementar código (GREEN)
3. Refatorar (REFACTOR)
```

**Mínimos**:
- ✅ Todos testes passando
- ✅ 90%+ cobertura de testes
- ✅ Zero warnings em build
- ✅ Build: `dotnet build --no-restore` → SUCCESS
- ✅ Tests: `dotnet test --no-build` → ALL PASSED

### 3. Commit com Mensagens Claras

```bash
# ✅ BOM
git commit -m "feat: Add PromptVersion aggregate with validation tests"
git commit -m "fix: Handle null reference in EventId.Parse()"
git commit -m "docs: Update ARCHITECTURE.md with skill orchestration"

# ❌ RUIM
git commit -m "fix stuff"
git commit -m "blah"
git commit -m "ajuste"
```

**Formato Conventional Commits**:
- `feat:` Nova funcionalidade
- `fix:` Correção de bug
- `docs:` Mudanças em documentação
- `refactor:` Refatoração sem mudança funcional
- `test:` Adição de testes
- `chore:` Dependências, build, etc

### 4. Abrir PR com Descrição

```markdown
## 📝 Descrição
Implementei PromptVersion aggregate com versionamento SemVer.

## 🎯 Tipo de Mudança
- [x] Feature
- [ ] Bug Fix
- [ ] Breaking Change

## ✅ Checklist
- [x] Código segue convenções do projeto
- [x] Testes adicionados (90%+ cobertura)
- [x] Build passa: `dotnet build --no-restore`
- [x] Testes passam: `dotnet test --no-build`
- [x] Documentação atualizada
- [x] Zero warnings

## 🧪 Teste Manualmente
1. Clone este branch
2. `dotnet build --no-restore`
3. `dotnet test --no-build`
4. Todos 34 testes devem passar

## 📸 Screenshots (se aplicável)
[Adicione capturas se houver mudanças visuais]
```

### 5. Review + Merge

- Código será reviewado por mantenedores
- Leia feedback com mente aberta
- Faça ajustes conforme solicitado
- Após aprovação, será merged

---

## 🏗️ Arquitetura de Contribuição

### Estrutura de Projetos (18 total)

```
src/
├── SentinelaOps.Domain/              ← Value Objects, Aggregates (Core)
├── SentinelaOps.Application/         ← Commands, Services
├── SentinelaOps.Infrastructure/      ← Persistência, Messaging
├── SentinelaOps.Api/                 ← REST API
├── SentinelaOps.Worker/              ← Background Processing

├── SentinelaOps.Skills.Abstractions/  ← Interfaces de Skills
├── SentinelaOps.Skills.Perimeter/
├── SentinelaOps.Skills.Intrusion/
├── SentinelaOps.Skills.Motion/
├── SentinelaOps.Skills.Vehicle/
├── SentinelaOps.Skills.HumanActivity/
├── SentinelaOps.Skills.FalsePositive/
├── SentinelaOps.Skills.Severity/
├── SentinelaOps.Skills.Summary/

tests/
├── SentinelaOps.Domain.Tests/        ← Testes de domínio (unit)
├── SentinelaOps.Application.Tests/   ← Testes de aplicação
├── SentinelaOps.Integration.Tests/   ← Testes de integração
├── SentinelaOps.Architecture.Tests/  ← Testes de arquitetura
├── SentinelaOps.E2E.Tests/          ← Testes end-to-end
└── SentinelaOps.Harness.Tests/      ← Testes de harness
```

### Camadas de Dependência (Clean Architecture)

```
┌─────────────────────────────┐
│   API / Worker / Harness    │  ← Interface com mundo externo
├─────────────────────────────┤
│   Application               │  ← Commands, Services
├─────────────────────────────┤
│   Domain                    │  ← Lógica pura (DDD)
├─────────────────────────────┤
│   Infrastructure            │  ← Implementação de Ports
└─────────────────────────────┘
```

**Regras Críticas**:
- Domain ❌ não conhece Application/Infrastructure
- Application ✅ conhece Domain
- Infrastructure ✅ implementa interfaces do Domain
- APIs ✅ orquestram Application + Infrastructure

---

## 🔍 Checklist Antes de Submeter PR

```
[ ] Código segue convenções C# do projeto?
[ ] Documentação XML completa em públicos?
[ ] Patterns DDD aplicados corretamente?
[ ] Testes escritos primeiro (TDD)?
[ ] 90%+ cobertura de testes?
[ ] Build passa sem warnings?
[ ] Testes passam 100%?
[ ] Commit messages são claras?
[ ] PR description está completa?
[ ] Sem código comentado ou debug?
[ ] Nenhuma dependência adicionada sem discussão?
```

---

## 📞 Comunicação

- **Dúvidas**: Abra issue com tag `question`
- **Bugs**: Use `bug` template
- **Features**: Use `feature-request` template
- **Discussions**: Use GitHub Discussions

---

## 📖 Recursos Importantes

| Recurso | Para Quem | Tempo |
|---------|-----------|-------|
| [README.md](README.md) | Todos | 5 min |
| [ARCHITECTURE.md](ARCHITECTURE.md) | Devs | 15 min |
| [SOLUTION_STRUCTURE.md](docs/architecture/SOLUTION_STRUCTURE.md) | Devs | 10 min |
| [Prompts de Skills](.github/skills/) | Features | 20 min |
| [NEXT_STEPS.md](NEXT_STEPS.md) | Novos devs | 5 min |

---

## 🎓 Aprendizado Contínuo

Este projeto usa:
- **Domain-Driven Design** - Modelo de domínio em primeiro lugar
- **Spec-Driven Development** - Specs antes de código
- **Clean Architecture** - Separação clara de camadas
- **SOLID Principles** - Código extensível e testável
- **Event-Driven Architecture** - Comunicação por eventos

Se não está familiar, recomendo:
1. Ler `docs/domain-model/DOMAIN_MODEL.md`
2. Estudar code em `src/SentinelaOps.Domain/Core/`
3. Reproduzir padrão em suas contribuições

---

## 🚨 Não Queremos

- ❌ Commits sem testes
- ❌ Breaking changes sem discussão
- ❌ Código sem documentação
- ❌ PRs contra main direto (use branches)
- ❌ Fuzzy mensagens de commit
- ❌ Dependências externas sem aprovação

---

## ✨ Contribuidores Destaque

Contribuições significativas serão destacadas em:
- README.md (seção Contributors)
- CHANGELOG.md
- Release notes

---

**Obrigado por contribuir! 🙏**

Qualquer dúvida, abra uma issue ou discussion. Estamos aqui para ajudar! 🚀
