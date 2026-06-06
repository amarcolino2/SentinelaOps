# Requisitos Funcionais

## Escopo de Requisitos Funcionais

Os requisitos funcionais definem **O QUÊ** o sistema deve fazer, independente de como.

Cada requisito segue o padrão:
- **REQ-ID**: Identificador único e rastreável
- **Título**: Breve descrição
- **Descrição**: Detalhes funcionais
- **Critério de Aceite**: Condições de sucesso objetivas
- **Bounded Context**: Qual contexto implementa
- **Prioridade**: P0 (crítico), P1 (alto), P2 (médio), P3 (baixo)

---

## Receptor de Eventos (Event Receiver Context)

### REQ-001: Receber Eventos de Videomonitoramento
**Prioridade**: P0
**Bounded Context**: Event Receiver, Monitoring Provider Adapter

**Descrição**:
O sistema deve receber eventos provenientes de qualquer sistema de monitoramento capaz de fornecer:
- Imagens JPEG (evidência visual)
- Metadados (timestamp, zona, câmera, etc)
- Eventos analíticos (movimento detectado, etc)
- Eventos de perímetro (intruso em perímetro)
- Eventos de intrusão (intruso dentro de área)
- Eventos personalizados (definidos pelo cliente)

**Critério de Aceite**:
- ✅ Aceita POST `/api/events` com JSON payload
- ✅ Valida presença de imagem + metadados obrigatórios
- ✅ Retorna 202 Accepted para eventos válidos
- ✅ Armazena evento em fila de processamento
- ✅ Retorna EventId para rastreamento
- ✅ Suporta múltiplas fontes de evento sem acoplamento

**Exemplo de Payload**:
```json
{
  "source": "camera-zone-5",
  "eventType": "intrusion",
  "timestamp": "2024-01-15T10:30:00Z",
  "imageBase64": "...",
  "metadata": {
    "zone": "5",
    "sensorId": "SENSOR-05",
    "sensitivity": "high"
  },
  "analyticsData": {
    "motionDetected": true,
    "movementDirection": "left-to-right"
  }
}
```

---

### REQ-002: Validar Eventos na Recepção
**Prioridade**: P0
**Bounded Context**: Event Receiver

**Descrição**:
Validações iniciais de integridade devem ocorrer na recepção para falhar rápido.

**Critério de Aceite**:
- ✅ Validar presença de campos obrigatórios
- ✅ Validar formato de imagem (JPEG, tamanho máximo 5MB)
- ✅ Validar timestamp não está no futuro
- ✅ Validar campos metadata obrigatórios
- ✅ Retornar 400 Bad Request para eventos inválidos
- ✅ Registrar erro com EventId para auditoria

---

## Orquestração de Analysis (Skill Orchestrator Context)

### REQ-003: Orquestrar Execução de Skills
**Prioridade**: P0
**Bounded Context**: Skill Orchestrator

**Descrição**:
O sistema deve executar uma sequência de Skills em pipeline para analisar evento.

**Critério de Aceite**:
- ✅ Lê configuração de pipeline de Skills
- ✅ Executa Skills em ordem sequencial
- ✅ Passa output de uma Skill como input para próxima
- ✅ Coleta resultados de todas as Skills
- ✅ Trata falha de Skill sem abandonar pipeline
- ✅ Registra tempo de execução de cada Skill
- ✅ Permite diferentes pipelines por tipo de evento

**Pipeline Padrão**:
```
Evento
  ↓
PerimeterAnalysisSkill
  ↓
FalsePositiveAnalysisSkill
  ↓
SeverityClassificationSkill
  ↓
IncidentSummarySkill
  ↓
Resultado Final
```

---

### REQ-004: Executar Skills com Resiliência
**Prioridade**: P1
**Bounded Context**: Skill Orchestrator

**Descrição**:
Garantir que falha de uma Skill não quebra todo o pipeline.

**Critério de Aceite**:
- ✅ Skill com timeout > 5s é cancelada
- ✅ Falha de Skill é registrada mas não falha pipeline
- ✅ Próxima Skill recebe resultado parcial
- ✅ Resultado final inclui informação de qual Skill falhou
- ✅ Métricas de falha são registradas

---

## Analysis Skills Context

### REQ-005: Implementar PerimeterAnalysisSkill
**Prioridade**: P0
**Bounded Context**: Skills, Inference Provider

**Descrição**:
Analisar se evento está dentro ou fora do perímetro definido.

**Critério de Aceite**:
- ✅ Recebe imagem + zona + definição de perímetro
- ✅ Usa IA para classificar localização
- ✅ Retorna: `{ location: "inside|outside|uncertain", confidence: 0.0-1.0 }`
- ✅ Justifica decisão com evidências da imagem
- ✅ Não acoplado a modelo específico

**Exemplo de Resultado**:
```json
{
  "skillName": "PerimeterAnalysisSkill",
  "classification": "inside",
  "confidence": 0.92,
  "justification": "Figura humana detectada claramente dentro da zona de perímetro definida",
  "evidence": [
    "Posição horizontal: 0.3-0.7",
    "Posição vertical: 0.2-0.8",
    "Distância estimada: 3-5m"
  ]
}
```

---

### REQ-006: Implementar IntrusionAnalysisSkill
**Prioridade**: P0
**Bounded Context**: Skills, Inference Provider

**Descrição**:
Analisar se evento constitui uma intrusão (entrada não autorizada).

**Critério de Aceite**:
- ✅ Recebe imagem + contexto de zona + histórico
- ✅ Classifica como: `intrusion | authorized_entry | suspicious | unclear`
- ✅ Retorna confidence score 0.0-1.0
- ✅ Fornece justificativa e evidências
- ✅ Considera horário (diferentes regras para noite/dia)

---

### REQ-007: Implementar FalsePositiveAnalysisSkill
**Prioridade**: P0
**Bounded Context**: Skills, Inference Provider

**Descrição**:
Analisar probabilidade de ser um falso positivo.

**Critério de Aceite**:
- ✅ Recebe resultados de Skills anteriores
- ✅ Analisa padrões que indicam falso positivo:
  - Movimento de árvore/folhas
  - Reflexo de luz
  - Animal pequeno
  - Artefatos de câmera
- ✅ Retorna: `{ probabilityFalsePositive: 0.0-1.0, reason: string }`
- ✅ Justifica cada conclusão

---

### REQ-008: Implementar SeverityClassificationSkill
**Prioridade**: P0
**Bounded Context**: Skills, Inference Provider

**Descrição**:
Classificar severidade do evento em níveis de risco.

**Critério de Aceite**:
- ✅ Recebe análise completa até este ponto
- ✅ Classifica como: `critical | high | medium | low | informational`
- ✅ Baseia-se em múltiplos fatores:
  - Tipo de evento (intrusão > movimento)
  - Localização (área sensível > geral)
  - Padrão (primeiro evento > série)
  - Histórico (zona problemática > zona calma)
- ✅ Retorna score de severidade 0.0-1.0

---

### REQ-009: Implementar IncidentSummarySkill
**Prioridade**: P0
**Bounded Context**: Skills, Inference Provider

**Descrição**:
Gerar resumo operacional em linguagem natural.

**Critério de Aceite**:
- ✅ Sintetiza análise completa em parágrafo
- ✅ Inclui: o quê, onde, quando, por que, confiança
- ✅ Fornece recomendação de ação
- ✅ Texto claro e objetivopara operador ler em < 10 segundos
- ✅ Exemplo:
  ```
  "Pessoa detectada dentro do perímetro da zona 5 com alta confiança (94%).
  Não há indicadores de falso positivo. Recomendação: investigação imediata."
  ```

---

## Inference Harness Context

### REQ-010: Comparar Múltiplos Modelos em Paralelo
**Prioridade**: P1
**Bounded Context**: Inference Harness

**Descrição**:
Sistema permite executar mesmo evento contra múltiplos modelos e comparar resultados.

**Critério de Aceite**:
- ✅ Suporta múltiplos modelos: Gemma, Qwen, Llama, OpenAI
- ✅ Executa em paralelo
- ✅ Coleta métricas: resultado, confiança, tempo, tokens, CPU, memória
- ✅ Exibe comparação lado-a-lado
- ✅ Histórico de comparações para análise

**Exemplo**:
```
Evento: Intrusão Zona 5

Gemma 3 4B     → intrusion, conf=0.87, time=1.2s, tokens=245
Qwen VL        → intrusion, conf=0.91, time=1.8s, tokens=312
Llama Vision   → suspicious,  conf=0.72, time=2.1s, tokens=289
OpenAI GPT-4V  → intrusion, conf=0.96, time=0.8s, tokens=198
```

---

### REQ-011: Rastrear Histórico de Prompts
**Prioridade**: P1
**Bounded Context**: Inference Harness

**Descrição**:
Sistema mantém histórico de todos os prompts e versões.

**Critério de Aceite**:
- ✅ Cada prompt tem versão semântica (v1.0.0)
- ✅ Registra: conteúdo, modelo, data, autor
- ✅ Permite reverter para versão anterior
- ✅ Exibe diff entre versões
- ✅ Rastreia desempenho por versão de prompt

---

## Persistência (Persistence Context)

### REQ-012: Armazenar Eventos Originais
**Prioridade**: P0
**Bounded Context**: Persistence

**Descrição**:
Preservar evento original recebido para auditoria.

**Critério de Aceite**:
- ✅ Armazena JSON original do evento
- ✅ Armazena imagem JPEG original
- ✅ Armazena timestamp recebimento
- ✅ Implementa index por EventId para busca rápida
- ✅ Independente de banco (funciona com SQLite, SQL Server, PostgreSQL)

---

### REQ-013: Armazenar Resultados de Inferência
**Prioridade**: P0
**Bounded Context**: Persistence

**Descrição**:
Persistir resultado completo de análise para auditoria e histórico.

**Critério de Aceite**:
- ✅ Armazena resultado de cada Skill
- ✅ Armazena classificação final
- ✅ Armazena confiança geral
- ✅ Armazena prompt usado (versão)
- ✅ Armazena modelo IA usado
- ✅ Armazena tempo de processamento
- ✅ Permite buscar por CorrelationId

---

### REQ-014: Armazenar Ações Operacionais
**Prioridade**: P1
**Bounded Context**: Persistence

**Descrição**:
Registrar todas as ações tomadas por operadores.

**Critério de Aceite**:
- ✅ EventId → Ação operacional (dismiss, escalate, investigate)
- ✅ Timestamp e usuário
- ✅ Motivo da ação
- ✅ Tempo entre recomendação e ação
- ✅ Permite análise de efetividade

---

## API (API Context)

### REQ-015: API REST para Recebimento de Eventos
**Prioridade**: P0
**Bounded Context**: API

**Descrição**:
Endpoint HTTP REST para receber eventos.

**Critério de Aceite**:
- ✅ POST `/api/v1/events` → 202 Accepted
- ✅ Documentado em OpenAPI 3.0
- ✅ Retorna Location header com URI do evento
- ✅ Timeout de resposta: < 500ms
- ✅ Suporta Basic Auth ou JWT

---

### REQ-016: API para Consultar Resultado de Evento
**Prioridade**: P0
**Bounded Context**: API

**Descrição**:
Endpoint para operador consultar resultado análise.

**Critério de Aceite**:
- ✅ GET `/api/v1/events/{eventId}` → 200 OK
- ✅ Retorna análise completa em JSON
- ✅ Inclui status de processamento
- ✅ Retorna 202 se ainda processando
- ✅ Retorna 404 se não encontrado

---

### REQ-017: API para Registrar Ação Operacional
**Prioridade**: P1
**Bounded Context**: API

**Descrição**:
Endpoint para operador registrar decisão tomada.

**Critério de Aceite**:
- ✅ POST `/api/v1/events/{eventId}/action`
- ✅ Body: `{ action: "dismiss|escalate|investigate", reason: string }`
- ✅ Registra operador, timestamp, decisão
- ✅ Retorna 200 OK

---

## Observabilidade

### REQ-018: Registrar Todos os Eventos em Structured Logging
**Prioridade**: P1
**Bounded Context**: Infrastructure, Observability

**Descrição**:
Logging estruturado em JSON com CorrelationId.

**Critério de Aceite**:
- ✅ Cada log inclui CorrelationId
- ✅ Formato JSON estruturado
- ✅ Níveis: DEBUG, INFO, WARN, ERROR
- ✅ CorrelationId rastreia evento desde recebimento até resultado final
- ✅ Logs persistidos em arquivo ou ElasticSearch

---

### REQ-019: Instrumentar com OpenTelemetry
**Prioridade**: P1
**Bounded Context**: Infrastructure, Observability

**Descrição**:
Traces distribuídos de todo processamento.

**Critério de Aceite**:
- ✅ Cada operação é um Span com CorrelationId
- ✅ Spans relacionadas hierarquicamente
- ✅ Exporta para Jaeger ou zipkin
- ✅ Rastreia latência de cada camada
- ✅ Rastreia falhas com stack traces

---

### REQ-020: Prover Health Checks
**Prioridade**: P2
**Bounded Context**: API, Infrastructure

**Descrição**:
Endpoints para monitoramento de saúde do sistema.

**Critério de Aceite**:
- ✅ GET `/health` → verifica todos componentes
- ✅ GET `/health/ready` → ready probe (k8s)
- ✅ GET `/health/live` → liveness probe (k8s)
- ✅ Valida conexões: RabbitMQ, Banco de dados, Ollama
- ✅ Retorna status detalhado por componente

---

## Segurança

### REQ-021: Autenticação via JWT
**Prioridade**: P1
**Bounded Context**: API, Security

**Descrição**:
APIs protegidas requerem JWT válido.

**Critério de Aceite**:
- ✅ Gerencia tokens JWT assinados
- ✅ Suporta refresh tokens
- ✅ Valida assinatura e expiration
- ✅ Retorna 401 Unauthorized sem token
- ✅ Retorna 403 Forbidden se expirado

---

### REQ-022: Autorização Baseada em Roles (RBAC)
**Prioridade**: P1
**Bounded Context**: API, Security

**Descrição**:
Controle de acesso baseado em papéis.

**Critério de Aceite**:
- ✅ Papéis: Admin, Operator, Analyst
- ✅ Cada endpoint requer role específico
- ✅ Operador só vê sua zona
- ✅ Admin vê tudo e pode alterar prompts
- ✅ Retorna 403 se sem permissão

---

### REQ-023: Auditoria de Todas as Ações
**Prioridade**: P1
**Bounded Context**: Persistence, Security

**Descrição**:
Registro imutável de quem fez o quê e quando.

**Critério de Aceite**:
- ✅ Cada ação registra: usuário, timestamp, operação, dados
- ✅ Impossível alterar registros
- ✅ Rastreabilidade de quem classificou evento
- ✅ Rastreabilidade de quem alterou prompt
- ✅ Exportável para auditoria externa

---

## Resumo de Requisitos Funcionais

| REQ-ID | Título | Prioridade | Status |
|--------|--------|-----------|--------|
| REQ-001 | Receber Eventos | P0 | Spec ✓ |
| REQ-002 | Validar Eventos | P0 | Spec ✓ |
| REQ-003 | Orquestrar Skills | P0 | Spec ✓ |
| REQ-004 | Resiliência | P1 | Spec ✓ |
| REQ-005 | PerimeterAnalysisSkill | P0 | Spec ✓ |
| REQ-006 | IntrusionAnalysisSkill | P0 | Spec ✓ |
| REQ-007 | FalsePositiveAnalysisSkill | P0 | Spec ✓ |
| REQ-008 | SeverityClassificationSkill | P0 | Spec ✓ |
| REQ-009 | IncidentSummarySkill | P0 | Spec ✓ |
| REQ-010 | Comparar Modelos | P1 | Spec ✓ |
| REQ-011 | Histórico de Prompts | P1 | Spec ✓ |
| REQ-012 | Armazenar Eventos | P0 | Spec ✓ |
| REQ-013 | Armazenar Inferências | P0 | Spec ✓ |
| REQ-014 | Armazenar Ações | P1 | Spec ✓ |
| REQ-015 | API Recebimento | P0 | Spec ✓ |
| REQ-016 | API Consulta | P0 | Spec ✓ |
| REQ-017 | API Ação | P1 | Spec ✓ |
| REQ-018 | Structured Logging | P1 | Spec ✓ |
| REQ-019 | OpenTelemetry | P1 | Spec ✓ |
| REQ-020 | Health Checks | P2 | Spec ✓ |
| REQ-021 | JWT Auth | P1 | Spec ✓ |
| REQ-022 | RBAC | P1 | Spec ✓ |
| REQ-023 | Auditoria | P1 | Spec ✓ |

---

## Fluxo de Rastreabilidade

```
Objetivo (OBJ-001: Reduzir Falsos Positivos)
  ├─ REQ-007 (FalsePositiveAnalysisSkill)
  ├─ REQ-010 (Comparar Modelos)
  ├─ REQ-011 (Histórico Prompts)
  └─ REQ-013 (Armazenar Inferências)

Objetivo (OBJ-002: Acelerar Decisão)
  ├─ REQ-003 (Orquestrar Skills)
  ├─ REQ-009 (IncidentSummarySkill)
  ├─ REQ-016 (API Consulta)
  └─ REQ-017 (API Ação)

Objetivo (OBJ-006: Auditabilidade)
  ├─ REQ-012 (Armazenar Eventos)
  ├─ REQ-013 (Armazenar Inferências)
  ├─ REQ-014 (Armazenar Ações)
  ├─ REQ-018 (Structured Logging)
  └─ REQ-023 (Auditoria)
```
