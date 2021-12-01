using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using movies.Data;
using movies.Entities;

namespace movies.Services
{
    public class MovieService : IMovieService
    {
        private readonly MoviesContext _ctx;

        public MovieService(MoviesContext context)
        {
            _ctx = context;
        }

        public async Task<(bool IsSuccess, Exception Exception, Movie Movie)> CreateAsync(Movie movie)
        {
            try
            {
                await _ctx.Movies.AddAsync(movie);
                await _ctx.SaveChangesAsync();

                return (true, null, movie);
            }
            catch(Exception e)
            {
                return (false, e, null);
            }
        }

        public async Task<(bool IsSuccess, Exception Exception)> DeleteAsync(Guid id)
        {
            try
            {
                var movie = await GetAsync(id);

                if(movie == default(Movie))
                {
                    return (false, new Exception("Not found"));
                }

                _ctx.Movies.Remove(movie);
                await _ctx.SaveChangesAsync();

                return (true,  null);
            }
            catch(Exception e)
            {
                return (false, e);
            }
        }

        public Task<bool> ExistsAsync(Guid id)
            => _ctx.Movies.AnyAsync(a => a.Id == id);

        public Task<List<Movie>> GetAllAsync()
            => _ctx.Movies
                .AsNoTracking()
                .Include(m => m.Actors)
                .Include(m => m.Genres)
                .ToListAsync();

        public Task<List<Movie>> GetAllAsync(string title)
            => _ctx.Movies
                .AsNoTracking()
                .Where(a => a.Title == title)
                .Include(m => m.Actors)
                .Include(m => m.Genres)
                .ToListAsync();

        public Task<Movie> GetAsync(Guid id)
            => _ctx.Movies
                .Include(m => m.Actors)
                .Include(m => m.Genres)
                .FirstOrDefaultAsync(a => a.Id == id);
    }
}
