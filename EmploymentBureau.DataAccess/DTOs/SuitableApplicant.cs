using EmploymentBureau.DataAccess.Entities;

namespace EmploymentBureau.DataAccess.DTOs;

public class SuitableApplicant
{
    public Applicant Applicant { get; set; }
    public Employer SuitableEmployer { get; set; }
}