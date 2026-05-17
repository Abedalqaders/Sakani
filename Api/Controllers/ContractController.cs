using Application.Common.Interfaces.Contract;
using Application.Dto.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]

public class ContractController : ControllerBase
{
    private readonly IContractAppService _contractAppService;

    public ContractController(IContractAppService contractAppService)
    {
        _contractAppService = contractAppService;
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Tenant")]
    public async Task<ActionResult<ContractResponseDto>> GetById(Guid id, CancellationToken ct)
    {
        var contract = await _contractAppService.GetContractWithPaymentsAsync(id, ct);
        return Ok(contract);
    }

    [HttpPost]
    [Authorize(Roles = "Tenant")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateContractDto dto, CancellationToken ct)
    {
       
        var contractId = await _contractAppService.CreateContractAsync(dto, ct);

        return CreatedAtAction(nameof(GetById), new { id = contractId }, contractId);
    }
    [HttpGet("my-active")]
    [Authorize(Roles = "Renter")]
    [ProducesResponseType(typeof(MyContractDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MyContractDetailsDto>> GetMyActiveContract(CancellationToken ct)
    {
        var contract = await _contractAppService.GetMyContractAsync(ct);

        // هنا بنحصد فائدة استخدام FirstOrDefaultAsync
        // إذا النتيجة null، بنرجع 404 بدل ما يضرب السيرفر Error 500
        if (contract == null)
        {
   
            return NotFound(new { message = "There is no active contract associated with this account at this time." });
        }

        return Ok(contract);
    }

    [HttpGet]
    [Authorize(Roles = "Tenant")]
    public async Task<ActionResult<IReadOnlyList<ContractBasicResponseDto>>> GetAll(CancellationToken ct)
    {
        var contracts = await _contractAppService.GetBasicContractsForTenantAsync(ct);
        return Ok(contracts);
    }

    [HttpGet("unit/{unitId:guid}",Name ="GetActiveContractForUnit")]
    [Authorize(Roles = "Tenant")]
    public async Task<ActionResult<ContractBasicResponseDto>> GetActiveByUnitId(Guid unitId, CancellationToken ct)
    {
        var contract = await _contractAppService.GetActiveContractByUnitId(unitId, ct);
        return Ok(contract);
    }
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Tenant")]
    public async Task<ActionResult<Guid>> Terminate(Guid id, CancellationToken ct)
    {
        var contractId = await _contractAppService.TerminateContractAsync(id, ct);
        return Ok(contractId);
    }
}