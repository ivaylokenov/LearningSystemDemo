namespace LearningSystem.Services.Admin
{
    using System;
    using System.Threading.Tasks;

    public interface IAdminCourseService
    {
        Task CreateAsync(
            string name,
            string description,
            DateTime startDate,
            DateTime endDate,
            string trainerId);
    }
}
