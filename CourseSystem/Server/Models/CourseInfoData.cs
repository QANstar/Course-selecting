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
        public string CourseName { get; set; }
        [DisplayName("老师姓名")]
        public string TeacherName { get; set; }
        [DisplayName("上课时间")]
        public string Time { get; set; }
        [DisplayName("类型")]
        public string Type { get; set; }
        [DisplayName("老师Id")]
        public long TeacherId { get; set; }
        [DisplayName("专业")]
        public string Major { get; set; }
        [DisplayName("学分")]
        public String Score { get; set; }
        public string Students { get; set; }
        [DisplayName("最大选课人数")]
        public int? MaxStuNum { get; set; }
    }
}
