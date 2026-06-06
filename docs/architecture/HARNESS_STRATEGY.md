# Estratégia de Harness Engineering

## Visão Geral

O Harness é um Bounded Context dedicado a experimentação, benchmark e comparação de modelos e prompts, sem afetar produção.

---

## Objetivos do Harness

1. **Comparar Modelos**: Executar mesmo evento em múltiplos modelos simultaneamente
2. **Comparar Prompts**: Testar versões diferentes de prompt
3. **Medir Métricas**: Latência, tokens, CPU, memória, precision, recall
4. **Tomar Decisões**: Baseado em dados, não em feeling
5. **Versionar Prompts**: Histórico completo de cada versão
6. **Auditar Inferências**: Rastrear qual prompt/modelo foi usado

---

## Arquitetura do Harness

### Componentes Principais

```
┌────────────────────────────────────────────┐
│           Harness API                      │
├────────────────────────────────────────────┤
│ GET /harness/benchmarks                    │
│ POST /harness/benchmarks/start              │
│ GET /harness/benchmarks/{id}               │
│ GET /harness/comparisons                   │
│ POST /harness/comparisons/generate         │
│ GET /harness/prompts                       │
│ POST /harness/prompts/versions              │
│ POST /harness/prompts/{id}/activate        │
└────────┬───────────────────────────────────┘
         │
         ↓
┌────────────────────────────────────────────┐
│      Harness Orchestrator                  │
├────────────────────────────────────────────┤
│ • Execute múltiplos modelos                │
│ • Coleta métricas                          │
│ • Gera comparação                          │
│ • Versiona prompts                         │
└────────┬───────────────────────────────────┘
         │
    ┌────┴────┬────────────┬──────────────┐
    ↓         ↓            ↓              ↓
┌───────┐ ┌────────┐ ┌──────────┐ ┌──────────┐
│Ollama │ │Azure   │ │Anthropic │ │Mistral   │
│Gemma  │ │OpenAI  │ │Claude    │ │          │
└───────┘ └────────┘ └──────────┘ └──────────┘
```

---

## Casos de Uso do Harness

### UC-H1: Comparar Dois Modelos
**Ator**: Researcher
**Fluxo**:
1. Seleciona evento de teste
2. Seleciona: Gemma 3 4B, Qwen VL 7B
3. Sistema executa em paralelo
4. Exibe lado-a-lado:
   - Resultado (classificação)
   - Confiança
   - Tempo
   - Tokens
   - CPU/Memória
5. Decisão: qual melhor para caso específico?

---

### UC-H2: Testar Nova Versão de Prompt
**Ator**: Analyst
**Fluxo**:
1. Editor de Prompts cria v1.1.1 de "PerimeterAnalysis"
2. Clica "Test Against Dataset"
3. Sistema executa v1.1.1 contra 1000 eventos históricos
4. Calcula métricas: precision, recall, f1
5. Compara com v1.1.0:
   - v1.1.0: precision 0.82, recall 0.88, f1 0.85
   - v1.1.1: precision 0.85, recall 0.86, f1 0.85
6. Analyst aprova (trade-off aceitável)
7. Sistema publica v1.1.1 como ativa
8. Novos eventos usam v1.1.1

---

### UC-H3: Gerar Relatório de Benchmarks
**Ator**: Administrator
**Fluxo**:
1. Seleciona: data range, modelos, prompts
2. Sistema compila relatório
3. Exibe: tabelas, gráficos, top performers
4. Exporta em PDF/CSV para apresentação

---

## Banco de Dados do Harness

### Tabelas

```sql
CREATE TABLE BenchmarkRuns (
    Id UUID PRIMARY KEY,
    CreatedAt DateTime,
    Status VARCHAR(50),  -- Running, Completed, Failed
    ModelA VARCHAR(255),
    ModelB VARCHAR(255),
    EventCount INT,
    TotalDuration TimeSpan
);

CREATE TABLE BenchmarkResults (
    Id UUID PRIMARY KEY,
    BenchmarkRunId UUID,
    EventId UUID,
    ModelName VARCHAR(255),
    Output VARCHAR(MAX),
    Confidence DOUBLE,
    ExecutionTime DOUBLE,
    TokensUsed INT,
    CpuUsagePercent DOUBLE,
    MemoryUsageMB DOUBLE,
    FOREIGN KEY (BenchmarkRunId) REFERENCES BenchmarkRuns
);

CREATE TABLE PromptVersionEvaluations (
    Id UUID PRIMARY KEY,
    PromptId UUID,
    PromptVersion VARCHAR(50),
    EventDatasetId UUID,
    Precision DOUBLE,
    Recall DOUBLE,
    F1Score DOUBLE,
    SampleSize INT,
    ExecutedAt DateTime,
    FOREIGN KEY (PromptId) REFERENCES Prompts
);

CREATE TABLE ComparisonResults (
    Id UUID PRIMARY KEY,
    CreatedAt DateTime,
    Model1 VARCHAR(255),
    Model2 VARCHAR(255),
    Winner VARCHAR(255),
    Summary VARCHAR(MAX),
    DetailedAnalysis VARCHAR(MAX)
);
```

---

## Implementação do Harness

### IHarnessCoordinator

```csharp
public interface IHarnessCoordinator
{
    // Benchmarks
    Task<BenchmarkRun> StartBenchmarkAsync(
        List<string> modelNames,
        EventDataset dataset,
        CancellationToken cancellation);
    
    Task<BenchmarkRun> GetBenchmarkAsync(Guid benchmarkId);
    
    // Comparisons
    Task<ComparisonResult> GenerateComparisonAsync(
        BenchmarkRun benchmark);
    
    // Prompt Evaluation
    Task<PromptEvaluationResult> EvaluatePromptVersionAsync(
        Prompt prompt,
        string version,
        EventDataset dataset,
        CancellationToken cancellation);
}

public class HarnessOrchestrator : IHarnessCoordinator
{
    public async Task<BenchmarkRun> StartBenchmarkAsync(
        List<string> modelNames,
        EventDataset dataset,
        CancellationToken cancellation)
    {
        var benchmark = new BenchmarkRun { Id = Guid.NewGuid() };
        
        var tasks = modelNames.Select(modelName =>
            ExecuteModelOnDatasetAsync(modelName, dataset, benchmark.Id, cancellation)
        ).ToList();
        
        await Task.WhenAll(tasks);  // Executa todos em paralelo
        
        await _repository.SaveAsync(benchmark);
        return benchmark;
    }
    
    private async Task ExecuteModelOnDatasetAsync(
        string modelName,
        EventDataset dataset,
        Guid benchmarkId,
        CancellationToken cancellation)
    {
        var inferenceProvider = _providerFactory.GetProvider(modelName);
        
        foreach (var evt in dataset.Events)
        {
            var result = await inferenceProvider.InferAsync(
                new InferenceRequest { ... },
                cancellation);
            
            var benchResult = new BenchmarkResult
            {
                BenchmarkRunId = benchmarkId,
                ModelName = modelName,
                EventId = evt.Id,
                Output = result.Output,
                Confidence = result.Confidence,
                ExecutionTime = result.ExecutionTime.TotalSeconds,
                TokensUsed = result.TokensUsed,
                // ... mais métricas
            };
            
            await _repository.SaveAsync(benchResult);
        }
    }
    
    public async Task<ComparisonResult> GenerateComparisonAsync(
        BenchmarkRun benchmark)
    {
        // 1. Agrupar resultados por modelo
        var resultsPerModel = await _repository
            .GetResultsByBenchmarkAsync(benchmark.Id);
        
        // 2. Calcular métricas
        var metricsPerModel = resultsPerModel
            .GroupBy(r => r.ModelName)
            .ToDictionary(g => g.Key, CalculateMetrics);
        
        // 3. Comparar
        var comparison = new ComparisonResult
        {
            Model1 = benchmark.ModelA,
            Model2 = benchmark.ModelB,
            Winner = DetermineWinner(metricsPerModel),
            DetailedAnalysis = GenerateAnalysis(metricsPerModel)
        };
        
        return comparison;
    }
}
```

---

## Métricas Coletadas

### Por Inferência
- Output (classificação)
- Confidence score
- Tempo total
- Tokens usados
- CPU %
- Memória MB
- Timestamp

### Agregadas por Dataset
- Precision
- Recall
- F1 Score
- Accuracy
- Mean latency
- Median latency
- p95 latency
- p99 latency
- Total tokens
- Average confidence

---

## Visualização de Resultados

### Tabela Comparativa
```
Model              Precision  Recall  F1     Latency  Tokens  CPU%
───────────────────────────────────────────────────────────────────
Gemma 3 4B         0.82       0.88    0.85   0.6s     245     45%
Qwen VL 7B         0.87       0.90    0.88   0.8s     312     65%
Llama Vision       0.85       0.91    0.88   1.2s     278     72%
OpenAI GPT-4V      0.96       0.94    0.95   0.3s     198     N/A
```

### Gráficos
- Latência vs Accuracy (scatter)
- Precision/Recall por modelo (bar)
- Throughput por modelo (line)
- Distribuição de confiança (histogram)

---

## Prompt Versioning & Testing

### Workflow de Teste de Prompt

1. **Criar Versão**
   - Editor modifica prompt
   - Sistema cria v1.1.1 (draft)

2. **Testar**
   - Executar contra dataset de teste
   - Calcular precision, recall, f1

3. **Comparar**
   - Comparar com versão anterior (v1.1.0)
   - Exibir delta

4. **Decidir**
   - Se ≥ baseline: aprovar
   - Se < baseline: rejeitar ou analisar

5. **Ativar**
   - Marcar como ativa
   - Novos eventos usam v1.1.1
   - Histórico rastreável

---

## Exemplo: Teste de Prompt

```
Prompt: PerimeterAnalysis v1.0.0
Status: Current

Teste Dataset: 1000 eventos anotados
Tempo: ~45 minutos

Resultados:
  Precision: 0.82
  Recall: 0.88
  F1: 0.85
  Accuracy: 0.85
  Confidence avg: 0.74

Editor propõe:
  "Adicione contexto sobre sombras dinâmicas"

Nova versão: v1.0.1
Status: Testing

Teste Dataset: mesmos 1000 eventos

Resultados:
  Precision: 0.84 (+2%)
  Recall: 0.87 (-1%)
  F1: 0.86 (+1%)
  Accuracy: 0.86 (+1%)
  Confidence avg: 0.75 (+1%)

Análise:
  ✅ Precision melhorou (menos falsos positivos)
  ✅ F1 melhorou
  ✅ Slight recall decrease (acceptable trade-off)

Decisão: APPROVE

Versão v1.0.1 agora ativa
Novos eventos usam v1.0.1
```

---

## Segurança e Isolamento

### Isolamento de Execução
- Benchmarks rodam em transações separadas
- Não afetam produção
- Dados de benchmark não contaminam analysis real

### Autenticação
- Apenas admin pode iniciar benchmark
- Apenas analyst pode editar prompts

### Auditoria
- Cada benchmark é auditado
- Quem criou, quando, com quais parâmetros
- Resultados imutáveis

---

## Escalabilidade do Harness

### Horizontal
- Executar múltiplos modelos em paralelo (threads)
- Usar connection pool para banco

### Vertical
- Usar GPU para acelerar modelos
- Aumentar memória se dataset é grande

### Temporal
- Agendador executa benchmarks fora de pico
- Evita contenção com produção

---

## Integração com Produção

### Feedback Loop
1. Prompt ativa em produção
2. Eventos processados com métrica de sucesso
3. Analyst coleta eventos "borderline"
4. Testa novo prompt contra esses eventos
5. Se melhora: ativa novo prompt
6. Volta ao passo 1

---

## Dashboard do Harness

```
┌─────────────────────────────────────────────────────┐
│           HARNESS DASHBOARD                        │
├─────────────────────────────────────────────────────┤
│                                                     │
│  Models Under Test:                                │
│  [ ] Gemma 3 4B      [ ] Qwen VL          [ ] Llama │
│                                                     │
│  Prompt Versions:                                  │
│  PerimeterAnalysis:                               │
│    Active: v1.0.1 (precision: 0.84, f1: 0.86)    │
│    Draft:  v1.0.2 (testing)                       │
│    History: [v1.0.0] [v1.0.1] ...                │
│                                                     │
│  Latest Benchmark:                                │
│  ID: 550e8400...                                  │
│  Status: Completed                                │
│  Models Tested: 3                                 │
│  Winner: OpenAI (f1: 0.95)                       │
│                                                     │
│  [Start New Benchmark] [Test Prompt] [Export] ...│
│                                                     │
└─────────────────────────────────────────────────────┘
```

---

## Roadmap do Harness

### Fase 1 (MVP)
- ✅ Comparar modelos
- ✅ Testar prompts
- ✅ Versionar prompts
- ✅ Métricas básicas

### Fase 2
- ➕ Synthetic dataset generation
- ➕ A/B testing automático
- ➕ Anomaly detection (prompt regrediu?)
- ➕ Cost analysis (custo por modelo)

### Fase 3
- ➕ Fine-tuning detection
- ➕ Training pipeline
- ➕ Model marketplace (contribuições da comunidade)
- ➕ Advanced analytics

---

## Próximos Passos

1. ✅ Definir arquitetura Harness
2. ➕ Implementar IHarnessCoordinator
3. ➕ Implementar repositórios do Harness
4. ➕ Implementar endpoints /harness
5. ➕ Implementar dashboard
6. ➕ Testar end-to-end
7. ➕ Integração com prompts
