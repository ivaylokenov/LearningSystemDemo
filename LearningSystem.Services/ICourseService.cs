namespace LearningSystem.Services
{
    using Models;   
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICourseService
    {
        Task<IEnumerable<CourseListingServiceModel>> ActiveAsync();

        Task<IEnumerable<CourseListingServiceModel>> FindAsync(string searchText);

        Task<TModel> ByIdAsync<TModel>(int id) where TModel : class;

        Task<bool> SignUpStudentAsync(int courseId, string studentId);

        Task<bool> SignOutStudentAsync(int courseId, string studentId);

        Task<bool> StudentIsEnrolledCourseAsync(int courseId, string studentId);

        Task<bool> SaveExamSubmission(int courseId, string studentId, byte[] examSubmission);
    }
}
