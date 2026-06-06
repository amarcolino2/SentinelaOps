namespace SentinelaOps.Domain.Core;

/// <summary>
/// Contrato de repositório para MonitoringEvent agregado.
/// Implementação será na camada Infrastructure.
/// </summary>
public interface IMonitoringEventRepository
{
    /// <summary>
    /// Persiste novo evento no repositório.
    /// </summary>
    Task AddAsync(MonitoringEvent monitoringEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém evento por seu ID.
    /// </summary>
    Task<MonitoringEvent?> GetByIdAsync(EventId eventId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todos os eventos por correlação ID.
    /// </summary>
    Task<IEnumerable<MonitoringEvent>> GetByCorrelationIdAsync(CorrelationId correlationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém eventos por zona.
    /// </summary>
    Task<IEnumerable<MonitoringEvent>> GetByZoneAsync(string zone, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém eventos por status.
    /// </summary>
    Task<IEnumerable<MonitoringEvent>> GetByStatusAsync(EventStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza evento existente.
    /// </summary>
    Task UpdateAsync(MonitoringEvent monitoringEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deleta evento por ID.
    /// </summary>
    Task DeleteAsync(EventId eventId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta total de eventos.
    /// </summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
