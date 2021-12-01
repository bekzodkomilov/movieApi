using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using movies.Data;
using movies.Entities;

namespace movies.Services
{
    public class GenreService : IGenreService
    {
        private readonly MoviesContext _ctx;
        private readonly ILogger<GenreService> _logger;

        public GenreService(MoviesContext context, ILogger<GenreService> logger)
        {
            _ctx = context;
            _logger = logger;
        }
        
        public async Task<(bool IsSuccess, Exception Exception, Genre Genre)> CreateAsync(Genre genre)
        {
            try
            {
                await _ctx.Genres.AddAsync(genre);
                await _ctx.SaveChangesAsync();

                return (true, null, genre);
            }
            catch(Exception e)
            {
                return (false, e, null);
            }
        }

        public async Task<(bool IsSuccess, Exception Exception)> DeleteAsync(Guid id)
        {
            var genre = await GetAsync(id);
            if(genre == default(Genre))
            {
                return (false, new ArgumentException("Not found."));
            }

            try
            {
                _ctx.Genres.Remove(genre);
                await _ctx.SaveChangesAsync();

                return (true, null);
            }
            catch(Exception e)
            {
                return (false, e);
            }
        }

        public Task<bool> ExistsAsync(Guid id)
            => _ctx.Genres.AnyAsync(g => g.Id == id);

        public Task<bool> ExistsAsync(string name)
            => _ctx.Genres.AnyAsync(g => g.Name == name);

        public Task<Genre> GetAsync(Guid id)
            => _ctx.Genres.Include(g => g.Movies).FirstOrDefaultAsync(g => g.Id == id);

        public Task<List<Genre>> GetAllAsync()
            => _ctx.Genres
                .AsNoTracking()
                .Include(g => g.Movies)
                .ToListAsync();
    }
}
