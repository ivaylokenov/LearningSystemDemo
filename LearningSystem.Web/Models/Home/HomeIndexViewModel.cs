namespace LearningSystem.Web.Models.Home
{
    using Services.Models;
    using System.Collections.Generic;

    public class HomeIndexViewModel : SearchFormModel
    {
        public IEnumerable<CourseListingServiceModel> Courses { get; set; }
    }
}
