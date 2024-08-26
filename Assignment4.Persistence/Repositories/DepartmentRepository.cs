using Microsoft.EntityFrameworkCore;
using MiniProject4.Application.Interfaces.IRepositories;
using MiniProject4.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject4.Persistence.Repositories
{
    public class DepartmentRepository:IDepartmentRepository
    {
        private readonly Miniproject4Context _context;
        public DepartmentRepository(Miniproject4Context context)
        {
            _context = context;
        }
        public async Task<bool> AddDepartment(Department department)
        {
            // Validasi apakah Mgrempno ada di database
            var managerExists = await _context.Employees.AnyAsync(e => e.Empno == department.Mgrempno);
            if (!managerExists)
            {
                return false;
            }

            var existingDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.Deptname == department.Deptname || d.Mgrempno == department.Mgrempno);
            if (existingDepartment != null)
            {
                return false;
            }

            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<Department>> GetAllDepartments(int pageNumber, int pageSize)
        {
            return await _context.Departments
                .OrderBy(d => d.Deptno)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<Department> GetDepartmentById(int deptNo)
        {
            if (deptNo <= 0)
            {
                throw new ArgumentException("Invalid department number.", nameof(deptNo));
            }
            var filtered = await _context.Departments.FirstOrDefaultAsync(cek => cek.Deptno == deptNo);
            return filtered;
        }
        public async Task<bool> UpdateDepartment(int deptNo, Department editDept)
        {
            if (deptNo <= 0 || editDept == null)
            {
                throw new ArgumentException("Invalid department number or data.");
            }
            var existingDepartment = await _context.Departments.FirstOrDefaultAsync(cek => cek.Deptno == deptNo);
            if (existingDepartment == null)
            {
                return false;
            }
            // Cek apakah Mgrempno sudah ada di database dan berbeda dari yang sedang diupdate
            var managerExists = await _context.Departments.AnyAsync(cek => cek.Mgrempno == editDept.Mgrempno && cek.Deptno != deptNo);

            if (managerExists)
            {
                // Jika Mgrempno sudah ada dan berbeda dari deptNo yang sedang diupdate, return false
                return false;
            }
            existingDepartment.Deptname = editDept.Deptname;
            existingDepartment.Mgrempno = editDept.Mgrempno;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteDepartment(int deptNo)
        {
            if (deptNo <= 0)
            {
                throw new ArgumentException("Invalid department number.", nameof(deptNo));
            }
            var department = await _context.Departments.FirstOrDefaultAsync(cek => cek.Deptno == deptNo);
            if (department == null)
            {
                return false;
            }
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
