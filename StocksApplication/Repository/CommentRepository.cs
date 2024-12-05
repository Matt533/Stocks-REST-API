using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using StocksApplication.Data;
using StocksApplication.Dtos.Comment;
using StocksApplication.Interfaces;
using StocksApplication.Models;

namespace StocksApplication.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _appDbContext;

        public CommentRepository(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _appDbContext.Comments.Include(a => a.AppUser).ToListAsync();


        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _appDbContext.Comments.Include(a => a.AppUser).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _appDbContext.Comments.AddAsync(commentModel);
            await _appDbContext.SaveChangesAsync();

            return commentModel;
        }

        public async Task<Comment?> UpdateAsync(int id, Comment commentModel)
        {
            var existingComment = await _appDbContext.Comments.FindAsync(id);

            if (existingComment is null)
            {
                return null;
            }

            existingComment.Title = commentModel.Title;
            existingComment.Content = commentModel.Content;

            await _appDbContext.SaveChangesAsync();

            return existingComment;

        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var commentModel = await _appDbContext.Comments.FirstOrDefaultAsync(x => x.Id == id);

            if (commentModel is null)
            {
                return null;
            }
            _appDbContext.Remove(commentModel);

            await _appDbContext.SaveChangesAsync();

            return commentModel;

        }

    }
}
