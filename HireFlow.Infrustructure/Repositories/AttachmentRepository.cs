using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;

namespace HireFlow.Infrustructure.Repositories;

public class AttachmentRepository : GenericRepository<Attachment>, IAttachmentRepository
{
    public AttachmentRepository(AppDbContext context) : base(context)
    {
    }
}