using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MovieReviewPassionProject.Models
{
    public class Actor
    {
        [Key]
        public int ActorId { get; set; }
        public string ActorName { get; set; }
        public int ActorAge { get; set; }
        public string Education { get; set; }
        public int RewardCount { get; set; }
        public bool ActorPoster { get; set; }
        public string ActorPosterExtension { get; set; }

        //one movie can belong to many actors
        public ICollection<Movie> Movies { get; set; }
    }

    public class ActorDto
    {
        public int ActorId { get; set; }
        public string ActorName { get; set; }
        public int ActorAge { get; set; }
        public string Education { get; set; }
        public int RewardCount { get; set; }
        public bool ActorPoster { get; set; }
        public string ActorPosterExtension { get; set; }
    }
}