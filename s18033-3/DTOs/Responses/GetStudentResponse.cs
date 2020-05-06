using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace s18033_3.DTOs.Requests
{
    public class GetStudentResponse
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
