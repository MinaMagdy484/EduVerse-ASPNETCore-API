using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExaminationSystem.Models;
using ExaminationSystem.ViewModels;
using ExaminationSystem.ViewModels.Assignment;
using ExaminationSystem.ViewModels.Attachment;
using ExaminationSystem.ViewModels.Submission;
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
    public class AssignmentController : ControllerBase
    {
        private readonly Context _context;

        public AssignmentController(Context context)
        {
            _context = context;
        }

        // GET: Assignment/GetAllAssignments
        // Returns all assignments
        [HttpGet]
        public async Task<ResultViewModel<IEnumerable<AssignmentResponseModel>>> GetAllAssignments()
        {
            try
            {
                var assignments = await _context.TimeLineItems
                    .Include(t => t.Assignment)
                    .Include(t => t.Assignment.AllowedExtensions)
                    .Include(t => t.Attachments)
                    .Where(t => t.Type == TimeLineItemType.Assignment && !t.IsDeleted)
                    .ToListAsync();

                var assignmentResponses = assignments.Select(t => new AssignmentResponseModel
                {
                    Id = t.Id,
                    CourseID = t.CourseID,
                    UserID = t.UserID,
                    Title = t.Title,
                    Content = t.Content,
                    CreatedAt = t.CreatedAt,
                    Deadline = t.Assignment.Deadline,
                    AllowedExtensions = t.Assignment.AllowedExtensions?.Select(ae => new AllowedExtensionResponseModel
                    {
                        Id = ae.Id,
                        Extension = ae.Extension
                    }).ToList(),
                    Attachments = t.Attachments.Select(a => new AttachmentResponseModel
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        FilePath = a.FilePath,
                        FileType = a.FileType,
                        FileSize = a.FileSize,
                        UploadDate = a.UploadDate
                    }).ToList()
                }).ToList();

                return new ResultViewModel<IEnumerable<AssignmentResponseModel>>
                {
                    IsSuccess = true,
                    Data = assignmentResponses
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<IEnumerable<AssignmentResponseModel>>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving assignments: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // GET: Assignment/GetAllCourseAssignments/{courseId}
        // Returns all assignments for a specific course
        [HttpGet("{courseId}")]
        public async Task<ResultViewModel<IEnumerable<AssignmentResponseModel>>> GetAllCourseAssignments(int courseId)
        {
            try
            {
                var assignments = await _context.TimeLineItems
                    .Include(t => t.Assignment)
                    .Include(t => t.Assignment.AllowedExtensions)
                    .Include(t => t.Attachments)
                    .Where(t => t.Type == TimeLineItemType.Assignment && t.CourseID == courseId && !t.IsDeleted)
                    .ToListAsync();

                var assignmentResponses = assignments.Select(t => new AssignmentResponseModel
                {
                    Id = t.Id,
                    CourseID = t.CourseID,
                    UserID = t.UserID,
                    Title = t.Title,
                    Content = t.Content,
                    CreatedAt = t.CreatedAt,
                    Deadline = t.Assignment.Deadline,
                    AllowedExtensions = t.Assignment.AllowedExtensions?.Select(ae => new AllowedExtensionResponseModel
                    {
                        Id = ae.Id,
                        Extension = ae.Extension
                    }).ToList(),
                    Attachments = t.Attachments.Select(a => new AttachmentResponseModel
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        FilePath = a.FilePath,
                        FileType = a.FileType,
                        FileSize = a.FileSize,
                        UploadDate = a.UploadDate
                    }).ToList()
                }).ToList();

                return new ResultViewModel<IEnumerable<AssignmentResponseModel>>
                {
                    IsSuccess = true,
                    Data = assignmentResponses
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<IEnumerable<AssignmentResponseModel>>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving course assignments: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // GET: Assignment/GetById/{assignmentId}
        // Returns a specific assignment by ID
        [HttpGet("{assignmentId}")]
        public async Task<ResultViewModel<AssignmentResponseModel>> GetById(int assignmentId)
        {
            try
            {
                var timelineItem = await _context.TimeLineItems
                    .Include(t => t.Assignment)
                    .Include(t => t.Assignment.AllowedExtensions)
                    .Include(t => t.Attachments)
                    .FirstOrDefaultAsync(t => t.Id == assignmentId && t.Type == TimeLineItemType.Assignment && !t.IsDeleted);

                if (timelineItem == null)
                {
                    return new ResultViewModel<AssignmentResponseModel>
                    {
                        IsSuccess = false,
                        Message = "Assignment not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                var assignmentResponse = new AssignmentResponseModel
                {
                    Id = timelineItem.Id,
                    CourseID = timelineItem.CourseID,
                    UserID = timelineItem.UserID,
                    Title = timelineItem.Title,
                    Content = timelineItem.Content,
                    CreatedAt = timelineItem.CreatedAt,
                    Deadline = timelineItem.Assignment.Deadline,
                    AllowedExtensions = timelineItem.Assignment.AllowedExtensions?.Select(ae => new AllowedExtensionResponseModel
                    {
                        Id = ae.Id,
                        Extension = ae.Extension
                    }).ToList(),
                    Attachments = timelineItem.Attachments.Select(a => new AttachmentResponseModel
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        FilePath = a.FilePath,
                        FileType = a.FileType,
                        FileSize = a.FileSize,
                        UploadDate = a.UploadDate
                    }).ToList()
                };

                return new ResultViewModel<AssignmentResponseModel>
                {
                    IsSuccess = true,
                    Data = assignmentResponse
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<AssignmentResponseModel>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving assignment: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // POST: Assignment/CreateAssignment
        // Creates a new assignment (for instructors only)
        [Authorize("Instructor")]
        [HttpPost]
        public async Task<ResultViewModel<int>> CreateAssignment(AssignmentCreateRequestModel request)
        {
            try
            {
                // Create TimeLineItem first
                var timelineItem = new TimeLineItem
                {
                    CourseID = request.CourseID,
                    UserID = request.InstructorID,
                    Type = TimeLineItemType.Assignment,
                    Title = request.Title,
                    Content = request.Content,
                    CreatedAt = DateTime.Now,
                    Attachments = new List<Attachment>()
                };

                // Create Assignment connected to TimeLineItem
                var assignment = new Assignment
                {
                    TimeLineItem = timelineItem,
                    Deadline = request.Deadline,
                    AllowedExtensions = new List<AssignmentAllowedExtension>()
                };

                // Add allowed extensions if any
                if (request.AllowedExtensions != null && request.AllowedExtensions.Any())
                {
                    foreach (var ext in request.AllowedExtensions)
                    {
                        var allowedExtension = new AssignmentAllowedExtension
                        {
                            Assignment = assignment,
                            Extension = ext
                        };
                        assignment.AllowedExtensions.Add(allowedExtension);
                    }
                }

                // Add attachments if any
                if (request.Attachments != null && request.Attachments.Any())
                {
                    foreach (var attachment in request.Attachments)
                    {
                        var newAttachment = new Attachment
                        {
                            FileName = attachment.FileName,
                            FilePath = attachment.FilePath,
                            FileType = attachment.FileType,
                            FileSize = attachment.FileSize,
                            UploadDate = DateTime.Now,
                            TimeLineItem = timelineItem
                        };
                        timelineItem.Attachments.Add(newAttachment);
                    }
                }

                // Save TimeLineItem and Assignment
                _context.TimeLineItems.Add(timelineItem);
                _context.Assignments.Add(assignment);
                await _context.SaveChangesAsync();

                return new ResultViewModel<int>
                {
                    IsSuccess = true,
                    Data = timelineItem.Id,
                    Message = "Assignment created successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<int>
                {
                    IsSuccess = false,
                    Message = "Failed to create assignment: " + ex.Message,
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // PUT: Assignment/EditAssignment
        // Updates an existing assignment
        [Authorize("Instructor")]
        [HttpPut]
        public async Task<ResultViewModel<bool>> EditAssignment(AssignmentEditRequestModel request)
        {
            try
            {
                var timelineItem = await _context.TimeLineItems
                    .Include(t => t.Assignment)
                    .Include(t => t.Assignment.AllowedExtensions)
                    .Include(t => t.Attachments)
                    .FirstOrDefaultAsync(t => t.Id == request.Id && t.Type == TimeLineItemType.Assignment && !t.IsDeleted);

                if (timelineItem == null)
                {
                    return new ResultViewModel<bool>
                    {
                        IsSuccess = false,
                        Message = "Assignment not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Check if the instructor is the one who created the assignment
                if (timelineItem.UserID != request.InstructorID)
                {
                    return new ResultViewModel<bool>
                    {
                        IsSuccess = false,
                        Message = "You don't have permission to edit this assignment",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Update TimeLineItem properties
                timelineItem.Title = request.Title;
                timelineItem.Content = request.Content;

                // Update the Assignment entity
                timelineItem.Assignment.Deadline = request.Deadline;

                // Update allowed extensions
                if (request.AllowedExtensions != null)
                {
                    // Remove existing allowed extensions
                    _context.AssignmentAllowedExtensions.RemoveRange(timelineItem.Assignment.AllowedExtensions);
                    timelineItem.Assignment.AllowedExtensions.Clear();

                    // Add new allowed extensions
                    foreach (var ext in request.AllowedExtensions)
                    {
                        var allowedExtension = new AssignmentAllowedExtension
                        {
                            AssignmentID = timelineItem.Assignment.Id,
                            Extension = ext
                        };
                        timelineItem.Assignment.AllowedExtensions.Add(allowedExtension);
                    }
                }

                // Update attachments
                if (request.Attachments != null)
                {
                    // Remove existing attachments
                    _context.Attachments.RemoveRange(timelineItem.Attachments);
                    timelineItem.Attachments.Clear();

                    // Add new attachments
                    foreach (var attachmentInfo in request.Attachments)
                    {
                        var attachment = new Attachment
                        {
                            FileName = attachmentInfo.FileName,
                            FilePath = attachmentInfo.FilePath,
                            FileType = attachmentInfo.FileType,
                            FileSize = attachmentInfo.FileSize,
                            UploadDate = DateTime.Now,
                            TimeLineItemID = timelineItem.Id
                        };
                        timelineItem.Attachments.Add(attachment);
                    }
                }

                await _context.SaveChangesAsync();

                return new ResultViewModel<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Assignment updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<bool>
                {
                    IsSuccess = false,
                    Message = "Failed to update assignment: " + ex.Message,
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // DELETE: Assignment/DeleteAssignment/{assignmentId}
        // Deletes an assignment
        [Authorize("Instructor")]
        [HttpDelete("{assignmentId}")]
        public async Task<ResultViewModel<bool>> DeleteAssignment(int assignmentId)
        {
            try
            {
                var timelineItem = await _context.TimeLineItems
                    .Include(t => t.Assignment)
                    .FirstOrDefaultAsync(t => t.Id == assignmentId && t.Type == TimeLineItemType.Assignment && !t.IsDeleted);

                if (timelineItem == null)
                {
                    return new ResultViewModel<bool>
                    {
                        IsSuccess = false,
                        Message = "Assignment not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Soft delete
                timelineItem.IsDeleted = true;
                if (timelineItem.Assignment != null)
                {
                    timelineItem.Assignment.IsDeleted = true;
                }

                await _context.SaveChangesAsync();

                return new ResultViewModel<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Assignment deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<bool>
                {
                    IsSuccess = false,
                    Message = "Failed to delete assignment: " + ex.Message,
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // POST: Assignment/SubmitAssignment
        // Submit an assignment (for students)
        [Authorize("Student")]
        [HttpPost]
        public async Task<ResultViewModel<int>> SubmitAssignment(AssignmentSubmissionRequestModel request)
        {
            try
            {
                // Check if the assignment exists (using TimelineItemId)
                var timelineItem = await _context.TimeLineItems
                    .Include(t => t.Assignment)
                    .Include(t => t.Assignment.AllowedExtensions)
                    .FirstOrDefaultAsync(t => t.Id == request.TimelineItemId && t.Type == TimeLineItemType.Assignment && !t.IsDeleted);

                if (timelineItem == null)
                {
                    return new ResultViewModel<int>
                    {
                        IsSuccess = false,
                        Message = "Assignment not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Check if deadline has passed
                if (DateTime.Now > timelineItem.Assignment.Deadline)
                {
                    return new ResultViewModel<int>
                    {
                        IsSuccess = false,
                        Message = "Assignment deadline has passed",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Check for any previous submissions by this student
                var existingSubmission = await _context.AssignmentSubmissions
                    .FirstOrDefaultAsync(s => s.AssignmentID == timelineItem.Assignment.Id && s.StudentID == request.StudentId && !s.IsDeleted);

                // If there's a previous submission, update it
                if (existingSubmission != null)
                {
                    existingSubmission.SubmissionDate = DateTime.Now;

                    // Remove old attachments
                    var oldAttachments = await _context.Attachments
                        .Where(a => a.AssignmentSubmissionID == existingSubmission.Id)
                        .ToListAsync();
                    _context.Attachments.RemoveRange(oldAttachments);

                    // Add new attachments
                    if (request.Attachments != null && request.Attachments.Any())
                    {
                        foreach (var attachment in request.Attachments)
                        {
                            var newAttachment = new Attachment
                            {
                                FileName = attachment.FileName,
                                FilePath = attachment.FilePath,
                                FileType = attachment.FileType,
                                FileSize = attachment.FileSize,
                                UploadDate = DateTime.Now,
                                AssignmentSubmissionID = existingSubmission.Id
                            };
                            _context.Attachments.Add(newAttachment);
                        }
                    }

                    await _context.SaveChangesAsync();

                    return new ResultViewModel<int>
                    {
                        IsSuccess = true,
                        Data = existingSubmission.Id,
                        Message = "Assignment submission updated successfully"
                    };
                }

                // Create new submission
                var submission = new AssignmentSubmission
                {
                    AssignmentID = timelineItem.Assignment.Id,
                    StudentID = request.StudentId,
                    SubmissionDate = DateTime.Now,
                    Grade = 0, // Default grade
                    Feedback = "" // Initialize with empty string to avoid NULL constraint violation
                };

                _context.AssignmentSubmissions.Add(submission);
                await _context.SaveChangesAsync();

                // Add attachments if any
                if (request.Attachments != null && request.Attachments.Any())
                {
                    foreach (var attachment in request.Attachments)
                    {
                        // Validate file extension if allowed extensions are specified
                        if (timelineItem.Assignment.AllowedExtensions != null && timelineItem.Assignment.AllowedExtensions.Any())
                        {
                            string fileExtension = System.IO.Path.GetExtension(attachment.FileName).ToLowerInvariant();
                            if (!timelineItem.Assignment.AllowedExtensions.Any(ae => ae.Extension.ToLowerInvariant() == fileExtension))
                            {
                                return new ResultViewModel<int>
                                {
                                    IsSuccess = false,
                                    Message = $"File extension {fileExtension} is not allowed for this assignment",
                                    ErrorCode = ErrorCode.None
                                };
                            }
                        }

                        var newAttachment = new Attachment
                        {
                            FileName = attachment.FileName,
                            FilePath = attachment.FilePath,
                            FileType = attachment.FileType,
                            FileSize = attachment.FileSize,
                            UploadDate = DateTime.Now,
                            AssignmentSubmissionID = submission.Id
                        };
                        _context.Attachments.Add(newAttachment);
                    }
                    await _context.SaveChangesAsync();
                }

                return new ResultViewModel<int>
                {
                    IsSuccess = true,
                    Data = submission.Id,
                    Message = "Assignment submitted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<int>
                {
                    IsSuccess = false,
                    Message = "Failed to submit assignment: " + ex.Message,
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }


        // GET: Assignment/GetSubmissions/{assignmentId}
        // Gets all submissions for an assignment (instructor only)
        [Authorize("Instructor")]
        [HttpGet("Submissions/{assignmentId}")]
        public async Task<ResultViewModel<IEnumerable<SubmissionResponseModel>>> GetSubmissions(int assignmentId)
        {
            try
            {
                // Check if the assignment exists and belongs to the instructor
                var assignment = await _context.Assignments
                    .Include(a => a.TimeLineItem)
                    .FirstOrDefaultAsync(a => a.TimeLineItemID == assignmentId && !a.IsDeleted);

                if (assignment == null)
                {
                    return new ResultViewModel<IEnumerable<SubmissionResponseModel>>
                    {
                        IsSuccess = false,
                        Message = "Assignment not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                var submissions = await _context.AssignmentSubmissions
                    .Include(s => s.Student)
                    .Include(s => s.Attachments)
                    .Where(s => s.AssignmentID == assignment.Id && !s.IsDeleted)
                    .ToListAsync();

                var submissionResponses = submissions.Select(s => new SubmissionResponseModel
                {
                    Id = s.Id,
                    StudentId = s.StudentID,
                    StudentName = $"{s.Student.FirstName} {s.Student.LastName}",
                    SubmissionDate = s.SubmissionDate,
                    Grade = s.Grade,
                    Feedback = s.Feedback,
                    Attachments = s.Attachments.Select(a => new AttachmentResponseModel
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        FilePath = a.FilePath,
                        FileType = a.FileType,
                        FileSize = a.FileSize,
                        UploadDate = a.UploadDate
                    }).ToList()
                }).ToList();

                return new ResultViewModel<IEnumerable<SubmissionResponseModel>>
                {
                    IsSuccess = true,
                    Data = submissionResponses
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<IEnumerable<SubmissionResponseModel>>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving submissions: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // PUT: Assignment/GradeSubmission
        // Grade a student's submission (instructor only)
        [Authorize("Instructor")]
        [HttpPut("Grade")]
        public async Task<ResultViewModel<bool>> GradeSubmission(SubmissionGradeRequestModel request)
        {
            try
            {
                // Use explicit tracking behavior
                var submission = await _context.AssignmentSubmissions
                    .AsTracking()  // Ensure entity is tracked
                    .Include(s => s.Assignment)
                    .Include(s => s.Assignment.TimeLineItem)
                    .FirstOrDefaultAsync(s => s.Id == request.SubmissionId && !s.IsDeleted);

                if (submission == null)
                {
                    return new ResultViewModel<bool>
                    {
                        IsSuccess = false,
                        Message = "Submission not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Check if the instructor is associated with this assignment's course
                var isInstructorAuthorized = await _context.CourseInstructors
                    .AnyAsync(ci => ci.CourseID == submission.Assignment.TimeLineItem.CourseID && ci.InstructorID == request.InstructorId);

                if (!isInstructorAuthorized)
                {
                    return new ResultViewModel<bool>
                    {
                        IsSuccess = false,
                        Message = "You don't have permission to grade this submission",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Update submission with grade and feedback - with explicit property assignment
                submission.Grade = request.Grade;
                submission.Feedback = request.Feedback ?? ""; // Ensure feedback is never null

                // Mark the entity as modified to ensure it's saved
                _context.Entry(submission).State = EntityState.Modified;

                // Save changes with explicit error handling
                var entriesSaved = await _context.SaveChangesAsync();

                if (entriesSaved == 0)
                {
                    return new ResultViewModel<bool>
                    {
                        IsSuccess = false,
                        Message = "Failed to save changes to database. No records were updated.",
                        ErrorCode = ErrorCode.UnKnown
                    };
                }

                return new ResultViewModel<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = $"Submission graded successfully. Record ID: {submission.Id}, Grade: {submission.Grade}"
                };
            }
            catch (DbUpdateException dbEx)
            {
                return new ResultViewModel<bool>
                {
                    IsSuccess = false,
                    Message = $"Database error: {dbEx.Message}. Inner exception: {dbEx.InnerException?.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<bool>
                {
                    IsSuccess = false,
                    Message = $"Failed to grade submission: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }


        // GET: Assignment/GetStudentSubmissions/{courseId}/{studentId}
        // Gets all submissions by a student for a course's assignments
        [HttpGet("StudentSubmissions/{courseId}/{studentId}")]
        public async Task<ResultViewModel<IEnumerable<StudentSubmissionResponseModel>>> GetStudentSubmissions(int courseId, int studentId)
        {
            try
            {
                // Get all assignments for the course
                var assignments = await _context.TimeLineItems
                    .Include(t => t.Assignment)
                    .Where(t => t.Type == TimeLineItemType.Assignment && t.CourseID == courseId && !t.IsDeleted)
                    .ToListAsync();

                var result = new List<StudentSubmissionResponseModel>();

                foreach (var assignment in assignments)
                {
                    // Find submission for this assignment by this student
                    var submission = await _context.AssignmentSubmissions
                        .Include(s => s.Attachments)
                        .FirstOrDefaultAsync(s => s.AssignmentID == assignment.Assignment.Id && s.StudentID == studentId && !s.IsDeleted);

                    var submissionResponse = new StudentSubmissionResponseModel
                    {
                        AssignmentId = assignment.Id,
                        AssignmentTitle = assignment.Title,
                        Deadline = assignment.Assignment.Deadline,
                        IsSubmitted = submission != null,
                        SubmissionId = submission?.Id ?? 0,
                        SubmissionDate = submission?.SubmissionDate,
                        Grade = submission?.Grade ?? 0,
                        Feedback = submission?.Feedback,
                        Attachments = submission?.Attachments.Select(a => new AttachmentResponseModel
                        {
                            Id = a.Id,
                            FileName = a.FileName,
                            FilePath = a.FilePath,
                            FileType = a.FileType,
                            FileSize = a.FileSize,
                            UploadDate = a.UploadDate
                        }).ToList() ?? new List<AttachmentResponseModel>()
                    };

                    result.Add(submissionResponse);
                }

                return new ResultViewModel<IEnumerable<StudentSubmissionResponseModel>>
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<IEnumerable<StudentSubmissionResponseModel>>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving student submissions: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }
    }
}
