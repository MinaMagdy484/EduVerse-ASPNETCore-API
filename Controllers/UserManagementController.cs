using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ExaminationSystem.Models;
using ExaminationSystem.ViewModels;
using ExaminationSystem.Data;
using ExaminationSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExaminationSystem.Exceptions;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserManagementController : ControllerBase
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;

        public UserManagementController(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserManagement/GetUserDetails/{userId}
        // Returns details of a specific user
        [HttpGet("{userId}")]
        public async Task<ResultViewModel<UserDetailsResponseModel>> GetUserDetails(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.Id == userId && !u.IsDeleted)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return new ResultViewModel<UserDetailsResponseModel>
                    {
                        IsSuccess = false,
                        Message = "User not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                var userDetails = new UserDetailsResponseModel
                {
                    Id = user.Id,
                    Name = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    ImageUrl = user.ImageURL,
                    Roles = roles.ToList()
                };

                return new ResultViewModel<UserDetailsResponseModel>
                {
                    IsSuccess = true,
                    Data = userDetails
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<UserDetailsResponseModel>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving user details: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // GET: UserManagement/GetCourseMembersDetails/{courseId}
        // Returns details of all members in a specific course
        [HttpGet("{courseId}")]
        public async Task<ResultViewModel<List<CourseMemberDetailsResponseModel>>> GetCourseMembersDetails(int courseId)
        {
            try
            {
                // Check if course exists
                var course = await _context.Courses
                    .FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted);

                if (course == null)
                {
                    return new ResultViewModel<List<CourseMemberDetailsResponseModel>>
                    {
                        IsSuccess = false,
                        Message = "Course not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Get all instructors for this course
                var instructorIds = await _context.CourseInstructors
                    .Where(ci => ci.CourseID == courseId && !ci.IsDeleted)
                    .Select(ci => ci.InstructorID)
                    .ToListAsync();

                var instructors = await _context.Users
                    .Where(u => instructorIds.Contains(u.Id) && !u.IsDeleted)
                    .ToListAsync();

                // Get all students for this course
                var studentIds = await _context.CourseStudents
                    .Where(cs => cs.CourseID == courseId && !cs.IsDeleted)
                    .Select(cs => cs.StudentID)
                    .ToListAsync();

                var students = await _context.Users
                    .Where(u => studentIds.Contains(u.Id) && !u.IsDeleted)
                    .ToListAsync();

                // Combine instructors and students into a single list of members
                var members = new List<CourseMemberDetailsResponseModel>();

                // Add instructors
                foreach (var instructor in instructors)
                {
                    members.Add(new CourseMemberDetailsResponseModel
                    {
                        UserId = instructor.Id,
                        Name = $"{instructor.FirstName} {instructor.LastName}",
                        Email = instructor.Email,
                        ImageUrl = instructor.ImageURL,
                        Role = "Instructor"
                    });
                }

                // Add students
                foreach (var student in students)
                {
                    members.Add(new CourseMemberDetailsResponseModel
                    {
                        UserId = student.Id,
                        Name = $"{student.FirstName} {student.LastName}",
                        Email = student.Email,
                        ImageUrl = student.ImageURL,
                        Role = "Student"
                    });
                }

                return new ResultViewModel<List<CourseMemberDetailsResponseModel>>
                {
                    IsSuccess = true,
                    Data = members
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<List<CourseMemberDetailsResponseModel>>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving course members: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }
    }

    // Response models for the UserManagement controller
    public class UserDetailsResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public List<string> Roles { get; set; }
    }

    public class CourseMemberDetailsResponseModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public string Role { get; set; }
    }
}
