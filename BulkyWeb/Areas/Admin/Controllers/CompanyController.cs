using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var objCompanyList = _unitOfWork.CompanyRepository.GetAll();
            return View(objCompanyList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Company company)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CompanyRepository.Add(company);
                _unitOfWork.Save();
                TempData["success"] = "Company created succesfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            Company? companyFromDb = _unitOfWork.CompanyRepository.Get(x => x.Id == id);
            if (companyFromDb == null)
            {
                return NotFound();
            }
            return View(companyFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Company company)
        {
            _unitOfWork.CompanyRepository.Update(company);
            _unitOfWork.Save();
            TempData["success"] = "Company updated successfully";
            return RedirectToAction("Index");
        }

        #region API CALLS

        public IActionResult GetAll()
        {
            var objCompaniesList = _unitOfWork.CompanyRepository.GetAll();
            return Json(new { data = objCompaniesList });
        }
        public IActionResult Delete(int id)
        {
            var companyFromDb = _unitOfWork.CompanyRepository.Get(x => x.Id == id);
            if (companyFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.CompanyRepository.Remove(companyFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
