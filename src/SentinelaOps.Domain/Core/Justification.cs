namespace SentinelaOps.Domain.Core;

/// <summary>
/// Justificativa textual para uma classificação ou ação.
/// Imutável, Value Object.
/// Máximo 2000 caracteres.
/// </summary>
public class Justification : IEquatable<Justification>
{
    private readonly string _text;

    /// <summary>
    /// Tamanho máximo permitido para justificativa.
    /// </summary>
    public const int MaxLength = 2000;

    private Justification(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Justification cannot be empty", nameof(text));

        if (text.Length > MaxLength)
            throw new ArgumentException($"Justification cannot exceed {MaxLength} characters", nameof(text));

        _text = text.Trim();
    }

    /// <summary>
    /// Factory method para criar Justification.
    /// </summary>
    public static Justification Create(string text) => new(text);

    /// <summary>
    /// Texto da justificativa.
    /// </summary>
    public string Text => _text;

    /// <summary>
    /// Retorna representação string da justificativa.
    /// </summary>
    public override string ToString() => _text;

    /// <summary>
    /// Verifica igualdade com outra Justification.
    /// </summary>
    public bool Equals(Justification? other) => other is not null && _text == other._text;

    /// <summary>
    /// Verifica igualdade com outro objeto.
    /// </summary>
    public override bool Equals(object? obj) => Equals(obj as Justification);

    /// <summary>
    /// Retorna hash code da justificativa.
    /// </summary>
    public override int GetHashCode() => _text.GetHashCode();

    /// <summary>
    /// Operador de igualdade.
    /// </summary>
    public static bool operator ==(Justification? left, Justification? right) => Equals(left, right);

    /// <summary>
    /// Operador de desigualdade.
    /// </summary>
    public static bool operator !=(Justification? left, Justification? right) => !Equals(left, right);
}
