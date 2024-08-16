using MiniProject4.Application.Interfaces;
using MiniProject4.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniProject4.Persistence.Models;

namespace MiniProject4.Persistence.Services
{
    public class ProjectService : IProject
    {
        private Miniproject4Context _context;
        public ProjectService(Miniproject4Context context)
        {
            _context = context;
        }
        // Method untuk menghasilkan projNo berikutnya
        private async Task<int> GenerateNextProjNo()
        {
            var lastProjNo = await _context.Projects
                .OrderByDescending(e => e.Projno)
                .Select(e => e.Projno)
                .FirstOrDefaultAsync();

            // Mulai dari 1 jika belum ada projek
            return lastProjNo + 1;
        }
        public async Task<bool> AddProject(Project project)
        {
            // Generate Projno otomatis
            if (project.Projno == 0)
            {
                project.Projno = await GenerateNextProjNo();
            }

            var existingProject = await _context.Projects
            .FirstOrDefaultAsync(cek => cek.Projno == project.Projno ||
                                         cek.Projname == project.Projname ||
                                         cek.Deptno == project.Deptno);

            if (existingProject != null)
            {
                return false;
            }

            // Check if any other project already exists with the same Deptno
            var departmentProjectExists = await _context.Projects
                .AnyAsync(p => p.Deptno == project.Deptno);
            if (departmentProjectExists)
            {
                return false;
            }

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
            return true;
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
            var financeDept = await _context.Departments.FirstOrDefaultAsync(d => d.Deptname == "Planning");
            if (financeDept == null)
            {
                return Enumerable.Empty<Project>();
            }

            return await _context.Projects
                .Where(p => p.Deptno == financeDept.Deptno)
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
    }
}
