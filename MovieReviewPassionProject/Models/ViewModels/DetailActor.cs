using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieReviewPassionProject.Models.ViewModels
{
    public class DetailActor
    {
        public ActorDto selectedActor { get; set; }
        public List<MovieDto> relatedMovies { get; set; }
    }
}