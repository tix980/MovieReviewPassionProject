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

        // GET: api/ActorData/ListActors
        [HttpGet]
        public List<ActorDto> ListActors()
        {
            List<Actor> Actors = db.Actors.ToList();
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

        // GET: api/ActorData/FindActor/{id}
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
                ActorAge = Actor.ActorAge
            };
            if (Actor == null)
            {
                return NotFound();
            }

            return Ok(ActorDto);
        }

        // PUT: api/ActorData/UpdateActor/{id}
        [ResponseType(typeof(void))]
        [HttpPost]
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

        // POST: api/ActorData/AddActor
        [ResponseType(typeof(Actor))]
        [HttpPost]
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

        // DELETE: api/ActorData/DeleteActor/{id}
        [ResponseType(typeof(Actor))]
        [HttpPost]
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