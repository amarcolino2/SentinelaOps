---
name: human-activity-analysis
description: Detecta e classifica atividade humana identificando comportamentos, estados emocionais aparentes e intenções suspeitas.
version: 1.0.0
category: event-analysis
priority: P1
inputs:
  - image: "JPEG do evento"
  - metadata: "Local, timestamp, contexto (trabalho, residencial, público)"
  - humanDetection: "Posição, pose, quantidade de pessoas"
outputs:
  - humanActivityDetected: "boolean"
  - activityType: "routine|suspicious|dangerous"
  - behaviorPatterns: "List of identified behaviors"
  - apparentState: "calm|agitated|aggressive|panicked"
  - riskAssessment: "low|medium|high|critical"
---

# Fluxo de Trabalho (Workflow)

## 1. **Detecção e Localização de Humanos**
   - Identificar presença de pessoas
   - Contar quantidade
   - Registrar posição (em área permitida? restrita?)
   - Verificar se horário e local correspondem a atividade esperada

## 2. **Análise de Pose e Postura**
   - Identificar pose (em pé, sentado, deitado, abaixado)
   - Observar postura (ereta, curvada, tensa)
   - Detectar gestos (apontando, sinalizando, escondendo)
   - Analisar orientação (olhando câmera, para trás, investigando)

## 3. **Detecção de Comportamento**
   - **Rotina**: Caminhar, conversar, trabalhar, descansar
   - **Suspeito**: Investigar objetos, violação de acesso, comportamento furtivo
   - **Perigoso**: Agressão, arma, comportamento hostil, corrida frenética

## 4. **Análise de Estado Emocional Aparente**
   - **Calmo**: Movimento controlado, respiração normal, expressão neutra
   - **Agitado**: Movimento acelerado, gestos rápidos, expressão tensa
   - **Agressivo**: Gestos ameaçadores, postura dominante, movimento direcionado
   - **Panicked**: Movimento erráti co, fuga, desespero aparente

## 5. **Análise de Intenção**
   - Comportamento exploratório vs predeterminado
   - Familiaridade com local (conhece caminho vs explora)
   - Esconder rosto ou identidade = suspeito
   - Interação com objetos restrito s = crítico

## 6. **Avaliação de Risco Geral**
   - Combinar pose, comportamento, estado emocional e contexto
   - Historicamente este tipo de atividade causou problema aqui?
   - Pessoa conhecida ou desconhecida?
   - Armas ou equipamento perigoso aparente?

---

# Regras de Negócio e Restrições

* **Sem Discriminação**: Não fazer julgamento baseado em aparência (raça, sexo, classe) - apenas comportamento observável.
* **Contexto é Tudo**: Pessoa correndo é suspeita em banco; normal em campo de esportes.
* **Privacidade Respeitada**: Descrever comportamento, não inferir pensamentos ou julgamentos morais.
* **Familiaridade Importa**: Pessoa comum no local é menos suspeita que desconhecido com comportamento idêntico.
* **Equipamento Visível**: Ferramentas, mochilas, bolsas em local restrito = aumentar alerta.
* **Sem Falsas Acusações**: Comportamento "nervoso" pode ser ansiedade legítima, não crime.

---

# Formato de Saída (Output)

## 📊 Resultado da Análise de Atividade Humana

```json
{
  "humanActivityDetected": true,
  "activityType": "routine|suspicious|dangerous",
  "apparentState": "calm|agitated|aggressive|panicked",
  "riskAssessment": "low|medium|high|critical",
  "analysisTimestamp": "2024-01-15T10:30:00Z"
}
```

### 🎯 Classificação de Atividade

**Tipo de Atividade**:
- **Routine**: Comportamento normal, esperado, rotineiro
- **Suspicious**: Desvio do padrão, comportamento investigativo, potencialmente perigoso
- **Dangerous**: Comportamento hostil, ameaçador, potencialmente violento

**Estado Aparente**:
- Calmo, Agitado, Agressivo, Panicked

### 👥 Detalhes da Detecção

**Quantidade de Pessoas**: [número]

**Pose e Postura**:
- Posição: [em pé | sentado | deitado | abaixado | outro]
- Postura: [ereta | curvada | tensa | relaxada]
- Orientação: [para câmera | para trás | lateral | investigando]

**Gestos Observados**:
- [Lista de gestos detectados: apontando, sinalizando, bloqueando rosto, etc]

**Comportamentos Identificados**:
- [Listagem de comportamentos observados com contexto]

### 💡 Interpretação

Narrativa clara:

> "Três pessoas em área restrita às 2:15 AM. Comportamento furtivo - investigando armários, cobrindo rostos, movimentos rápidos. Estado aparente: agitado. Nenhuma justificativa legítima para esta atividade no horário/local. Risco: CRITICAL."

### ⚠️ Observações e Contexto

- [Se em area permitida: "Atividade normal para zona em horário comercial"]
- [Se comportamento nervoso: "Pessoa pode estar nervosa legitimamente - sem evidência de crime"]
- [Se equipamento: "Mochila grande, luvas - equipamento consistente com roubo"]

### 📋 Recomendação

- **Ação**: [Investigação | Vigilância | Sem ação]
- **Verificação**: [Autorização de acesso | Identificação | Registro]

---

# Exemplos

### Caso 1: Atividade Suspeita (CRITICAL)
```
Tipo: Dangerous
Estado: Aggressive
Risco: CRITICAL

Análise: Duas pessoas investigando área restrita, bloqueando rostos, comportamento coordenado.
Recomendação: Ação imediata - revisar câmeras, contato segurança
```

### Caso 2: Nervosismo Legítimo (LOW)
```
Tipo: Routine
Estado: Agitated
Risco: LOW

Análise: Pessoa conhecida na zona autorizada, comportamento nervoso mas nada suspeito.
Recomendação: Sem ação - nervosismo comum em ambiente corporativo
```

### Caso 3: Atividade Rotineira (NORMAL)
```
Tipo: Routine
Estado: Calm
Risco: LOW

Análise: Funcionário em área autorizada, horário comercial, comportamento normal.
Recomendação: Sem alerta
```
