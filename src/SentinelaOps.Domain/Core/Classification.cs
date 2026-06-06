namespace SentinelaOps.Domain.Core;

/// <summary>
/// Classificação de um evento após análise.
/// Valores possíveis: Valid, PossibleFalsePositive, Suspicious, HumanReviewRequired, Inconclusive
/// Imutável, Value Object.
/// </summary>
public class Classification : IEquatable<Classification>
{
    /// <summary>
    /// Classificação de evento válido e confirmado.
    /// </summary>
    public static readonly Classification Valid = new(ClassificationValue.Valid);

    /// <summary>
    /// Classificação de possível falso positivo.
    /// </summary>
    public static readonly Classification PossibleFalsePositive = new(ClassificationValue.PossibleFalsePositive);

    /// <summary>
    /// Classificação de evento suspeito.
    /// </summary>
    public static readonly Classification Suspicious = new(ClassificationValue.Suspicious);

    /// <summary>
    /// Classificação requer revisão humana.
    /// </summary>
    public static readonly Classification HumanReviewRequired = new(ClassificationValue.HumanReviewRequired);

    /// <summary>
    /// Classificação inconclusiva.
    /// </summary>
    public static readonly Classification Inconclusive = new(ClassificationValue.Inconclusive);

    private readonly ClassificationValue _value;

    private Classification(ClassificationValue value)
    {
        _value = value;
    }

    /// <summary>
    /// Factory method para criar Classification.
    /// </summary>
    public static Classification Create(ClassificationValue value) => new(value);

    /// <summary>
    /// Parse Classification from string.
    /// </summary>
    public static Classification Parse(string value)
    {
        if (!Enum.TryParse<ClassificationValue>(value, ignoreCase: true, out var enumValue))
            throw new ArgumentException($"Invalid classification value: {value}", nameof(value));

        return new Classification(enumValue);
    }

    /// <summary>
    /// Valor da classificação.
    /// </summary>
    public ClassificationValue Value => _value;

    /// <summary>
    /// Se classificação indica potencial ameaça.
    /// </summary>
    public bool IsThreat => _value is ClassificationValue.Suspicious or ClassificationValue.HumanReviewRequired;

    /// <summary>
    /// Se classificação requer ação humana.
    /// </summary>
    public bool RequiresHumanReview => _value is ClassificationValue.HumanReviewRequired or ClassificationValue.Inconclusive;

    /// <summary>
    /// Retorna representação string da classificação.
    /// </summary>
    public override string ToString() => _value.ToString();

    /// <summary>
    /// Verifica igualdade com outra instância de Classification.
    /// </summary>
    public bool Equals(Classification? other) => other is not null && _value == other._value;

    /// <summary>
    /// Verifica igualdade com outro objeto.
    /// </summary>
    public override bool Equals(object? obj) => Equals(obj as Classification);

    /// <summary>
    /// Retorna hash code da classificação.
    /// </summary>
    public override int GetHashCode() => _value.GetHashCode();

    /// <summary>
    /// Operador de igualdade.
    /// </summary>
    public static bool operator ==(Classification? left, Classification? right) => Equals(left, right);

    /// <summary>
    /// Operador de desigualdade.
    /// </summary>
    public static bool operator !=(Classification? left, Classification? right) => !Equals(left, right);
}

/// <summary>
/// Enum for Classification values.
/// </summary>
public enum ClassificationValue
{
    /// <summary>
    /// Evento válido e confirmado.
    /// </summary>
    Valid = 0,

    /// <summary>
    /// Evento que pode ser falso positivo.
    /// </summary>
    PossibleFalsePositive = 1,

    /// <summary>
    /// Evento suspeito que requer monitoramento.
    /// </summary>
    Suspicious = 2,

    /// <summary>
    /// Evento que requer revisão humana.
    /// </summary>
    HumanReviewRequired = 3,

    /// <summary>
    /// Classificação inconclusiva.
    /// </summary>
    Inconclusive = 4
}
