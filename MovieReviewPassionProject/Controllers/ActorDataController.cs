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
using MovieReviewPassionProject.Models;

namespace MovieReviewPassionProject.Controllers
{
    public class ActorDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ///Objective: Create a method that allow us to return all actors from the database
        /// <summary>
        /// return all actors from the database
        /// </summary>
        /// <returns>List of actors in the database</returns>
        /// <example>GET: api/ActorData/ListActors</example>
        [HttpGet]
        public List<ActorDto> ListActors()
        {
            List<Actor> Actors = db.Actors.ToList();
            List<ActorDto> ActorDtos = new List<ActorDto>();

            Actors.ForEach(a => ActorDtos.Add(new ActorDto()
            {
                ActorId = a.ActorId,
                ActorName = a.ActorName,
                ActorAge = a.ActorAge,
                Education = a.Education,
                RewardCount = a.RewardCount,
                ActorImg = a.ActorImg
            }));

            return ActorDtos;
        }

        ///Objective: Create a method that allow us to return all actors that are related to the selected movie
        ///by entering a interger value of the selected movie id
        /// <summary>
        /// Return all actors that are related to the movie from the database
        /// </summary>
        /// <param name="id">movieID</param>
        /// <returns>List of actors that are related to the selected movie</returns>
        ///<example>GET: api/ActorData/ListActorsForMovie</example>
        [HttpGet]
        public List<ActorDto> ListActorsForMovie(int id)
        {
            List<Actor> Actors = db.Actors.Where(a => a.Movies.Any(
                m => m.MovieID == id)).ToList();
            List<ActorDto> ActorDtos = new List<ActorDto>();

            Actors.ForEach(a => ActorDtos.Add(new ActorDto()
            {
                ActorId = a.ActorId, 
                ActorName = a.ActorName,
                ActorAge = a.ActorAge,
                Education = a.Education,
                RewardCount = a.RewardCount,
                ActorImg = a.ActorImg
            }));

            return ActorDtos;
        }

        ///Objective: Create a method that allow us to return all actors that are not related to the selected movie
        ///by entering a interger value of the selected movie id
        /// <summary>
        /// Return all actors that are not related to the movie from the database
        /// </summary>
        /// <param name="id">movieID</param>
        /// <returns>List of actors that are not related to the selected movie</returns>
        ///<example>GET: api/ActorData/ListActorsNotInMovie</example>
        [HttpGet]
        public List<ActorDto> ListActorsNotInMovie(int id)
        {
            List<Actor> Actors = db.Actors.Where(a => !a.Movies.Any(
                m => m.MovieID == id)).ToList();
            List<ActorDto> ActorDtos = new List<ActorDto>();

            Actors.ForEach(a => ActorDtos.Add(new ActorDto()
            {
                ActorId = a.ActorId,
                ActorImg = a.ActorImg,
                ActorName = a.ActorName,
                ActorAge = a.ActorAge,
                Education = a.Education,
                RewardCount = a.RewardCount
            }));

            return ActorDtos;
        }

        ///Objective: Create a method that allow us to return the selected actor by entering a interger value of the selected actor id
        /// <summary>
        /// Return the selected the actor from the database
        /// </summary>
        /// <param name="id">actorID</param>
        /// <return>The selected actor</return>
        ///<example>GET: api/ActorData/FindActor/{id}</example>
        [ResponseType(typeof(Actor))]
        [HttpGet]
        public IHttpActionResult FindActor(int id)
        {
            Actor Actor = db.Actors.Find(id);
            ActorDto ActorDto = new ActorDto()
            {
                ActorId = Actor.ActorId,
                ActorImg = Actor.ActorImg,
                ActorName = Actor.ActorName,
                ActorAge = Actor.ActorAge,
                Education = Actor.Education,
                RewardCount = Actor.RewardCount
            };
            if (Actor == null)
            {
                return NotFound();
            }

            return Ok(ActorDto);
        }

        ///Objective: Create a method that allow us to access the selected actor by entering a interger value of the selected actor  id
        ///Then Update the selected actor with JSON form data of the actor  model 
        /// <summary>
        /// Update the selected the actor  from the database
        /// </summary>
        /// <param name="id">actor ID</param>
        /// <param name="actor ">Actor  JSON form data</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        ///<example>POST: api/ActorData/UpdateActor/{id}</example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateActor(int id, Actor actor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != actor.ActorId)
            {
                return BadRequest();
            }

            db.Entry(actor).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActorExists(id))
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

        ///Objective: Create a method that allow us to add a new actor by JSON form data of the actor model into the database 
        /// <summary>
        /// Add a new actor into the database
        /// </summary>
        /// <param name="actor">Actor JSON form data</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        ///<example>POST: api/ActorData/AddActor</example>
        [ResponseType(typeof(Actor))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddActor(Actor actor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Actors.Add(actor);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = actor.ActorId }, actor);
        }

        ///Objective: Create a method that allow us to delete the selected actor by entering a interger value of the selected actor id
        /// <summary>
        /// Remove the selected the actor from the database
        /// </summary>
        /// <param name="id">actorID</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        ///<example>POST: api/ActorData/DeleteActor/{id}</example>
        [ResponseType(typeof(Actor))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteActor(int id)
        {
            Actor actor = db.Actors.Find(id);
            if (actor == null)
            {
                return NotFound();
            }

            db.Actors.Remove(actor);
            db.SaveChanges();

            return Ok(actor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ActorExists(int id)
        {
            return db.Actors.Count(e => e.ActorId == id) > 0;
        }
    }
}