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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContractResponseDto>> GetById(Guid id, CancellationToken ct)
    {
        var contract = await _contractAppService.GetContractWithPaymentsAsync(id, ct);
        if (contract == null) {
            return NotFound("Contract not found");
        }
        return Ok(contract);
    }

    [HttpPost]
    [Authorize(Roles = "Tenant")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateContractDto dto, CancellationToken ct)
    {
       
        var contractId = await _contractAppService.CreateContractAsync(dto, ct);

        return CreatedAtAction(nameof(GetById), new { id = contractId }, contractId);
    }
    [HttpGet("my-active")]
    [Authorize(Roles = "Renter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MyContractDetailsDto>> GetMyActiveContract(CancellationToken ct)
    {
        var contract = await _contractAppService.GetMyContractAsync(ct);

        if (contract == null)
        {
   
            return NotFound("No Contract Found");
        }

        return Ok(contract);
    }

    [HttpGet]
    [Authorize(Roles = "Tenant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ContractBasicResponseDto>>> GetAll(CancellationToken ct)
    {
        var contracts = await _contractAppService.GetBasicContractsForTenantAsync(ct);
        return Ok(contracts);
    }

    [HttpGet("unit/{unitId:guid}",Name ="GetActiveContractForUnit")]
    [Authorize(Roles = "Tenant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContractBasicResponseDto>> GetActiveByUnitId(Guid unitId, CancellationToken ct)
    {
        var contract = await _contractAppService.GetActiveContractByUnitId(unitId, ct);
        if(contract == null)
        {
            return NotFound("No active contract found for the specified unit.");
        }
        return Ok(contract);
    }
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Tenant")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Terminate(Guid id, CancellationToken ct)
    {
        var contractId = await _contractAppService.TerminateContractAsync(id, ct);
        return NoContent();
    }
}