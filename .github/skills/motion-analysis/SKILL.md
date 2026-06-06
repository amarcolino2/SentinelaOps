---
name: motion-analysis
description: Analisa padrões de movimento de objetos para detectar anomalias, rotas incomuns e mudanças comportamentais.
version: 1.0.0
triggers:
  - object trajectory analysis
  - speed anomalies
  - unusual route patterns
  - movement direction analysis
context: SentinelaOps video monitoring decision support
---

# Fluxo de Trabalho (Workflow)

A IA deve executar os seguintes passos **exatamente nesta ordem**:

1. **Extração de Trajetória**
   - Rastrear posição do objeto em múltiplos frames
   - Calcular velocidade instantânea em pixels/segundo
   - Determinar direção de movimento (ângulo)
   - Identificar pontos de parada e mudanças de direção

2. **Análise de Velocidade**
   - Classificar velocidade (parado, lento, normal, rápido, muito rápido)
   - Comparar com velocidade esperada para objeto (pessoa: 1-2 m/s, veículo: 5-15 m/s)
   - Detectar aceleração/desaceleração brusca
   - Identificar mudanças de velocidade não naturais

3. **Análise de Trajetória**
   - Detectar rotas esperadas vs. rotas anômalas
   - Identificar comportamento de evasão (zigzag, esconderijo)
   - Verificar se objeto segue caminhos permitidos
   - Calcular índice de anomalia de rota (0-100)

4. **Análise Comparativa**
   - Comparar com movimento histórico de objeto similar
   - Usar padrões baseline por tipo de objeto
   - Detectar desvios significativos
   - Quantificar nível de desvio

5. **Cálculo de Score**
   - Velocidade anômala: 0-30 pontos
   - Trajetória anômala: 0-40 pontos
   - Padrão histórico: 0-20 pontos
   - Inconsistência comportamental: 0-10 pontos

6. **Estruturação de Resultado**
   - Compilar análise em formato estruturado
   - Fornecer visualização de trajetória
   - Listar anomalias detectadas

---

# Regras de Negócio e Restrições

## Obrigatórias
* **Múltiplos frames necessários**: Mínimo 10 frames (0.3 segundos) para análise
* **Escala consistente**: Calibrar velocidade em pixels/frame para metros/segundo
* **Oclusões consideradas**: Movimento sob oclusão reduz confiança
* **Contexto de zona**: Velocidade esperada depende de zona (trabalho vs. lazer)

## Contextual
* Pessoa correndo em corredor = normal; pessoa correndo em zona restrita = suspeita
* Veículo aceleração rápida em estacionamento = suspeito
* Movimento parado frequente em área de vigilância = investigação recomendada
* Mudanças de direção abruptas = comportamento evasivo

## Segurança
* Não descrever rotas de evasão conhecidas
* Não revelar zonas cegas ou pontos de vigilância fraca

---

# Formato de Saída (Output)

```json
{
  "skillName": "MotionAnalysis",
  "timestamp": "2024-01-15T10:30:45Z",
  "eventId": "{cameraId}_{timestamp}",
  
  "analysis": {
    "classification": "ANÓMALA|SUSPEITA|NORMAL|COMPORTAMENTO_EVASIVO",
    "anomalyLevel": "CRÍTICA|ALTA|MÉDIA|BAIXA|NENHUMA",
    "confidence": 84,
    "baselineComparison": "MUITO_DESVIO|DESVIO|NORMAL|CONSISTENTE"
  },
  
  "summary": "{Uma frase técnica sobre padrão de movimento}",
  
  "velocityAnalysis": {
    "averageVelocity": { value: 2.3, unit: "m/s", classification: "CORRER" },
    "maxVelocity": { value: 3.1, unit: "m/s" },
    "velocityAnomalies": [
      { time: "22:30:15", velocity: 0, eventType: "PARADA_ABRUPTA" },
      { time: "22:30:45", velocity: 3.1, eventType: "ACELERACAO_BRUSCA" }
    ],
    "expectedVelocity": { min: 1.0, max: 2.0, unit: "m/s" },
    "velocityScore": 28
  },
  
  "trajectoryAnalysis": {
    "pathType": "LINEAR|ZIGZAG|CIRCULAR|COMPLEXA",
    "anomalyPatterns": ["ZIGZAG_MOVEMENT", "FREQUENT_STOPS", "DIRECTION_CHANGES"],
    "allowedPathFollowed": false,
    "evasionBehavior": true,
    "trajectoryScore": 38,
    "waypoints": [
      { position: [100, 200], time: "22:30:00", action: "ENTRADA" },
      { position: [150, 210], time: "22:30:05", action: "MOVIMENTO_LINEAR" },
      { position: [155, 205], time: "22:30:10", action: "PARADA" },
      { position: [180, 220], time: "22:30:15", action: "MUDANÇA_ABRUPTA_DIRECAO" }
    ]
  },
  
  "comparativeAnalysis": {
    "objectId": "person_001",
    "historicalPattern": "PESSOA_ESPERADA_HORARIO_COMERCIAL",
    "currentDeviation": "MOVIMENTO_NOTURNO_ZONA_RESTRITA",
    "deviationPercentage": 78,
    "historicalScore": 18
  },
  
  "scoreBreakdown": {
    "velocity": 28,
    "trajectory": 38,
    "historical": 18,
    "behavioral": 0,
    "total": 84
  },
  
  "evidence": [
    "Velocidade média: 2.3 m/s (acima de normal para zona)",
    "Movimento em zigzag detectado",
    "Três mudanças abruptas de direção",
    "Duas paradas estratégicas (escaneamento)",
    "Desvio 78% do padrão histórico"
  ],
  
  "justification": "Movimento anômalo: velocidade elevada (2.3 vs. 1.0-2.0 m/s esperado), padrão zigzag, múltiplas paradas estratégicas. Desvio 78% do comportamento histórico esperado. Score: 84%.",
  
  "recommendations": [
    "Correlacionar com câmeras adjacentes para rastrear completo",
    "Comparar com outros eventos no horário noturno",
    "Alertar operador sobre movimento evasivo"
  ],
  
  "nextSkills": ["human-activity-analysis", "severity-classification"]
}
```

---

# Exemplo Prático

**Input:**
```
evento {
  objectId: "person_001"
  frames: [ frame_1, frame_2, ..., frame_15 ]
  fps: 30
  calibration: "1_pixel = 0.05_meters"
  historicalPatterns: { type: "PERSON", expectedZone: "hallway", expectedSpeed: "1.5_ms" }
}
```

**Output:**
```
classification: "ANOMALA"
anomalyLevel: "ALTA"
confidence: 84

velocityAnalysis: { averageVelocity: 2.3, expectedVelocity: 1.5, deviation: "+53%" }
trajectoryAnalysis: { pathType: "ZIGZAG", anomalyPatterns: ["ZIGZAG", "PARADAS", "MUDANCAS"] }

scoreBreakdown: { velocity: 28, trajectory: 38, historical: 18, total: 84 }

recommendations: [
  "Rastrear em câmeras adjacentes",
  "Alertar operador"
]
```

---

# Notas Importantes

- Análise de movimento é **chave para detecção de intenção**
- Padrões históricos devem ser constantemente atualizados
- Velocidade esperada depende de contexto (emergência vs. normal)
- Trajetória anômala frequentemente precede ação suspeita
