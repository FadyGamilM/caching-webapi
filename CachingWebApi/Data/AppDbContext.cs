using Microsoft.EntityFrameworkCore;
using CachingApi.Models;

namespace CachingApi.Data;

public class AppDbContext : DbContext
{
   public AppDbContext(DbContextOptions<AppDbContext> options) 
      : base(options)
   {

   }
   public DbSet<Student> Students { get; set; }
}