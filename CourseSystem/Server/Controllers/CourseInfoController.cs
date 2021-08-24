using CourseSystem.Entity;
using CourseSystem.Server.Models;
using Humanizer;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSystem.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CourseInfoController : Controller
    {
        CourseselectiondbContext Context;
        public CourseInfoController(CourseselectiondbContext context)
        {
            Context = context;
        }

        [HttpGet]
        public List<CourseInfoData> GetCourseInfo(String major,String courseId,long teacherId)
        {
            if(teacherId == 0)  
            {
                if (major == null)
                {
                    if (courseId == null)
                    {
                        var result = Context.CourseData;
                        return QueryCourseInfo(result).ToList();
                    }
                    else
                    {
                        int[] chosenId = Array.ConvertAll(courseId.Split(new char[] { ',' }), int.Parse);
                        var result = Context.CourseData.Where(x => chosenId.Contains(x.Id));
                        return QueryCourseInfo(result).ToList();
                    }

                }
                else
                {
                    if (courseId == null)
                    {
                        var result = Context.CourseData.Where(x => x.Major == major || x.Major == "全校");
                        return QueryCourseInfo(result).ToList();
                    }
                    else
                    {
                        int[] chosenId = Array.ConvertAll(courseId.Split(new char[] { ',' }), int.Parse);
                        var result = Context.CourseData.Where(x => ( x.Major == major || x.Major == "全校") && !chosenId.Contains(x.Id));
                        return QueryCourseInfo(result).ToList();
                    }

                }
            }
            else
            {
                var result = Context.CourseData.Where(x => x.TeacherId == teacherId);
                return QueryCourseInfo(result).ToList();
            }


            
        }

        private IQueryable<CourseInfoData> QueryCourseInfo(IQueryable<CourseDatum> query)
        {
            return query.Select(x => new CourseInfoData()
            {
                Id = x.Id,
                CourseName = x.CourseName,
                TeacherName= x.TeacherName,
                Time= x.Time,
                Type=x.Type,
                TeacherId= (long)x.TeacherId,
                Major=x.Major,
                Score =  x.Score.ToString(),
                Students = x.Students,
                MaxStuNum= x.MaxStuNum,
            });
        }

        [HttpPost]
        public int AddStudent(CourseDatum courseData)
        {
            Entity.CourseDatum course = Context.CourseData.FirstOrDefault(x => x.Id == courseData.Id);
            if (course.Students == null || course.Students.Replace(" ", "") == "")
            {
                course.Students =  courseData.Students;
            }
            else
            {
                course.Students = course.Students.Replace(" ", "") + ',' + courseData.Students;
            }

            Context.SaveChanges();
            return course.Id;

        }

        [HttpPost]
        public int CancelStudent(CourseDatum courseData)
        {
            Entity.CourseDatum course = Context.CourseData.FirstOrDefault(x => x.Id == courseData.Id);
            String[] courseList = course.Students.Replace(" ", "").Split(',');
            ArrayList list = new ArrayList(courseList);
            list.Remove(courseData.Students);
            courseList = (string[])list.ToArray(typeof(string));
            course.Students = String.Join(',', courseList);
            Context.SaveChanges();
            return course.Id;

        }

        [HttpPost]
        public int EditOrAddCourse(CourseDatum courseData)
        {
            Entity.CourseDatum entity;
            if (courseData.Id == 0)
            {
                entity = new Entity.CourseDatum();
                Context.Add(entity);
            }
            else
{
                entity = Context.CourseData.FirstOrDefault(x => x.Id == courseData.Id);
            }
            entity.CourseName = courseData.CourseName;
            entity.TeacherName = courseData.TeacherName;
            entity.Time = courseData.Time;
            entity.Type = courseData.Type;
            entity.TeacherId =  courseData.TeacherId;
            entity.Major = courseData.Major;
            entity.Score = courseData.Score;
            entity.MaxStuNum = courseData.MaxStuNum;
            Context.SaveChanges();
            return entity.Id;

        }

        [HttpDelete]
        public void DelCourse(int courseId)
        {
            Context.CourseData.Remove(Context.CourseData.Find(courseId));
            Context.SaveChanges();
        }
    }
}
