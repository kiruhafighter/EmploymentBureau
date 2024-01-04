namespace EmploymentBureau.DataAccess.Entities;

public sealed class Applicant
{
    public int ApplicantId { get; set; }
        
    public string Surname  { get; set; }
    
    public string Name  { get; set; }
        
    public string Qualification  { get; set; }
    
    public string ActivityType  { get; set; }
    
    public string OtherData  { get; set; }
    
    public decimal DesiredSalary { get; set; }
}