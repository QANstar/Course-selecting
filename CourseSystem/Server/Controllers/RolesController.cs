using CourseSystem.Entity;
using CourseSystem.Server.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSystem.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RolesController : Controller
    {
        CourseselectiondbContext Context;

        public RolesController(CourseselectiondbContext context)
        {
            Context = context;
        }

        [HttpGet]
        public List<RolesModel> GetRoles(long userId)
        {
            var result = Context.RolesTables.Where(x => x.Id == userId);
            return QueryStudentInfo(result).ToList();
        }

        private IQueryable<RolesModel> QueryStudentInfo(IQueryable<Entity.RolesTable> query)
        {
            return query.Select(x => new RolesModel()
            {
                Id = x.Id,
                Roles = x.Roles
            });
        }

        [HttpGet]
        public String GetTeachetInfo(long userId)
        {
            var result = Context.TeacherInfos.FirstOrDefault(x => x.Id == userId);
            return result.Name;
        }

    }
}
