
using BulkyBookWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.Data
{
    // We need to add Entity Framework Core
    // The class here is inheriting from EFC
    public class ApplicationDbContext :DbContext
    {
        // in this contructor we are establishing the connection
        // when we get the Applicatio Database Context we have to pass that to the Base Class (DbContext) provided by Entity Framwork Core
        // configuring the database context options on the ApplicationDbContext class
        // what we are doing here is saying that we will parameterize (recieve) some options here
        // and those options we are just passing to the 'base' class as an argument
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
        {
        }

        // whatever Models we create inside the database we will have to create a DbSet
        // what this will do is create a database table with the name 'Categories' corresponding to the Category Model Type and its properties
        // when working with EFC there are two approaches: Code first & Database first.
        // here we are using the code first approach because we are creating our models first and then making a database table from that
        public DbSet<Category> Categories { get; set; }

        // we have to tell our application that we have to use the DbContext inside ApplicatioDbContext inside the Program.cs file
        // using SQL Server and using the connection string defined in the appsettings.json

        // next we have actually create the database and the table:
        // When using the code first approach in EFC you have to first use Migrations to push changes to the database and create it
        // in the Package Manager Console we use the command "add-migration <migration name>" then "update-database" if all goes well


    }
}
