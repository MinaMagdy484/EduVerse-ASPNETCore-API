using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExaminationSystem.Models;
using ExaminationSystem.ViewModels;
using ExaminationSystem.ViewModels.Post;
using ExaminationSystem.ViewModels.Assignment;
using ExaminationSystem.ViewModels.Comment;
using ExaminationSystem.ViewModels.Attachment;
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
    public class TimelineController : ControllerBase
    {
        private readonly Context _context;

        public TimelineController(Context context)
        {
            _context = context;
        }

        // GET: Timeline/GetCourseTimeline/{courseId}
        // Returns all timeline items for a course (posts and assignments) with attachments and comments
        [HttpGet("{courseId}")]
        public async Task<ResultViewModel<IEnumerable<TimelineItemResponseModel>>> GetCourseTimeline(int courseId)
        {
            try
            {
                var timelineItems = await _context.TimeLineItems
                    .Include(t => t.Post)
                    .Include(t => t.Assignment)
                    .Include(t => t.Assignment.AllowedExtensions)
                    .Include(t => t.Attachments)
                    .Include(t => t.Comments)
                    .ThenInclude(c => c.CommentedBy)
                    .Where(t => t.CourseID == courseId && !t.IsDeleted)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                var timelineResponseItems = new List<TimelineItemResponseModel>();

                foreach (var item in timelineItems)
                {
                    var timelineResponse = new TimelineItemResponseModel
                    {
                        Id = item.Id,
                        CourseID = item.CourseID,
                        UserID = item.UserID,
                        Title = item.Title,
                        Content = item.Content,
                        CreatedAt = item.CreatedAt,
                        Type = item.Type,
                        Attachments = item.Attachments.Select(a => new AttachmentResponseModel
                        {
                            Id = a.Id,
                            FileName = a.FileName,
                            FilePath = a.FilePath,
                            FileType = a.FileType,
                            FileSize = a.FileSize,
                            UploadDate = a.UploadDate
                        }).ToList(),
                        Comments = item.Comments.Where(c => !c.IsDeleted).Select(c => new CommentResponseModel
                        {
                            Id = c.Id,
                            Content = c.Content,
                            CommentedById = c.CommentedByID,
                            CommentedByName = $"{c.CommentedBy.FirstName} {c.CommentedBy.LastName}",
                            CommentedByImage = c.CommentedBy.ImageURL,
                            Time = c.Time,
                            TimelineItemId = c.TimeLineItemID,
                            TimelineItemTitle = item.Title
                        }).OrderBy(c => c.Time).ToList()
                    };

                    // Add additional properties based on the type
                    if (item.Type == TimeLineItemType.Assignment && item.Assignment != null)
                    {
                        timelineResponse.AssignmentDetails = new AssignmentDetailsModel
                        {
                            Deadline = item.Assignment.Deadline,
                            AllowedExtensions = item.Assignment.AllowedExtensions?.Select(ae => new AllowedExtensionResponseModel
                            {
                                Id = ae.Id,
                                Extension = ae.Extension
                            }).ToList()
                        };
                    }
                    else if (item.Type == TimeLineItemType.Post && item.Post != null)
                    {
                        // You could add post-specific details if needed in the future
                        timelineResponse.PostDetails = new PostDetailsModel
                        {
                            // Currently, posts don't have special properties beyond the timeline item
                        };
                    }

                    timelineResponseItems.Add(timelineResponse);
                }

                return new ResultViewModel<IEnumerable<TimelineItemResponseModel>>
                {
                    IsSuccess = true,
                    Data = timelineResponseItems
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<IEnumerable<TimelineItemResponseModel>>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving course timeline: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // GET: Timeline/GetTimelineItem/{timelineItemId}
        // Returns a specific timeline item with all its data
        [HttpGet("{timelineItemId}")]
        public async Task<ResultViewModel<TimelineItemResponseModel>> GetTimelineItem(int timelineItemId)
        {
            try
            {
                var timelineItem = await _context.TimeLineItems
                    .Include(t => t.Post)
                    .Include(t => t.Assignment)
                    .Include(t => t.Assignment.AllowedExtensions)
                    .Include(t => t.Attachments)
                    .Include(t => t.Comments)
                    .ThenInclude(c => c.CommentedBy)
                    .FirstOrDefaultAsync(t => t.Id == timelineItemId && !t.IsDeleted);

                if (timelineItem == null)
                {
                    return new ResultViewModel<TimelineItemResponseModel>
                    {
                        IsSuccess = false,
                        Message = "Timeline item not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                var timelineResponse = new TimelineItemResponseModel
                {
                    Id = timelineItem.Id,
                    CourseID = timelineItem.CourseID,
                    UserID = timelineItem.UserID,
                    Title = timelineItem.Title,
                    Content = timelineItem.Content,
                    CreatedAt = timelineItem.CreatedAt,
                    Type = timelineItem.Type,
                    Attachments = timelineItem.Attachments.Select(a => new AttachmentResponseModel
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        FilePath = a.FilePath,
                        FileType = a.FileType,
                        FileSize = a.FileSize,
                        UploadDate = a.UploadDate
                    }).ToList(),
                    Comments = timelineItem.Comments.Where(c => !c.IsDeleted).Select(c => new CommentResponseModel
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CommentedById = c.CommentedByID,
                        CommentedByName = $"{c.CommentedBy.FirstName} {c.CommentedBy.LastName}",
                        CommentedByImage = c.CommentedBy.ImageURL,
                        Time = c.Time,
                        TimelineItemId = c.TimeLineItemID,
                        TimelineItemTitle = timelineItem.Title
                    }).OrderBy(c => c.Time).ToList()
                };

                // Add additional properties based on the type
                if (timelineItem.Type == TimeLineItemType.Assignment && timelineItem.Assignment != null)
                {
                    timelineResponse.AssignmentDetails = new AssignmentDetailsModel
                    {
                        Deadline = timelineItem.Assignment.Deadline,
                        AllowedExtensions = timelineItem.Assignment.AllowedExtensions?.Select(ae => new AllowedExtensionResponseModel
                        {
                            Id = ae.Id,
                            Extension = ae.Extension
                        }).ToList()
                    };
                }
                else if (timelineItem.Type == TimeLineItemType.Post && timelineItem.Post != null)
                {
                    // You could add post-specific details if needed in the future
                    timelineResponse.PostDetails = new PostDetailsModel
                    {
                        // Currently, posts don't have special properties beyond the timeline item
                    };
                }

                return new ResultViewModel<TimelineItemResponseModel>
                {
                    IsSuccess = true,
                    Data = timelineResponse
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<TimelineItemResponseModel>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving timeline item: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // GET: Timeline/GetCourseTimelineByType/{courseId}/{type}
        // Returns all timeline items of a specific type for a course
        [HttpGet("{courseId}/{type}")]
        public async Task<ResultViewModel<IEnumerable<TimelineItemResponseModel>>> GetCourseTimelineByType(int courseId, TimeLineItemType type)
        {
            try
            {
                var timelineItems = await _context.TimeLineItems
                    .Include(t => t.Post)
                    .Include(t => t.Assignment)
                    .Include(t => t.Assignment.AllowedExtensions)
                    .Include(t => t.Attachments)
                    .Include(t => t.Comments)
                    .ThenInclude(c => c.CommentedBy)
                    .Where(t => t.CourseID == courseId && t.Type == type && !t.IsDeleted)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                var timelineResponseItems = new List<TimelineItemResponseModel>();

                foreach (var item in timelineItems)
                {
                    var timelineResponse = new TimelineItemResponseModel
                    {
                        Id = item.Id,
                        CourseID = item.CourseID,
                        UserID = item.UserID,
                        Title = item.Title,
                        Content = item.Content,
                        CreatedAt = item.CreatedAt,
                        Type = item.Type,
                        Attachments = item.Attachments.Select(a => new AttachmentResponseModel
                        {
                            Id = a.Id,
                            FileName = a.FileName,
                            FilePath = a.FilePath,
                            FileType = a.FileType,
                            FileSize = a.FileSize,
                            UploadDate = a.UploadDate
                        }).ToList(),
                        Comments = item.Comments.Where(c => !c.IsDeleted).Select(c => new CommentResponseModel
                        {
                            Id = c.Id,
                            Content = c.Content,
                            CommentedById = c.CommentedByID,
                            CommentedByName = $"{c.CommentedBy.FirstName} {c.CommentedBy.LastName}",
                            CommentedByImage = c.CommentedBy.ImageURL,
                            Time = c.Time,
                            TimelineItemId = c.TimeLineItemID,
                            TimelineItemTitle = item.Title
                        }).OrderBy(c => c.Time).ToList()
                    };

                    // Add additional properties based on the type
                    if (type == TimeLineItemType.Assignment && item.Assignment != null)
                    {
                        timelineResponse.AssignmentDetails = new AssignmentDetailsModel
                        {
                            Deadline = item.Assignment.Deadline,
                            AllowedExtensions = item.Assignment.AllowedExtensions?.Select(ae => new AllowedExtensionResponseModel
                            {
                                Id = ae.Id,
                                Extension = ae.Extension
                            }).ToList()
                        };
                    }
                    else if (type == TimeLineItemType.Post && item.Post != null)
                    {
                        // You could add post-specific details if needed in the future
                        timelineResponse.PostDetails = new PostDetailsModel
                        {
                            // Currently, posts don't have special properties beyond the timeline item
                        };
                    }

                    timelineResponseItems.Add(timelineResponse);
                }

                return new ResultViewModel<IEnumerable<TimelineItemResponseModel>>
                {
                    IsSuccess = true,
                    Data = timelineResponseItems
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<IEnumerable<TimelineItemResponseModel>>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving course timeline by type: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }
    }

    // Define the view models needed for the timeline controller
    public class TimelineItemResponseModel
    {
        public int Id { get; set; }
        public int CourseID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public TimeLineItemType Type { get; set; }
        public List<AttachmentResponseModel> Attachments { get; set; } = new List<AttachmentResponseModel>();
        public List<CommentResponseModel> Comments { get; set; } = new List<CommentResponseModel>();
        public AssignmentDetailsModel AssignmentDetails { get; set; }
        public PostDetailsModel PostDetails { get; set; }
    }

    public class AssignmentDetailsModel
    {
        public DateTime Deadline { get; set; }
        public List<AllowedExtensionResponseModel> AllowedExtensions { get; set; } = new List<AllowedExtensionResponseModel>();
    }

    public class PostDetailsModel
    {
        // Currently, posts don't have special properties beyond the timeline item
    }
}
