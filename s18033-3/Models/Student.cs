using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Models
{
    public class Student
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IndexNumber { get; set; }
        public DateTime Birthdate { get; set; }

        public string Password { get; set; }
        public string Studies { get; set; }
    }
}