using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExaminationSystem.Models;
using ExaminationSystem.ViewModels;
using ExaminationSystem.ViewModels.Post;
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
    public class PostController : ControllerBase
    {
        private readonly Context _context;

        public PostController(Context context)
        {
            _context = context;
        }

        // GET: Post/GetAllPosts
        // Returns all posts
        [HttpGet]
        public async Task<ResultViewModel<IEnumerable<PostResponseModel>>> GetAllPosts()
        {
            try
            {
                var posts = await _context.TimeLineItems
                    .Include(t => t.Post)
                    .Include(t => t.Attachments)
                    .Where(t => t.Type == TimeLineItemType.Post && !t.IsDeleted)
                    .ToListAsync();

                var postResponses = posts.Select(t => new PostResponseModel
                {
                    Id = t.Id,
                    CourseID = t.CourseID,
                    UserID = t.UserID,
                    Title = t.Title,
                    Content = t.Content,
                    CreatedAt = t.CreatedAt,
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

                return new ResultViewModel<IEnumerable<PostResponseModel>>
                {
                    IsSuccess = true,
                    Data = postResponses
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<IEnumerable<PostResponseModel>>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving posts: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // GET: Post/GetAllCoursePosts/{courseId}
        // Returns all posts for a specific course
        [HttpGet("{courseId}")]
        public async Task<ResultViewModel<IEnumerable<PostResponseModel>>> GetAllCoursePosts(int courseId)
        {
            try
            {
                var posts = await _context.TimeLineItems
                    .Include(t => t.Post)
                    .Include(t => t.Attachments)
                    .Where(t => t.Type == TimeLineItemType.Post && t.CourseID == courseId && !t.IsDeleted)
                    .ToListAsync();

                var postResponses = posts.Select(t => new PostResponseModel
                {
                    Id = t.Id,
                    CourseID = t.CourseID,
                    UserID = t.UserID,
                    Title = t.Title,
                    Content = t.Content,
                    CreatedAt = t.CreatedAt,
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

                return new ResultViewModel<IEnumerable<PostResponseModel>>
                {
                    IsSuccess = true,
                    Data = postResponses
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<IEnumerable<PostResponseModel>>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving course posts: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // GET: Post/GetById/{postId}
        // Returns a specific post by ID
        [HttpGet("{postId}")]
        public async Task<ResultViewModel<PostResponseModel>> GetById(int postId)
        {
            try
            {
                var timelineItem = await _context.TimeLineItems
                    .Include(t => t.Post)
                    .Include(t => t.Attachments)
                    .FirstOrDefaultAsync(t => t.Id == postId && t.Type == TimeLineItemType.Post && !t.IsDeleted);

                if (timelineItem == null)
                {
                    return new ResultViewModel<PostResponseModel>
                    {
                        IsSuccess = false,
                        Message = "Post not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                var postResponse = new PostResponseModel
                {
                    Id = timelineItem.Id,
                    CourseID = timelineItem.CourseID,
                    UserID = timelineItem.UserID,
                    Title = timelineItem.Title,
                    Content = timelineItem.Content,
                    CreatedAt = timelineItem.CreatedAt,
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

                return new ResultViewModel<PostResponseModel>
                {
                    IsSuccess = true,
                    Data = postResponse
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<PostResponseModel>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving post: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // POST: Post/CreatePost
        // Creates a new post (for instructors only)
        [Authorize("Instructor")]
        [HttpPost]
        public async Task<ResultViewModel<int>> CreatePost(PostCreateRequestModel request)
        {
            try
            {
                // Create TimeLineItem first
                var timelineItem = new TimeLineItem
                {
                    CourseID = request.CourseID,
                    UserID = request.InstructorID,
                    Type = TimeLineItemType.Post,
                    Title = request.Title,
                    Content = request.Content,
                    CreatedAt = DateTime.Now,
                    Attachments = new List<Attachment>()
                };

                // Create Post connected to TimeLineItem
                var post = new Post
                {
                    TimeLineItem = timelineItem,
                    Content = request.Content,
                    CreatedAt = DateTime.Now
                };

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

                // Save TimeLineItem and Post
                _context.TimeLineItems.Add(timelineItem);
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                return new ResultViewModel<int>
                {
                    IsSuccess = true,
                    Data = timelineItem.Id,
                    Message = "Post created successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<int>
                {
                    IsSuccess = false,
                    Message = "Failed to create post: " + ex.Message,
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // PUT: Post/EditPost
        // Updates an existing post
        [Authorize("Instructor")]
        [HttpPut]
        public async Task<ResultViewModel<bool>> EditPost(PostEditRequestModel request)
        {
            try
            {
                var timelineItem = await _context.TimeLineItems
                    .Include(t => t.Post)
                    .Include(t => t.Attachments)
                    .FirstOrDefaultAsync(t => t.Id == request.Id && t.Type == TimeLineItemType.Post && !t.IsDeleted);

                if (timelineItem == null)
                {
                    return new ResultViewModel<bool>
                    {
                        IsSuccess = false,
                        Message = "Post not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Check if the instructor is the one who created the post
                if (timelineItem.UserID != request.InstructorID)
                {
                    return new ResultViewModel<bool>
                    {
                        IsSuccess = false,
                        Message = "You don't have permission to edit this post",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Update TimeLineItem properties
                timelineItem.Title = request.Title;
                timelineItem.Content = request.Content;

                // Update the Post entity
                if (timelineItem.Post != null)
                {
                    timelineItem.Post.Content = request.Content;
                }

                // Handle attachments update if needed
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
                    Message = "Post updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<bool>
                {
                    IsSuccess = false,
                    Message = "Failed to update post: " + ex.Message,
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // DELETE: Post/DeletePost/{postId}
        // Deletes a post
        [Authorize("Instructor")]
        [HttpDelete("{postId}")]
        public async Task<ResultViewModel<bool>> DeletePost(int postId)
        {
            try
            {
                var timelineItem = await _context.TimeLineItems
                    .Include(t => t.Post)
                    .FirstOrDefaultAsync(t => t.Id == postId && t.Type == TimeLineItemType.Post && !t.IsDeleted);

                if (timelineItem == null)
                {
                    return new ResultViewModel<bool>
                    {
                        IsSuccess = false,
                        Message = "Post not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Soft delete
                timelineItem.IsDeleted = true;
                if (timelineItem.Post != null)
                {
                    timelineItem.Post.IsDeleted = true;
                }

                await _context.SaveChangesAsync();

                return new ResultViewModel<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Post deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<bool>
                {
                    IsSuccess = false,
                    Message = "Failed to delete post: " + ex.Message,
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }
    }
}

