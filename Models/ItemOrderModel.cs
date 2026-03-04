using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class ItemOrderModel
    {
        [Key]
        public int Orderid{get; set;}
        public int Customerid{get; set;}
        public int Itemid{get; set;}
        public int Quantity{get; set;}
        public int UnitCost{get; set;}
        public int TotalCost{get; set;}
        public int a_stock{get; set;}
    }
}