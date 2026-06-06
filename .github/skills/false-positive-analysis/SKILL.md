---
name: false-positive-analysis
description: Reduz falsos positivos através de análise contextual, verificação de padrões históricos e validação de confiabilidade.
version: 1.0.0
triggers:
  - suspicious event detected
  - high risk classification
  - before escalating to human review
  - confidence uncertainty
context: SentinelaOps video monitoring decision support
---

# Fluxo de Trabalho (Workflow)

A IA deve executar os seguintes passos **exatamente nesta ordem**:

1. **Análise de Evidência**
   - Revisar todas as evidências fornecidas por skills anteriores
   - Avaliar qualidade de cada evidência (forte/média/fraca)
   - Identificar contradições entre evidências
   - Calcular índice de consistência de evidência

2. **Validação Contextual**
   - Verificar contexto de zona (permitido vs. proibido)
   - Considerar contexto temporal (horário normal vs. suspeito)
   - Analisar contexto ambiental (condições de luminosidade, clima)
   - Avaliar contexto histórico (comportamento esperado vs. observado)

3. **Análise de Padrões**
   - Comparar com eventos históricos similares
   - Avaliar frequência de padrão (raro vs. comum)
   - Verificar sazonalidade (horário, dia da semana)
   - Detectar se evento é outlier ou parte de série

4. **Verificação de Artefatos**
   - Detectar possíveis artefatos de câmera (reflexo, sombra, movimento de câmera)
   - Validar se detecção é real ou falha de sensor
   - Avaliar qualidade da imagem (desfoque, saturação)
   - Confirmar se evento é genuíno

5. **Cálculo de Verossimilhança**
   - Probabilidade de falso positivo: 0-100%
   - Probabilidade de verdadeiro positivo: 0-100%
   - Risco residual após análise: 0-100%
   - Recomendação de ação

6. **Estruturação de Resultado**
   - Compilar análise comparativa
   - Fornecer recomendação clara (manter, reduzir, descartar alerta)
   - Justificar decisão com evidência

---

# Regras de Negócio e Restrições

## Obrigatórias
* **Não descartar sem justificativa**: Qualquer evento pode ser real
* **Conservador em redução**: Errar por excesso de alerta é melhor que ignorar ameaça
* **Evidência múltipla necessária**: Uma evidência forte + histórico = maior confiança
* **Contexto sempre**: Mesma ação pode ser normal ou suspeita dependendo de zona/hora

## Contextual
* Pessoa parada em corredor durante expediente = NORMAL
* Pessoa parada em corredor noturno = SUSPEITA
* Sombra de árvore movendo-se = possível FALSO_POSITIVO
* Reflexo em vidro = possível FALSO_POSITIVO (mas validar)

## Segurança
* Focar em redução de falsos positivos, não em cobertura de segurança
* Manter vigilância mesmo em padrão histórico

---

# Formato de Saída (Output)

```json
{
  "skillName": "FalsePositiveAnalysis",
  "timestamp": "2024-01-15T10:30:45Z",
  "eventId": "{cameraId}_{timestamp}",
  "dependsOn": ["perimeter-analysis", "intrusion-analysis", "motion-analysis", "human-activity-analysis", "vehicle-analysis"],
  
  "analysis": {
    "falsePositiveProbability": 15,
    "truePositiveProbability": 85,
    "residualRisk": 12,
    "recommendation": "MANTER_ALERTA|REDUZIR_SEVERIDADE|DESCARTAR_ALERTA|ESCALACAO_IMEDIATA",
    "confidence": 88
  },
  
  "summary": "{Uma frase técnica sobre validade do evento}",
  
  "evidenceValidation": {
    "totalEvidences": 6,
    "strongEvidences": 4,
    "mediumEvidences": 2,
    "weakEvidences": 0,
    "contradictions": 0,
    "consistencyScore": 92
  },
  
  "contextualValidation": {
    "zoneContext": "AREA_RESTRITA_NOTURNA",
    "zoneAppropriate": false,
    "timeContext": "22:45",
    "timeAppropriate": false,
    "environmentalFactors": {
      "lighting": "DARK",
      "weather": "CLEAR",
      "cameraQuality": "BOAS",
      "imageQuality": "CLARA_FOCADA"
    }
  },
  
  "patternAnalysis": {
    "historicalSimilar": 2,
    "patternFrequency": "RARO",
    "isOutlier": true,
    "seasonality": "NAO_ESPERADO_HORARIO",
    "isAnomaly": true
  },
  
  "artifactValidation": {
    "camerArtifacts": false,
    "reflections": false,
    "shadows": false,
    "motionArtifacts": false,
    "sensorErrors": false,
    "detectionQuality": "GENUINO"
  },
  
  "probabilityAnalysis": {
    "falsePositiveProbability": 15,
    "causes": [
      "Possível oclusão parcial de objeto (reduz confiança em 5%)",
      "Qualidade de imagem levemente comprometida (reduz confiança em 3%)",
      "Padrão raro mas não impossível (reduz confiança em 7%)"
    ],
    "truePositiveProbability": 85,
    "indicators": [
      "Múltiplas skills confirmam evento suspeito",
      "Contexto zona/hora é inapropriado para atividade",
      "Histórico mostra padrão raro",
      "Sem evidência de artefato"
    ]
  },
  
  "riskAssessment": {
    "residualRisk": 12,
    "riskFactors": [
      "Pequena possibilidade de falso positivo (15%)",
      "Score agregado: 81% de probabilidade de ameaça real"
    ],
    "mitigations": [
      "Correlacionar com câmeras adjacentes",
      "Verificar dados de acesso físico",
      "Rastrear movimento até saída"
    ]
  },
  
  "comparison": {
    "historicalEvents": [
      { date: "2024-01-10", similarity: 78, outcome: "VALIDADO_INTRUSAO" },
      { date: "2024-01-05", similarity: 65, outcome: "FALSO_POSITIVO_MANUTENCAO" }
    ],
    "pastOutcomeRate": 0.6
  },
  
  "evidence": [
    "Múltiplas skills confirmam evento anômalo",
    "Contexto: zona restrita, horário noturno = inapropriado",
    "Imagem clara, sem artefatos detectados",
    "Padrão histórico similar validado como verdadeiro positivo em 78%",
    "Sem explicação ambiental óbvia"
  ],
  
  "justification": "Análise de falso positivo: 85% probabilidade de verdadeiro positivo. Múltiplas skills corroboram. Contexto inapropriado para atividade. Imagem de boa qualidade, sem artefatos. Histórico similar foi validado. Risco residual: 12%. Recomendação: MANTER_ALERTA com nível ajustado.",
  
  "recommendations": [
    "MANTER alerta em nível MEDIUM/ALTO",
    "NÃO reduzir severidade",
    "Rastrear movimento completo",
    "Correlacionar com acesso físico"
  ],
  
  "nextSkills": ["severity-classification", "incident-summary"]
}
```

---

# Exemplo Prático

**Input:**
```
evento {
  previousSkillsResults: {
    perimeter: { classification: "VIOLATION", confidence: 88 },
    intrusion: { classification: "TENTATIVA_INTRUSAO", confidence: 87 },
    motion: { classification: "ANOMALA", confidence: 84 },
    humanActivity: { riskAssessment: "SUSPEITA", confidence: 82 }
  },
  imageQuality: "CLARA_FOCADA",
  historicalContext: { rarePattern: true, similarities: 2 }
}
```

**Output:**
```
falsePositiveProbability: 15
truePositiveProbability: 85
recommendation: "MANTER_ALERTA"
confidence: 88

evidenceValidation: {
  totalEvidences: 6,
  strongEvidences: 4,
  consistency: 92
}

justification: "Múltiplas evidências fortes. Contexto inapropriado. Histórico similar foi validado. FP risk: 15%."

recommendations: [
  "MANTER alerta",
  "Rastrear completo"
]
```

---

# Notas Importantes

- **Objetivo principal**: Reduzir falsos positivos SEM comprometer detecção
- **Balanço crítico**: Erro de comissão (alerta falso) vs. erro de omissão (ignorar ameaça)
- **Histórico é ouro**: Padrões similares validados aumentam confiança dramaticamente
- **Contexto contextual**: Mesma atividade pode ser normal ou suspeita dependendo de zona/hora
