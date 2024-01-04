namespace EmploymentBureau.DataAccess.Entities;

public sealed class Employer
{
    public int EmployerId { get; set; }
        
    public string Name  { get; set; }
    
    public string ActivityType  { get; set; }
        
    public string Address  { get; set; }
    
    public int Number  { get; set; }
}