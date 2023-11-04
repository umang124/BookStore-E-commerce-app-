using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.ProductRepository.GetAll(includeProperties:"Category");
            return View(productList);
        }
        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new ShoppingCart();
            cart.ProductId = productId;
            cart.Count = 1;
            cart.Product = _unitOfWork.ProductRepository.Get(x => x.Id == productId, "Category");
            return View(cart);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Details(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            cart.ApplicationUserId = userId;

            ShoppingCart cartObj = _unitOfWork.ShoppingCartRepository.Get(x => x.ApplicationUserId == userId && x.ProductId == cart.ProductId);

            if (cartObj != null)
            {
                cartObj.Count = cart.Count + cartObj.Count;
                _unitOfWork.ShoppingCartRepository.Update(cartObj);
            } 
            else
            {
                _unitOfWork.ShoppingCartRepository.Add(cart);
            }
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}