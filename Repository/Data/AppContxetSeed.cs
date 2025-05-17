using Core.Entites;
using Core.IRepositories;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Repository.Data
{
    public static class AppContxetSeed
    {
        public static async Task SeedAsync(AppDbContext dbContext , RoleManager<IdentityRole> roleManager)
        {
            if (!dbContext.Roles.Any())
            {
                var RolesString = File.ReadAllText("../Repository/Data/DataSeed/roles.json");
                var Roles = JsonSerializer.Deserialize<List<string>>(RolesString);
                if (Roles?.Count() > 0)
                {
                    foreach (var Role in Roles)
                        await roleManager.CreateAsync(new IdentityRole(Role));
                    await dbContext.SaveChangesAsync();
                }
            }
            if (!dbContext.Categories.Any())
            {
                var CategoriesString = File.ReadAllText("../Repository/Data/DataSeed/category.json");
                var Categories = JsonSerializer.Deserialize<List<Category>>(CategoriesString);
                if (Categories?.Count() > 0)
                {
                    foreach (var Category in Categories)
                        await dbContext.Set<Category>().AddAsync(Category);
                    await dbContext.SaveChangesAsync();
                }
            }
            if (!dbContext.Tags.Any())
            {
                var TagsString = File.ReadAllText("../Repository/Data/DataSeed/tag.json");
                var Tags = JsonSerializer.Deserialize<List<Tag>>(TagsString);
                if (Tags?.Count() > 0)
                {
                    foreach (var Tag in Tags)
                        await dbContext.Set<Tag>().AddAsync(Tag);
                    await dbContext.SaveChangesAsync();
                }
            }
            if(!dbContext.Events.Any())
            {
                var EventString = File.ReadAllText("../Repository/Data/DataSeed/events.json");
                var Events = JsonSerializer.Deserialize<List<Event>>(EventString);
                if(Events?.Count() > 0 )
                {
                    foreach(var Event in Events) 
                        await dbContext.Set<Event>().AddAsync(Event);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
