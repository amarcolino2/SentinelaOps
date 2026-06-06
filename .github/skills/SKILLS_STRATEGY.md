# Estratégia de Skills

## Visão Geral

Skills são unidades independentes de análise. Cada Skill resolve um aspecto específico do problema de análise de eventos.

---

## Arquitetura de Skills

### Interface ISkill

```csharp
public interface ISkill
{
    // Identificação
    string Name { get; }
    string Description { get; }
    string Version { get; }
    
    // Execução
    Task<SkillResult> ExecuteAsync(SkillContext context, CancellationToken cancellation);
    
    // Validação
    bool CanHandle(SkillContext context);
}

public class SkillContext
{
    public MonitoringEvent Event { get; set; }
    public Dictionary<string, object> PreviousResults { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
    public ILogger Logger { get; set; }
}

public class SkillResult
{
    public string SkillName { get; set; }
    public bool Success { get; set; }
    public object Output { get; set; }
    public double? Confidence { get; set; }
    public string Justification { get; set; }
    public List<string> Evidence { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}
```

---

## Skills Iniciais (Fase 1)

### 1. PerimeterAnalysisSkill

**Responsabilidade**: Classificar se evento está dentro ou fora do perímetro definido

**Entrada**:
- Image (JPEG)
- Zone (configuração de zona)
- PerimeterDefinition (polígono ou regras)

**Saída**:
```json
{
  "skillName": "PerimeterAnalysisSkill",
  "classification": "inside|outside|uncertain",
  "confidence": 0.92,
  "justification": "Pessoa claramente dentro da zona de perímetro",
  "evidence": [
    "Posição dentro dos limites definidos",
    "Distância estimada: 3-5m",
    "Angulaçãoconsistente com câmera"
  ],
  "metadata": {
    "position_x": 0.45,
    "position_y": 0.52,
    "distance_estimated": "4m"
  }
}
```

**Lógica**:
1. Recebe imagem + zona
2. Chama Inference Provider com prompt
3. Parseia resultado JSON
4. Valida classification válida
5. Retorna SkillResult

**Implementação**: `SentinelaOps.Skills.Perimeter/PerimeterAnalysisSkill.cs`

---

### 2. IntrusionAnalysisSkill

**Responsabilidade**: Classificar se evento constitui intrusão (entrada não autorizada)

**Entrada**:
- Image
- Resultados de PerimeterAnalysis
- Histórico de zona
- Configuração de horários

**Saída**:
```json
{
  "classification": "intrusion|authorized_entry|suspicious|unclear",
  "confidence": 0.87,
  "indicators": ["unauthorized_movement", "boundary_crossing"],
  "risk_level": "high|medium|low"
}
```

**Lógica**:
1. Se não está dentro (de PerimeterAnalysis), retorna authorized_entry
2. Se está dentro, analisa tipo de movimento
3. Verifica histórico de zona
4. Verifica se horário permite entrada

**Implementação**: `SentinelaOps.Skills.Intrusion/IntrusionAnalysisSkill.cs`

---

### 3. FalsePositiveAnalysisSkill

**Responsabilidade**: Analisar probabilidade de ser falso positivo

**Entrada**:
- Image
- Resultados de skills anteriores
- Características de zona

**Saída**:
```json
{
  "probability_false_positive": 0.15,
  "likely_causes": ["movimento_folhas", "sombra_dinamica"],
  "confidence": 0.88,
  "recommendation": "likely_real_event"
}
```

**Detecção de Padrões**:
- ❌ Movimento de árvores/folhas
- ❌ Reflexo de luz solar
- ❌ Sombras dinâmicas
- ❌ Animais pequenos (gatos, pássaros)
- ❌ Artefatos de câmera
- ❌ Chuva/neve

**Implementação**: `SentinelaOps.Skills.FalsePositive/FalsePositiveAnalysisSkill.cs`

---

### 4. SeverityClassificationSkill

**Responsabilidade**: Classificar severidade do evento

**Entrada**:
- Análise completa até este ponto
- Zona sensitive config
- Histórico de zona

**Saída**:
```json
{
  "severity_level": "critical|high|medium|low|informational",
  "score": 0.85,
  "factors": [
    "intrusion_detected: +0.3",
    "zone_sensitive: +0.2",
    "probability_false_positive: -0.1",
    "recent_history: +0.25"
  ]
}
```

**Fatores de Cálculo**:
- Tipo de evento (intrusão > movimento > irrelevante)
- Localização (zona sensível > zona normal)
- Padrão (primeiro evento > série repetida)
- Histórico (zona problemática > zona calma)

**Implementação**: `SentinelaOps.Skills.Severity/SeverityClassificationSkill.cs`

---

### 5. IncidentSummarySkill

**Responsabilidade**: Gerar resumo operacional em linguagem natural

**Entrada**:
- Análise completa de todos skills

**Saída**:
```json
{
  "summary": "Pessoa detectada dentro do perímetro da zona 5 com alta confiança (94%). Sem indicadores de falso positivo. Classificação: INTRUSÃO. Recomendação: investigação imediata.",
  "recommended_action": "escalate|investigate|dismiss|review",
  "priority": "urgent|high|normal|low",
  "key_points": [
    "Entrada não autorizada confirmada",
    "Confiança da análise: 94%",
    "Histórico: segunda ocorrência nesta zona este mês"
  ]
}
```

**Linguagem Natural**:
- Claro e conciso
- Sem jargão técnico
- Actionable para operador
- < 100 palavras

**Implementação**: `SentinelaOps.Skills.Summary/IncidentSummarySkill.cs`

---

## Pipeline de Skills

### Configuração

```yaml
# appsettings.json
SkillsPipelines:
  Default:
    - Name: PerimeterAnalysisSkill
      Timeout: 5
      Required: true
    - Name: FalsePositiveAnalysisSkill
      Timeout: 5
      Required: true
    - Name: SeverityClassificationSkill
      Timeout: 5
      Required: false
    - Name: IncidentSummarySkill
      Timeout: 5
      Required: false
  
  Detailed:
    - PerimeterAnalysisSkill
    - IntrusionAnalysisSkill
    - MotionAnalysisSkill
    - FalsePositiveAnalysisSkill
    - SeverityClassificationSkill
    - IncidentSummarySkill
```

### Execução

```csharp
public class SkillOrchestrator
{
    public async Task<PipelineResult> ExecutePipelineAsync(
        MonitoringEvent evt,
        string pipelineName,
        CancellationToken cancellation)
    {
        var config = _config.GetPipeline(pipelineName);
        var result = new PipelineResult();
        var context = new SkillContext { Event = evt };
        
        foreach (var skillConfig in config.Skills)
        {
            try
            {
                var skill = _registry.GetSkill(skillConfig.Name);
                
                var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
                cts.CancelAfter(skillConfig.Timeout);
                
                var skillResult = await skill.ExecuteAsync(context, cts.Token);
                
                result.AddSkillResult(skillResult);
                context.PreviousResults[skillConfig.Name] = skillResult.Output;
            }
            catch (OperationCanceledException)
            {
                result.AddSkillFailure(skillConfig.Name, "Timeout");
            }
            catch (Exception ex)
            {
                if (skillConfig.Required) throw;
                result.AddSkillFailure(skillConfig.Name, ex.Message);
            }
        }
        
        return result;
    }
}
```

---

## Skills Futuras (Fase 2+)

### MotionAnalysisSkill
- Analisar tipo e padrão de movimento
- Detectar: humano, veículo, animal
- Entropia de movimento

### HumanActivityAnalysisSkill
- Classificar tipo de atividade humana
- Detectar: caminhando, correndo, parado, posturas suspeitas
- Análise de gestos

### VehicleAnalysisSkill
- Classificar tipo de veículo
- Detectar: carro, moto, bicicleta, truck
- Análise de velocidade e trajetória

### BehaviorAnalysisSkill
- Analisar comportamento anormal
- Detectar: comportamento suspeito, loitering, padrões incomuns

---

## Como Implementar Nova Skill

### Passo 1: Criar Projeto
```bash
mkdir src/SentinelaOps.Skills.MyFeature
cd src/SentinelaOps.Skills.MyFeature
dotnet new classlib
# Adicionar referência: SentinelaOps.Skills.Abstractions
```

### Passo 2: Implementar ISkill
```csharp
public class MyFeatureAnalysisSkill : ISkill
{
    public string Name => "MyFeatureAnalysisSkill";
    public string Description => "Analisa aspecto específico";
    public string Version => "1.0.0";
    
    public async Task<SkillResult> ExecuteAsync(SkillContext context, CancellationToken cancellation)
    {
        // 1. Validar entrada
        if (!CanHandle(context))
            return SkillResult.Failure("Contexto inadequado");
        
        // 2. Executar análise (chamar Inference Provider)
        var result = await _inferenceProvider.InferAsync(request, cancellation);
        
        // 3. Parsear resultado
        var parsed = ParseResult(result.Output);
        
        // 4. Retornar
        return SkillResult.Success(Name, parsed.Classification, parsed.Confidence);
    }
    
    public bool CanHandle(SkillContext context) => true;
}
```

### Passo 3: Registrar Skill
```csharp
// No SkillRegistry
registry.Register<MyFeatureAnalysisSkill>();
```

### Passo 4: Adicionar ao Pipeline
```yaml
SkillsPipelines:
  Default:
    - ...existing...
    - Name: MyFeatureAnalysisSkill
      Timeout: 5
```

### Passo 5: Testar
```csharp
// tests/SentinelaOps.Skills.MyFeature.Tests/MyFeatureSkillTests.cs
[Fact]
public async Task ExecuteAsync_ShouldClassifyCorrectly()
{
    // Arrange
    var skill = new MyFeatureAnalysisSkill(...);
    var context = new SkillContext { ... };
    
    // Act
    var result = await skill.ExecuteAsync(context);
    
    // Assert
    Assert.True(result.Success);
    Assert.True(result.Confidence > 0.7);
}
```

### Passo 6: Deploy
Nenhuma mudança em código existente necessária!
Skill automáticamente descoberta e executada.

---

## Composição de Skills

### Padrão 1: Sequencial (Linear)
```
Skill1 → Skill2 → Skill3 → Resultado
```
Padrão: Default pipeline

### Padrão 2: Condicional
```
Skill1 (classificação)
  ├─ Se resultado A → Skill2A
  └─ Se resultado B → Skill2B
```
Implementação: SkillOrchestrator valida e roteia

### Padrão 3: Paralelo (Harness)
```
Skill1 (paralelo em múltiplos modelos)
├─ Modelo A
├─ Modelo B
└─ Modelo C
Compare resultados
```
Implementação: Harness executa paralelo

---

## Testes de Skills

### Teste Unitário
```csharp
[Fact]
public async Task ExecuteAsync_WithValidInput_ReturnsSuccess()
{
    // Mock do InferenceProvider
    var mockProvider = new Mock<IInferenceProvider>();
    mockProvider.Setup(x => x.InferAsync(It.IsAny<InferenceRequest>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new InferenceResult { Output = "..." });
    
    var skill = new PerimeterAnalysisSkill(mockProvider.Object);
    var context = new SkillContext { /* ... */ };
    
    var result = await skill.ExecuteAsync(context, CancellationToken.None);
    
    Assert.True(result.Success);
}
```

### Teste de Integração
```csharp
[Fact]
public async Task SkillPipeline_E2E_ProcessesEventCorrectly()
{
    // Use real Ollama
    var inferenceProvider = new OllamaInferenceProvider(ollamaClient);
    var orchestrator = new SkillOrchestrator(skillRegistry, config, inferenceProvider);
    
    var evt = LoadTestEvent("test_image.jpg");
    var result = await orchestrator.ExecutePipelineAsync(evt, "Default", CancellationToken.None);
    
    Assert.NotNull(result.FinalResult);
    Assert.True(result.FinalResult.Confidence > 0.5);
}
```

---

## Performance de Skills

### SLA por Skill

| Skill | p95 Latência | p99 Latência |
|-------|---|---|
| PerimeterAnalysisSkill | 0.6s | 1.2s |
| FalsePositiveAnalysisSkill | 0.7s | 1.4s |
| SeverityClassificationSkill | 0.5s | 1.0s |
| IncidentSummarySkill | 0.4s | 0.9s |
| **Total Pipeline** | **2.0s** | **4.0s** |

### Otimização

1. **Cache de Prompts**: Versão de prompt em memória (não recarregar sempre)
2. **Parallelização Futura**: Se skills forem independentes, executar em paralelo
3. **Modelo Mais Rápido**: Fase 2 permite trocar para modelo mais rápido

---

## Histórico e Auditoria de Skills

Cada execução de Skill é registrada:
```json
{
  "skillName": "PerimeterAnalysisSkill",
  "skillVersion": "1.0.0",
  "executedAt": "2024-01-15T10:30:00Z",
  "executedBy": "system",
  "input": { /* ... */ },
  "output": { /* ... */ },
  "executionTime": "0.234s",
  "status": "success"
}
```

Permite:
- ✅ Auditoria completa
- ✅ Replayabilidade (executar novamente com novo modelo/prompt)
- ✅ Análise de efetividade
- ✅ Rastreabilidade

---

## Documentação Individual de Skills (Fase 1)

### 8 Skills Iniciais com Especificação Completa

Cada skill possui especificação técnica detalhada em arquivo separado:

| Skill | Arquivo | Descrição | Prioridade |
|-------|---------|-----------|-----------|
| **1. Perimeter Analysis** | [perimeter-analysis.md](perimeter-analysis.md) | Detecta violações de perímetro | P0 |
| **2. Intrusion Analysis** | [intrusion-analysis.md](intrusion-analysis.md) | Classifica tentativas de intrusão | P0 |
| **3. Motion Analysis** | [motion-analysis.md](motion-analysis.md) | Analisa padrões de movimento | P1 |
| **4. Human Activity Analysis** | [human-activity-analysis.md](human-activity-analysis.md) | Detecta atividade humana suspeita | P1 |
| **5. Vehicle Analysis** | [vehicle-analysis.md](vehicle-analysis.md) | Identifica e classifica veículos | P1 |
| **6. False Positive Analysis** | [false-positive-analysis.md](false-positive-analysis.md) | Valida ameaças vs falsos positivos | P0 |
| **7. Severity Classification** | [severity-classification.md](severity-classification.md) | Classifica nível de severidade | P0 |
| **8. Incident Summary** | [incident-summary.md](incident-summary.md) | Gera relatório operacional consolidado | P0 |

### Conteúdo de Cada Especificação

Cada arquivo individual contém:

✅ **YAML Frontmatter**
- Nome, descrição, versão
- Inputs esperados
- Outputs gerados
- Prioridade

✅ **Fluxo de Trabalho**
- Passo a passo exato de execução
- Lógica de decisão
- Validações

✅ **Regras de Negócio**
- Restrições de processamento
- Casos especiais
- Condições de erro

✅ **Formato de Saída**
- JSON estruturado
- Exemplos reais
- Casos de uso

✅ **Exemplos Práticos**
- Cenários LOW/MEDIUM/HIGH/CRITICAL
- Interpretação de resultados
- Ações recomendadas

### Pipeline Padrão (Fase 1)

```
Evento de Monitoramento
         ↓
    Perimeter Analysis      ← Detecta violação de perímetro
         ↓
  Intrusion Analysis        ← Classifica tipo de intrusão
         ↓
   Motion Analysis          ← Analisa padrão de movimento
         ↓
 Human Activity Analysis    ← Detecta comportamento suspeito
         ↓
  Vehicle Analysis          ← Identifica veículos envolvidos
         ↓
False Positive Analysis     ← Valida se é real ou ambiental
         ↓
Severity Classification     ← Classifica nível de severidade
         ↓
  Incident Summary          ← Gera relatório operacional
         ↓
   Resultado Final          ← Pronto para operador humano
```

### Composição Recomendada por Cenário

**Cenário 1: Detecção Rápida (< 2.2s)**
```
PerimeterAnalysis → FalsePositiveAnalysis → SeverityClassification → IncidentSummary
Reduz para 4 skills em sequência linear
```

**Cenário 2: Análise Completa**
```
Todas as 8 skills em sequência
Tempo esperado: 1.8-2.2s (com latência de IA)
```

**Cenário 3: Harness (Comparação)**
```
Executar pipeline completo em paralelo com múltiplos modelos
Compare resultados, metrics, confiança
```

---

## Próximos Passos

1. ✅ Definir interface ISkill
2. ✅ Especificar 8 skills iniciais em detalhe (COMPLETO)
3. ➕ Implementar 8 skills no código
4. ➕ Implementar SkillOrchestrator
5. ➕ Testes de skills (>90% coverage)
6. ➕ Integração com pipeline
7. ➕ Harness para teste/comparação
8. ➕ Fase 2: Adicionar skills adicionais (MotionAnalysis, BehaviorAnalysis, etc)
