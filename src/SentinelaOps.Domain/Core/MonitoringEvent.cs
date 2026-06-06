namespace SentinelaOps.Domain.Core;

/// <summary>
/// Evento de monitoramento - Raiz do Agregado Event.
/// Encapsula evento recebido e seu ciclo de vida.
/// </summary>
public class MonitoringEvent
{
    private readonly List<DomainEvent> _domainEvents = new();

    /// <summary>
    /// ID único do evento.
    /// </summary>
    public EventId EventId { get; }

    /// <summary>
    /// ID de correlação para rastreamento end-to-end.
    /// </summary>
    public CorrelationId CorrelationId { get; }

    /// <summary>
    /// Metadados do evento.
    /// </summary>
    public EventMetadata Metadata { get; }

    /// <summary>
    /// Status atual do evento.
    /// </summary>
    public EventStatus Status { get; private set; }

    /// <summary>
    /// Data/hora em que o evento foi recebido.
    /// </summary>
    public DateTime ReceivedAt { get; }

    private Classification? _classification;
    private Confidence? _confidence;
    private Justification? _justification;
    private List<string> _evidence = new();

    private MonitoringEvent(
        EventId eventId,
        CorrelationId correlationId,
        EventMetadata metadata,
        DateTime receivedAt)
    {
        EventId = eventId ?? throw new ArgumentNullException(nameof(eventId));
        CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        ReceivedAt = receivedAt;
        Status = EventStatus.Received;
    }

    /// <summary>
    /// Factory method para criar novo MonitoringEvent.
    /// </summary>
    public static MonitoringEvent Create(
        EventId eventId,
        CorrelationId correlationId,
        EventMetadata metadata,
        DateTime receivedAt)
    {
        var monitoringEvent = new MonitoringEvent(eventId, correlationId, metadata, receivedAt);
        monitoringEvent.RaiseDomainEvent(new EventReceivedDomainEvent(
            eventId.ToString(),
            correlationId.ToString(),
            metadata.Zone,
            receivedAt
        ));
        return monitoringEvent;
    }

    /// <summary>
    /// Marca evento como iniciando processamento.
    /// </summary>
    public void StartAnalysis()
    {
        if (Status != EventStatus.Received)
            throw new InvalidOperationException($"Cannot start analysis on event with status {Status}");

        Status = EventStatus.Processing;
        RaiseDomainEvent(new AnalysisStartedDomainEvent(
            EventId.ToString(),
            CorrelationId.ToString(),
            DateTime.UtcNow
        ));
    }

    /// <summary>
    /// Completa análise do evento com resultado.
    /// </summary>
    public void CompleteAnalysis(Classification classification, Confidence confidence, Justification justification, List<string> evidence)
    {
        if (Status != EventStatus.Processing)
            throw new InvalidOperationException($"Cannot complete analysis on event with status {Status}");

        _classification = classification ?? throw new ArgumentNullException(nameof(classification));
        _confidence = confidence ?? throw new ArgumentNullException(nameof(confidence));
        _justification = justification ?? throw new ArgumentNullException(nameof(justification));
        _evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));

        Status = EventStatus.Analyzed;
        RaiseDomainEvent(new AnalysisCompletedDomainEvent(
            EventId.ToString(),
            CorrelationId.ToString(),
            classification.ToString(),
            confidence.Percentage,
            DateTime.UtcNow
        ));
    }

    /// <summary>
    /// Marca evento como arquivado.
    /// </summary>
    public void Archive()
    {
        if (Status == EventStatus.Archived)
            throw new InvalidOperationException("Event is already archived");

        Status = EventStatus.Archived;
    }

    /// <summary>
    /// Obter resultado de análise (se concluída).
    /// </summary>
    public AnalysisResult? GetAnalysisResult()
    {
        if (Status != EventStatus.Analyzed || _classification is null)
            return null;

        return new AnalysisResult(
            _classification,
            _confidence!,
            _justification!,
            _evidence.AsReadOnly()
        );
    }

    /// <summary>
    /// Obter eventos de domínio que ocorreram neste agregado.
    /// </summary>
    public IReadOnlyList<DomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();

    /// <summary>
    /// Limpar eventos de domínio (após persistência).
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    private void RaiseDomainEvent(DomainEvent @event) => _domainEvents.Add(@event);
}

/// <summary>
/// Status de um evento de monitoramento.
/// </summary>
public enum EventStatus
{
    /// <summary>
    /// Evento recebido, aguardando processamento.
    /// </summary>
    Received = 0,

    /// <summary>
    /// Evento em processamento.
    /// </summary>
    Processing = 1,

    /// <summary>
    /// Análise concluída.
    /// </summary>
    Analyzed = 2,

    /// <summary>
    /// Evento arquivado.
    /// </summary>
    Archived = 3
}

/// <summary>
/// Resultado de análise de um evento.
/// </summary>
public class AnalysisResult
{
    /// <summary>
    /// Classificação do resultado.
    /// </summary>
    public Classification Classification { get; }

    /// <summary>
    /// Valor de confiança do resultado.
    /// </summary>
    public Confidence Confidence { get; }

    /// <summary>
    /// Justificativa do resultado.
    /// </summary>
    public Justification Justification { get; }

    /// <summary>
    /// Lista de evidências.
    /// </summary>
    public IReadOnlyList<string> Evidence { get; }

    /// <summary>
    /// Construtor de AnalysisResult.
    /// </summary>
    public AnalysisResult(Classification classification, Confidence confidence, Justification justification, IReadOnlyList<string> evidence)
    {
        Classification = classification ?? throw new ArgumentNullException(nameof(classification));
        Confidence = confidence ?? throw new ArgumentNullException(nameof(confidence));
        Justification = justification ?? throw new ArgumentNullException(nameof(justification));
        Evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
    }
}
