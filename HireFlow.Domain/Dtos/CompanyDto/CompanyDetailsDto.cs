using HireFlow.Domain.Dtos.CategoryDto;

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

    public Guid? CityId { get; set; }

    public string? CityName { get; set; }

    public Guid? ProvinceId { get; set; }

    public string? ProvinceName { get; set; }

    public List<CategoryViewDto> Categories { get; set; } = new();
}