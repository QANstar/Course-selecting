using CourseSystem.Entity;
using CourseSystem.Server.Models;
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
    public class StudentInfoController : Controller
    {

        CourseselectiondbContext Context;

        public StudentInfoController(CourseselectiondbContext context)
        {
            Context = context;
        }

        [HttpGet]
        public List<StudentInfoModel> GetStudentInfo(long userId,String studentsId)
        {
            if (studentsId == null||studentsId.Replace(" ", "") == "")
            {
                if (userId == 0)
                {
                    var result = Context.StudentInfos;
                    return QueryStudentInfo(result).ToList();
                }
                else
                {
                    var result = Context.StudentInfos.Where(x => x.Id == userId);
                    return QueryStudentInfo(result).ToList();
                }
            }
            else
            {
                long[] stuId = Array.ConvertAll(studentsId.Split(new char[] { ',' }), long.Parse);
                var result = Context.StudentInfos.Where(x => stuId.Contains(x.Id));
                return QueryStudentInfo(result).ToList();
            }

            
        }

        private IQueryable<StudentInfoModel> QueryStudentInfo(IQueryable<Entity.StudentInfo> query)
        {
            return query.Select(x => new StudentInfoModel()
            {
                Id = x.Id,
                Name = x.Name,
                Grade = x.Grade,
                College = x.College,
                Major = x.Major,
                Class = x.Class,
            });
        }

        [HttpGet]
        public List<TrainingPlanInfoModel> GetTrainingPlanInfo(String MajorName)
        {
            var result = Context.TrainingPlans.Where(x => x.Major == MajorName);
            return QueryTrainingPlanInfo(result).ToList();
        }

        private IQueryable<TrainingPlanInfoModel> QueryTrainingPlanInfo(IQueryable<Entity.TrainingPlan> query)
        {
            return query.Select(x => new TrainingPlanInfoModel()
            {
                Id = x.Id,
                Major  = x.Major,
                RequiredScore = x.RequiredScore,
                OptionalScore = x.OptionalScore,
                MinorScore = x.MinorScore,
            });
        }

        [HttpGet]
        public List<StudentsCoursesModel> GetStudentsCoursesInfo(long studentId)
        {
            var result = Context.StudentsCourseInfoTables.Where(x => x.StudentId == studentId);
            return QueryStudentsCoursesInfo(result).ToList();
        }

        private IQueryable<StudentsCoursesModel> QueryStudentsCoursesInfo(IQueryable<Entity.StudentsCourseInfoTable> query)
        {
            return query.Select(x => new StudentsCoursesModel()
            {
                Id=x.Id,
                StudentId = (long)x.StudentId,
                ChooseCourse = x.ChooseCourse,
                CompletedRequiredScore = (int)x.CompletedRequiredScore,
                ChooseRequiredScore = (int)x.ChooseRequiredScore,
                CompletedElectiveScore = (int)x.CompletedElectiveScore,
                ChooseElectiveScore = (int)x.ChooseElectiveScore,
                CompletedMinorScore = (int)x.CompletedMinorScore,
                ChooseMinorScore = (int)x.ChooseMinorScore,
            });
        }

        [HttpPost]
        public int AddCourse(StudentsCoursesModel courseData)
        {
            Entity.StudentsCourseInfoTable studentsCourseData = Context.StudentsCourseInfoTables.FirstOrDefault(x => x.StudentId == courseData.StudentId);
            if (studentsCourseData.ChooseCourse == null || studentsCourseData.ChooseCourse.Replace(" ", "")=="")
            {
                studentsCourseData.ChooseCourse = courseData.ChooseCourse;
            }
            else
            {
                studentsCourseData.ChooseCourse = studentsCourseData.ChooseCourse.Replace(" ", "") + ',' + courseData.ChooseCourse;
            }
            
            studentsCourseData.ChooseRequiredScore = studentsCourseData.ChooseRequiredScore + courseData.ChooseRequiredScore;
            studentsCourseData.ChooseMinorScore = studentsCourseData.ChooseMinorScore + courseData.ChooseMinorScore;
            studentsCourseData.ChooseElectiveScore = studentsCourseData.ChooseElectiveScore + courseData.ChooseElectiveScore;
            Context.SaveChanges();
            return studentsCourseData.Id;

        }

        [HttpPost]
        public int CancelCourse(StudentsCoursesModel courseData)
        {
            Entity.StudentsCourseInfoTable studentsCourseData = Context.StudentsCourseInfoTables.FirstOrDefault(x => x.StudentId == courseData.StudentId);
            String[] courseList = studentsCourseData.ChooseCourse.Replace(" ", "").Split(',');
            ArrayList list = new ArrayList(courseList);
            list.Remove(courseData.ChooseCourse);
            courseList = (string[])list.ToArray(typeof(string));
            studentsCourseData.ChooseCourse = String.Join(',', courseList);
            studentsCourseData.ChooseRequiredScore = studentsCourseData.ChooseRequiredScore - courseData.ChooseRequiredScore;
            studentsCourseData.ChooseMinorScore = studentsCourseData.ChooseMinorScore - courseData.ChooseMinorScore;
            studentsCourseData.ChooseElectiveScore = studentsCourseData.ChooseElectiveScore - courseData.ChooseElectiveScore;
            Context.SaveChanges();
            return studentsCourseData.Id;

        }
    }
}
