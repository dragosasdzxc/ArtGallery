using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.Models.Views
{
    public class AuctionhistoryView
    {
        public int IDAH { get; set; }
        public int IDU { get; set; }
        public int IDA { get; set; }
        public int IDUA { get; set; }
        public decimal PriceAu { get; set; }
        public string NameU { get; set; }
        public string NameA { get; set; }
        public string NameUA { get; set; }
    }
}