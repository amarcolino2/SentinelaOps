---
name: perimeter-analysis
description: Analisa eventos de perímetro para detectar intrusões, violações de zona de proteção e comportamentos suspeitos em áreas restritas.
version: 1.0.0
triggers:
  - monitoring event with perimeter data
  - zone boundary crossing
  - unauthorized area access
context: SentinelaOps video monitoring decision support
---

# Fluxo de Trabalho (Workflow)

A IA deve executar os seguintes passos **exatamente nesta ordem**:

1. **Análise Inicial de Contexto**
   - Validar metadados do evento (timestamp, câmera, zona, sensibilidade)
   - Confirmar que imagem contém perímetro definido
   - Verificar condições ambientais (luz, weather, oclusões)

2. **Detecção de Objetos**
   - Identificar pessoas, veículos, animais na cena
   - Localizar objetos na imagem (posição, dimensão, movimento)
   - Classificar tipo de objeto (PED, VEH, ANIMAL, OTHER)

3. **Análise de Zona**
   - Verificar se objeto está dentro/fora da zona permitida
   - Calcular distância em relação à linha de perímetro
   - Determinar se houve cruzamento de fronteira

4. **Avaliação de Risco**
   - Analisar velocidade e trajetória do objeto
   - Verificar comportamento (parado, caminhando, correndo, escalando)
   - Avaliar intenção (acidental vs. deliberado)

5. **Geração de Confiança**
   - Calcular score de confiança (0-100) baseado em:
     * Clareza da detecção de objeto (0-40%)
     * Definição clara do perímetro (0-30%)
     * Condições ambientais (0-20%)
     * Histórico do objeto (0-10%)

6. **Estruturação de Resultado**
   - Compilar análise em formato estruturado
   - Listar evidências visuais utilizadas
   - Fornecer justificativa em linguagem técnica

---

# Regras de Negócio e Restrições

## Obrigatórias
* **Nunca assumir**: Se a zona não está claramente definida, marcar como "ZONA_INDEFINIDA"
* **Conservador na detecção**: Preferir falso negativo a falso positivo (errar por falta de confiança)
* **Metadados obrigatórios**: Exigir timestamp, cameraId, zoneId na entrada
* **Imagem legível**: Se imagem está corrompida/desfocada, retornar "IMAGEM_INADEQUADA"

## Contextual
* Perímetro pode ser: cerca, muro, linha de demarcação, luz infravermelha, virtual
* Comportamento noturno pode reduzir confiança em 15-20%
* Animais domésticos reduzem risco para 30% mesmo dentro da zona
* Veículos autorizados não devem gerar alerta mesmo cruzando perímetro

## Segurança
* Não revelar pontos fracos de perímetro em justificativa
* Usar linguagem técnica, nunca instruções operacionais

---

# Formato de Saída (Output)

```json
{
  "skillName": "PerimeterAnalysis",
  "timestamp": "2024-01-15T10:30:45Z",
  "eventId": "{cameraId}_{timestamp}",
  
  "analysis": {
    "classification": "VIOLATION|SUSPICIOUS|NORMAL|ZONE_UNDEFINED|IMAGEM_INADEQUADA",
    "riskLevel": "CRITICAL|HIGH|MEDIUM|LOW|NONE",
    "confidence": 85,
    "
  },
  
  "summary": "{Uma frase técnica explicando classificação}",
  
  "detectedObjects": [
    {
      "id": "obj_001",
      "type": "PERSON|VEHICLE|ANIMAL|OTHER",
      "position": {"x": 123, "y": 456, "zone": "INSIDE|OUTSIDE"},
      "behavior": "STOPPED|WALKING|RUNNING|CLIMBING|UNKNOWN",
      "speed": "STATIONARY|SLOW|MEDIUM|FAST",
      "confidence": 92
    }
  ],
  
  "zoneAnalysis": {
    "zoneId": "north_perimeter",
    "zoneDefined": true,
    "boundaryClarity": "CLEAR|MODERATE|POOR",
    "crossingDetected": true,
    "crossingDirection": "INBOUND|OUTBOUND",
    "crossingBehavior": "DELIBERATE|ACCIDENTAL|UNKNOWN"
  },
  
  "environmentalFactors": {
    "lighting": "BRIGHT|NORMAL|DIM|DARK",
    "weather": "CLEAR|CLOUDY|RAIN|FOG",
    "occlusions": "NONE|MINOR|SIGNIFICANT",
    "visibilityImpact": 0
  },
  
  "evidence": [
    "Pessoa detectada cruzando linha de perímetro norte",
    "Movimento deliberado em direção a área restrita",
    "Sem uniformidade ou identificação visual"
  ],
  
  "justification": "Detecção de pessoa cruzando perímetro em zona restrita com movimento deliberado. Clareza de imagem: ÓTIMA. Condições: noturnas (impacto -15%). Score ajustado: 85%.",
  
  "recommendations": [
    "Enviar operador para verificação local",
    "Aumentar nível de alerta nesta câmera",
    "Revisar histórico dos últimos 5 minutos"
  ],
  
  "nextSkills": ["intrusion-analysis", "severity-classification"]
}
```

---

# Exemplo Prático

**Input:**
```
evento {
  cameraId: "cam-north-01"
  timestamp: "2024-01-15T22:30:45Z"
  zone: { id: "north_perimeter", type: "fence_line", sensitive: true }
  image: "/events/cam-north-01_20240115_223045.jpg"
  metadata: { lighting: "dark", weather: "clear" }
}
```

**Output:**
```
classification: "VIOLATION"
confidence: 88
summary: "Pessoa detectada cruzando perímetro norte com movimento deliberado em área restrita."

detectedObjects: [
  { type: "PERSON", position: "OUTSIDE", behavior: "RUNNING", confidence: 94 }
]

evidence: [
  "Pessoa em movimento rápido cruzando cerca de perímetro",
  "Trajetória direcionada para edifício administrativo",
  "Sem crachá ou identificação visível"
]

recommendations: [
  "Operador deve se dirigir imediatamente para verif icação",
  "Ativar protocolos de segurança nível 2",
  "Documentar incidente para auditoria"
]
```

---

# Notas Importantes

- Perímetro análise é **crítica para prevenção**
- Errar por conservadorismo é melhor que ignorar ameaça
- Timestamp deve estar sempre em UTC ISO 8601
- Histórico de zona deve informar padrões (rotas esperadas, horários)
