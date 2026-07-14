using HireFlow.Domain.Interfaces.InterfaceOfService;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProvincesController : ControllerBase
{
    private readonly IProvinceService _provinceService;

    public ProvincesController(IProvinceService provinceService)
    {
        _provinceService = provinceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var provinces = await _provinceService.GetAllProvincesAsync();
        return Ok(provinces);
    }
}