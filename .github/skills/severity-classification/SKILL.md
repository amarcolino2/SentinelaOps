---
name: severity-classification
description: Classifica severidade e urgência de incidentes para priorização de resposta operacional.
version: 1.0.0
triggers:
  - event classification needed
  - incident priority assignment required
  - resource allocation decision needed
  - escalation determination
context: SentinelaOps video monitoring decision support
---

# Fluxo de Trabalho (Workflow)

A IA deve executar os seguintes passos **exatamente nesta ordem**:

1. **Consolidação de Análises**
   - Coletar resultados de todas as skills anteriores
   - Validar completude (nenhuma análise faltando)
   - Resolver conflitos entre análises
   - Calcular score agregado

2. **Avaliação de Impacto**
   - Determinar potencial de dano (pessoal, patrimonial, operacional)
   - Estimar risco de escalação (pode piorar?)
   - Avaliar envolvimento de pessoas
   - Verificar proximidade com áreas críticas

3. **Determinação de Urgência**
   - Evento em progresso vs. evento passado
   - Velocidade de escalação (lento vs. rápido)
   - Tempo de resposta disponível
   - Janela de oportunidade para resposta

4. **Cálculo de Matriz de Risco**
   - Probabilidade: baixa/média/alta
   - Impacto: baixo/médio/alto
   - Urgência: baixa/média/alta
   - Score agregado: 0-100

5. **Classificação de Severidade**
   - CRÍTICO: Risco iminente de vida ou dano grave
   - ALTO: Risco significativo de dano
   - MÉDIO: Potencial de dano, requer investigação
   - BAIXO: Anomalia detectada, monitoramento
   - NENHUM: Evento validado como normal

6. **Determinação de Resposta**
   - Tipo de resposta (imediata, dentro de X minutos, monitoramento)
   - Recursos recomendados (número de pessoas, equipamentos)
   - Escalation path (quem informar)
   - SLA de resposta esperada

---

# Regras de Negócio e Restrições

## Obrigatórias
* **Probabilidade alta + Impacto alto = CRÍTICO automaticamente**
* **Em progresso = elevar um nível automaticamente**
* **Sem análise de risco = não classificar (retornar INCOMPLETO)**
* **Pessoas envolvidas = elevar impacto para mínimo MÉDIO**

## Contextual
* Intrusão em progresso = CRÍTICO (vida em risco)
* Intrusão completada = ALTO (dano já ocorreu)
* Anomalia comportamental = MÉDIO (requer investigação)
* Violação técnica sem contexto = BAIXO (monitoramento)

## Segurança
* Errar por cima (CRÍTICO vs. ALTO) é aceitável
* Errar por baixo (BAIXO vs. MÉDIO quando CRÍTICO apropriado) é inaceitável

---

# Formato de Saída (Output)

```json
{
  "skillName": "SeverityClassification",
  "timestamp": "2024-01-15T10:30:45Z",
  "eventId": "{cameraId}_{timestamp}",
  "dependsOn": ["all_previous_skills"],
  
  "analysis": {
    "severityLevel": "CRITICO|ALTO|MEDIO|BAIXO|NENHUM",
    "urgencyLevel": "IMEDIATA|ALTA|MEDIA|BAIXA",
    "responseRequired": true,
    "escalationRequired": true,
    "overallConfidence": 89
  },
  
  "summary": "{Uma frase técnica sobre severidade e ação recomendada}",
  
  "riskMatrix": {
    "probability": {
      "value": "ALTA",
      "score": 8,
      "reasoning": "Múltiplas evidências corroboram evento genuíno"
    },
    "impact": {
      "value": "ALTO",
      "score": 8,
      "reasoning": "Intrusão em progresso em área crítica"
    },
    "urgency": {
      "value": "IMEDIATA",
      "score": 9,
      "reasoning": "Evento em progresso, risco dinâmico"
    },
    "riskScore": 25,
    "scoreInterpretation": "CRÍTICO (20-27)"
  },
  
  "consolidatedAnalysis": {
    "perimeterAnalysis": {
      "classification": "VIOLATION",
      "confidence": 88,
      "weight": 0.15
    },
    "intrusionAnalysis": {
      "classification": "INTRUSAO_ATIVA",
      "confidence": 87,
      "weight": 0.25
    },
    "motionAnalysis": {
      "classification": "ANOMALA",
      "confidence": 84,
      "weight": 0.15
    },
    "humanActivityAnalysis": {
      "classification": "EVASIVO",
      "confidence": 82,
      "weight": 0.20
    },
    "vehicleAnalysis": {
      "classification": null,
      "weight": 0.0
    },
    "falsePositiveAnalysis": {
      "falsePositiveProbability": 15,
      "confidence": 88,
      "weight": 0.25
    },
    "aggregatedScore": 85
  },
  
  "impactAssessment": {
    "affectedAreas": ["storage_building", "administrative_area"],
    "peopleInvolved": 2,
    "criticalAssetsAtRisk": true,
    "potentialForEscalation": true,
    "estimatedTimeToCompromise": "5_minutes"
  },
  
  "urgencyFactors": {
    "eventInProgress": true,
    "dynamicThreat": true,
    "timeForResponse": "CRÍTICA",
    "responseWindowRemaining": "minutes"
  },
  
  "responseRecommendation": {
    "immediateAction": "DISPATCH_SECURITY",
    "resourcesRequired": {
      "securityPersonnel": 2,
      "equipment": ["radio", "flashlight", "body_camera"],
      "additionalSupport": "ON_STANDBY"
    },
    "escalationPath": [
      "Security Chief (imediato)",
      "Facility Manager (imediato)",
      "Local Police (se intrusão confirmada)"
    ],
    "SLAResponseTime": "2_minutes"
  },
  
  "communicationTemplate": {
    "alertLevel": "🔴 CRÍTICO",
    "incidentType": "Intrusão ativa em progresso",
    "location": "Prédio de Armazenamento, Portão Norte",
    "numberOfIntruders": 2,
    "lastSeen": "Área administrativa, 23:01:15",
    "direction": "Desconhecido",
    "recommendations": "Dispatch imediato de segurança. Alertar vizinhos. Preparar coordenação com polícia."
  },
  
  "evidence": [
    "Intrusão ativa confirmada por múltiplas skills",
    "Dois indivíduos em coordenação",
    "Movimento evasivo detectado",
    "Contexto: horário noturno, zona crítica",
    "Probabilidade de verdadeiro positivo: 85%"
  ],
  
  "justification": "CRÍTICO: Intrusão ativa em progresso com múltiplas evidências. Risco iminente. Probability: ALTA (8/10). Impact: ALTO (8/10). Urgency: IMEDIATA (9/10). Score agregado: 85%. FP risk: 15% (aceitável). Recomendação: DISPATCH IMEDIATO.",
  
  "recommendations": [
    "🚨 DISPATCH IMEDIATO de 2 pessoas de segurança",
    "Alertar Gerente de Instalações",
    "Rastrear movimento em câmeras adjacentes em tempo real",
    "Preparar coordenação com polícia local",
    "Documentar todos os eventos para auditoria",
    "Revisar integridade de perímetro após resolução"
  ],
  
  "followUpSkills": ["incident-summary"]
}
```

---

# Exemplo Prático

**Input:**
```
evento {
  allPreviousSkillsResults: {
    perimeter: VIOLATION,
    intrusion: INTRUSAO_ATIVA,
    motion: ANOMALA,
    humanActivity: EVASIVO,
    falsePositive: { probability: 15 }
  },
  eventInProgress: true,
  criticalArea: true
}
```

**Output:**
```
severityLevel: "CRITICO"
urgencyLevel: "IMEDIATA"
escalationRequired: true
confidence: 89

riskMatrix: {
  probability: "ALTA" (8/10),
  impact: "ALTO" (8/10),
  urgency: "IMEDIATA" (9/10),
  riskScore: 25 (CRÍTICO)
}

responseRecommendation: {
  immediateAction: "DISPATCH_SECURITY",
  SLAResponseTime: "2_minutes",
  escalationPath: ["Security_Chief", "Facility_Manager", "Police"]
}

recommendations: [
  "DISPATCH IMEDIATO",
  "Rastrear em câmeras",
  "Preparar polícia"
]
```

---

# Notas Importantes

- **Severidade determina resposta**: CRÍTICO sempre requer ação imediata
- **Matriz de risco é framework**: Considerar contexto além de números
- **Em progresso é game-changer**: Evento em progresso sempre elevar urgência
- **Documentação é obrigatória**: Toda classificação deve ser auditável
