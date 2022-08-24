namespace DesafioDtiBack.Models;
public class LoanRequest
{
    public PersonType Type { get; set; }
    public string? Document { get; set; }
    public string? Name { get; set; }
    public double CurrentDebt { get; set; }
    public double RequestedLoan { get; set; }
}