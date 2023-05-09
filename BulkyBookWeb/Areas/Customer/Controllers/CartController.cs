using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Win32.SafeHandles;
using Stripe.Checkout;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty] // this automatically binds the property when we post the form, so that we have one property to use it throughout the controller
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public int OrderTotal { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // because we only want to get the shopping carts for a particular user we need create a filter condition
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(
                    u => u.ApplicationUserId == claim.Value,
                    includeProperties: "Product"),
                    OrderHeader = new()
            };
            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(
                        cart.Count,
                        cart.Product.Price,
                        cart.Product.Price50,
                        cart.Product.Price100
                    );

                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
            return View(ShoppingCartVM);
        }

		public IActionResult Summary()
		{
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // because we only want to get the shopping carts for a particular user we need create a filter condition
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(
                    u => u.ApplicationUserId == claim.Value,
                    includeProperties: "Product"),
                    OrderHeader = new()
            };
            // this will retrieve all the Application User details for our logged in user
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(
                    u => u.Id == claim.Value);
            // now we can populate all the properties inside OrderHeader
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
			ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
			ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

			foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(
                        cart.Count,
                        cart.Product.Price,
                        cart.Product.Price50,
                        cart.Product.Price100
                    );
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
		}

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
		public IActionResult SummaryPOST()
		{
            // when we submit everything we can get them from the form, but it is a good idea to start fresh and load everything from the Shopping Cart db
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // loading the shopping cart again
            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(
                    u => u.ApplicationUserId == claim.Value,
                    includeProperties: "Product");

            // when the order is placed we do have update some details in OrderHeader
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;

			foreach (var cart in ShoppingCartVM.ListCart)
			{
				cart.Price = GetPriceBasedOnQuantity(
						cart.Count,
						cart.Product.Price,
						cart.Product.Price50,
						cart.Product.Price100
					);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

            // Entity is rad, once an OrderHeader is saved to the Db it will automatically populate it inside of the OrderHeader object here
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

			// create the order details for all the items in the shopping cart
			foreach (var cart in ShoppingCartVM.ListCart)
			{
				// based on all the items in the cart we need to populate the order detail
				OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
			}

            // Stripe settings
            var domain = "https://localhost:44322/"; // you can get this URL from the IWebHostEnvironment, you would have to add that using DI
			var options = new SessionCreateOptions // when working with Stripe we will be creating a session and adding details on the options var
			{
				// LineItems basically represents all the items in your shopping cart, rather than adding a static list, we can loop through our shopping cart using a foreach
				LineItems = new List<SessionLineItemOptions>(), 
				Mode = "payment",
				// Customer area, Cart controller, OrderConfirmation action method, passing the OrderHeaderId. A redirection url after successful payment
				SuccessUrl = domain+$"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                // if canceled the custoemr is redirected to the path below
				CancelUrl = domain+"cusotmer/cart/index",
			};

            foreach(var item in ShoppingCartVM.ListCart) // creating one variable for each item in the shopping cart 
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions // confifure all the price data
                    {
                        UnitAmount = (long)(item.Price * 100), // $20.00 -> 2000 (multiply by 100, because the unit amount is done in Cents not dollars)
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title // we can add more items here like, Description = item.Product.Description,
                        },
                    },
                    Quantity = item.Count, // based on this quantity and the individual price it will do the calculation and display the grand total
                };
                options.LineItems.Add(sessionLineItem);
			}

			var service = new SessionService();
			Session session = service.Create(options);
            // updating the SessionId and PaymentIntentId properties of the OrderHeader class
            _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.Save();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);


			//_unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart); // removes the collection from the shopping cart
            //_unitOfWork.Save();
			//return RedirectToAction("Index","Home"); // redirecting to the Index action of the Home Controller
		}


        public IActionResult OrderConfirmation(int id)
        {
            // based on the Id passed in we will have to retrieve the OrderHeader from the Db, and the reason for that is because we have to check the stripe status
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == id);
			var service = new SessionService();
			Session session = service.Get(orderHeader.SessionId); // getting the order, not creating it
            // because you could potentially navigate to the url manually, we configure it so we know that a payment is ACTUALLY done
            if(session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }
            // before remove the shopping cart data we will need to retrieve it again
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            HttpContext.Session.Clear();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts); // removes the collection from the shopping cart
            _unitOfWork.Save();
            
            return View(id); // returning to the View of the id passed into OrderConfirmation
		}


		public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

		public IActionResult Minus(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            if (cart.Count <= 1)
            {
				_unitOfWork.ShoppingCart.Remove(cart);
                var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;
                HttpContext.Session.SetInt32(SD.SessionCart, count);
            }
            else
            {
				_unitOfWork.ShoppingCart.DecrementCount(cart, 1);
			}
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Remove(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
			_unitOfWork.ShoppingCart.Remove(cart);
			_unitOfWork.Save();

            var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.SessionCart, count);

            return RedirectToAction(nameof(Index));
		}


		private double GetPriceBasedOnQuantity(double quantity, double price, double price50, double price100)
        {
            if (quantity <= 50)
            {
				return price;
			}
            else
            {
				if (quantity <= 100)
				{
					return price50;
				}
                return price100;
			}
        }
    }
}
