namespace SentinelaOps.Domain.Core;

/// <summary>
/// Identificador único de um evento de monitoramento.
/// Formato: {cameraId}_{timestamp:yyyyMMddHHmmss}_{sequenceNumber}
/// </summary>
public class EventId : IEquatable<EventId>
{
    private readonly string _value;

    private EventId(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Factory method para criar novo EventId.
    /// </summary>
    public static EventId Create(string cameraId, DateTime timestamp, int sequence)
    {
        if (string.IsNullOrWhiteSpace(cameraId))
            throw new ArgumentException("Camera ID cannot be empty", nameof(cameraId));

        var value = $"{cameraId}_{timestamp:yyyyMMddHHmmss}_{sequence:D4}";
        return new EventId(value);
    }

    /// <summary>
    /// Parse EventId from string representation.
    /// </summary>
    public static EventId Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("EventId value cannot be empty", nameof(value));

        return new EventId(value);
    }

    /// <summary>
    /// Retorna representação string do EventId.
    /// </summary>
    public override string ToString() => _value;

    /// <summary>
    /// Verifica igualdade com outro EventId.
    /// </summary>
    public bool Equals(EventId? other) => other is not null && _value == other._value;

    /// <summary>
    /// Verifica igualdade com outro objeto.
    /// </summary>
    public override bool Equals(object? obj) => Equals(obj as EventId);

    /// <summary>
    /// Retorna hash code do EventId.
    /// </summary>
    public override int GetHashCode() => _value.GetHashCode();

    /// <summary>
    /// Operador de igualdade.
    /// </summary>
    public static bool operator ==(EventId? left, EventId? right) => Equals(left, right);

    /// <summary>
    /// Operador de desigualdade.
    /// </summary>
    public static bool operator !=(EventId? left, EventId? right) => !Equals(left, right);
}
