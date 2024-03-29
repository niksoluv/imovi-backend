﻿using imovi_backend.Core.IRepositories;
using imovi_backend.Data;
using imovi_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace imovi_backend.Core.Repositories
{
    public class CommentsRepository : GenericRepository<Comment>, ICommentsRepository
    {
        public CommentsRepository(
            ApplicationContext context,
            ILogger logger) : base(context, logger)
        {

        }

        public async Task<Comment> CreateComment(Comment comment, User user)
        {
            try
            {
                var existingMovie = await _context.Movies.Where(movie => movie.MovieId == comment.Movie.MovieId)
                    .FirstOrDefaultAsync();

                if (existingMovie == null)
                    await _context.Movies.AddAsync(comment.Movie);

                comment.User = await _context.Users.Where(u => u.Id == user.Id).FirstOrDefaultAsync();
                await dbSet.AddAsync(comment);
                return comment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} CreateComment method error", typeof(CommentsRepository));
                return null;
            }
        }

        public async Task<Comment> EditComment(Comment comment, User user)
        {
            try
            {
                var existingComment = await dbSet.Where(c => c.Id == comment.Id)
                    .FirstOrDefaultAsync();

                if (existingComment == null)
                    return null;

                existingComment.Data=comment.Data;
                dbSet.Update(existingComment);
                return existingComment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} EditComment method error", typeof(CommentsRepository));
                return null;
            }
        }

        public async Task<Comment> ReplyComment(CommentReplyDTO commentReply, User user)
        {
            try
            {
                Comment comment = await dbSet.Where(c=>c.Id==commentReply.CommentId).FirstOrDefaultAsync();

                CommentReply reply = new CommentReply()
                {
                    User = user,
                    Data = commentReply.Data,
                    CommentId = comment.Id
                };

                await _context.CommentReplies.AddAsync(reply);

                return comment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} CreateComment method error", typeof(CommentsRepository));
                return null;
            }
        }

        public async Task<List<Comment>> GetMovieComments(string movieId)
        {
            try
            {
                var comments = await dbSet
                    .Where(comment => comment.Movie.MovieId == movieId)
                    .Include(c => c.Movie)
                    .Include(c => c.User)
                    .Include(c => c.UsersLikes)
                    .Include(c=>c.CommentReplies)
                    .ThenInclude(cr=>cr.User)
                    .ToListAsync();

                comments = comments.OrderByDescending(c => c.Date).ToList();

                foreach (Comment comment in comments)
                {
                    comment.CommentReplies = comment.CommentReplies.OrderByDescending(c => c.Date).ToList();
                }
                return comments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetMovieComments method error", typeof(CommentsRepository));
                return null;
            }
        }

        public async Task<Comment> LikeComment(Guid commentId, Guid userId)
        {
            try
            {
                var comment = await dbSet.Where(c => c.Id == commentId).FirstOrDefaultAsync();

                if (comment == null)
                    return null;

                LikedComment likedComment = new LikedComment()
                {
                    CommentId = commentId,
                    UserId = userId,
                };

                _context.LikedComments.Add(likedComment);

                comment.Likes++;

                return comment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} LikeComment method error", typeof(CommentsRepository));
                return null;
            }
        }

        public async Task<Comment> UnlikeComment(Guid commentId, Guid userId)
        {
            try
            {
                var comment = await dbSet.Where(c => c.Id == commentId).FirstOrDefaultAsync();

                if (comment == null)
                    return null;

                if (comment.Likes > 0)
                    comment.Likes--;

                LikedComment likedComment = await _context.LikedComments
                    .Where(lc => lc.CommentId == commentId && lc.UserId == userId).FirstOrDefaultAsync();
                _context.LikedComments.Remove(likedComment);

                return comment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} UnlikeComment method error", typeof(CommentsRepository));
                return null;
            }
        }
    }
}
