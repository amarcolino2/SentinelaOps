# Requisitos Não-Funcionais

## Escopo de Requisitos Não-Funcionais

Os requisitos não-funcionais definem **COMO** o sistema deve se comportar em termos de qualidade, desempenho, segurança e operacionalidade.

Padrão:
- **NFREQ-ID**: Identificador único
- **Aspecto**: Performance | Segurança | Escalabilidade | Confiabilidade | Mantibilidade | Observabilidade
- **Métrica**: Medível e testável

---

## Performance

### NFRE-P-001: Latência de Processamento
**Aspecto**: Performance
**Métrica**: p95 latência < 2 segundos por evento

**Descrição**:
Tempo total do evento: recebimento → análise completa → resultado disponível.

**Critério de Aceite**:
- ✅ p50 (mediana) < 0.5s
- ✅ p95 < 2s
- ✅ p99 < 5s
- ✅ Inclui tempo de espera em fila

**Contexto**:
- Ollama (Gemma 3 4B) rodando em GPU local: 0.3-0.5s
- 5 Skills executadas sequencialmente: +1.0s
- Persistência em SQLite: +0.2s

**Teste**:
```
load_test.sh [events=1000] [concurrent=10]
→ Gera relatório com p50, p95, p99
```

---

### NFRE-P-002: Throughput Mínimo
**Aspecto**: Performance
**Métrica**: Processar > 10 eventos/segundo de forma sustentada

**Descrição**:
Sistema deve manter pelo menos 10 eventos por segundo continuamente.

**Critério de Aceite**:
- ✅ Pico: 20 eventos/segundo por até 1 minuto
- ✅ Sustentado: 10 eventos/segundo por 1 hora
- ✅ Sem degradação de latência com carga
- ✅ Sem perda de eventos (todos enfileirados)

**Teste**:
```
load_test.sh events=36000 concurrent=10 duration=3600
→ Verifica: throughput, latência, perda de eventos
```

---

### NFRE-P-003: Uso de Memória
**Aspecto**: Performance
**Métrica**: < 2GB de memória base, escalamento linear com carga

**Descrição**:
Aplicação não deve vazar memória ou consumir excesso.

**Critério de Aceite**:
- ✅ Baseline (idle): < 500MB
- ✅ Com 10 eventos/s: < 1.5GB
- ✅ Sem picos incomuns
- ✅ GC cleanup eficiente (picos volta à baseline em < 30s)

**Monitoramento**:
```
Prometheus: process_resident_memory_bytes
Grafana dashboard mostra tendência
```

---

### NFRE-P-004: Latência de Consulta de Banco
**Aspecto**: Performance
**Métrica**: p99 < 100ms por query

**Descrição**:
Operações de leitura e escrita em banco não devem ser gargalo.

**Critério de Aceite**:
- ✅ INSERT evento: p99 < 50ms
- ✅ SELECT evento por ID: p99 < 20ms
- ✅ SELECT últimos 100 eventos: p99 < 100ms
- ✅ Índices em lugar (EventId, timestamp, zone)

---

## Escalabilidade

### NFRE-S-001: Escalabilidade Horizontal de Workers
**Aspecto**: Escalabilidade
**Métrica**: Suportar múltiplas instâncias de Worker processando em paralelo

**Descrição**:
Adicionar Workers sem ponto único de falha.

**Critério de Aceite**:
- ✅ RabbitMQ distribui eventos para múltiplos workers
- ✅ Cada worker processa evento independentemente
- ✅ Sem race conditions no banco
- ✅ Escalável até 100 workers

**Configuração**:
```yaml
RabbitMQ:
  Queue: events
  Prefetch: 1
  Multiple Workers: consumer_tag auto-assigned
```

---

### NFRE-S-002: Escalabilidade Vertical do Ollama
**Aspecto**: Escalabilidade
**Métrica**: Ollama escala com GPU disponível

**Descrição**:
Aproveitar GPUs para acelerar inferência.

**Critério de Aceite**:
- ✅ Funciona com CPU (sem GPU): ~0.5s por evento
- ✅ Funciona com 1x GPU: ~0.2s por evento
- ✅ Funciona com 2x GPU: ~0.15s por evento
- ✅ Config automática detecta GPU

---

### NFRE-S-003: Independência de Banco de Dados
**Aspecto**: Escalabilidade
**Métrica**: Trocar banco (SQLite → PostgreSQL → SQL Server) sem código de domínio

**Descrição**:
Abstração de persistência permite escalar banco sem modificação de lógica.

**Critério de Aceite**:
- ✅ Domain layer não conhece SQLite
- ✅ Repository abstrai persistência
- ✅ Migrations automáticas aplicadas
- ✅ Testes passam com múltiplos bancos

---

## Confiabilidade & Resiliência

### NFRE-R-001: Disponibilidade de Produção
**Aspecto**: Confiabilidade
**Métrica**: 99.5% uptime mensal (< 3.6 horas downtime)

**Descrição**:
Sistema deve estar disponível para operadores.

**Critério de Aceite**:
- ✅ Health checks validam pré-requisitos
- ✅ Graceful shutdown permite terminar eventos em processamento
- ✅ Sem perda de dados em crash
- ✅ Recovery automático de falhas transientes

---

### NFRE-R-002: Circuit Breaker para Serviços Externos
**Aspecto**: Resiliência
**Métrica**: Falha de Ollama/RabbitMQ não quebra sistema

**Descrição**:
Sistema degrada gracefully se dependência cair.

**Critério de Aceite**:
- ✅ Circuit breaker ao Ollama
- ✅ Circuit breaker ao RabbitMQ
- ✅ Retry com backoff exponencial (1s → 2s → 4s → 8s)
- ✅ Fallback para modo degradado
- ✅ Max 3 retries, então fail-fast

**Estados**:
```
Closed (normal)
  → Falha detectada → Open (bloqueado)
  → Após 60s → Half-Open (testando)
  → Sucesso → Closed
  → Falha → Open
```

---

### NFRE-R-003: Dead Letter Queue para Eventos Falhados
**Aspecto**: Resiliência
**Métrica**: 100% rastreabilidade de eventos falhados

**Descrição**:
Eventos que falham são armazenados para análise posterior.

**Critério de Aceite**:
- ✅ RabbitMQ DLQ para eventos falhados
- ✅ Registra motivo da falha
- ✅ Permite reprocessamento manual
- ✅ Alerta se DLQ cresce (> 10 eventos/hora)

---

### NFRE-R-004: Idempotência de Processamento
**Aspecto**: Resiliência
**Métrica**: Reprocessar evento 2x = mesmo resultado

**Descrição**:
Falhas em rede ou aplicação não causam duplicação ou inconsistência.

**Critério de Aceite**:
- ✅ EventId único identifica evento
- ✅ Reprocessar evento não cria duplicate
- ✅ Timestamp de primeira processagem é preservado
- ✅ Resultado anterior é sobrescrito atomicamente

---

## Segurança

### NFRE-SEC-001: Criptografia em Trânsito
**Aspecto**: Segurança
**Métrica**: TLS 1.3 obrigatório para conexões HTTPS

**Descrição**:
Dados sensíveis (imagens, resultados) não viajam em plaintext.

**Critério de Aceite**:
- ✅ HTTPS com TLS 1.3
- ✅ Certificados válidos (Let's Encrypt ou CA corporativa)
- ✅ HSTS header ativo
- ✅ Sem fallback a HTTP
- ✅ RabbitMQ com AMQPS

---

### NFRE-SEC-002: Criptografia em Repouso (Opcional)
**Aspecto**: Segurança
**Métrica**: Imagens e resultados sensíveis criptografados no banco

**Descrição**:
Se banco é comprometido, dados são inutilizáveis.

**Critério de Aceite**:
- ✅ Campo de imagem encriptado AES-256
- ✅ Campo de resultado encriptado
- ✅ Chave mestra em variável de ambiente
- ✅ Overhead < 5% em performance

---

### NFRE-SEC-003: Validação de Entrada Rigorosa
**Aspecto**: Segurança
**Métrica**: 100% de entradas validadas

**Descrição**:
Prevenir injection attacks, XXE, etc.

**Critério de Aceite**:
- ✅ JSON validado contra schema
- ✅ Imagens validadas (magic bytes, size)
- ✅ Strings validadas (charset, length, pattern)
- ✅ Numeros validados (range, type)
- ✅ Testes de fuzzing passam

---

### NFRE-SEC-004: Não Armazenar Credenciais em Código
**Aspecto**: Segurança
**Métrica**: Zero credenciais em commits

**Descrição**:
Credenciais vêm apenas de variáveis de ambiente ou secrets manager.

**Critério de Aceite**:
- ✅ Git hooks bloqueiam commits com credentials
- ✅ Config lê de environment variables
- ✅ Suporte para Azure Key Vault
- ✅ Logs nunca exibem credenciais

---

### NFRE-SEC-005: Auditoria Imutável
**Aspecto**: Segurança
**Métrica**: Logs de auditoria não podem ser alterados

**Descrição**:
Para compliance (LGPD, GDPR, etc).

**Critério de Aceite**:
- ✅ Audit log armazenado separadamente
- ✅ Append-only (não permite UPDATE/DELETE)
- ✅ Hash chain para detectar alterações
- ✅ Exportável para sistema externo

---

## Observabilidade & Mantibilidade

### NFRE-O-001: Estrutura de Logs com CorrelationId
**Aspecto**: Observabilidade
**Métrica**: 100% dos logs têm CorrelationId

**Descrição**:
Rastrear evento completo através de toda cadeia de processamento.

**Critério de Aceite**:
- ✅ Cada request recebe correlationId único
- ✅ Propagado através de toda cadeia
- ✅ Formato: `{"correlationId": "uuid", ...}`
- ✅ Pesquisável em ELK/Splunk

---

### NFRE-O-002: Distributed Tracing
**Aspecto**: Observabilidade
**Métrica**: Traçar latência de cada componente

**Descrição**:
Entender onde tempo é gasto em processamento.

**Critério de Aceite**:
- ✅ Cada operação é um Span
- ✅ Spans relacionadas hierarquicamente
- ✅ Integra com Jaeger ou Zipkin
- ✅ Exporta automáticamente

**Exemplo**:
```
Event Received Span (0-2000ms)
├─ Validation Span (0-50ms)
├─ Queue Wait Span (50-200ms)
├─ Skill Execution Span (200-1800ms)
│  ├─ PerimeterAnalysis (200-600ms)
│  ├─ FalsePositiveAnalysis (600-1200ms)
│  └─ SeverityClassification (1200-1800ms)
└─ Persistence Span (1800-2000ms)
```

---

### NFRE-O-003: Métricas Quantitativas
**Aspecto**: Observabilidade
**Métrica**: Prometheus scrape de métricas a cada 30s

**Descrição**:
Monitorar saúde e performance do sistema.

**Critério de Aceite**:
- ✅ Métricas: latência, throughput, erros, eventos processados
- ✅ Por contexto: API, Harness, Skills, Persistência
- ✅ Alerts em SLO breaches
- ✅ Grafana dashboards

**Métricas Chave**:
```
sentinela_events_received_total (counter)
sentinela_events_processed_total (counter)
sentinela_events_failed_total (counter)
sentinela_processing_duration_seconds (histogram)
sentinela_skill_duration_seconds (histogram)
sentinela_inference_latency_seconds (histogram)
sentinela_confidence_score_distribution (histogram)
sentinela_false_positive_rate (gauge)
```

---

### NFRE-O-004: Health Checks Compostos
**Aspecto**: Observabilidade
**Métrica**: Verificar estado completo do sistema

**Descrição**:
Um único endpoint mostra saúde completa.

**Critério de Aceite**:
- ✅ GET `/health/ready` → todas dependências OK
- ✅ GET `/health/live` → aplicação responsiva
- ✅ Detalhes de cada componente
- ✅ Retorna 200 ou 503 (não 200 com problema)

---

## Mantibilidade & Qualidade de Código

### NFRE-M-001: Cobertura de Testes
**Aspecto**: Mantibilidade
**Métrica**: > 80% de cobertura de código

**Descrição**:
Confiança ao refatorar.

**Critério de Aceite**:
- ✅ Domain layer: > 90%
- ✅ Application layer: > 85%
- ✅ Infrastructure layer: > 70%
- ✅ Relatório gerado em CI/CD

---

### NFRE-M-002: Documentação Executável
**Aspecto**: Mantibilidade
**Métrica**: README + diagramas + exemplos funcionais

**Descrição**:
Documentação que não fica desatualizada.

**Critério de Aceite**:
- ✅ README com setup em < 5 minutos
- ✅ Exemplos de API com curl/postman
- ✅ Diagramas em Mermaid (versionados)
- ✅ ADRs documentam decisões
- ✅ Guides de deployment

---

### NFRE-M-003: Consistência de Código
**Aspecto**: Mantibilidade
**Métrica**: EditorConfig + Linter + Formatter

**Descrição**:
Código consistente facilita manutenção.

**Critério de Aceite**:
- ✅ EditorConfig padroniza estilos
- ✅ StyleCop (C# linter) passa sem warnings
- ✅ Prettier (se houver JS)
- ✅ Pre-commit hooks aplicam formatter
- ✅ CI/CD falha se código não formatado

---

### NFRE-M-004: Ciclo de Build Rápido
**Aspecto**: Mantibilidade
**Métrica**: Build < 30 segundos

**Descrição**:
Feedback rápido durante desenvolvimento.

**Critério de Aceite**:
- ✅ `dotnet build` < 10s (incremental)
- ✅ Testes unitários < 15s
- ✅ CI/CD full < 5 minutos
- ✅ Sem timeouts aleatórios

---

## Compliance & Regulatório

### NFRE-C-001: LGPD/GDPR Compliance
**Aspecto**: Compliance
**Métrica**: Capacidade de deletar dados de uma pessoa

**Descrição**:
Direito ao esquecimento.

**Critério de Aceite**:
- ✅ Encontrar todos eventos de uma pessoa
- ✅ Deletar cascata sem deixar órfãos
- ✅ Auditoria do apagamento
- ✅ Verificação de completude

---

## Resumo de Requisitos Não-Funcionais

| ID | Aspecto | Métrica | Alvo |
|----|---------|---------|------|
| NFRE-P-001 | Performance | Latência p95 | < 2s |
| NFRE-P-002 | Performance | Throughput | > 10 evt/s |
| NFRE-P-003 | Performance | Memória | < 2GB base |
| NFRE-P-004 | Performance | Query latência p99 | < 100ms |
| NFRE-S-001 | Escalabilidade | Workers | 100+ escalável |
| NFRE-S-002 | Escalabilidade | GPU | Linear |
| NFRE-S-003 | Escalabilidade | Banco | Agnóstico |
| NFRE-R-001 | Confiabilidade | Uptime | 99.5% |
| NFRE-R-002 | Resiliência | Circuit breaker | Auto-recovery |
| NFRE-R-003 | Resiliência | DLQ | 100% rastreado |
| NFRE-R-004 | Resiliência | Idempotência | Garantida |
| NFRE-SEC-001 | Segurança | TLS | 1.3 obrigatório |
| NFRE-SEC-002 | Segurança | Criptografia repouso | AES-256 |
| NFRE-SEC-003 | Segurança | Validação entrada | 100% |
| NFRE-SEC-004 | Segurança | Credenciais | Env vars |
| NFRE-SEC-005 | Segurança | Auditoria | Imutável |
| NFRE-O-001 | Observabilidade | Logs | CorrelationId |
| NFRE-O-002 | Observabilidade | Tracing | Jaeger |
| NFRE-O-003 | Observabilidade | Métricas | Prometheus |
| NFRE-O-004 | Observabilidade | Health | Compostos |
| NFRE-M-001 | Mantibilidade | Cobertura | > 80% |
| NFRE-M-002 | Mantibilidade | Docs | Executável |
| NFRE-M-003 | Mantibilidade | Código | Consistente |
| NFRE-M-004 | Mantibilidade | Build | < 30s |
| NFRE-C-001 | Compliance | LGPD/GDPR | Direito ao esquecimento |
