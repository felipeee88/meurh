using Microsoft.EntityFrameworkCore;
using MeuRh.Domain.Entities;
using MeuRh.Infra.Data.Configurations;

namespace MeuRh.Infra.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserMap());
    }
}

