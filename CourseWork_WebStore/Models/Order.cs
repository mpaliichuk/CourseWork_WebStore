using System;
using System.ComponentModel.DataAnnotations;

namespace CourseWork_WebStore.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }


        public User User { get; set; } 
        public ICollection<OrderItem> OrderItems { get; set; }
    }

}

