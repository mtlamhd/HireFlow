using System.ComponentModel.DataAnnotations;
using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Dtos.JobAdDto;

public class CreateJobAdDto
{
    
        [Required(ErrorMessage = "Job title is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters.")]
        public string Title { get; set; } 

        [Required(ErrorMessage = "Job description is required.")]
        [MinLength(10, ErrorMessage = "Description must be at least 10 characters long.")]
        public string Description { get; set; } 

        [Required(ErrorMessage = "City selection is required.")]
        public Guid CityId { get; set; }

        [Required(ErrorMessage = "Category selection is required.")]
        public Guid CategoryId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number.")]
        public decimal? Salary { get; set; }

        [Required(ErrorMessage = "Employment type is required.")]
        [EnumDataType(typeof(EmploymentTypeEnum), ErrorMessage = "Invalid employment type.")]
        public EmploymentTypeEnum EmploymentType { get; set; }

        
        public List<Guid> SkillIds { get; set; } = new();
    }

