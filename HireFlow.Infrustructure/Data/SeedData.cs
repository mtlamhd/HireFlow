using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrustructure.Data;

public class SeedData
{
   
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category("Software and IT"),
                    new Category("Marketing and Advertising"),
                    new Category("Design and Art"),
                    new Category("Sales"),
                    new Category("Finance and Accounting"),
                    new Category("Customer Support")
                };
                await context.Categories.AddRangeAsync(categories);
            }

            if (!await context.Provinces.AnyAsync())
            {
                var tehran = new Province("Tehran");
                var isfahan = new Province("Isfahan");
                var fars = new Province("Fars");

                await context.Provinces.AddRangeAsync(tehran, isfahan, fars);

                var cities = new List<City>
                {
                    new City("Tehran", tehran.Id),
                    new City("Shemiranat", tehran.Id),
                    new City("Rey", tehran.Id),
                    new City("Isfahan", isfahan.Id),
                    new City("Kashan", isfahan.Id),
                    new City("Shiraz", fars.Id)
                };
                await context.Cities.AddRangeAsync(cities);
            }
            if (!await context.Skills.AnyAsync())
            {
                var skills = new List<Skill>
                {
                    new Skill("C# and .NET"),
                    new Skill("ASP.NET Core Web API"),
                    new Skill("SQL Server / Entity Framework"),
                    new Skill("JavaScript / TypeScript"),
                    new Skill("React.js"),
                    new Skill("Python"),
                    new Skill("Docker and DevOps"),
                    new Skill("Git and GitHub"),
                    new Skill("Project Management"),
                    new Skill("UI/UX Design")
                };
                await context.Skills.AddRangeAsync(skills);
            }

            await context.SaveChangesAsync();
        }
    }
