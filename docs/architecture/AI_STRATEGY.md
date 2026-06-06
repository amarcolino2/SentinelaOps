# Estratégia de Inteligência Artificial

## Visão Geral

A estratégia de IA define:
1. Abstração agnóstica de modelo
2. Seleção de modelo inicial (Ollama + Gemma 3)
3. Critérios de avaliação de modelos
4. Roadmap de evolução de modelos

---

## Princípio: Agnósticismo Total

### Isolamento de Modelo
O domínio não conhece:
- ❌ Ollama, Gemma, Qwen, Llama, OpenAI, etc
- ✅ Interface: IInferenceProvider
- ✅ Contrato: InferenceRequest → InferenceResult

### Camada de Abstração
```
┌──────────────────────────┐
│   Application Layer      │
│  (Skills, Orchestrator)  │
└────────────┬─────────────┘
             │
      ┌──────▼───────┐
      │ IInference   │
      │ Provider     │
      └──────┬───────┘
             │
    ┌────────┴──────────┬──────────────┬─────────────┐
    ▼                   ▼              ▼             ▼
┌─────────┐      ┌──────────┐    ┌──────────┐  ┌──────────┐
│ Ollama  │      │ Azure    │    │ Anthropic│  │ Mistral  │
│ Gemma   │      │ OpenAI   │    │ Claude   │  │ API      │
└─────────┘      └──────────┘    └──────────┘  └──────────┘
```

---

## Modelo Inicial: Ollama + Gemma 3 4B

### Justificativa

| Critério | Escolha | Razão |
|----------|---------|-------|
| **Custo** | Gratuito (local) | Nenhuma quota de API |
| **Latência** | 0.3-0.5s (GPU) | Aceitável para análise |
| **Modelo** | Gemma 3 4B | Balance performance/tamanho |
| **Setup** | Docker local | Nenhuma infraestrutura external |
| **Agnósticismo** | IInferenceProvider | Fácil trocar depois |

### Especificações Técnicas

**Ollama**:
- Versão: Latest
- Motor: llama.cpp
- Quantização: 4-bit (q4_0) para Gemma 3 4B
- Contexto: 2048 tokens

**Gemma 3 4B**:
- Parâmetros: 4B
- Conhecimento cutoff: Mid-2023
- Arquitetura: Transformer
- Gemma 3 (última versão Gemma)
- Performance:
  - Latência: 0.3s (GPU RTX 3080)
  - Throughput: 3-4 tokens/segundo

### Capabilidades Suportadas
- ✅ Vision (análise de imagens)
- ✅ Reasoning (análise lógica)
- ✅ Structured output (JSON parsing)
- ✅ Prompt templating
- ✅ Context window suficiente

---

## Interface IInferenceProvider

### Contrato
```csharp
public interface IInferenceProvider
{
    Task<InferenceResult> InferAsync(
        InferenceRequest request,
        CancellationToken cancellation);
}

public class InferenceRequest
{
    public string Prompt { get; set; }
    public string ImageBase64 { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
}

public class InferenceResult
{
    public string Output { get; set; }
    public double? Confidence { get; set; }
    public int TokensUsed { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public DateTime ExecutedAt { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}
```

### Implementações Fase 1
```csharp
public class OllamaInferenceProvider : IInferenceProvider
{
    public async Task<InferenceResult> InferAsync(
        InferenceRequest request,
        CancellationToken cancellation)
    {
        // 1. Construir prompt final (combine template + dados)
        // 2. Chamar Ollama REST API
        // 3. Parsear resposta
        // 4. Extrair confidence se presente
        // 5. Registrar métricas (tempo, tokens)
        // 6. Retornar InferenceResult
    }
}
```

---

## Prompts Iniciais (Exemplos)

### PerimeterAnalysisSkill Prompt

**Template v1.0.0**:
```
Você é um especialista em análise de segurança de videomonitoramento.

Analise a imagem fornecida e determine se a detecção está DENTRO ou FORA do perímetro definido.

ZONA: {zone}
PERÍMETRO: {perimeter_description}
CÂMERA: {camera_name}

Imagem: [attached]

Responda em JSON:
{
  "classification": "inside|outside|uncertain",
  "confidence": 0.0-1.0,
  "justification": "explicar decisão",
  "evidence": ["evidência 1", "evidência 2"],
  "metadata": {
    "position_x": 0.0-1.0,
    "position_y": 0.0-1.0,
    "distance_estimated": "meters"
  }
}

Seja preciso e justifique claramente cada decisão.
```

### FalsePositiveAnalysisSkill Prompt

**Template v1.0.0**:
```
Você é um especialista em redução de falsos positivos em videomonitoramento.

Analise os resultados da análise anterior e determine a probabilidade de ser um falso positivo.

ANÁLISE ANTERIOR: {previous_analysis}
IMAGEM: [attached]
ZONA: {zone}
HISTÓRICO: {historical_context}

Possíveis causas de falso positivo:
- Movimento de árvores/folhas
- Reflexo de luz solar
- Sombras dinâmicas
- Animais pequenos (gatos, pássaros)
- Artefatos de câmera
- Chuva/neve

Responda em JSON:
{
  "probability_false_positive": 0.0-1.0,
  "likely_causes": ["causa 1", "causa 2"],
  "confidence_in_assessment": 0.0-1.0,
  "justification": "explicação"
}

Seja conservador: dúvida = favor de falso positivo.
```

---

## Critérios de Avaliação de Modelos

### Métricas Quantitativas

| Métrica | Alvo | Descrição |
|---------|------|-----------|
| **Latência p95** | < 2s | Tempo de resposta para análise completa |
| **Precision** | > 0.85 | Reduzir falsos positivos |
| **Recall** | > 0.90 | Não perder eventos reais |
| **F1 Score** | > 0.87 | Balance precision/recall |
| **Tokens por Req** | < 500 | Manter custo baixo |
| **Confiança Média** | > 0.75 | Scores devem ser altos quando correto |

### Métricas Qualitativas
- ✅ Justificativa clara (não "black box")
- ✅ Saída estruturada (JSON parseable)
- ✅ Sem alucinações
- ✅ Estável (resultados consistentes)

---

## Comparação de Modelos Fase 1

| Modelo | Custo | Latência | Qualidade | Agnóstico | Seleção |
|--------|-------|----------|-----------|-----------|---------|
| **Gemma 3 4B** | Grátis | 0.5s | Boa | ✅ | ✅ Inicial |
| Qwen VL 7B | Grátis | 0.8s | Muito boa | ✅ | ➕ Fase 2 |
| Llama Vision | Grátis | 1.2s | Excelente | ✅ | ➕ Fase 2 |
| OpenAI GPT-4V | $$$$ | 0.3s | Excelente | ✅ | ➕ Fase 2 |
| Azure OpenAI | $$$$ | 0.4s | Excelente | ✅ | ➕ Fase 2 |

---

## Roadmap de Evolução de Modelos

### Fase 1 (MVP): Ollama Local
```
Gemma 3 4B (local)
  ✅ Grátis
  ✅ Latência aceitável
  ✅ Qualidade adequada para MVP
  ✅ Prototipagem rápida
```

### Fase 2: Comparação e Otimização
```
Teste paralelo:
├─ Gemma 3 4B (manter)
├─ Qwen VL 7B (melhor visão)
└─ Llama Vision (mais preciso)

Harness permite:
├─ Executar evento em paralelo
├─ Comparar resultados
├─ Medir latência/custo
└─ Decidir baseado em dados
```

### Fase 3: Produção Escalada
```
Deploy em produção:
├─ Modelo com melhor f1 score
├─ Múltiplas instâncias (escala)
└─ A/B testing se desejado
```

### Fase 4: Cloud-Based
```
Integrar com provedores:
├─ Azure OpenAI (enterprise)
├─ Anthropic Claude (reasoning)
├─ Mistral (balance)
└─ Suportar múltiplos simultâneos
```

---

## Estratégia de Testagem de Modelos

### Dataset de Avaliação

**Tamanho**: 1000 eventos anotados

**Distribuição**:
- 60% eventos válidos (true positives)
- 25% falsos positivos (teste de redução)
- 10% edge cases (sombras, reflexos)
- 5% inconclusive

**Anotação**: Manual por operadores + consensus

### Execução de Teste

1. **Baseline** (Gemma 3 4B):
   - Execute em 1000 eventos
   - Registre: classification, confidence, tempo, tokens
   - Calcule: precision, recall, f1

2. **Novo Modelo**:
   - Execute em mesmos 1000 eventos
   - Compare: f1 score, latência, custo

3. **Decisão**:
   - Se novo ≥ baseline em f1 e latência: aprove
   - Se novo > baseline em f1 mas latência pior: análise
   - Se novo < baseline: rejeite

### Exemplo de Resultado

```
Teste de Qwen VL 7B vs Gemma 3 4B

Dataset: 1000 eventos
Tempo total: 45 minutos (executado em paralelo)

Gemma 3 4B:
├─ Precision: 0.82
├─ Recall: 0.88
├─ F1: 0.85
├─ Latência p95: 1.2s
├─ Tokens/req: 245
└─ CPU: 45%

Qwen VL 7B:
├─ Precision: 0.87
├─ Recall: 0.90
├─ F1: 0.88 (✅ +3.5%)
├─ Latência p95: 1.8s (❌ +50%)
├─ Tokens/req: 312
└─ CPU: 78%

Recomendação:
Qwen tem melhor accuracy mas latência é 50% pior.
Se latência crítica: mantém Gemma.
Se accuracy crítica: upgrade para Qwen.
Alternativa: ensemble (usar ambas).
```

---

## Estratégia de Fallback

Se modelo primário falha:
1. Retry automático (3 tentativas, backoff exponencial)
2. Se continua falhando: confidence = 0.0 (marca como inconfiável)
3. Próxima skill recebe resultado com confiança baixa
4. Resultado final marca: "IA temporariamente indisponível"
5. Operador pode tomar decisão manual

---

## Configuração Runtime de Modelo

### Via Variáveis de Ambiente

```bash
# Fase 1
export INFERENCE_PROVIDER=Ollama
export OLLAMA_ENDPOINT=http://ollama:11434
export OLLAMA_MODEL=gemma3:4b

# Fase 2 - switch para Azure OpenAI
export INFERENCE_PROVIDER=AzureOpenAI
export AZURE_OPENAI_ENDPOINT=https://xxx.openai.azure.com/
export AZURE_OPENAI_DEPLOYMENT=gpt-4-vision
export AZURE_OPENAI_KEY=xxx
```

### Via Configuração
```yaml
# appsettings.json
Inference:
  Provider: Ollama
  Timeout: 5
  RetryPolicy:
    MaxRetries: 3
    InitialDelay: 1000
    MaxDelay: 8000
  Ollama:
    Endpoint: http://ollama:11434
    Model: gemma3:4b
```

---

## SLA de Performance de Modelo

| Métrica | SLA | Ação se violado |
|---------|-----|---|
| Latência p95 | < 2s | Alerta, análise de causa |
| Taxa de erro | < 1% | Alerta, investigar |
| Confidence média | > 0.7 | Alerta, revisar prompt |
| Throughput | > 10 evt/s | Scale modelo (réplicas) |

---

## Próximos Passos

1. ✅ Implementar IInferenceProvider interface
2. ✅ Implementar OllamaInferenceProvider
3. ✅ Setup Docker Compose com Ollama
4. ✅ Configurar prompts iniciais em database
5. ✅ Implementar Harness para comparação
6. ➕ Fase 2: Adicionar mais modelos via mesma interface
7. ➕ Fase 2: A/B testing automático
8. ➕ Fase 3: Fine-tuning baseado em dados
