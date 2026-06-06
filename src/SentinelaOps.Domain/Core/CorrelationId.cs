namespace SentinelaOps.Domain.Core;

/// <summary>
/// Identificador de correlação único para rastrear evento através de toda a cadeia de processamento.
/// Formato UUID: {guid}
/// </summary>
public class CorrelationId : IEquatable<CorrelationId>
{
    private readonly Guid _value;

    private CorrelationId(Guid value)
    {
        _value = value;
    }

    /// <summary>
    /// Factory method para criar novo CorrelationId.
    /// </summary>
    public static CorrelationId Create() => new(Guid.NewGuid());

    /// <summary>
    /// Parse CorrelationId from string representation.
    /// </summary>
    public static CorrelationId Parse(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException("CorrelationId must be valid GUID", nameof(value));

        return new CorrelationId(guid);
    }

    /// <summary>
    /// Parse CorrelationId from Guid.
    /// </summary>
    public static CorrelationId From(Guid guid) => new(guid);

    /// <summary>
    /// Valor GUID subjacente.
    /// </summary>
    public Guid Value => _value;

    /// <summary>
    /// Retorna representação string do CorrelationId.
    /// </summary>
    public override string ToString() => _value.ToString();

    /// <summary>
    /// Verifica igualdade com outro CorrelationId.
    /// </summary>
    public bool Equals(CorrelationId? other) => other is not null && _value == other._value;

    /// <summary>
    /// Verifica igualdade com outro objeto.
    /// </summary>
    public override bool Equals(object? obj) => Equals(obj as CorrelationId);

    /// <summary>
    /// Retorna hash code do CorrelationId.
    /// </summary>
    public override int GetHashCode() => _value.GetHashCode();

    /// <summary>
    /// Operador de igualdade.
    /// </summary>
    public static bool operator ==(CorrelationId? left, CorrelationId? right) => Equals(left, right);

    /// <summary>
    /// Operador de desigualdade.
    /// </summary>
    public static bool operator !=(CorrelationId? left, CorrelationId? right) => !Equals(left, right);
}
