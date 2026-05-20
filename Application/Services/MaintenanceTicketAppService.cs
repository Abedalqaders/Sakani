using Application.Common.Interfaces.ImageTicket;
using Application.Common.Interfaces.MaintenaceTicket;
using Application.Common.Interfaces.User;
using Application.Common.Interfaces.General;
using Application.Dto.MaintenanceTicket;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Common.Interfaces.Unit;
namespace Application.Services
{
    public class MaintenanceTicketAppService: IMaintenanceTicketAppService
    {
        private readonly IMaintenanceTicketRepository _ticketRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly ITicketImageRepository _imageRepository;
        private readonly IUnitAppService _unitAppService;
        public MaintenanceTicketAppService(
            IMaintenanceTicketRepository ticketRepository,
            ICurrentUserService currentUserService, // تصحيح النوع هنا
            IUnitOfWork unitOfWork,
            IFileService fileService, // إضافة النقص
            ITicketImageRepository imageRepository, IUnitAppService unitAppService) // إضافة النقص
        {
            _ticketRepository = ticketRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _imageRepository = imageRepository;
            _unitAppService = unitAppService;
        }
        public async Task<Guid> CreateTicketAsync(CreateTicketDto dto, CancellationToken ct)
        {
            var renterId = _currentUserService.RenterId.GetValueOrDefault(); 
            var unitid = await _unitAppService.GetUnitIdByRenter(ct);
            var ticket = new MaintenanceTicket
            {
                Id = Guid.NewGuid(),
                RenterId = renterId,
                UnitId = unitid,
                Subject = dto.Subject,
                Description = dto.Description,
                TicketStatus = TicketStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            _ticketRepository.Add(ticket);
            await _unitOfWork.SaveChangesAsync(ct);

            return ticket.Id;
        }
        public async Task UpdateAsync(UpdateTicketDto dto, CancellationToken ct)
        {
            var renterId = _currentUserService.RenterId.GetValueOrDefault();
            var ticket = await _ticketRepository.GetByIdAsync(dto.Id, ct);

            if (ticket == null || ticket.RenterId != renterId)
                throw new KeyNotFoundException("Ticket not found or access denied.");

            // Business Rule: لا يمكن تعديل التذكرة إذا استلمها الفني
            if (ticket.TicketStatus != TicketStatus.Open)
                throw new InvalidOperationException("Cannot update ticket. Management has already started processing it.");

            ticket.Subject = dto.Subject;
            ticket.Description = dto.Description;

            _ticketRepository.Update(ticket);
            await _unitOfWork.SaveChangesAsync(ct);
        }
        public async Task<IReadOnlyList<TicketResponseDto>> GetMyTicketsAsync(CancellationToken ct)
        {
            var renterId =  _currentUserService.RenterId.GetValueOrDefault();
            var query =   _ticketRepository.GetByRenterIdAsync(renterId);

            return await query
         .OrderByDescending(t => t.CreatedAt)
         .Select(t => new TicketResponseDto
         {
             Id = t.Id,
             UnitId = t.UnitId,
             UnitNo = t.Unit.UnitNo,
             Subject = t.Subject,
             Description = t.Description,
             Status = t.TicketStatus,
             CreatedAt = t.CreatedAt,
             Images = t.Images.Select(img => new TicketImageDto
             {
                 Id = img.Id,
                 ImageUrl = img.ImagePath
             }).ToList()
         })
         .ToListAsync(ct); 

        }

        public async Task<TicketResponseDto?> GetByIdForRenterAsync(Guid id, CancellationToken ct)
        {
            var renterId = _currentUserService.RenterId.GetValueOrDefault();
            var ticket = await _ticketRepository.GetByIdAsync(id, ct);
            
            
            if (ticket == null||ticket.RenterId!=renterId)
                return null;

            return new TicketResponseDto
            {
                Id = ticket.Id,
                UnitId = ticket.UnitId,
                UnitNo = ticket.Unit?.UnitNo ?? " ",
                Subject = ticket.Subject,
                Description = ticket.Description,
                Status = ticket.TicketStatus,
                CreatedAt = ticket.CreatedAt,

                Images = ticket.Images?.Select(img => new TicketImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImagePath
                }).ToList() ?? new List<TicketImageDto>()
            };
        }
        public async Task<TicketResponseDto?> GetByIdForTenantAsync(Guid id, CancellationToken ct)
        {
       
            var ticket = await _ticketRepository.GetByIdAsync(id, ct);

            if (ticket == null)
                return null;
            return new TicketResponseDto
            {
                Id = ticket.Id,
                UnitId = ticket.UnitId,
                UnitNo = ticket.Unit?.UnitNo ?? " ",
                Subject = ticket.Subject,
                Description = ticket.Description,
                Status = ticket.TicketStatus,
                CreatedAt = ticket.CreatedAt,

                Images = ticket.Images?.Select(img => new TicketImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImagePath
                }).ToList() ?? new List<TicketImageDto>()
            };
        }
        public async Task CancelTicketAsync(Guid id, CancellationToken ct)

        {

            var renterId = _currentUserService.RenterId.GetValueOrDefault();

            var ticket = await _ticketRepository.GetByIdAsync(id, ct);



            if (ticket == null || ticket.RenterId != renterId)

                throw new KeyNotFoundException("Ticket not found or access denied.");



            if (ticket.TicketStatus != TicketStatus.Open)

                throw new InvalidOperationException("You can only cancel an open ticket.");



            ticket.TicketStatus = TicketStatus.Closed;



            _ticketRepository.Update(ticket);

            await _unitOfWork.SaveChangesAsync(ct);

        }
        public async Task UpdateStatusAsync(UpdateTicketStatusDto dto, CancellationToken ct)
        {
            var ticket = await _ticketRepository.GetByIdAsync(dto.TicketId, ct);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found.");

            if (ticket.TicketStatus == TicketStatus.Closed )
                throw new InvalidOperationException("Cannot change status of a cancelled ticket.");

            // Reset escalation notification if reopened/put back to work
            if ((dto.NewStatus == TicketStatus.Open || dto.NewStatus == TicketStatus.InProgress) && 
                 ticket.TicketStatus != dto.NewStatus)
            {
                ticket.IsEscalationNotified = false;
            }

            ticket.TicketStatus = dto.NewStatus;

            _ticketRepository.Update(ticket);
            await _unitOfWork.SaveChangesAsync(ct);
        }
        public async Task<IReadOnlyList<TicketResponseDto>> GetAllTicketsAsync(TicketFilterDto filter, CancellationToken ct)
        {
            var tickets = await _ticketRepository.GetFilteredAsync(filter, ct);

            return tickets.Select(t => new TicketResponseDto
            {
                Id = t.Id,
                UnitId = t.UnitId,
                Subject = t.Subject,
                Description = t.Description,
                Status = t.TicketStatus,
                CreatedAt = t.CreatedAt
            }).ToList();
        }
        public async Task<string> UploadImageAsync(Guid ticketId, Stream fileStream, string fileName, CancellationToken ct)
        {
            var renterId = _currentUserService.RenterId.GetValueOrDefault();
            var ticket = await _ticketRepository.GetByIdAsync(ticketId, ct);

            if (ticket == null || ticket.RenterId != renterId)
                throw new KeyNotFoundException("Ticket not found or access denied.");

            // 1. حفظ الملف فعلياً على السيرفر
            string imageUrl = await _fileService.SaveFileAsync(fileStream, fileName, "tickets");

            // 2. حفظ الرابط في الداتابيز
            var ticketImage = new TicketImage
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                ImagePath = imageUrl,
           
            };

            _imageRepository.Add(ticketImage); // افترض إنك ضفت IGenericRepository<TicketImage> بالـ Constructor
            await _unitOfWork.SaveChangesAsync(ct);

            return imageUrl;
        }

    }
}
