using ReadLineAcademy.Models.EntityModels;
using ReadLineAcademy.Models.ViewModels;
using ReadLineAcademy.Repositories.Absractions.Base;
using ReadLineAcademy.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace ReadLineAcademy.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]

        public OrderVM OrderVM { get; set; }
       public OrderController(IUnitOfWork  unitOfWork)
          {
                _unitOfWork = unitOfWork;       
          }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int orderId)
        {

            OrderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(data=>data.Id==orderId,includeProperties:"ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(data=>data.OrderId==orderId,includeProperties:"Product"),
            };
            return View(OrderVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetail([Bind("OrderHeader")] OrderVM orderVM)
        {


            var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(data => data.Id == orderVM.OrderHeader.Id,tracked:false);
            orderHeaderFromDb.Name= orderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber= orderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress= orderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City= orderVM.OrderHeader.City;
            orderHeaderFromDb.State= orderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode= orderVM.OrderHeader.PostalCode;
            if(orderVM.OrderHeader.Carrier!=null)
            {
                orderHeaderFromDb.Carrier= orderVM.OrderHeader.Carrier;
            }
               
            if(orderVM.OrderHeader.TrackingNumber!=null)
            {
                orderHeaderFromDb.TrackingNumber= orderVM.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details Update Successfully";
            return RedirectToAction("Details","Order",new {orderId=orderHeaderFromDb.Id});

        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess); 
            _unitOfWork.Save();
            TempData["Success"] = "Order Status Update Successfully";
            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult ShipOrder()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(data => data.Id == OrderVM.OrderHeader.Id, tracked: false);
            orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeaderFromDb.TrackingNumber= OrderVM.OrderHeader.TrackingNumber;
            orderHeaderFromDb.OrderStatus = SD.StatusShipped;
            orderHeaderFromDb.ShippingDate = DateTime.Now;
            if(orderHeaderFromDb.PaymentStatus==SD.PaymentStatusDelayedPayment)
            {
                orderHeaderFromDb.PaymentDueDate= DateTime.Now.AddDays(30);
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["Success"] = "Order Shipped Successfully";
            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
       
        public IActionResult CancelOrder()
        {

            var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(data => data.Id == OrderVM.OrderHeader.Id);

            if (orderHeaderFromDb.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent=orderHeaderFromDb.PaymentIntentId
                    //Amount= (long?)orderHeaderFromDb.OrderTotal,
                    //Charge= "ch_3Netc0BBXgaBoWLr1jXQoUla"

                };

                // Check if PaymentIntentId is not null before adding it to options
                //if (!string.IsNullOrEmpty(orderHeaderFromDb.PaymentIntentId))
                //{
                  //  options.PaymentIntent = orderHeaderFromDb.PaymentIntentId;
                   
                //}

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderHeaderFromDb.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
                else
                {
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderFromDb.Id, SD.StatusCancelled, SD.StatusCancelled);
                }

                _unitOfWork.Save();

                TempData["Success"] = "Order Cancelled Successfully";
                return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });

            
        }
        [ActionName("Details")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details_PAY_NOW()
        {
            OrderVM.OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            OrderVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(data => data.OrderHeader.Id == OrderVM.OrderHeader.Id, includeProperties: "Product");
            //stripe logic
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in OrderVM.OrderDetail)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }


            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePayment(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentConfirmation(int orderHeaderId)
        {

            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderHeaderId);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                //this is an order by company

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePayment(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }


            return View(orderHeaderId);
        }



        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;
            orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            if(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;

                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.OrderHeader.GetAll(data=>data.ApplicationId==claim.Value,includeProperties: "ApplicationUser");
            }
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(data => data.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(data => data.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(data => data.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(data => data.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }
            return Json(new {data=orderHeaders});   
        }
        #endregion
    }
}
