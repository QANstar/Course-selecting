using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSystem.Server.Models
{
    /// <summary>
    /// 学生信息模型
    /// </summary>
    public class StudentInfoModel
    {
        public long Id { get; set; }
        public String Name { get; set; }
        public String Grade { get; set; }
        public String College { get; set; }
        public String Major { get; set; }
        public String Class { get; set; }

    }
}
