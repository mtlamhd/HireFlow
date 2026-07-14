using HireFlow.Domain.Interfaces.InterfaceOfService;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly ICityService _cityService;

    public CitiesController(ICityService cityService)
    {
        _cityService = cityService;
    }

    [HttpGet("by-province/{provinceId}")]
    public async Task<IActionResult> GetByProvince(Guid provinceId)
    {
        var cities = await _cityService.GetCitiesByProvinceIdAsync(provinceId);
        return Ok(cities);
    }
}