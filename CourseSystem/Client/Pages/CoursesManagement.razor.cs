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
    /// <summary>
    /// 课程管理
    /// </summary>
    public partial class CoursesManagement
    {
        bool isLoading = true;//加载指示
        private static IEnumerable<int> PageItemsSource => new int[] { 13, 15, 20 };
        private String[] week = { "一","二","三","四","五","六","天" };
        [Inject]
        [NotNull]
        private ToastService ToastService { get; set; }
        [CascadingParameter]
        [Inject]
        public HttpClient Http { get; set; }

        private List<CourseInfoData> courseInfoData = new List<CourseInfoData>();
        private CourseInfoData SearchModel { get; set; } = new CourseInfoData();

        protected async override Task OnInitializedAsync()
        {
            courseInfoData = await Http.GetFromJsonAsync<List<CourseInfoData>>("api/courseinfo/getcourseinfo");
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
            if (!string.IsNullOrEmpty(SearchModel.Type))
            {
                items = items.Where(item => item.Type?.Contains(SearchModel.Type, StringComparison.OrdinalIgnoreCase) ?? false);
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
                                 || (item.TeacherName?.Contains(options.SearchText) ?? false) || (item.Time?.Contains(options.SearchText) ?? false) || (item.Score?.Contains(options.SearchText) ?? false) || (item.Type?.Contains(options.SearchText) ?? false) || (item.Major?.Contains(options.SearchText) ?? false));
                }
            }
            items = items.Skip((options.PageIndex - 1) * options.PageItems).Take(options.PageItems).ToList();
            return Task.FromResult(new QueryData<CourseInfoData>()
            {
                Items = items,
                IsSearch = !string.IsNullOrEmpty(SearchModel.CourseName) || !string.IsNullOrEmpty(SearchModel.TeacherName) || !string.IsNullOrEmpty(SearchModel.Time) || !string.IsNullOrEmpty(SearchModel.Score) || !string.IsNullOrEmpty(SearchModel.Type) || !string.IsNullOrEmpty(SearchModel.Major),
                TotalCount = total,
            });
        }

        private static Task<CourseInfoData> OnAddAsync() => Task.FromResult(new CourseInfoData() { });

        private async Task<bool> OnSaveAsync(CourseInfoData item)
        {
            String teacherName = await Http.GetStringAsync($"api/Roles/GetTeachetInfo?userId={item.TeacherId}");
            if(teacherName!=null)
            {
                item.TeacherName = teacherName;
                var result = await Http.PostAsJsonAsync<CourseInfoData>($"api/CourseInfo/EditOrAddCourse", item);
                if (result.IsSuccessStatusCode)
                {
                    if (item.Id == 0)
                    {
                        item.Id = await result.Content.ReadFromJsonAsync<int>();
                        courseInfoData.Add(item);
                    }
                    else
                    {
                        courseInfoData.ForEach(i =>
                        {
                            if (i.Id == item.Id)
                            {
                                i.CourseName = item.CourseName;
                                i.Major = item.Major;
                                i.Score = item.Score;
                                i.TeacherId = item.TeacherId;
                                i.TeacherName = item.TeacherName;
                                i.Time = item.Time;
                                i.Type = item.Type;
                                i.MaxStuNum = item.MaxStuNum;
                            }
                        });
                    }
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

        }

        private Task<bool> OnDeleteAsync(IEnumerable<CourseInfoData> items)
        {
            items.ToList().ForEach(async i => await Http.DeleteAsync($"api/CourseInfo/DelCourse?courseId={i.Id}"));
            items.ToList().ForEach(i => courseInfoData.Remove(i));
            return Task.FromResult(true);
        }
    }
}
