﻿using DataModel.Models;
using Microsoft.EntityFrameworkCore;

namespace DataModel
{
    // Add migration
    // dotnet ef migrations add <MIGRATION NAME> -s ../Src

    // Update database with latest migration(s)
    // dotnet ef database update -s ../Src

    // Remove latest migration
    // dotnet ef migrations remove -s ../Src

    // Revert the database to a migration
    // dotnet ef database update <MIGRATION NAME> -s ../Src

    // Generate SQL script
    // dotnet ef migrations script -s ../Src

    // -s ../Src is used to point to appsettings in Src application. 
    // Remember to set environment eg. 'setx ASPNETCORE_ENVIRONMENT Development'

    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        { }

        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
        }
    }
}
