
using System.ComponentModel.DataAnnotations;

namespace BulkyBookWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}

/* We can create a database table using the Entity Framework Core
 * 
 * We can use the [] annotations that informs the creation of the SQL script what we want 
 * 
 */