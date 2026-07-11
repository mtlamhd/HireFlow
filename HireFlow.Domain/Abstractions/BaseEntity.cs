using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Abstractions;

    public abstract class BaseEntity
    {
        public Guid Id { get; private set; } = new SequentialGuid.SequentialGuid();

       

        
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public Guid? CreatedById { get; protected set; }
        public User? Creator { get; protected set; }

        
        public DateTime? ModifiedAt { get; private set; }
        public Guid? ModifiedById { get; private set; }
        public User? Modifier { get; private set; }

        
        public DateTime? DeletedAt { get; private set; }
        public Guid? DeletedById { get; private set; }
        public User? Deleter { get; private set; }

        public bool IsDeleted { get; private set; }

        public void SetModificationInfo(Guid requesterId)
        {
            ModifiedAt = DateTime.UtcNow;
            ModifiedById = requesterId;
        }

        public void SetAsDeleted(Guid requesterId)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedById = requesterId;
            SetModificationInfo(requesterId);
        }
        
    }
    
