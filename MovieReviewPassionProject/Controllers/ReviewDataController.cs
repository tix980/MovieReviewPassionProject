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

        ///Objective: Create a method that allow us to return all reviews from the database
        /// <summary>
        /// return all reviews from the database
        /// </summary>
        /// <returns>List of reviews in the database</returns>
        /// <example>GET: api/ReviewData/ListReviews</example>
        [HttpGet]
        public List<ReviewDto> ListReviews()
        {
            List<Review> Reviews = db.Reviews.ToList();
            List<ReviewDto> ReviewDtos = new List<ReviewDto>();
            Reviews.ForEach(r => ReviewDtos.Add(new ReviewDto()
            {
                MovieID = r.Movie.MovieID,
                MovieName = r.Movie.MovieName,
                ReviewID = r.ReviewID,
                AuthorName = r.AuthorName,
                StarRatings = r.StarRatings,
                ReviewDetail = r.ReviewDetail,
                ReviewDate = r.ReviewDate
            }));
            return ReviewDtos;
        }

        ///Objective: Create a method that allow us to return all reviews that are related to the selected movie
        ///by entering a interger value of the selected movie id
        /// <summary>
        /// Return all reviews that are related to the movie from the database
        /// </summary>
        /// <param name="id">MovieID</param>
        /// <returns>List of reviews that are related to the selected movie</returns>
        ///<example>GET: api/ReviewData/ListReviewsForMovie/id</example>
        [HttpGet]
        public List<ReviewDto> ListReviewsForMovie(int id)
        {
            List<Review> Reviews = db.Reviews.Where(r=>r.MovieID == id).ToList();
            List<ReviewDto> ReviewDtos = new List<ReviewDto>();
            Reviews.ForEach(r => ReviewDtos.Add(new ReviewDto()
            {
                MovieID = r.Movie.MovieID,
                MovieName = r.Movie.MovieName,
                ReviewID = r.ReviewID,
                AuthorName = r.AuthorName,
                StarRatings = r.StarRatings,
                ReviewDetail = r.ReviewDetail,
                ReviewDate = r.ReviewDate
            }));
            return ReviewDtos;
        }

        ///Objective: Create a method that allow us to return the selected review by entering a interger value of the selected review id
        /// <summary>
        /// Return the selected the review from the database
        /// </summary>
        /// <param name="id">reviewID</param>
        /// <return>The selected review</return>
        ///<example>GET: api/ReviewData/FindReview/{id}</example>
        // GET: api/ReviewData/FindReview/{id}
        [ResponseType(typeof(Review))]
        [HttpGet]
        public IHttpActionResult FindReview(int id)
        {
            Review Review = db.Reviews.Find(id);
            ReviewDto ReviewDto = new ReviewDto()
            {
                MovieID = Review.Movie.MovieID,
                MovieName = Review.Movie.MovieName,
                ReviewID = Review.ReviewID,
                AuthorName = Review.AuthorName,
                StarRatings = Review.StarRatings,
                ReviewDetail = Review.ReviewDetail,
                ReviewDate = Review.ReviewDate

            };
            if (Review == null)
            {
                return NotFound();
            }

            return Ok(ReviewDto);
        }

        ///Objective: Create a method that allow us to access the selected review by entering a interger value of the selected review id
        ///Then Update the selected review with JSON form data of the review model 
        /// <summary>
        /// Update the selected the review from the database
        /// </summary>
        /// <param name="id">reviewID</param>
        /// <param name="review">review JSON form data</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        ///<example>POST: api/ReviewData/UpdateReview/{id}</example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
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

        ///Objective: Create a method that allow us to add a new review by JSON form data of the review model into the database 
        /// <summary>
        /// Add a new review into the database
        /// </summary>
        /// <param name="review">review JSON form data</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        ///<example>POST: api/ReviewData/AddReview</example>
        [ResponseType(typeof(Review))]
        [HttpPost]
        [Authorize]
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

        ///Objective: Create a method that allow us to delete the selected review by entering a interger value of the selected review id
        /// <summary>
        /// Remove the selected the review from the database
        /// </summary>
        /// <param name="id">reviewID</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        ///<example>POST: api/ReviewData/DeleteReview/{id}</example>
        [ResponseType(typeof(Review))]
        [HttpPost]
        [Authorize]
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