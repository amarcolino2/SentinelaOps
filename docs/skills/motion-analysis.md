---
name: motion-analysis
description: Analisa padrões de movimento para detectar atividade anormal, comportamentos suspeitos e rastreabilidade de objetos.
version: 1.0.0
category: event-analysis
priority: P1
inputs:
  - image: "JPEG ou sequência de frames"
  - metadata: "Zona, timestamp, configurações de sensibilidade"
  - motionVectors: "Direção e magnitude de movimento"
outputs:
  - motionDetected: "boolean"
  - activityType: "normal|suspicious|frantic"
  - movementPattern: "linear|circular|erratic|stationary"
  - velocity: "slow|moderate|high|very_high"
  - anomalyScore: "0.0-1.0"
---

# Fluxo de Trabalho (Workflow)

## 1. **Captura de Movimento**
   - Detectar pixels em movimento frame a frame
   - Calcular magnitude (quanto mudou) e direção (para onde)
   - Filtrar ruído (chuva, sombras, iluminação)
   - Gerar mapa de calor de movimento

## 2. **Extração de Trajetória**
   - Rastrear entidades ao longo do tempo
   - Construir vetor de movimento (início → fim)
   - Calcular velocidade estimada
   - Identificar pontos de parada ou mudança de direção

## 3. **Análise de Padrão**
   - **Linear**: Movimento direto, previsível (pessoa caminhando normal)
   - **Circular**: Padrão repetitivo (vigilância, investigação do local)
   - **Erratic**: Movimento aleatório, impulsivo (corrida, fuga, pânico)
   - **Stationary**: Objeto parado ou movimento mínimo

## 4. **Estimativa de Velocidade**
   - **Slow**: < 1 m/s (pessoas caminhando normal, vigilância)
   - **Moderate**: 1-2 m/s (caminhada rápida, trote)
   - **High**: 2-4 m/s (corrida)
   - **Very High**: > 4 m/s (sprint, veículo)

## 5. **Cálculo de Anomalia**
   - Comparar com padrão esperado da zona
   - Considerar contexto: horário, atividade normal
   - Avaliar consistência: movimento coeso vs caótico
   - Detectar mudanças abruptas de direção

## 6. **Validação Contextual**
   - Zona comercial com corrida = anormal (fuga)
   - Zona esportiva com corrida = normal
   - Movimento circular em perímetro = investigação (suspeito)
   - Movimento rápido saindo de zona restrita = crítico

---

# Regras de Negócio e Restrições

* **Contexto Temporal**: Movimento rápido durante intervalo comercial ≠ problema. Mesmo movimento às 3 AM = suspeito.
* **Zona Sensível**: Movimento próximo a áreas críticas (cofre, servidor) = aumentar alerta independente de velocidade.
* **Velocidade Relativa**: Velocidade para o contexto (corrida em pátio = normal; corrida em corredor = suspeito).
* **Animais e Vento**: Filtrar movimento de pequenos animais e vegetação balançando.
* **Sem Privacidade Excessiva**: Rastrear movimento é legítimo; não tirar conclusões sobre intenção.

---

# Formato de Saída (Output)

## 📊 Resultado da Análise de Movimento

```json
{
  "motionDetected": true,
  "activityType": "normal|suspicious|frantic",
  "movementPattern": "linear|circular|erratic|stationary",
  "velocity": "slow|moderate|high|very_high",
  "anomalyScore": 0.65,
  "analysisTimestamp": "2024-01-15T10:30:00Z"
}
```

### 🎯 Análise de Movimento

**Tipo de Atividade**:
- **Normal**: Movimento esperado para zona e horário
- **Suspicious**: Desvio do padrão, mas não crítico
- **Frantic**: Movimento caótico, impulsivo, indicativo de pânico/fuga

**Padrão de Movimento**:
- Linear: Trajetória reta e previsível
- Circular: Padrão em órbita ou vigilância
- Erratic: Mudanças abruptas de direção
- Stationary: Pouco ou nenhum movimento

### 📈 Velocidade e Dinâmica

- **Velocidade Estimada**: [slow | moderate | high | very_high]
- **Mudanças de Direção**: [nenhuma | ocasional | frequente | constante]
- **Aceleração**: [gradual | normal | abrupta]

### 💡 Interpretação

Narrativa do padrão:

> "Entidade movendo-se em círculo próximo a zona de acesso restrito, velocidade alternando entre rápida e parada. Padrão circular consistente por 3 minutos sugere vigilância/reconhecimento. Anomalia: 0.78 (suspeito)."

### ⚠️ Observações

- [Se movimento rápido em zona crítica: "Proximidade a servidor room + movimento acelerado = alto risco"]
- [Se padrão repetitivo: "Quatro voltas no perímetro - comportamento investigativo"]

---

# Exemplos

### Caso 1: Movimento Frenético (Suspeito)
```
Tipo: Frantic
Padrão: Erratic
Velocidade: Very High
Anomalia: 0.89

Análise: Pessoa alternando entre corrida e parada abrupta, mudanças frequentes de direção.
Interpretação: Possível fuga, pânico ou comportamento agressivo.
```

### Caso 2: Vigilância Circular (Suspeito)
```
Tipo: Suspicious
Padrão: Circular
Velocidade: Moderate
Anomalia: 0.72

Análise: Movimento em órbita próximo a perímetro, pausa em pontos-chave.
Interpretação: Reconhecimento ou investigação do local.
```

### Caso 3: Movimento Normal (Esperado)
```
Tipo: Normal
Padrão: Linear
Velocidade: Moderate
Anomalia: 0.15

Análise: Pessoa caminhando direto em horário comercial, velocidade normal.
Interpretação: Atividade esperada e sem alerta.
```
