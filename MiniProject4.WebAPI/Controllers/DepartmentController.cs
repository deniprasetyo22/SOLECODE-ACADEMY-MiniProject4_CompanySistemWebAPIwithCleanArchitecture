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
    /// Handles operations related to departments, including adding, retrieving, updating, and deleting departments.
    /// </summary>
    public class DepartmentController : ControllerBase
    {
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;

        /// <summary>
        /// Initializes the DepartmentController with the department and employee services.
        /// </summary>
        /// <param name="departmentService">The service used to manage departments.</param>
        /// <param name="employeeService">The service used to manage employees.</param>
        public DepartmentController(DepartmentService departmentService, EmployeeService employeeService)
        {
            _departmentService = departmentService;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Adds a new department to the system.
        /// </summary>
        /// <remarks>
        /// Ensure that the department data is not null and that both the department name and manager employee number are provided.
        /// The manager employee number must exist in the employee database.
        ///
        /// Sample request:
        ///
        ///     POST /Department
        ///     {
        ///        "deptname": "IT",
        ///        "mgrEmpNo": 123
        ///     }
        /// </remarks>
        /// <param name="department">The department to be added.</param>
        /// <returns>Success message if the department is added successfully or an error message if validation fails.</returns>
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Department>> AddDepartment([FromBody] Department department)
        {
            if (department == null)
            {
                return BadRequest("Department data cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(department.Deptname) || department.Mgrempno <= 0)
            {
                return BadRequest("Department name and Manager Employee Number are required.");
            }

            var managerExists = await _employeeService.GetEmployeeById(department.Mgrempno);
            if (managerExists == null)
            {
                return BadRequest("Manager Employee Number does not exist.");
            }

            var success = await _departmentService.AddDepartment(department);
            if (!success)
            {
                return BadRequest("Department Name or Manager Employee Number already exists.");
            }

            return Ok("Department data successfully added.");
        }

        /// <summary>
        /// Retrieves a paginated list of all departments in the system.
        /// </summary>
        /// <remarks>
        /// Provide the page number and page size for pagination.
        ///
        /// Sample request:
        ///
        ///     GET /Department
        /// </remarks>
        /// <param name="pageNumber">The page number for pagination. Must be greater than zero.</param>
        /// <param name="pageSize">The number of departments to retrieve per page. Must be greater than zero.</param>
        /// <returns>A paginated list of departments.</returns>
        /// <response code="200">Returns a paginated list of departments.</response>
        /// <response code="400">If the page number or page size is less than or equal to zero.</response>
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<Department>>> GetAllDepartments([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }

            var departments = await _departmentService.GetAllDepartments(pageNumber, pageSize);
            return Ok(departments);
        }


        /// <summary>
        /// Retrieves the details of a department by its number.
        /// </summary>
        /// <remarks>
        /// Ensure that the department number provided is valid.
        ///
        /// Sample request:
        ///
        ///     GET /Department/1
        /// </remarks>
        /// <param name="deptNo">The department number of the department to be retrieved.</param>
        /// <returns>Department details if found or an error message if the department is not found.</returns>
        [HttpGet("{deptNo}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Department>> GetDepartmentById(int deptNo)
        {
            var department = await _departmentService.GetDepartmentById(deptNo);
            if (department == null)
            {
                return NotFound("Department not found.");
            }
            return Ok(department);
        }

        /// <summary>
        /// Updates the details of a department by its number.
        /// </summary>
        /// <remarks>
        /// Ensure that the department data is not null and that both the department name and manager employee number are provided.
        /// The manager employee number must exist in the employee database.
        ///
        /// Sample request:
        ///
        ///     PUT /Department/1
        ///     {
        ///        "deptname": "Updated Department Name",
        ///        "mgrEmpNo": 456
        ///     }
        /// </remarks>
        /// <param name="deptNo">The department number of the department to be updated.</param>
        /// <param name="editDept">The updated department data.</param>
        /// <returns>Success message if the department is updated successfully or an error message if validation fails.</returns>
        [HttpPut("{deptNo}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateDepartment(int deptNo, [FromBody] Department editDept)
        {
            if (editDept == null)
            {
                return BadRequest("Department data cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(editDept.Deptname) || editDept.Mgrempno <= 0)
            {
                return BadRequest("Department name and Manager Employee Number are required.");
            }

            var managerExists = await _employeeService.GetEmployeeById(editDept.Mgrempno);
            if (managerExists == null)
            {
                return BadRequest("Manager Employee Number does not exist.");
            }

            var success = await _departmentService.UpdateDepartment(deptNo, editDept);
            if (!success)
            {
                return BadRequest("Department Name or Manager Employee Number already exists.");
            }

            return Ok("Department data successfully updated.");
        }

        /// <summary>
        /// Deletes a department by its number.
        /// </summary>
        /// <remarks>
        /// Ensure that the department number provided is valid.
        ///
        /// Sample request:
        ///
        ///     DELETE /Department/1
        /// </remarks>
        /// <param name="deptNo">The department number of the department to be deleted.</param>
        /// <returns>Success message if the department is deleted successfully or an error message if the department is not found.</returns>
        [HttpDelete("{deptNo}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteDepartment(int deptNo)
        {
            var success = await _departmentService.DeleteDepartment(deptNo);
            if (!success)
            {
                return BadRequest("Department Not Found");
            }
            return Ok("Department Data Successfully Deleted.");
        }
    }

}
