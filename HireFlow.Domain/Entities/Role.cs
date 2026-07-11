using Microsoft.AspNetCore.Identity;

namespace HireFlow.Domain.Entities;

public sealed class Role : IdentityRole<Guid>
{
    private Role()
    {
    }

    public Role(string rollName) : base(rollName)
    {
        Id = new SequentialGuid.SequentialGuid();
    }
}