using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using movies.Data;
using movies.Entities;

namespace movies.Services
{
    public class ActorService : IActorService
    {
        private readonly MoviesContext _ctx;

        public ActorService(MoviesContext context)
        {
            _ctx = context;
        }

        public async Task<(bool IsSuccess, Exception Exception, Actor Actor)> CreateAsync(Actor actor)
        {
            try
            {
                await _ctx.Actors.AddAsync(actor);
                await _ctx.SaveChangesAsync();

                return (true, null, actor);
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
                var actor = await GetAsync(id);

                if(actor == default(Actor))
                {
                    return (false, new Exception("Not found"));
                }

                _ctx.Actors.Remove(actor);
                await _ctx.SaveChangesAsync();

                return (true,  null);
            }
            catch(Exception e)
            {
                return (false, e);
            }
        }

        public Task<bool> ExistsAsync(Guid id)
            => _ctx.Actors.AnyAsync(a => a.Id == id);

        public Task<List<Actor>> GetAllAsync()
            => _ctx.Actors
                .AsNoTracking()
                .Include(a => a.Movies)
                .ToListAsync();

        public Task<Actor> GetAsync(Guid id)
            => _ctx.Actors.Include(a => a.Movies).FirstOrDefaultAsync(a => a.Id == id);
    }
}
