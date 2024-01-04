namespace EmploymentBureau.DataAccess.Entities;

public sealed class Contract
{
    public int ContractId { get; set; }
        
    public int ApplicantId  { get; set; }
    
    public int EmployerId  { get; set; }
        
    public string Position  { get; set; }
    
    public decimal Commission  { get; set; }
}