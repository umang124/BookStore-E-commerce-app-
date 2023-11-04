using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfwork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfwork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.CategoryRepository.GetAll()
                                                       .Select(x => new SelectListItem
                                                       {
                                                           Text = x.Name,
                                                           Value = x.Id.ToString()
                                                       });
            ProductVM productVM = new ProductVM();
            productVM.Product = new Product();
            productVM.CategoryList = CategoryList;

            return View(productVM);
        }
        [HttpPost]
        public IActionResult Create(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }

                _unitOfWork.ProductRepository.Add(productVM.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? productFromDb = _unitOfWork.ProductRepository.Get(x => x.Id == id);

            if (productFromDb == null)
            {
                return NotFound();
            }

            IEnumerable<SelectListItem> CategoryList = _unitOfWork.CategoryRepository.GetAll()
                                                       .Select(x => new SelectListItem
                                                       {
                                                           Text = x.Name,
                                                           Value = x.Id.ToString()
                                                       });

            ProductVM productVM = new ProductVM();
            productVM.Product = productFromDb;
            productVM.CategoryList = CategoryList;

            return View(productVM);
        }

        [HttpPost]
        public IActionResult Edit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        // delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                _unitOfWork.ProductRepository.Update(productVM.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var objProductList = _unitOfWork.ProductRepository.GetAll(includeProperties:"Category");

            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productToBeDeleted = _unitOfWork.ProductRepository.Get(x => x.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            if (!string.IsNullOrEmpty(productToBeDeleted.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            _unitOfWork.ProductRepository.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
