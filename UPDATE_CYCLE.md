# 📋 Como Atualizar Arquivos de Continuidade

**QUANDO**: Ao completar cada etapa (1.3, 1.4, 1.5, etc)  
**QUANTO TEMPO**: 5 minutos  
**AUTOMÁTICO?**: Parcialmente - use este template

---

## 🔄 Template de Atualização (Copie e Cole)

### Ao Completar Uma Etapa

**Exemplo**: Você completou Etapa 1.4 (PromptVersion + InferenceExecution)

#### 1️⃣ Atualize `NEXT_STEPS.md`

Substitua esta seção:
```markdown
## 🔴 ÚLTIMO STATUS CONFIRMADO

**Data**: [DATA_ANTIGA]
**Etapa Completa**: [ETAPA_ANTIGA]
**Status**: [STATUS_ANTIGO]
```

Por isto:
```markdown
## 🔴 ÚLTIMO STATUS CONFIRMADO

**Data**: [DATA_NOVA] ← Mude para hoje
**Etapa Completa**: 1.4 - PromptVersion + InferenceExecution ← Mude para o que completou
**Status**: ✅ BUILD OK + 24+X TESTS PASSING ← Mude o número de testes
```

---

#### 2️⃣ Atualize `NEXT_STEPS.md` → Seção "PRÓXIMO PASSO IMEDIATO"

Substitua:
```markdown
### Step 1: Create PromptVersion Aggregate
```

Por:
```markdown
### Step 1: Create Application Services
```

E atualize:
- Nome do arquivo
- Estrutura mínima de código
- Tempo estimado

---

#### 3️⃣ Atualize `HOW_TO_RESUME.md` → Tabela "Estado Atual"

Substitua:
```markdown
| Etapas Completadas | 1.1, 1.2, 1.3 ✅ |
| Próximas Etapas | 1.4 (PromptVersion), 1.5 (InferenceExecution) |
```

Por:
```markdown
| Etapas Completadas | 1.1, 1.2, 1.3, 1.4 ✅ |
| Próximas Etapas | 1.5 (Application Services), 1.6 (Skills Integration) |
```

---

#### 4️⃣ Atualize `IMPLEMENTATION_PROGRESS.md` → Adicione Seção Nova

Adicione NO FINAL da seção de etapas:

```markdown
---

### ✅ Etapa 1.4 - PromptVersion + InferenceExecution Aggregates

**Status**: COMPLETO

**Implementado**:
- PromptVersion.cs (value object + aggregate root)
- IPromptVersionRepository.cs
- InferenceExecution.cs (value object + aggregate root)
- IInferenceExecutionRepository.cs
- 10+ unit tests (all passing)

**Build Status**: ✅ 18/18 projects
**Test Status**: ✅ 34/34 tests passing (Domain.Tests)
**Data**: 2026-06-XX ← Coloque a data de hoje
```

---

## ⏱️ Processo Rápido (5 min)

```
1. Etapa completada? 
   ↓
2. Execute: dotnet build --no-restore && dotnet test --no-build
   ↓
3. Abra NEXT_STEPS.md
   → Encontre "ÚLTIMO STATUS CONFIRMADO"
   → Atualize data + etapa + número de testes
   ↓
4. Encontre "PRÓXIMO PASSO IMEDIATO"
   → Mude para próxima etapa
   ↓
5. Abra HOW_TO_RESUME.md
   → Atualize tabela "Estado Atual"
   ↓
6. Abra IMPLEMENTATION_PROGRESS.md
   → Adicione nova seção com etapa completada
   ↓
7. DONE! ✅
```

---

## 📊 O que Muda em Cada Arquivo

### `.copilot-instructions.md`
- **Muda?** ❌ NÃO - deixe como está
- **Por quê?** Instruções são genéricas

### `HOW_TO_RESUME.md`
- **Muda?** ✅ SIM - tabela "Estado Atual"
- **Quanto?** 2 linhas
- **Tempo**: 1 minuto

### `NEXT_STEPS.md`
- **Muda?** ✅ SIM - praticamente tudo
- **Quanto?** ~80% do arquivo
- **Tempo**: 3 minutos

### `IMPLEMENTATION_PROGRESS.md`
- **Muda?** ✅ SIM - adiciona nova seção
- **Quanto?** +1 seção por etapa
- **Tempo**: 1 minuto

---

## 📝 Checklist de Atualização

```
Ao completar cada etapa:

[ ] Rodei `dotnet build --no-restore` com sucesso?
[ ] Rodei `dotnet test --no-build` com sucesso?
[ ] Atualizei "ÚLTIMO STATUS CONFIRMADO" em NEXT_STEPS.md?
[ ] Atualizei "PRÓXIMO PASSO IMEDIATO" em NEXT_STEPS.md?
[ ] Atualizei tabela em HOW_TO_RESUME.md?
[ ] Adicionei seção de conclusão em IMPLEMENTATION_PROGRESS.md?
```

---

## 🤖 AUTOMAÇÃO (Opcional)

Se quiser **TOTALMENTE automático**, crie um script PowerShell:

```powershell
# update-continuity.ps1
param(
    [string]$CompletedStage = "1.4",
    [string]$NextStage = "1.5",
    [int]$TestCount = 34,
    [string]$DateCompleted = (Get-Date -Format "2026-MM-dd")
)

# Atualiza HOW_TO_RESUME.md
(Get-Content "HOW_TO_RESUME.md") -replace 
    "Etapas Completadas.*", 
    "Etapas Completadas | 1.1, 1.2, 1.3, $CompletedStage ✅ |" |
    Set-Content "HOW_TO_RESUME.md"

# Atualiza NEXT_STEPS.md
(Get-Content "NEXT_STEPS.md") -replace 
    "Data.*", 
    "Data**: $DateCompleted" |
    Set-Content "NEXT_STEPS.md"

Write-Host "✅ Continuity files updated!"
```

**Executar**:
```bash
.\update-continuity.ps1 -CompletedStage "1.4" -NextStage "1.5" -TestCount 34
```

---

## 🎯 Resposta Direta

**Frequência de Atualização**:
- ✅ **A cada etapa completada** (1.3 → 1.4 → 1.5, etc)
- ✅ **Depois de `dotnet test` passar**
- ✅ **Antes de iniciar nova etapa**

**Esforço**:
- 📊 ~5 minutos de trabalho manual
- 🤖 Pode ser 100% automático com script

**Benefício**:
- ✨ Continuidade perfeita para qualquer IA
- 🎯 Zero confusão sobre "onde estamos"
- ⚡ Pode iniciar/parar a qualquer momento

Quer que eu crie o **script PowerShell automático**? 🚀
