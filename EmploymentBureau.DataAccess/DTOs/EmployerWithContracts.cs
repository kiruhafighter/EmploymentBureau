using EmploymentBureau.DataAccess.Entities;

namespace EmploymentBureau.DataAccess.DTOs;

public class EmployerWithContracts
{
    public Employer Employer { get; set; }
    public List<Contract> Contracts { get; set; }
}