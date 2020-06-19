using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.Models.Views
{
    public class ArtCategory
    {
        public List<ArtView> av { get; set; }
        public UsercategoryView ucv { get; set; }
    }
}