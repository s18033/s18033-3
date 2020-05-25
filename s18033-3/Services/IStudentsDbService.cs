﻿using Cw3.Models;
using Microsoft.AspNetCore.Mvc;
using s18033_3.DTOs.Requests;
using s18033_3.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace s18033_3.Services
{
    public interface IStudentsDbService
    {
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            throw new NotImplementedException();
        }

        public EnrollStudentResponse PromoteStudents(int semester, string studies)
        {
            throw new NotImplementedException();
        }

        public Boolean IsStudentExists(string indexNumber)
        {
            throw new NotImplementedException();
        }

        public Student GetStudent(string indexNumber)
        {
            throw new NotImplementedException();
        }
    }
}
