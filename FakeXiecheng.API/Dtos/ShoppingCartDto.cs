using System;
using System.Collections.Generic;
using FakeXiecheng.API.Models;

namespace FakeXiecheng.API.Dtos
{
    public class ShoppingCartDto
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public ICollection<LineItemDto> ShoppingCartItem { get; set; }
    }
}