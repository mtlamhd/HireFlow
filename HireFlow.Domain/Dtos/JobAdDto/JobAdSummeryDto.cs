using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Dtos.JobAdDto;


    public class JobAdSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } 
        public string CityName { get; set; } 
        public string CategoryName { get; set; } 
        public decimal? Salary { get; set; }
        public EmploymentTypeEnum EmploymentType { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpireAt { get; set; }
        public int ApplicationsCount { get; set; } 
    }
