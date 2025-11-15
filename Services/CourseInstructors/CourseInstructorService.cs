using ExaminationSystem.DTO.Course;
using ExaminationSystem.Helpers;
using ExaminationSystem.Models;
using ExaminationSystem.Repositories.Bases;
using ExaminationSystem.Services.Accounts;
using System.Linq.Expressions;

namespace ExaminationSystem.Services.CourseInstructors
{
    public class CourseInstructorService : ICourseInstructorService
    {
        private readonly IRepository<CourseInstructor> _repository;
        private readonly CurrentUserService _currentUser;
        public CourseInstructorService(IRepository<CourseInstructor> repository,CurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUser = currentUserService;
        }

        public async Task<IEnumerable<CourseInstructorDTO>> Get(Expression<Func<CourseInstructor, bool>> predicate)
        {
            IEnumerable<CourseInstructor> courseInstructors = (await _repository.GetAllAsync(predicate)).ToList();
            return courseInstructors.AsQueryable().Map<CourseInstructorDTO>();
        }

        public async Task Add(CourseInstructorDTO courseInstructorDTO, int InstructorId)
        {
            // var courseInstructor = courseInstructorDTO.MapOne<CourseInstructor>();
           var courseInstructor =  new CourseInstructor
            {
                CourseID = courseInstructorDTO.CourseID,
                InstructorID = InstructorId,
            };
            await _repository.AddAsync(courseInstructor);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteRange(IEnumerable<CourseInstructorDTO> courseInstructorDTOs)
        {
            var courseInstructors = courseInstructorDTOs.AsQueryable().Map<CourseInstructor>();
            _repository.DeleteRange(courseInstructors);
            await _repository.SaveChangesAsync();
        }
    }
}
