using ArtGallery.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.Models.Views
{
    public class AddArtView
    {
        public HttpPostedFileBase ImageFile { get; set; }
        public ArtView art { get; set; }
        public List<Artstatu> ast {get;set;}
        public List<Arttype> at { get; set; }
        public List<Category> cate { get; set; }
        public IEnumerable<ArtstatusView> astedit { get; set; }
        public IEnumerable<ArttypeView> atedit { get; set; }
        public IEnumerable<CategoryView> cateedit { get; set; }
    }
}