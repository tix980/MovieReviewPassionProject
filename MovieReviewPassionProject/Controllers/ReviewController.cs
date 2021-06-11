using MovieReviewPassionProject.Models;
using MovieReviewPassionProject.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MovieReviewPassionProject.Controllers
{
    public class ReviewController : Controller
    {

        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static ReviewController()
        {
            
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44336/api/"); 
        }

        // GET: Review/List
        public ActionResult List()
        {
            //Accessing a list of reviews from ReviewData ListReviews() method.
            //curl https://localhost:44336/api/ReviewData/ListReviews
            string url = "ReviewData/ListReviews";
            HttpResponseMessage response = client.GetAsync(url).Result;

            List<ReviewDto> reviews = response.Content.ReadAsAsync<List<ReviewDto>>().Result;

            return View(reviews);

        }

        // GET: Review/Details/{id}
        public ActionResult Details(int id)
        {
            //Accessing a selected review info from the reviews table with FindReview(int id) method
            // curl https://localhost:44336/api/ReviewData/FindReview /{ id}
            string url = "ReviewData/FindReview/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            ReviewDto selectedReview = response.Content.ReadAsAsync<ReviewDto>().Result;

            return View(selectedReview);
        }

        // GET: Review/New
        public ActionResult New()
        {
            //Access a list of movies that can be associated(select) with the new review
            string url = "MovieData/ListMovies";
            HttpResponseMessage response = client.GetAsync(url).Result;
            List<MovieDto> movieOptions = response.Content.ReadAsAsync<List<MovieDto>>().Result;
            return View(movieOptions);
        }

        // POST: Review/Create
        [HttpPost]
        public ActionResult Create(Review review)
        {
            //creating an review json data, turn into a string, then eject it into the database
            //curl -d @movie.json -H "Content-Type:application/json" https://localhost:44336/api/ReviewData/AddReview
            string url = "ReviewData/AddReview";

            string jsonpayload = jss.Serialize(review);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Review/Edit/{id}
        public ActionResult Edit(int id)
        {
            //////Instantiating the UpdateReview ViewModel
            UpdateReview ViewModel = new UpdateReview();

            //Accessing the selected review that we want to update
            string url = "reviewdata/findreview/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ReviewDto selectedreview = response.Content.ReadAsAsync<ReviewDto>().Result;
            ViewModel.SelectedReview = selectedreview;

            //Accessing all the exisiting movies' information from the movies database
            url = "MovieData/ListMovies";
            response = client.GetAsync(url).Result;
            List<MovieDto> movieOptions = response.Content.ReadAsAsync<List<MovieDto>>().Result;
            ViewModel.MovieOptions = movieOptions;

            return View(ViewModel);
        }

        // POST: Review/Update/{id}
        [HttpPost]
        public ActionResult Update(int id, Review review)
        {
            //Use the UpdateReview(int id) Method to update the selected review's information
            string url = "ReviewData/UpdateReview/" + id;
            string jsonpayload = jss.Serialize(review);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Review/DeleteConfirm/{id}
        public ActionResult DeleteConfirm(int id)
        {
            //Accessing a delete confirm page that display the full detail of the selected review
            string url = "reviewdata/findreview/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ReviewDto selectedreview = response.Content.ReadAsAsync<ReviewDto>().Result;
            return View(selectedreview);
        }

        // POST: Review/Delete/{id}
        [HttpPost]
        public ActionResult Delete(int id)
        {
            //Accessing the DeleteReview(int id) method to delete the selected review
            string url = "ReviewData/DeleteReview/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
