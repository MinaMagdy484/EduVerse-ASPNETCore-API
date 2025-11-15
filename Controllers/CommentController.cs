using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExaminationSystem.Models;
using ExaminationSystem.ViewModels;
using ExaminationSystem.ViewModels.Comment;
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
    public class CommentController : ControllerBase
    {
        private readonly Context _context;

        public CommentController(Context context)
        {
            _context = context;
        }

        // GET: Comment/GetAllComments
        // Returns all comments
        [HttpGet]
        public async Task<ResultViewModel<IEnumerable<CommentResponseModel>>> GetAllComments()
        {
            try
            {
                var comments = await _context.Comments
                    .Include(c => c.CommentedBy)
                    .Include(c => c.TimeLineItem)
                    .Where(c => !c.IsDeleted)
                    .OrderBy(c => c.Time)
                    .ToListAsync();

                var commentResponses = comments.Select(c => new CommentResponseModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    CommentedById = c.CommentedByID,
                    CommentedByName = $"{c.CommentedBy.FirstName} {c.CommentedBy.LastName}",
                    CommentedByImage = c.CommentedBy.ImageURL,
                    Time = c.Time,
                    TimelineItemId = c.TimeLineItemID,
                    TimelineItemTitle = c.TimeLineItem.Title
                }).ToList();

                return new ResultViewModel<IEnumerable<CommentResponseModel>>
                {
                    IsSuccess = true,
                    Data = commentResponses
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<IEnumerable<CommentResponseModel>>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving comments: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // GET: Comment/GetByTimelineItem/{timelineItemId}
        // Returns all comments for a specific timeline item
        [HttpGet("{timelineItemId}")]
        public async Task<ResultViewModel<IEnumerable<CommentResponseModel>>> GetByTimelineItem(int timelineItemId)
        {
            try
            {
                var comments = await _context.Comments
                    .Include(c => c.CommentedBy)
                    .Include(c => c.TimeLineItem) // Include the TimeLineItem navigation property
                    .Where(c => c.TimeLineItemID == timelineItemId && !c.IsDeleted)
                    .OrderBy(c => c.Time)
                    .ToListAsync();

                var commentResponses = comments.Select(c => new CommentResponseModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    CommentedById = c.CommentedByID,
                    CommentedByName = $"{c.CommentedBy.FirstName} {c.CommentedBy.LastName}",
                    CommentedByImage = c.CommentedBy.ImageURL,
                    Time = c.Time,
                    TimelineItemId = c.TimeLineItemID,
                    TimelineItemTitle = c.TimeLineItem.Title // Populate the TimelineItemTitle
                }).ToList();

                return new ResultViewModel<IEnumerable<CommentResponseModel>>
                {
                    IsSuccess = true,
                    Data = commentResponses
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<IEnumerable<CommentResponseModel>>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving comments: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }


        // GET: Comment/GetById/{commentId}
        // Returns a specific comment by ID
        [HttpGet("{commentId}")]
        public async Task<ResultViewModel<CommentResponseModel>> GetById(int commentId)
        {
            try
            {
                var comment = await _context.Comments
                    .Include(c => c.CommentedBy)
                    .Include(c => c.TimeLineItem) // Include the TimeLineItem navigation property
                    .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

                if (comment == null)
                {
                    return new ResultViewModel<CommentResponseModel>
                    {
                        IsSuccess = false,
                        Message = "Comment not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                var commentResponse = new CommentResponseModel
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    CommentedById = comment.CommentedByID,
                    CommentedByName = $"{comment.CommentedBy.FirstName} {comment.CommentedBy.LastName}",
                    CommentedByImage = comment.CommentedBy.ImageURL,
                    Time = comment.Time,
                    TimelineItemId = comment.TimeLineItemID,
                    TimelineItemTitle = comment.TimeLineItem.Title // Populate the TimelineItemTitle
                };

                return new ResultViewModel<CommentResponseModel>
                {
                    IsSuccess = true,
                    Data = commentResponse
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<CommentResponseModel>
                {
                    IsSuccess = false,
                    Message = $"Error retrieving comment: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }


        // POST: Comment/AddComment
        // Adds a comment to a timeline item
        [Authorize]
        [HttpPost]
        public async Task<ResultViewModel<int>> AddComment(CommentCreateModel request)
        {
            try
            {
                // Verify the timeline item exists
                var timelineItem = await _context.TimeLineItems
                    .FirstOrDefaultAsync(t => t.Id == request.TimelineItemId && !t.IsDeleted);

                if (timelineItem == null)
                {
                    return new ResultViewModel<int>
                    {
                        IsSuccess = false,
                        Message = "Timeline item not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Create and save the comment
                var comment = new Comment
                {
                    TimeLineItemID = request.TimelineItemId,
                    CommentedByID = request.UserId,
                    Content = request.Content,
                    Time = DateTime.Now
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                return new ResultViewModel<int>
                {
                    IsSuccess = true,
                    Data = comment.Id,
                    Message = "Comment added successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<int>
                {
                    IsSuccess = false,
                    Message = $"Failed to add comment: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // PUT: Comment/EditComment
        // Edit an existing comment
        [Authorize]
        [HttpPut]
        public async Task<ResultViewModel<bool>> EditComment(CommentEditModel request)
        {
            try
            {
                var comment = await _context.Comments
                    .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted);

                if (comment == null)
                {
                    return new ResultViewModel<bool>
                    {
                        IsSuccess = false,
                        Message = "Comment not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Ensure the user is the one who made the comment
                if (comment.CommentedByID != request.UserId)
                {
                    return new ResultViewModel<bool>
                    {
                        IsSuccess = false,
                        Message = "You don't have permission to edit this comment",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Update comment content
                comment.Content = request.Content;

                await _context.SaveChangesAsync();

                return new ResultViewModel<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Comment updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<bool>
                {
                    IsSuccess = false,
                    Message = $"Failed to update comment: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }

        // DELETE: Comment/DeleteComment/{commentId}/{userId}
        // Deletes a comment
        [Authorize]
        [HttpDelete("{commentId}/{userId}")]
        public async Task<ResultViewModel<bool>> DeleteComment(int commentId, int userId)
        {
            try
            {
                var comment = await _context.Comments
                    .Include(c => c.TimeLineItem)
                    .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

                if (comment == null)
                {
                    return new ResultViewModel<bool>
                    {
                        IsSuccess = false,
                        Message = "Comment not found",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Allow deletion if:
                // 1. The user is the one who made the comment, OR
                // 2. The user is the creator of the timeline item
                if (comment.CommentedByID != userId && comment.TimeLineItem.UserID != userId)
                {
                    return new ResultViewModel<bool>
                    {
                        IsSuccess = false,
                        Message = "You don't have permission to delete this comment",
                        ErrorCode = ErrorCode.None
                    };
                }

                // Soft delete
                comment.IsDeleted = true;
                await _context.SaveChangesAsync();

                return new ResultViewModel<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Comment deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<bool>
                {
                    IsSuccess = false,
                    Message = $"Failed to delete comment: {ex.Message}",
                    ErrorCode = ErrorCode.UnKnown
                };
            }
        }
    }
    
}
