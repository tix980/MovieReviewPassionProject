using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MovieReviewPassionProject.Models
{
    public class Movie
    {
        [Key]
        public int MovieID { get; set; }
        public string MovieName { get; set; }
        public string MovieGenre { get; set; }
        public DateTime Year { get; set; }
        public string MovieInfo {get;set;}

        //images are in content/img/movies/{id}.{extension}
        public bool MoviePoster { get; set; }
        public string MoviePosterExtension { get; set; }

        //one movie can have many reviews
        public ICollection<Review> Reviews { get; set; }

        //one movie can have many actors
        public ICollection<Actor> Actors { get; set; }
    }
    
    public class MovieDto
    {
        public int MovieID { get; set; }
        public string MovieName { get; set; }
        public string MovieGenre { get; set; }
        public DateTime Year { get; set; }
        public string MovieInfo { get; set; }
        public bool MoviePoster { get; set; }
        public string MoviePosterExtension { get; set; }

    }
}