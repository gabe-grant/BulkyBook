using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        // this constructor tells our application that we need an implementation of the ApplicationDbContext
        // and remember the .UseSqlServer() method inside Program.cs is providing us with the access to the database
        // the context object from the db is assigned to the private variable available here in this class
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        // now we interact with it by retreiving the DbSet<Categories> from the db and covnverting the table to a list and assign it to the variable
        // this is beatiful becuase we don't have to write a select statement
        // we then pass this list to the Category View as an IEnumerable
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _db.Categories;
            return View(objCategoryList);
        }
        
        // GET
        public IActionResult Create()
        {
            return View();
        }

        // POST
        // the inputted form details in the Create View are added to the database connection in the Create action because of its a POST method
        // Redirecting to the Index Method Action allows us return to the Index view after successful creation of a databse entry
        // you could also redirect to a different method in a different controller by passing it as a second argument
        // server side validation with tag helpers is God mode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot match the name exactly");
            }
            if (ModelState.IsValid) { 
                _db.Categories.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
    }
}
