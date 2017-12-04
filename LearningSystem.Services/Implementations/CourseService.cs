namespace LearningSystem.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CourseService : ICourseService
    {
        private readonly LearningSystemDbContext db;

        public CourseService(LearningSystemDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<CourseListingServiceModel>> ActiveAsync()
            => await this.db
                .Courses
                .OrderByDescending(c => c.Id)
                .Where(c => c.StartDate >= DateTime.UtcNow)
                .ProjectTo<CourseListingServiceModel>()
                .ToListAsync();

        public async Task<IEnumerable<CourseListingServiceModel>> FindAsync(string searchText)
        {
            searchText = searchText ?? string.Empty;

            return await this.db
                .Courses
                .OrderByDescending(c => c.Id)
                .Where(c => c.Name.ToLower().Contains(searchText.ToLower()))
                .ProjectTo<CourseListingServiceModel>()
                .ToListAsync();
        }

        public async Task<TModel> ByIdAsync<TModel>(int id) where TModel : class
            => await this.db
                .Courses
                .Where(c => c.Id == id)
                .ProjectTo<TModel>()
                .FirstOrDefaultAsync();

        public async Task<bool> SignUpStudentAsync(int courseId, string studentId)
        {
            var courseInfo = await this.GetCourseInfo(courseId, studentId);

            if (courseInfo == null 
                || courseInfo.StartDate < DateTime.UtcNow
                || courseInfo.UserIsEnrolled)
            {
                return false;
            }

            var studentInCourse = new StudentCourse
            {
                CourseId = courseId,
                StudentId = studentId
            };

            this.db.Add(studentInCourse);

            await this.db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SignOutStudentAsync(int courseId, string studentId)
        {
            var courseInfo = await this.GetCourseInfo(courseId, studentId);

            if (courseInfo == null
                || courseInfo.StartDate < DateTime.UtcNow
                || !courseInfo.UserIsEnrolled)
            {
                return false;
            }
            
            var studentInCourse = await this.db
                .FindAsync<StudentCourse>(courseId, studentId);

            this.db.Remove(studentInCourse);

            await this.db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> StudentIsEnrolledCourseAsync(int courseId, string studentId)
            => await this.db
                .Courses
                .AnyAsync(c => c.Id == courseId && c.Students.Any(s => s.StudentId == studentId));

        private async Task<CourseWithStudentInfo> GetCourseInfo(int courseId, string studentId)
            => await this.db
                .Courses
                .Where(c => c.Id == courseId)
                .Select(c => new CourseWithStudentInfo
                {
                    StartDate = c.StartDate,
                    UserIsEnrolled = c.Students.Any(s => s.StudentId == studentId)
                })
                .FirstOrDefaultAsync();

        public async Task<bool> SaveExamSubmission(int courseId, string studentId, byte[] examSubmission)
        {
            var studentInCourse = await this.db
                .FindAsync<StudentCourse>(courseId, studentId);

            if (studentInCourse == null)
            {
                return false;
            }

            studentInCourse.ExamSubmission = examSubmission;

            await this.db.SaveChangesAsync();

            return true;
        }
    }
}
