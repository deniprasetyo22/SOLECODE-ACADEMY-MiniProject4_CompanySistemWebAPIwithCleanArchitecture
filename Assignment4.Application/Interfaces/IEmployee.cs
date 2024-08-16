using MiniProject4.Domain.Models;
using MiniProject4.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject4.Application.Interfaces
{
    public interface IEmployee
    {
        Task<(bool isSuccess, string message)> AddEmployee(Employee employee, int maxEmployees);
        Task<IEnumerable<Employee>> GetAllEmployees(int pageNumber, int pageSize);
        Task<Employee> GetEmployeeById(int empNo);
        Task<bool> UpdateEmployee(int empNo, Employee editEmp);
        Task<bool> DeleteEmployee(int empNo);
        Task<IEnumerable<Employee>> GetEmployeesFromBRICS();
        Task<IEnumerable<Employee>> GetEmployeesBornBetween1980And1990();
        Task<IEnumerable<Employee>> GetFemaleEmployeesBornAfter1990();
        Task<IEnumerable<Employee>> GetFemaleManagers();
        Task<IEnumerable<Employee>> GetEmployeesNotManagers();
        Task<IEnumerable<DepartmentEmployeeCount>> GetDepartmentsWithMoreThanTenEmployees();

    }
}
