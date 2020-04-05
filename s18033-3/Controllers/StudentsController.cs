using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DAL;
using Cw3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace s18033_3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {

        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        //[HttpGet]
        // public string GetStudent(string orderBy)
        // {
        //     return $"Kowalski, Malewski, Andrzejewski sortowanie={orderBy}";
        // }

        [HttpGet]
        public IActionResult GetStudent(string orderBy)
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{indexNumber}")]
        public IActionResult GetEnrollment(string indexNumber)
        {
            return Ok(_dbService.GetEnrollments(indexNumber));
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, Student student)
        {
            return Ok($"Aktualizacja studenta {id} i imieniu {student.FirstName} ukończona.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id, Student student)
        {
            return Ok($"Usuwanie studenta {id} i imieniu {student.FirstName} ukończone ");
        }
    }
}
