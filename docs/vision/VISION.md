# Visão do Produto - Sentinela Ops

## Declaração de Visão

**Sentinela Ops** é uma plataforma Open Source de apoio à decisão operacional para ambientes de videomonitoramento que utiliza Inteligência Artificial multimodal para reduzir falsos positivos e acelerar a tomada de decisão operacional.

## Propósito

O projeto serve como:
1. **Plataforma Prática**: Sistema funcional de análise inteligente de eventos de videomonitoramento
2. **Referência Arquitetural**: Exemplo público de arquitetura moderna em .NET 8, DDD, Clean Architecture
3. **Demonstração de IA Aplicada**: Caso de uso real de IA multimodal em sistemas distribuídos
4. **Catálogo de Padrões**: Documentação de patterns de SDD, Harness Engineering e Skills-Based Architecture

## Posicionamento Estratégico

### O que NÃO é
- **Substituição de operadores**: A plataforma **não automatiza** decisões operacionais
- **Sistema autônomo**: Requer supervisão humana contínua
- **Solução específica de vendor**: Agnóstica a marcas de câmeras e sistemas de monitoramento

### O que É
- **Sistema de Contexto**: Fornece contexto rico para aceleração de decisão
- **Classificador Inteligente**: Reduz falsos positivos através de análise multimodal
- **Amplificador Operacional**: Aumenta a capacidade cognitiva do operador
- **Plataforma Extensível**: Permite adicionar novos tipos de análise sem modificação do domínio

## Benefícios Esperados

### Para Operadores
- ✅ Redução de tempo para tomada de decisão (30-40%)
- ✅ Confiança aumentada em classificações (via score de confiança)
- ✅ Contexto justificado para cada alertat
- ✅ Recomendações operacionais baseadas em análise

### Para Administradores
- ✅ Redução de falsos positivos (alvo: 70-80%)
- ✅ Auditoria completa de todas as inferências
- ✅ Capacidade de comparar modelos e prompts
- ✅ Histórico rastreável de todas as decisões

### Para Arquitetos
- ✅ Exemplo de DDD rigoroso em .NET 8
- ✅ Padrões de IA abstrata (não acoplada a modelo específico)
- ✅ Skills-Based Architecture demonstrada em produção
- ✅ Harness Engineering para experimentação contínua

## Restrições Estratégicas

1. **Independência de Modelo**: Nenhuma dependência acoplada a modelos de IA específicos
2. **Independência de Monitoramento**: Agnóstica a sistemas de câmeras e CCTV
3. **Pureza Arquitetural**: Aplicação rigorosa de DDD, Clean Architecture, SOLID
4. **Auditabilidade Total**: Toda inferência é rastreável e auditável

## Horizonte de Evolução

### Fase 1 (Núcleo)
- Análise de eventos com Ollama + Gemma 3
- Skills iniciais: Perímetro, Intrusão, Movimento
- Persistência: SQLite
- Mensageria: RabbitMQ
- API básica e Harness

### Fase 2 (Expansão)
- Adicionar mais Skills (Veículos, Atividade Humana, etc.)
- Suporte para múltiplos modelos
- Persistência em SQL Server / PostgreSQL
- Azure Service Bus / Kafka

### Fase 3 (Evolução)
- Azure OpenAI, Anthropic, Mistral
- Integrações ONVIF, RTSP nativas
- Dashboards avançados
- Machine Learning para calibração de confiança

## Princípios de Design

| Princípio | Aplicação |
|-----------|-----------|
| **Claridade** | Documentação é executável; especificação precede código |
| **Composição** | Skills são blocos independentes combinávei s |
| **Auditabilidade** | Toda inferência é registrada com rastreabilidade completa |
| **Extensibilidade** | Novas Skills e modelos sem modificação do domínio |
| **Testabilidade** | Testes em múltiplas camadas: unitária, integração, arquitetura, E2E |

## Sucesso Medido Por

1. **Redução de Falsos Positivos**: Métrica primária
2. **Tempo de Decisão**: Redução perceptível para operador
3. **Arquitetura Pura**: Aderência a DDD e princípios SOLID
4. **Capacidade de Experimentação**: Harness permite comparação de modelos
5. **Comunidade**: Adoção e contribuições externas (Open Source)

## Contexto Organizacional

- **Proprietário do Produto**: Arquitetor de Software Principal
- **Público Alvo Primário**: Operadores de segurança, arquitetos de software
- **Contexto de Uso**: Centros de monitoramento 24/7, ambientes corporativos
- **Escala Esperada**: De centenas a milhares de eventos por dia
- **Criticidade**: Suporte à decisão (não automação de decisão)
