using MovieReviewPassionProject.Models;
using MovieReviewPassionProject.Models.ViewModels;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MovieReviewPassionProject.Controllers
{
    public class MovieController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static MovieController(){
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false
            };
            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44336/api/");
         }

        /// <summary>
        /// create a method that can obtain authentication cookie(AspNet.ApplicationCookie) and be instantiated in other methods.
        /// This can allow us to do changes to the database after we have logged in the server as an admin.
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;

        }


        // GET: Movie/List
        public ActionResult List()
        {   
            //Accessing a list of movies from ListMovies method
            //curl https://localhost:44336/api/MovieData/ListMovies

            string url = "MovieData/ListMovies";
            HttpResponseMessage response = client.GetAsync(url).Result;
            List<MovieDto> movies = response.Content.ReadAsAsync<List<MovieDto>>().Result;

            return View(movies);
        }

        // GET: Movie/FindMovie/5
        public ActionResult Details(int id)
        {
            //Accessing a selected movie info with FindMovie(int id) method from the movieData api controller
            //curl https://localhost:44336/api/MovieData/FindMovie/{id}
            DetailMovie ViewModel = new DetailMovie();
            string url = "MovieData/FindMovie/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            MovieDto selectedmovie = response.Content.ReadAsAsync<MovieDto>().Result;
            ViewModel.SelectedMovie = selectedmovie;

            //Accessing a list of reviews that are related to the selected movie
            url = "reviewdata/ListReviewsForMovie/" + id;
            response = client.GetAsync(url).Result;
            List<ReviewDto> AllReviews = response.Content.ReadAsAsync<List<ReviewDto>>().Result;
            ViewModel.relatedReviews = AllReviews;

            //Accessing a list of actors that are related to the selected movie
            url = "actorData/ListActorsForMovie/" + id;
            response = client.GetAsync(url).Result;
            List<ActorDto> relatedActors = response.Content.ReadAsAsync<List<ActorDto>>().Result;
            ViewModel.relatedActors = relatedActors;

            //Accessing a list of actors that are not related to the selected movie
            url = "actorData/ListActorsNotInMovie/" + id;
            response = client.GetAsync(url).Result;
            List<ActorDto> AvailableActors = response.Content.ReadAsAsync<List<ActorDto>>().Result;
            ViewModel.AvailableActors = AvailableActors;

            return View(ViewModel);
        }

        //POST: Movie/Associate/{animalid}
        [HttpPost]
        [Authorize]
        public ActionResult Associate(int id,int ActorId)
        {
            GetApplicationCookie();
            //Associate a selected Movie and a selected Actor 
            string url = "MovieData/AssociateMovieWithActor/" + id +"/" + ActorId;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("details/" +id);
        }

        //GET: Movie/NotAssociate/{id}?ActorId ={ActorId}
        [HttpGet]
        [Authorize]
        public ActionResult NotAssociate(int id, int ActorId)
        {
            GetApplicationCookie();
            // Seperate the connection between a selected Movie and a selected Actor 
            Debug.WriteLine("Attempting to unassociate movie :" + id + " with actor: " + ActorId);
            string url = "MovieData/NotAssociateMovieWithActor/" + id + "/" + ActorId;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("details/" + id);
        }

        // GET: Movie/Create
        [Authorize]
        public ActionResult New()
        {
            //information about all reviews
            //GET api/reviewData/ListReviews
            string url = "ReviewData/ListReviews";
            HttpResponseMessage response = client.GetAsync(url).Result;
            List<ReviewDto> AllReviews = response.Content.ReadAsAsync<List<ReviewDto>>().Result;
            return View(AllReviews);
        }

        // POST: Movie/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Movie movie)
        {
            GetApplicationCookie();
            //curl -d @movie.json -H "Content-Type:application/json" https://localhost:44336/api/MovieData/AddReview

            string url = "MovieData/addmovie";

            // Turn movie model into a json object
            string jsonpayload = jss.Serialize(movie);

            Debug.WriteLine(jsonpayload);

            // Turn json payload into a string
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            // eject payload into the database, the result shows up right away
            HttpResponseMessage response =  client.PostAsync(url, content).Result;
            //If something is wrong with this method(like mispell of url string, it will direct user to an error page.
            Debug.WriteLine(content);
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

        // GET: Movie/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            //Accessing the detail information of the selected movie
            DetailMovie ViewModel = new DetailMovie();
            string url = "moviedata/findmovie/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            MovieDto selectedmovie = response.Content.ReadAsAsync<MovieDto>().Result;
            ViewModel.SelectedMovie = selectedmovie;

            //Accessing All reviews from the review table
            url = "reviewdata/listreviews/";
             response = client.GetAsync(url).Result;
            List<ReviewDto> AllReviews = response.Content.ReadAsAsync<List<ReviewDto>>().Result;
            ViewModel.relatedReviews = AllReviews;


            return View(ViewModel);
        }

        // POST: Movie/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Movie movie, HttpPostedFileBase MoviePoster)
        {
            GetApplicationCookie();
            //Accessing and updating the selected movie's information  
            string url = "moviedata/updatemovie/" + id;
            string jsonpayload = jss.Serialize(movie);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
            if (response.IsSuccessStatusCode && MoviePoster != null)
            {
                url = "MovieData/UploadPoster/" + id;
                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(MoviePoster.InputStream);
                requestcontent.Add(imagecontent, "MoviePoster", MoviePoster.FileName);
                response = client.PostAsync(url, requestcontent).Result;
                return RedirectToAction("List");
            }else if(response.IsSuccessStatusCode){
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        // GET: Movie/Delete/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            //Accessing a delete confirm page that display the full detail of the selected movie
            string url = "moviedata/findMovie/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            MovieDto selectedmovie = response.Content.ReadAsAsync<MovieDto>().Result;
                
            return View(selectedmovie);
        }

        // POST: Movie/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            //Accessing the DeleteMovie(int id) method to delete the selected movie
            string url = "MovieData/DeleteMovie/" + id;
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
