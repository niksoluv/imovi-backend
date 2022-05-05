﻿using imovi_backend.Core.IRepositories;
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

        public async Task<List<Comment>> GetMovieComments(string movieId)
        {
            try
            {
                var comments = await dbSet
                    .Where(comment=>comment.Movie.MovieId==movieId)
                    .Include(c=>c.Movie)
                    .Include(c=>c.User)
                    .ToListAsync(); 
                comments.Reverse();
                return comments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetMovieComments method error", typeof(CommentsRepository));
                return null;
            }
        }
    }
}
