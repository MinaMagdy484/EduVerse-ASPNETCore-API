using ExaminationSystem.DTO.Course;

namespace ExaminationSystem.Mediators.Courses
{
    public interface ICourseMediator
    {
        Task<CourseDTO> GetById(int id);
        Task<IEnumerable<CourseDTO>> GetAll();
        Task<int> AddCourse(CourseCreateDTO courseDTO);
        Task EditCourse(CourseEditDTO courseEditDTO);
        Task DeleteCourse(int id);
        Task<int> AssignStudentToCourse(CourseStudentDTO courseStudentDTO);
    }
}
