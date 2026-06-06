---
name: perimeter-analysis
description: Analisa eventos de violação de perímetro identificando características visuais, padrões de movimento e níveis de ameaça.
version: 1.0.0
category: event-analysis
priority: P0
inputs:
  - image: "JPEG do evento"
  - metadata: "Zona, timestamp, configuração de sensores"
  - contextHistory: "Eventos anteriores da mesma zona"
outputs:
  - isPerimeterViolation: "boolean"
  - violationType: "entry|exit|suspicious_proximity"
  - confidence: "0.0-1.0"
  - visualEvidence: "Descrição das características visuais"
  - riskLevel: "low|medium|high|critical"
---

# Fluxo de Trabalho (Workflow)

A análise de perímetro processa eventos de vigilância para detectar violações de limite de zona com precisão e justificativa.

## 1. **Validação e Contextualização**
   - Validar se a imagem contém dados suficientes (claridade, enquadramento)
   - Confirmar se o evento ocorreu dentro dos limites da zona monitorada
   - Recuperar histórico da zona (padrão normal, violações anteriores)
   - Identificar hora do dia, condições de iluminação, atividade típica

## 2. **Análise Visual e Detecção de Objetos**
   - Identificar entidades presentes: pessoas, veículos, objetos
   - Mapear posição de cada entidade em relação ao perímetro definido
   - Detectar movimento (vetor e velocidade)
   - Avaliar trajetória (se cruza, se aproxima, se afasta do perímetro)

## 3. **Classificação de Tipo de Violação**
   - **Entry**: Entidade cruzando de fora para dentro
   - **Exit**: Entidade cruzando de dentro para fora
   - **Suspicious Proximity**: Entidade muito próxima ao perímetro mas sem cruzamento
   - **No Violation**: Atividade normal dentro ou fora da zona

## 4. **Cálculo de Risco**
   - **Low**: Proximidade detectada, movimento previsível, contexto normal
   - **Medium**: Violação clara mas horário comercial, padrão conhecido
   - **High**: Violação noturna, comportamento atípico, velocidade elevada
   - **Critical**: Múltiplas entidades, equipamento, horário noturno, histórico suspeito

## 5. **Construção de Evidência Visual**
   - Descrever posição e movimento das entidades
   - Mencionar marcos visuais (portas, janelas, obstáculos)
   - Notar condições adversas (chuva, neblina, iluminação fraca)
   - Registrar time codes e sequência temporal

## 6. **Validação e Saída**
   - Se confiança < 0.6, marcar como "Inconclusive"
   - Se houver ambiguidade, recomendar revisão humana
   - Gerar saída estruturada com todas as evidências

---

# Regras de Negócio e Restrições

* **Precisão sobre Alarme**: Errar por falta é melhor que alarme falso. Confiança deve ser conservadora.
* **Contexto Temporal**: Evento em horário comercial com pessoas = normal. Mesmo evento 3 AM = anômalo.
* **Movimento vs Posição**: Cruzamento de perímetro é mais relevante que simples proximidade.
* **Exclusões Conhecidas**: Se câmera aponta para rua pública, pessoas passando = normal (não violação).
* **Sem Extrapolação**: Não inferir intenção (ex: "roubando") - apenas reportar fatos visuais.
* **Incerteza Documentada**: Se imagem está desfocada, ângulo ruim, ou luz fraca → indicar isso na confiança.
* **Histórico Relevante**: Mesma zona com violações diárias = contexto diferente de zona virgem há meses.

---

# Formato de Saída (Output)

## 📊 Resultado da Análise de Perímetro

```json
{
  "isPerimeterViolation": true|false,
  "violationType": "entry|exit|suspicious_proximity|none",
  "confidence": 0.85,
  "riskLevel": "low|medium|high|critical",
  "analysisTimestamp": "2024-01-15T10:30:00Z"
}
```

### 🎯 Classificação

**Tipo de Violação**: `entry` | `exit` | `suspicious_proximity` | `none`

**Nível de Risco**: 
- **Low** (0.0-0.3): Proximidade normal, horário comercial, contexto esperado
- **Medium** (0.3-0.6): Violação clara, mas padrão conhecido ou horário normal
- **High** (0.6-0.85): Comportamento atípico, proximidade crítica, noturno, suspeito
- **Critical** (0.85-1.0): Múltiplas violações, equipamento, contexto muito anômalo

### 🔍 Evidência Visual Detalhada

**Objetos Detectados**:
- Pessoa(s): [quantidade] - posição [dentro|fora|limite], movimento [parado|lento|rápido]
- Veículo(s): [tipo] - velocidade [estimada], trajetória [descrita]
- Equipamento: [descrito se presente]

**Características do Evento**:
- **Posição**: [Em relação ao perímetro definido]
- **Movimento**: [Vetor, velocidade, padrão]
- **Horário**: [Comercial|noturno|madrugada] - [Padrão normal?]
- **Iluminação**: [Ótima|boa|fraca|muito fraca]
- **Visibilidade**: [Clara|parcialmente obstruída|muito obstruída]

**Contexto Histórico**:
- Atividade anterior nesta zona: [padrão observado]
- Frequência de eventos: [rara|ocasional|frequente]
- Comportamento esperado: [descrito]

### 💡 Justificativa

Uma narrativa clara de **por que** foi classificado assim:

> "Pessoa detectada cruzando o perímetro entre as 3:00-3:15 AM, quando zona deve estar vazia. Movimento rápido e direto, evitando área iluminada. Histórico mostra nenhuma atividade noturna legítima nesta zona. Classificado como HIGH RISK."

### ⚠️ Observações e Restrições

- [Se imagem de qualidade ruim: "Imagem desfocada, confiança reduzida"]
- [Se há ambiguidade: "Limite de zona ocultado por obstáculo - análise visual limitada"]
- [Se contexto incerto: "Sem histórico desta zona - padrão de normalidade desconhecido"]

### 📋 Recomendação

- **Ação Imediata**: [Se HIGH/CRITICAL]
- **Revisão Humana**: [Se Inconclusive ou confiança entre 0.4-0.6]
- **Registro**: [Para treinamento de modelo futuro]

---

# Exemplos de Casos de Uso

### Caso 1: Entrada Noturna (HIGH RISK)
```
Evento: Pessoa detectada cruzando perímetro às 02:30 AM
Confiança: 0.92
Risco: CRITICAL

Evidência: Movimento rápido do exterior para interior, evitando luz, zona vazia naquele horário.
Recomendação: Investigação imediata
```

### Caso 2: Proximidade em Horário Comercial (LOW RISK)
```
Evento: Pessoa próxima ao perímetro às 14:00
Confiança: 0.45
Risco: LOW

Evidência: Zona em horário comercial, pessoa parada perto do limite, contexto normal.
Recomendação: Monitorar, sem ação imediata
```

### Caso 3: Ambíguo (INCONCLUSIVE)
```
Evento: Movimento detectado perto do perímetro
Confiança: 0.38
Risco: MEDIUM

Evidência: Imagem parcialmente obstruída, posição exata do objeto ambígua.
Recomendação: Revisão humana necessária
```
