using System.ComponentModel.DataAnnotations;

namespace CourseWork_WebStore.ViewModels
{
    public class CreateOrderViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
