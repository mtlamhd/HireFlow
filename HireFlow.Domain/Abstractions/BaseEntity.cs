namespace HireFlow.Domain.Abstractions;

    public abstract class BaseEntity 
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public bool IsDeleted { get; private set; }

        public DateTime? UpdatedAt { get; private set; }

        public void SetUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            IsDeleted = true;
            SetUpdated();
        }
    }
    
