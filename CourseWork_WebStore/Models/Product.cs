using System;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;

namespace CourseWork_WebStore.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; } 

        [Required]
        public decimal Price { get; set; } 

        [Required]
        public int Stock { get; set; } 

        public int CategoryId { get; set; }


        public Category Category { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }

}

