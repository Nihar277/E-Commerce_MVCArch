using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC.Models;
using MVC.Repositories;

namespace MVC.Controllers
{
    // [Route("[controller]")]
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly CustomerRepository customerRepository;

        public CustomerController(ILogger<CustomerController> logger, CustomerRepository repository)
        {
            _logger = logger;
            customerRepository=repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(vm_Login login)
        {
            CustomerModel customer1= await customerRepository.Login(login);
            if (ModelState.IsValid)
            {
                if(customer1.Customerid >= 0)
                {
                    HttpContext.Session.SetString("CustomerData", JsonSerializer.Serialize(customer1));
                    Console.WriteLine("Customer Data: " + JsonSerializer.Serialize(customer1));
                    return new JsonResult(new {success=true, customerData=customer1});
                }
                else
                {
                    return new JsonResult(new {success=false, message="Username or password is incorrect!"});
                }
            }
            return View(login);
        }

        public async Task<IActionResult> GetCustomer(int id)
        {
            CustomerModel customer=await customerRepository.GetCustomerById(id);
            return Json(customer);
        }

        public IActionResult UserDashboard()
        {
            return View();
        }

        public IActionResult ItemDashboard()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}