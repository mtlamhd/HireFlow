namespace HireFlow.Domain.Dtos.CityDto;

public class CityViewDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid ProvinceId { get; set; }
}