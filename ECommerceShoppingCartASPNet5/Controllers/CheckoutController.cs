using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerceShoppingCartASPNet5.Helpers;
using ECommerceShoppingCartASPNet5.Models;
using Stripe;

namespace ECommerceShoppingCartASPNet5.Controllers
{
    public class CheckoutController : Controller
    {
        
        [TempData]
        public string TotalAmount { get; set; }
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
            ViewBag.DollarAmount = cart.Sum(item => item.Product.Price * item.Quantity);
            ViewBag.total = Math.Round(ViewBag.DollarAmount, 2)*100;
            ViewBag.total = Convert.ToInt64(ViewBag.total);
             long total = ViewBag.total;
            TotalAmount = total.ToString();
            return View();
        }
        [HttpPost]
        public IActionResult Processing(string stripeToken, string stripeEmail)
        {
            var optionsCust = new CustomerCreateOptions
            {
                Email = stripeEmail,
                Name = "Robert",
                Phone = "04-234567"

            };
            var serviceCust = new CustomerService();
            Customer customer = serviceCust.Create(optionsCust);
            var optionsCharge = new ChargeCreateOptions
            {
               /*Amount = HttpContext.Session.GetLong("Amount")*/
               Amount = Convert.ToInt64(TempData["TotalAmount"]),
                Currency = "USD",
                Description = "Buying Flowers",
                Source = stripeToken,
                ReceiptEmail = stripeEmail,
                            
            };
            var service = new ChargeService();
            Charge charge = service.Create(optionsCharge);
            if (charge.Status == "succeeded")
            {
                string BalanceTransactionId = charge.BalanceTransactionId;
                ViewBag.AmountPaid =Convert.ToDecimal(charge.Amount) % 100 / 100 + (charge.Amount)/100 ;
                ViewBag.BalanceTxId = BalanceTransactionId;
                ViewBag.Customer = customer.Name;
              //return View();
            }
           
            return View();
        }

    }
}
