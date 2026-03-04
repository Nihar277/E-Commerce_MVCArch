using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class CustomerModel
    {
        [Key]
        public int Customerid{get; set;}
        public string Email{get; set;}
        public string Password{get; set;}
        public string Name{get; set;}
    }
}