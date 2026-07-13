namespace HireFlow.Domain.Dtos.CompanyDto;

public class CompanyDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public Guid? LogoId { get; set; }
}