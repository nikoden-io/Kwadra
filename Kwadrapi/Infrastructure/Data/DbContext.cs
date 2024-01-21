using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DomainContext : DbContext
{
    public DomainContext(DbContextOptions<DomainContext> options) : base(options)
    {
    }


    public required DbSet<DemoSensor> DemoSensors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}