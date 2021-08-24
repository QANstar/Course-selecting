using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSystem.Server.Models
{
    public class StudentsCoursesModel
    {
        public int Id { get; set; }
        public long StudentId { get; set; }
        public string ChooseCourse { get; set; }
        public int CompletedRequiredScore { get; set; }
        public int ChooseRequiredScore { get; set; }
        public int CompletedElectiveScore { get; set; }
        public int ChooseElectiveScore { get; set; }
        public int CompletedMinorScore { get; set; }
        public int ChooseMinorScore { get; set; }
    }
}
