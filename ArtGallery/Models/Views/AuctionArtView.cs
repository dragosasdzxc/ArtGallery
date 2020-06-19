using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.Models.Views
{
    public class AuctionArtView
    {
        public List<AuctionhistoryView> ahv { get; set; }
        public ArtView av { get; set; }
        public int currentPrice { get; set; }
    }
}