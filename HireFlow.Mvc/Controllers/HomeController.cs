using System.Diagnostics;
using HireFlow.Domain.Dtos.JobAdDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repo;
using Microsoft.AspNetCore.Mvc;
using HireFlow.Mvc.Models;

namespace HireFlow.Mvc.Controllers;
public class HomeController : Controller
{
private readonly IUnitOfWork _unitOfWork;

    public HomeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IActionResult> Index([FromQuery] JobAdSearchDto searchDto)
    {
        List<PublicJobAdSummaryDto> jobAds;
        
        if (IsSearchFilterApplied(searchDto))
        {
            jobAds = await _unitOfWork.JobAds.SearchActiveJobAdsAsync(searchDto);
        }
        else
        {
            var paging = new Paging { PageNumber = searchDto.PageNumber <= 0 ? 1 : searchDto.PageNumber, PageSize = 10 };
            jobAds = await _unitOfWork.JobAds.GetActiveJobAdsAsync(paging);
        }
        
        ViewBag.Cities = await _unitOfWork.Cities.QueryAsync(c => true, new Paging { PageSize = 100 });
        ViewBag.Categories = await _unitOfWork.Categories.QueryAsync(c => true, new Paging { PageSize = 100 });
        
        ViewBag.EmploymentTypes = Enum.GetValues(typeof(EmploymentTypeEnum))
            .Cast<EmploymentTypeEnum>()
            .Select(e => new { Value = (int)e, Name = GetEmploymentTypeName(e) })
            .ToList();
        
        ViewBag.SearchModel = searchDto;

        return View(jobAds);
    }

  
    private string GetEmploymentTypeName(EmploymentTypeEnum type)
    {
        return type switch
        {
            EmploymentTypeEnum.FullTime => "تمام‌وقت",
            EmploymentTypeEnum.PartTime => "پاره‌وقت",
            EmploymentTypeEnum.Remote => "دورکاری",
            EmploymentTypeEnum.Internship => "کارآموزی",
            _ => type.ToString()
        };
    }

   
    public async Task<IActionResult> Details(Guid id)
    {
        if (id == Guid.Empty)
            return NotFound();

        var jobAd = await _unitOfWork.JobAds.GetPublicJobAdDetailsAsync(id);
        if (jobAd == null || !jobAd.IsActive || jobAd.ExpireAt <= DateTime.UtcNow)
        {
            return NotFound("آگهی مورد نظر یافت نشد یا منقضی شده است.");
        }

        return View(jobAd);
    }

    private bool IsSearchFilterApplied(JobAdSearchDto dto)
    {
        return !string.IsNullOrWhiteSpace(dto.Title) ||
               dto.EmploymentType.HasValue ||
               (dto.CityId.HasValue && dto.CityId.Value != Guid.Empty) ||
               (dto.CategoryId.HasValue && dto.CategoryId.Value != Guid.Empty) ||
               dto.MinSalary.HasValue;
    }
}