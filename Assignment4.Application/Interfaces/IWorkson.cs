using MiniProject4.Domain.Models;
using MiniProject4.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject4.Application.Interfaces
{
    public interface IWorksOn
    {
        Task<IEnumerable<Workson>> GetAllWorkson(int pageNumber, int pageSize);
        Task<Workson?> GetWorksonById(int empNo, int projNo);
        Task<(bool Success, string Message)> AddWorkson(Workson worksOn, int maxHoursWorked, int maxProject);
        Task<bool> UpdateWorkson(int empNo, int projNo, Workson editWorksOn);
        Task<bool> DeleteWorkson(int empNo, int projNo);
    }
}
