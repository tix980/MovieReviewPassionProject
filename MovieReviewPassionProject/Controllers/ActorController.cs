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
    public class ActorController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static ActorController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false
            };
            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44336/api/");
        }

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

        // GET: Actor/List
        public ActionResult List()
        {
            //Accessing a list of actors from  ListActors() Method
            string url = "ActorData/ListActors";
            HttpResponseMessage response = client.GetAsync(url).Result;
            List<ActorDto> actors = response.Content.ReadAsAsync<List<ActorDto>>().Result;
            return View(actors);
        }

        // GET: Actor/Details/{id}
        public ActionResult Details(int id)
        {
            //Accessing a selected actor info from the actors table with FindActor(int id) method
            DetailActor ViewModel = new DetailActor();
            string url = "ActorData/FindActor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ActorDto selectedactor = response.Content.ReadAsAsync<ActorDto>().Result;
            ViewModel.selectedActor = selectedactor;

            //show all movies that are related to one actor
            url = "MovieData/ListMoviesForActor/" + id;
            response = client.GetAsync(url).Result;
            List<MovieDto> relatedMovies = response.Content.ReadAsAsync<List<MovieDto>>().Result;
            ViewModel.relatedMovies = relatedMovies;

            return View(ViewModel);

        }

        // GET: Actor/New
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST: Actor/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Actor actor)
        {
            GetApplicationCookie();
            //creating an actor json data, turn into a string, then eject it into the database
            string url = "ActorData/AddActor";
            string jsonpayload = jss.Serialize(actor);
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

        // GET: Actor/Edit/{id}
        [Authorize]
        public ActionResult Edit(int id)
        {
            //Accessing the selected actor that we want to update
            string url = "ActorData/FindActor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ActorDto selectedactor = response.Content.ReadAsAsync<ActorDto>().Result;
            return View(selectedactor);
        }

        // POST: Actor/Update/{id}
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Actor actor)
        {
            GetApplicationCookie();
            //Use the UpdateActor(int id) Method to update the selected actor's information
            string url = "ActorData/UpdateActor/" + id;
            string jsonpayload = jss.Serialize(actor);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
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


        // GET: Actor/DeleteConfirm/{id}
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            //Accessing a delete confirm page that display the full detail of the selected actor
            string url = "ActorData/FindActor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ActorDto selectedactor = response.Content.ReadAsAsync<ActorDto>().Result;
            return View(selectedactor);
        }

        // POST: Actor/Delete/{id}
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            //Accessing the DeleteActor(int id) method to delete the selected actor
            string url = "actorData/DeleteActor/" + id;
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
