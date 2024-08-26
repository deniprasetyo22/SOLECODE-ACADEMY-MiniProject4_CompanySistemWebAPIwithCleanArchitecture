using MiniProject4.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniProject4.Persistence.Models;
using Microsoft.Extensions.Configuration;
using MiniProject4.Application.Interfaces.IServices;
using MiniProject4.Application.Interfaces.IRepositories;


namespace MiniProject4.Persistence.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public async Task<(bool isSuccess, string message)> AddEmployee(Employee employee)
        {
           return await _employeeRepository.AddEmployee(employee);
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees(int pageNumber, int pageSize)
        {
            return await _employeeRepository.GetAllEmployees(pageNumber, pageSize);
        }
        public async Task<Employee> GetEmployeeById(int empNo)
        {
            return await _employeeRepository.GetEmployeeById(empNo);
        }
        public async Task<bool> UpdateEmployee(int empNo, Employee editEmp)
        {
            return await _employeeRepository.UpdateEmployee(empNo, editEmp);
        }
        public async Task<bool> DeleteEmployee(int empNo)
        {
            return await _employeeRepository.DeleteEmployee(empNo);
        }
        public async Task<IEnumerable<Employee>> GetEmployeesFromBRICS()
        {
            return await _employeeRepository.GetEmployeesFromBRICS();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesBornBetween1980And1990()
        {
            return await _employeeRepository.GetEmployeesBornBetween1980And1990();
        }
        public async Task<IEnumerable<Employee>> GetFemaleEmployeesBornAfter1990()
        {
            return await _employeeRepository.GetFemaleEmployeesBornAfter1990();
        }
        public async Task<IEnumerable<Employee>> GetFemaleManagers()
        {
            return await _employeeRepository.GetFemaleManagers();
        }
        public async Task<IEnumerable<Employee>> GetEmployeesNotManagers()
        {
            return await _employeeRepository.GetEmployeesNotManagers();
        }

        public async Task<IEnumerable<DepartmentEmployeeCount>> GetDepartmentsWithMoreThanTenEmployees()
        {
            return await _employeeRepository.GetDepartmentsWithMoreThanTenEmployees();
        }

        public async Task<List<object>> GetEmployeesWhoAreNeitherManagersNorSupervisors()
        {
            return await _employeeRepository.GetEmployeesWhoAreNeitherManagersNorSupervisors();
        }

        public async Task<IEnumerable<object>> GetEmployeesWhoShouldRetire()
        {
            return await _employeeRepository.GetEmployeesWhoShouldRetire();
        }
        public async Task<IEnumerable<object>> GetEmployeesInITDepartment()
        {
            return await _employeeRepository.GetEmployeesInITDepartment();
        }

        public async Task<IEnumerable<object>> GetManagersDueToRetireThisYear()
        {
            return await _employeeRepository.GetManagersDueToRetireThisYear();
        }

        public async Task<int> GetNumberOfFemaleManagers()
        {
            return await _employeeRepository.GetNumberOfFemaleManagers();
        }

        public async Task<IEnumerable<object>> GetEmployeeAgeInfo()
        {
           return await _employeeRepository.GetEmployeeAgeInfo();
        }
        public async Task<IEnumerable<object>> GetTotalHoursWorkedByEmployee()
        {
            return await _employeeRepository.GetTotalHoursWorkedByEmployee();
        }
        public async Task<IEnumerable<object>> GetManagersUnder40()
        {
            return await _employeeRepository.GetManagersUnder40();
        }
    }
}
