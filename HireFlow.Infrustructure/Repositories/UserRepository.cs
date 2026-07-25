using HireFlow.Domain.Dtos.AdminDto;
using HireFlow.Domain.Dtos.SkillDto;
using HireFlow.Domain.Dtos.UserDto;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repo;
using HireFlow.Infrustructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrustructure.Repositories;


    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context; 

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PendingEmployerDto>> GetUnapprovedEmployersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Where(user =>
                    !user.IsApproved &&
                    user.Companies.Any())
                .Select(user => new PendingEmployerDto
                {
                    UserId = user.Id,
                    Username = user.UserName!,

                    CompanyId = user.Companies
                        .OrderBy(company => company.CreatedAt)
                        .Select(company => company.Id)
                        .First(),

                    CompanyName = user.Companies
                        .OrderBy(company => company.CreatedAt)
                        .Select(company => company.Name)
                        .First()
                })
                .ToListAsync();
        }
        
        public async Task<JobSeekerProfileDto?> GetProfileByIdAsync(Guid userId)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new JobSeekerProfileDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber!,
                    Email = u.Email,
                    BirthDate = u.BirthDate,
                    NationalId = u.NationalId,
                    ProfileImageId = u.ProfileImageId,
                    ResumeId = u.ResumeId,
                    Skills = u.UserSkills.Select(us => new SkillViewDto
                    {
                        Id = us.Skill.Id,
                        Name = us.Skill.Name
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }
        public async Task<bool> UpdateMyProfileAsync(Guid userId, UpdateJobSeekerProfileDto dto, List<Guid> validSkillIds, Guid requesterId)
            {
               
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return false;
                }
        
               
                user.UpdateProfile(dto.FirstName, dto.LastName, requesterId);
                user.CompletePersonalInfo(dto.NationalId, dto.BirthDate, requesterId);
                user.Email = dto.Email;
                user.NormalizedEmail = dto.Email.ToUpper();
        
                
                await _context.UserSkills
                    .Where(us => us.UserId == userId)
                    .ExecuteDeleteAsync();
                
                if (validSkillIds.Any())
                {
                    var newUserSkills = validSkillIds.Select(skillId => new UserSkill(userId, skillId));
                    await _context.UserSkills.AddRangeAsync(newUserSkills);
                }
                
                await _context.SaveChangesAsync();
                return true;
            }
        public async Task<bool> SetUserResumeAsync(Guid userId, Guid attachmentId, Guid requesterId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return false;
            
            user.SetResume(attachmentId, requesterId);
    
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveUserResumeAsync(Guid userId, Guid requesterId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return false;

           
            user.RemoveResume(requesterId);
    
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetUserProfileImageAsync(Guid userId, Guid attachmentId, Guid requesterId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return false;

          
            user.SetProfileImage(attachmentId, requesterId);
    
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveUserProfileImageAsync(Guid userId, Guid requesterId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return false;

           
            user.RemoveProfileImage(requesterId);
    
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<List<AdminJobSeekerSummaryDto>> GetAllJobSeekersAsync(string roleName)
        {
            
            var roleId = await _context.Roles
                .Where(r => r.Name == roleName)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (roleId == Guid.Empty)
                return new List<AdminJobSeekerSummaryDto>();

           
            return await _context.Users
                .AsNoTracking()
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == roleId))
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new AdminJobSeekerSummaryDto
                {
                    Id = u.Id,
                    Username = u.UserName!,
                    FullName = (string.IsNullOrWhiteSpace(u.FirstName) && string.IsNullOrWhiteSpace(u.LastName))
                        ? u.UserName!
                        : (u.FirstName + " " + u.LastName).Trim(),
                    Email = u.Email,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }
        
        public async Task<AdminJobSeekerDetailsDto?> GetJobSeekerDetailsForAdminAsync(Guid id)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new AdminJobSeekerDetailsDto
                {
                    Id = u.Id,
                    Username = u.UserName!,
                    FullName = (string.IsNullOrWhiteSpace(u.FirstName) && string.IsNullOrWhiteSpace(u.LastName))
                        ? u.UserName!
                        : (u.FirstName + " " + u.LastName).Trim(),
                    Email = u.Email,
                    NationalId = u.NationalId,
                    BirthDate = u.BirthDate,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    ResumeId = u.ResumeId,
                    ResumeFileName = u.Resume != null ? u.Resume.FileName : null,
                    Skills = u.UserSkills.Select(us => new SkillViewDto()
                    {
                        Id = us.Skill.Id,
                        Name = us.Skill.Name
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }
        
    }