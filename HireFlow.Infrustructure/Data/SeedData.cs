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

            await context.SaveChangesAsync();
        }
    }
