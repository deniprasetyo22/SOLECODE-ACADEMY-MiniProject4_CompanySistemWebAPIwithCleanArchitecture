using MiniProject4.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject4.Application.Interfaces.IRepositories
{
    public interface IDepartmentRepository
    {
        Task<bool> AddDepartment(Department department);
        Task<IEnumerable<Department>> GetAllDepartments(int pageNumber, int pageSize);
        Task<Department> GetDepartmentById(int dptNo);
        Task<bool> UpdateDepartment(int deptNo, Department editDept);
        Task<bool> DeleteDepartment(int deptNo);
    }
}
