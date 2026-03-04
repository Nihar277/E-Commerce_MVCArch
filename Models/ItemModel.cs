using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class ItemModel
    {
        [Key]
        public int? Itemid{get; set;}
        public string Name{get; set;}
        public string Category{get; set;}
        public string? Image{get; set;}
        public IFormFile? ItemImage{get; set;}
        public int UnitCost{get; set;}
        public int TotalStock{get; set;}
        public int CurrentStock{get; set;}
    }
}