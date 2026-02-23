using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Week3_L1.Models
{
    public class Student
    {
        
        [JsonPropertyName("name")]
        public required string StudentName { get; set; }

        [JsonPropertyName("id")]
        public required int StudentId { get; set; }
    }
}
