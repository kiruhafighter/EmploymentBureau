using EmploymentBureau.DataAccess.Entities;

namespace EmploymentBureau.DataAccess.DTOs;

public class ApplicantWithContracts
{
    public Applicant Applicant { get; set; }
    public List<ContractDetail> Contracts { get; set; }
}