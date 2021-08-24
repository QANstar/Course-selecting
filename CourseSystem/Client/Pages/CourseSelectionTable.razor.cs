using CourseSystem.Server.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using BootstrapBlazor.Components;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components.Web;
using TechTalk.SpecFlow;
using System.Diagnostics.CodeAnalysis;

namespace CourseSystem.Client.Pages
{
    public partial class CourseSelectionTable
    {
        bool isLoading = true;
        private static IEnumerable<int> PageItemsSource => new int[] { 13, 15, 20 };
        private String[] week = { "一", "二", "三", "四", "五", "六", "天" };
        IEnumerable<MenuItem> MenuItems = new List<MenuItem>(new MenuItem[]
            { 
                new MenuItem(){ Text = "必修",Url="",IsActive=true},
                new MenuItem(){ Text = "选修",Url=""},
                new MenuItem(){ Text = "辅修",Url=""},
            });
        [Inject]
        [NotNull]
        private SwalService? SwalService { get; set; }
        [Inject]
        [NotNull]
        private ToastService ToastService { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        private long userId;
        [Inject]
        public HttpClient Http { get; set; }

        private List<StudentInfoModel> studentInfoData = new List<StudentInfoModel>();
        private List<StudentsCoursesModel> studentsCourseInfoData = new List<StudentsCoursesModel>();
        private List<CourseInfoData> getData = new List<CourseInfoData>();
        private List<CourseInfoData> courseInfoData = new List<CourseInfoData>();
        private CourseInfoData SearchModel { get; set; } = new CourseInfoData();

        protected async override Task OnInitializedAsync()
        {
          
            var authState = await authenticationStateTask;
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                userId = long.Parse(user.Identity.Name);

            }
            studentsCourseInfoData = await Http.GetFromJsonAsync<List<StudentsCoursesModel>>("api/studentinfo/getstudentscoursesInfo?studentId=" + userId);
            studentInfoData = await Http.GetFromJsonAsync<List<StudentInfoModel>>("api/studentinfo/getstudentinfo?userid=" + userId);
            getData = await Http.GetFromJsonAsync<List<CourseInfoData>>("api/courseinfo/getcourseinfo?major=" + studentInfoData[0].Major + "&courseId=" + studentsCourseInfoData[0].ChooseCourse);
            courseInfoData = getData.Where(x => x.Type.Contains("必修")).ToList();
            isLoading = false;
            await base.OnInitializedAsync();
        }

        private Task<QueryData<CourseInfoData>> OnQueryAsync(QueryPageOptions options)
        {
            IEnumerable<CourseInfoData> items = courseInfoData;
            var total = items.Count();
            // 处理高级搜索
            if (!string.IsNullOrEmpty(SearchModel.CourseName))
            {
                items = items.Where(item => item.CourseName?.Contains(SearchModel.CourseName, StringComparison.OrdinalIgnoreCase) ?? false);
            }
            if (!string.IsNullOrEmpty(SearchModel.TeacherName))
            {
                items = items.Where(item => item.TeacherName?.Contains(SearchModel.TeacherName, StringComparison.OrdinalIgnoreCase) ?? false);
            }
            if (!string.IsNullOrEmpty(SearchModel.Time))
            {
                items = items.Where(item => item.Time?.Contains(SearchModel.Time, StringComparison.OrdinalIgnoreCase) ?? false);
            }
            if (!string.IsNullOrEmpty(SearchModel.Score))
            {
                items = items.Where(item => item.Score?.Contains(SearchModel.Score, StringComparison.OrdinalIgnoreCase) ?? false);
            }
            // 处理 Searchable=true 列与 SeachText 模糊搜索
            if (options.Searchs.Any())
            {
                items = items.Where(options.Searchs.GetFilterFunc<CourseInfoData>(FilterLogic.Or));
            }
            else
            {
                // 处理 SearchText 模糊搜索
                if (!string.IsNullOrEmpty(options.SearchText))
                {
                    items = items.Where(item => (item.CourseName?.Contains(options.SearchText) ?? false)
                                 || (item.TeacherName?.Contains(options.SearchText) ?? false) || (item.Time?.Contains(options.SearchText) ?? false) || (item.Score?.Contains(options.SearchText) ?? false)) ;
                }
            }
            items = items.Skip((options.PageIndex - 1) * options.PageItems).Take(options.PageItems).ToList();
            return Task.FromResult(new QueryData<CourseInfoData>()
            {
                Items = items,
                IsSearch = !string.IsNullOrEmpty(SearchModel.CourseName) || !string.IsNullOrEmpty(SearchModel.TeacherName) || !string.IsNullOrEmpty(SearchModel.Time) || !string.IsNullOrEmpty(SearchModel.Score),
                TotalCount = total,
            });
        }

        public Task OnClickMenu(MenuItem item)
        {
            courseInfoData = getData.Where(x => x.Type.Contains(item.Text)).ToList();
            StateHasChanged();
            return Task.FromResult(courseInfoData);
        }

        private async Task OnRowButtonClick(CourseInfoData item)
        {
            bool ret = true;
            bool isMax = false;
            bool isClash = false;
            List<StudentsCoursesModel> studentsCourseInfoData2 = new List<StudentsCoursesModel>();
            List<CourseInfoData> CourseData2 = new List<CourseInfoData>();
            studentsCourseInfoData2 = await Http.GetFromJsonAsync<List<StudentsCoursesModel>>("api/studentinfo/getstudentscoursesInfo?studentId=" + userId);
            if (studentsCourseInfoData2[0].ChooseCourse.Replace(" ", "") == "")
            {
                CourseData2 = null;
            }
            else
            {
                CourseData2 = await Http.GetFromJsonAsync<List<CourseInfoData>>("api/courseinfo/getcourseinfo?courseId=" + studentsCourseInfoData2[0].ChooseCourse);
            }
            if(CourseData2.Exists(x => x.Time.Replace(" ", "") == item.Time.Replace(" ", "")))
            {
                var op = new SwalOption()
                {
                    Category = SwalCategory.Error,
                    Title = "当前课程时间冲突",
                    Content = "当前课程时间冲突，无法选课",
                };
                SwalService.Show(op);
                isClash = true;
            }
            if (!isClash && item.Students.Replace(" ", "").Split(',').Length >= item.MaxStuNum)
            {
                var op = new SwalOption()
                {
                    Category = SwalCategory.Error,
                    Title = "当前课程已满",
                    Content = "当前课程人数已满，无法选课",
                };
                SwalService.Show(op);
                isMax = true;
            }
            if (!isMax && !isClash && ( int.Parse(item.Score)+ studentsCourseInfoData[0].ChooseElectiveScore+ studentsCourseInfoData[0].ChooseMinorScore+ studentsCourseInfoData[0].ChooseRequiredScore)>32)
            {
                var op = new SwalOption()
                {
                    Category = SwalCategory.Warning,
                    Title = "学分超出32分",
                    Content = "所有课程学分已超过32分，如选择则多余学分将不会记录，是否继续选择",
                    IsConfirm = true
                };
                ret = await SwalService.ShowModal(op);
            }
            else
            {
                ret = true;
            }
            if (ret == true && !isMax && !isClash)
            {
                StudentsCoursesModel addCourse = new StudentsCoursesModel();
                CourseInfoData courseData = new CourseInfoData();
                var authState = await authenticationStateTask;
                var user = authState.User;

                if (user.Identity.IsAuthenticated)
                {
                    addCourse.StudentId = long.Parse(user.Identity.Name);
                    addCourse.ChooseCourse = item.Id.ToString();
                    if (item.Type.Contains("必修"))
                    {
                        addCourse.ChooseRequiredScore = int.Parse(item.Score);
                        studentsCourseInfoData[0].ChooseRequiredScore = studentsCourseInfoData[0].ChooseRequiredScore + int.Parse(item.Score);
                    }
                    else if (item.Type.Contains("选修"))
                    {
                        addCourse.ChooseElectiveScore = int.Parse(item.Score);
                        studentsCourseInfoData[0].ChooseElectiveScore = studentsCourseInfoData[0].ChooseElectiveScore + int.Parse(item.Score);
                    }
                    else
                    {
                        addCourse.ChooseMinorScore = int.Parse(item.Score);
                        studentsCourseInfoData[0].ChooseMinorScore = studentsCourseInfoData[0].ChooseMinorScore + int.Parse(item.Score);
                    }
                    courseData.Id = item.Id;
                    courseData.Students = user.Identity.Name;
                }

                var result = await Http.PostAsJsonAsync<StudentsCoursesModel>($"api/StudentInfo/AddCourse", addCourse);
                var courseResult = await Http.PostAsJsonAsync<CourseInfoData>($"api/CourseInfo/AddStudent", courseData);
                if (result.IsSuccessStatusCode)
                {
                    getData.RemoveAll(x => x.Id == item.Id);
                    courseInfoData.RemoveAll(x => x.Id == item.Id);
                    StateHasChanged();
                    await ToastService.Show(new ToastOption()
                    {
                        Category = ToastCategory.Success,
                        Title = "选课成功",
                        Content = "选课成功，4 秒后自动关闭"
                    });
                }
                else
                {
                    await ToastService.Show(new ToastOption()
                    {
                        Category = ToastCategory.Error,
                        Title = "选课失败",
                        Content = "选课失败，4 秒后自动关闭"
                    });
                }
            }
            else
            {
                await ToastService.Show(new ToastOption()
                {
                    Category = ToastCategory.Information,
                    Title = "取消选课",
                    Content = "取消选课，4 秒后自动关闭"
                });
            }
            

        }

    }
}
