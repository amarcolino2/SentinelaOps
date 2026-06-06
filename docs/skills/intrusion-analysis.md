---
name: intrusion-analysis
description: Detecta e classifica tentativas de intrusão analisando comportamento, ferramentas, métodos e contexto de risco.
version: 1.0.0
category: event-analysis
priority: P0
inputs:
  - image: "JPEG do evento"
  - metadata: "Localização, timestamp, tipo de acesso (porta, janela, cerca)"
  - securityContext: "Pontos de entrada conhecidos, histórico de tentativas"
outputs:
  - isIntrusionAttempt: "boolean"
  - intrusionType: "forced_entry|unauthorized_access|suspicious_behavior"
  - confidence: "0.0-1.0"
  - toolsDetected: "Equipamento usado para intrusão"
  - threatLevel: "low|medium|high|critical"
---

# Fluxo de Trabalho (Workflow)

Detecta e classifica tentativas de intrusão com análise comportamental, visual de ferramentas e contexto de segurança.

## 1. **Análise do Ponto de Entrada**
   - Identificar qual ponto de acesso foi alvo (porta, janela, cerca, teto, etc)
   - Verificar se está blindado, fechado ou desprotegido
   - Confirmar se é ponto de entrada legítimo ou não autorizado
   - Recuperar histórico: quantas vezes foi alvo, padrão de ataques

## 2. **Detecção de Ferramentas e Métodos**
   - Detectar se há ferramentas de intrusão visíveis (pé-de-cabra, chave inglesa, furadeira)
   - Analisar sinais de força: danos, arranhões, marcas de arrombamento
   - Observar técnica: quebra rápida vs lenta, acessórios profissionais vs improvisados
   - Classificar nível de sofisticação: amador vs profissional

## 3. **Análise Comportamental**
   - Observar se ator está disfarçado ou ocultando rosto/identidade
   - Verificar se foge quando detectado ou continua tranquilo
   - Analisar padrão: agir sozinho vs em grupo, coordenação
   - Notar familiaridade: conhecimento prévio do local vs exploração aleatória

## 4. **Análise do Timing**
   - Registrar hora do evento (madrugada = mais suspeito)
   - Considerar dia da semana e sazonalidade
   - Verificar se coincide com períodos de menor vigilância
   - Correlacionar com atividades legítimas esperadas

## 5. **Cálculo de Nível de Ameaça**
   - **Low**: Tentativa clara mas amadora, ferramentas improvisadas, contexto menos crítico
   - **Medium**: Intrusão bem executada, mas sem equipamento pesado, horário comercial
   - **High**: Intrusão profissional, ferramentas especializadas, comportamento coordenado
   - **Critical**: Múltiplos atores, equipamento avançado, alvo crítico (safe, servidor), padrão de crime organizado

## 6. **Validação de Risco**
   - Comparar com tentativas anteriores no local
   - Verificar se há padrão de escalação
   - Considerar valor do ativo visado
   - Alertar se há risco imediato a pessoas

---

# Regras de Negócio e Restrições

* **Sem Falsas Acusações**: Manutenção legítima ou reparos autorizados não são intrusão.
* **Intenção Importa**: Chute na porta = claramente intrusivo. Pessoa vitrando janela = ambíguo.
* **Ferramentas Decisivas**: Presença de pé-de-cabra, chave inglesa ou máscara = altamente suspeito.
* **Contexto de Acesso**: Se ponto foi explicitamente fechado/selado = toda tentativa é intrusão.
* **Sem Discriminação**: Não assumir intenção criminosa por aparência (raça, classe, etc) - análise visual objetiva.
* **Histórico Relevante**: Porta vitrando 1x = baixa probabilidade. Mesma porta 5x em 6 meses = padrão claro.
* **Ambiguidade Confessa**: Se impossível determinar intenção = marcar como Inconclusive.

---

# Formato de Saída (Output)

## 📊 Resultado da Análise de Intrusão

```json
{
  "isIntrusionAttempt": true|false,
  "intrusionType": "forced_entry|unauthorized_access|suspicious_behavior|none",
  "confidence": 0.88,
  "threatLevel": "low|medium|high|critical",
  "analysisTimestamp": "2024-01-15T10:30:00Z"
}
```

### 🎯 Classificação

**Tipo de Intrusão**: `forced_entry` | `unauthorized_access` | `suspicious_behavior` | `none`

**Nível de Ameaça**:
- **Low** (0.0-0.3): Tentativa visível mas clara, amadora, ferramentas improvisadas
- **Medium** (0.3-0.6): Intrusão bem executada, horário normal, equipamento básico
- **High** (0.6-0.85): Profissional, especializado, comportamento coordenado, horário crítico
- **Critical** (0.85-1.0): Equipamento avançado, múltiplos atores, alvo crítico, padrão organizado

### 🔍 Evidência Técnica

**Ponto de Entrada**:
- Localização: [porta | janela | cerca | teto | outro]
- Status: [intacto | arranhado | dano leve | dano severo | aberto]
- Histórico: [nunca alvo | alvo ocasional | alvo frequente | alvo múltiplas vezes]

**Ferramentas e Método**:
- Ferramentas detectadas: [listadas ou "nenhuma visível"]
- Sinais de força: [nenhum | leve | moderado | severo]
- Técnica: [amateurish | semi-profissional | profissional]

**Comportamento do Ator**:
- Quantidade: [1 pessoa | múltiplos indivíduos]
- Disfarce: [nenhum | parcial | completo]
- Reação: [calmo | apressado | agressivo | fuga]
- Coordenação: [ação individual | coordenada | comunicação observada]

**Contexto Temporal**:
- Horário: [comercial | noturno | madrugada]
- Dia: [útil | fim de semana | feriado]
- Sincronismo: [coincide com menor vigilância? | comportamento padrão?]

### 💡 Justificativa

Narrativa clara do raciocínio:

> "Dois indivíduos detectados com pé-de-cabra tentando forçar porta de acesso traseiro às 2:47 AM. Porta foi explicitamente selada semana anterior. Comportamento coordenado, disfarçados, fuga quando luz ativada. Correspondência exata com tentativa similar 6 meses atrás. Classificado como CRITICAL THREAT - padrão de crime organizado."

### ⚠️ Observações

- [Se identificável: "Rosto parcialmente visível - passível de identificação"]
- [Se contexto incerto: "Equipamento em cena poderia ser legítimo (manutenção) - verificar autorização"]
- [Se padrão: "Quarta tentativa em 8 semanas neste ponto - escalação clara"]

### 📋 Recomendação

- **Ação Imediata**: [Ativar protocolo de segurança, contato policial]
- **Investigação**: [Análise forense, revisão de câmeras adjacentes]
- **Registro**: [Padrão crime, associação com tentativas anteriores]

---

# Exemplos de Casos de Uso

### Caso 1: Intrusão Profissional (CRITICAL)
```
Evento: Dois indivíduos com ferramentas especializadas forçando porta selada
Confiança: 0.95
Ameaça: CRITICAL

Evidência: Pé-de-cabra, comportamento coordenado, disfarçados, madrugada, padrão anterior.
Recomendação: Ação imediata - contato policial, revisão de câmeras
```

### Caso 2: Tentativa Amadora (LOW)
```
Evento: Pessoa tentando abrir janela
Confiança: 0.52
Ameaça: LOW

Evidência: Mãos nuas, sem ferramentas, comportamento tímido, horário noturno mas tentativa fraca.
Recomendação: Vigilância, mas padrão típico de tentativa inexperiente
```

### Caso 3: Manutenção Legítima Ambígua (INCONCLUSIVE)
```
Evento: Pessoa com ferramenta próxima à porta selada
Confiança: 0.38
Ameaça: MEDIUM

Evidência: Ferramenta poderia ser chave inglesa (manutenção) ou pé-de-cabra (intrusão) - qualidade de imagem ruim.
Recomendação: Revisar autorização de manutenção para este horário
```
