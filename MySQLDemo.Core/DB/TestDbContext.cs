using Microsoft.EntityFrameworkCore;
using MySQLDemo.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLDemo.Core.DB
{
    public class TestDbContext: DbContext
    {
        public DbSet<User> User { get; set; }
       
        public TestDbContext(DbContextOptions<TestDbContext> options):base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "Data Source=192.168.2.145;port=3306;Initial Catalog=ITest;user id=web;password=bCBJU1ZA;Character Set=utf8mb4;Allow User Variables=True");
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseMySql(
        //        "Data Source=192.168.2.145;port=3306;Initial Catalog=ITest;user id=web;password=bCBJU1ZA;Character Set=utf8mb4;Allow User Variables=True");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex(u => u.Name).IsUnique();
        }
    }
}
