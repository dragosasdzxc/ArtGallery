using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.Models.Views
{
    public class IndexView
    {
        public List<ArtView> top6newest { get; set; }
        public List<ArtView> top6popular { get; set; }
        public List<ArtView> top6insale { get; set; }
        public List<ArtView> top6inauction { get; set; }
        public List<CategoryView> cate { get; set; }

    }
}