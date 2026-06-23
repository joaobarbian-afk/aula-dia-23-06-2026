using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BancadDigitalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BancadDigitalAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<RegistroBancada> Historico { get; set; }

    }
}