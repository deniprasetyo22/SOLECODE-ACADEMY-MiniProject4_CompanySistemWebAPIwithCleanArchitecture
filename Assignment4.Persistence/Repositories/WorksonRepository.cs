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
    public class WorksonRepository:IWorksonRepository
    {
        private readonly Miniproject4Context _context;
        private readonly IConfiguration _configuration;

        public WorksonRepository(Miniproject4Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message)> AddWorkson(Workson worksOn)
        {
            var maxHoursWorked = int.Parse(_configuration["WorksonSettings:MaxHoursWorked"]);
            var maxProject = int.Parse(_configuration["WorksonSettings:MaxProject"]);
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
        public async Task<(int maxHours, int minHours)> GetMaxAndMinHoursWorked()
        {
            // Mengecek apakah ada data di tabel Worksons
            if (!await _context.Worksons.AnyAsync())
            {
                // Jika tidak ada data, kembalikan nilai 0 untuk maxHours dan minHours
                return (0, 0);
            }

            // Mengambil nilai maksimum dan minimum jam kerja
            var maxHours = await _context.Worksons
                .Where(w => w.Hoursworked.HasValue)
                .MaxAsync(w => w.Hoursworked.Value);

            var minHours = await _context.Worksons
                .Where(w => w.Hoursworked.HasValue)
                .MinAsync(w => w.Hoursworked.Value);

            return (maxHours, minHours);
        }
        public async Task<Dictionary<string, int>> GetTotalHoursWorkedByEmployee()
        {
            var totalHours = await (from w in _context.Worksons
                                    join e in _context.Employees on w.Empno equals e.Empno
                                    group w by new { e.Fname, e.Lname } into g
                                    select new
                                    {
                                        EmployeeName = $"{g.Key.Fname} {g.Key.Lname}",
                                        TotalHours = g.Sum(w => w.Hoursworked ?? 0) //operator null-coalescing, w.Hoursworked ?? 0 berarti: jika w.Hoursworked memiliki nilai (tidak null), maka ambil nilainya. Jika w.Hoursworked adalah null, maka gunakan nilai default 0
                                    })
                                    .ToDictionaryAsync(x => x.EmployeeName, x => x.TotalHours);

            return totalHours;
        }
    }
}
