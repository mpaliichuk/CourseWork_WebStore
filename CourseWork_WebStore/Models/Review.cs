using System;
using System.ComponentModel.DataAnnotations;

namespace CourseWork_WebStore.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Rating { get; set; }

        [Required]
        [StringLength(100)]
        public string Comment { get; set; }


        public User User { get; set; }
        public Product Product { get; set; }

    }

}

