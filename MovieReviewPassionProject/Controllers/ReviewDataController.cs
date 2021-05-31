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
    public class ReviewDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ReviewData/ListReviews
        [HttpGet]
        public List<ReviewDto> ListReviews()
        {
            List<Review> Reviews = db.Reviews.ToList();
            List<ReviewDto> ReviewDtos = new List<ReviewDto>();
            Reviews.ForEach(r => ReviewDtos.Add(new ReviewDto()
            {
                MovieName = r.Movie.MovieName,
                ReviewID = r.ReviewID,
                ReviewDetail = r.ReviewDetail,
                ReviewDate = r.ReviewDate
            }));
            return ReviewDtos;
        }

        // GET: api/ReviewData/FindReview/{id}
        [ResponseType(typeof(Review))]
        [HttpGet]
        public IHttpActionResult FindReview(int id)
        {
            Review Review = db.Reviews.Find(id);
            ReviewDto ReviewDto = new ReviewDto()
            {
                MovieName = Review.Movie.MovieName,
                ReviewID = Review.ReviewID,
                ReviewDetail = Review.ReviewDetail,
                ReviewDate = Review.ReviewDate

            };
            if (Review == null)
            {
                return NotFound();
            }

            return Ok(ReviewDto);
        }

        // POST: api/ReviewData/UpdateReview/{id}
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateReview(int id, Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != review.ReviewID)
            {
                return BadRequest();
            }

            db.Entry(review).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
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

        // POST: api/ReviewData/AddReview
        [ResponseType(typeof(Review))]
        [HttpPost]
        public IHttpActionResult AddReview(Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Reviews.Add(review);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = review.ReviewID }, review);
        }

        // DELETE: api/ReviewData/DeleteReview/{id}
        [ResponseType(typeof(Review))]
        [HttpPost]
        public IHttpActionResult DeleteReview(int id)
        {
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return NotFound();
            }

            db.Reviews.Remove(review);
            db.SaveChanges();

            return Ok(review);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReviewExists(int id)
        {
            return db.Reviews.Count(e => e.ReviewID == id) > 0;
        }
    }
}