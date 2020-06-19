using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.Models.Views
{
    public class CartView
    {
        public int IDCA { get; set; }
        public int IDU { get; set; }
        public int IDA { get; set; }
        public bool StatusCA { get; set; }
        public string NameU { get; set; }
        public string NameA { get; set; }
        public decimal PriceA { get; set; }

    }

    public class CartDetailView
    {
        public List<CartView> cartdetail { get; set; }
    }
}