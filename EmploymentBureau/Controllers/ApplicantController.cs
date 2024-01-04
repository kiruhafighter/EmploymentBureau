using EmploymentBureau.DataAccess.DTOs;
using EmploymentBureau.DataAccess.Entities;
using EmploymentBureau.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmploymentBureau.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class ApplicantController : ControllerBase
{
    private readonly ApplicantRepository _applicantRepository;
    
    public ApplicantController(ApplicantRepository applicantRepository)
    {
        _applicantRepository = applicantRepository;
    }
    
    // POST: api/Applicants
    [HttpPost]
    public async Task<ActionResult<Applicant>> PostApplicant(Applicant applicant)
    {
        var applicantId = await _applicantRepository.AddApplicant(applicant);
        applicant.ApplicantId = applicantId;
        return CreatedAtAction(nameof(GetApplicant), new { id = applicantId }, applicant);
    }

    // GET: api/Applicants/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Applicant>> GetApplicant(int id)
    {
        var applicant = await _applicantRepository.GetApplicant(id);

        if (applicant == null)
        {
            return NotFound();
        }

        return applicant;
    }

    // GET: api/Applicants
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Applicant>>> GetApplicants()
    {
        return Ok(await _applicantRepository.GetAllApplicants());
    }

    // PUT: api/Applicants/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutApplicant(int id, Applicant applicant)
    {
        if (id != applicant.ApplicantId)
        {
            return BadRequest();
        }

        await _applicantRepository.UpdateApplicant(applicant);

        return NoContent();
    }

    // DELETE: api/Applicants/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteApplicant(int id)
    {
        var applicant = await _applicantRepository.GetApplicant(id);
        if (applicant == null)
        {
            return NotFound();
        }

        await _applicantRepository.DeleteApplicant(id);

        return NoContent();
    }
    
    [HttpGet("applicants-with-contracts-employers")]
    public async Task<ActionResult<IEnumerable<ApplicantWithContracts>>> GetApplicantsWithContractsAndEmployers()
    {
        return Ok(await _applicantRepository.GetApplicantsWithContractsAndEmployers());
    }
    
    [HttpGet("applicants-for-employers")]
    public async Task<ActionResult<IEnumerable<SuitableApplicant>>> FindApplicantsSuitableForEmployers()
    {
        return Ok(await _applicantRepository.FindApplicantsSuitableForEmployers());
    }
    
    [HttpGet("applicants-without-contracts")]
    public async Task<ActionResult<IEnumerable<Applicant>>> GetApplicantsWithNoContracts()
    {
        return Ok(await _applicantRepository.GetApplicantsWithNoContracts());
    }
    
    [HttpGet("applicants-with-multiple-employers")]
    public async Task<ActionResult<IEnumerable<ApplicantWithEmployersCount>>> GetApplicantsWithMultipleEmployers()
    {
        return Ok(await _applicantRepository.GetApplicantsWithMultipleEmployers());
    }
    
    [HttpGet("applicants-by-qualification-salary")]
    public async Task<ActionResult<IEnumerable<Applicant>>> SearchApplicantsByQualificationAndSalary([AsParameters] string qualification, decimal minSalary, decimal maxSalary)
    {
        return Ok(await _applicantRepository.SearchApplicantsByQualificationAndSalary(qualification, minSalary, maxSalary));
    }
}