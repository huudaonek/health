    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using insure_fixlast.Data;
    using insure_fixlast.Models;
    using Microsoft.AspNetCore.Http;
    using System.Text.Json;

    namespace insure_fixlast.Controllers
    {
        public class ServicesController : Controller
        {
            private readonly insure_fixlastContext _context;

            public ServicesController(insure_fixlastContext context)
            {
                _context = context;
            }

            // GET: Services
            public async Task<IActionResult> Index()
            {
                return View(await _context.Service.ToListAsync());
            }

            public async Task<IActionResult> UserService()
            {
                return View(await _context.Service.ToListAsync());
            }

            // GET: Services/Details/5
            public async Task<IActionResult> Details(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var service = await _context.Service.FirstOrDefaultAsync(m => m.Id == id);
                if (service == null)
                {
                    return NotFound();
                }

                return View(service);
            }

            // GET: Services/Create
            public IActionResult Create()
            {
                return View();
            }

            // POST: Services/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("Id,Description,Name,Price,Claim,Time")] Service service)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(service);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(service);
            }

            // GET: Services/Edit/5
            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var service = await _context.Service.FindAsync(id);
                if (service == null)
                {
                    return NotFound();
                }
                return View(service);
            }

            // POST: Services/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Name,Price,Claim,Time")] Service service)
            {
                if (id != service.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(service);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ServiceExists(service.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(service);
            }

            // GET: Services/Delete/5
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var service = await _context.Service.FirstOrDefaultAsync(m => m.Id == id);
                if (service == null)
                {
                    return NotFound();
                }

                return View(service);
            }

            // POST: Services/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var service = await _context.Service.FindAsync(id);
                if (service != null)
                {
                    _context.Service.Remove(service);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool ServiceExists(int id)
            {
                return _context.Service.Any(e => e.Id == id);
            }

            // POST: Services/ReviewOrder
            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult ReviewOrder(List<int> selectedServices, Dictionary<int, int> quantities)
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }
                if (selectedServices == null || !selectedServices.Any())
                {
                    TempData["ErrorMessage"] = "Please select at least one service.";
                    return RedirectToAction(nameof(Index));
                }

                var selectedServiceDetails = new List<Service>();
                foreach (var serviceId in selectedServices)
                {
                    var service = _context.Service.FirstOrDefault(s => s.Id == serviceId);
                    if (service != null)
                    {
                        selectedServiceDetails.Add(service);
                    }
                }
                HttpContext.Session.SetString("SelectedServices", JsonSerializer.Serialize(selectedServiceDetails));
                HttpContext.Session.SetString("ServiceQuantities", JsonSerializer.Serialize(quantities));

                return RedirectToAction(nameof(Checkout));
            }

            public IActionResult Checkout()
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                int? userId = null;

                if (!string.IsNullOrEmpty(userIdString))
                {
                    userId = int.Parse(userIdString);
                }

                if (userId == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }

                // Lấy thông tin công ty và tên công ty dựa trên ID người dùng
                var (companyName, _) = GetCompanyInfoByUserId(userId.Value);

                var selectedServicesJson = HttpContext.Session.GetString("SelectedServices");
                var serviceQuantitiesJson = HttpContext.Session.GetString("ServiceQuantities");

                if (string.IsNullOrEmpty(selectedServicesJson) || string.IsNullOrEmpty(serviceQuantitiesJson))
                {
                    TempData["ErrorMessage"] = "No services found in the order. Please select services again.";
                    return RedirectToAction(nameof(Index));
                }

                var selectedServices = JsonSerializer.Deserialize<List<Service>>(selectedServicesJson);
                var serviceQuantities = JsonSerializer.Deserialize<Dictionary<int, int>>(serviceQuantitiesJson);

                var checkoutViewModel = new CheckoutViewModel
                {
                    CompanyName = companyName,
                    SelectedServices = selectedServices,
                    ServiceQuantities = serviceQuantities
                };

                return View(checkoutViewModel);
            }

            private (string, int?) GetCompanyInfoByUserId(int userId)
            {
                var company = _context.Company.Include(c => c.Account).FirstOrDefault(c => c.AccountId == userId);
                var companyId = company?.Id;
                var companyName = company?.Name;
                return (companyName, companyId);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> PlaceOrder(string paymentMethod)
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }

                var companyId = GetCompanyIdByAccountId(int.Parse(userId));
                if (companyId == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var selectedServicesJson = HttpContext.Session.GetString("SelectedServices");
                var serviceQuantitiesJson = HttpContext.Session.GetString("ServiceQuantities");

                if (string.IsNullOrEmpty(selectedServicesJson) || string.IsNullOrEmpty(serviceQuantitiesJson))
                {
                    TempData["ErrorMessage"] = "No services found in the order. Please select services again.";
                    return RedirectToAction(nameof(Index));
                }

                var selectedServices = JsonSerializer.Deserialize<List<Service>>(selectedServicesJson);
                var serviceQuantities = JsonSerializer.Deserialize<Dictionary<int, int>>(serviceQuantitiesJson);

                var order = new Order
                {
                    PriceTotal = 0, // Tạm thời để giá trị này là 0, bạn có thể tính lại sau khi thêm các chi tiết đơn hàng
                    CompanyId = companyId.Value,
                    Payment = Enum.Parse<Payment>(paymentMethod.ToLower()) // Chuyển chuỗi paymentMethod thành giá trị enum Payment
                };

                _context.Order.Add(order);
                await _context.SaveChangesAsync();

                decimal totalPrice = 0;

                foreach (var service in selectedServices)
                {
                    var quantity = serviceQuantities.ContainsKey(service.Id) ? serviceQuantities[service.Id] : 1;
                    var total = service.Price * quantity;
                    totalPrice += total;

                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ServiceId = service.Id,
                        Quantity = quantity
                    };

                    _context.OrderDetail.Add(orderDetail);
                }

                order.PriceTotal = totalPrice; // Cập nhật lại giá trị tổng sau khi tính toán
                await _context.SaveChangesAsync();

                // Xóa session sau khi đặt hàng thành công
                HttpContext.Session.Remove("SelectedServices");
                HttpContext.Session.Remove("ServiceQuantities");

                return RedirectToAction(nameof(Thanks));
            }

            public IActionResult GetPurchasedPackages()
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                int? userId = null;

                if (!string.IsNullOrEmpty(userIdString))
                {
                    userId = int.Parse(userIdString);
                }

                if (userId == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }

                // Lấy thông tin công ty dựa trên ID người dùng
                var (_, companyId) = GetCompanyInfoByUserId(userId.Value);

                if (companyId == null)
                {
                    TempData["ErrorMessage"] = "Company not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Lấy tất cả đơn hàng của công ty
                var orders = _context.Order
                    .Include(o => o.Details)
                        .ThenInclude(od => od.Service)
                    .Where(o => o.CompanyId == companyId)
                    .OrderByDescending(o => o.Id)
                    .ToList();

                if (orders == null || !orders.Any())
                {
                    TempData["ErrorMessage"] = "No orders found.";
                    return RedirectToAction(nameof(Index));
                }

                // Gộp tất cả các dịch vụ từ các đơn hàng
                var allOrderDetails = orders.SelectMany(o => o.Details).ToList();

                return View(allOrderDetails);
            }

            public int? GetCompanyIdByAccountId(int accountId)
            {
                var company = _context.Company.FirstOrDefault(c => c.AccountId == accountId);
                return company?.Id;
            }

            public IActionResult Thanks()
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                int? userId = null;

                if (!string.IsNullOrEmpty(userIdString))
                {
                    userId = int.Parse(userIdString);
                }

                if (userId == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }

                // Lấy thông tin công ty và tên công ty dựa trên ID người dùng
                var (_, companyId) = GetCompanyInfoByUserId(userId.Value);

                // Lấy đơn hàng mới nhất của công ty
                var latestOrder = _context.Order
                    .Include(o => o.Details)
                        .ThenInclude(od => od.Service)
                    .Where(o => o.CompanyId == companyId)
                    .OrderByDescending(o => o.Id)
                    .FirstOrDefault();

                if (latestOrder == null)
                {
                    TempData["ErrorMessage"] = "No order found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(latestOrder);
            }

            // GET: Services/Assign
            public IActionResult Assign()
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString))
                {
                    return RedirectToAction("Login", "Accounts");
                }

                var userId = int.Parse(userIdString);
                var (_, companyId) = GetCompanyInfoByUserId(userId);
                if (companyId == null)
                {
                    TempData["ErrorMessage"] = "Company not found.";
                    return RedirectToAction(nameof(Index));
                }

                var employees = _context.Employee.Where(e => e.CompanyId == companyId).ToList();
                var orders = _context.Order
                    .Include(o => o.Details)
                        .ThenInclude(od => od.Service)
                    .Where(o => o.CompanyId == companyId)
                    .OrderByDescending(o => o.Id)
                    .ToList();

                var allOrderDetails = orders.SelectMany(o => o.Details).ToList();

                var viewModel = new AssignPackageViewModel
                {
                    Employees = employees,
                    OrderDetails = allOrderDetails
                };

                return View(viewModel);
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int employeeId, List<int> orderDetailIds)
        {
            if (orderDetailIds == null || orderDetailIds.Count == 0)
            {
                TempData["ErrorMessage"] = "No order details selected.";
                return RedirectToAction(nameof(GetPurchasedPackages));
            }

            try
            {
                foreach (var orderDetailId in orderDetailIds)
                {
                    var orderDetail = await _context.OrderDetail.FindAsync(orderDetailId);
                    if (orderDetail == null)
                    {
                        TempData["ErrorMessage"] = $"Order detail with ID {orderDetailId} not found.";
                        return RedirectToAction(nameof(GetPurchasedPackages));
                    }

                    if (orderDetail.EmployeeId != null)
                    {
                        TempData["ErrorMessage"] = $"Order detail with ID {orderDetailId} is already assigned to an employee.";
                        return RedirectToAction(nameof(GetPurchasedPackages));
                    }

                    orderDetail.EmployeeId = employeeId;
                }

                await _context.SaveChangesAsync(); // SaveChangesAsync after updating all order details
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while assigning packages: {ex.Message}";
                // You can log the exception for further investigation if needed
                return RedirectToAction(nameof(GetPurchasedPackages));
            }

            return RedirectToAction(nameof(GetPurchasedPackages));
        }




    }
}
