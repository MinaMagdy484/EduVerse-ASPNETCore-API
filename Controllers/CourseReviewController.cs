using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExaminationSystem.Models;
using ExaminationSystem.ViewModels;
using ExaminationSystem.ViewModels.CourseReview;
using ExaminationSystem.Data;
using ExaminationSystem.Enums;
using ExaminationSystem.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CourseReviewController : ControllerBase
    {
        private readonly Context _context;

        public CourseReviewController(Context context)
        {
            _context = context;
        }

                // POST: CourseReview/CreateReview
        // Creates a new course review (for students only)
        [Authorize("Student")]
        [HttpPost]
        public async Task<ResultViewModel<int>> CreateReview(CourseReviewCreateRequestModel request)
        {
            try
            {
                // Check if the course exists
                var course = await _context.Courses
                    .FirstOrDefaultAsync(c => c.Id == request.CourseID && !c.IsDeleted);

                if (course == null)
                {
                    return new ResultViewModel<int>
                    {
                        IsSuccess = false,
                        Message = "Course not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Check if the student is enrolled in the course
                var enrollment = await _context.CourseStudents
                    .FirstOrDefaultAsync(cs => cs.CourseID == request.CourseID && cs.StudentID == request.StudentID && !cs.IsDeleted);

                if (enrollment == null)
                {
                    return new ResultViewModel<int>
                    {
                        IsSuccess = false,
                        Message = "Student is not enrolled in this course",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Check if the student has already reviewed this course
                var existingReview = await _context.CourseReviews
                    .FirstOrDefaultAsync(cr => cr.CourseID == request.CourseID && cr.StudentID == request.StudentID && !cr.IsDeleted);

                if (existingReview != null)
                {
                    return new ResultViewModel<int>
                    {
                        IsSuccess = false,
                        Message = "You have already reviewed this course",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Create the review
                var review = new CourseReview
                {
                    CourseID = request.CourseID,
                    StudentID = request.StudentID,
                    Rating = request.Rating,
                    Comment = request.Comment ?? string.Empty,
                    CreatedAt = DateTime.UtcNow
                };

                _context.CourseReviews.Add(review);
                await _context.SaveChangesAsync();

                return new ResultViewModel<int>
                {
                    IsSuccess = true,
                    Data = review.Id,
                    Message = "Review created successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<int>
                {
                    IsSuccess = false,
                    Message = "Failed to create review: " + ex.Message,
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // PUT: CourseReview/EditReview
        // Updates an existing course review (for students only)
        [Authorize("Student")]
        [HttpPut]
        public async Task<ResultViewModel<bool>> EditReview(CourseReviewEditRequestModel request)
        {
            try
            {
                var review = await _context.CourseReviews
                    .FirstOrDefaultAsync(cr => cr.Id == request.Id && !cr.IsDeleted);

                if (review == null)
                {
                    return new ResultViewModel<bool>
                    {
                        IsSuccess = false,
                        Message = "Review not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Check if the student is the one who created the review
                if (review.StudentID != request.StudentID)
                {
                    return new ResultViewModel<bool>
                    {
                        IsSuccess = false,
                        Message = "You don't have permission to edit this review",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Update review properties
                review.Rating = request.Rating;
                review.Comment = request.Comment ?? string.Empty;

                await _context.SaveChangesAsync();

                return new ResultViewModel<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Review updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<bool>
                {
                    IsSuccess = false,
                    Message = "Failed to update review: " + ex.Message,
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // DELETE: CourseReview/DeleteReview/{reviewId}
        // Deletes a course review (for students only)
        [Authorize("Student")]
        [HttpDelete("{reviewId}")]
        public async Task<ResultViewModel<bool>> DeleteReview(int reviewId)
        {
            try
            {
                var review = await _context.CourseReviews
                    .FirstOrDefaultAsync(cr => cr.Id == reviewId && !cr.IsDeleted);

                if (review == null)
                {
                    return new ResultViewModel<bool>
                    {
                        IsSuccess = false,
                        Message = "Review not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Soft delete
                review.IsDeleted = true;
                await _context.SaveChangesAsync();

                return new ResultViewModel<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Review deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<bool>
                {
                    IsSuccess = false,
                    Message = "Failed to delete review: " + ex.Message,
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }



        // GET: CourseReview/GetAllReviews
        // Returns all course reviews
        [HttpGet]
        public async Task<ResultViewModel<IEnumerable<CourseReviewResponseModel>>> GetAllReviews()
        {
            try
            {                // Use manual joins with proper user hierarchy approach
                var reviewResponses = await (from cr in _context.CourseReviews
                                            join c in _context.Courses on cr.CourseID equals c.Id into courseJoin
                                            from c in courseJoin.DefaultIfEmpty()
                                            join u in _context.Users on cr.StudentID equals u.Id into userJoin
                                            from u in userJoin.DefaultIfEmpty()
                                            where !cr.IsDeleted
                                            orderby cr.CreatedAt descending
                                            select new CourseReviewResponseModel
                                            {
                                                Id = cr.Id,
                                                CourseID = cr.CourseID,
                                                CourseName = c != null ? c.Name : "Unknown Course",
                                                StudentID = cr.StudentID,
                                                StudentName = u != null ? $"{u.FirstName} {u.LastName}" : "Unknown Student",
                                                Rating = cr.Rating,
                                                Comment = cr.Comment ?? string.Empty,
                                                CreatedAt = cr.CreatedAt
                                            }).ToListAsync();

                if (!reviewResponses.Any())
                {
                    return new ResultViewModel<IEnumerable<CourseReviewResponseModel>>
                    {
                        IsSuccess = true,
                        Data = new List<CourseReviewResponseModel>(),
                        Message = "No reviews found"
                    };
                }

                return new ResultViewModel<IEnumerable<CourseReviewResponseModel>>
                {
                    IsSuccess = true,
                    Data = reviewResponses
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<IEnumerable<CourseReviewResponseModel>>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving reviews: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }        // GET: CourseReview/GetAllCourseReviews/{courseId}
        // Returns all reviews for a specific course
        [HttpGet("{courseId}")]
        public async Task<ResultViewModel<IEnumerable<CourseReviewResponseModel>>> GetAllCourseReviews(int courseId)
        {
            try
            {
                // First, check if the course exists
                var courseExists = await _context.Courses
                    .AnyAsync(c => c.Id == courseId && !c.IsDeleted);

                if (!courseExists)
                {
                    return new ResultViewModel<IEnumerable<CourseReviewResponseModel>>
                    {
                        IsSuccess = false,
                        Message = "Course not found",
                        ErrorCode = ErrorCode.None
                    };
                }                // Use manual joins with proper user hierarchy approach
                var reviewResponses = await (from cr in _context.CourseReviews
                                            join c in _context.Courses on cr.CourseID equals c.Id into courseJoin
                                            from c in courseJoin.DefaultIfEmpty()
                                            join u in _context.Users on cr.StudentID equals u.Id into userJoin
                                            from u in userJoin.DefaultIfEmpty()
                                            where cr.CourseID == courseId && !cr.IsDeleted
                                            orderby cr.CreatedAt descending
                                            select new CourseReviewResponseModel
                                            {
                                                Id = cr.Id,
                                                CourseID = cr.CourseID,
                                                CourseName = c != null ? c.Name : "Unknown Course",
                                                StudentID = cr.StudentID,
                                                StudentName = u != null ? $"{u.FirstName} {u.LastName}" : "Unknown Student",
                                                Rating = cr.Rating,
                                                Comment = cr.Comment ?? string.Empty,
                                                CreatedAt = cr.CreatedAt
                                            }).ToListAsync();return new ResultViewModel<IEnumerable<CourseReviewResponseModel>>
                {
                    IsSuccess = true,
                    Data = reviewResponses,
                    Message = reviewResponses.Any() ? string.Empty : "No reviews found for this course"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<IEnumerable<CourseReviewResponseModel>>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving course reviews: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }        // GET: CourseReview/GetById/{reviewId}
        // Returns a specific review by ID
        [HttpGet("{reviewId}")]
        public async Task<ResultViewModel<CourseReviewResponseModel>> GetById(int reviewId)
        {
            try
            {                // Use manual join with proper user hierarchy approach
                var reviewResponse = await (from cr in _context.CourseReviews
                                           join c in _context.Courses on cr.CourseID equals c.Id into courseJoin
                                           from c in courseJoin.DefaultIfEmpty()
                                           join u in _context.Users on cr.StudentID equals u.Id into userJoin
                                           from u in userJoin.DefaultIfEmpty()
                                           where cr.Id == reviewId && !cr.IsDeleted
                                           select new CourseReviewResponseModel
                                           {
                                               Id = cr.Id,
                                               CourseID = cr.CourseID,
                                               CourseName = c != null ? c.Name : "Unknown Course",
                                               StudentID = cr.StudentID,
                                               StudentName = u != null ? $"{u.FirstName} {u.LastName}" : "Unknown Student",
                                               Rating = cr.Rating,
                                               Comment = cr.Comment ?? string.Empty,
                                               CreatedAt = cr.CreatedAt
                                           }).FirstOrDefaultAsync();

                if (reviewResponse == null)
                {
                    return new ResultViewModel<CourseReviewResponseModel>
                    {
                        IsSuccess = false,
                        Message = "Review not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                return new ResultViewModel<CourseReviewResponseModel>
                {
                    IsSuccess = true,
                    Data = reviewResponse
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<CourseReviewResponseModel>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving review: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }



        

        // GET: CourseReview/GetCourseStatistics/{courseId}
        // Gets comprehensive statistics for a specific course
        [HttpGet("{courseId}")]
        public async Task<ResultViewModel<object>> GetCourseStatistics(int courseId)
        {
            try
            {
                // First, check if the course exists
                var course = await _context.Courses
                    .FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted);

                if (course == null)
                {
                    return new ResultViewModel<object>
                    {
                        IsSuccess = false,
                        Message = "Course not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Get review statistics
                var reviews = await _context.CourseReviews
                    .Where(cr => cr.CourseID == courseId && !cr.IsDeleted)
                    .ToListAsync();

                var totalReviews = reviews.Count;
                var averageRating = totalReviews > 0 ? Math.Round(reviews.Average(r => r.Rating), 2) : 0.0;

                // Rating distribution (1-5 stars)
                var ratingDistribution = new Dictionary<int, int>();
                for (int i = 1; i <= 5; i++)
                {
                    ratingDistribution[i] = reviews.Count(r => r.Rating == i);
                }

                // Get enrollment statistics
                var totalEnrollments = await _context.CourseStudents
                    .CountAsync(cs => cs.CourseID == courseId && !cs.IsDeleted);

                // Get instructor information
                var instructors = await (from ci in _context.CourseInstructors
                                        join u in _context.Users on ci.InstructorID equals u.Id
                                        where ci.CourseID == courseId && !ci.IsDeleted && !u.IsDeleted
                                        select new
                                        {
                                            Id = u.Id,
                                            Name = $"{u.FirstName} {u.LastName}"
                                        }).ToListAsync();

                // Get recent review activity (last 30 days)
                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
                var recentReviews = reviews.Count(r => r.CreatedAt >= thirtyDaysAgo);

                // Calculate review completion rate (students who reviewed vs enrolled)
                var reviewCompletionRate = totalEnrollments > 0 
                    ? Math.Round((double)totalReviews / totalEnrollments * 100, 2) 
                    : 0.0;

                // Get most recent reviews for preview
                var recentReviewPreviews = await (from cr in _context.CourseReviews
                                                 join u in _context.Users on cr.StudentID equals u.Id
                                                 where cr.CourseID == courseId && !cr.IsDeleted && !u.IsDeleted
                                                 orderby cr.CreatedAt descending
                                                 select new
                                                 {
                                                     StudentName = $"{u.FirstName} {u.LastName}",
                                                     Rating = cr.Rating,
                                                     Comment = cr.Comment != null && cr.Comment.Length > 100 
                                                              ? cr.Comment.Substring(0, 100) + "..." 
                                                              : cr.Comment ?? "",
                                                     CreatedAt = cr.CreatedAt
                                                 }).Take(5).ToListAsync();

                var statistics = new
                {
                    CourseInfo = new
                    {
                        Id = course.Id,
                        Name = course.Name,
                        Code = course.Code,
                        Description = course.Description,
                        StartDate = course.StartDate,
                        EndDate = course.EndDate,
                        CreditHours = course.CreditHours
                    },
                    ReviewStatistics = new
                    {
                        TotalReviews = totalReviews,
                        AverageRating = averageRating,
                        RatingDistribution = ratingDistribution,
                        RecentReviews = recentReviews,
                        ReviewCompletionRate = reviewCompletionRate
                    },
                    EnrollmentStatistics = new
                    {
                        TotalEnrollments = totalEnrollments,
                        InstructorCount = instructors.Count,
                        Instructors = instructors
                    },
                    RecentActivity = new
                    {
                        RecentReviewPreviews = recentReviewPreviews
                    },
                    GeneratedAt = DateTime.UtcNow
                };

                return new ResultViewModel<object>
                {
                    IsSuccess = true,
                    Data = statistics
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<object>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving course statistics: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }        // GET: CourseReview/GetTopRatedCourses
        // Returns top-rated courses based on average rating (minimum reviews required)
        [HttpGet]
        public async Task<ResultViewModel<object>> GetTopRatedCourses(int limit = 10, int minReviews = 3)
        {
            try
            {
                // Step 1: Get course review statistics using a simpler approach
                var courseReviewStats = await (from cr in _context.CourseReviews
                                             where !cr.IsDeleted
                                             group cr by cr.CourseID into g
                                             where g.Count() >= minReviews
                                             select new
                                             {
                                                 CourseId = g.Key,
                                                 ReviewCount = g.Count(),
                                                 AverageRating = g.Average(r => r.Rating)
                                             })
                                             .OrderByDescending(x => x.AverageRating)
                                             .ThenByDescending(x => x.ReviewCount)
                                             .Take(limit)
                                             .ToListAsync();

                if (!courseReviewStats.Any())
                {
                    return new ResultViewModel<object>
                    {
                        IsSuccess = true,
                        Data = new
                        {
                            TopRatedCourses = new List<object>(),
                            Message = $"No courses found with at least {minReviews} reviews",
                            Criteria = new { Limit = limit, MinReviews = minReviews }
                        },
                        Message = $"No courses found with at least {minReviews} reviews"
                    };
                }

                // Step 2: Get course details for the top-rated courses
                var courseIds = courseReviewStats.Select(x => x.CourseId).ToList();
                var courses = await _context.Courses
                    .Where(c => courseIds.Contains(c.Id) && !c.IsDeleted)
                    .ToListAsync();

                // Step 3: Get enrollment counts
                var enrollmentCounts = await (from cs in _context.CourseStudents
                                            where courseIds.Contains(cs.CourseID) && !cs.IsDeleted
                                            group cs by cs.CourseID into g
                                            select new
                                            {
                                                CourseId = g.Key,
                                                Count = g.Count()
                                            }).ToListAsync();

                // Step 4: Get instructor information
                var instructorsByCourse = await (from ci in _context.CourseInstructors
                                               join u in _context.Users on ci.InstructorID equals u.Id
                                               where courseIds.Contains(ci.CourseID) && !ci.IsDeleted && !u.IsDeleted
                                               select new
                                               {
                                                   CourseId = ci.CourseID,
                                                   InstructorId = u.Id,
                                                   InstructorName = $"{u.FirstName} {u.LastName}"
                                               }).ToListAsync();

                // Step 5: Combine all data
                var result = courseReviewStats.Select(stats =>
                {
                    var course = courses.FirstOrDefault(c => c.Id == stats.CourseId);
                    var enrollmentCount = enrollmentCounts.FirstOrDefault(ec => ec.CourseId == stats.CourseId)?.Count ?? 0;
                    var courseInstructors = instructorsByCourse
                        .Where(i => i.CourseId == stats.CourseId)
                        .Select(i => new { i.InstructorId, i.InstructorName })
                        .ToList();

                    return new
                    {
                        CourseId = stats.CourseId,
                        CourseName = course?.Name ?? "Unknown Course",
                        CourseCode = course?.Code ?? "",
                        Description = course?.Description ?? "",
                        CreditHours = course?.CreditHours ?? 0,
                        StartDate = course?.StartDate,
                        EndDate = course?.EndDate,
                        AverageRating = Math.Round(stats.AverageRating, 2),
                        TotalReviews = stats.ReviewCount,
                        TotalEnrollments = enrollmentCount,
                        Instructors = courseInstructors,
                        ReviewCompletionRate = enrollmentCount > 0 
                            ? Math.Round((double)stats.ReviewCount / enrollmentCount * 100, 2) 
                            : 0.0
                    };
                }).ToList();

                var response = new
                {
                    TopRatedCourses = result,
                    TotalFound = result.Count,
                    Criteria = new 
                    { 
                        Limit = limit, 
                        MinReviews = minReviews 
                    },
                    GeneratedAt = DateTime.UtcNow
                };

                return new ResultViewModel<object>
                {
                    IsSuccess = true,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<object>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving top-rated courses: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }
    }
}
