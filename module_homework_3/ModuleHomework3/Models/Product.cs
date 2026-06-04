using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModuleHomework3.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Category")]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Название товара")]
        public string Name { get; set; } = "";

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Цена не может быть отрицательной")]
        [Display(Name = "Цена (руб.)")]
        public decimal Price { get; set; }

        public Category? Category { get; set; }
    }
}
