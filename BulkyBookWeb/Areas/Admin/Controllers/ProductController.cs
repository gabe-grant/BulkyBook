
using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            // if it's null or zero the db will know to create a product
            if (id == null || id == 0)
            {
                // ViewBag internally inserts data into the ViewData dictionary. So the key of ViewData and property of ViewBag must NOT match.
                // ViewBag and ViewData are not ideal because in such cases are views are not tightly binded to one particular model. One View One Model
                // If you wanted to make one model for two different views (like insert/update "upsert") you can create a ViewModel

                // ViewBag.CategoryList = CategoryList;
                // ViewData["CoverTypeList"] = CoverTypeList;
                return View(productVM);
            }
            else 
            {
                // update the product

            }


            return View(productVM);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRothPath = _webHostEnvironment.WebRootPath;
                // if it NOT null that means a file was uploaded
                if (file != null)
                {
                    // a new GUID is a generated new file name, incase someone uploads a two file of the same name
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRothPath, @"images\products");
                    // renaming the file but keeping the same extension
                    var extension = Path.GetExtension(file.FileName);
                    // now we copy the file that was uploaded inside the images folder
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    { 
                        file.CopyTo(fileStreams);
                    }
                    // save it all in the database
                    obj.Product.ImageUrl = @"\images\products" + fileName + extension;
                }
                _unitOfWork.Product.Add(obj.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var coverTypeFromDbFirst = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);

            if (coverTypeFromDbFirst == null)
            {
                return NotFound();
            }

            return View(coverTypeFromDbFirst);
        }

        // POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.CoverType.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "CoverType deleted successfully";
            return RedirectToAction("Index");
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll() 
        { 
            var productList =  _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            return Json(new { data = productList });
        }
        // this API loads the data from the db and makes it available for the call in the Product Index View
        // https://localhost:44322/Admin/Product/getall
        #endregion

    }
}
