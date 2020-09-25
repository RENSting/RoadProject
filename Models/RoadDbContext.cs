using System;
using Microsoft.EntityFrameworkCore;

namespace RoadProject.Models
{
    public class RoadDbContext:DbContext
    {
        public DbSet<Project> Project{get;set;}

        public DbSet<Category> Category{get;set;}

        public DbSet<Document> Document{get;set;}

        public DbSet<Log> Log{get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=data/road.db");
        }
    }
}