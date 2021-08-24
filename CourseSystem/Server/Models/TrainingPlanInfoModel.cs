using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSystem.Server.Models
{
    public class TrainingPlanInfoModel
    {
        public int Id { get; set; }
        public string Major { get; set; }
        public string RequiredScore { get; set; }
        public string OptionalScore { get; set; }
        public string MinorScore { get; set; }
    }
}
