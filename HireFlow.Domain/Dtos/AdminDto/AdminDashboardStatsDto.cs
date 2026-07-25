namespace HireFlow.Domain.Dtos.AdminDto;

public class AdminDashboardStatsDto
{
    
    public int TotalJobSeekers { get; set; }
    public int TotalEmployers { get; set; }
    public int TotalPendingEmployers { get; set; }

  
    public int TotalActiveJobAds { get; set; }
    public int TotalInactiveJobAds { get; set; }

    
    public int InitialRequestsCount { get; set; }
    public int UnderReviewRequestsCount { get; set; }
    public int InterviewRequestsCount { get; set; }
    public int AcceptedRequestsCount { get; set; }
    public int RejectedRequestsCount { get; set; }
    public int CancelledRequestsCount { get; set; }
}