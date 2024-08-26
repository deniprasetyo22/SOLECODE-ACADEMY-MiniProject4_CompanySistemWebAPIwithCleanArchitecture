using MiniProject4.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniProject4.Persistence.Models;
using MiniProject4.Application.Interfaces.IServices;
using MiniProject4.Application.Interfaces.IRepositories;

namespace MiniProject4.Persistence.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }
        public async Task<bool> AddDepartment(Department department)
        {
            return await _departmentRepository.AddDepartment(department);
        }
        public async Task<IEnumerable<Department>> GetAllDepartments(int pageNumber, int pageSize)
        {
            return await _departmentRepository.GetAllDepartments(pageNumber, pageSize);
        }
        public async Task<Department> GetDepartmentById(int deptNo)
        {
            return await _departmentRepository.GetDepartmentById(deptNo);
        }
        public async Task<bool> UpdateDepartment(int deptNo, Department editDept)
        {
            return await _departmentRepository.UpdateDepartment(deptNo, editDept);
        }
        public async Task<bool> DeleteDepartment(int deptNo)
        {
            return await _departmentRepository.DeleteDepartment(deptNo);
        }
    }

}
