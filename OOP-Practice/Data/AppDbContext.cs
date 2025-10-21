using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OOP_Practice.Models;

namespace OOP_Practice.Data;

public class AppDbContext : DbContext
{
    public AppDbContext (DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Product { get; set; }
}
