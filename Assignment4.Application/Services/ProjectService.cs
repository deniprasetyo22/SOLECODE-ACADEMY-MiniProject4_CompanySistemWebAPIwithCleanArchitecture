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
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task<(bool isSuccess, string message)> AddProject(Project project)
        {
            return await _projectRepository.AddProject(project);
        }

        public async Task<IEnumerable<Project>> GetAllProjects(int pageNumber, int pageSize)
        {
            return await _projectRepository.GetAllProjects(pageNumber, pageSize);
        }
        public async Task<Project> GetProjectById(int projNo)
        {
            return await _projectRepository.GetProjectById(projNo);
        }
        public async Task<bool> UpdateProject(int projNo, Project editProj)
        {
            return await _projectRepository.UpdateProject(projNo, editProj);
        }
        public async Task<bool> DeleteProject(int projNo)
        {
            return await _projectRepository.DeleteProject(projNo);
        }
        public async Task<IEnumerable<Project>> GetProjectsManagedByDept()
        {
            return await _projectRepository.GetProjectsManagedByDept();
        }
        public async Task<IEnumerable<Project>> GetProjectsWithNoEmployees()
        {
            return await _projectRepository.GetProjectsWithNoEmployees();
        }
        public async Task<IEnumerable<Project>> GetProjectsManagedByITAndHR()
        {
            return await _projectRepository.GetProjectsManagedByITAndHR();
        }
        public async Task<IEnumerable<object>> GetFemaleManagersAndTheirProjects()
        {
            return await _projectRepository.GetFemaleManagersAndTheirProjects();
        }
    }
}
