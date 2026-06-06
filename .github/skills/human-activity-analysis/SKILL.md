---
name: human-activity-analysis
description: Reconhece e classifica atividades humanas como trabalho, locomoção, descanso, comportamento suspeito e interações.
version: 1.0.0
triggers:
  - human detected in scene
  - activity pattern recognition
  - behavioral classification needed
  - unusual activity observed
context: SentinelaOps video monitoring decision support
---

# Fluxo de Trabalho (Workflow)

A IA deve executar os seguintes passos **exatamente nesta ordem**:

1. **Detecção de Pose Humana**
   - Identificar articulações principais (cabeça, ombros, cotovelos, punhos, quadril, joelhos, tornozelos)
   - Calcular ângulos de postura (inclinação, flexão)
   - Verificar simetria do corpo
   - Validar se é realmente humano (confiança >75%)

2. **Classificação de Estado Corporal**
   - Parado em pé
   - Em movimento (caminhando, correndo)
   - Agachado ou inclinado
   - Deitado (caído, repouso)
   - Escalando
   - Outros estados

3. **Análise de Atividade**
   - Trabalho: postura ativa, movimentos repetitivos
   - Locomoção: movimento linear, padrão de caminhada
   - Descanso: parado, postura relaxada
   - Comportamento evasivo: agachamento, ocultação
   - Interação: múltiplos humanos próximos

4. **Análise de Gestos**
   - Levantar, abaixar, empurrar, puxar, carregar
   - Apontar, sinalizar, comunicar
   - Olhar (direção, frequência)
   - Manipulação de objetos

5. **Cálculo de Score**
   - Pose confiança: 0-30 pontos
   - Atividade consistência: 0-40 pontos
   - Contexto apropriado: 0-20 pontos
   - Histórico comportamental: 0-10 pontos

6. **Estruturação de Resultado**
   - Compilar análise em formato estruturado
   - Categorizar atividade com confiança
   - Fornecer recomendações

---

# Regras de Negócio e Restrições

## Obrigatórias
* **Pose mínima**: Detectar pelo menos 5 articulações principais com >70% confiança
* **Contexto geográfico**: Atividade esperada depende de zona (escritório, estacionamento, etc.)
* **Horário contexto**: Atividades diferentes esperadas em turnos diferentes
* **Sem especulação**: Apenas classificar atividades visualmente evidentes

## Contextual
* Pessoa em pé em escritório às 9:00 = NORMAL
* Pessoa agachada em zona restrita noturna = SUSPEITA
* Múltiplas pessoas juntas em horário comercial = NORMAL
* Múltiplas pessoas em coordenação noturna = SUSPEITA

## Segurança
* Não identificar indivíduos específicos
* Não descrever padrões de vulnerabilidade pessoal
* Focar em atividade, não em características biométricas

---

# Formato de Saída (Output)

```json
{
  "skillName": "HumanActivityAnalysis",
  "timestamp": "2024-01-15T10:30:45Z",
  "eventId": "{cameraId}_{timestamp}",
  
  "analysis": {
    "humanDetected": true,
    "poseConfidence": 87,
    "activityClassification": "TRABALHO|LOCOMOCAO|DESCANSO|EVASIVO|INTERACAO|DESCONHECIDO",
    "activityConfidence": 82,
    "riskAssessment": "NORMAL|SUSPEITA|PREOCUPANTE",
    "contextAlignment": "APROPRIADO|QUESTIONAVEL|INADEQUADO"
  },
  
  "summary": "{Uma frase técnica sobre atividade humana detectada}",
  
  "poseAnalysis": {
    "detectedJoints": 15,
    "totalJoints": 17,
    "skeletonConfidence": 87,
    "bodyPosture": {
      "orientation": "UPRIGHT|BENT|CROUCHED|PRONE",
      "balance": "STABLE|UNSTABLE",
      "tension": "RELAXED|NORMAL|TENSE"
    }
  },
  
  "activityBreakdown": {
    "primaryActivity": { type: "LOCOMOCAO", confidence: 89, duration: "2.5_seconds" },
    "secondaryActivities": [
      { type: "OBSERVACAO", confidence: 76, duration: "0.8_seconds" },
      { type: "MANIPULACAO_OBJETO", confidence: 64, duration: "1.2_seconds" }
    ]
  },
  
  "gestureAnalysis": {
    "detectedGestures": [
      { gesture: "OLHAR_DIRECIONADO", direction: "DIREITA", confidence: 78 },
      { gesture: "ALCANCE_ALTO", confidence: 71 },
      { gesture: "MOVIMENTO_LATERAL", confidence: 83 }
    ]
  },
  
  "objectInteraction": {
    "objectDetected": true,
    "objectType": "DESCONHECIDO",
    "interactionType": "CARREGAR|MANIPULAR|EXAMINAR|EMPURRAR|PUXAR",
    "interactionConfidence": 68
  },
  
  "scoreBreakdown": {
    "pose": 26,
    "activity": 33,
    "context": 19,
    "historical": 4,
    "total": 82
  },
  
  "contextAnalysis": {
    "zone": "storage_area",
    "timeOfDay": "22:45",
    "expectedActivities": ["LOCOMOCAO", "OBSERVACAO"],
    "currentActivity": "LOCOMOCAO",
    "alignment": "PARCIAL"
  },
  
  "evidence": [
    "Pose humana detectada com 87% confiança",
    "Movimento consistente de locomoção",
    "Paradas frequentes para observação",
    "Gestos de alcance detectados (posível manipulação)",
    "Postura tensa, não relaxada"
  ],
  
  "justification": "Atividade de locomoção com paradas estratégicas para observação. Detecção de gestos de alcance e manipulação. Contexto: horário noturno em área de armazenamento. Comportamento alinhado parcialmente com padrão esperado. Score: 82%.",
  
  "recommendations": [
    "Correlacionar com dados de acesso (cartão, chave)",
    "Verificar se funcionário autorizado",
    "Rastrear destino final do movimento"
  ],
  
  "nextSkills": ["false-positive-analysis", "severity-classification"]
}
```

---

# Exemplo Prático

**Input:**
```
evento {
  image: "/events/cam-storage-01_20240115_224500.jpg"
  skeleton: { joints: 17, confidence: 0.87 }
  context: { zone: "storage", time: "22:45", expectedActivity: "LOCOMOCAO" }
}
```

**Output:**
```
humanDetected: true
activityClassification: "LOCOMOCAO"
activityConfidence: 82
riskAssessment: "SUSPEITA"

poseAnalysis: {
  orientation: "UPRIGHT",
  balance: "STABLE",
  tension: "TENSE"
}

gestureAnalysis: [
  { gesture: "OLHAR_DIRECIONADO", direction: "DIREITA" },
  { gesture: "MOVIMENTO_ALCANCE" }
]

recommendations: [
  "Verificar se autorizado em horário noturno",
  "Rastrear movimento"
]
```

---

# Notas Importantes

- Análise de atividade humana é **contexto-dependente**
- Mesma atividade pode ser normal ou suspeita dependendo de zona/hora
- Pose humana detector precisa calibração contínua
- Sempre correlacionar com dados de acesso quando possível
