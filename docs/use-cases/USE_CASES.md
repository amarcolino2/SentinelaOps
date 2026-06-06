# Casos de Uso

## Overview de Casos de Uso

Os Casos de Uso descrevem fluxos de interação entre atores e o sistema.

### Atores Identificados

1. **Operador de Monitoramento**: Monitora câmeras, toma decisões
2. **Administrador do Sistema**: Configura, gerencia usuários, modelos
3. **Analyst de Segurança**: Revisa eventos, calibra confiança
4. **Sistema Externo de Monitoramento**: CCTV, sensor, alarme
5. **Modelo de IA (Ollama)**: Processa inferência
6. **Pesquisador**: Executa experimentos no Harness

---

## UC-001: Operador Recebe e Analisa Evento

**Descrição**: Fluxo principal de uso da plataforma.

**Atores Envolvidos**:
- Operador (primário)
- Sistema Sentinela Ops
- Ollama (secundário)

**Pré-condições**:
- ✅ Operador logado no sistema
- ✅ Câmeras configuradas e transmitindo
- ✅ Ollama rodando e acessível
- ✅ Prompts de análise versionados

**Fluxo Principal**:

1. Sistema de Monitoramento (câmera, sensor) detecta evento
2. Sistema Sentinela recebe evento (POST `/api/v1/events`)
3. Sistema valida evento (formato, campos obrigatórios)
4. Sistema enfileira evento para processamento
5. Worker retira evento da fila
6. Worker executa Skill Pipeline:
   - PerimeterAnalysisSkill
   - FalsePositiveAnalysisSkill
   - SeverityClassificationSkill
   - IncidentSummarySkill
7. Worker persiste resultado completo (evento + análise)
8. API retorna resultado quando consultado
9. Operador recebe notificação (push/pull) do novo evento
10. Operador consulta resultado (GET `/api/v1/events/{eventId}`)
11. Operador vê:
    - Classificação final
    - Confiança geral
    - Resumo em linguagem natural
    - Recomendação de ação
    - Evidências por Skill
12. Operador toma decisão:
    - Dismiss (falso positivo)
    - Escalate (investigação)
    - Investigate (ação imediata)
13. Operador registra ação (POST `/api/v1/events/{eventId}/action`)
14. Sistema persiste decisão para auditoria
15. ✅ **Fim**: Evento processado e decidido

**Fluxos Alternativos**:

**FA-001**: Se Skill falha
- Worker registra falha
- Próxima Skill recebe resultado parcial
- Resultado final inclui: "Skill X falhou"
- Operador vê aviso mas continua com análise parcial

**FA-002**: Se Ollama não responde
- Circuit breaker abre após 3 falhas
- Skill retorna resultado com confiança 0.0
- Mensagem: "IA temporariamente indisponível"

**FA-003**: Evento fica muito tempo em fila
- Se > 5 minutos, retorna HTTP 503
- Evento não é perdido, continua em fila
- Operador tenta novamente depois

**Pós-condições**:
- ✅ Evento persistido
- ✅ Análise disponível
- ✅ Ação registrada
- ✅ Auditoria completa

**Critério de Sucesso**:
- p95 latência < 2 segundos
- Confiança média > 0.7
- Operador toma decisão em < 30 segundos

---

## UC-002: Administrator Configura Novo Tipo de Skill

**Descrição**: Adicionar nova capacidade de análise (ex: VehicleAnalysisSkill).

**Atores Envolvidos**:
- Administrador (primário)
- Sistema Sentinela Ops
- Developer (para novo código de Skill)

**Pré-condições**:
- ✅ Administrador logado
- ✅ Skill implementada e testada

**Fluxo Principal**:

1. Administrador acessa `/admin/skills`
2. Clica "Register New Skill"
3. Preenche:
   - Nome: "VehicleAnalysisSkill"
   - Descrição: "Analisa presença de veículos"
   - Implementação: assembly name
   - Timeout: 5 segundos
   - Retry policy: 2 tentativas
4. Sistema valida:
   - Assembly existe
   - Implementa ISkill
   - Métodos obrigatórios presentes
5. Sistema registra Skill no catálogo
6. Administrador adiciona a um Pipeline
7. Seleciona posição no pipeline:
   - Após "IntrusionAnalysisSkill"
   - Antes "FalsePositiveAnalysisSkill"
8. Sistema salva configuração
9. ✅ **Fim**: Skill ativa para novos eventos

**Fluxos Alternativos**:

**FA-001**: Skill não implementa ISkill
- Sistema retorna erro
- Mostra lista de métodos faltando
- Administrador retorna para dev

**FA-002**: Skill falha em teste
- Sistema executa evento de teste
- Se falha, mostra stack trace
- Administrador pode aprovar mesmo assim (com aviso)

**Pós-condições**:
- ✅ Skill registrada
- ✅ Pipeline atualizado
- ✅ Novos eventos usam pipeline

**Critério de Sucesso**:
- Adicionar Skill < 5 minutos
- Zero modificação em código existente
- Sem redeploy necessário

---

## UC-003: Researcher Compara Modelos no Harness

**Descrição**: Executar mesmo evento contra múltiplos modelos para avaliar.

**Atores Envolvidos**:
- Researcher (primário)
- Sistema Harness
- Modelos (Gemma, Qwen, Llama, OpenAI)

**Pré-condições**:
- ✅ Acesso ao Harness
- ✅ Eventos de teste carregados
- ✅ Múltiplos modelos configurados

**Fluxo Principal**:

1. Researcher acessa `/harness`
2. Seleciona evento de teste
3. Seleciona modelo(s) para comparação:
   - [x] Gemma 3 4B
   - [x] Qwen VL
   - [x] Llama Vision
   - [ ] OpenAI (não disponível)
4. Sistema executa evento contra cada modelo
5. Coleta resultados e métricas:
   - Classificação final
   - Score de confiança
   - Tempo de resposta
   - Tokens usados
   - CPU/Memória consumida
6. Sistema gera relatório comparativo
7. Exibe lado-a-lado
8. Researcher vê:
   - Qual modelo mais rápido
   - Qual mais confiante
   - Qual menos custoso
   - Discrepâncias entre modelos
9. ✅ **Fim**: Dados para decisão

**Fluxos Alternativos**:

**FA-001**: Um modelo falha
- Sistema continua com outros
- Resultado final marca qual falhou

**FA-002**: Event Dataset muito grande
- Sistema processa em batches
- Exibe progresso e ETA

**Pós-condições**:
- ✅ Resultados persistidos
- ✅ Comparação disponível
- ✅ Histórico mantido

**Critério de Sucesso**:
- Comparação completa em < 10 segundos por evento
- Métricas precisas e auditáveis
- Resultado exportável em CSV/JSON

---

## UC-004: Analyst Calibra Confiança de Prompts

**Descrição**: Ajustar prompt para melhorar confiança de classificações.

**Atores Envolvidos**:
- Analyst (primário)
- Sistema Harness
- Harness

**Pré-condições**:
- ✅ Histórico de eventos com respostas reais
- ✅ Métricas de desempenho disponíveis
- ✅ Acesso ao editor de prompts

**Fluxo Principal**:

1. Analyst vê métrica: "Falso positivos 25%" (alvo: 15%)
2. Acessa `/harness/prompts`
3. Seleciona prompt "PerimeterAnalysis v1.0.0"
4. Vê histórico:
   - v1.0.0: precision 0.75
   - v1.0.1: precision 0.78
   - v1.1.0: precision 0.82 (current)
5. Clica "Edit" para criar v1.1.1
6. Modifica prompt:
   - Adiciona instrução: "Se borrão detectado, confiança < 0.5"
   - Adiciona exemplo de falso positivo
7. Clica "Test Against Dataset"
8. Sistema executa v1.1.1 contra 1000 eventos históricos
9. Compara com v1.1.0:
   - v1.1.0: precision 0.82, recall 0.88
   - v1.1.1: precision 0.85, recall 0.86
   - Melhoria: +3% precision, -2% recall
10. Analyst aprova (trade-off aceitável)
11. Sistema publica v1.1.1
12. ✅ **Fim**: Novo prompt em produção

**Fluxos Alternativos**:

**FA-001**: Prompt piora performance
- Sistema alerta: "Precision caiu 5%"
- Analyst pode desaprovar

**FA-002**: Prompt não muda significativamente
- Sistema sugere "Delta < 1%, talvez aguardar mais dados"

**Pós-condições**:
- ✅ Nova versão de prompt
- ✅ Métrica de desempenho registrada
- ✅ Eventos antigos rearranjáveis com novo prompt (auditável)

**Critério de Sucesso**:
- Testar prompt contra 1000 eventos em < 5 minutos
- Histórico completo de cada versão
- Rollback possível em < 1 minuto

---

## UC-005: Administrator Audita Decisões

**Descrição**: Revisar histórico de classificações e ações.

**Atores Envolvidos**:
- Administrator (primário)
- Sistema (secundário)

**Pré-condições**:
- ✅ Acesso ao módulo de auditoria
- ✅ Dados de eventos persistidos

**Fluxo Principal**:

1. Administrator acessa `/admin/audit`
2. Aplica filtros:
   - Data: últimos 7 dias
   - Zone: Zone 5
   - Classificação final: "PossibleFalsePositive"
   - Ação: "dismiss"
3. Sistema retorna 523 eventos
4. Administrator seleciona um evento
5. Vê rastreamento completo:
   - **Evento recebido**: 2024-01-15 10:30:00
   - **Validação**: OK
   - **Skills executadas**:
     - PerimeterAnalysisSkill: inside (0.92)
     - FalsePositiveAnalysisSkill: 0.78 (alta prob)
     - SeverityClassificationSkill: low
     - IncidentSummarySkill: resumo
   - **Prompt usado**: PerimeterAnalysis v1.1.0
   - **Modelo**: Ollama gemma3:4b
   - **CorrelationId**: uuid-xxx
   - **Tempo total**: 1.234s
   - **Ação registrada**: dismiss
   - **Usuário**: operator@domain
   - **Timestamp**: 2024-01-15 10:30:02
   - **Motivo**: "Reflexo detectado"
6. Administrator pode:
   - Exportar relatório
   - Comparar com outro modelo
   - Executar novamente com novo prompt
7. ✅ **Fim**: Auditoria completa

**Pós-condições**:
- ✅ Rastreamento imutável
- ✅ Relatório gerado
- ✅ Dados para compliance

**Critério de Sucesso**:
- Recuperar evento em < 1 segundo
- Mostrar rastreamento completo
- Exportar em 100+ formatos (CSV, JSON, PDF)

---

## Matriz de Rastreabilidade: Use Case → Requisitos

| UC | Requisitos Funcionais | Requisitos Não-Funcionais |
|----|---|---|
| UC-001 | REQ-001, REQ-002, REQ-003, REQ-004, REQ-005 a REQ-009, REQ-012, REQ-013, REQ-015, REQ-016, REQ-017 | NFRE-P-001, NFRE-P-002, NFRE-R-001 a NFRE-R-004, NFRE-O-001 a NFRE-O-004 |
| UC-002 | REQ-003, REQ-004 | NFRE-M-001, NFRE-M-002, NFRE-M-003, NFRE-M-004 |
| UC-003 | REQ-010, REQ-011, REQ-012, REQ-013 | NFRE-P-001, NFRE-P-002, NFRE-S-001 |
| UC-004 | REQ-011, REQ-012, REQ-013, REQ-019 | NFRE-P-001, NFRE-O-001 a NFRE-O-003 |
| UC-005 | REQ-012, REQ-013, REQ-014, REQ-018, REQ-019, REQ-023 | NFRE-O-001, NFRE-SEC-005, NFRE-C-001 |

---

## Resumo de Casos de Uso

| Caso de Uso | Ator Primário | Frequência | Criticidade |
|---|---|---|---|
| UC-001 | Operador | Contínua (100+ eventos/dia) | **P0 (Crítica)** |
| UC-002 | Administrador | Semanal | P2 (Baixa) |
| UC-003 | Researcher | Diária | P1 (Alta) |
| UC-004 | Analyst | Semanal | P1 (Alta) |
| UC-005 | Administrator | Diária (compliance) | P1 (Alta) |

---

## Extensões Futuras (Fase 2+)

### UC-006: System Integrates with ONVIF/RTSP
**Descrição**: Fonte de eventos nativa de câmeras IP
**Status**: Futura (Fase 2)

### UC-007: Operator Visualizes Event on Video Feed
**Descrição**: Overlay de análise sobre vídeo ao vivo
**Status**: Futura (Fase 2)

### UC-008: System Auto-Escalates Critical Events
**Descrição**: Notificar supervisores automaticamente
**Status**: Futura (Fase 2)

### UC-009: Researcher Generates Synthetic Events
**Descrição**: Dataset de teste para treinar/testar
**Status**: Futura (Fase 3)

### UC-010: System Migrates Prompt to New Model
**Descrição**: Adaptar prompt ao trocar de modelo
**Status**: Futura (Fase 3)
