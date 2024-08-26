using MiniProject4.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject4.Application.Interfaces.IRepositories
{
    public interface IProjectRepository
    {
        Task<(bool isSuccess, string message)> AddProject(Project project);
        Task<IEnumerable<Project>> GetAllProjects(int pageNumber, int pageSize);
        Task<Project> GetProjectById(int projNo);
        Task<bool> UpdateProject(int projNo, Project editProj);
        Task<bool> DeleteProject(int projNo);
        Task<IEnumerable<Project>> GetProjectsManagedByDept();
        Task<IEnumerable<Project>> GetProjectsWithNoEmployees();
        Task<IEnumerable<Project>> GetProjectsManagedByITAndHR();
        Task<IEnumerable<object>> GetFemaleManagersAndTheirProjects();
    }
}
