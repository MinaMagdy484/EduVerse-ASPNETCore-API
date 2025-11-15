using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExaminationSystem.DTO.Course;
using ExaminationSystem.Helpers;
using ExaminationSystem.Mediators.Courses;
using ExaminationSystem.Models;
using ExaminationSystem.Services.Accounts;
using ExaminationSystem.Services.Courses;
using ExaminationSystem.ViewModels;
using ExaminationSystem.ViewModels.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseMediator _courseMediator;
        private readonly CurrentUserService _currentUser;

        public CourseController(ICourseMediator courseMediator, CurrentUserService currentUser)
        {
            _courseMediator = courseMediator;
            _currentUser = currentUser;

        }

        [HttpGet]
        public async Task<ResultViewModel<IEnumerable<CourseViewModel>>> GetAllCourses()
        {
            var courses = (await _courseMediator.GetAll()).AsQueryable().Map<CourseViewModel>();

            return new ResultViewModel<IEnumerable<CourseViewModel>>
            {
                IsSuccess = true,
                Data = courses,
            };
        }

        [HttpGet("{id}")]
        public async Task<ResultViewModel<CourseViewModel>> GetCourseByID(int id)
        {
            var course = (await _courseMediator.GetById(id)).MapOne<CourseViewModel>();

            return new ResultViewModel<CourseViewModel>
            {
                IsSuccess = true,
                Data = course,
            };
        }

        [Authorize("Instructor")]
        [HttpPost]
        public async Task<ResultViewModel<int>> CreateCourse(CourseCreateDTO courseVM)
        {
            var courseDTO = courseVM.MapOne<CourseCreateDTO>();
            int courseId =  await _courseMediator.AddCourse(courseDTO);
            
            return new ResultViewModel<int>
            {
                IsSuccess = true,
                Data = courseId,
            };
        }

        [Authorize("Instructor")]
        [HttpPut]
        public async Task<ResultViewModel<bool>> EditCourse(CourseEditDTO courseEditDTO)
        {
            //var courseDTO = courseVM.MapOne<CourseDTO>();
            await _courseMediator.EditCourse(courseEditDTO);

            return new ResultViewModel<bool>
            {
                IsSuccess = true
            };
        }

        [Authorize("Instructor")]
        [HttpDelete]
        public async Task<ResultViewModel<bool>> DeleteCourse(int id) 
        {
            await _courseMediator.DeleteCourse(id);
            
            return new ResultViewModel<bool>
            {
                IsSuccess = true
            };
        }

        [Authorize("Student")]
        [HttpPost]
        public async Task<IActionResult> EnrollStudent(CourseEnrolmentDTO courseEnrollmenttDTO)
            {
            if (!int.TryParse(_currentUser.UserId, out int studentId))
                return BadRequest("Invalid or missing UserId in token.");
            
            CourseStudentDTO courseStudentDTO = new CourseStudentDTO
            {
                CourseID = courseEnrollmenttDTO.CourseID,

                StudentID = studentId

            };
            int id = await _courseMediator.AssignStudentToCourse(courseStudentDTO);
            ResultViewModel<int> result = new ResultViewModel<int>
            {
                IsSuccess = true,
                Data = id
            };
            return Ok(result);
        }
    }
}
