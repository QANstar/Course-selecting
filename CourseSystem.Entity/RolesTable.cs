﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseSystem.Entity
{
    [Table("RolesTable")]
    public partial class RolesTable
    {
        [Key]
        public long Id { get; set; }
        [StringLength(10)]
        public string Roles { get; set; }
    }
}