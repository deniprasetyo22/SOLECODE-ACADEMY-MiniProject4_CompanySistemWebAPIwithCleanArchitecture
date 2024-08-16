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
    public class EmployeeService : IEmployee
    {
        private readonly Miniproject4Context _context;
        public EmployeeService(Miniproject4Context context)
        {
            _context = context;
        }
        public async Task<(bool isSuccess, string message)> AddEmployee(Employee employee, int maxEmployees)
        {
            if (employee == null)
            {
                return (false, "Employee data cannot be null.");
            }

            // Check if employee with the same Empno already exists
            var existingEmployee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(cek => cek.Empno == employee.Empno);
            if (existingEmployee != null)
            {
                return (false, "Employee with the same Empno already exists.");
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(employee.Fname))
            {
                return (false, "First name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(employee.Lname))
            {
                return (false, "Last name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(employee.Position))
            {
                return (false, "Position cannot be empty.");
            }

            if (employee.Deptno <= 0)
            {
                return (false, "Invalid department number.");
            }

            // Check if the department exists
            var existingDepartment = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(dep => dep.Deptno == employee.Deptno);
            if (existingDepartment == null)
            {
                return (false, "The specified department does not exist.");
            }

            // Validate the maximum number of employees in the specified department
            if (employee.Deptno == 1) // Assuming 1 is the department number for IT
            {
                var deptEmployeeCount = await _context.Employees
                    .AsNoTracking()
                    .CountAsync(e => e.Deptno == employee.Deptno);

                if (deptEmployeeCount >= maxEmployees)
                {
                    return (false, $"The department already has the maximum number of employees {maxEmployees}.");
                }
            }

            // Add the new employee
            await _context.Employees.AddAsync(employee);
            var changes = await _context.SaveChangesAsync();

            if (changes > 0)
            {
                return (true, "Employee added successfully.");
            }

            return (false, "Failed to add employee. No changes were made.");
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees(int pageNumber, int pageSize)
        {
            return await _context.Employees
                .OrderBy(a => a.Empno)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<Employee> GetEmployeeById(int empNo)
        {
            var filtered = await _context.Employees.FirstOrDefaultAsync(cek => cek.Empno == empNo);
            return filtered;
        }
        public async Task<bool> UpdateEmployee(int empNo, Employee editEmp)
        {
            var existingEmployee = await _context.Employees.FirstOrDefaultAsync(cek => cek.Empno == empNo);
            if (existingEmployee == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(editEmp.Fname) || string.IsNullOrEmpty(editEmp.Lname) ||
                string.IsNullOrEmpty(editEmp.Position) || editEmp.Deptno == 0)
            {
                return false;
            }

            var existingDepartment = await _context.Departments.FirstOrDefaultAsync(dep => dep.Deptno == editEmp.Deptno);
            if (existingDepartment == null)
            {
                return false;
            }
            existingEmployee.Fname = editEmp.Fname;
            existingEmployee.Lname = editEmp.Lname;
            existingEmployee.Address = editEmp.Address;
            existingEmployee.Dob = editEmp.Dob;
            existingEmployee.Sex = editEmp.Sex;
            existingEmployee.Position = editEmp.Position;
            existingEmployee.Deptno = editEmp.Deptno;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteEmployee(int empNo)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(cek => cek.Empno == empNo);
            if (employee == null)
            {
                return false;
            }
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<Employee>> GetEmployeesFromBRICS()
        {
            var bricsCountries = new List<string> { "Brazil", "Russia", "India", "China", "South Africa" };
            var employeesFromBRICS = await _context.Employees
                .Where(cek => bricsCountries
                .Any(country => cek.Address.Contains(country)))
                .OrderBy(a => a.Lname)
                .ThenBy(a => a.Fname)
                .ToListAsync();
            return employeesFromBRICS;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesBornBetween1980And1990()
        {
            var employeesBornBetween1980And1990 = await _context.Employees
                .Where(cek => cek.Dob >= new DateOnly(1980, 1, 1) && cek.Dob <= new DateOnly(1990, 12, 31))
                .OrderBy(a => a.Empno)
                .ToListAsync();
            return employeesBornBetween1980And1990;
        }
        public async Task<IEnumerable<Employee>> GetFemaleEmployeesBornAfter1990()
        {
            var femaleEmployeesBornAfter1990 = await _context.Employees
                .Where(cek => cek.Sex == "Female" && cek.Dob > new DateOnly(1990, 12, 31))
                .ToListAsync();

            return femaleEmployeesBornAfter1990;
        }
        public async Task<IEnumerable<Employee>> GetFemaleManagers()
        {
            var femaleManagers = await _context.Employees
                .Where(cek => cek.Sex == "Female" && _context.Departments.Any(a => a.Mgrempno == cek.Empno))
                .OrderBy(b => b.Lname)
                .ThenBy(c => c.Fname)
                .ToListAsync();

            return femaleManagers;
        }
        public async Task<IEnumerable<Employee>> GetEmployeesNotManagers()
        {
            var employeesNotManagers = await _context.Employees
                .Where(cek => !_context.Departments.Any(a => a.Mgrempno == cek.Empno))
                .ToListAsync();

            return employeesNotManagers;
        }
        public async Task<int> GetNumberOfFemaleManagers()
        {
            var count = await _context.Employees
                .Where(e => e.Position == "HR Manager" && e.Sex == "Female")
                .CountAsync();

            return count;
        }

        public async Task<IEnumerable<DepartmentEmployeeCount>> GetDepartmentsWithMoreThanTenEmployees()
        {
            var departmentEmployeeCounts = await _context.Employees
                .GroupBy(e => e.Deptno)
                .Select(g => new
                {
                    Deptno = g.Key,
                    EmployeeCount = g.Count()
                })
                .Where(dc => dc.EmployeeCount > 10)
                .Join(_context.Departments,
                    dc => dc.Deptno,
                    d => d.Deptno,
                    (dc, d) => new DepartmentEmployeeCount
                    {
                        Deptno = d.Deptno,
                        Deptname = d.Deptname,
                        EmployeeCount = dc.EmployeeCount
                    })
                .ToListAsync();

            return departmentEmployeeCounts;
        }
        

    }
}
