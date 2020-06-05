using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArtGallery.Models.Views
{
    public class UserView
    {
        public int IDU { get; set; }
        public string FirstnameU { get; set; }
        public string LastnameU { get; set; }
        public string AddressU { get; set; }
        public string PhoneU { get; set; }
        public string EmailU { get; set; }
        public System.DateTime DofbU { get; set; }
        public bool SexU { get; set; }

        public string UsernameU
        {
            get; set;
        }


        public string PasswordU { get; set; }
        public System.DateTime DaycreateU { get; set; }
        public int IDUT { get; set; }
        public bool StatusU { get; set; }
        public string Statusbtn { get; set; }
        public string Status { get; set; }
        public string Usertype { get; set; }

    }
}