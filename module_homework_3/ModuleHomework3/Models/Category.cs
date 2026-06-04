using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModuleHomework3.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название категории")]
        public string Name { get; set; } = "";

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
