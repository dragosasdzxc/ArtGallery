using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.Models.Views
{
    public class ArtView
    {
        public int IDA { get; set; }
        public string NameA { get; set; }
        public int WidthA { get; set; }
        public int HeightA { get; set; }
        public int DepthA { get; set; }
        public string MaterialA { get; set; }
        public System.DateTime DaycreateA { get; set; }
        public string DescriptionA { get; set; }
        public decimal PriceA { get; set; }
        public int IDAT { get; set; }
        public string NameAT { get; set; }
        public int IDAS { get; set; }
        public string NameAS { get; set; }
        public int IDU { get; set; }
        public string NameU{ get; set; }
        public int IDC { get; set; }
        public string NameC { get; set; }
        public int IDUC { get; set; }
        public string NameUC { get; set; }
        public string FilePath { get; set; }
        public int FileSize { get; set; }
        public bool StatusA { get; set; }
        public Nullable<int> LikeCount { get; set; }
        public string Status { get; set; }
        public string Statusbtn { get; set; }
        public string Likebtn { get; set; }
        public string Cartbtn { get; set; }
        public string UCbtn { get; set; }
        public string Transbtn { get; set; }
    }
}