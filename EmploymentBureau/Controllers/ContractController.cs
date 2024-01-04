using EmploymentBureau.DataAccess.DTOs;
using EmploymentBureau.DataAccess.Entities;
using EmploymentBureau.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmploymentBureau.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class ContractController : ControllerBase
{
    private readonly ContractRepository _contractRepository;
    
    public ContractController(ContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }
    
    // POST: api/Contracts
    [HttpPost]
    public async Task<ActionResult<Contract>> PostContract(Contract contract)
    {
        var contractId = await _contractRepository.AddContract(contract);
        contract.ContractId = contractId;
        return CreatedAtAction(nameof(GetContract), new { id = contractId }, contract);
    }

    // GET: api/Contracts/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Contract>> GetContract(int id)
    {
        var contract = await _contractRepository.GetContract(id);

        if (contract == null)
        {
            return NotFound();
        }

        return contract;
    }

    // GET: api/Contracts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Contract>>> GetContracts()
    {
        return Ok(await _contractRepository.GetAllContracts());
    }

    // PUT: api/Contracts/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutContract(int id, Contract contract)
    {
        if (id != contract.ContractId)
        {
            return BadRequest();
        }

        await _contractRepository.UpdateContract(contract);

        return NoContent();
    }

    // DELETE: api/Contracts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContract(int id)
    {
        var contract = await _contractRepository.GetContract(id);
        if (contract == null)
        {
            return NotFound();
        }

        await _contractRepository.DeleteContract(id);

        return NoContent();
    }
    
    [HttpGet("with-high-commission")]
    public async Task<ActionResult<IEnumerable<Contract>>> GetContractsWithHighCommission([AsParameters] decimal threshold)
    {
        return Ok(await _contractRepository.GetContractsWithHighCommission(threshold));
    }
    
    [HttpGet("average-commission-per-position")]
    public async Task<ActionResult<IEnumerable<AverageCommissionByPosition>>> GetAverageCommissionByPosition()
    {
        return Ok(await _contractRepository.GetAverageCommissionByPosition());
    }
}