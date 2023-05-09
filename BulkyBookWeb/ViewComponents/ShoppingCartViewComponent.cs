using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // all we want to do is that if a user is logged in we will retrieve their session
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            
            if(claim != null) // if claim is not not null then the user is not logged in
            {
                if(HttpContext.Session.GetInt32(SD.SessionCart) != null) // if the session cart is not null then the session is already set
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionCart)); // and then send the value of the session back in the View
                }
                else // if the session is null, we need to go to the db and retrieve the count and assign it to the session
                {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                        _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);

                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
            }
            else // is the claim is null, i.e. when the user signs out or a user has not logged in once at the website
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
