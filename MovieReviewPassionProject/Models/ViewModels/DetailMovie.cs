using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieReviewPassionProject.Models.ViewModels
{
    public class DetailMovie
    {
        //Use it to store information to present to /movie/update{id}
        public MovieDto SelectedMovie { get; set; }
        //all reviews to choose from
        public List<ReviewDto> relatedReviews { get; set; }
        public List<ActorDto> relatedActors { get; set; }
        public List<ActorDto> AvailableActors { get; set; }
    }
}