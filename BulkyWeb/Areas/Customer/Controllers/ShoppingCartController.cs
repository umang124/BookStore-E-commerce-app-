using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var shoppingCartVM = new ShoppingCartVM();
            shoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(x => x.ApplicationUserId == userId, "Product");

            foreach(var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderTotal = cart.Price * cart.Count + shoppingCartVM.OrderTotal;
            }

            return View(shoppingCartVM);
        }

        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(x => x.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(x => x.Id == cartId);

            if (cartFromDb.Count <= 1)
            {
                _unitOfWork.ShoppingCartRepository.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Summary()
        {
            return View();
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(x => x.Id == cartId);
            _unitOfWork.ShoppingCartRepository.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        private double GetPriceBasedOnQuantity(ShoppingCart cart)
        {
            if (cart.Count <= 50)
            {
                return cart.Product.Price;
            } 
            else if (cart.Count <= 100)
            {
                return cart.Product.Price50;
            }
            else
            {
                return cart.Product.Price100;
            }
        }
    }
}
