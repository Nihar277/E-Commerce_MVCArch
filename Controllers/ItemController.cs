using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC.Models;
using MVC.Repositories;
using Npgsql.Replication.PgOutput;

namespace MVC.Controllers
{
    // [Route("[controller]")]
    public class ItemController : Controller
    {
        private readonly ItemRepository itemRepository;

        public ItemController(ItemRepository item)
        {
            itemRepository = item;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetItems()
        {
            List<ItemModel> item = await itemRepository.GetAllItems();
            return Json(item);
        }

        public async Task<IActionResult> GetItemById(int id)
        {
            ItemModel item = await itemRepository.GetItemById(id);
            return Json(item);
        }

        public async Task<IActionResult> Buy(int id)
        {
            ItemModel item = await itemRepository.GetItemById(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item); // Pass item data to View
        }
        
        public IActionResult ItemDashboard()
        {
            return View();
        }
        public IActionResult UserDashboard()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(ItemModel item)
        {
            try
            {
                
                if (item.ItemImage == null || item.ItemImage.Length == 0)
                {
                    return Json(new { success = false, message = "Item image required" });
                }

                var fileName = item.Name + Path.GetExtension(item.ItemImage.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "item_images");
                Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await item.ItemImage.CopyToAsync(stream);
                }
                item.Image = fileName;

                var status = await itemRepository.AddItem(item);

                if (status == 1)
                {
                    return Json(new { success = true, message = "Item Added Successfully" });
                }
                else if (status == 0)
                {
                    return Json(new { success = false, message = "Error while adding Item" });
                }
                else
                {
                    return Json(new { success = false, message = "There is some error while adding new item" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex.Message);
                return Json(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ItemModel item)
        {
            try
            {
                if (item.ItemImage != null && item.ItemImage.Length > 0)
                {
                    var fileName = item.Name + Path.GetExtension(item.ItemImage.FileName);
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "item_images");
                    Directory.CreateDirectory(folderPath);
                    var filePath = Path.Combine(folderPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await item.ItemImage.CopyToAsync(stream);
                    }
                    item.Image = fileName;
                }

                var status = await itemRepository.EditItem(item);

                if (status == 1)
                {
                    return Json(new { success = true, message = "Item Updated Successfully." });
                }
                else if (status == 0)
                {
                    return Json(new { success = false, message = "Item not found." });
                }
                else
                {
                    return Json(new { success = false, message = "There is some error while editign the item" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            int status = await itemRepository.DeleteItem(id);
            if (status == 1)
            {
                return Json(new { success = true, message = "Item Deleted Successfully" });
            }
            else
            {
                return Json(new { success = false, message = "There is some error while deleting the item." });
            }
        }
        public async Task<IActionResult> ItemPurchase(int id)
        {
            ItemModel item = await itemRepository.GetItemById(id);
            return View(item);
        }
        public async Task<IActionResult> GetOrderById(int id)
        {
            List<vm_OrderModel> vm_OrderModels=await itemRepository.GetItemOrderById(id);
            return Json(vm_OrderModels);
        }
        public IActionResult OrderPage()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddItemOrder(ItemOrderModel itemOrder)
        {
            int status= await itemRepository.AddItemOrder(itemOrder);
            if (status == 1)
            {
                return Json(new {success=true, message="Item Order Placed Successfuly"});
            }
            else
            {
                return Json(new {success=false, message="There is some error while placing the order"});
            }
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}