using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class vm_OrderModel
    {
        public int Customerid{get; set;}
        public int Orderid{get; set;}
        public int Itemid{get; set;} 
        public int Quantity{get; set;}
        public int UnitCost{get; set;}
        public int TotalCost{get; set;}
        public int TotalStock{get; set;}
        public string ItemName{get; set;}
        public string? Image{get; set;}
        public IFormFile? ItemImage{get; set;}
    }
}