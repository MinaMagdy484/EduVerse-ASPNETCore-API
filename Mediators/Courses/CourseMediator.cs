using ExaminationSystem.DTO.Course;
using ExaminationSystem.Services.Accounts;
using ExaminationSystem.Services.CourseInstructors;
using ExaminationSystem.Services.Courses;
using ExaminationSystem.Services.CourseStudents;
using ExaminationSystem.Services.Departments;
using ExaminationSystem.Services.Exams;

namespace ExaminationSystem.Mediators.Courses
{
    public class CourseMediator : ICourseMediator
    {
        private readonly ICourseService _courseService;
        private readonly ICourseInstructorService _courseInstructorService;
        private readonly ICourseStudentService _courseStudentService;
        private readonly IDepartmentService _departmentService;
        private readonly IExamService _examService;
        private readonly CurrentUserService _currentUser;

        public CourseMediator(ICourseService courseService,
            ICourseInstructorService courseInstructorService,
            ICourseStudentService courseStudentService,
            IDepartmentService departmentService,
            IExamService examService,
            CurrentUserService currentUser)
        {
            _courseService = courseService;
            _courseInstructorService = courseInstructorService;
            _courseStudentService = courseStudentService;
            _departmentService = departmentService;
            _examService = examService;
            _currentUser = currentUser;
        }

        public async Task<int> AddCourse(CourseCreateDTO courseDTO)
        {
            int courseId = await _courseService.Add(courseDTO);
            int InstructorId = int.Parse(_currentUser.UserId);

            // Add course-instructor relationships
            if (InstructorId != null)
            {
                
                    var courseInstructorDTO = new CourseInstructorDTO
                    {
                        CourseID = courseId,
                       // InstructorID = instructorId
                    };
                    await _courseInstructorService.Add(courseInstructorDTO, InstructorId);
                
            }
            
            return courseId;
        }

        public async Task EditCourse(CourseEditDTO courseEditDTO)
        {
            await _courseService.Update(courseEditDTO);
        }


        public async Task DeleteCourse(int id)
        {
            var course = await _courseService.GetByID(id);

            if (course != null)
            {
                await _courseService.Delete(id);

                var courseInstructors = await _courseInstructorService.Get(ci => ci.CourseID == id);

                if (courseInstructors != null)
                {
                    await _courseInstructorService.DeleteRange(courseInstructors);
                }

                var courseStudents = await _courseStudentService.Get(ci => ci.CourseID == id);

                if (courseStudents != null)
                {
                    await _courseStudentService.DeleteRange(courseStudents);
                }
            }
        }

        public async Task<IEnumerable<CourseDTO>> GetAll()
        {
            return await _courseService.GetAll();
        }

        public async Task<CourseDTO> GetById(int id)
        {
            return await _courseService.GetByID(id);
        }

        public async Task<int> AssignStudentToCourse(CourseStudentDTO courseStudentDTO)
        {
            int id = await _courseStudentService.Add(courseStudentDTO);
            return id;
        }
    }
}
