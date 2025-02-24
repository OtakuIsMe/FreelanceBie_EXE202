using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.src.api.domains.Enum;

namespace BE.src.api.domains.DTOs.Post
{
    public class PostJobFilterDTO
    {
        public string? Title { get; set; }
        public WorkTypeEnum? WorkType { get; set; }
        public string? WorkLocation { get; set; }
        public string? CompanyName { get; set; }
        public EmploymentTypeEnum? EmploymentType { get; set; }       
        public int? MinExperience { get; set; }
        public int? MaxExperience { get; set; }

        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserCity { get; set; }
        public string? UserEducation { get; set; }

        public string? SpecialtyName { get; set; }
    }
}