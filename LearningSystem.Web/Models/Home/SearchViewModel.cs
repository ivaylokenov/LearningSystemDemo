namespace LearningSystem.Web.Models.Home
{
    using Services.Models;
    using System.Collections.Generic;

    public class SearchViewModel
    {
        public IEnumerable<CourseListingServiceModel> Courses { get; set; } 
            = new List<CourseListingServiceModel>();

        public IEnumerable<UserListingServiceModel> Users { get; set; }
            = new List<UserListingServiceModel>();

        public string SearchText { get; set; }
    }
}
