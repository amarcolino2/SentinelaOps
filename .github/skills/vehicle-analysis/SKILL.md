---
name: vehicle-analysis
description: Detecção e análise de veículos incluindo tipo, placa, velocidade, comportamento e integridade estrutural.
version: 1.0.0
triggers:
  - vehicle detected in monitoring zone
  - vehicle speed analysis needed
  - unauthorized vehicle access
  - vehicle behavior anomaly
context: SentinelaOps video monitoring decision support
---

# Fluxo de Trabalho (Workflow)

A IA deve executar os seguintes passos **exatamente nesta ordem**:

1. **Detecção e Classificação de Veículo**
   - Confirmar detecção de veículo (confiança >80%)
   - Classificar tipo: CAR, TRUCK, VAN, MOTORCYCLE, BUS, OTHER
   - Identificar marca/modelo quando possível
   - Estimar ano/geração

2. **Extração de Dados de Identificação**
   - Tentar ler placa de identificação
   - Avaliar legibilidade da placa (0-100%)
   - Registrar formato de placa (estadual, municipal, especial)
   - Nota: Nem sempre placa será legível

3. **Análise de Velocidade**
   - Calcular velocidade a partir de trajetória
   - Comparar com limite de zona
   - Detectar aceleração/desaceleração
   - Avaliar padrão de velocidade (consistente vs. variável)

4. **Análise de Comportamento**
   - Rota percorrida
   - Paradas não autorizadas
   - Manobras suspeitas (reversão, mudança abrupta)
   - Tempo de permanência
   - Compatibilidade com zona

5. **Análise de Integridade**
   - Danos visíveis
   - Carregamento anômalo
   - Equipamentos incomuns
   - Sinais de roubo/adulteração

6. **Cálculo de Score**
   - Identificação segura: 0-25 pontos
   - Velocidade apropriada: 0-25 pontos
   - Comportamento apropriado: 0-30 pontos
   - Integridade aparente: 0-20 pontos

---

# Regras de Negócio e Restrições

## Obrigatórias
* **Tipo de veículo obrigatório**: Sem classificação clara, marcar como "TIPO_DESCONHECIDO"
* **Placa opcional**: Nem sempre será legível; não aumentar confiança se placa ilegível
* **Zona específica**: Mesmo veículo pode ser normal em algumas zonas e suspeito em outras
* **Histórico importante**: Veículos conhecidos reduzem risco mesmo com anomalia

## Contextual
* Veículo administrativo em entrada = NORMAL
* Veículo desconhecido em área de armazenamento noturno = SUSPEITO
* Velocidade alta em estacionamento = SUSPEITO
* Parada não autorizada = REPORTE OBRIGATÓRIO

## Segurança
* Não revelar vulnerabilidades de acesso veicular
* Não descrever rotas de acesso fácil
* Focar em comportamento, não em características do proprietário

---

# Formato de Saída (Output)

```json
{
  "skillName": "VehicleAnalysis",
  "timestamp": "2024-01-15T10:30:45Z",
  "eventId": "{cameraId}_{timestamp}",
  
  "analysis": {
    "vehicleDetected": true,
    "detectionConfidence": 92,
    "classification": "AUTORIZADO|SUSPEITO|NAO_IDENTIFICADO|PROIBIDO",
    "riskLevel": "CRÍTICA|ALTA|MÉDIA|BAIXA|NENHUMA",
    "overallConfidence": 81
  },
  
  "summary": "{Uma frase técnica sobre veículo e comportamento}",
  
  "vehicleIdentification": {
    "type": "CAR|TRUCK|VAN|MOTORCYCLE|BUS|OTHER",
    "typeConfidence": 94,
    "make": "DESCONHECIDA",
    "model": "DESCONHECIDA",
    "year": "2015-2020",
    "color": "BRANCO",
    "bodyCondition": "BOAS|DANOS_MENORES|DANOS_MODERADOS|DANOS_GRAVES"
  },
  
  "plateAnalysis": {
    "plateDetected": true,
    "plateText": "ABC1234",
    "plateReadability": 92,
    "plateFormat": "MERCOSUL|ESTADUAL|ESPECIAL|DESCONHECIDO",
    "plateStatus": "CONHECIDA|RASTREADA|ROUBADA|SUSPENSA"
  },
  
  "speedAnalysis": {
    "detectedSpeed": { value: 25, unit: "km/h" },
    "zoneSpeedLimit": { value: 20, unit: "km/h" },
    "speedViolation": true,
    "speedVariation": "CONSISTENTE|VARIAVEL|ACELERACAO|DESACELERACAO",
    "speedScore": 15
  },
  
  "behaviorAnalysis": {
    "route": "ENTRADA_PRINCIPAL → AREA_ADMIN → SAIDA",
    "stopsDetected": 2,
    "unauthorizedStops": true,
    "maneuvers": ["REVERSAO", "MUDANCA_BRUSCA_DIRECAO"],
    "suspiciousManeuvering": true,
    "dwellTime": 180,
    "dwellAuthorized": false,
    "behaviorScore": 22
  },
  
  "integrityAnalysis": {
    "visibleDamage": false,
    "abnormalLoading": true,
    "loadingType": "VOLUME_ALTO",
    "unusualEquipment": false,
    "toolsDetected": false,
    "integrityScore": 18
  },
  
  "scoreBreakdown": {
    "identification": 23,
    "speed": 15,
    "behavior": 22,
    "integrity": 18,
    "total": 78
  },
  
  "accessHistory": {
    "vehicleKnown": false,
    "previousVisits": 0,
    "lastVisit": "NUNCA",
    "authorizedVehicle": false
  },
  
  "evidence": [
    "Veículo tipo van, cor branca, desconhecido",
    "Placa ABC1234 legível com 92% confiança",
    "Velocidade: 25 km/h em zona de 20 km/h",
    "Parada não autorizada em área de armazenamento",
    "Carregamento volumoso detectado",
    "Manobras: reversão, mudança abrupta de direção"
  ],
  
  "justification": "Veículo desconhecido com padrão suspeito: velocidade acima do limite, parada não autorizada, carregamento volumoso, manobras anômalas. Não consta no histórico de acessos. Score: 78%.",
  
  "recommendations": [
    "Registrar passagem em auditoria de acesso",
    "Comparar placa com banco de dados de veículos autorizados",
    "Se placa roubada/suspensa: ALERTA CRÍTICO",
    "Rastrear veículo em câmeras de saída"
  ],
  
  "nextSkills": ["false-positive-analysis", "severity-classification"]
}
```

---

# Exemplo Prático

**Input:**
```
evento {
  image: "/events/cam-parking-01_20240115_103045.jpg"
  camera: "entrada_principal",
  detectedObject: { type: "VEH", confidence: 0.92 }
}
```

**Output:**
```
vehicleDetected: true
classification: "SUSPEITO"
riskLevel: "MEDIA"

vehicleIdentification: { type: "VAN", color: "BRANCO", confidence: 94 }
plateAnalysis: { plateText: "ABC1234", readability: 92 }
speedAnalysis: { detectedSpeed: 25, limit: 20, violation: true }
behaviorAnalysis: { unauthorizedStops: true, suspiciousManeuvering: true }

recommendations: [
  "Rastrear veículo em câmeras",
  "Verificar placa no banco de dados"
]
```

---

# Notas Importantes

- Análise de veículo é **crítica para controle de acesso**
- Placa legível aumenta significativamente a rastreabilidade
- Comportamento anômalo em veículo é indicador forte de intenção suspeita
- Sempre correlacionar com histórico de acesso veicular
