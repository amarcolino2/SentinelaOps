namespace SentinelaOps.Domain.Core;

/// <summary>
/// Metadados de um evento de monitoramento.
/// Imutável, Value Object.
/// </summary>
public class EventMetadata : IEquatable<EventMetadata>
{
    private readonly Dictionary<string, object> _attributes;

    /// <summary>
    /// Zona do evento de monitoramento.
    /// </summary>
    public string Zone { get; }

    /// <summary>
    /// ID do sensor que detectou o evento.
    /// </summary>
    public string SensorId { get; }

    /// <summary>
    /// Nível de sensibilidade do evento.
    /// </summary>
    public EventSensitivity Sensitivity { get; }

    /// <summary>
    /// Data/hora em que o evento ocorreu.
    /// </summary>
    public DateTime OccurredAt { get; }

    /// <summary>
    /// Construtor de EventMetadata.
    /// </summary>
    public EventMetadata(string zone, string sensorId, EventSensitivity sensitivity, DateTime occurredAt, Dictionary<string, object>? customAttributes = null)
    {
        if (string.IsNullOrWhiteSpace(zone))
            throw new ArgumentException("Zone cannot be empty", nameof(zone));

        if (string.IsNullOrWhiteSpace(sensorId))
            throw new ArgumentException("SensorId cannot be empty", nameof(sensorId));

        Zone = zone;
        SensorId = sensorId;
        Sensitivity = sensitivity;
        OccurredAt = occurredAt;
        _attributes = new Dictionary<string, object>(customAttributes ?? new Dictionary<string, object>());
    }

    /// <summary>
    /// Obter atributo customizado por chave.
    /// </summary>
    public object? GetAttribute(string key) => _attributes.TryGetValue(key, out var value) ? value : null;

    /// <summary>
    /// Obter todos os atributos customizados.
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomAttributes => _attributes.AsReadOnly();

    /// <summary>
    /// Verifica igualdade com outro EventMetadata.
    /// </summary>
    public bool Equals(EventMetadata? other)
    {
        if (other is null) return false;
        return Zone == other.Zone
            && SensorId == other.SensorId
            && Sensitivity == other.Sensitivity
            && OccurredAt == other.OccurredAt
            && _attributes.SequenceEqual(other._attributes);
    }

    /// <summary>
    /// Verifica igualdade com outro objeto.
    /// </summary>
    public override bool Equals(object? obj) => Equals(obj as EventMetadata);

    /// <summary>
    /// Retorna hash code dos metadados.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(Zone, SensorId, Sensitivity, OccurredAt);

    /// <summary>
    /// Operador de igualdade.
    /// </summary>
    public static bool operator ==(EventMetadata? left, EventMetadata? right) => Equals(left, right);

    /// <summary>
    /// Operador de desigualdade.
    /// </summary>
    public static bool operator !=(EventMetadata? left, EventMetadata? right) => !Equals(left, right);
}

/// <summary>
/// Nível de sensibilidade de um evento.
/// </summary>
public enum EventSensitivity
{
    /// <summary>
    /// Sensibilidade baixa.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Sensibilidade média.
    /// </summary>
    Medium = 1,

    /// <summary>
    /// Sensibilidade alta.
    /// </summary>
    High = 2
}
