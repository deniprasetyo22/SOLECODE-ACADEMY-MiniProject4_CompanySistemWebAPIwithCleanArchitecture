using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniProject4.Application.Interfaces.IRepositories;
using MiniProject4.Domain.Models;
using MiniProject4.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject4.Persistence.Repositories
{
    public class EmployeeRepository:IEmployeeRepository
    {
        private readonly Miniproject4Context _context;
        private readonly IConfiguration _configuration;
        public EmployeeRepository(Miniproject4Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<(bool isSuccess, string message)> AddEmployee(Employee employee)
        {
            if (employee == null)
            {
                return (false, "Employee data cannot be null.");
            }
            // Dapatkan nilai maksimal karyawan dari konfigurasi
            var maxEmployees = int.Parse(_configuration["EmployeeSettings:MaxEmployees"]);

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
            return (true, "Employee added successfully.");
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

        public async Task<List<object>> GetEmployeesWhoAreNeitherManagersNorSupervisors()
        {
            var emp = from value in _context.Employees
                      join dept in _context.Departments on value.Empno equals dept.Mgrempno
                      select value;

            var result = await _context.Employees
                .Except(emp)
                .Where(w => !w.Position.Contains("Supervisor") && !w.Position.Contains("Manager"))
                .Select(s => new
                {
                    FirstName = s.Fname,
                    LastName = s.Lname,
                    Position = s.Position,
                    Sex = s.Sex,
                    Deptno = s.Deptno,
                })
                .ToListAsync<object>();
            return result;
        }

        public async Task<IEnumerable<object>> GetEmployeesWhoShouldRetire()
        {
            var retirementAge = int.Parse(_configuration["EmployeeSettings:RetirementAge"]);
            var today = DateOnly.FromDateTime(DateTime.Now);
            var retirees = await _context.Employees
                .Where(e => today.Year - e.Dob.Year >= retirementAge)
                .Select(e => new
                {
                    Empno = e.Empno,
                    FirstName = e.Fname,
                    LastName = e.Lname,
                    Position = e.Position,
                    Sex = e.Sex,
                    Deptno = e.Deptno,
                    Dob = e.Dob,
                    Age = today.Year - e.Dob.Year
                })
                .ToListAsync();

            return retirees;
        }
        public async Task<IEnumerable<object>> GetEmployeesInITDepartment()
        {
            return await _context.Employees
                .Where(e => e.Deptno == 1)
                .Select(e => new
                {
                    Fname = e.Fname,
                    Lname = e.Lname,
                    Address = e.Address
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetManagersDueToRetireThisYear()
        {
            var retirementAge = int.Parse(_configuration["EmployeeSettings:RetirementAge"]);
            var emp = await _context.Employees
                .Where(w => DateOnly.FromDateTime(DateTime.Now).Year - w.Dob.Year >= retirementAge && w.Position.Contains("Manager"))
                .Select(s => new
                {
                    Fname = s.Fname,
                    Lname = s.Lname,
                    Position = s.Position,
                    Address = s.Address,
                    Age = DateOnly.FromDateTime(DateTime.Now).Year - s.Dob.Year
                })
                .ToListAsync<object>();
            return emp;
        }

        public async Task<int> GetNumberOfFemaleManagers()
        {
            var count = await _context.Employees
            .AsNoTracking()
            .Where(e => e.Position.Contains("Manager") && e.Sex == "Female")
            .CountAsync();
            return count;
        }

        public async Task<IEnumerable<object>> GetEmployeeAgeInfo()
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var employeeData = await _context.Employees
                .AsNoTracking()
                .Select(e => new
                {
                    FullName = $"{e.Fname} {e.Lname}",
                    Deptno = e.Deptno,
                    Age = currentDate.Year - e.Dob.Year - (currentDate < e.Dob.AddYears(currentDate.Year - e.Dob.Year) ? 1 : 0)
                })
                .ToListAsync();

            return employeeData.Cast<object>(); // Cast<object>() digunakan untuk mengkonversi setiap elemen dalam koleksi ke tipe object, sehingga koleksi bisa dikembalikan dengan tipe yang sesuai.
        }
        public async Task<IEnumerable<object>> GetTotalHoursWorkedByEmployee()
        {
            var totalHoursData = await (from w in _context.Worksons
                                        join e in _context.Employees
                                        on w.Empno equals e.Empno
                                        join p in _context.Projects
                                        on w.Projno equals p.Projno
                                        group new { e.Fname, e.Lname, p.Projname, w.Hoursworked }
                                        by new { e.Fname, e.Lname, p.Projname } into g
                                        select new
                                        {
                                            FullName = $"{g.Key.Fname} {g.Key.Lname}",
                                            Projname = g.Key.Projname,
                                            TotalHours = g.Sum(x => x.Hoursworked ?? 0)
                                        }).ToListAsync();

            return totalHoursData.Cast<object>();
        }
        public async Task<IEnumerable<object>> GetManagersUnder40()
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var managersUnder40 = await (from e in _context.Employees
                                         where e.Position.Contains("Manager")
                                         let age = currentDate.Year - e.Dob.Year - (currentDate < new DateOnly(e.Dob.Year, e.Dob.Month, e.Dob.Day) ? 1 : 0)
                                         where age < 40
                                         select new
                                         {
                                             FullName = $"{e.Fname} {e.Lname}",
                                             Position = e.Position,
                                             Dob = e.Dob,
                                             Age = age
                                         }).ToListAsync();

            return managersUnder40.Cast<object>();
        }
    }
}
