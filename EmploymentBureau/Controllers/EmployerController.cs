using EmploymentBureau.DataAccess.DTOs;
using EmploymentBureau.DataAccess.Entities;
using EmploymentBureau.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmploymentBureau.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class EmployerController : ControllerBase
{
    private EmployerRepository _employerRepository;
    
    public EmployerController(EmployerRepository employerRepository)
    {
        _employerRepository = employerRepository;
    }
    
    // POST: api/Employers
    [HttpPost]
    public async Task<ActionResult<Employer>> PostEmployer(Employer employer)
    {
        var employerId = await _employerRepository.AddEmployer(employer);
        employer.EmployerId = employerId;
        return CreatedAtAction(nameof(GetEmployer), new { id = employerId }, employer);
    }

    // GET: api/Employers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Employer>> GetEmployer(int id)
    {
        var employer = await _employerRepository.GetEmployer(id);

        if (employer == null)
        {
            return NotFound();
        }

        return employer;
    }

    // GET: api/Employers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employer>>> GetEmployers()
    {
        return Ok(await _employerRepository.GetAllEmployers());
    }

    // PUT: api/Employers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEmployer(int id, Employer employer)
    {
        if (id != employer.EmployerId)
        {
            return BadRequest();
        }

        await _employerRepository.UpdateEmployer(employer);

        return NoContent();
    }

    // DELETE: api/Employers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployer(int id)
    {
        var employer = await _employerRepository.GetEmployer(id);
        if (employer == null)
        {
            return NotFound();
        }

        await _employerRepository.DeleteEmployer(id);

        return NoContent();
    }
    
    [HttpGet("employers-with-contracts")]
    public async Task<ActionResult<IEnumerable<EmployerWithContracts>>> GetEmployersWithContracts()
    {
        return Ok(await _employerRepository.GetEmployersWithContracts());
    }
    
    [HttpGet("employers-with-active-contracts")]
    public async Task<ActionResult<IEnumerable<Employer>>> GetEmployersWithActiveContracts()
    {
        return Ok(await _employerRepository.GetEmployersWithActiveContracts());
    }
    
    [HttpGet("employers-by-activity-type")]
    public async Task<ActionResult<IEnumerable<Employer>>> GetEmployersByActivityType([AsParameters] string activityType)
    {
        return Ok(await _employerRepository.GetEmployersByActivityType(activityType));
    }
    
    [HttpGet("contract-count")]
    public async Task<ActionResult<IEnumerable<ContractCountPerEmployer>>> GetContractCountPerEmployer()
    {
        return Ok(await _employerRepository.GetContractCountPerEmployer());
    }
}