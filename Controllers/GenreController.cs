using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using movies.Mappers;
using movies.Models;
using movies.Services;
using Newtonsoft.Json;

namespace movies.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : ControllerBase
    {
        private readonly ILogger<GenreController> _logger;
        private readonly IGenreService _genreService;

        public GenreController(ILogger<GenreController> logger, IGenreService genreService)
        {
            _logger = logger;
            _genreService = genreService;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(NewGenre genre)
        {
            if(await _genreService.ExistsAsync(genre.Name))
            {
                return Conflict(nameof(genre.Name));
            }

            var result = await _genreService.CreateAsync(genre.ToEntity());
            
            if(result.IsSuccess)
            {
                return Ok(result.Genre);
            }

            return BadRequest(
            new { 
                isSuccess = false, 
                error = result.Exception.Message 
            });
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            if(await _genreService.ExistsAsync(id))
            {
                var json = JsonConvert.SerializeObject(
                    await _genreService.GetAsync(id), Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }
                );
                return Ok(json);
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var json = JsonConvert.SerializeObject(
                await _genreService.GetAllAsync(), Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
            );
            return Ok(json);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid id)
            => Ok(await _genreService.DeleteAsync(id));
    }
}
