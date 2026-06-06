---
name: severity-classification
description: Classifica nível de severidade de incidentes baseado em risco, impacto potencial e recomendações de ação.
version: 1.0.0
category: event-analysis
priority: P0
inputs:
  - analysisResults: "Consolidação de todas as skills anteriores"
  - assetValue: "Valor do ativo em risco"
  - criticalityLevel: "Importância operacional da zona"
outputs:
  - severityLevel: "info|warning|alert|critical|emergency"
  - impactScore: "0.0-1.0"
  - urgencyScore: "0.0-1.0"
  - recommendedResponse: "monitor|notify|investigate|lockdown"
---

# Fluxo de Trabalho (Workflow)

## 1. **Agregação de Riscos**
   - Consolidar resultados de todas as skills anteriores
   - Perimeter Risk: risco de violação de perímetro
   - Intrusion Risk: risco de intrusão
   - Motion Risk: risco baseado em movimento anômalo
   - Human Risk: risco baseado em comportamento humano
   - Vehicle Risk: risco baseado em atividade veicular
   - False Positive Score: reduzir severidade se falso positivo provável

## 2. **Avaliação de Contexto de Negócio**
   - Zona é crítica (servidor, dados, caixa) vs. baixa criticidade
   - Ativo em risco tem valor alto vs. baixo
   - Horário de operação (comercial = menor risco | noturno = maior risco)
   - Proximidade a pessoas (risco de segurança pessoal?)

## 3. **Cálculo de Score de Impacto Potencial**
   - **Nenhum**: Atividade normal, sem risco
   - **Baixo**: Risco mínimo, sem impacto operacional
   - **Médio**: Risco moderado, possível interrupção
   - **Alto**: Risco significante, impacto claro
   - **Crítico**: Risco severo, impacto operacional ou segurança pessoal

## 4. **Cálculo de Score de Urgência**
   - Ameaça é imediata ou latente?
   - Situação está deteriorando ou estável?
   - Ação humana necessária nos próximos minutos ou horas?
   - Escala:
     - **Baixa Urgência** (0.0-0.2): Monitorar, sem ação imediata
     - **Urgência Média** (0.2-0.5): Investigação recomendada
     - **Alta Urgência** (0.5-0.8): Ação imediata recomendada
     - **Urgência Crítica** (0.8-1.0): Ativação de protocolo de emergência

## 5. **Associação a Padrões Conhecidos**
   - Este padrão corresponde a crime conhecido?
   - Histórico de escalação desta atividade?
   - Padrão coordenado ou oportunista?
   - Correlação com outros eventos simultâneos?

## 6. **Classificação de Severidade Final**
   - Combinar Impact Score + Urgency Score + Padrão Histórico
   - Aplicar matriz de decisão
   - Gerar recomendação de resposta

---

# Regras de Negócio e Restrições

* **Impacto > Intenção**: Não importa intenção aparente - impacto real determina severidade.
* **Contexto é Multiplicador**: Mesma atividade vale 2x mais em zona crítica noturna que escritório dia.
* **Escalação Rápida**: Se score aumenta de Low para Critical em curto tempo = CRITICAL urgência.
* **Sem Normalizacao de Risco**: Mesmo que frequente, intrusão = sempre sério.
* **Segurança Pessoal Vence**: Se há risco a pessoas, escalar acima de risco patrimonial.

---

# Formato de Saída (Output)

## 📊 Resultado da Classificação de Severidade

```json
{
  "severityLevel": "info|warning|alert|critical|emergency",
  "impactScore": 0.78,
  "urgencyScore": 0.85,
  "recommendedResponse": "monitor|notify|investigate|lockdown",
  "analysisTimestamp": "2024-01-15T10:30:00Z"
}
```

### 🎯 Níveis de Severidade

| Nível | Score | Descrição | Ação |
|-------|-------|-----------|------|
| **INFO** | 0.0-0.15 | Atividade normal, sem risco | Monitor automático |
| **WARNING** | 0.15-0.35 | Anomalia leve, baixo risco | Notificar, monitorar |
| **ALERT** | 0.35-0.60 | Risco moderado, investigação recomendada | Investigação imediata |
| **CRITICAL** | 0.60-0.85 | Risco significante, ação recomendada | Ativação de protocolo |
| **EMERGENCY** | 0.85-1.0 | Risco severo, ameaça iminente | Lockdown + contato policial |

### 📈 Scores

**Impact Score** (0.0-1.0):
- Potencial dano ao ativo
- Interrupção operacional
- Risco a pessoas

**Urgency Score** (0.0-1.0):
- Velocidade de ameaça
- Deterioração da situação
- Necessidade de ação imediata

### 🔍 Análise de Risco Consolidado

**Contribuições ao Score Final**:
- Perimeter Risk: [%]
- Intrusion Risk: [%]
- Motion Risk: [%]
- Human Behavior Risk: [%]
- Vehicle Risk: [%]
- False Positive Reduction: [negativo se alto]

**Contexto de Negócio**:
- Criticalidade da zona: [low | medium | high | critical]
- Horário: [comercial | noturno | madrugada]
- Presença de pessoas: [nenhuma | algumas | muitas]

### 💡 Justificativa

Narrativa clara da classificação:

> "Intrusão detectada em zona crítica (servidor room) às 2:30 AM com falso positivo score baixo. Múltiplas skills concordam (perimeter + intrusion + human + motion). Impacto: CRÍTICO (acesso a dados). Urgência: CRÍTICA (madrugada, sem pessoal). Severidade: EMERGENCY - Lockdown imediato recomendado."

### 📋 Resposta Recomendada

**Ação Imediata**:
- **Monitor**: Continuar observando, nenhuma ação humana
- **Notify**: Notificar equipe via email/SMS, para review
- **Investigate**: Enviar segurança para investigação presencial
- **Lockdown**: Ativar protocolo de emergência, contato policial, bloqueio de acesso

---

# Exemplos

### Caso 1: Intrusão Noturna em Zona Crítica (EMERGENCY)
```
Severidade: EMERGENCY
Impact: 0.95
Urgência: 0.92
Ação: Lockdown

Análise: Múltiplas skills confirmam intrusão em servidor room madrugada.
Recomendação: Lockdown imediato, contato policial
```

### Caso 2: Movimento Suspeito em Zona Aberta (ALERT)
```
Severidade: ALERT
Impact: 0.48
Urgência: 0.55
Ação: Investigate

Análise: Movimento investigativo próximo a perímetro, horário comercial, baixo impacto.
Recomendação: Investigação em 30 minutos, não crítico
```

### Caso 3: Falso Positivo Provável (INFO)
```
Severidade: INFO
Impact: 0.08
Urgência: 0.05
Ação: Monitor

Análise: Chuva detectada como movimento, zona baixa criticidade, histórico 100% false em chuva.
Recomendação: Descarte, monitorar automaticamente
```

### Caso 4: Comportamento Suspeito Noturno (CRITICAL)
```
Severidade: CRITICAL
Impact: 0.72
Urgência: 0.78
Ação: Investigate

Análise: Pessoa investigando perímetro madrugada, zona média criticidade, comportamento coordenado suspeito.
Recomendação: Investigação imediata, agentes de segurança no local
```
