using Asp.Versioning;
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
    /// Handles operations related to projects, including adding, retrieving, updating, and deleting projects.
    /// </summary>
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectService;
        private readonly DepartmentService _departmentService;

        /// <summary>
        /// Initializes the ProjectController with the project and department services.
        /// </summary>
        /// <param name="projectService">The service used to manage projects.</param>
        /// <param name="departmentService">The service used to manage departments.</param>
        public ProjectController(ProjectService projectService, DepartmentService departmentService)
        {
            _projectService = projectService;
            _departmentService = departmentService;
        }

        /// <summary>
        /// Adds a new project to the system.
        /// </summary>
        /// <remarks>
        /// Ensure that the project data is not null, and that the project name and department number are provided.
        /// Validate that the department exists and that there is no existing project with the same name or department number.
        ///
        /// Sample request:
        ///
        ///     POST /Project
        ///     {
        ///        "projname": "New Project",
        ///        "deptno": 1
        ///     }
        /// </remarks>
        /// <param name="project">The project to be added.</param>
        /// <returns>Success message if the project is added successfully or an error message if validation fails.</returns>
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Project>> AddProject([FromBody] Project project)
        {
            if (project == null)
            {
                return BadRequest("Project data cannot be null.");
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(project.Projname) || project.Deptno == 0)
            {
                return BadRequest("Project Name and Department Number are required.");
            }

            // Validate department exists
            var departmentExists = await _departmentService.GetDepartmentById(project.Deptno);
            if (departmentExists == null)
            {
                return BadRequest("Department Number does not exist.");
            }

            // Check for existing project with the same name or department number
            var success = await _projectService.AddProject(project);
            if (!success)
            {
                return BadRequest("Project Name or Department Number already exists or Department Number is already used by another project.");
            }

            return Ok("Project Data Successfully Added.");
        }

        /// <summary>
        /// Retrieves a paginated list of all projects in the system.
        /// </summary>
        /// <remarks>
        /// Provide the page number and page size for pagination.
        ///
        /// Sample request:
        ///
        ///     GET /Project/page/1/size/10
        /// </remarks>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of projects to retrieve per page.</param>
        /// <returns>A paginated list of projects.</returns>
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<Project>>> GetAllProjects([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var project = await _projectService.GetAllProjects(pageNumber, pageSize);
            return Ok(project);
        }

        /// <summary>
        /// Retrieves the details of a project by its number.
        /// </summary>
        /// <remarks>
        /// Ensure that the project number provided is valid.
        ///
        /// Sample request:
        ///
        ///     GET /Project/1
        /// </remarks>
        /// <param name="projNo">The project number of the project to be retrieved.</param>
        /// <returns>Project details if found or an error message if the project is not found.</returns>
        [HttpGet("{projNo}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Project>> GetProjectById(int projNo)
        {
            var project = await _projectService.GetProjectById(projNo);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        /// <summary>
        /// Updates the details of a project by its number.
        /// </summary>
        /// <remarks>
        /// Ensure that the project data is not null, and that the project name and department number are provided.
        /// Validate that the department exists and that the updated project name and department number are valid.
        ///
        /// Sample request:
        ///
        ///     PUT /Project/1
        ///     {
        ///        "projname": "Updated Project",
        ///        "deptno": 2
        ///     }
        /// </remarks>
        /// <param name="projNo">The project number of the project to be updated.</param>
        /// <param name="editProj">The updated project data.</param>
        /// <returns>Success message if the project is updated successfully or an error message if validation fails.</returns>
        [HttpPut("{projNo}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateProject(int projNo, [FromBody] Project editProj)
        {
            if (editProj == null)
            {
                return BadRequest("Project data cannot be null.");
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(editProj.Projname) || editProj.Deptno == 0)
            {
                return BadRequest("Project Name and Department Number are required.");
            }

            // Validate department exists
            var departmentExists = await _departmentService.GetDepartmentById(editProj.Deptno);
            if (departmentExists == null)
            {
                return BadRequest("Department Number does not exist.");
            }

            var success = await _projectService.UpdateProject(projNo, editProj);
            if (!success)
            {
                return BadRequest("Project Name already exists or Department Number is already used by another project.");
            }

            return Ok("Project Data Successfully Updated.");
        }

        /// <summary>
        /// Deletes a project by its number.
        /// </summary>
        /// <remarks>
        /// Ensure that the project number provided is valid.
        ///
        /// Sample request:
        ///
        ///     DELETE /Project/1
        /// </remarks>
        /// <param name="projNo">The project number of the project to be deleted.</param>
        /// <returns>Success message if the project is deleted successfully or an error message if the project is not found.</returns>
        [HttpDelete("{projNo}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteProject(int projNo)
        {
            var success = await _projectService.DeleteProject(projNo);
            if (!success)
            {
                return BadRequest();
            }
            return Ok("Project Data Successfully Deleted.");
        }

        /// <summary>
        /// Retrieves a list of projects managed by any department.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Project/managed-by-dept
        /// </remarks>
        /// <returns>A list of projects managed by departments.</returns>
        [HttpGet("managed-by-dept")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjectsManagedByDept()
        {
            var projects = await _projectService.GetProjectsManagedByDept();
            return Ok(projects);
        }

        /// <summary>
        /// Retrieves a list of projects managed by specific departments (e.g., Finance, Sales).
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Project/managed-by-departments
        /// </remarks>
        /// <returns>A list of projects managed by the specified departments.</returns>
        [HttpGet("managed-by-departments")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjectsManagedByDepartments()
        {
            var departmentNames = new[] { "Finance", "Sales" };
            var projects = await _projectService.GetProjectsManagedByDepartments(departmentNames);
            if (projects == null || !projects.Any())
            {
                return NotFound("No projects found managed by the specified departments.");
            }
            return Ok(projects);
        }

        /// <summary>
        /// Retrieves a list of projects that have no employees assigned.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Project/no-employees
        /// </remarks>
        /// <returns>A list of projects with no employees.</returns>
        [HttpGet("no-employees")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjectsWithNoEmployees()
        {
            var projects = await _projectService.GetProjectsWithNoEmployees();
            return Ok(projects);
        }
    }
}
