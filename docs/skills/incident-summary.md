---
name: incident-summary
description: Consolida análise de todas as skills gerando relatório executivo com conclusões e recomendações para operadores.
version: 1.0.0
category: event-synthesis
priority: P0
inputs:
  - allSkillsResults: "Consolidação de resultado de todas as 7 skills anteriores"
  - severityLevel: "Classificação final de severidade"
  - operatorContext: "Conhecimento do operador, histórico de incidentes"
outputs:
  - executiveSummary: "Resumo 1-3 frases"
  - detailedNarrative: "Descrição completa do incidente"
  - evidence: "Evidências visuais e técnicas"
  - recommendation: "Ação recomendada para operador"
  - followUpActions: "Próximos passos para investigação"
---

# Fluxo de Trabalho (Workflow)

## 1. **Consolidação de Dados**
   - Agregar resultados de todas as 7 skills de análise
   - Validar consistência entre skills (se há contradições?)
   - Construir narrativa cronológica do evento
   - Identificar os elementos-chave para operador humano

## 2. **Síntese para Leigo**
   - Traduzir linguagem técnica para operacional
   - Remover jargão de IA/ML
   - Focar em "o que aconteceu" não em "como a IA decidiu"
   - Estruturar para decisão humana rápida

## 3. **Construção de Narrative de Incidente**
   - **O quê**: Que tipo de evento ocorreu
   - **Onde**: Localização precisa (câmera, zona, zona geográfica)
   - **Quando**: Timestamp exato e horário contextual
   - **Quem/O Quê**: Entidades envolvidas (pessoas, veículos, objetos)
   - **Como**: Padrão de atividade e progressão dos eventos
   - **Porquê (Suspeito)**: Por que isto é considerado anômalo/suspeito
   - **Confiança**: Nível de certeza da análise

## 4. **Destaque de Evidências Críticas**
   - Selecionar 3-5 evidências mais relevantes
   - Descrever de forma visual e compreensível
   - Vincular evidência a conclusão (não é óbvio para humano)
   - Incluir timestamps e referências a câmeras

## 5. **Formulação de Recomendação Operacional**
   - Ação específica e acionável
   - Urgência clara (agora vs. próximas horas)
   - Quem executar (segurança? policial? gerenciam?)
   - O que observar (se investigar, quais sinais estão atentos?)

## 6. **Documentação de Follow-up**
   - Que informações adicionais ajudariam?
   - Que câmeras revisar?
   - Que pessoas contactar?
   - Como escalar se necessário?
   - Como registrar para treinamento futuro?

---

# Regras de Negócio e Restrições

* **Clareza Absoluta**: Operador humano toma decisão final - resumo deve ser compreensível para não-técnico.
* **Sem Alarmismo**: Descrever fatos, não dramatizar. "Pessoa em zona restrita" não "INTRUSÃO!!!".
* **Sem Desculpas para IA**: "A IA acredita que..." não é útil. Fatos que levam a conclusão sim.
* **Actionable Sempre**: Recomendação vaga é inútil. Específico: "Chamar segurança para revisar câmera 4" sim.
* **Rastreável**: Incluir referências que permitam operador validar análise depois.
* **Sem Invasão de Privacidade**: Descrever comportamento suspeito, não especulação sobre pensamentos.

---

# Formato de Saída (Output)

## 📊 Relatório de Incidente Consolidado

```json
{
  "incidentId": "unique-id",
  "timestamp": "2024-01-15T10:30:00Z",
  "severity": "critical",
  "executiveSummary": "Pessoas investigando perímetro de zona crítica madrugada com ferramentas suspeitas.",
  "recommendation": "investigate|lockdown|monitor",
  "analysisTimestamp": "2024-01-15T10:30:05Z"
}
```

---

## 📋 Sumário Executivo

**[Uma ou duas frases que um gerente entenderia sem contexto técnico]**

Exemplo:
> "Tentativa de intrusão detectada na zona de servidores às 2:47 AM. Múltiplos indicadores de ameaça - pessoas com ferramentas, comportamento coordenado, horário crítico. Investigação imediata recomendada."

---

## 🔍 Narrativa Detalhada do Incidente

### O Evento
[Descrição clara do que ocorreu, sem jargão técnico]

Exemplo:
> "Às 14:32 do dia 15 de janeiro, câmera frontal detectou três pessoas próximas à porta sul do perímetro. Comportamento: parado, observando câmeras (evitando contato visual). Um indivíduo segura o que parece ser uma ferramenta manual (possível pé-de-cabra). Não portam equipamento de trabalho legítimo (uniforme, crachá)."

### A Sequência Temporal
1. **14:30** - Presença de veículo desconhecido próximo ao perímetro
2. **14:31** - Três ocupantes descem do veículo
3. **14:32** - Aproximam-se da porta sul, começam a investigar
4. **14:33** - Um indivíduo extrai objeto da bolsa
5. **14:34** - Tentativa de força na porta detectada
6. **[Atual]** - Situação desenvolvendo em tempo real

### Por Que Isto é Suspeito
[Explicação clara de por que isto não é atividade normal]

Exemplo:
> "1. Acesso à zona sul é restrito a pessoal autorizado com crachá - nenhum presente.
> 2. Horário 14:30 é fora de expediente de manutenção autorizada.
> 3. Veículo não consta em lista de fornecedores/visitantes.
> 4. Comportamento evita câmeras intencionalmente (evita contato visual).
> 5. Ferramenta sugere intenção de força - não compatível com atividade legítima."

---

## 🎯 Evidências Críticas

### Evidência 1: Comportamento Coordenado
**O quê**: Três pessoas operando como unidade, comunicando silenciosamente
**Onde**: Perímetro sul, próximo a porta de acesso
**Quando**: 14:32-14:34
**Por quê importante**: Atividade coordenada é indicador de intenção premeditada, não atividade casual

### Evidência 2: Ferramenta Suspeita
**O quê**: Objeto manual extraído de bolsa, compatível com pé-de-cabra ou ferramenta de arrombamento
**Onde**: Mão do indivíduo #2
**Quando**: 14:33
**Por quê importante**: Ferramenta de força indica intenção de arrombamento, não acesso autorizado

### Evidência 3: Veículo Desconhecido
**O quê**: Sedan preto, placa não legível, não consta em registros
**Onde**: Estacionamento próximo à entrada sul
**Quando**: 14:30 (primeira detecção)
**Por quê importante**: Veículo desconhecido não autorizado sugere visitantes não esperados

### [Evidência 4]: [Se aplicável]
### [Evidência 5]: [Se aplicável]

---

## 💡 Análise de Risco Consolidado

| Skill | Resultado | Contribuição | Confiança |
|-------|-----------|--------------|-----------|
| Perimeter Analysis | VIOLATION | Pessoa cruzando limite não autorizado | 0.92 |
| Intrusion Analysis | HIGH THREAT | Método de força, comportamento profissional | 0.88 |
| Motion Analysis | ERRATIC | Movimento coordenado, alternar rapid/parado | 0.85 |
| Human Activity | DANGEROUS | Comportamento investigativo, evitar câmeras | 0.79 |
| Vehicle Analysis | UNAUTHORIZED | Veículo desconhecido, placa ilegível | 0.76 |
| False Positive | LOW PROBABILITY | Contexto claramente suspeito, não ambiental | 0.15 |
| Severity Classification | CRITICAL | Múltiplos indicadores, zona crítica, madrugada | - |

**Confiança Geral**: 0.87 (Verdadeiro Positivo Muito Provável)

---

## 📋 Recomendação Operacional

### Ação Imediata: **INVESTIGATE - ESCALATE**

**Para Fazer Agora (Próximos 5 Minutos)**:
1. ☐ Notificar segurança presencial - enviar agente para zona sul imediatamente
2. ☐ Ativar áudio (se disponível) - escutar comunicação entre indivíduos
3. ☐ Revisar câmera 4 (zona oeste) - confirmar veículo, placa, informações
4. ☐ Preparar contato com polícia - ter informações prontas se necessário

**Para Observar Durante Investigação**:
- Entidade tenta força? → LOCKDOWN IMEDIATO
- Entidade tenta fuga? → Registre placa, descrição
- Entidade desiste, sai do perímetro? → Continue observando, registre movimento

**Informações para Polícia (se necessário)**:
- Timestamp: 2024-01-15 14:32
- Localização: Perímetro sul, porta de acesso
- Descrição: 3 indivíduos, 1 ferramenta suspeita, veículo preto placa [ilegível]
- Câmeras: Footage câmeras 3, 4, 5 disponível

---

## 🔄 Próximas Etapas para Investigação

### Se Confirmado Incidente
1. Coletar e preservar footage das câmeras 3-5 (últimos 30 minutos antes/depois)
2. Rastrear veículo via câmeras externas - placa, direção, saída
3. Registrar identidades dos indivíduos (se identificáveis)
4. Documenta danos (se houver) para segurado
5. Fazer relatório formal para autoridades

### Se Falso Positivo Confirmado
1. Documentar por que foi mal classificado
2. Usar como dado de treinamento para modelo futuro
3. Ajustar sensibilidade de skill relevante (se necessário)
4. Notificar interessados (gerenciamento, segurança) da resolução

### Para Melhoria Contínua
- Este incidente será incluído em dataset de treinamento?
- Há padrão de escalação (quantidade tentativas em zona)?
- Necessário revisar configurações de câmera ou sensor?

---

## 📎 Referências

**Câmeras Envolvidas**: 3, 4, 5
**Zones Impactadas**: Perimeter Sud
**Timestamps Críticos**: 14:30, 14:32, 14:33, 14:34
**Arquivo de Footage**: [Link ou referência para footage]
**Relatório Completo**: [Link para análise técnica completa se existir]

---

# Exemplos

## Exemplo 1: Incidente Crítico
```
🚨 CRITICAL INCIDENT SUMMARY

Sumário: Intrusão detectada em zona de servidor madrugada com múltiplos indicadores profissionais.

Ação: LOCKDOWN + POLÍCIA

Evidências:
- Três indivíduos com ferramentas arrombamento
- Comportamento coordenado, evitar câmeras
- Madrugada, zona vazia esperada
- Padrão consistente com crime organizado anterior

Recomendação: Lockdown imediato, contato policial, preservar evidence
```

## Exemplo 2: Incidente Moderado
```
⚠️ ALERT INCIDENT SUMMARY

Sumário: Pessoa investigando perímetro durante horário comercial, comportamento suspeito.

Ação: INVESTIGATE

Evidências:
- Movimento circular próximo a perímetro
- Fotografando ou observando câmeras
- Desconhecido não presente em lista de visitantes
- Comportamento consistente com reconhecimento

Recomendação: Agente segurança contacte pessoa, verifique identidade, registre
```

## Exemplo 3: Falso Positivo
```
ℹ️ INFO - LIKELY FALSE POSITIVE

Sumário: Movimento detectado próximo a perímetro durante chuva, análise indica ambiental.

Ação: MONITOR

Evidências:
- Histórico: 100% false positives quando chuva nesta zona
- Câmera 2 qualidade ruim durante precipitação
- Comportamento sensor inconsistente
- Múltiplos alarmes em 5 minutos (típico ruído ambiental)

Recomendação: Descarte, monitorar continuamente, revisar calibração câmera após chuva
```
