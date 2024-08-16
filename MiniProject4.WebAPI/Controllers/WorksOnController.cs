﻿using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniProject4.Domain.Models;
using MiniProject4.Persistence.Models;
using MiniProject4.Persistence.Services;

namespace MiniProject4.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[Controller]")]
    [ApiController]
    /// <summary>
    /// Handles operations related to the assignment of employees to projects, including adding, retrieving, updating, and deleting work assignments.
    /// </summary>
    public class WorksonController : ControllerBase
    {
        private readonly WorksonService _worksonService;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes the WorksonController with the workson service.
        /// </summary>
        /// <param name="worksonService">The service used to manage work assignments.</param>
        public WorksonController(WorksonService worksonService, IConfiguration configuration)
        {
            _worksonService = worksonService;
            _configuration = configuration;
        }

        /// <summary>
        /// Adds a new work assignment to the system.
        /// </summary>
        /// <remarks>
        /// Ensure that the workson data is not null. Convert the date from the object representation to DateOnly format.
        /// 
        /// Sample request:
        ///
        ///     POST /Workson
        ///     {
        ///        "empNo": 1,
        ///        "projNo": 5,
        ///        "dateworked": "2024-08-09",
        ///        "hoursworked": 8
        ///     }
        /// </remarks>
        /// <param name="worksOn">The work assignment to be added.</param>
        /// <returns>Success message if the work assignment is added successfully or an error message if it already exists or if employee/project not found.</returns>
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AddWorkson([FromBody] Workson worksOn)
        {
            if (worksOn == null)
            {
                return BadRequest("Workson data cannot be null.");
            }
            var maxHoursWorked = _configuration.GetValue<int>("WorksonSettings:MaxHoursWorked");
            var maxProject = _configuration.GetValue<int>("WorksonSettings:MaxProject");
            worksOn.ConvertDateWorkedObjectToDateOnly();

            var (success, message) = await _worksonService.AddWorkson(worksOn, maxHoursWorked, maxProject);
            if (!success)
            {
                return BadRequest(message);
            }

            return Ok(message);
        }


        /// <summary>
        /// Retrieves a paginated list of all workson in the system.
        /// </summary>
        /// <remarks>
        /// Provide the page number and page size for pagination.
        ///
        /// Sample request:
        ///
        ///     GET /Workson?pageNumber=1&pageSize=10
        /// </remarks>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of workson to retrieve per page.</param>
        /// <returns>A paginated list of workson.</returns>
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<Workson>>> GetAllWorkson([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }

            var worksons = await _worksonService.GetAllWorkson(pageNumber, pageSize);
            return Ok(worksons);
        }

        /// <summary>
        /// Retrieves the details of a work assignment by employee number and project number.
        /// </summary>
        /// <remarks>
        /// Ensure that both employee number and project number provided are valid.
        ///
        /// Sample request:
        ///
        ///     GET /Workson/1/101
        /// </remarks>
        /// <param name="empNo">The employee number of the work assignment to be retrieved.</param>
        /// <param name="projNo">The project number of the work assignment to be retrieved.</param>
        /// <returns>Work assignment details if found or an error message if not found.</returns>
        [HttpGet("{empNo}/{projNo}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Workson>> GetWorksonById( int empNo, int projNo)
        {
            var worksOn = await _worksonService.GetWorksonById(empNo, projNo);
            if (worksOn == null)
            {
                return NotFound();
            }
            return Ok(worksOn);
        }

        /// <summary>
        /// Updates the details of a work assignment by employee number and project number.
        /// </summary>
        /// <remarks>
        /// Ensure that the workson data is not null.
        ///
        /// Sample request:
        ///
        ///     PUT /Workson/1/101
        ///     {
        ///        "dateworked": "2024-08-10",
        ///        "hoursworked": 9
        ///     }
        /// </remarks>
        /// <param name="empNo">The employee number of the work assignment to be updated.</param>
        /// <param name="projNo">The project number of the work assignment to be updated.</param>
        /// <param name="editWorksOn">The updated work assignment data.</param>
        /// <returns>Success message if the work assignment is updated successfully or an error message if not found or employee/project not found.</returns>
        [HttpPut("{empNo}/{projNo}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateWorkson(int empNo, int projNo, [FromBody] Workson editWorksOn)
        {
            if (editWorksOn == null)
            {
                return BadRequest("Workson data cannot be null.");
            }

            var success = await _worksonService.UpdateWorkson(empNo, projNo, editWorksOn);
            if (!success)
            {
                return NotFound("Workson not found or Employee/Project not found.");
            }

            return Ok("Workson updated successfully.");
        }

        /// <summary>
        /// Deletes a work assignment by employee number and project number.
        /// </summary>
        /// <remarks>
        /// Ensure that both employee number and project number provided are valid.
        ///
        /// Sample request:
        ///
        ///     DELETE /Workson/1/101
        /// </remarks>
        /// <param name="empNo">The employee number of the work assignment to be deleted.</param>
        /// <param name="projNo">The project number of the work assignment to be deleted.</param>
        /// <returns>Success message if the work assignment is deleted successfully or an error message if not found.</returns>
        [HttpDelete("{empNo}/{projNo}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteWorkson(int empNo, int projNo)
        {
            var success = await _worksonService.DeleteWorkson(empNo, projNo);
            if (!success)
            {
                return NotFound("Workson not found.");
            }
            return Ok("Workson deleted successfully.");
        }
    }
}
