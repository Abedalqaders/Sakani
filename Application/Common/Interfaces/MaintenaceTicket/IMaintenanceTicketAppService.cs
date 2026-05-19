using Application.Dto.MaintenanceTicket;

namespace Application.Common.Interfaces.MaintenaceTicket
{
    public interface IMaintenanceTicketAppService
    {
        Task<Guid> CreateTicketAsync(CreateTicketDto dto, CancellationToken ct);
        Task UpdateAsync(UpdateTicketDto dto, CancellationToken ct); // أضفنا هذه
        Task<IReadOnlyList<TicketResponseDto>> GetMyTicketsAsync(CancellationToken ct);
        Task<TicketResponseDto?> GetByIdForRenterAsync(Guid id, CancellationToken ct);
        Task<TicketResponseDto?> GetByIdForTenantAsync(Guid id, CancellationToken ct);
        Task CancelTicketAsync(Guid id, CancellationToken ct);
        Task UpdateStatusAsync(UpdateTicketStatusDto dto, CancellationToken ct);
        Task<IReadOnlyList<TicketResponseDto>> GetAllTicketsAsync(TicketFilterDto filter, CancellationToken ct);
        // تم توحيد الاسم مع الكلاس
        Task<string> UploadImageAsync(Guid ticketId, Stream fileStream, string fileName, CancellationToken ct);
    }
}