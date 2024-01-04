using EmploymentBureau.DataAccess.Entities;

namespace EmploymentBureau.DataAccess.DTOs;

public class ContractDetail
{
    public Contract Contract { get; set; }
    public Employer Employer { get; set; }
}