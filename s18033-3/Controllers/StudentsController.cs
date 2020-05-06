using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Cw3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using s18033_3.DTOs.Requests;
using s18033_3.DTOs.Responses;
using s18033_3.Services;

namespace s18033_3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {

        private IStudentsDbService _service;

        public StudentsController(IStudentsDbService service)
        {
            this._service = service;
        }

        [HttpGet]
        public IActionResult GetStudent(string index)
        {
            var student = _service.GetStudent(index);

            if (student == null) {
                return NotFound();
            }

            var response = new GetStudentResponse();
            response.FirstName = student.FirstName;
            response.LastName = student.LastName;

            return Ok(response);
        }
    }
}