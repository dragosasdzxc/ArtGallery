using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.Models.Views
{
    public class MessageListView
    {
        public int IDM { get; set; }
        public int IDUS { get; set; }
        public int IDUR { get; set; }
        public string ContentM { get; set; }
        public bool StatusM { get; set; }
        public int IDMT { get; set; }
        public string NameUS { get; set; }
        public string NameUR { get; set; }
        public string NameMT { get; set; }
        public string Status { get; set; }
        public Nullable<int> IDA { get; set; }
        public string NameA { get; set; }
    }
}