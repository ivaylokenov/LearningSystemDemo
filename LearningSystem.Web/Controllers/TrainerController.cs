namespace LearningSystem.Web.Controllers
{
    using Data.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models.Trainer;
    using Services;
    using Services.Models;
    using System;
    using System.Threading.Tasks;

    [Authorize(Roles = WebConstants.TrainerRole)]
    public class TrainerController : Controller
    {
        private readonly ITrainerService trainers;
        private readonly ICourseService courses;
        private readonly UserManager<User> userManager;

        public TrainerController(
            ITrainerService trainers,
            ICourseService courses,
            UserManager<User> userManager)
        {
            this.trainers = trainers;
            this.userManager = userManager;
            this.courses = courses;
        }

        public async Task<IActionResult> Courses()
        {
            var userId = this.userManager.GetUserId(User);
            var courses = await this.trainers.CoursesAsync(userId);

            return View(courses);
        }

        public async Task<IActionResult> Students(int id)
        {
            var userId = this.userManager.GetUserId(User);
            if (!await this.trainers.IsTrainer(id, userId))
            {
                return NotFound();
            }
            
            return View(new StudentsInCourseViewModel
            {
                Students = await this.trainers.StudentsInCourseAsync(id),
                Course = await this.courses.ByIdAsync<CourseListingServiceModel>(id)
            });
        }

        [HttpPost]
        public async Task<IActionResult> GradeStudent(int id, string studentId, Grade grade)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return BadRequest();
            }

            var userId = this.userManager.GetUserId(User);
            if (!await this.trainers.IsTrainer(id, userId))
            {
                return BadRequest();
            }

            var success = await this.trainers.AddGradeAsync(id, studentId, grade);

            if (!success)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Students), new { id });
        }

        public async Task<IActionResult> DownloadExam(int id, string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return BadRequest();
            }

            var userId = this.userManager.GetUserId(User);
            if (!await this.trainers.IsTrainer(id, userId))
            {
                return BadRequest();
            }

            var examContents = await this.trainers.GetExamSubmissionAsync(id, studentId);

            if (examContents == null)
            {
                return BadRequest();
            }

            var studentInCourseNames = await this.trainers
                .StudentInCourseNamesAsync(id, studentId);

            if (studentInCourseNames == null)
            {
                return BadRequest();
            }
            
            return File(
                examContents, 
                "application/zip", 
                $"{studentInCourseNames.CourseName}-{studentInCourseNames.Username}-{DateTime.UtcNow.ToString("MM-DD-yyyy")}.zip");
        }
    }
}
