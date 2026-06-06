namespace SentinelaOps.Domain.Core;

/// <summary>
/// Base class para eventos de domínio.
/// </summary>
public abstract class DomainEvent
{
    /// <summary>
    /// Data/hora em que o evento ocorreu.
    /// </summary>
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

/// <summary>
/// Evento de domínio disparado quando evento é recebido.
/// </summary>
public class EventReceivedDomainEvent : DomainEvent
{
    /// <summary>
    /// ID do evento.
    /// </summary>
    public string EventId { get; }

    /// <summary>
    /// ID de correlação.
    /// </summary>
    public string CorrelationId { get; }

    /// <summary>
    /// Zona de ocorrência.
    /// </summary>
    public string Zone { get; }

    /// <summary>
    /// Data/hora de recebimento.
    /// </summary>
    public DateTime ReceivedAt { get; }

    /// <summary>
    /// Construtor do EventReceivedDomainEvent.
    /// </summary>
    public EventReceivedDomainEvent(string eventId, string correlationId, string zone, DateTime receivedAt)
    {
        EventId = eventId ?? throw new ArgumentNullException(nameof(eventId));
        CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        Zone = zone ?? throw new ArgumentNullException(nameof(zone));
        ReceivedAt = receivedAt;
    }
}

/// <summary>
/// Evento de domínio disparado quando análise inicia.
/// </summary>
public class AnalysisStartedDomainEvent : DomainEvent
{
    /// <summary>
    /// ID do evento.
    /// </summary>
    public string EventId { get; }

    /// <summary>
    /// ID de correlação.
    /// </summary>
    public string CorrelationId { get; }

    /// <summary>
    /// Data/hora de início da análise.
    /// </summary>
    public DateTime StartedAt { get; }

    /// <summary>
    /// Construtor do AnalysisStartedDomainEvent.
    /// </summary>
    public AnalysisStartedDomainEvent(string eventId, string correlationId, DateTime startedAt)
    {
        EventId = eventId ?? throw new ArgumentNullException(nameof(eventId));
        CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        StartedAt = startedAt;
    }
}

/// <summary>
/// Evento de domínio disparado quando análise completa.
/// </summary>
public class AnalysisCompletedDomainEvent : DomainEvent
{
    /// <summary>
    /// ID do evento.
    /// </summary>
    public string EventId { get; }

    /// <summary>
    /// ID de correlação.
    /// </summary>
    public string CorrelationId { get; }

    /// <summary>
    /// Classificação do resultado.
    /// </summary>
    public string Classification { get; }

    /// <summary>
    /// Valor de confiança (0-100).
    /// </summary>
    public double Confidence { get; }

    /// <summary>
    /// Data/hora de conclusão.
    /// </summary>
    public DateTime CompletedAt { get; }

    /// <summary>
    /// Construtor do AnalysisCompletedDomainEvent.
    /// </summary>
    public AnalysisCompletedDomainEvent(string eventId, string correlationId, string classification, double confidence, DateTime completedAt)
    {
        EventId = eventId ?? throw new ArgumentNullException(nameof(eventId));
        CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        Classification = classification ?? throw new ArgumentNullException(nameof(classification));
        Confidence = confidence;
        CompletedAt = completedAt;
    }
}
