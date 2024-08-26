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
    public class WorksonService : IWorksonService
    {
        private readonly IWorksonRepository _worksonRepository;

        public WorksonService(IWorksonRepository worksonRepository)
        {
            _worksonRepository = worksonRepository;
        }

        public async Task<(bool Success, string Message)> AddWorkson(Workson worksOn)
        {
            return await _worksonRepository.AddWorkson(worksOn);
        }


        public async Task<IEnumerable<Workson>> GetAllWorkson(int pageNumber, int pageSize)
        {
            return await _worksonRepository.GetAllWorkson(pageNumber, pageSize);
        }

        public async Task<Workson?> GetWorksonById(int empNo, int projNo)
        {
            return await _worksonRepository.GetWorksonById(empNo, projNo);
        }


        public async Task<bool> UpdateWorkson(int empNo, int projNo, Workson editWorksOn)
        {
            return await _worksonRepository.UpdateWorkson(empNo, projNo, editWorksOn);
        }

        public async Task<bool> DeleteWorkson(int empNo, int projNo)
        {
            return await _worksonRepository.DeleteWorkson(empNo, projNo);
        }
        public async Task<(int maxHours, int minHours)> GetMaxAndMinHoursWorked()
        {
            return await _worksonRepository.GetMaxAndMinHoursWorked();
        }
        public async Task<Dictionary<string, int>> GetTotalHoursWorkedByEmployee()
        {
            return await _worksonRepository.GetTotalHoursWorkedByEmployee();
        }
    }
}
