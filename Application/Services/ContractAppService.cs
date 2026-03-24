//using Application.Common.Interfaces;
//using Application.Dto.Contract;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Application.Services
//{
//    public class ContractAppService: IContractAppService
//    {
//        private readonly IContractAppService _contractAppService;
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IUnitAppService _unitAppService;
        
//        public ContractAppService(IContractAppService _contractAppService, IUnitOfWork _unitOfWork)
//        {
//            this._contractAppService = _contractAppService;
//            this._unitOfWork = _unitOfWork;
//        }

//        public async Task<Guid> CreateContractAsync(CreateContractDto dto, CancellationToken ct)
//        {
//            if(_unitAppService.GetByIdAsync(dto.UnitId))
//        }
//    }
//}
