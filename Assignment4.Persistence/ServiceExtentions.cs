using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniProject4.Application.Interfaces.IRepositories;
using MiniProject4.Application.Interfaces.IServices;
using MiniProject4.Persistence.Models;
using MiniProject4.Persistence.Repositories;
using MiniProject4.Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject5.Persistence
{
    public static class ServiceExtentions
    {
        public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<Miniproject4Context>(options => options.UseNpgsql(connectionString));
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IWorksonRepository, WorksonRepository>();
            services.AddScoped<IWorksonService, WorksonService>();
        }
    }
}
