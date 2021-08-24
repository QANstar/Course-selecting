using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSystem.Server.Models
{
    public class CourseInfoData
    {
        public int Id { get; set; }
        [DisplayName("课程名称")]
        [Required]
        public string CourseName { get; set; }
        [DisplayName("老师姓名")]
        [Required]
        public string TeacherName { get; set; }
        [DisplayName("上课时间")]
        [Required]
        public string Time { get; set; }
        [DisplayName("类型")]
        [Required]
        public string Type { get; set; }
        [DisplayName("老师Id")]
        [Required]
        public long TeacherId { get; set; }
        [DisplayName("专业")]
        [Required]
        public string Major { get; set; }
        [DisplayName("学分")]
        [Required]
        public String Score { get; set; }
        public string Students { get; set; }
        [DisplayName("最大选课人数")]
        [Required]
        public int? MaxStuNum { get; set; }
    }
}
