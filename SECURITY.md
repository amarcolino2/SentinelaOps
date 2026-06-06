# 🔒 Segurança

Política de segurança e como relatar vulnerabilidades.

---

## 🛡️ Princípios de Segurança

Este projeto segue:
- ✅ OWASP Top 10
- ✅ SOLID Principles (Defense in Depth)
- ✅ Secure by Default
- ✅ Principle of Least Privilege
- ✅ Input Validation
- ✅ Output Encoding

---

## 🚨 Relatar Vulnerabilidades

**NÃO** abra uma issue pública para vulnerabilidades de segurança.

Em vez disso, envie um **email privado**:
```
security@example.com
```

**Inclua**:
- Descrição detalhada da vulnerabilidade
- Passos para reproduzir
- Impacto potencial
- Sugestão de correção (se tiver)

**Resposta esperada**: Dentro de 48 horas

---

## 🔐 Práticas de Segurança

### Autenticação
- JWT Bearer Tokens
- Token expiration enforcement
- Secure token storage

### Autorização
- Role-Based Access Control (RBAC)
- Policy-based authorization
- Principle of Least Privilege

### Data Protection
- Input validation (whitelist approach)
- SQL Injection prevention (Parameterized queries)
- XSS prevention (Output encoding)
- CSRF protection (SameSite cookies)

### Secrets Management
- Never commit secrets to git
- Use secure configuration providers
- Environment variables para dev
- Azure Key Vault para produção

### Dependency Management
- Scan para vulnerabilidades conhecidas (NuGet Audit)
- Keep dependencies updated
- Review CHANGELOG de dependências críticas

### Logging & Auditing
- Log todas as operações sensíveis
- Never log sensitive data (passwords, tokens, PII)
- Estrutured logging (Serilog)
- Centralized log aggregation

---

## 🔄 Security Update Policy

- **Critical**: Patch dentro de 24h
- **High**: Patch dentro de 1 semana
- **Medium**: Patch dentro de 2 semanas
- **Low**: Patch na próxima release

---

## ✅ Segurança em PR

Ao submeter uma PR que envolva segurança:

1. **Descreva** o risco de segurança
2. **Explique** como foi mitigado
3. **Teste** todos os edge cases
4. **Revise** com pelo menos 2 mantenedores

---

## 🧪 Testes de Segurança

Executamos:
- ✅ Dependency vulnerability scanning (NuGet Audit)
- ✅ Static code analysis (Roslyn)
- ✅ SAST (Static Application Security Testing)
- ✅ Architecture tests (para validar camadas)

---

## 📋 Conformidade

Este projeto aims para:
- ✅ GDPR (Data Protection)
- ✅ OWASP Top 10
- ✅ CWE Top 25
- ✅ NIST Cybersecurity Framework

---

## 💡 Security Best Practices para Contribuidores

### ✅ Faça

```csharp
// ✅ BOM: Validação com whitelist
public void ProcessEvent(string eventType)
{
    var validTypes = new[] { "perimeter", "intrusion", "motion" };
    if (!validTypes.Contains(eventType))
        throw new ArgumentException("Invalid event type");
}

// ✅ BOM: Parameterized queries
var result = await context.MonitoringEvents
    .FromSqlInterpolated($"SELECT * FROM Events WHERE Id = {eventId}")
    .ToListAsync();

// ✅ BOM: Secure default
public class SecurityPolicy
{
    public bool RequiresMFA { get; set; } = true;  // Default true
    public int TokenExpirationMinutes { get; set; } = 15;
}
```

### ❌ NÃO Faça

```csharp
// ❌ RUIM: String concatenation (SQL Injection!)
var query = $"SELECT * FROM Events WHERE Id = {userId}";

// ❌ RUIM: Logging de dados sensíveis
logger.LogInformation($"User {username} logged in with password {password}");

// ❌ RUIM: Hardcoded secrets
var apiKey = "sk-12345-abcde";

// ❌ RUIM: Trusting user input
if (inputFromUser == "admin") { /* dangerous */ }
```

---

## 📞 Contatos de Segurança

| Caso | Contato |
|------|---------|
| Vulnerabilidade descoberta | security@example.com |
| Security audit | audit@example.com |
| Compliance questions | legal@example.com |

---

## 📚 Recursos

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [CWE Top 25](https://cwe.mitre.org/top25/)
- [NIST Cybersecurity Framework](https://www.nist.gov/cyberframework)
- [Microsoft Security Best Practices](https://docs.microsoft.com/en-us/dotnet/standard/security/)

---

**Obrigado por ajudar a manter o SentinelaOps seguro!** 🔒
