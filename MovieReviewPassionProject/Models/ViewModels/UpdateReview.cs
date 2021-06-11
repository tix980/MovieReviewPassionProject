using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieReviewPassionProject.Models.ViewModels
{
    public class UpdateReview
    {
        public ReviewDto SelectedReview { get; set; }
        public List<MovieDto> MovieOptions { get; set; }
    }
}