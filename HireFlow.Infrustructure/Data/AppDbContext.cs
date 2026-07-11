using System.Reflection;
using HireFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrustructure.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Province> Provinces { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Notification>  Notifications { get; set; }
    public DbSet<JobAdSkill>  JobAdSkills { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<JobAd> JobAds  { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Attachment> Attachments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}