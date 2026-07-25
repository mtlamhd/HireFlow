namespace HireFlow.Domain.Dtos.UserDto;

public class EmployerProfileDto
{
   
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Username { get; set; } 
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
    
       
        public int? Age 
        { 
            get 
            {
                if (!BirthDate.HasValue)
                    return null;

                var today = DateTime.UtcNow.Date;
                var age = today.Year - BirthDate.Value.Year;

                if (BirthDate.Value.Date > today.AddYears(-age))
                    age--;

                return age;
            }
        }

        public string? NationalId { get; set; }
        public Guid? ProfileImageId { get; set; } 
        public bool IsApproved { get; set; } 
    }
