using EmploymentBureau.DataAccess.Entities;

namespace EmploymentBureau.DataAccess.DTOs;

public class ApplicantWithEmployersCount
{
    public Applicant Applicant { get; set; }
    public int EmployersCount { get; set; }
}