﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseSystem.Entity
{
    [Table("TrainingPlan")]
    public partial class TrainingPlan
    {
        [Key]
        public int Id { get; set; }
        [StringLength(10)]
        public string Major { get; set; }
        [StringLength(10)]
        public string RequiredScore { get; set; }
        [StringLength(10)]
        public string OptionalScore { get; set; }
        [StringLength(10)]
        public string MinorScore { get; set; }
    }
}