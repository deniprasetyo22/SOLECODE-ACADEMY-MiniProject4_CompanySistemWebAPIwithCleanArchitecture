using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniProject4.Application.Interfaces.IRepositories;
using MiniProject4.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject4.Persistence.Repositories
{
    public class ProjectRepository:IProjectRepository
    {
        private readonly Miniproject4Context _context;
        private readonly IConfiguration _configuration;
        public ProjectRepository(Miniproject4Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<(bool isSuccess, string message)> AddProject(Project project)
        {
            if (project == null)
            {
                return (false, "Project data cannot be null.");
            }

            // Dapatkan nilai maksimal proyek per departemen dari konfigurasi
            var maxProjectsPerDepartment = int.Parse(_configuration["ProjectSettings:MaxProjectsPerDepartment"]);

            // Check if a project with the same Projno or Projname already exists
            var existingProject = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(cek => cek.Projno == project.Projno || cek.Projname == project.Projname);

            if (existingProject != null)
            {
                return (false, "A project with the same Projno or Projname already exists.");
            }

            // Check if the department can handle the maximum number of projects
            var currentProjectCount = await _context.Projects
                .AsNoTracking()
                .CountAsync(p => p.Deptno == project.Deptno);

            if (currentProjectCount >= maxProjectsPerDepartment)
            {
                return (false, $"The department already has the maximum number of projects ({maxProjectsPerDepartment}).");
            }

            // Ensure the department number is valid
            var existingDepartment = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(dep => dep.Deptno == project.Deptno);

            if (existingDepartment == null)
            {
                return (false, "The specified department does not exist.");
            }

            // Add the project to the database
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            return (true, "Project added successfully.");
        }


        public async Task<IEnumerable<Project>> GetAllProjects(int pageNumber, int pageSize)
        {
            return await _context.Projects
                .OrderBy(p => p.Projno)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<Project> GetProjectById(int projNo)
        {
            var filtered = await _context.Projects.FirstOrDefaultAsync(cek => cek.Projno == projNo);
            return filtered;
        }
        public async Task<bool> UpdateProject(int projNo, Project editProj)
        {
            var existingProject = await _context.Projects
            .FirstOrDefaultAsync(cek => cek.Projno == projNo);

            if (existingProject == null)
            {
                return false; // Project not found
            }

            var duplicateProjectName = await _context.Projects
                .AnyAsync(cek => cek.Projname == editProj.Projname && cek.Projno != projNo);

            if (duplicateProjectName)
            {
                return false; // Project name already exists
            }

            var validDepartment = await _context.Departments
                .AnyAsync(dept => dept.Deptno == editProj.Deptno);

            if (!validDepartment)
            {
                return false; // Invalid department number
            }

            // Check if another project exists with the same Deptno
            var departmentProjectExists = await _context.Projects
                .AnyAsync(p => p.Deptno == editProj.Deptno && p.Projno != projNo);
            if (departmentProjectExists)
            {
                return false; // Deptno already used by another project
            }

            existingProject.Projname = editProj.Projname;
            existingProject.Deptno = editProj.Deptno;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteProject(int projNo)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(cek => cek.Projno == projNo);
            if (project == null)
            {
                return false;
            }
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<Project>> GetProjectsManagedByDept()
        {
            var planningDept = await _context.Departments.FirstOrDefaultAsync(d => d.Deptname == "Planning");
            if (planningDept == null)
            {
                return null;
            }

            return await _context.Projects
                .Where(p => p.Deptno == planningDept.Deptno)
                .Select(p => new Project
                {
                    Projno = p.Projno,
                    Projname = p.Projname,
                    Deptno = p.Deptno
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<Project>> GetProjectsWithNoEmployees()
        {
            var projectsWithNoEmployees = await _context.Projects
                .Where(p => !_context.Worksons
                .Any(w => w.Projno == p.Projno))
                .OrderBy(p => p.Projno)
                .ToListAsync();

            return projectsWithNoEmployees;
        }
        public async Task<IEnumerable<Project>> GetProjectsManagedByITAndHR()
        {
            // deptNo untuk IT adalah 1 dan HR adalah 5
            var itAndHrProjects = await _context.Projects
                .AsNoTracking()
                .Where(p => p.Deptno == 1 || p.Deptno == 5)
                .ToListAsync();

            return itAndHrProjects;
        }
        public async Task<IEnumerable<object>> GetFemaleManagersAndTheirProjects()
        {
            var result = await (from emp in _context.Employees
                                join dept in _context.Departments on emp.Empno equals dept.Mgrempno //menggabungkan tabel Employees dengan Departments berdasarkan Empno sebagai manager (Mgrempno) di departemen.
                                join proj in _context.Projects on dept.Deptno equals proj.Deptno //menggabungkan tabel Departments dengan Projects berdasarkan Deptno.
                                where emp.Sex == "Female" && emp.Position.Contains("Manager")
                                select new
                                {
                                    ManagerName = $"{emp.Fname} {emp.Lname}",
                                    emp.Position,
                                    emp.Sex,
                                    ProjectName = proj.Projname,
                                    proj.Deptno
                                }).ToListAsync();

            return result;
        }
    }
}
