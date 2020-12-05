using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FakeXiecheng.API.Models
{
    public class ShoppingCart
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
        
        public ICollection<LineItem> ShoppingCartItem { get; set; }
    }
}