---
name: intrusion-analysis
description: Analisa indicadores de intrusão ativa como arrombamento, escalada, destruição de obstáculos e comportamento evasivo.
version: 1.0.0
triggers:
  - perimeter violation confirmed
  - forced entry detected
  - evasive behavior observed
  - multiple boundary crossings
context: SentinelaOps video monitoring decision support
---

# Fluxo de Trabalho (Workflow)

A IA deve executar os seguintes passos **exatamente nesta ordem**:

1. **Análise de Indicadores Físicos**
   - Detectar tentativas de arrombamento (portas, janelas)
   - Identificar escalada de estruturas (cercas, muros, prédios)
   - Verificar destruição ou derrubada de obstáculos
   - Confirmar uso de ferramentas ou equipamentos de intrusão

2. **Análise de Comportamento**
   - Avaliar movimentos evasivos (zigzag, agachamento, rastejamento)
   - Detectar paradas frequentes para observação
   - Identificar coordenação entre múltiplos indivíduos
   - Verificar comunicação visual/gestual entre pessoas

3. **Análise Temporal**
   - Considerar hora do evento (períodos de menor vigilância)
   - Avaliar velocidade de execução (planejado vs. apressado)
   - Verificar padrão de retirada (fuga vs. pausa tática)
   - Analisar duração total da ação

4. **Análise de Intenção**
   - Diferenciar acesso não autorizado de roubo/vandalismo
   - Avaliar se houve acesso a áreas críticas
   - Verificar manipulação de equipamentos/propriedade
   - Determinar nível de premeditação

5. **Cálculo de Score de Intrusão**
   - Física (forçamento detectado): 0-40 pontos
   - Comportamental (evasivo confirmado): 0-30 pontos
   - Temporal (horário suspeito): 0-20 pontos
   - Coordenação (múltiplas pessoas): 0-10 pontos
   - Score final: 0-100

6. **Estruturação de Resultado**
   - Compilar análise em formato estruturado
   - Gerar sequência de eventos
   - Fornecer recomendações de resposta

---

# Regras de Negócio e Restrições

## Obrigatórias
* **Confirmar perimetral primeiro**: Só ativar se perimeter-analysis retornou VIOLATION ou SUSPICIOUS
* **Evidência física necessária**: Sem indicadores físicos, classificar como "SUSPEITO" não "INTRUSÃO"
* **Conservador em intenção**: Malandro intent só com múltiplos indicadores concordantes
* **Documentar cada passo**: Incluir qual evidência levou a qual conclusão

## Contextual
* Funcionários autorizados com falha de acesso = ACESSO_NAO_AUTORIZADO (não INTRUSÃO)
* Arrombamento visível = elevar automaticamente para INTRUSÃO_ATIVA
* Escalada de estrutura + cobertura = indicador forte de intrusão
* Múltiplas pessoas coordenadas = intrusão organizada

## Segurança
* Nunca descrever técnicas de intrusão em justificativa
* Não revelar vulnerabilidades específicas exploradas
* Usar linguagem técnica, neutra, sem juízos de valor

---

# Formato de Saída (Output)

```json
{
  "skillName": "IntrusionAnalysis",
  "timestamp": "2024-01-15T10:30:45Z",
  "eventId": "{cameraId}_{timestamp}",
  "dependsOn": ["perimeter-analysis"],
  
  "analysis": {
    "classification": "INTRUSAO_ATIVA|TENTATIVA_INTRUSAO|ACESSO_NAO_AUTORIZADO|SUSPEITO|NORMAL",
    "intrusionType": "ARROMBAMENTO|ESCALADA|EVASAO|COORDENADA|DESCONHECIDO",
    "riskLevel": "CRITICAL|HIGH|MEDIUM|LOW|NONE",
    "confidence": 87,
    "sophistication": "IMPROVISADA|PLANEJADA|PROFISSIONAL"
  },
  
  "summary": "{Uma frase técnica descrevendo tipo e nível de intrusão}",
  
  "physicalIndicators": {
    "forcedEntry": { detected: true, location: "north_gate", confidence: 92 },
    "toolsDetected": true,
    "obstaclesDestroyed": true,
    "damageLevel": "MINOR|MODERATE|SEVERE"
  },
  
  "behavioralIndicators": {
    "evasiveBehavior": true,
    "multipleStops": 3,
    "coordination": true,
    "numberOfIndividuals": 2,
    "communicationDetected": true
  },
  
  "temporalAnalysis": {
    "timeOfEvent": "22:30",
    "peakVulnerabilityTime": true,
    "executionSpeed": "RAPID|MEASURED|HESITANT",
    "withdrawalPattern": "ORGANIZED|PANICKED|METHODICAL"
  },
  
  "intentionAnalysis": {
    "appearentTarget": "STORAGE_BUILDING|OFFICE|EQUIPMENT|UNKNOWN",
    "premeditationLevel": "HIGH|MEDIUM|LOW",
    "accessToSensitiveAreas": true,
    "equipmentManipulation": true
  },
  
  "scoreBreakdown": {
    "physical": 38,
    "behavioral": 28,
    "temporal": 15,
    "coordination": 6,
    "total": 87
  },
  
  "eventSequence": [
    { time: "22:30:15", action: "Pessoa aproxima-se de portão norte" },
    { time: "22:30:22", action: "Tenta abrir portão (travado)" },
    { time: "22:30:28", action: "Retira ferramenta de mochila" },
    { time: "22:30:45", action: "Força abertura do portão com alavanca" },
    { time: "22:30:52", action: "Portão abre, pessoa atravessa" },
    { time: "22:30:55", action: "Segunda pessoa segue em movimento rápido" }
  ],
  
  "evidence": [
    "Tentativa visível de forçamento de portão",
    "Uso de ferramenta para alavanca",
    "Movimento evasivo - agachamento repetido",
    "Coordenação entre dois indivíduos",
    "Horário: 22:30 (fora de horário operacional)",
    "Sem uniforme ou crachá visível"
  ],
  
  "justification": "Arrombamento confirmado de portão norte com ferramenta. Dois indivíduos em coordenação deliberada. Movimento evasivo. Horário de pico de vulnerabilidade. Score: 87%.",
  
  "recommendations": [
    "ATIVAR RESPOSTA IMEDIATA - Chamada para segurança",
    "Bloquear saídas do perímetro",
    "Acompanhar movimento dos intrusos em câmeras consecutivas",
    "Gerar log de segurança para auditoria"
  ],
  
  "nextSkills": ["severity-classification", "incident-summary"]
}
```

---

# Exemplo Prático

**Input:**
```
evento {
  perimeter-analysis: { classification: "VIOLATION", confidence: 88 }
  image: "/events/cam-north-01_20240115_223045.jpg"
  images_sequence: [ img_1, img_2, img_3, img_4, img_5 ]
  timeWindow: "22:30:00 - 22:31:00"
}
```

**Output:**
```
classification: "INTRUSAO_ATIVA"
confidence: 87
summary: "Arrombamento confirmado com ferramenta em portão norte. Dois indivíduos em coordenação deliberada."

physicalIndicators: {
  forcedEntry: true,
  toolsDetected: true,
  obstaclesDestroyed: true
}

scoreBreakdown: { physical: 38, behavioral: 28, temporal: 15, coordination: 6, total: 87 }

recommendations: [
  "RESPOSTA IMEDIATA",
  "Bloquear saídas",
  "Rastrear em câmeras seguintes"
]
```

---

# Notas Importantes

- Intrusão é cenário crítico que requer resposta imediata
- Errar por **excesso de cautela** é aceitável, sub-avisar é inaceitável
- Sequência temporal é chave para detectar intrusão coordenada
- Sempre fornecer recomendações operacionais específicas
