using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieReviewPassionProject.Models
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }
        public string AuthorName { get; set; }
        public string StarRatings { get; set; }
        public string ReviewDetail { get; set; }
        public DateTime ReviewDate { get; set; }

        //one review can only belong to one movie
        [ForeignKey("Movie")]
        public int MovieID { get; set; }
        public virtual Movie Movie { get; set; }
    }

    public class ReviewDto
    {
        public int MovieID { get; set; }
        public string MovieName { get; set; }
        public int ReviewID { get; set; }
        public string AuthorName { get; set; }
        public string StarRatings { get; set; }
        public string ReviewDetail { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}