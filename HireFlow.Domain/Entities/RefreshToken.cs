using HireFlow.Domain.Abstractions;

namespace HireFlow.Domain.Entities;

    
public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } 

    public string Token { get; private set; } 

    public DateTime ExpiresAt { get; private set; }

    public bool IsRevoked { get; private set; } = false;

  
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

   
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken() { } 

    public RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        IsRevoked = false;
    }

    
    public void Revoke(Guid requesterId)
    {
        if (IsRevoked) 
            return;

        IsRevoked = true;
        SetModificationInfo(requesterId);
    }
}