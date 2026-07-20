namespace HireFlow.Domain.Entities;

public class Paging
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public int GetSkip()
    {
        return (PageNumber - 1) * PageSize;
    }
}