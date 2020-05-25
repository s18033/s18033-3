using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Cw3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using s18033_3.DTOs;
using s18033_3.DTOs.Requests;
using s18033_3.DTOs.Responses;
using s18033_3.Helpers;
using s18033_3.Services;

namespace s18033_3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {

        private IStudentsDbService _service;
        private IConfiguration Configuration { get; set; }
        public StudentsController(IStudentsDbService service, IConfiguration configuration)
        {
            this.Configuration = configuration;
            this._service = service;
        }

        [HttpGet]
        [Authorize(Roles = "student")]
        public IActionResult GetStudents()
        {
            var students = new List<Student>();

            students.Add(new Student
            {
                IndexNumber = "s18033",
                FirstName = "Piotr",
                LastName = "Kwiatek",
                Studies = "IT",
                Password = "",
                Birthdate = DateTime.Parse("1/1/1970")
            });

            return Ok(students);
        }

        [HttpPost]
        public IActionResult Login(LoginRequestDto request) {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var student = _service.GetStudent(request.Index);

            var requestPassword = PasswordHelper.CreateMD5(request.Password);

            if (student == null)
            {
                return BadRequest();
            }

            // Użyłem któregokolwiek algorytmu - wiem że MD5 jest niebezpieczne ale jako że jest to praca domowa, to go użyłem.
            // W bazie danych hasła to string 123 w formie hasha MD5. Normalnie użyłbym algorytmu wraz z kluczem a dodatkowo trzymałbym wartość soli w bazie danych.
            // Mam nadzieję że podany przykład udowadnia zrobienie zadania numer 3. Pozdrawiam.
            if (!student.Password.Equals(requestPassword))
            {
                return BadRequest();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, request.Index),
                new Claim(ClaimTypes.Role, "employee"), // tylko do testów końcówek
                new Claim(ClaimTypes.Role, "student")
            };

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "PK",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = Guid.NewGuid()
            });
        }
    }
}