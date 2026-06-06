---
name: incident-summary
description: Consolida análises em relatório executivo estruturado para operadores com classificação, evidências, justificativa e próximos passos.
version: 1.0.0
triggers:
  - all prior analyses completed
  - incident report required
  - operator briefing needed
  - incident documentation
context: SentinelaOps video monitoring decision support
---

# Fluxo de Trabalho (Workflow)

A IA deve executar os seguintes passos **exatamente nesta ordem**:

1. **Consolidação de Resultados**
   - Coletar outputs de todas as 7 skills anteriores
   - Validar completude de análise
   - Resolver qualquer conflito residual
   - Compilar score agregado final

2. **Estruturação de Relatório**
   - Criar título executivo (uma linha, técnico)
   - Compilar sumário (2-3 frases)
   - Organizar seções por impacto
   - Estruturar timeline de eventos

3. **Síntese de Evidências**
   - Listar evidências mais impactantes (top 5)
   - Agrupar por categoria (visual, comportamental, temporal, contextual)
   - Classificar por força (forte, média, fraca)
   - Fornecer referências a frames/timestamps

4. **Geração de Justificativa**
   - Explicar classification final em linguagem técnica
   - Descrever como cada skill contribuiu ao resultado
   - Justificar confiança agregada
   - Explicar risco residual

5. **Estruturação de Ações**
   - Listar ações recomendadas em ordem de prioridade
   - Fornecer SLA de execução
   - Indicar responsáveis
   - Fornecer critérios de sucesso

6. **Documentação para Auditoria**
   - Gerar trace completo da análise
   - Incluir timestamps de todos os eventos
   - Documentar decisões e justificativas
   - Preparar para revisão posterior

---

# Regras de Negócio e Restrições

## Obrigatórias
* **Linguagem clara para operadores**: Técnica mas compreensível
* **Sem especulação**: Apenas fatos e evidências
* **Completude necessária**: Incluir todas as perspectivas de análise
* **Auditabilidade**: Sempre possível rastrear cada conclusão

## Contextual
* Incident CRÍTICO: Resposta detalhada, recomendações específicas
* Incident ALTO: Resposta completa, alternativas possíveis
* Incident MÉDIO: Resumo detalhado, recomendações gerais
* Incident BAIXO: Sumário conciso, monitoramento sugerido

## Segurança
* Não revelar técnicas de intrusão
* Focar em resposta operacional
* Documentar para conformidade/auditoria

---

# Formato de Saída (Output)

```json
{
  "skillName": "IncidentSummary",
  "timestamp": "2024-01-15T10:30:45Z",
  "eventId": "{cameraId}_{timestamp}",
  "reportGenerated": "2024-01-15T23:02:30Z",
  "dependsOn": ["all_previous_skills"],
  
  "incidentReport": {
    "title": "Intrusão Ativa em Progresso - Prédio Administrativo",
    "severity": "CRITICO",
    "confidence": 85,
    "statusNow": "EM_PROGRESSO",
    "estimatedResolutionTime": "5_minutes",
    "escalationInProgress": true
  },
  
  "executiveSummary": "Intrusão ativa confirmada com múltiplas indicadores: arrombamento de portão, dois indivíduos em coordenação, movimento evasivo em zona restrita noturna. Risco iminente. Dispatch de segurança recomendado.",
  
  "timeline": [
    {
      "timestamp": "2024-01-15T22:30:00Z",
      "event": "EVENTO_INICIAL",
      "location": "Portão Norte",
      "description": "Pessoa detectada aproximando-se de portão norte"
    },
    {
      "timestamp": "2024-01-15T22:30:15Z",
      "event": "TENTATIVA_ACESSO",
      "location": "Portão Norte",
      "description": "Tentativa de abrir portão travado"
    },
    {
      "timestamp": "2024-01-15T22:30:45Z",
      "event": "ARROMBAMENTO",
      "location": "Portão Norte",
      "description": "Portão forçado com ferramenta (alavanca)"
    },
    {
      "timestamp": "2024-01-15T22:30:52Z",
      "event": "PERIMETER_BREACH",
      "location": "Portão Norte",
      "description": "Segundo indivíduo entra rapidamente após primeiro"
    },
    {
      "timestamp": "2024-01-15T23:01:15Z",
      "event": "MOVIMENTO_COMPLETO",
      "location": "Prédio Administrativo",
      "description": "Ambos os indivíduos rastreados em movimento evasivo"
    }
  ],
  
  "skillAnalysesSummary": {
    "perimeterAnalysis": {
      "classification": "VIOLATION",
      "confidence": 88,
      "keyFinding": "Cruzamento deliberado de perímetro com comportamento proposital"
    },
    "intrusionAnalysis": {
      "classification": "INTRUSAO_ATIVA",
      "confidence": 87,
      "keyFinding": "Arrombamento confirmado com ferramenta, múltiplos indivíduos"
    },
    "motionAnalysis": {
      "classification": "ANOMALA",
      "confidence": 84,
      "keyFinding": "Padrão de movimento evasivo com paradas estratégicas"
    },
    "humanActivityAnalysis": {
      "classification": "EVASIVO",
      "confidence": 82,
      "keyFinding": "Comportamento humano consistente com intrusão deliberada"
    },
    "vehicleAnalysis": {
      "classification": "N/A",
      "confidence": null,
      "keyFinding": "Sem veículo detectado; possível acesso pedestre"
    },
    "falsePositiveAnalysis": {
      "falsePositiveProbability": 15,
      "confidence": 88,
      "keyFinding": "Múltiplas evidências corroboras evento genuíno"
    },
    "severityClassification": {
      "severity": "CRITICO",
      "confidence": 89,
      "keyFinding": "Evento em progresso com risco iminente"
    }
  },
  
  "keyEvidence": [
    {
      "type": "VISUAL",
      "strength": "FORTE",
      "description": "Arrombamento visível de portão norte com ferramenta",
      "frameRef": "frame_045",
      "timestamp": "2024-01-15T22:30:45Z"
    },
    {
      "type": "COMPORTAMENTAL",
      "strength": "FORTE",
      "description": "Dois indivíduos em coordenação deliberada",
      "frameRef": "frame_048-055",
      "timestamp": "2024-01-15T22:30:48Z-22:30:55Z"
    },
    {
      "type": "TEMPORAL",
      "strength": "MÉDIA",
      "description": "Horário noturno fora de operação (22:30)",
      "frameRef": "metadata",
      "timestamp": "2024-01-15T22:30:00Z"
    },
    {
      "type": "CONTEXTUAL",
      "strength": "FORTE",
      "description": "Zona restrita, movimento evasivo, sem autorização",
      "frameRef": "multiple_frames",
      "timestamp": "ongoing"
    },
    {
      "type": "SEQUENCIAL",
      "strength": "FORTE",
      "description": "Sequência: aproximação → tentativa → arrombamento → entrada",
      "frameRef": "timeline_frames",
      "timestamp": "2024-01-15T22:30:00Z-22:30:55Z"
    }
  ],
  
  "justification": "Intrusão genuína confirmada com confiança 85%. Sete indicadores independentes corroboram: arrombamento visível, múltiplos indivíduos em coordenação, padrão de movimento evasivo, contexto noturno, horário inapropriado, zona restrita, sequência temporal coerente. Falso positivo risk: 15% (residual aceitável). Scores agregados: Perimeter 88%, Intrusion 87%, Motion 84%, Behavior 82%, FP-Test 88%, Severity 89%. Todas as análises congruentes.",
  
  "operatorBriefing": {
    "situation": "INTRUSÃO ATIVA EM PROGRESSO",
    "location": "Prédio Administrativo (Entrada: Portão Norte)",
    "numberOfInvolved": 2,
    "riskLevel": "CRÍTICO",
    "immediateThreats": [
      "Roubo de dados/equipamentos",
      "Dano a propriedade",
      "Possível violência se confrontados"
    ],
    "lastKnownLocation": "Prédio Administrativo (última detecção 23:01:15)",
    "lastKnownBehavior": "Movimento evasivo, paradas estratégicas, possível reconhecimento",
    "estimatedObjective": "Acesso a áreas críticas (desconhecido)"
  },
  
  "recommendedActions": [
    {
      "priority": 1,
      "action": "DISPATCH IMEDIATO",
      "responsibility": "Security Chief",
      "SLA": "2 minutos",
      "details": "Dispatch de 2-3 pessoas de segurança armadas para prédio administrativo"
    },
    {
      "priority": 2,
      "action": "BLOQUEIO DE SAÍDAS",
      "responsibility": "Security Team",
      "SLA": "3 minutos",
      "details": "Posicionar pessoal em saídas principais e secundárias"
    },
    {
      "priority": 3,
      "action": "RASTREAMENTO EM TEMPO REAL",
      "responsibility": "Surveillance Operator",
      "SLA": "IMEDIATO",
      "details": "Monitorar câmeras adjacentes para rastrear movimento completo dos intrusos"
    },
    {
      "priority": 4,
      "action": "CONTATO COM POLÍCIA",
      "responsibility": "Facility Manager",
      "SLA": "5 minutos",
      "details": "Chamar polícia local com detalhes: 2 intrusos, prédio administrativo, horário 22:30"
    },
    {
      "priority": 5,
      "action": "DOCUMENTAÇÃO PARA AUDITORIA",
      "responsibility": "Security Chief",
      "SLA": "Após resolução",
      "details": "Coletar vídeos de todas as câmeras envolvidas, timeline completa, ações tomadas"
    }
  ],
  
  "successCriteria": [
    "Intrusos localizados e contidos",
    "Perímetro seguro e restaurado",
    "Nenhuma perda patrimonial confirmada",
    "Documentação completa para auditoria",
    "Polícia informada e possível investigação iniciada"
  ],
  
  "escalationContacts": [
    {
      "role": "Security Chief",
      "action": "IMEDIATO",
      "method": "Radio + Phone"
    },
    {
      "role": "Facility Manager",
      "action": "IMEDIATO",
      "method": "Phone + SMS"
    },
    {
      "role": "CEO/Executive",
      "action": "DENTRO DE 15 MIN",
      "method": "Phone"
    },
    {
      "role": "Polícia Local",
      "action": "DENTRO DE 5 MIN",
      "method": "911/Emergência"
    }
  ],
  
  "auditTrail": {
    "eventId": "cam-north-01_20240115_223045",
    "analysisStartTime": "2024-01-15T22:30:45Z",
    "analysisCompleteTime": "2024-01-15T23:02:30Z",
    "skillsExecuted": [
      "perimeter-analysis",
      "intrusion-analysis",
      "motion-analysis",
      "human-activity-analysis",
      "vehicle-analysis",
      "false-positive-analysis",
      "severity-classification",
      "incident-summary"
    ],
    "overallConfidence": 85,
    "reportApprovedBy": "SISTEMA_AUTOMATICO",
    "lastUpdated": "2024-01-15T23:02:30Z"
  },
  
  "followUpRequired": {
    "immediate": [
      "Rastrear movement until resolution",
      "Coordinate with police",
      "Brief security team"
    ],
    "short_term": [
      "Post-incident review meeting",
      "Assess perimeter damage",
      "Update security protocols"
    ],
    "long_term": [
      "Implement additional monitoring",
      "Review access control logs",
      "Conduct full security audit"
    ]
  }
}
```

---

# Exemplo de Saída para Operador

```
🔴 ALERTA CRÍTICO - INTRUSÃO ATIVA EM PROGRESSO

📍 LOCAL: Prédio Administrativo (Entrada: Portão Norte)
⏰ HORA: 22:30-23:01
👥 INDIVÍDUOS: 2 pessoas
🎯 SEVERIDADE: CRÍTICO

📋 SUMÁRIO:
Intrusão ativa confirmada com múltiplos indicadores: arrombamento 
de portão, dois indivíduos em coordenação deliberada, padrão de 
movimento evasivo em zona restrita noturna. Risco iminente.

⚡ AÇÕES IMEDIATAS:
1. [2 MIN] Dispatch de 2-3 pessoas de segurança armadas
2. [3 MIN] Bloquear saídas do prédio
3. [AGORA] Rastrear em câmeras adjacentes
4. [5 MIN] Chamar polícia local

🔒 STATUS: EM PROGRESSO
🎯 PRÓX. PASSO: Aguardar posição de segurança para confronto seguro
```

---

# Notas Importantes

- **Operador precisa de ação clara e imediata**: Relatório deve ser acionável
- **Confiança importante**: Incluir nível de confiança em cada conclusão
- **Auditoria obrigatória**: Toda conclusão deve ser rastreável
- **Escalation é crítica**: Contatos e timings devem ser precisos
- **Documentação para pós-análise**: Permitir que especialistas revisem decisões
