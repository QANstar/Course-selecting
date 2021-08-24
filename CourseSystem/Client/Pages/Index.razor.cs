using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using CourseSystem.Server.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace CourseSystem.Client.Pages
{
    public partial class Index
    {
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        private long userId;
        [Inject]
        public HttpClient Http { get; set; }

        private List<StudentInfoModel> studentInfoData = new List<StudentInfoModel>();
        private List<TrainingPlanInfoModel> trainingPlanInfoData= new List<TrainingPlanInfoModel>();
        private List<StudentsCoursesModel> studentsCoursesData = new List<StudentsCoursesModel>();
        private int chosenCourseNum = 0;
        protected async override Task OnInitializedAsync()
        {
            var authState = await authenticationStateTask;
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                userId = long.Parse(user.Identity.Name);
                
            }
            studentInfoData = await Http.GetFromJsonAsync<List<StudentInfoModel>>("api/studentinfo/getstudentinfo?userid="+ userId);
            trainingPlanInfoData = await Http.GetFromJsonAsync<List<TrainingPlanInfoModel>>("api/studentinfo/gettrainingplaninfo?majorname=" + studentInfoData[0].Major);
            studentsCoursesData = await Http.GetFromJsonAsync<List<StudentsCoursesModel>>("api/studentinfo/getstudentscoursesinfo?studentid=" + userId);
            if(studentsCoursesData[0].ChooseCourse.Replace(" ", "") == "")
            {
                chosenCourseNum = 0;
            }
            else
            {
                chosenCourseNum = studentsCoursesData[0].ChooseCourse.Split(',').Length;
            }
            await base.OnInitializedAsync();
        }
    }
}
