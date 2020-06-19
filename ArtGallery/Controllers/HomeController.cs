using ArtGallery.Models;
using ArtGallery.Models.Entities;
using ArtGallery.Models.Views;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ArtGallery.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            IndexView iv = new IndexView();
            iv.top6newest = Top6Newest();
            iv.top6popular = Top6Popular();
            iv.top6insale = Top6Insale();
            iv.top6inauction = Top6Inauction();
            iv.cate = CateList();
            return View(iv);
        }

        public List<CategoryView> CateList()
        {
            List<CategoryView> cv = null;
            using (var ctx = new ArtGalleryEntities())
            {
                cv = ctx.Categories.Where(c => c.StatusC == true).Select(c => new CategoryView()
                {
                    IDC = c.IDC,
                    NameC = c.NameC
                }).ToList<CategoryView>();
            }
            return cv;
        }

        public List<ArtView> Top6Newest()
        {
            List<ArtView> av = null;

            if (Session["IDU"] != null)
            {

                using (var ctx = new ArtGalleryEntities())
                {
                    var idutmp = 0;
                    if (Session["IDU"] != null)
                    {
                        idutmp = int.Parse(Session["IDU"].ToString());
                    }

                    av = ctx.Arts.Where(a => a.StatusA == true).OrderByDescending(a => a.IDA).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        //Likebtn = ctx.LikeLists.Where(l => l.IDU == int.Parse(Session["IDU"].ToString()) && l.IDA == a.IDA).Any()==true?"Unlike":"Like",
                        Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == idutmp && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = ctx.Carts.Where(c => c.IDU == idutmp && c.IDA == a.IDA).Any() == true ? "Already in cart" : "Add to cart"
                    }).Take(6).ToList<ArtView>();
                }

                return av;
            }
            else
            {
                using (var ctx = new ArtGalleryEntities())
                {
                    av = ctx.Arts.Where(a => a.StatusA == true).OrderByDescending(a => a.IDA).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        Likebtn = "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = "Add to cart"
                    }).Take(6).ToList<ArtView>();
                }

                return av;
            }

        }

        public List<ArtView> Top6Popular()
        {
            List<ArtView> av = null;

            if (Session["IDU"] != null)
            {

                using (var ctx = new ArtGalleryEntities())
                {
                    var idutmp = 0;
                    if (Session["IDU"] != null)
                    {
                        idutmp = int.Parse(Session["IDU"].ToString());
                    }

                    av = ctx.Arts.Where(a => a.StatusA == true).OrderByDescending(a => a.LikeCount).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        //Likebtn = ctx.LikeLists.Where(l => l.IDU == int.Parse(Session["IDU"].ToString()) && l.IDA == a.IDA).Any()==true?"Unlike":"Like",
                        Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == idutmp && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = ctx.Carts.Where(c => c.IDU == idutmp && c.IDA == a.IDA).Any() == true ? "Already in cart" : "Add to cart"
                    }).Take(6).ToList<ArtView>();
                }

                return av;
            }
            else
            {
                using (var ctx = new ArtGalleryEntities())
                {
                    av = ctx.Arts.Where(a => a.StatusA == true).OrderByDescending(a => a.LikeCount).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        Likebtn = "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = "Add to cart"
                    }).Take(6).ToList<ArtView>();
                }

                return av;
            }

        }

        public List<ArtView> Top6Insale()
        {
            List<ArtView> av = null;

            if (Session["IDU"] != null)
            {

                using (var ctx = new ArtGalleryEntities())
                {
                    var idutmp = 0;
                    if (Session["IDU"] != null)
                    {
                        idutmp = int.Parse(Session["IDU"].ToString());
                    }

                    av = ctx.Arts.Where(a => a.StatusA == true && a.IDAS == ctx.Artstatus.Where(at => at.NameAS == "Sell").Select(at => at.IDAS).FirstOrDefault()).OrderByDescending(a => a.IDA).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        //Likebtn = ctx.LikeLists.Where(l => l.IDU == int.Parse(Session["IDU"].ToString()) && l.IDA == a.IDA).Any()==true?"Unlike":"Like",
                        Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == idutmp && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = ctx.Carts.Where(c => c.IDU == idutmp && c.IDA == a.IDA).Any() == true ? "Already in cart" : "Add to cart"
                    }).Take(6).ToList<ArtView>();
                }

                return av;
            }
            else
            {
                using (var ctx = new ArtGalleryEntities())
                {
                    av = ctx.Arts.Where(a => a.StatusA == true && a.IDAS == ctx.Artstatus.Where(at => at.NameAS == "Sell").Select(at => at.IDAS).FirstOrDefault()).OrderByDescending(a => a.IDA).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        Likebtn = "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = "Add to cart"
                    }).Take(6).ToList<ArtView>();
                }

                return av;
            }

        }

        public List<ArtView> Top6Inauction()
        {
            List<ArtView> av = null;

            if (Session["IDU"] != null)
            {

                using (var ctx = new ArtGalleryEntities())
                {
                    var idutmp = 0;
                    if (Session["IDU"] != null)
                    {
                        idutmp = int.Parse(Session["IDU"].ToString());
                    }

                    av = ctx.Arts.Where(a => a.StatusA == true && a.IDAS == ctx.Artstatus.Where(at => at.NameAS == "Auction").Select(at => at.IDAS).FirstOrDefault()).OrderByDescending(a => a.IDA).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        //Likebtn = ctx.LikeLists.Where(l => l.IDU == int.Parse(Session["IDU"].ToString()) && l.IDA == a.IDA).Any()==true?"Unlike":"Like",
                        Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == idutmp && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = ctx.Carts.Where(c => c.IDU == idutmp && c.IDA == a.IDA).Any() == true ? "Already in cart" : "Add to cart"
                    }).Take(6).ToList<ArtView>();
                }

                return av;
            }
            else
            {
                using (var ctx = new ArtGalleryEntities())
                {
                    av = ctx.Arts.Where(a => a.StatusA == true && a.IDAS == ctx.Artstatus.Where(at => at.NameAS == "Auction").Select(at => at.IDAS).FirstOrDefault()).OrderByDescending(a => a.IDA).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        Likebtn = "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = "Add to cart"
                    }).Take(6).ToList<ArtView>();
                }

                return av;
            }

        }

        public ActionResult About()
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }


            return View();
        }

        public ActionResult Contact()
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }

            return View();
        }

        public ActionResult CreateNewUsers(UserView uv)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] == null)
            {
                return View();
            }
            else
            {
                TempData["Alert"] = "Please logout";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult CreateNewUsersDB(UserView uv)
        {
            if (Session["IDU"] == null)
            {
                DateTime aDate = DateTime.Now;
                //var date = aDate.ToString("yyyy/MM/dd");

                string pwcv = Encrypt(uv.PasswordU);

                using (var ctx = new ArtGalleryEntities())
                {
                    ctx.Users.Add(new User
                    {
                        FirstnameU = uv.FirstnameU,
                        LastnameU = uv.LastnameU,
                        AddressU = uv.AddressU,
                        EmailU = uv.EmailU,
                        PhoneU = uv.PhoneU,
                        DofbU = uv.DofbU,
                        SexU = uv.SexU,
                        UsernameU = uv.UsernameU,
                        PasswordU = pwcv,
                        DaycreateU = aDate,
                        IDUT = ctx.Usertypes.Where(ut => ut.NameUT == "Artlover").Select(ut => ut.IDUT).FirstOrDefault(),
                        StatusU = true
                    });
                    if (ctx.SaveChanges() > 0)
                    {
                        TempData["Alert"] = "Create successful!";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["Alert"] = "Create failed";
                        return RedirectToAction("Index", "Home");
                    }


                }
            }
            else
            {
                TempData["Alert"] = "Please logout";
                return RedirectToAction("Index", "Home");
            }

        }

        public static string Encrypt(string text)
        {
            string key = "A!9HHhi%XjjYY4YP2@Nob009X";
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateEncryptor())
                    {
                        byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
                        byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                        return Convert.ToBase64String(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        public static string Decrypt(string cipher)
        {
            string key = "A!9HHhi%XjjYY4YP2@Nob009X";
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(cipher);
                        byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return UTF8Encoding.UTF8.GetString(bytes);
                    }
                }
            }
        }

        public ActionResult EditUser(int id = 0)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }

            if (Session["IDU"] != null)
            {
                if (int.Parse(Session["IDU"].ToString()) == id)
                {
                    UserView us = null;
                    using (var db = new ArtGalleryEntities())
                    {
                        string prepw = db.Users.Where(u => u.IDU == id).Select(u => u.PasswordU).FirstOrDefault();
                        string pwdc = Decrypt(prepw);
                        us = db.Users.Where(u => u.IDU == id).Select(u => new UserView()
                        {
                            IDU = u.IDU,
                            FirstnameU = u.FirstnameU,
                            LastnameU = u.LastnameU,
                            PhoneU = u.PhoneU,
                            AddressU = u.AddressU,
                            EmailU = u.EmailU,
                            DofbU = u.DofbU,
                            SexU = u.SexU,
                            UsernameU = u.UsernameU,
                            PasswordU = pwdc,
                            StatusU = u.StatusU,
                            IDUT = u.IDUT,
                            Status = u.StatusU == true ? "Unlock" : "Lock",
                            Usertype = db.Usertypes.Where(ut => ut.IDUT == u.IDUT).Select(ut => ut.NameUT).FirstOrDefault(),
                        }).FirstOrDefault();
                    }
                    return View(us);
                }
                else
                {
                    TempData["Alert"] = "You can not edit another user profile!";
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["Alert"] = "Please login!";
                return RedirectToAction("Login", "Login");
            }


        }

        public ActionResult EditUserDB(int id, UserView uv)
        {
            if (Session["IDU"] != null)
            {
                using (var ctx = new ArtGalleryEntities())
                {

                    string pwcv = Encrypt(uv.PasswordU);

                    var ex = ctx.Users.Where(u => u.IDU == id).FirstOrDefault<User>();
                    ex.FirstnameU = uv.FirstnameU;
                    ex.LastnameU = uv.LastnameU;
                    ex.AddressU = uv.AddressU;
                    ex.EmailU = uv.EmailU;
                    ex.PhoneU = uv.PhoneU;
                    ex.DofbU = uv.DofbU;
                    ex.SexU = uv.SexU;
                    ex.UsernameU = uv.UsernameU;
                    ex.PasswordU = pwcv;
                    if (ctx.SaveChanges() > 0)
                    {
                        TempData["Alert"] = "Edit successful!";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["Alert"] = "EDIT FAILED! MAYBE YOU DON'T CHANGE ANYTHING IN YOUR PROFILE, IF THIS ERROR HAS STILL EXISTS AFTER YOU CHANGED YOUR PROFILE INFORMATION, PLEASE CONTACT US!";
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            else
            {
                TempData["Alert"] = "Please login!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult AddNewArt()
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {
                ArtGalleryEntities db = new ArtGalleryEntities();
                int idu = int.Parse(Session["IDU"].ToString());
                string ut = Session["Usertype"].ToString();
                if (ut == "ArtistBasic")
                {
                    int count = db.Arts.Where(u => u.IDU == idu).Count();
                    if (count < 200)
                    {
                        int id = int.Parse(Session["IDU"].ToString());
                        ArtGalleryEntities ctx = new ArtGalleryEntities();
                        var ast = ctx.Artstatus.ToList();
                        var at = ctx.Arttypes.Where(a => a.StatusA == true).ToList();
                        var cate = ctx.Categories.Where(c => c.StatusC == true).ToList();
                        var aav = new AddArtView { ast = ast, at = at, cate = cate };
                        return View(aav);
                    }
                    else
                    {
                        TempData["Alert"] = "You have reach limit, can not add more art!";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else if (ut == "ArtistStandard")
                {
                    int count = db.Arts.Where(u => u.IDU == idu).Count();
                    if (count < 1000)
                    {
                        int id = int.Parse(Session["IDU"].ToString());
                        ArtGalleryEntities ctx = new ArtGalleryEntities();
                        var ast = ctx.Artstatus.ToList();
                        var at = ctx.Arttypes.Where(a => a.StatusA == true).ToList();
                        var cate = ctx.Categories.Where(c => c.StatusC == true).ToList();
                        var aav = new AddArtView { ast = ast, at = at, cate = cate };
                        return View(aav);
                    }
                    else
                    {
                        TempData["Alert"] = "You have reach limit, can not add more art!";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["Alert"] = "Please upgrade to artist to add your art!";
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["Alert"] = "Please login!";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddNewArtDB(HttpPostedFileBase ImageFile, AddArtView aav)
        {
            int ids = int.Parse(Session["IDU"].ToString());
            if (Session["IDU"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (ImageFile != null)
                    {
                        try
                        {
                            DateTime aDate = DateTime.Now;
                            var date = aDate.ToString("MM/dd/yyyy");
                            var imagename = Path.GetFileName(ImageFile.FileName);
                            var ServerSaveImagePath = Server.MapPath("~/ImageUpload/") + imagename;
                            int filesize = ImageFile.ContentLength;
                            int size = filesize / 1000000;
                            //Save file to server folder  
                            ImageFile.SaveAs(ServerSaveImagePath);
                            using (var db = new ArtGalleryEntities())
                            {
                                db.Arts.Add(new Art
                                {
                                    NameA = aav.art.NameA,
                                    WidthA = aav.art.WidthA,
                                    HeightA = aav.art.HeightA,
                                    DepthA = aav.art.DepthA,
                                    MaterialA = aav.art.MaterialA,
                                    DaycreateA = aDate,
                                    DescriptionA = aav.art.DescriptionA,
                                    PriceA = aav.art.PriceA,
                                    IDAT = aav.art.IDAT,
                                    IDAS = aav.art.IDAS,
                                    IDC = aav.art.IDC,
                                    IDU = int.Parse(Session["IDU"].ToString()),
                                    IDUC = 1,
                                    FileSize = size,
                                    FilePath = "~/ImageUpload/" + imagename,
                                    StatusA = true,
                                    LikeCount = 0
                                });
                                db.SaveChanges();
                            }


                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                            throw;
                        }

                    }

                }
                TempData["Alert"] = "Success!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Alert"] = "Error!";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult GetArtList()
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {

                List<ArtView> av = null;
                using (var ctx = new ArtGalleryEntities())
                {
                    var idutmp = 0;
                    if (Session["IDU"] != null)
                    {
                        idutmp = int.Parse(Session["IDU"].ToString());
                    }

                    av = ctx.Arts.Where(a => a.StatusA == true).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        //Likebtn = ctx.LikeLists.Where(l => l.IDU == int.Parse(Session["IDU"].ToString()) && l.IDA == a.IDA).Any()==true?"Unlike":"Like",
                        Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == idutmp && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = ctx.Carts.Where(c => c.IDU == idutmp && c.IDA == a.IDA).Any() == true ? "Already in cart" : "Add to cart"
                    }).ToList<ArtView>();
                }

                return View(av);
            }
            else
            {
                List<ArtView> av = null;
                using (var ctx = new ArtGalleryEntities())
                {
                    av = ctx.Arts.Where(a => a.StatusA == true).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        Likebtn = "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = "Add to cart"
                    }).ToList<ArtView>();
                }

                return View(av);
            }
        }

        public ActionResult GetArtListByID(int id = 0)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {
                if (int.Parse(Session["IDU"].ToString()) == id)
                {
                    List<ArtView> av = null;
                    using (var ctx = new ArtGalleryEntities())
                    {
                        av = ctx.Arts.Where(a => a.IDU == id).Select(a => new ArtView()
                        {
                            IDA = a.IDA,
                            NameA = a.NameA,
                            DaycreateA = a.DaycreateA,
                            DepthA = a.DepthA,
                            DescriptionA = a.DescriptionA,
                            FilePath = a.FilePath,
                            FileSize = a.FileSize,
                            HeightA = a.HeightA,
                            IDAS = a.IDAS,
                            IDAT = a.IDAT,
                            IDC = a.IDC,
                            IDU = a.IDU,
                            IDUC = a.IDUC,
                            //Likebtn = ctx.LikeLists.Where(l => l.IDU == int.Parse(Session["IDU"].ToString()) && l.IDA == a.IDA).Any()==true?"Unlike":"Like",
                            Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == id && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                            LikeCount = a.LikeCount,
                            MaterialA = a.MaterialA,
                            NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                            NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                            NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                            NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                            NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                            PriceA = a.PriceA,
                            StatusA = a.StatusA,
                            Status = a.StatusA == true ? "Unlock" : "Lock",
                            Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                            WidthA = a.WidthA
                        }).ToList<ArtView>();
                    }

                    return View(av);
                }
                else
                {
                    TempData["Alert"] = "You can not manage another user arts!";
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["Alert"] = "Please login!";
                return RedirectToAction("Login", "Login");
            }
        }

        /*public void LockArtByID(int ida, int idu)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Arts.Where(u => u.IDA == ida && u.IDU == idu).FirstOrDefault<Art>();
                ex.StatusA = false;
                ctx.SaveChanges();
            }
        }

        public void UnlockArtByID(int ida, int idu)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Arts.Where(u => u.IDA == ida && u.IDU == idu).FirstOrDefault<Art>();
                ex.StatusA = true;
                ctx.SaveChanges();
            }
        }*/

        public ActionResult EditArtByID(int ida = 0, int idu = 0)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {
                if (int.Parse(Session["IDU"].ToString()) == idu)
                {
                    AddArtView avv = new AddArtView();
                    avv.art = GetEditArtByID(ida, idu);
                    avv.astedit = GetArtstatus();
                    avv.atedit = GetArttypes();
                    avv.cateedit = GetCategory();
                    return View(avv);
                }
                else
                {
                    TempData["Alert"] = "You can not edit another user art!";
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["Alert"] = "Please login!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ArtView GetEditArtByID(int ida, int idu)
        {


            ArtView av = null;
            using (var ctx = new ArtGalleryEntities())
            {
                av = ctx.Arts.Where(a => a.IDU == idu && a.IDA == ida).Select(a => new ArtView()
                {
                    IDA = a.IDA,
                    NameA = a.NameA,
                    DaycreateA = a.DaycreateA,
                    DepthA = a.DepthA,
                    DescriptionA = a.DescriptionA,
                    FilePath = a.FilePath,
                    FileSize = a.FileSize,
                    HeightA = a.HeightA,
                    IDAS = a.IDAS,
                    IDAT = a.IDAT,
                    IDC = a.IDC,
                    IDU = a.IDU,
                    IDUC = a.IDUC,
                    //Likebtn = ctx.LikeLists.Where(l => l.IDU == int.Parse(Session["IDU"].ToString()) && l.IDA == a.IDA).Any()==true?"Unlike":"Like",
                    Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == idu && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                    LikeCount = a.LikeCount,
                    MaterialA = a.MaterialA,
                    NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                    NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                    NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                    NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                    NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                    PriceA = a.PriceA,
                    StatusA = a.StatusA,
                    Status = a.StatusA == true ? "Unlock" : "Lock",
                    Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                    WidthA = a.WidthA
                }).FirstOrDefault();
            }

            return av;

        }

        public IEnumerable<ArtstatusView> GetArtstatus()
        {
            IEnumerable<ArtstatusView> usertype = null;
            using (var ctx = new ArtGalleryEntities())
            {
                usertype = ctx.Artstatus.Select(u => new ArtstatusView()
                {
                    IDAS = u.IDAS,
                    NameAS = u.NameAS,
                }).ToList<ArtstatusView>();
            }
            return usertype;
        }

        public IEnumerable<ArttypeView> GetArttypes()
        {
            IEnumerable<ArttypeView> usertype = null;
            using (var ctx = new ArtGalleryEntities())
            {
                usertype = ctx.Arttypes.Where(a => a.StatusA == true).Select(u => new ArttypeView()
                {
                    IDAT = u.IDAT,
                    NameAT = u.NameAT,
                }).ToList<ArttypeView>();
            }
            return usertype;
        }

        public IEnumerable<CategoryView> GetCategory()
        {
            IEnumerable<CategoryView> usertype = null;
            using (var ctx = new ArtGalleryEntities())
            {
                usertype = ctx.Categories.Where(a => a.StatusC == true).Select(u => new CategoryView()
                {
                    IDC = u.IDC,
                    NameC = u.NameC,
                }).ToList<CategoryView>();
            }
            return usertype;
        }

        [HttpPost]
        public ActionResult EditArtByIDDB(HttpPostedFileBase ImageFile, AddArtView aav, int ida, int idu)
        {
            if (Session["IDU"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (ImageFile != null)
                    {
                        try
                        {
                            DateTime aDate = DateTime.Now;
                            var date = aDate.ToString("MM/dd/yyyy");
                            var imagename = Path.GetFileName(ImageFile.FileName);
                            var ServerSaveImagePath = Server.MapPath("~/ImageUpload/") + imagename;
                            int filesize = ImageFile.ContentLength;
                            int size = filesize / 1000000;
                            //Save file to server folder  
                            ImageFile.SaveAs(ServerSaveImagePath);
                            using (var db = new ArtGalleryEntities())
                            {
                                var ex = db.Arts.Where(a => a.IDA == ida && a.IDU == idu).FirstOrDefault<Art>();
                                ex.NameA = aav.art.NameA;
                                ex.WidthA = aav.art.WidthA;
                                ex.HeightA = aav.art.HeightA;
                                ex.DepthA = aav.art.DepthA;
                                ex.MaterialA = aav.art.MaterialA;
                                ex.DaycreateA = aDate;
                                ex.DescriptionA = aav.art.DescriptionA;
                                ex.PriceA = aav.art.PriceA;
                                ex.IDAT = aav.art.IDAT;
                                ex.IDAS = aav.art.IDAS;
                                ex.IDC = aav.art.IDC;
                                ex.IDU = int.Parse(Session["IDU"].ToString());
                                ex.IDUC = 1;
                                ex.FileSize = size;
                                ex.FilePath = "~/ImageUpload/" + imagename;
                                ex.StatusA = true;
                                db.SaveChanges();
                            }


                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                            throw;
                        }

                    }
                    else
                    {
                        try
                        {
                            using (var db = new ArtGalleryEntities())
                            {
                                var ex = db.Arts.Where(a => a.IDA == ida && a.IDU == idu).FirstOrDefault<Art>();
                                ex.NameA = aav.art.NameA;
                                ex.WidthA = aav.art.WidthA;
                                ex.HeightA = aav.art.HeightA;
                                ex.DepthA = aav.art.DepthA;
                                ex.MaterialA = aav.art.MaterialA;
                                ex.DescriptionA = aav.art.DescriptionA;
                                ex.PriceA = aav.art.PriceA;
                                ex.IDAT = aav.art.IDAT;
                                ex.IDAS = aav.art.IDAS;
                                ex.IDC = aav.art.IDC;
                                ex.IDU = int.Parse(Session["IDU"].ToString());
                                ex.IDUC = 1;
                                ex.FilePath = aav.art.FilePath;
                                ex.StatusA = true;
                                db.SaveChanges();
                            }


                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                            throw;
                        }
                    }
                }
                TempData["Alert"] = "Edit success!";
                return RedirectToAction("Index", "Home");

            }
            else
            {
                TempData["Alert"] = "Please login!";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult LikeArt(int ida)
        {
            if (Session["IDU"] != null)
            {
                int idu = int.Parse(Session["IDU"].ToString());
                using (var ctx = new ArtGalleryEntities())
                {
                    ctx.LikeLists.Add(new LikeList()
                    {
                        IDA = ida,
                        IDU = idu
                    });
                    var ex = ctx.Arts.Where(a => a.IDA == ida).FirstOrDefault();
                    ex.LikeCount += 1;
                    ctx.SaveChanges();
                }
                return Json(new { success = true, message = "Thank for like my art!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Error has been occurred!" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult UnlikeArt(int ida)
        {
            if (Session["IDU"] != null)
            {
                int idu = int.Parse(Session["IDU"].ToString());
                using (var ctx = new ArtGalleryEntities())
                {
                    var ex = ctx.LikeLists.Where(u => u.IDU == idu && u.IDA == ida).FirstOrDefault<LikeList>();
                    ctx.Entry(ex).State = System.Data.Entity.EntityState.Deleted;
                    var e = ctx.Arts.Where(a => a.IDA == ida).FirstOrDefault();
                    e.LikeCount -= 1;
                    ctx.SaveChanges();
                }
                return Json(new { success = true, message = "Thank for like my art!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Error has been occurred!" }, JsonRequestBehavior.AllowGet);
            }

        }

        public void DeleteArt(int ida = 0, int idu = 0)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var exm = ctx.MessageLists.Where(m => m.IDA == ida);
                ctx.MessageLists.RemoveRange(exm);
                var exc = ctx.Carts.Where(c => c.IDA == ida);
                ctx.Carts.RemoveRange(exc);
                var exl = ctx.LikeLists.Where(l => l.IDA == ida);
                ctx.LikeLists.RemoveRange(exl);
                var exa = ctx.Auctionhistories.Where(a => a.IDA == ida);
                ctx.Auctionhistories.RemoveRange(exa);
                var extd = ctx.Transdetails.Where(t => t.IDA == ida);
                int uidth = extd.Select(t => t.UIDTH).FirstOrDefault();
                ctx.Transdetails.RemoveRange(extd);
                var exth = ctx.Transhistories.Where(t => t.UIDTH == uidth);
                ctx.Transhistories.RemoveRange(exth);
                var ex = ctx.Arts.Where(u => u.IDU == idu && u.IDA == ida).FirstOrDefault<Art>();
                string path = ex.FilePath;
                string fullPath = Request.MapPath(path);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                ctx.Arts.Remove(ex);
                ctx.SaveChanges();
            }
        }

        public ActionResult AddToCart(int ida = 0)
        {
            if (Session["IDU"] != null)
            {
                using (var ctx = new ArtGalleryEntities())
                {
                    var ex = ctx.Arts.Where(a => a.IDA == ida).FirstOrDefault();
                    string checkartst = ctx.Artstatus.Where(at => at.IDAS == ex.IDAS).Select(at => at.NameAS).FirstOrDefault();
                    if (ex.IDU != int.Parse(Session["IDU"].ToString()))
                    {
                        if (checkartst == "Cancel auction")
                        {
                            return Json(new { success = false, message = "This art has been cancel auction!" }, JsonRequestBehavior.AllowGet);
                        }
                        else if (checkartst == "Auction")
                        {
                            return Json(new { success = false, message = "This art is in auction!" }, JsonRequestBehavior.AllowGet);
                        }
                        else if (checkartst == "Sold")
                        {
                            return Json(new { success = false, message = "This art has been sold!" }, JsonRequestBehavior.AllowGet);
                        }
                        else if (checkartst == "Sell")
                        {
                            ctx.Carts.Add(new Cart()
                            {
                                IDA = ida,
                                IDU = int.Parse(Session["IDU"].ToString()),
                                StatusCA = true
                            });
                            ctx.SaveChanges();
                            return Json(new { success = true, message = "Thank for choose my art!" }, JsonRequestBehavior.AllowGet);
                        }
                        else if (checkartst == "View only")
                        {
                            return Json(new { success = false, message = "This art is view only!" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { success = false, message = "Error has been occurred!" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "You can not add your art to your cart!" }, JsonRequestBehavior.AllowGet);
                    }

                }

            }
            else
            {
                return Json(new { success = false, message = "Please login!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetUserCategoryByID(int id = 0)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {
                if (int.Parse(Session["IDU"].ToString()) == id)
                {
                    List<UsercategoryView> ucv = null;
                    using (var ctx = new ArtGalleryEntities())
                    {

                        ucv = ctx.Usercategories.Where(c => c.IDU == id).Select(c => new UsercategoryView()
                        {
                            IDUC = c.IDUC,
                            NameUC = c.NameUC,
                            DaycreateUC = c.DaycreateUC,
                            PriceUC = c.PriceUC,
                            StatusUC = c.StatusUC,
                            Status = c.StatusUC == true ? "Unlock" : "Lock",
                            Statusbtn = c.StatusUC == true ? "Lock" : "Unlock",
                            NameU = ctx.Users.Where(u => u.IDU == c.IDU).Select(u => u.FirstnameU + u.LastnameU).FirstOrDefault(),
                            IDU = c.IDU
                        }).ToList<UsercategoryView>();
                    }
                    return View(ucv);
                }
                else
                {
                    TempData["Alert"] = "You can not manage another user category!!!";
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["Alert"] = "Please login";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult CreateNewUserCategory()
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {
                ArtGalleryEntities db = new ArtGalleryEntities();
                int idu = int.Parse(Session["IDU"].ToString());
                string ut = Session["Usertype"].ToString();
                if (ut == "ArtistBasic")
                {
                    int count = db.Usercategories.Where(u => u.IDU == idu).Count();
                    if (count < 10)
                    {
                        return View();
                    }
                    else {
                        TempData["Alert"] = "You have reach limit, can not add more your own category!";
                        return RedirectToAction("Index","Home");
                    }
                }
                else if (ut == "ArtistStandard")
                {
                    int count = db.Usercategories.Where(u => u.IDU == idu).Count();
                    if (count < 100)
                    {
                        return View();
                    }
                    else
                    {
                        TempData["Alert"] = "You have reach limit, can not add more your own category!";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else {
                    TempData["Alert"] = "Please upgrade to artist to add your own category!";
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["Alert"] = "Please login";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult CreateNewUserCategoryDB(UsercategoryView ucv)
        {
            if (Session["IDU"] != null)
            {
                using (var ctx = new ArtGalleryEntities())
                {
                    DateTime aDate = DateTime.Now;
                    ctx.Usercategories.Add(new Usercategory
                    {
                        NameUC = ucv.NameUC,
                        StatusUC = true,
                        PriceUC = 0,
                        DaycreateUC = aDate,
                        IDU = int.Parse(Session["IDU"].ToString())
                    });
                    ctx.SaveChanges();
                }
                TempData["Alert"] = "Create success!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Alert"] = "Please login";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult GetArtInUCByID(int iduc = 0, int idu = 0)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {
                if (int.Parse(Session["IDU"].ToString()) == idu)
                {
                    List<ArtView> ucv = null;
                    using (var ctx = new ArtGalleryEntities())
                    {
                        ucv = ctx.Arts.Where(a => a.IDUC == iduc).Select(a => new ArtView()
                        {
                            IDA = a.IDA,
                            NameA = a.NameA,
                            DaycreateA = a.DaycreateA,
                            DepthA = a.DepthA,
                            DescriptionA = a.DescriptionA,
                            FilePath = a.FilePath,
                            FileSize = a.FileSize,
                            HeightA = a.HeightA,
                            IDAS = a.IDAS,
                            IDAT = a.IDAT,
                            IDC = a.IDC,
                            IDU = a.IDU,
                            IDUC = a.IDUC,
                            Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == idu && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                            LikeCount = a.LikeCount,
                            MaterialA = a.MaterialA,
                            NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                            NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                            NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                            NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                            NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                            PriceA = a.PriceA,
                            StatusA = a.StatusA,
                            Status = a.StatusA == true ? "Unlock" : "Lock",
                            Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                            WidthA = a.WidthA
                        }).ToList<ArtView>();
                        Session["UCval"] = iduc;
                        Session["UCname"] = ctx.Usercategories.Where(uc => uc.IDUC == iduc).Select(uc => uc.NameUC).FirstOrDefault();
                    }

                    return View(ucv);
                }
                else
                {
                    TempData["Alert"] = "You can not manage another user category!!!";
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["Alert"] = "Please login";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ChooseArtByID(int idu = 0)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {
                if (int.Parse(Session["IDU"].ToString()) == idu)
                {
                    List<ArtView> ucv = null;
                    int iduc = int.Parse(Session["UCval"].ToString());
                    using (var ctx = new ArtGalleryEntities())
                    {
                        ucv = ctx.Arts.Where(a => a.IDU == idu && a.IDUC != iduc && a.StatusA == true).Select(a => new ArtView()
                        {
                            IDA = a.IDA,
                            NameA = a.NameA,
                            DaycreateA = a.DaycreateA,
                            DepthA = a.DepthA,
                            DescriptionA = a.DescriptionA,
                            FilePath = a.FilePath,
                            FileSize = a.FileSize,
                            HeightA = a.HeightA,
                            IDAS = a.IDAS,
                            IDAT = a.IDAT,
                            IDC = a.IDC,
                            IDU = a.IDU,
                            IDUC = a.IDUC,
                            Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == idu && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                            LikeCount = a.LikeCount,
                            MaterialA = a.MaterialA,
                            NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                            NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                            NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                            NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                            NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                            PriceA = a.PriceA,
                            StatusA = a.StatusA,
                            Status = a.StatusA == true ? "Unlock" : "Lock",
                            Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                            WidthA = a.WidthA,
                            UCbtn = (ctx.Arts.FirstOrDefault(ar => ar.IDA == a.IDA && ar.IDUC == iduc)) != null ? "Remove" : "Add",
                        }).ToList<ArtView>();

                    }
                    return View(ucv);
                }
                else
                {
                    TempData["Alert"] = "You can not manage another user category!!!";
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["Alert"] = "Please login";
                return RedirectToAction("Index", "Home");
            }
        }

        public void RemoveArtFromUserCategoryByID(int ida)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Arts.Where(a => a.IDA == ida).FirstOrDefault();
                ex.IDUC = 1;
                ctx.SaveChanges();
            }
        }

        public void AddArtToUserCategoryDB(int ida)
        {
            int iduc = int.Parse(Session["UCval"].ToString());
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Arts.Where(a => a.IDA == ida).FirstOrDefault();
                ex.IDUC = iduc;
                ctx.SaveChanges();

            }
        }

        public void LockUserCategoryByID(int iduc, int idu)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Usercategories.Where(u => u.IDUC == iduc && u.IDU == idu).FirstOrDefault<Usercategory>();
                ex.StatusUC = false;
                ctx.SaveChanges();
            }
        }

        public void UnlockUserCategoryByID(int iduc, int idu)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Usercategories.Where(u => u.IDUC == iduc && u.IDU == idu).FirstOrDefault<Usercategory>();
                ex.StatusUC = true;
                ctx.SaveChanges();
            }
        }

        public void DeleteUserCategoryDB(int iduc, int idu)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Arts.Where(a => a.IDUC == iduc).ToList();
                ex.ForEach(a => a.IDUC = 1);
                var e = ctx.Usercategories.Where(u => u.IDUC == iduc).FirstOrDefault();
                ctx.Usercategories.Remove(e);
                ctx.SaveChanges();
            }
        }

        public ActionResult GetCartList(int idu = 0)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {
                if (int.Parse(Session["IDU"].ToString()) == idu)
                {
                    /*List<CartView> cv = null;
                    using (var ctx = new ArtGalleryEntities())
                    {
                        cv = ctx.Carts.Where(c => c.IDU == idu).Select(c => new CartView()
                        {
                            IDCA = c.IDCA,
                            IDA = c.IDA,
                            IDU = c.IDU,
                            NameA = ctx.Arts.Where(a => a.IDA == c.IDA).Select(a => a.NameA).FirstOrDefault(),
                            NameU = ctx.Users.Where(u => u.IDU == c.IDU).Select(u => u.FirstnameU + u.LastnameU).FirstOrDefault(),
                            PriceA = ctx.Arts.Where(a => a.IDA == c.IDA).Select(a => a.PriceA).FirstOrDefault(),
                        }).ToList<CartView>();

                    }
                    return View(cv);*/

                    ArtGalleryEntities ctx = new ArtGalleryEntities();
                    List<CartView> cv = ctx.Carts.Where(c => c.IDU == idu).Select(c => new CartView()
                    {
                        IDCA = c.IDCA,
                        IDA = c.IDA,
                        IDU = c.IDU,
                        NameA = ctx.Arts.Where(a => a.IDA == c.IDA).Select(a => a.NameA).FirstOrDefault(),
                        NameU = ctx.Users.Where(u => u.IDU == c.IDU).Select(u => u.FirstnameU + u.LastnameU).FirstOrDefault(),
                        PriceA = ctx.Arts.Where(a => a.IDA == c.IDA).Select(a => a.PriceA).FirstOrDefault(),
                    }).ToList<CartView>();
                    Session.Add("CartList", cv);
                    return View(cv);

                }
                else
                {
                    TempData["Alert"] = "You can not manage another user category!!!";
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["Alert"] = "Please login";
                return RedirectToAction("Index", "Home");
            }
        }

        public void RemoveArtFromCart(int ida, int idu)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Carts.Where(c => c.IDU == idu && c.IDA == ida).FirstOrDefault();
                ctx.Carts.Remove(ex);
                ctx.SaveChanges();
            }
        }

        public ActionResult GetMessageList(int idu = 0)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {
                if (int.Parse(Session["IDU"].ToString()) == idu)
                {
                    List<MessageListView> ml = null;
                    using (var ctx = new ArtGalleryEntities())
                    {
                        ml = ctx.MessageLists.Where(m => m.IDUR == idu).Select(m => new MessageListView()
                        {
                            IDM = m.IDM,
                            IDUS = m.IDUS,
                            IDUR = m.IDUR,
                            IDMT = m.IDMT,
                            ContentM = m.ContentM,
                            StatusM = m.StatusM,
                            NameMT = ctx.Messagetypes.Where(mt => mt.IDMT == m.IDMT).Select(mt => mt.NameMT).FirstOrDefault(),
                            NameUR = ctx.Users.Where(u => u.IDU == m.IDUR).Select(u => u.FirstnameU + u.LastnameU).FirstOrDefault(),
                            NameUS = ctx.Users.Where(u => u.IDU == m.IDUS).Select(u => u.FirstnameU + u.LastnameU).FirstOrDefault(),
                            Status = m.StatusM == true ? "Seen" : "Unseen",
                            IDA = m.IDA,
                            NameA = ctx.Arts.Where(a => a.IDA == m.IDA).Select(a => a.NameA).FirstOrDefault(),
                        }).ToList<MessageListView>();

                    }
                    return View(ml);
                }
                else
                {
                    TempData["Alert"] = "You can not manage another user category!!!";
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["Alert"] = "Please login";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public JsonResult GetMessageContentByID(int idm)
        {
            MessageListView mv = new MessageListView();
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.MessageLists.Where(m => m.IDM == idm).FirstOrDefault();
                ex.StatusM = true;
                ctx.SaveChanges();
                mv = ctx.MessageLists.Where(m => m.IDM == idm).Select(m => new MessageListView()
                {
                    IDM = m.IDM,
                    ContentM = m.ContentM,
                    IDA = m.IDA,
                    IDMT = m.IDMT,
                    IDUR = m.IDUR,
                    IDUS = m.IDUS,
                    StatusM = m.StatusM,
                    Status = m.StatusM == true ? "Seen" : "Unseen",
                    NameMT = ctx.Messagetypes.Where(mt => mt.IDMT == m.IDMT).Select(mt => mt.NameMT).FirstOrDefault(),
                    NameUR = ctx.Users.Where(u => u.IDU == m.IDUR).Select(u => u.FirstnameU + u.LastnameU).FirstOrDefault(),
                    NameUS = ctx.Users.Where(u => u.IDU == m.IDUS).Select(u => u.FirstnameU + u.LastnameU).FirstOrDefault(),
                }).FirstOrDefault();
            }
            return Json(mv, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void SendMessage(int idus, int idur, string contentm)
        {
            idus += int.Parse(Session["IDU"].ToString());
            using (var ctx = new ArtGalleryEntities())
            {
                ctx.MessageLists.Add(new MessageList
                {
                    IDUS = idus,
                    IDUR = idur,
                    ContentM = contentm,
                    IDMT = ctx.Messagetypes.Where(mt => mt.NameMT == "Message").Select(mt => mt.IDMT).FirstOrDefault(),
                    StatusM = false
                });
                ctx.SaveChanges();
            }
        }

        public void DeleteMessage(int idm)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.MessageLists.Where(m => m.IDM == idm).FirstOrDefault();
                ctx.MessageLists.Remove(ex);
                ctx.SaveChanges();
            }
        }

        public ActionResult ChooseUsertype()
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {
                return View();

            }
            else
            {
                TempData["Alert"] = "Please login";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult UpgradeBasicArtist(string Cancel = null)
        {
            if (Session["Usertype"].ToString() != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    TempData["Alert"] = "You are Admin, you can not change your usertype!";
                    return RedirectToAction("Index", "Home");
                }
                else if (Session["Usertype"].ToString() == "ArtistBasic")
                {
                    TempData["Alert"] = "You are always ArtistBasic, you can only upgrade to ArtistStandard!";
                    return RedirectToAction("Index", "Home");
                }
                else if (Session["Usertype"].ToString() == "ArtistStandard")
                {
                    TempData["Alert"] = "You are always ArtistStandard, you can not upgrade your usertype more!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //getting the apiContext  
                    APIContext apiContext = PaypalConfiguration.GetAPIContext();
                    try
                    {
                        //A resource representing a Payer that funds a payment Payment Method as paypal  
                        //Payer Id will be returned when payment proceeds or click to pay  
                        string payerId = Request.Params["PayerID"];
                        if (string.IsNullOrEmpty(payerId))
                        {
                            //this section will be executed first because PayerID doesn't exist  
                            //it is returned by the create function call of the payment class  
                            // Creating a payment  
                            // baseURL is the url on which paypal sendsback the data.  
                            string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Home/UpgradeBasicArtist?";
                            //here we are generating guid for storing the paymentID received in session  
                            //which will be used in the payment execution  
                            var guid = Convert.ToString((new Random()).Next(100000));
                            //CreatePayment function gives us the payment approval url  
                            //on which payer is redirected for paypal account payment  
                            var createdPayment = this.CreatePaymentBasic(apiContext, baseURI + "guid=" + guid);
                            //get links returned from paypal in response to Create function call  
                            var links = createdPayment.links.GetEnumerator();
                            string paypalRedirectUrl = null;
                            while (links.MoveNext())
                            {
                                Links lnk = links.Current;
                                if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                                {
                                    //saving the payapalredirect URL to which user will be redirected for payment  
                                    paypalRedirectUrl = lnk.href;
                                }
                            }
                            // saving the paymentID in the key guid  
                            Session.Add(guid, createdPayment.id);
                            return Redirect(paypalRedirectUrl);
                        }
                        else
                        {
                            // This function exectues after receving all parameters for the payment  
                            var guid = Request.Params["guid"];
                            var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                            //If executed payment failed then we will show payment failure message to user  
                            if (executedPayment.state.ToLower() != "approved")
                            {
                                TempData["Alert"] = "Payment failed!";
                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Alert"] = "Error has been occurred!";
                        return RedirectToAction("Index", "Home");
                    }
                    //on successful payment, show success page to user.
                    int idu = int.Parse(Session["IDU"].ToString());
                    using (var ctx = new ArtGalleryEntities())
                    {
                        var ex = ctx.Users.Where(u => u.IDU == idu).FirstOrDefault();
                        ex.IDUT = ctx.Usertypes.Where(ut => ut.NameUT == "ArtistBasic").Select(ut => ut.IDUT).FirstOrDefault();
                        ctx.SaveChanges();
                    }
                    Session["Usertype"] = "ArtistBasic";
                    TempData["Alert"] = "Payment success!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login!";
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult UpgradeStandardArtist(string Cancel = null)
        {
            if (Session["Usertype"].ToString() != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    TempData["Alert"] = "You are Admin, you can not change your usertype!";
                    return RedirectToAction("Index", "Home");
                }
                else if (Session["Usertype"].ToString() == "ArtistStandard")
                {
                    TempData["Alert"] = "You are always ArtistStandard, you can not upgrade your usertype more!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //getting the apiContext  
                    APIContext apiContext = PaypalConfiguration.GetAPIContext();
                    try
                    {
                        //A resource representing a Payer that funds a payment Payment Method as paypal  
                        //Payer Id will be returned when payment proceeds or click to pay  
                        string payerId = Request.Params["PayerID"];
                        if (string.IsNullOrEmpty(payerId))
                        {
                            //this section will be executed first because PayerID doesn't exist  
                            //it is returned by the create function call of the payment class  
                            // Creating a payment  
                            // baseURL is the url on which paypal sendsback the data.  
                            string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Home/UpgradeStandardArtist?";
                            //here we are generating guid for storing the paymentID received in session  
                            //which will be used in the payment execution  
                            var guid = Convert.ToString((new Random()).Next(100000));
                            //CreatePayment function gives us the payment approval url  
                            //on which payer is redirected for paypal account payment  
                            var createdPayment = this.CreatePaymentStandard(apiContext, baseURI + "guid=" + guid);
                            //get links returned from paypal in response to Create function call  
                            var links = createdPayment.links.GetEnumerator();
                            string paypalRedirectUrl = null;
                            while (links.MoveNext())
                            {
                                Links lnk = links.Current;
                                if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                                {
                                    //saving the payapalredirect URL to which user will be redirected for payment  
                                    paypalRedirectUrl = lnk.href;
                                }
                            }
                            // saving the paymentID in the key guid  
                            Session.Add(guid, createdPayment.id);
                            return Redirect(paypalRedirectUrl);
                        }
                        else
                        {
                            // This function exectues after receving all parameters for the payment  
                            var guid = Request.Params["guid"];
                            var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                            //If executed payment failed then we will show payment failure message to user  
                            if (executedPayment.state.ToLower() != "approved")
                            {
                                TempData["Alert"] = "Payment failed!";
                                return RedirectToAction("Index", "Home");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Alert"] = "Error has been occurred!";
                        return RedirectToAction("Index", "Home");
                    }
                    //on successful payment, show success page to user. 
                    int idu = int.Parse(Session["IDU"].ToString());
                    using (var ctx = new ArtGalleryEntities())
                    {
                        var ex = ctx.Users.Where(u => u.IDU == idu).FirstOrDefault();
                        ex.IDUT = ctx.Usertypes.Where(ut => ut.NameUT == "ArtistStandard").Select(ut => ut.IDUT).FirstOrDefault();
                        ctx.SaveChanges();
                    }
                    Session["Usertype"] = "ArtistStandard";
                    TempData["Alert"] = "Payment success!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login!";
                return RedirectToAction("Index", "Home");
            }

        }

        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePaymentBasic(APIContext apiContext, string redirectUrl)
        {
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = "Upgrade Basic Artist",
                currency = "USD",
                price = "199",
                quantity = "1",
                sku = "sku"
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "1",
                shipping = "2",
                subtotal = itemList.items.Sum(s => int.Parse(s.price.ToString()) * int.Parse(s.quantity.ToString())).ToString(), //"199"
                //subtotal = "199"
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = (int.Parse(details.tax) + int.Parse(details.shipping) + int.Parse(details.subtotal)).ToString(),// Total must be equal to sum of tax, shipping and subtotal. "202" 
                //total="202",
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Convert.ToString((new Random()).Next(100000)), //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }

        private Payment CreatePaymentStandard(APIContext apiContext, string redirectUrl)
        {
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = "Upgrade Standard Artist",
                currency = "USD",
                price = "589",
                quantity = "1",
                sku = "sku"
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "1",
                shipping = "2",
                subtotal = itemList.items.Sum(s => int.Parse(s.price.ToString()) * int.Parse(s.quantity.ToString())).ToString(), //"589"
                //subtotal="589"
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = (int.Parse(details.tax) + int.Parse(details.shipping) + int.Parse(details.subtotal)).ToString(),// Total must be equal to sum of tax, shipping and subtotal. "592" 
                //total="592",
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Convert.ToString((new Random()).Next(100000)), //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }

        public ActionResult PayCartMultiple(string Cancel = null)
        {
            List<CartView> cv = new List<CartView>();
            cv = Session["CartList"] as List<CartView>;
            if (cv.Count() > 0)
            {
                //getting the apiContext  
                APIContext apiContext = PaypalConfiguration.GetAPIContext();
                try
                {
                    //A resource representing a Payer that funds a payment Payment Method as paypal  
                    //Payer Id will be returned when payment proceeds or click to pay  
                    string payerId = Request.Params["PayerID"];
                    if (string.IsNullOrEmpty(payerId))
                    {
                        //this section will be executed first because PayerID doesn't exist  
                        //it is returned by the create function call of the payment class  
                        // Creating a payment  
                        // baseURL is the url on which paypal sendsback the data.  
                        string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Home/PayCartMultiple?";
                        //here we are generating guid for storing the paymentID received in session  
                        //which will be used in the payment execution  
                        var guid = Convert.ToString((new Random()).Next(100000));
                        //CreatePayment function gives us the payment approval url  
                        //on which payer is redirected for paypal account payment  
                        var createdPayment = this.CreatePaymentCartMultiple(apiContext, baseURI + "guid=" + guid);
                        //get links returned from paypal in response to Create function call  
                        var links = createdPayment.links.GetEnumerator();
                        string paypalRedirectUrl = null;
                        while (links.MoveNext())
                        {
                            Links lnk = links.Current;
                            if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                            {
                                //saving the payapalredirect URL to which user will be redirected for payment  
                                paypalRedirectUrl = lnk.href;
                            }
                        }
                        // saving the paymentID in the key guid  
                        Session.Add(guid, createdPayment.id);
                        return Redirect(paypalRedirectUrl);
                    }
                    else
                    {
                        // This function exectues after receving all parameters for the payment  
                        var guid = Request.Params["guid"];
                        var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                        //If executed payment failed then we will show payment failure message to user  
                        if (executedPayment.state.ToLower() != "approved")
                        {
                            TempData["Alert"] = "Payment failed!";
                            return RedirectToAction("Index", "Home");
                        }

                    }
                }
                catch (Exception ex)
                {
                    TempData["Alert"] = "Error has been occurred!";
                    return RedirectToAction("Index", "Home");
                }
                //on successful payment, show success page to user.   
                int uidth = new Random().Next(1, 2000000000);
                int total = 0;
                DateTime aDate = DateTime.Now;
                int idu = int.Parse(Session["IDU"].ToString());
                using (var ctx = new ArtGalleryEntities())
                {
                    foreach (var item in cv)
                    {
                        ctx.Transdetails.Add(new Transdetail
                        {
                            IDA = item.IDA,
                            UIDTH = uidth,
                        });
                        var exa = ctx.Arts.Where(a => a.IDA == item.IDA).FirstOrDefault();
                        exa.IDAS = ctx.Artstatus.Where(at => at.NameAS == "Sold").Select(at => at.IDAS).FirstOrDefault();
                        total += Convert.ToInt32(item.PriceA);
                    }
                    ctx.Transhistories.Add(new Transhistory
                    {
                        IDU = idu,
                        DaycreateTH = aDate,
                        Total = total,
                        UIDTH = uidth,
                    });
                    var ex = ctx.Carts.Where(c => c.IDU == idu).ToList();
                    ctx.Carts.RemoveRange(ex);
                    ctx.SaveChanges();
                }

                TempData["Alert"] = "Payment success!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Alert"] = "Your cart is empty, please add at least 1 art to your cart!";
                return RedirectToAction("Index", "Home");
            }

        }

        private Payment CreatePaymentCartMultiple(APIContext apiContext, string redirectUrl)
        {
            List<CartView> cv = new List<CartView>();
            cv = Session["CartList"] as List<CartView>;
            int subtotal = 0;
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            itemList = new ItemList();
            List<Item> items = new List<Item>();
            foreach (var item in cv)
            {

                Item it = new Item();
                it.name = item.NameA;
                it.currency = "USD";
                it.price = (Convert.ToInt32(item.PriceA)).ToString();
                it.quantity = "1";
                it.sku = item.NameA;
                //sku must be difference
                //it.sku = "sku"+item.NameA;
                subtotal += Convert.ToInt32(item.PriceA);
                items.Add(it);
            };
            itemList.items = items;

            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "1",
                shipping = "2",
                subtotal = subtotal.ToString(),
                //subtotal = itemList.items.Sum(s => int.Parse(s.price.ToString()) * int.Parse(s.quantity.ToString())).ToString(), //"589"
                //subtotal="589"
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = (int.Parse(details.tax) + int.Parse(details.shipping) + int.Parse(details.subtotal)).ToString(),// Total must be equal to sum of tax, shipping and subtotal. "592" 
                                                                                                                        //total="592",
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Convert.ToString((new Random()).Next(100000)), //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }

        public ActionResult Test()
        {
            List<CartView> cv = new List<CartView>();
            //cv = (List<CartView>)Session["CartList"];
            cv = Session["CartList"] as List<CartView>;
            if (cv.Count() > 0)
            {
                TempData["Alert"] = "Test success!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Alert"] = "Test failed!";
                return RedirectToAction("Index", "Home");
            }


        }

        public ActionResult ArtInAuctionByID(int ida = 0)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {
                Session["IDA"] = ida;
                ArtGalleryEntities db = new ArtGalleryEntities();
                int idau = db.Arts.Where(a => a.IDA == ida).Select(a => a.IDU).FirstOrDefault();
                if (int.Parse(Session["IDU"].ToString()) == idau)
                {
                    TempData["Alert"] = "You can not join auction your art!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    using (var ctx = new ArtGalleryEntities())
                    {
                        var ex = ctx.Arts.Where(a => a.IDA == ida).FirstOrDefault();
                        string checkartst = ctx.Artstatus.Where(at => at.IDAS == ex.IDAS).Select(at => at.NameAS).FirstOrDefault();
                        if (checkartst == "Cancel auction")
                        {
                            TempData["Alert"] = "This art has been cancel auction!";
                            return RedirectToAction("Index", "Home");
                        }
                        else if (checkartst == "Auction")
                        {
                            AuctionArtView aav = new AuctionArtView();
                            aav.av = GetArtAuctionByID(ida);
                            aav.ahv = GetTop5AuctionArtHistory(ida);

                            if (ctx.Auctionhistories.Where(ah => ah.IDA == ida).Any())
                            {
                                aav.currentPrice = Convert.ToInt32((ctx.Auctionhistories.Where(ah => ah.IDA == ida).OrderByDescending(ah => ah.PriceAu).Select(ah => ah.PriceAu)).First());
                            }
                            else
                            {
                                aav.currentPrice = Convert.ToInt32(ctx.Arts.Where(a => a.IDA == ida).Select(a => a.PriceA).FirstOrDefault());
                            }

                            return View(aav);
                        }
                        else if (checkartst == "Sold")
                        {
                            TempData["Alert"] = "This art has been sold";
                            return RedirectToAction("Index", "Home");
                        }
                        else if (checkartst == "Sell")
                        {
                            TempData["Alert"] = "This art is only for sale!";
                            return RedirectToAction("Index", "Home");
                        }
                        else if (checkartst == "View only")
                        {
                            TempData["Alert"] = "This art is view only!";
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["Alert"] = "Error has ben occurred!";
                            return RedirectToAction("Index", "Home");
                        }
                    }

                }


            }
            else
            {
                TempData["Alert"] = "Please login!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ArtView GetArtAuctionByID(int ida)
        {
            ArtView av = null;
            using (var ctx = new ArtGalleryEntities())
            {
                av = ctx.Arts.Where(a => a.IDA == ida).Select(a => new ArtView
                {
                    IDA = a.IDA,
                    NameA = a.NameA,
                    DaycreateA = a.DaycreateA,
                    DepthA = a.DepthA,
                    DescriptionA = a.DescriptionA,
                    FilePath = a.FilePath,
                    FileSize = a.FileSize,
                    HeightA = a.HeightA,
                    IDAS = a.IDAS,
                    IDAT = a.IDAT,
                    IDC = a.IDC,
                    IDU = a.IDU,
                    IDUC = a.IDUC,
                    LikeCount = a.LikeCount,
                    MaterialA = a.MaterialA,
                    NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                    NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                    NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                    NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                    NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                    PriceA = a.PriceA,
                    StatusA = a.StatusA,
                    Status = a.StatusA == true ? "Unlock" : "Lock",
                    WidthA = a.WidthA,
                }).FirstOrDefault();
            }
            return av;
        }

        public List<AuctionhistoryView> GetTop5AuctionArtHistory(int ida)
        {
            List<AuctionhistoryView> ahv = null;
            using (var ctx = new ArtGalleryEntities())
            {
                ahv = ctx.Auctionhistories.Where(ah => ah.IDA == ida).OrderByDescending(ah => ah.IDAH).Select(ah => new AuctionhistoryView()
                {
                    IDA = ah.IDA,
                    IDU = ah.IDU,
                    IDUA = ah.IDUA,
                    NameA = ctx.Arts.Where(a => a.IDA == ah.IDA).Select(a => a.NameA).FirstOrDefault(),
                    NameU = ctx.Users.Where(a => a.IDU == ah.IDU).Select(a => a.FirstnameU + a.LastnameU).FirstOrDefault(),
                    NameUA = ctx.Users.Where(a => a.IDU == ah.IDUA).Select(a => a.FirstnameU + a.LastnameU).FirstOrDefault(),
                    PriceAu = ah.PriceAu,
                    IDAH = ah.IDAH,
                }).Take(5).ToList<AuctionhistoryView>();
            }
            return ahv;
        }

        [HttpGet]
        public JsonResult GetTop5AuctionParticipants()
        {
            int ida = int.Parse(Session["IDA"].ToString());
            List<AuctionhistoryView> auv = new List<AuctionhistoryView>();
            using (var ctx = new ArtGalleryEntities())
            {
                auv = ctx.Auctionhistories.Where(ah => ah.IDA == ida).OrderByDescending(ah => ah.IDAH).Select(ah => new AuctionhistoryView()
                {
                    IDA = ah.IDA,
                    IDU = ah.IDU,
                    IDUA = ah.IDUA,
                    NameA = ctx.Arts.Where(a => a.IDA == ah.IDA).Select(a => a.NameA).FirstOrDefault(),
                    NameU = ctx.Users.Where(a => a.IDU == ah.IDU).Select(a => a.FirstnameU + a.LastnameU).FirstOrDefault(),
                    NameUA = ctx.Users.Where(a => a.IDU == ah.IDUA).Select(a => a.FirstnameU + a.LastnameU).FirstOrDefault(),
                    PriceAu = ah.PriceAu,
                    IDAH = ah.IDAH,
                }).Take(5).ToList<AuctionhistoryView>();
            }
            return Json(auv, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdatePriceAuction(int ida = 0, int price = 0)
        {
            int idua = int.Parse(Session["IDU"].ToString());
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Auctionhistories.Where(a => a.IDA == ida).OrderByDescending(a => a.PriceAu).FirstOrDefault();
                int highestprice = Convert.ToInt32(ex.PriceAu);
                if (highestprice < price)
                {
                    ctx.Auctionhistories.Add(new Auctionhistory
                    {
                        IDA = ida,
                        IDU = ctx.Arts.Where(a => a.IDA == ida).Select(a => a.IDU).FirstOrDefault(),
                        IDUA = idua,
                        PriceAu = price,
                    });
                    ctx.SaveChanges();
                    return Json(new { success = true, message = "Join auction success!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Your price must higher than highest price!" }, JsonRequestBehavior.AllowGet);
                }

            }
        }

        public ActionResult GetAuctionArtList(int idu = 0)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {
                List<ArtView> av = null;
                using (var ctx = new ArtGalleryEntities())
                {
                    av = ctx.Arts.Where(a => a.IDU == idu && a.IDAS == ctx.Artstatus.Where(at => at.NameAS == "Auction").Select(at => at.IDAS).FirstOrDefault()).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        WidthA = a.WidthA,
                    }).ToList<ArtView>();
                }
                return View(av);
            }
            else
            {
                TempData["Alert"] = "Please login";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult GetAuctionRequestByID(int idu = 0, int ida = 0)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {
                if (int.Parse(Session["IDU"].ToString()) == idu)
                {
                    AuctionArtView aav = new AuctionArtView();
                    aav.av = GetArtAuctionByID(ida);
                    aav.ahv = GetTop5AuctionArtHistory(ida);
                    using (var ctx = new ArtGalleryEntities())
                    {
                        if (ctx.Auctionhistories.Where(ah => ah.IDA == ida).Any())
                        {
                            aav.currentPrice = Convert.ToInt32((ctx.Auctionhistories.Where(ah => ah.IDA == ida).OrderByDescending(ah => ah.PriceAu).Select(ah => ah.PriceAu)).First());
                        }
                        else
                        {
                            aav.currentPrice = Convert.ToInt32(ctx.Arts.Where(a => a.IDA == ida).Select(a => a.PriceA).FirstOrDefault());
                        }
                    }
                    return View(aav);
                }
                else
                {
                    TempData["Alert"] = "You can not manage another auction request!";
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["Alert"] = "Please login";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AcceptAuctionDB(int idah = 0, int idus = 0, int idur = 0, int ida = 0)
        {
            if (Session["IDU"] != null)
            {
                if (int.Parse(Session["IDU"].ToString()) == idus)
                {
                    using (var ctx = new ArtGalleryEntities())
                    {
                        ctx.MessageLists.Add(new MessageList
                        {
                            IDA = ida,
                            IDUS = idus,
                            IDUR = idur,
                            ContentM = "You have successfully bid for this art, click 'Accept' button to be directed to the payment page by paypal.",
                            StatusM = false,
                            IDMT = ctx.Messagetypes.Where(m => m.NameMT == "Auction accept").Select(m => m.IDMT).FirstOrDefault(),
                        });
                        var exre = ctx.Auctionhistories.Where(ah => ah.IDAH != idah);
                        ctx.Auctionhistories.RemoveRange(exre);
                        var ex = ctx.Arts.Where(a => a.IDA == ida).FirstOrDefault();
                        ex.IDAS = ctx.Artstatus.Where(at => at.NameAS == "Cancel auction").Select(at => at.IDAS).FirstOrDefault();
                        //var price = ctx.Auctionhistories.Where(ah => ah.IDAH == idah).Select(ah => ah.PriceAu).FirstOrDefault();
                        //ex.PriceA = price;
                        ctx.SaveChanges();
                    }
                    TempData["Alert"] = "Auction accept message has been sent to your customer!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Alert"] = "You can not manage another auction request!";
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["Alert"] = "Please login";
                return RedirectToAction("Index", "Home");
            }
        }

        public void DeclineAuctionRequestDB(int idah)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Auctionhistories.Where(a => a.IDAH == idah);
                ctx.Auctionhistories.RemoveRange(ex);
                ctx.SaveChanges();
            }
        }

        public void DeclineAuctionMessageDB(int idm)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.MessageLists.Where(a => a.IDM == idm).FirstOrDefault();
                int ida = int.Parse(ex.IDA.ToString());
                int idur = ex.IDUS;
                int idus = int.Parse(Session["IDU"].ToString());
                string nameur = ctx.Users.Where(u => u.IDU == idus).Select(u => u.FirstnameU + u.LastnameU).FirstOrDefault();
                var exa = ctx.Arts.Where(a => a.IDA == ida).FirstOrDefault();
                exa.IDAS = ctx.Artstatus.Where(a => a.NameAS == "Auction").Select(a => a.IDAS).FirstOrDefault();
                ctx.MessageLists.Add(new MessageList
                {
                    ContentM = nameur + " declined to pay for the painting he successfully bid on, contact him/her for further details.",
                    IDA = ida,
                    IDMT = ctx.Messagetypes.Where(m => m.NameMT == "Message").Select(m => m.IDMT).FirstOrDefault(),
                    IDUR = idur,
                    IDUS = idus,
                    StatusM = false,
                });
                ctx.MessageLists.Remove(ex);
                ctx.SaveChanges();
            }
        }

        public ActionResult GetAcceptAuctionArt(int ida = 0)
        {
            if (Session["IDU"] != null)
            {
                using (var ctx = new ArtGalleryEntities())
                {

                }
                return View();
            }
            else
            {
                TempData["Alert"] = "Please login";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult GetIDA(int ida)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Arts.Where(a => a.IDA == ida).FirstOrDefault();
                string checkartst = ctx.Artstatus.Where(at => at.IDAS == ex.IDAS).Select(at => at.NameAS).FirstOrDefault();
                if (checkartst == "Cancel auction")
                {
                    Session["IDA"] = ida;
                    return RedirectToAction("PayAuction", "Home");
                }
                else if (checkartst == "Auction")
                {
                    TempData["Alert"] = "This art is in auction!";
                    return RedirectToAction("Index", "Home");
                }
                else if (checkartst == "Sold")
                {
                    TempData["Alert"] = "This art has been sold";
                    return RedirectToAction("Index", "Home");
                }
                else if (checkartst == "Sell")
                {
                    TempData["Alert"] = "This art is only for sale!";
                    return RedirectToAction("Index", "Home");
                }
                else if (checkartst == "View only")
                {
                    TempData["Alert"] = "This art is view only!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Alert"] = "Error has ben occurred!";
                    return RedirectToAction("Index", "Home");
                }
            }

        }

        public ActionResult PayAuction(string Cancel = null)
        {
            if (Session["Usertype"].ToString() != null)
            {
                //getting the apiContext  
                APIContext apiContext = PaypalConfiguration.GetAPIContext();
                try
                {
                    //A resource representing a Payer that funds a payment Payment Method as paypal  
                    //Payer Id will be returned when payment proceeds or click to pay  
                    string payerId = Request.Params["PayerID"];
                    if (string.IsNullOrEmpty(payerId))
                    {
                        //this section will be executed first because PayerID doesn't exist  
                        //it is returned by the create function call of the payment class  
                        // Creating a payment  
                        // baseURL is the url on which paypal sendsback the data.  
                        string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Home/PayAuction?";
                        //here we are generating guid for storing the paymentID received in session  
                        //which will be used in the payment execution  
                        var guid = Convert.ToString((new Random()).Next(100000));
                        //CreatePayment function gives us the payment approval url  
                        //on which payer is redirected for paypal account payment  
                        var createdPayment = this.CreatePaymentAuction(apiContext, baseURI + "guid=" + guid);
                        //get links returned from paypal in response to Create function call  
                        var links = createdPayment.links.GetEnumerator();
                        string paypalRedirectUrl = null;
                        while (links.MoveNext())
                        {
                            Links lnk = links.Current;
                            if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                            {
                                //saving the payapalredirect URL to which user will be redirected for payment  
                                paypalRedirectUrl = lnk.href;
                            }
                        }
                        // saving the paymentID in the key guid  
                        Session.Add(guid, createdPayment.id);
                        return Redirect(paypalRedirectUrl);
                    }
                    else
                    {
                        // This function exectues after receving all parameters for the payment  
                        var guid = Request.Params["guid"];
                        var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                        //If executed payment failed then we will show payment failure message to user  
                        if (executedPayment.state.ToLower() != "approved")
                        {
                            TempData["Alert"] = "Payment failed!";
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Alert"] = "Error has been occurred!";
                    return RedirectToAction("Index", "Home");
                }
                //on successful payment, show success page to user.
                int uidth = new Random().Next(1, 2000000000);
                DateTime aDate = DateTime.Now;
                int idu = int.Parse(Session["IDU"].ToString());
                int ida = int.Parse(Session["IDA"].ToString());
                using (var ctx = new ArtGalleryEntities())
                {
                    ctx.Transhistories.Add(new Transhistory
                    {
                        DaycreateTH = aDate,
                        IDU = idu,
                        Total = ctx.Auctionhistories.Where(a => a.IDA == ida).Select(a => a.PriceAu).FirstOrDefault(),
                        UIDTH = uidth,

                    });
                    ctx.Transdetails.Add(new Transdetail
                    {
                        IDA = ida,
                        UIDTH = uidth,
                    });
                    var ex = ctx.Arts.Where(a => a.IDA == ida).FirstOrDefault();
                    ex.IDAS = ctx.Artstatus.Where(at => at.NameAS == "Sold").Select(at => at.IDAS).FirstOrDefault();
                    var exau = ctx.Auctionhistories.Where(a => a.IDA == ida).FirstOrDefault();
                    ctx.Auctionhistories.Remove(exau);
                    var exmes = ctx.MessageLists.Where(m => m.IDMT == ctx.Messagetypes.Where(mt => mt.NameMT == "Auction accept").Select(mt => mt.IDMT).FirstOrDefault() && m.IDA == ida).FirstOrDefault();
                    ctx.MessageLists.Remove(exmes);
                    ctx.MessageLists.Add(new MessageList
                    {
                        IDA = ida,
                        IDMT = ctx.Messagetypes.Where(mt => mt.NameMT == "Notification").Select(mt => mt.IDMT).FirstOrDefault(),
                        ContentM = "I have successfully paid for the painting that I have bid on from you, please deliver it to me quickly, you can see my address and phonenumber on my profile.",
                        IDUS = idu,
                        IDUR = ctx.Arts.Where(a => a.IDA == ida).Select(a => a.IDU).FirstOrDefault(),
                        StatusM = false,
                    });
                    ctx.SaveChanges();
                }
                TempData["Alert"] = "Payment success!";
                return RedirectToAction("Index", "Home");

            }
            else
            {
                TempData["Alert"] = "Please login!";
                return RedirectToAction("Index", "Home");
            }

        }

        private Payment CreatePaymentAuction(APIContext apiContext, string redirectUrl)
        {
            int price = 0;
            string name;
            string wdh;
            int ida = int.Parse(Session["IDA"].ToString());
            using (var ctx = new ArtGalleryEntities())
            {
                price = Convert.ToInt32(ctx.Auctionhistories.Where(a => a.IDA == ida).Select(a => a.PriceAu).FirstOrDefault());
                name = ctx.Arts.Where(a => a.IDA == ida).Select(a => a.NameA).FirstOrDefault();
                wdh = ctx.Arts.Where(a => a.IDA == ida).Select(a => a.NameA + a.WidthA + a.DepthA + a.HeightA).FirstOrDefault();
            }
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = name,
                currency = "USD",
                price = price.ToString(),
                quantity = "1",
                sku = wdh
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "1",
                shipping = "2",
                subtotal = itemList.items.Sum(s => int.Parse(s.price.ToString()) * int.Parse(s.quantity.ToString())).ToString(),

            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = (int.Parse(details.tax) + int.Parse(details.shipping) + int.Parse(details.subtotal)).ToString(),// Total must be equal to sum of tax, shipping and subtotal. "592" 

                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Convert.ToString((new Random()).Next(100000)), //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }

        public ActionResult UserDetail(int idu = 0)
        {

            UserView uv = new UserView();
            using (var ctx = new ArtGalleryEntities())
            {
                string prepw = ctx.Users.Where(u => u.IDU == idu).Select(u => u.PasswordU).FirstOrDefault();
                string pwdc = Decrypt(prepw);
                uv = ctx.Users.Where(u => u.IDU == idu).Select(u => new UserView()
                {
                    IDU = u.IDU,
                    FirstnameU = u.FirstnameU,
                    LastnameU = u.LastnameU,
                    PhoneU = u.PhoneU,
                    AddressU = u.AddressU,
                    EmailU = u.EmailU,
                    DofbU = u.DofbU,
                    SexU = u.SexU,
                    UsernameU = u.UsernameU,
                    PasswordU = pwdc,
                    StatusU = u.StatusU,
                    IDUT = u.IDUT,
                    Status = u.StatusU == true ? "Unlock" : "Lock",
                    Usertype = ctx.Usertypes.Where(ut => ut.IDUT == u.IDUT).Select(ut => ut.NameUT).FirstOrDefault(),

                }).FirstOrDefault();
            }
            return View(uv);

        }

        public ActionResult UserArt(int idu = 0)
        {
            var idutmp = 0;

            if (Session["IDU"] != null)
            {
                idutmp = int.Parse(Session["IDU"].ToString());
                List<ArtView> av = null;
                using (var ctx = new ArtGalleryEntities())
                {
                    av = ctx.Arts.Where(a => a.IDU == idu).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        //Likebtn = ctx.LikeLists.Where(l => l.IDU == int.Parse(Session["IDU"].ToString()) && l.IDA == a.IDA).Any()==true?"Unlike":"Like",
                        Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == idutmp && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = ctx.Carts.Where(c => c.IDU == idutmp && c.IDA == a.IDA).Any() == true ? "Already in cart" : "Add to cart"
                    }).ToList<ArtView>();
                }

                return View(av);
            }
            else
            {
                List<ArtView> av = null;
                using (var ctx = new ArtGalleryEntities())
                {
                    av = ctx.Arts.Where(a => a.IDU == idu).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        //Likebtn = ctx.LikeLists.Where(l => l.IDU == int.Parse(Session["IDU"].ToString()) && l.IDA == a.IDA).Any()==true?"Unlike":"Like",
                        Likebtn = "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = "Add to cart"
                    }).ToList<ArtView>();
                }
                return View(av);

            }


        }

        public ActionResult ArtDetail(int ida = 0)
        {
            using (var ctx = new ArtGalleryEntities())
            {

                int idu = ctx.Arts.Where(a => a.IDA == ida).Select(a => a.IDU).FirstOrDefault();
                ArtDetailView adv = new ArtDetailView();
                if (Session["IDU"] != null)
                {
                    int idutmp = int.Parse(Session["IDU"].ToString());
                    adv.av = ctx.Arts.Where(a => a.IDA == ida).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == idutmp && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = ctx.Carts.Where(c => c.IDU == idutmp && c.IDA == a.IDA).Any() == true ? "Already in cart" : "Add to cart"
                    }).FirstOrDefault();
                }
                else
                {
                    adv.av = ctx.Arts.Where(a => a.IDA == ida).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        Likebtn = "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = "Add to cart"
                    }).FirstOrDefault();
                }

                adv.uv = ctx.Users.Where(u => u.IDU == idu).Select(u => new UserView()
                {
                    IDU = u.IDU,
                    FirstnameU = u.FirstnameU,
                    LastnameU = u.LastnameU,
                    PhoneU = u.PhoneU,
                    AddressU = u.AddressU,
                    EmailU = u.EmailU,
                    DofbU = u.DofbU,
                    SexU = u.SexU,
                    StatusU = u.StatusU,
                    IDUT = u.IDUT,
                    Status = u.StatusU == true ? "Unlock" : "Lock",
                    Usertype = ctx.Usertypes.Where(ut => ut.IDUT == u.IDUT).Select(ut => ut.NameUT).FirstOrDefault(),
                }).FirstOrDefault();
                return View(adv);
            }
        }

        public ActionResult TranshistoryList(int idu)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {
                if (int.Parse(Session["IDU"].ToString()) == idu)
                {
                    List<TranshistoryView> thv = null;
                    using (var ctx = new ArtGalleryEntities())
                    {
                        thv = ctx.Transhistories.Where(t => t.IDU == idu).Select(t => new TranshistoryView()
                        {
                            IDTH = t.IDTH,
                            IDU = t.IDU,
                            DaycreateTH = t.DaycreateTH,
                            NameU = ctx.Users.Where(u => u.IDU == idu).Select(u => u.FirstnameU + u.LastnameU).FirstOrDefault(),
                            Total = t.Total,
                            UIDTH = t.UIDTH
                        }).ToList<TranshistoryView>();

                    }
                    return View(thv);
                }
                else
                {
                    TempData["Alert"] = "You can not manage another user category!!!";
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["Alert"] = "Please login";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult TransdetailList(int idu, int uidth)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {
                if (int.Parse(Session["IDU"].ToString()) == idu)
                {
                    List<TransdetailView> tdv = null;
                    using (var ctx = new ArtGalleryEntities())
                    {
                        tdv = ctx.Transdetails.Where(t => t.UIDTH == uidth).Select(t => new TransdetailView()
                        {
                            IDA = t.IDA,
                            UIDTH = t.UIDTH,
                            IDTD = t.IDTD,
                            NameA = ctx.Arts.Where(a => a.IDA == t.IDA).Select(a => a.NameA).FirstOrDefault(),
                            PriceA = ctx.Arts.Where(a => a.IDA == t.IDA).Select(a => a.PriceA).FirstOrDefault(),
                        }).ToList<TransdetailView>();

                    }
                    return View(tdv);
                }
                else
                {
                    TempData["Alert"] = "You can not manage another user category!!!";
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["Alert"] = "Please login";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult UserCategory(int idu)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                List<UsercategoryView> ucv = null;
                ucv = ctx.Usercategories.Where(u => u.IDU == idu && u.StatusUC == true).Select(u => new UsercategoryView()
                {
                    IDU = u.IDU,
                    DaycreateUC = u.DaycreateUC,
                    IDUC = u.IDUC,
                    NameU = ctx.Users.Where(a => a.IDU == u.IDU).Select(a => a.FirstnameU + a.LastnameU).FirstOrDefault(),
                    NameUC = u.NameUC,
                    PriceUC = u.PriceUC
                }).ToList<UsercategoryView>();
                return View(ucv);
            }
        }

        public ActionResult UserArtCategory(int iduc)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                ArtCategory ac = new ArtCategory();
                ac.ucv = ctx.Usercategories.Where(a => a.IDUC == iduc).Select(a => new UsercategoryView()
                {
                    IDUC = a.IDUC,
                    IDU = a.IDU,
                    DaycreateUC = a.DaycreateUC,
                    NameU = ctx.Users.Where(u => u.IDU == a.IDU).Select(u => u.FirstnameU + u.LastnameU).FirstOrDefault(),
                    NameUC = a.NameUC,
                    PriceUC = a.PriceUC,
                }).FirstOrDefault();
                if (Session["IDU"] != null)
                {
                    int idutmp = int.Parse(Session["IDU"].ToString());
                    ac.av = ctx.Arts.Select(a => new ArtView()
                    {

                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        //Likebtn = ctx.LikeLists.Where(l => l.IDU == int.Parse(Session["IDU"].ToString()) && l.IDA == a.IDA).Any()==true?"Unlike":"Like",
                        Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == idutmp && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = ctx.Carts.Where(c => c.IDU == idutmp && c.IDA == a.IDA).Any() == true ? "Already in cart" : "Add to cart"

                    }).ToList<ArtView>();
                }
                else
                {
                    ac.av = ctx.Arts.Select(a => new ArtView()
                    {

                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        //Likebtn = ctx.LikeLists.Where(l => l.IDU == int.Parse(Session["IDU"].ToString()) && l.IDA == a.IDA).Any()==true?"Unlike":"Like",
                        Likebtn = "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = "Add to cart"
                    }).ToList<ArtView>();
                }
                return View(ac);
            }
        }

        public ActionResult GetArtByIDC(int idc = 0)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {

                List<ArtView> av = null;
                using (var ctx = new ArtGalleryEntities())
                {
                    var idutmp = 0;
                    if (Session["IDU"] != null)
                    {
                        idutmp = int.Parse(Session["IDU"].ToString());
                    }

                    av = ctx.Arts.Where(a => a.StatusA == true && a.IDC == idc).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        //Likebtn = ctx.LikeLists.Where(l => l.IDU == int.Parse(Session["IDU"].ToString()) && l.IDA == a.IDA).Any()==true?"Unlike":"Like",
                        Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == idutmp && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = ctx.Carts.Where(c => c.IDU == idutmp && c.IDA == a.IDA).Any() == true ? "Already in cart" : "Add to cart"
                    }).ToList<ArtView>();
                }

                return View(av);
            }
            else
            {
                List<ArtView> av = null;
                using (var ctx = new ArtGalleryEntities())
                {
                    av = ctx.Arts.Where(a => a.StatusA == true && a.IDC == idc).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        Likebtn = "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = "Add to cart"
                    }).ToList<ArtView>();
                }

                return View(av);
            }
        }

        public ActionResult ArtInSale()
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {

                List<ArtView> av = null;
                using (var ctx = new ArtGalleryEntities())
                {
                    var idutmp = 0;
                    if (Session["IDU"] != null)
                    {
                        idutmp = int.Parse(Session["IDU"].ToString());
                    }

                    av = ctx.Arts.Where(a => a.StatusA == true && a.IDAS == ctx.Artstatus.Where(c => c.NameAS == "Sell").Select(c => c.IDAS).FirstOrDefault()).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        //Likebtn = ctx.LikeLists.Where(l => l.IDU == int.Parse(Session["IDU"].ToString()) && l.IDA == a.IDA).Any()==true?"Unlike":"Like",
                        Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == idutmp && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = ctx.Carts.Where(c => c.IDU == idutmp && c.IDA == a.IDA).Any() == true ? "Already in cart" : "Add to cart"
                    }).ToList<ArtView>();
                }

                return View(av);
            }
            else
            {
                List<ArtView> av = null;
                using (var ctx = new ArtGalleryEntities())
                {
                    av = ctx.Arts.Where(a => a.StatusA == true && a.IDAS == ctx.Artstatus.Where(c => c.NameAS == "Sell").Select(c => c.IDAS).FirstOrDefault()).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        Likebtn = "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                        Cartbtn = "Add to cart"
                    }).ToList<ArtView>();
                }

                return View(av);
            }
        }

        public ActionResult ArtInAuction()
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {

                List<ArtView> av = null;
                using (var ctx = new ArtGalleryEntities())
                {
                    var idutmp = 0;
                    if (Session["IDU"] != null)
                    {
                        idutmp = int.Parse(Session["IDU"].ToString());
                    }

                    av = ctx.Arts.Where(a => a.StatusA == true && a.IDAS == ctx.Artstatus.Where(c => c.NameAS == "Auction").Select(c => c.IDAS).FirstOrDefault()).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        //Likebtn = ctx.LikeLists.Where(l => l.IDU == int.Parse(Session["IDU"].ToString()) && l.IDA == a.IDA).Any()==true?"Unlike":"Like",
                        Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == idutmp && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                    }).ToList<ArtView>();
                }

                return View(av);
            }
            else
            {
                List<ArtView> av = null;
                using (var ctx = new ArtGalleryEntities())
                {
                    av = ctx.Arts.Where(a => a.StatusA == true && a.IDAS == ctx.Artstatus.Where(c => c.NameAS == "Auction").Select(c => c.IDAS).FirstOrDefault()).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        Likebtn = "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                    }).ToList<ArtView>();
                }

                return View(av);
            }
        }

        public ActionResult ArtNewest()
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {

                List<ArtView> av = null;
                using (var ctx = new ArtGalleryEntities())
                {
                    var idutmp = 0;
                    if (Session["IDU"] != null)
                    {
                        idutmp = int.Parse(Session["IDU"].ToString());
                    }

                    av = ctx.Arts.Where(a => a.StatusA == true).OrderByDescending(a => a.IDA).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        //Likebtn = ctx.LikeLists.Where(l => l.IDU == int.Parse(Session["IDU"].ToString()) && l.IDA == a.IDA).Any()==true?"Unlike":"Like",
                        Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == idutmp && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                    }).ToList<ArtView>();
                }

                return View(av);
            }
            else
            {
                List<ArtView> av = null;
                using (var ctx = new ArtGalleryEntities())
                {
                    av = ctx.Arts.Where(a => a.StatusA == true).OrderByDescending(a => a.IDA).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        Likebtn = "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                    }).ToList<ArtView>();
                }

                return View(av);
            }
        }

        public ActionResult ArtPopular()
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {

                List<ArtView> av = null;
                using (var ctx = new ArtGalleryEntities())
                {
                    var idutmp = 0;
                    if (Session["IDU"] != null)
                    {
                        idutmp = int.Parse(Session["IDU"].ToString());
                    }

                    av = ctx.Arts.Where(a => a.StatusA == true).OrderByDescending(a => a.LikeCount).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        //Likebtn = ctx.LikeLists.Where(l => l.IDU == int.Parse(Session["IDU"].ToString()) && l.IDA == a.IDA).Any()==true?"Unlike":"Like",
                        Likebtn = (ctx.LikeLists.FirstOrDefault(l => l.IDU == idutmp && l.IDA == a.IDA)) != null ? "Unlike" : "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                    }).ToList<ArtView>();
                }

                return View(av);
            }
            else
            {
                List<ArtView> av = null;
                using (var ctx = new ArtGalleryEntities())
                {
                    av = ctx.Arts.Where(a => a.StatusA == true).OrderByDescending(a => a.LikeCount).Select(a => new ArtView()
                    {
                        IDA = a.IDA,
                        NameA = a.NameA,
                        DaycreateA = a.DaycreateA,
                        DepthA = a.DepthA,
                        DescriptionA = a.DescriptionA,
                        FilePath = a.FilePath,
                        FileSize = a.FileSize,
                        HeightA = a.HeightA,
                        IDAS = a.IDAS,
                        IDAT = a.IDAT,
                        IDC = a.IDC,
                        IDU = a.IDU,
                        IDUC = a.IDUC,
                        Likebtn = "Like",
                        LikeCount = a.LikeCount,
                        MaterialA = a.MaterialA,
                        NameAS = ctx.Artstatus.Where(ast => ast.IDAS == a.IDAS).Select(ast => ast.NameAS).FirstOrDefault(),
                        NameAT = ctx.Arttypes.Where(ast => ast.IDAT == a.IDAT).Select(ast => ast.NameAT).FirstOrDefault(),
                        NameC = ctx.Categories.Where(ast => ast.IDC == a.IDC).Select(ast => ast.NameC).FirstOrDefault(),
                        NameU = ctx.Users.Where(ast => ast.IDU == a.IDU).Select(ast => ast.FirstnameU + ast.LastnameU).FirstOrDefault(),
                        NameUC = ctx.Usercategories.Where(ast => ast.IDUC == a.IDUC).Select(ast => ast.NameUC).FirstOrDefault(),
                        PriceA = a.PriceA,
                        StatusA = a.StatusA,
                        Status = a.StatusA == true ? "Unlock" : "Lock",
                        Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                        WidthA = a.WidthA,
                        Transbtn = ctx.Artstatus.Where(at => at.IDAS == a.IDAS).Select(at => at.NameAS).FirstOrDefault(),
                    }).ToList<ArtView>();
                }

                return View(av);
            }
        }
    }
}