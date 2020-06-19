using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.Models.Views
{
    public class UsercategoryView
    {
        public int IDUC { get; set; }
        public string NameUC { get; set; }
        public System.DateTime DaycreateUC { get; set; }
        public bool StatusUC { get; set; }
        public decimal PriceUC { get; set; }
        public int IDU { get; set; }
        public string Status { get; set; }
        public string Statusbtn { get; set; }
        public string NameU { get; set; }
    }
}