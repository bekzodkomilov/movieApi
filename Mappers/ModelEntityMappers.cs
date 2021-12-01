using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using movies.Models;

namespace movies.Mappers
{
    public static class ModelEntityMappers
    {
        public static Entities.Genre ToEntity(this Models.NewGenre genre)
            => new Entities.Genre(genre.Name);

        public static Entities.Actor ToEntity(this Models.NewActor actor)
            => new Entities.Actor()
            {
                Id = Guid.NewGuid(),
                Fullname = actor.Fullname,
                Birthdate = actor.Birthdate,
                Image = toByte(actor.Image)
            };

        public static Entities.Movie ToEntity(this Models.NewMovie movie, 
            IEnumerable<Entities.Actor> actors, 
            IEnumerable<Entities.Genre> genres)
                => new Entities.Movie()
                {
                    Id = Guid.NewGuid(),
                    Title = movie.Title,
                    Description = movie.Description,
                    ReleaseDate = movie.ReleaseDate,
                    Rating = movie.Rating,
                    Actors = actors.ToList(),
                    Genres = genres.ToList(),
                    Image = toByte(movie.Image)
                };
        private static byte[] toByte(IFormFile image)
        {
            var memoryStream = new MemoryStream();
            image.CopyToAsync(memoryStream);
            var result = memoryStream.ToArray();
            return result;
        }
    }
}
