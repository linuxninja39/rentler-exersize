using Microsoft.EntityFrameworkCore;
using Rentler.Interview.Api.Entities;

namespace Rentler.Interview.Api.Configuration;

public class FoodContext : DbContext
{
    public DbSet<Foods> Foods { get; set; }

    public FoodContext(DbContextOptions<FoodContext> options) : base(options)
    {
    }
}