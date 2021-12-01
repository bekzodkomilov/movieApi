using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using movies.Mappers;
using movies.Models;
using movies.Services;
using Newtonsoft.Json;

namespace movies.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _ms;
        private readonly IActorService _as;
        private readonly IGenreService _gs;

        public MovieController(
            IMovieService movieService,
            IGenreService genreService,
            IActorService actorService)
        {
            _ms = movieService;
            _as = actorService;
            _gs = genreService;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromForm] NewMovie movie)
        {
            if(movie.ActorIds.Count() < 1 || movie.GenreIds.Count() < 1)
            {
                return BadRequest("Actors and Genres are required");
            }

            if(!movie.GenreIds.All(id => _gs.ExistsAsync(id).Result))
            {
                return BadRequest("Genre doesnt exist");
            }

            if(!movie.ActorIds.All(id => _as.ExistsAsync(id).Result))
            {
                return BadRequest("Actor doesnt exist");
            }

            var genres = movie.GenreIds.Select(id => _gs.GetAsync(id).Result);
            var actors = movie.ActorIds.Select(id => _as.GetAsync(id).Result);
            
            var result = await _ms.CreateAsync(movie.ToEntity(actors, genres));

            if(result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(result.Exception.Message);
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var json = JsonConvert.SerializeObject(
                await _ms.GetAllAsync(), Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
            );
            return Ok(json);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            if(await _ms.ExistsAsync(id))
            {
                var movie = await _ms.GetAsync(id);
                var json = JsonConvert.SerializeObject(
                    movie, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }
                );
                return File(movie.Image, "image/jpg");
            }

            return NotFound();
        }
        
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid id)
            => Ok(await _ms.DeleteAsync(id));
    }
}
