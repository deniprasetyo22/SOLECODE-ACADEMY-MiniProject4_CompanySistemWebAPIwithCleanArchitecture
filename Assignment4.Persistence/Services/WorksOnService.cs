using MiniProject4.Application.Interfaces;
using MiniProject4.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniProject4.Persistence.Models;
using Microsoft.Extensions.Configuration;

namespace MiniProject4.Persistence.Services
{
    public class WorksonService : IWorksOn
    {
        private readonly Miniproject4Context _context;

        public WorksonService(Miniproject4Context context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> AddWorkson(Workson worksOn, int maxHoursWorked, int maxProject)
        {
            if (worksOn == null)
            {
                return (false, "Workson data cannot be null.");
            }


            // Validasi apakah Workson sudah ada
            var existingWorkson = await _context.Worksons
                .FirstOrDefaultAsync(w => w.Empno == worksOn.Empno && w.Projno == worksOn.Projno);
            if (existingWorkson != null)
            {
                return (false, "Workson already exists.");
            }

            // Validasi apakah jam kerja melebihi batas yang ditentukan
            if (worksOn.Hoursworked.HasValue && worksOn.Hoursworked.Value > maxHoursWorked)
            {
                return (false, $"Hours worked cannot more than {maxHoursWorked} hours.");
            }

            // Validasi jumlah proyek yang sudah ada untuk karyawan
            var employeeProjectCount = await _context.Worksons
                .CountAsync(w => w.Empno == worksOn.Empno);

            if (employeeProjectCount >= maxProject)
            {
                return (false, $"An employee can be assigned a maximum of {maxProject} projects.");
            }

            // Validasi keberadaan Employee dan Project
            var employeeExists = await _context.Employees.AnyAsync(e => e.Empno == worksOn.Empno);
            var projectExists = await _context.Projects.AnyAsync(p => p.Projno == worksOn.Projno);
            if (!employeeExists || !projectExists)
            {
                return (false, "Employee or Project not found.");
            }

            _context.Worksons.Add(worksOn);
            await _context.SaveChangesAsync();
            return (true, "Workson added successfully.");
        }


        public async Task<IEnumerable<Workson>> GetAllWorkson(int pageNumber, int pageSize)
        {
            return await _context.Worksons
                .OrderBy(w => w.Empno)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Workson?> GetWorksonById(int empNo, int projNo)
        {
            return await _context.Worksons.FirstOrDefaultAsync(w => w.Empno == empNo && w.Projno == projNo);
        }


        public async Task<bool> UpdateWorkson(int empNo, int projNo, Workson editWorksOn)
        {
            var existingWorkson = await _context.Worksons
            .FirstOrDefaultAsync(w => w.Empno == empNo && w.Projno == projNo);
            if (existingWorkson == null)
            {
                return false;
            }

            // Validasi keberadaan Employee dan Project
            var employeeExists = await _context.Employees.AnyAsync(e => e.Empno == editWorksOn.Empno);
            var projectExists = await _context.Projects.AnyAsync(p => p.Projno == editWorksOn.Projno);
            if (!employeeExists || !projectExists)
            {
                return false;
            }

            existingWorkson.Dateworked = editWorksOn.Dateworked;
            existingWorkson.Hoursworked = editWorksOn.Hoursworked;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteWorkson(int empNo, int projNo)
        {
            var existingWorkson = await _context.Worksons.FirstOrDefaultAsync(w => w.Empno == empNo || w.Projno == projNo);
            if (existingWorkson == null)
            {
                return false;
            }

            _context.Worksons.Remove(existingWorkson);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
