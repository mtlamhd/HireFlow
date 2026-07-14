using HireFlow.Domain.Abstractions;

namespace HireFlow.Domain.Entities;

public class CompanyCategory : BaseEntity
{
    public Guid CompanyId { get; private set; }
    public Company Company { get; private set; } 

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } 

    private CompanyCategory() { }

    public CompanyCategory(Guid companyId, Guid categoryId)
    {
        CompanyId = companyId;
        CategoryId = categoryId;
    }
}