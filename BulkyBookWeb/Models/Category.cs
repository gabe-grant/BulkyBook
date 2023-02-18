
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyBookWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        // this allows us to display a name in what ever fashion we want instead of using just the specific Property name
        [DisplayName("Display Order")]
        [Range(1,100, ErrorMessage ="Display order must be between 1 and 100!")]
        public int DisplayOrder { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}

/* We can create a database table using the Entity Framework Core
 * 
 * We can use the [] annotations that informs the creation of the SQL script with specifics i.e. Primary Key, etc..
 * 
 */