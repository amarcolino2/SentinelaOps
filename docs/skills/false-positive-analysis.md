---
name: false-positive-analysis
description: Valida e classifica eventos como verdadeiras ameaças ou falsos positivos reduzindo alarmes desnecessários.
version: 1.0.0
category: event-analysis
priority: P0
inputs:
  - priorEvent: "Resultado de análises anteriores (perimeter, intrusion, motion, human, vehicle)"
  - alarmHistory: "Histórico de alarmes na zona"
  - environmentalFactors: "Condições clima, iluminação, eventos programados"
outputs:
  - isFalsePositive: "boolean"
  - falsePositiveReason: "environmental|sensor_malfunction|legitimate_activity|other"
  "confidence": "0.0-1.0"
  - recommendedAction: "dismiss|investigate|escalate"
---

# Fluxo de Trabalho (Workflow)

## 1. **Análise de Contexto Histórico**
   - Verificar frequência de alarmes nesta zona
   - Identificar padrões de falsos positivos anteriores
   - Analisar horários e datas com maiores falsas ativações
   - Correlacionar com eventos externos (chuva, vento, manutenção)

## 2. **Avaliação de Fatores Ambientais**
   - Chuva/neve causando movimento visual falso
   - Sombras de árvores ou estruturas causando detecção
   - Variação de iluminação (pôr do sol, crepúsculo)
   - Reflexos em vidro ou água
   - Fumaça ou poeira em suspensão
   - Vento movimentando objetos, bandeiras, vegetação

## 3. **Análise de Plausibilidade**
   - Evento ocorre em zona comum para atividade (ex: pessoa em escritório)
   - Horário e contexto indicam atividade legítima
   - Múltiplas câmeras confirmando vs. apenas uma câmera
   - Qualidade de imagem (nitidez, distância, ângulo)

## 4. **Validação de Equipamento**
   - Câmera conhecida como problemática?
   - Sensor com histórico de falsas ativações?
   - Manutenção recente ou calibração necessária?
   - FOV (campo de visão) corretamente configurado?

## 5. **Análise de Consistência**
   - Todos os sensores concordam? (camera, PIR, door sensor)
   - Evento aparece em múltiplas frames ou apenas uma?
   - Objeto rastreável ao longo da sequência?
   - Padrão movimento coerente ou saltos aleatórios (sensor error)?

## 6. **Classificação de Confiança**
   - Se > 0.8: Verdadeiro positivo confiável
   - Se 0.6-0.8: Provável verdadeiro, pero investigação recomendada
   - Se 0.4-0.6: Ambíguo, revisar humano
   - Se < 0.4: Provável falso positivo, baixa prioridade

---

# Regras de Negócio e Restrições

* **Falso Positivo > Alarme Falso**: Errar por cautelosidade é preferível a ignorar verdadeira ameaça.
* **Histórico Domina**: Se zona tem 95% de false positives em horário X, novo alarme nesta hora = provável falso.
* **Múltipla Confirmação**: Um sensor = baixa confiança. Múltiplos sensores concordando = alta confiança.
* **Qualidade Importa**: Imagem desfocada/distante reduz confiança; imagem clara próxima aumenta.
* **Sem Contexto Não Funciona**: Mesmo evento é falso em horário comercial e crítico à noite.
* **Automação Limitada**: Falsos positivos <0.4 podem ser descartados automaticamente; >0.4 requer revisão.

---

# Formato de Saída (Output)

## 📊 Resultado da Análise de Falso Positivo

```json
{
  "isFalsePositive": true,
  "falsePositiveReason": "environmental|sensor_malfunction|legitimate_activity|other",
  "confidence": 0.85,
  "recommendedAction": "dismiss|investigate|escalate",
  "analysisTimestamp": "2024-01-15T10:30:00Z"
}
```

### 🎯 Classificação

**Razão de Falso Positivo**:
- **Environmental**: Chuva, sombras, iluminação, vento, vegetação
- **Sensor Malfunction**: Câmera com problema, calibração incorreta, FOV fora
- **Legitimate Activity**: Atividade normal, autorizada, esperada
- **Other**: Não classificado nos anteriores

**Ação Recomendada**:
- **Dismiss**: Baixíssima confiança em ameaça, descartar alarme
- **Investigate**: Ambíguo ou suspeito o suficiente para revisão humana
- **Escalate**: Verdadeiro positivo provável, escalar para segurança

### 📊 Análise Detalhada

**Fatores Ambientais Detectados**:
- [Listagem de fatores que podem causar falso positivo]

**Histórico da Zona**:
- Frequência de alarmes: [rara | ocasional | frequente]
- Taxa de falsos positivos: [baixa % | média % | alta %]
- Padrão temporal: [horários com mais false positives]

**Validação de Equipamento**:
- Câmera: [funcionando normal | com problema conhecido]
- Sensores: [concordam | discordam | dados conflitivos]

**Análise de Consistência**:
- Múltiplos frames: [confirmam | contradizem]
- Múltiplos sensores: [concordam | discordam]
- Qualidade de dado: [alta | média | baixa]

### 💡 Justificativa

Narrativa clara:

> "Evento às 6:47 AM em zona comercial durante hora de chegada. Pessoa detectada é funcionário comum. Câmera funciona normal, imagem clara. Contexto: atividade esperada para horário. Histórico: zero alarmes verdadeiros nesta hora em 3 meses. Classificado como FALSE POSITIVE com 92% confiança - descarte recomendado."

### ⚠️ Observações

- [Se ambiental: "Chuva pesada neste momento - múltiplos sensores malfuncionando"]
- [Se sensor problem: "Câmera sudoeste conhecida por 15% false positive rate - baixa confiança"]
- [Se legítimo: "Atividade de limpeza autorizada - nenhuma anomalia"]

---

# Exemplos

### Caso 1: Falso Positivo Claro - Descarte
```
Razão: Environmental
Confiança: 0.92
Ação: Dismiss

Análise: Chuva detectada, vento forte, histórico 100% false positive em chuva.
Recomendação: Descarte automático, não escalar
```

### Caso 2: Ambíguo - Investigar
```
Razão: Legitimate Activity (provável)
Confiança: 0.55
Ação: Investigate

Análise: Pessoa em zona durante expediente, mas câmera qualidade ruim, sensor PIR discorda.
Recomendação: Revisão humana necessária para confirmação
```

### Caso 3: Verdadeiro Positivo - Escalar
```
Razão: Suspicious (não é falso)
Confiança: 0.88
Ação: Escalate

Análise: Pessoa fora de horário autorizado, múltiplos sensores confirmam, câmera alta qualidade.
Recomendação: Escalar para segurança imediatamente
```

### Caso 4: Atividade Legítima - Descarte
```
Razão: Legitimate Activity
Confiança: 0.78
Ação: Dismiss

Análise: Equipe de limpeza autorizada em horário noturno esperado, identificados por uniforme.
Recomendação: Descarte, atividade rotineira
```
