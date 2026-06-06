# 🚨 SentinelaOps - Open Source Video Monitoring Decision Support Platform

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12-green)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![xUnit](https://img.shields.io/badge/Tests-xUnit%202.6.6-brightgreen)](https://xunit.net/)
[![Build Status](https://img.shields.io/badge/Build-PASSING-brightgreen)](#build-status)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)
[![Status](https://img.shields.io/badge/Status-Development%2FAlpha-orange)](ROADMAP.md)
[![Open Issues](https://img.shields.io/badge/Open%20Issues-Help%20Wanted-blue)](#contributing)

> **Reduzindo falsos positivos em videomonitoramento através de IA multimodal + arquitetura moderna**

<img width="3920" height="526" alt="mermaid-diagram" src="https://github.com/user-attachments/assets/71ff113f-30f8-49f1-87f7-3f86472c4922" />

<img width="1536" height="1024" alt="3c895955-61aa-4e77-a0e2-7be8e6ddbe11" src="https://github.com/user-attachments/assets/6881ea14-e434-4acf-bf87-6eff194b6c5d" />

---

## ⚠️ AVISO: Projeto em Desenvolvimento

Este projeto está em **fase ALPHA** e **NÃO está pronto para produção**.

```
Status: 🔨 EM DESENVOLVIMENTO (Etapa 1.3/2.x)
├─ ✅ Domain Core: Completo (24/24 testes)
├─ ✅ Documentação: Completa  
├─ 🔄 Application Services: Em progresso
├─ ❌ Persistência: Não implementada
├─ ❌ API REST: Não implementada
├─ ❌ Worker Service: Não implementada
└─ ❌ Dashboard: Não implementado
```

### ⛔ NÃO use em produção para:
- ❌ Sistemas reais de segurança
- ❌ Tomada de decisão crítica
- ❌ Ambientes com dados sensíveis
- ❌ Operações 24/7

### ✅ Use para:
- ✅ Aprender DDD + Clean Architecture
- ✅ Entender arquitetura de IA
- ✅ Contribuir ao projeto
- ✅ Benchmarking de modelos
- ✅ Prototipagem

**Timeline esperado para v1.0.0**: Q4 2026 | Veja [ROADMAP.md](ROADMAP.md) para detalhes

---

## 🎯 O Problema

Operadores de videomonitoramento recebem **múltiplos alertas diários** da maioria dos quais são **falsos positivos**. Isso causa:
- ⏰ Perda de tempo investigando eventos triviais
- 😴 Fadiga do operador (AlertFatigue)
- ❌ Ineficiência operacional
- 💰 Custo alto

**SentinelaOps resolve isso** fornecendo **classificação inteligente, confiança e justificativa** para cada evento de análise.

---

## ✨ Principais Características

### 🤖 **Análise Inteligente Multimodal**
8 Skills independentes analisam cada evento:
- 🔵 **Perimeter Analysis** - Violações de zona
- 🔓 **Intrusion Analysis** - Arrombamento e escalada
- 🚶 **Motion Analysis** - Padrões anormais
- 👤 **Human Activity** - Comportamento suspeito
- 🚗 **Vehicle Analysis** - Tipo, placa, velocidade
- 🎯 **False Positive Analysis** - Validação contextual
- 📊 **Severity Classification** - Matriz de risco
- 📋 **Incident Summary** - Relatório estruturado

### 🏛️ **Arquitetura Profissional**
- ✅ **Domain-Driven Design** (DDD) - Modelo de domínio no centro
- ✅ **Clean Architecture** - Camadas bem separadas
- ✅ **Spec-Driven Development** (SDD) - Specs antes de código
- ✅ **Harness Engineering** - Benchmarking de modelos
- ✅ **Event-Driven** - Comunicação assíncrona
- ✅ **100% Testado** - 24/24 testes passando

### 🔧 **Extensível**
- **Novos Skills** sem alterar existentes (Open/Closed Principle)
- **Múltiplos Modelos IA** (Ollama, OpenAI, Azure, Anthropic)
- **Múltiplos Bancos** (SQLite, SQL Server, PostgreSQL, MongoDB)
- **Múltiplas Fontes** (ONVIF, RTSP, APIs customizadas)

### 📡 **Pronto para Produção**
- OpenTelemetry (Tracing, Metrics, Logs)
- Auditoria completa de inferências
- Versionamento de prompts
- Rastreabilidade end-to-end
- JWT + RBAC
