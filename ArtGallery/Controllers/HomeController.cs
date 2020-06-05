using ArtGallery.Models.Entities;
using ArtGallery.Models.Views;
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
                        Cartbtn = ctx.Carts.Where(c => c.IDU == idutmp && c.IDA==a.IDA).Any() == true ? "Already in cart" : "Add to cart"
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
                        Cartbtn = "Add to cart"
                    }).ToList<ArtView>();
                }

                return View(av);
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

        public ActionResult EditUser(int id)
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
                else {
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
                                    LikeCount=0                                    
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
                        Cartbtn = "Add to cart"
                    }).ToList<ArtView>();
                }

                return View(av);
            }
        }

        public ActionResult GetArtListByID(int id)
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
                else {
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

        public void LockArtByID(int ida, int idu)
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
        }

        public ActionResult EditArtByID(int ida, int idu)
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
                else {
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

        public void DeleteArt(int ida, int idu)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Arts.Where(u => u.IDU == idu && u.IDA == ida).FirstOrDefault<Art>();
                string path = ex.FilePath;
                FileInfo file = new FileInfo(path);
                file.Delete();
                ctx.Entry(ex).State = System.Data.Entity.EntityState.Deleted;
                var el = ctx.LikeLists.Where(l => l.IDA == ida);
                ctx.LikeLists.RemoveRange(el);
                ctx.SaveChanges();
            }
        }

        public ActionResult AddToCart(int ida)
        {
            if (Session["IDU"] != null)
            {
                using (var ctx = new ArtGalleryEntities())
                {
                    ctx.Carts.Add(new Cart()
                    {
                        IDA = ida,
                        IDU = int.Parse(Session["IDU"].ToString()),
                        StatusCA=true
                    });
                    ctx.SaveChanges();
                }
                return Json(new { success = true, message = "Thank for choose my art!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Error has been occurred!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetUserCategoryByID(int id) {
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
                else {
                    TempData["Alert"] = "You can not manage another user category!!!";
                    return RedirectToAction("Index", "Home");
                }
                
            }
            else
            {
                TempData["Alert"] = "Please login";
                return RedirectToAction("Index","Home");
            }
        }

        public ActionResult CreateNewUserCategory() {
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
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult CreateNewUserCategoryDB(UsercategoryView ucv)
        {
            if (Session["IDU"] != null)
            {
                using (var ctx = new ArtGalleryEntities()) {
                    DateTime aDate = DateTime.Now;
                    ctx.Usercategories.Add(new Usercategory
                    {
                        NameUC=ucv.NameUC,
                        StatusUC=true,
                        PriceUC=0,
                        DaycreateUC=aDate,
                        IDU=int.Parse(Session["IDU"].ToString())
                    });
                    ctx.SaveChanges();
                }
                TempData["Alert"] = "Create success!";
                return RedirectToAction("Index","Home");
            }
            else
            {
                TempData["Alert"] = "Please login";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult GetArtInUCByID(int iduc,int idu) {
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

        public ActionResult ChooseArtByID(int idu) {
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
                        ucv = ctx.Arts.Where(a => a.IDU==idu  && a.IDUC!=iduc).Select(a => new ArtView()
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
                            UCbtn= (ctx.Arts.FirstOrDefault(ar=>ar.IDA==a.IDA && ar.IDUC==iduc)) != null ? "Remove" : "Add",
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

        public void RemoveArtFromUserCategoryByID(int ida) {
            using (var ctx = new ArtGalleryEntities()) {
                var ex = ctx.Arts.Where(a => a.IDA == ida).FirstOrDefault();
                ex.IDUC = 1;
                ctx.SaveChanges();
            }
        }

        public void AddArtToUserCategoryDB(int ida) {
            int iduc = int.Parse(Session["UCval"].ToString());
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Arts.Where(a => a.IDA == ida).FirstOrDefault();
                ex.IDUC = iduc;
                ctx.SaveChanges();
                Session["UCval"] = null;

            }
        }

        public void LockUserCategoryByID(int iduc, int idu)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Usercategories.Where(u => u.IDUC == iduc && u.IDU == idu).FirstOrDefault<Usercategory>();
                ex.StatusUC= false;
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

        public void DeleteUserCategoryDB(int iduc, int idu) {
            using (var ctx = new ArtGalleryEntities()) {
                var ex = ctx.Arts.Where(a => a.IDUC == iduc).ToList();
                ex.ForEach(a=>a.IDUC=1);
                var e = ctx.Usercategories.Where(u => u.IDUC == iduc).FirstOrDefault();
                ctx.Usercategories.Remove(e);
                ctx.SaveChanges();
            }
        }

        public ActionResult GetCartList(int idu) {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["IDU"] != null)
            {
                if (int.Parse(Session["IDU"].ToString()) == idu)
                {
                    List<CartView> cv = null;
                    using (var ctx = new ArtGalleryEntities())
                    {
                        cv = ctx.Carts.Where(c => c.IDU == idu).Select(c => new CartView()
                        {
                            IDCA = c.IDCA,
                            IDA = c.IDA,
                            IDU = c.IDU,
                            NameA = ctx.Arts.Where(a => a.IDA == c.IDA).Select(a => a.NameA).FirstOrDefault(),
                        }).ToList<CartView>();

                    }
                    return View();
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
    }
}