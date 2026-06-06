---
name: vehicle-analysis
description: Detecta, identifica e classifica veículos avaliando tipo, comportamento suspeito e risco de segurança.
version: 1.0.0
category: event-analysis
priority: P1
inputs:
  - image: "JPEG do evento"
  - metadata: "Local, timestamp, zona autorizada para veículos"
  - vehicleDetection: "Tipo, cor, placa, posição"
outputs:
  - vehicleDetected: "boolean"
  - vehicleType: "car|truck|motorcycle|bus|other"
  - isUnauthorized: "boolean"
  - suspiciousBehavior: "boolean"
  - threatLevel: "low|medium|high|critical"
---

# Fluxo de Trabalho (Workflow)

## 1. **Detecção e Identificação de Veículo**
   - Detectar presença de veículo
   - Classificar tipo (carro, caminhão, moto, ônibus, outro)
   - Extrair características visuais (cor, tamanho, marca estimada)
   - Tentar ler placa (se visível)
   - Registrar posição e orientação

## 2. **Verificação de Autorização**
   - Comparar contra lista de veículos autorizados
   - Verificar se zona permite veículos
   - Validar se está em estacionamento designado
   - Confirmar se horário é apropriado para circulação

## 3. **Análise de Comportamento do Veículo**
   - **Normal**: Circulação esperada, estacionamento apropriado, velocidade normal
   - **Suspeito**: Circulação repetida, estacionamento inapropriado, movimento lento/investigativo
   - **Perigoso**: Aproximação agressiva, bloqueando saídas, comportamento anômalo

## 4. **Avaliação do Motorista/Ocupantes**
   - Quantidade de ocupantes (motorista apenas? múltiplos?)
   - Visibilidade de rostos (escondidos? bloqueados?)
   - Comportamento (normal | observando local | comunicando)
   - Permanência no veículo ou saída

## 5. **Análise de Equipamento Aparente**
   - Danos ao veículo (recente? intencional?)
   - Equipamento externo (placas, acessórios)
   - Bagagem ou carga (apropriada para tipo de veículo?)
   - Identificação profissional visível (logo, uniforme)

## 6. **Cálculo de Risco**
   - Veículo desconhecido em zona restrita = alto risco
   - Veículo autorizado em comportamento normal = baixo risco
   - Veículo suspeito com múltiplos visitantes = médio a alto risco
   - Comportamento coordenado com intrusão = crítico

---

# Regras de Negócio e Restrições

* **Sem Discriminação**: Cor do veículo ou aparência do motorista não determina suspeita sozinho.
* **Contexto Espacial**: Veículo estacionado normalmente em garagem autorizada ≠ suspeito, mesmo que desconhecido.
* **Autorização Prévia**: Verificar se há autorização de entrega, manutenção ou visita legítima.
* **Placa Fake**: Placa ilegível, coberta ou falsa = CRÍTICO.
* **Frequência Anômala**: Mesmo veículo viajando zona múltiplas vezes rapidamente = investigativo.

---

# Formato de Saída (Output)

## 📊 Resultado da Análise de Veículo

```json
{
  "vehicleDetected": true,
  "vehicleType": "car|truck|motorcycle|bus|other",
  "isUnauthorized": true,
  "suspiciousBehavior": false,
  "threatLevel": "low|medium|high|critical",
  "analysisTimestamp": "2024-01-15T10:30:00Z"
}
```

### 🚗 Identificação do Veículo

**Tipo**: [car | truck | motorcycle | bus | other]

**Características Visuais**:
- Cor: [cor dominante]
- Tamanho: [pequeno | médio | grande]
- Marca: [se identificável]
- Placa: [se legível] ou [não legível]

### 🎯 Status de Autorização

- **Autorizado**: [Sim | Não | Desconhecido]
- **Zona Apropriada**: [Sim | Não]
- **Horário Apropriado**: [Sim | Não | Fora do expediente]

### 👥 Occupantes

**Quantidade**: [número estimado]

**Visibilidade**: [rosto visível | parcialmente obscurecido | completamente bloqueado]

**Comportamento**:
- Motorista: [normal | observando | comunicando]
- Passageiros: [nenhum | presentes | saído do veículo]

### 💡 Análise de Risco

**Comportamento**:
- Normal circulação esperada
- Estacionamento inapropriado
- Movimento investigativo
- Bloqueio de saídas ou acesso crítico

**Narrativa**:

> "Veículo desconhecido, placa não legível, estacionado próximo a entrada de servidor room. Motorista dentro do veículo observando edifício por 12 minutos. Sem justificativa legítima. Risco: CRITICAL."

### ⚠️ Observações

- [Se placa coberta/fake: "Placa deliberadamente ilegível - CRÍTICO"]
- [Se veículo autorizado: "Veículo de fornecedor legítimo em horário de entrega autorizada"]
- [Se múltiplas visitas: "Terceira passagem pela zona em 20 minutos - padrão investigativo"]

---

# Exemplos

### Caso 1: Veículo Suspeito (CRITICAL)
```
Tipo: Car
Autorizado: Não
Comportamento: Suspicious
Risco: CRITICAL

Análise: Veículo desconhecido, placa ilegível, motorista observando perímetro por tempo anormalmente longo.
Recomendação: Registro de placa, investigação de ocupantes
```

### Caso 2: Entrega Autorizada (LOW)
```
Tipo: Truck
Autorizado: Sim
Comportamento: Normal
Risco: LOW

Análise: Veículo de fornecedor registrado, driver com uniforme/identificação, horário de entrega autorizado.
Recomendação: Sem alerta
```

### Caso 3: Estacionamento Inapropriado (MEDIUM)
```
Tipo: Car
Autorizado: Sim
Comportamento: Suspicious
Risco: MEDIUM

Análise: Veículo registrado mas estacionado fora de zona autorizada, motorista dentro por tempo excessivo.
Recomendação: Verificação de motivo, potencial infração
```
