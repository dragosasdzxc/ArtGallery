using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.Models.Views
{
    public class TransView
    {
        public List<TransdetailView> tdv { get; set; }
        public TranshistoryView thv { get; set; }
    }
}