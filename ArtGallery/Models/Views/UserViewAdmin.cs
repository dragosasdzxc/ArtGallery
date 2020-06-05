using ArtGallery.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.Models.Views
{
    public class UserViewAdmin
    {
        public UserView user { get; set; }
        public List<Usertype> usertype { get; set; }
        public IEnumerable<UsertypeView> usertypeedit { get; set; }
    }
}