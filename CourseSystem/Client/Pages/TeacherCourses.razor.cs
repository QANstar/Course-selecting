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
    public partial class TeacherCourses
    {
        bool isLoading = true;
        private static IEnumerable<int> PageItemsSource => new int[] { 13, 15, 20 };
        private String[] week = { "一", "二", "三", "四", "五", "六", "天" };
        [Inject]
        [NotNull]
        private ToastService ToastService { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        private long userId;
        [Inject]
        public HttpClient Http { get; set; }

        private List<CourseInfoData> courseInfoData = new List<CourseInfoData>();
        private List<StudentInfoModel> studentInfoData = new List<StudentInfoModel>();
        private CourseInfoData SearchModel { get; set; } = new CourseInfoData();
        private StudentInfoModel StuSearchModel { get; set; } = new StudentInfoModel();
        private bool IsBackdropOpen { get; set; }

        protected async override Task OnInitializedAsync()
        {

            var authState = await authenticationStateTask;
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                userId = long.Parse(user.Identity.Name);

            }
            courseInfoData = await Http.GetFromJsonAsync<List<CourseInfoData>>("api/courseinfo/getcourseinfo?teacherId=" + userId);
            isLoading = false;
            await base.OnInitializedAsync();
        }

        private Task<QueryData<CourseInfoData>> OnQueryAsync(QueryPageOptions options)
        {
            IEnumerable<CourseInfoData> items = courseInfoData;
            // 设置记录总数
            var total = items.Count();
            // 处理高级搜索
            if (!string.IsNullOrEmpty(SearchModel.CourseName))
            {
                items = items.Where(item => item.CourseName?.Contains(SearchModel.CourseName, StringComparison.OrdinalIgnoreCase) ?? false);
            }
            if (!string.IsNullOrEmpty(SearchModel.Type))
            {
                items = items.Where(item => item.Type?.Contains(SearchModel.Type, StringComparison.OrdinalIgnoreCase) ?? false);
            }
            if (!string.IsNullOrEmpty(SearchModel.Time))
            {
                items = items.Where(item => item.Time?.Contains(SearchModel.Time, StringComparison.OrdinalIgnoreCase) ?? false);
            }
            if (!string.IsNullOrEmpty(SearchModel.Score))
            {
                items = items.Where(item => item.Score?.Contains(SearchModel.Score, StringComparison.OrdinalIgnoreCase) ?? false);
            }
            if (!string.IsNullOrEmpty(SearchModel.Major))
            {
                items = items.Where(item => item.Major?.Contains(SearchModel.Major, StringComparison.OrdinalIgnoreCase) ?? false);
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
                                 || (item.Type?.Contains(options.SearchText) ?? false) || (item.Time?.Contains(options.SearchText) ?? false) || (item.Score?.Contains(options.SearchText) ?? false) || (item.Major?.Contains(options.SearchText) ?? false));
                }
            }
            items = items.Skip((options.PageIndex - 1) * options.PageItems).Take(options.PageItems).ToList();
            return Task.FromResult(new QueryData<CourseInfoData>()
            {
                Items = items,
                IsSearch = !string.IsNullOrEmpty(SearchModel.CourseName) || !string.IsNullOrEmpty(SearchModel.Type) || !string.IsNullOrEmpty(SearchModel.Time) || !string.IsNullOrEmpty(SearchModel.Score) || !string.IsNullOrEmpty(SearchModel.Major),
                TotalCount = total,
            });
        }
        private async Task OnRowButtonClick(CourseInfoData item)
        {
            if(item.Students == null || item.Students.Replace(" ", "") == "")
            {
                studentInfoData = null;
            }
            else
            {
                studentInfoData = await Http.GetFromJsonAsync<List<StudentInfoModel>>("api/studentinfo/getstudentinfo?studentsId=" + item.Students.Replace(" ", ""));
            }
            
            IsBackdropOpen = true;
            StateHasChanged();
        }

        private Task<QueryData<StudentInfoModel>> OnQueryAsyncStu(QueryPageOptions options)
        {
            IEnumerable<StudentInfoModel> items = studentInfoData;
            // 处理高级搜索
            if (!string.IsNullOrEmpty(StuSearchModel.Name))
            {
                items = items.Where(item => item.Name?.Contains(StuSearchModel.Name, StringComparison.OrdinalIgnoreCase) ?? false);
            }
            if (!string.IsNullOrEmpty(StuSearchModel.Grade))
            {
                items = items.Where(item => item.Grade?.Contains(StuSearchModel.Grade, StringComparison.OrdinalIgnoreCase) ?? false);
            }
            if (!string.IsNullOrEmpty(StuSearchModel.College))
            {
                items = items.Where(item => item.College?.Contains(StuSearchModel.College, StringComparison.OrdinalIgnoreCase) ?? false);
            }
            if (!string.IsNullOrEmpty(StuSearchModel.Major))
            {
                items = items.Where(item => item.Major?.Contains(StuSearchModel.Major, StringComparison.OrdinalIgnoreCase) ?? false);
            }
            if (!string.IsNullOrEmpty(StuSearchModel.Class))
            {
                items = items.Where(item => item.Class?.Contains(StuSearchModel.Class, StringComparison.OrdinalIgnoreCase) ?? false);
            }
            // 处理 Searchable=true 列与 SeachText 模糊搜索
            if (options.Searchs.Any())
            {
                items = items.Where(options.Searchs.GetFilterFunc<StudentInfoModel>(FilterLogic.Or));
            }
            else
            {
                // 处理 SearchText 模糊搜索
                if (!string.IsNullOrEmpty(options.SearchText))
                {
                    items = items.Where(item => (item.Name?.Contains(options.SearchText) ?? false) || (item.Grade?.Contains(options.SearchText) ?? false) || (item.College?.Contains(options.SearchText) ?? false) || (item.Major?.Contains(options.SearchText) ?? false) || (item.Class?.Contains(options.SearchText) ?? false));
                }
            }
            return Task.FromResult(new QueryData<StudentInfoModel>()
            {
                Items = items,
                IsSearch = !string.IsNullOrEmpty(StuSearchModel.Name) || !string.IsNullOrEmpty(StuSearchModel.Grade) || !string.IsNullOrEmpty(StuSearchModel.College) || !string.IsNullOrEmpty(StuSearchModel.Major) || !string.IsNullOrEmpty(StuSearchModel.Class),
            });
        }
    }
}
