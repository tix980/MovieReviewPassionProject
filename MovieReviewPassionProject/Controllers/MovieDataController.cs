using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace MovieReviewPassionProject.Models
{
    public class MovieDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ///Objective: Create a method that allow us to return all movies from the database
        /// <summary>
        /// return all movies from the database
        /// </summary>
        /// <returns>List of movies in the database</returns>
        /// <example>GET: api/MovieData/ListMovies</example>
        [HttpGet]
        public List<MovieDto> ListMovies()
        {
            List<Movie> Movies = db.Movies.ToList();
            List<MovieDto> MovieDtos = new List<MovieDto>();
            Movies.ForEach(m => MovieDtos.Add(new MovieDto() {
                MovieID = m.MovieID,
                MovieImg = m.MovieImg,
                MovieName = m.MovieName,
                MovieGenre = m.MovieGenre,
                MovieInfo = m.MovieInfo
            }));
            return MovieDtos;
        }

        ///Objective: Create a method that allow us to return all movies that are related to the selected actor
        ///by entering a interger value of the selected actor id
        /// <summary>
        /// Return all movies that are related to the selected actor from the database
        /// </summary>
        /// <param name="id">ActorId</param>
        /// <returns>List of movies that are related to the selected actor</returns>
        ///<example>GET: api/MovieData/ListMoviesForActor</example>
        [HttpGet]
        public List<MovieDto> ListMoviesForActor(int id)
        {
            List<Movie> Movies = db.Movies.Where(m=>m.Actors.Any(
                a=>a.ActorId == id)).ToList();
            List<MovieDto> MovieDtos = new List<MovieDto>();
            Movies.ForEach(m => MovieDtos.Add(new MovieDto()
            {
                MovieID = m.MovieID,
                MovieImg = m.MovieImg,
                MovieName = m.MovieName,
                MovieGenre = m.MovieGenre,
                MovieInfo = m.MovieInfo
            }));
            return MovieDtos;
        }

        ///Objective: Create a method that allow us to associate the selected movie with the selected actor by entering interger value of their id
        /// <summary>
        /// Connect a movie id with a actor id
        /// </summary>
        /// <param name="movieid">The MovieID primary key</param>
        /// <param name="actorid">The actorID primary key</param>
        ///<return>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </return>
        /// <example>POST : api/moviedata/AssociateMovieWithActor/{movieid}/{actorid}</example>
        [HttpPost]
        [Route("api/moviedata/AssociateMovieWithActor/{movieid}/{actorid}")]
        [Authorize]
        public IHttpActionResult AssociateMovieWithActor(int movieid, int actorid)
        {
            Movie SelectedMovie = db.Movies.Include(m => m.Actors).Where(m=>m.MovieID == movieid).FirstOrDefault();
            Actor SelectedActor = db.Actors.Find(actorid);

            if(SelectedMovie == null || SelectedActor == null)
            {
                return NotFound();
            }

            SelectedMovie.Actors.Add(SelectedActor);
            db.SaveChanges();

            return Ok();
        }

        ///Objective: Create a method that allow us to separate the selected movie with the selected actor by entering interger value of their id
        /// <summary>
        /// separate a movie id with a actor id
        /// </summary>
        /// <param name="movieid">The MovieID primary key</param>
        /// <param name="actorid">The actorID primary key</param>
        ///<return>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </return>
        /// <example>POST : api/moviedata/NotAssociateMovieWithActor/{movieid}/{actorid}</example>
        [HttpPost]
        [Route("api/moviedata/NotAssociateMovieWithActor/{movieid}/{actorid}")]
        [Authorize]
        public IHttpActionResult NotAssociateMovieWithActor(int movieid, int actorid)
        {
            Movie SelectedMovie = db.Movies.Include(m => m.Actors).Where(m => m.MovieID == movieid).FirstOrDefault();
            Actor SelectedActor = db.Actors.Find(actorid);

            if (SelectedMovie == null || SelectedActor == null)
            {
                return NotFound();
            }

            SelectedMovie.Actors.Remove(SelectedActor);
            db.SaveChanges();

            return Ok();
        }


        ///Objective: Create a method that allow us to return the selected movie by entering a interger value of the selected movie id
        /// <summary>
        /// Return the selected the movie from the database
        /// </summary>
        /// <param name="id">MovieID</param>
        /// <return>The selected movie</return>
        ///<example>GET: api/MovieData/FindMovie/{id}</example>
        [ResponseType(typeof(Movie))]
        [HttpGet]
        public IHttpActionResult FindMovie(int id)
        {
            Movie Movie = db.Movies.Find(id);
            MovieDto MovieDto = new MovieDto()
            {
                MovieID = Movie.MovieID,
                MovieImg = Movie.MovieImg,
                MovieName = Movie.MovieName,
                MovieGenre = Movie.MovieGenre,
                MovieInfo = Movie.MovieInfo
            };
            if (Movie == null)
            {
                return NotFound();
            }

            return Ok(Movie);
        }

        ///Objective: Create a method that allow us to access the selected movie by entering a interger value of the selected movie id
        ///Then Update the selected movie with JSON form data of the movie model 
        /// <summary>
        /// Update the selected the movie from the database
        /// </summary>
        /// <param name="id">MovieID</param>
        /// <param name="movie">Movie JSON form data</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        ///<example>POST: api/MovieData/UpdateMovie/{id}</example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateMovie(int id, Movie movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != movie.MovieID)
            {
                return BadRequest();
            }

            db.Entry(movie).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        ///Objective: Create a method that allow us to add a new movie by JSON form data of the movie model into the database 
        /// <summary>
        /// Add a new movie into the database
        /// </summary>
        /// <param name="movie">Movie JSON form data</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        ///<example>POST: api/MovieData/AddMovie</example>
        [ResponseType(typeof(Movie))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddMovie(Movie movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Movies.Add(movie);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = movie.MovieID }, movie);
        }

        ///Objective: Create a method that allow us to delete the selected movie by entering a interger value of the selected movie id
        /// <summary>
        /// Remove the selected the movie from the database
        /// </summary>
        /// <param name="id">MovieID</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        ///<example>POST: api/MovieData/DeleteMovieMovie/{id}</example>
        [ResponseType(typeof(Movie))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteMovie(int id)
        {
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return NotFound();
            }

            db.Movies.Remove(movie);
            db.SaveChanges();

            return Ok(movie);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MovieExists(int id)
        {
            return db.Movies.Count(e => e.MovieID == id) > 0;
        }
    }
}