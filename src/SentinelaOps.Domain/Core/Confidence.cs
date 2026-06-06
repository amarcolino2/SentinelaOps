namespace SentinelaOps.Domain.Core;

/// <summary>
/// Confiança em análise: range 0.0 até 1.0 (0% até 100%).
/// Imutável, Value Object.
/// </summary>
public class Confidence : IEquatable<Confidence>, IComparable<Confidence>
{
    private readonly double _value;

    /// <summary>
    /// Valor mínimo permitido para confiança.
    /// </summary>
    public const double MinValue = 0.0;

    /// <summary>
    /// Valor máximo permitido para confiança.
    /// </summary>
    public const double MaxValue = 1.0;

    private Confidence(double value)
    {
        if (value < MinValue || value > MaxValue)
            throw new ArgumentOutOfRangeException(nameof(value), $"Confidence must be between {MinValue} and {MaxValue}");

        _value = value;
    }

    /// <summary>
    /// Factory method para criar Confidence.
    /// </summary>
    public static Confidence Create(double value) => new(value);

    /// <summary>
    /// Factory method para criar Confidence a partir de percentual (0-100).
    /// </summary>
    public static Confidence FromPercentage(double percentage) => new(percentage / 100.0);

    /// <summary>
    /// Máxima confiança possível (1.0 / 100%).
    /// </summary>
    public static Confidence MaxConfidence => new(MaxValue);

    /// <summary>
    /// Mínima confiança possível (0.0 / 0%).
    /// </summary>
    public static Confidence MinConfidence => new(MinValue);

    /// <summary>
    /// Valor de confiança (0.0-1.0).
    /// </summary>
    public double Value => _value;

    /// <summary>
    /// Valor como percentual (0-100).
    /// </summary>
    public double Percentage => _value * 100.0;

    /// <summary>
    /// Retorna representação string como percentual.
    /// </summary>
    public override string ToString() => $"{Percentage:F2}%";

    /// <summary>
    /// Verifica igualdade com outra instância de Confidence.
    /// </summary>
    public bool Equals(Confidence? other) => other is not null && Math.Abs(_value - other._value) < 0.0001;

    /// <summary>
    /// Verifica igualdade com outro objeto.
    /// </summary>
    public override bool Equals(object? obj) => Equals(obj as Confidence);

    /// <summary>
    /// Retorna hash code de confiança.
    /// </summary>
    public override int GetHashCode() => _value.GetHashCode();

    /// <summary>
    /// Compara com outra instância de Confidence.
    /// </summary>
    public int CompareTo(Confidence? other)
    {
        if (other is null) return 1;
        return _value.CompareTo(other._value);
    }

    /// <summary>
    /// Operador de igualdade.
    /// </summary>
    public static bool operator ==(Confidence? left, Confidence? right) => Equals(left, right);

    /// <summary>
    /// Operador de desigualdade.
    /// </summary>
    public static bool operator !=(Confidence? left, Confidence? right) => !Equals(left, right);

    /// <summary>
    /// Operador de menor que.
    /// </summary>
    public static bool operator <(Confidence? left, Confidence? right) => left is not null && right is not null && left._value < right._value;

    /// <summary>
    /// Operador de menor ou igual.
    /// </summary>
    public static bool operator <=(Confidence? left, Confidence? right) => left is not null && right is not null && left._value <= right._value;

    /// <summary>
    /// Operador de maior que.
    /// </summary>
    public static bool operator >(Confidence? left, Confidence? right) => left is not null && right is not null && left._value > right._value;

    /// <summary>
    /// Operador de maior ou igual.
    /// </summary>
    public static bool operator >=(Confidence? left, Confidence? right) => left is not null && right is not null && left._value >= right._value;
}
