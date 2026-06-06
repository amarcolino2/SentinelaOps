# ⚡ PRÓXIMOS PASSOS - SentinelaOps

**Leia isto primeiro quando retomar** (2 minutos)

---

## 🔴 ÚLTIMO STATUS CONFIRMADO

**Data**: 2026-06-06  
**Etapa Completa**: 1.3 - Domain Layer Core  
**Status**: ✅ BUILD OK + 24/24 TESTS PASSING  

```
✅ Feito:
   - EventId, CorrelationId, Confidence, Classification, Justification
   - EventMetadata, EventSensitivity
   - MonitoringEvent Aggregate Root
   - 3 Domain Events
   - IMonitoringEventRepository
   - 24 unit tests (all passing)
   - Full solution compiles (18/18 projects)
```

---

## 🟢 PRÓXIMO PASSO IMEDIATO

### Step 1: Create PromptVersion Aggregate

**Arquivo**: `src/SentinelaOps.Domain/Core/PromptVersion.cs`

**Estrutura Mínima**:
```csharp
public class PromptVersion
{
    public PromptVersionId Id { get; }
    public CorrelationId CorrelationId { get; }
    public string Content { get; }
    public SemanticVersion Version { get; }
    public PromptVersionStatus Status { get; private set; }
    public PromptVersionMetrics Metrics { get; }
    public DateTime CreatedAt { get; }
    public DateTime? ActivatedAt { get; private set; }
    
    // Factory pattern
    public static PromptVersion Create(PromptVersionId id, string content, SemanticVersion version)
    
    // Lifecycle
    public void Activate()
    public void Deactivate()
    public void Rollback(PromptVersionId previousVersionId)
    public void UpdateMetrics(PromptVersionMetrics metrics)
    
    // Domain events
    private void RaiseDomainEvent(DomainEvent @event)
}

public enum PromptVersionStatus { Draft = 0, Active = 1, Inactive = 2 }
```

**Tempo Estimado**: 20 minutos

---

### Step 2: Create IPromptVersionRepository

**Arquivo**: `src/SentinelaOps.Domain/Core/IPromptVersionRepository.cs`

```csharp
public interface IPromptVersionRepository
{
    Task AddAsync(PromptVersion promptVersion, CancellationToken cancellationToken = default);
    Task<PromptVersion?> GetByIdAsync(PromptVersionId id, CancellationToken cancellationToken = default);
    Task<PromptVersion?> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PromptVersion>> GetHistoryAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(PromptVersion promptVersion, CancellationToken cancellationToken = default);
    Task DeleteAsync(PromptVersionId id, CancellationToken cancellationToken = default);
}
```

**Tempo Estimado**: 5 minutos

---

### Step 3: Create Unit Tests

**Arquivo**: `tests/SentinelaOps.Domain.Tests/PromptVersionTests.cs`

**Mínimo de Testes**: 8 tests
- Create_WithValidInput
- Activate_ChangesStatus
- Activate_RaisesDomainEvent
- Deactivate_ChangesStatus
- Rollback_ToVersionV1
- UpdateMetrics_UpdatesValues
- GetMetrics_ReturnsSnapshot

**Tempo Estimado**: 15 minutos

---

### Step 4: Build & Test

```bash
cd d:\Estudos\Dev_Assistida_IA\SentinelaOps

# Build
dotnet build src/SentinelaOps.Domain/SentinelaOps.Domain.csproj --no-restore

# Test
dotnet test tests/SentinelaOps.Domain.Tests/SentinelaOps.Domain.Tests.csproj --no-build

# Expected: SUCCESS + 24+ tests passing
```

**Tempo Estimado**: 10 minutos

---

## 🔵 DEPOIS DO STEP 1-4

Repetar os mesmos 4 passos para:

1. **InferenceExecution Aggregate**
   - Arquivo: `src/SentinelaOps.Domain/Core/InferenceExecution.cs`
   - Similar structure a PromptVersion
   - Properties: InferenceExecutionId, PromptVersionId, Status, Input, Output, Latency, TokensUsed
   - Methods: Create, StartExecution, CompleteExecution, FailExecution
   - Tempo: 45 minutos (4 steps)

2. **Application Services** (Etapa 1.5)
   - Arquivo: `src/SentinelaOps.Application/Commands/ProcessEventCommandHandler.cs`
   - Orquestra: EventProcessingService + 8 Skills
   - Tempo: 2 horas

3. **Skills Integration** (Etapa 1.6)
   - Já têm .github/skills/[name]/SKILL.md pronto
   - Apenas criar application service handlers
   - Tempo: 1.5 horas

---

## 🔧 VERIFICAÇÃO RÁPIDA

```bash
# Verificar que tudo está ok antes de começar
cd d:\Estudos\Dev_Assistida_IA\SentinelaOps

# 1. Build (esperado: 18 projects SUCCESS)
dotnet build --no-restore

# 2. Tests (esperado: 24/24 PASSED)
dotnet test --no-build
```

---

## 📞 ERROS COMUNS

| Erro | Solução |
|------|---------|
| CS1591 (missing XML) | Adicionar `/// <summary>` no agregado |
| "cannot find type" | Verificar namespace em PromptVersion.cs |
| Build falha | Execute `dotnet clean` depois `dotnet build` |
| Tests não encontram | Garantir ProjectReference em .csproj |

---

## 🔗 REFERÊNCIAS

- Pattern Example: `src/SentinelaOps.Domain/Core/MonitoringEvent.cs` (look here!)
- Test Example: `tests/SentinelaOps.Domain.Tests/DomainTests.cs` (copy this pattern)
- Spec: `docs/domain-model/DOMAIN_MODEL.md` (PromptVersion + InferenceExecution specs)
- Architecture: `docs/architecture/SOLUTION_STRUCTURE.md` (project layout)

---

**Quick Start**: Read this file → Run `dotnet build --no-restore` → Create PromptVersion.cs → Done! 🚀
