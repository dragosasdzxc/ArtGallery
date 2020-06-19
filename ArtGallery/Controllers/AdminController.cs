using ArtGallery.Models.Views;
using ArtGallery.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace ArtGallery.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult AdminHomePage()
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"].ToString() == "Admin")
            {
                return View();
            }
            else {
                TempData["Alert"] = "You don't have permission to access admin page!";
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult GetUserList(UserView uv)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    List<UserView> us = null;
                    using (var db = new ArtGalleryEntities())
                    {
                        int eid = db.Usertypes.Where(ut => ut.NameUT == "Admin").Select(ut => ut.IDUT).FirstOrDefault();
                        us = db.Users.Where(u => u.IDUT != eid).Select(u => new UserView()
                        {
                            IDU = u.IDU,
                            FirstnameU = u.FirstnameU,
                            LastnameU = u.LastnameU,
                            Status = u.StatusU == true ? "Unlock" : "Lock",
                            Usertype = db.Usertypes.Where(ut => ut.IDUT == u.IDUT).Select(ut => ut.NameUT).FirstOrDefault(),
                            Statusbtn = u.StatusU == true ? "Lock" : "Unlock",
                        }).ToList<UserView>();
                    }
                    return View(us);
                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult CreateNewUsers()
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    ArtGalleryEntities ctx = new ArtGalleryEntities();
                    var usertype = ctx.Usertypes.Where(ut => ut.StatusUT == true).ToList();
                    var viewmodelut = new UserViewAdmin { usertype = usertype };
                    return View(viewmodelut);
                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }

        }

        public ActionResult CreateNewUsersDB(UserViewAdmin uv)
        {
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    DateTime aDate = DateTime.Now;
                    //var date = aDate.ToString("yyyy/MM/dd");

                    string pwcv = Encrypt(uv.user.PasswordU);

                    using (var ctx = new ArtGalleryEntities())
                    {
                        ctx.Users.Add(new User
                        {
                            FirstnameU = uv.user.FirstnameU,
                            LastnameU = uv.user.LastnameU,
                            AddressU = uv.user.AddressU,
                            EmailU = uv.user.EmailU,
                            PhoneU = uv.user.PhoneU,
                            DofbU = uv.user.DofbU,
                            SexU = uv.user.SexU,
                            UsernameU = uv.user.UsernameU,
                            PasswordU = pwcv,
                            DaycreateU = aDate,
                            IDUT = ctx.Usertypes.Where(ut => ut.NameUT == "Artlover").Select(ut => ut.IDUT).FirstOrDefault(),
                            StatusU = true
                        });
                        if (ctx.SaveChanges() > 0)
                        {
                            TempData["Alert"] = "Create successful!";
                            return RedirectToAction("AdminHomePage", "Admin");
                        }
                        else
                        {
                            TempData["Alert"] = "Create failed";
                            return RedirectToAction("CreateNewUsers", "Admin");
                        }


                    }
                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }

        }

        public JsonResult CheckUsernameAvailability(string userdata)
        {
            ArtGalleryEntities ctx = new ArtGalleryEntities();
            System.Threading.Thread.Sleep(200);
            var SeachData = ctx.Users.Where(x => x.UsernameU == userdata).SingleOrDefault();
            if (SeachData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
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
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    UserViewAdmin uva = new UserViewAdmin();
                    uva.user = GetUserById(id);
                    uva.usertypeedit = GetUsertypes();
                    return View(uva);
                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }

        }

        public UserView GetUserById(int id)
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
            return us;
        }

        public IEnumerable<UsertypeView> GetUsertypes()
        {
            IEnumerable<UsertypeView> usertype = null;
            using (var ctx = new ArtGalleryEntities())
            {
                usertype = ctx.Usertypes.Select(u => new UsertypeView()
                {
                    IDUT = u.IDUT,
                    NameUT = u.NameUT,
                }).ToList<UsertypeView>();
            }
            return usertype;
        }

        public ActionResult EditUserDB(int id, UserViewAdmin uv)
        {
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    using (var ctx = new ArtGalleryEntities())
                    {

                        string pwcv = Encrypt(uv.user.PasswordU);

                        var ex = ctx.Users.Where(u => u.IDU == id).FirstOrDefault<User>();
                        ex.FirstnameU = uv.user.FirstnameU;
                        ex.LastnameU = uv.user.LastnameU;
                        ex.AddressU = uv.user.AddressU;
                        ex.EmailU = uv.user.EmailU;
                        ex.PhoneU = uv.user.PhoneU;
                        ex.DofbU = uv.user.DofbU;
                        ex.SexU = uv.user.SexU;
                        ex.UsernameU = uv.user.UsernameU;
                        ex.PasswordU = pwcv;
                        ex.IDUT = uv.user.IDUT;
                        if (ctx.SaveChanges() > 0)
                        {
                            TempData["Alert"] = "Edit successful!";
                            return RedirectToAction("GetUserList", "Admin");
                        }
                        else
                        {
                            TempData["Alert"] = "EDIT FAILED! MAYBE YOU DON'T CHANGE ANYTHING IN YOUR PROFILE, IF THIS ERROR HAS STILL EXISTS AFTER YOU CHANGED YOUR PROFILE INFORMATION, PLEASE CONTACT US!";
                            return RedirectToAction("GetUserList", "Admin");
                        }
                    }
                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult ChangeStatusUserDB(int id) {
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    using (var ctx = new ArtGalleryEntities())
                    {
                        var e = ctx.Users.Where(u => u.IDU == id).FirstOrDefault<User>();
                        if (e.StatusU == true) {
                            e.StatusU = false;
                        } else if (e.StatusU == false) {
                            e.StatusU = true;
                        }
                        if (ctx.SaveChanges() > 0)
                        {
                            return RedirectToAction("GetUserList", "Admin");
                        }
                        else {
                            TempData["Alert"] = "Error has been occured!";
                            return RedirectToAction("GetUserList", "Admin");
                        }
                    }
                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public void LockUser(int id) {
            using (var ctx = new ArtGalleryEntities()) {
                var ex = ctx.Users.Where(u => u.IDU == id).FirstOrDefault<User>();
                ex.StatusU = false;
                ctx.MessageLists.Add(new MessageList
                {
                    IDMT = ctx.Messagetypes.Where(mt => mt.NameMT == "Message").Select(mt => mt.IDMT).FirstOrDefault(),
                    IDUS = int.Parse(Session["IDU"].ToString()),
                    IDUR = id,
                    StatusM = false,
                    ContentM = "Your account has been locked!"
                });
                ctx.SaveChanges();
            }
        }

        public void UnlockUser(int id)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Users.Where(u => u.IDU == id).FirstOrDefault<User>();
                ex.StatusU = true;
                ctx.MessageLists.Add(new MessageList
                {
                    IDMT = ctx.Messagetypes.Where(mt => mt.NameMT == "Message").Select(mt => mt.IDMT).FirstOrDefault(),
                    IDUS = int.Parse(Session["IDU"].ToString()),
                    IDUR = id,
                    StatusM = false,
                    ContentM = "Your account has been unlocked, be careful and do not be lock again!"
                });
                ctx.SaveChanges();
            }
        }

        public void DeleteUser(int id) {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Users.Where(u => u.IDU == id).FirstOrDefault<User>();
                ctx.Entry(ex).State = System.Data.Entity.EntityState.Deleted;
                ctx.SaveChanges();
            }
        }

        /*public ActionResult GetUsertypeList() {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    List<UsertypeView> ut = null;
                    using (var ctx = new ArtGalleryEntities())
                    {
                        int eid = ctx.Usertypes.Where(utt => utt.NameUT == "Admin").Select(utt => utt.IDUT).FirstOrDefault();
                        ut = ctx.Usertypes.Where(u => u.IDUT != eid).Select(u => new UsertypeView()
                        {
                            IDUT=u.IDUT,
                            NameUT=u.NameUT,
                            StatusUT=u.StatusUT,
                            Status=u.StatusUT==true?"Unlock":"Lock",
                            Statusbtn=u.StatusUT==true?"Lock":"Unlock"
                        }).ToList<UsertypeView>();
                    }
                    return View(ut);
                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult CreateNewUsertype() {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    return View();
                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult CreateNewUsertypeDB(UsertypeView utv)
        {
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    using (var ctx = new ArtGalleryEntities()) {
                        ctx.Usertypes.Add(new Usertype()
                        {
                            NameUT = utv.NameUT,
                            StatusUT=true
                        });
                        ctx.SaveChanges();
                    }
                    TempData["Alert"] = "Create success!";
                    return RedirectToAction("GetUsertypeList", "Admin");
                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult EditUsertype(int id=0) {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    UsertypeView utv = null;
                    using (var db = new ArtGalleryEntities())
                    {
                        utv = db.Usertypes.Where(u => u.IDUT == id).Select(u => new UsertypeView()
                        {
                            IDUT=u.IDUT,
                            NameUT=u.NameUT,
                        }).FirstOrDefault();
                    }
                    return View(utv);

                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult EditUsertypeDB(int id,UsertypeView utv) {
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    using (var ctx = new ArtGalleryEntities())
                    {

                        var ex = ctx.Usertypes.Where(u => u.IDUT == id).FirstOrDefault<Usertype>();
                        ex.NameUT = utv.NameUT;
                        if (ctx.SaveChanges() > 0)
                        {
                            TempData["Alert"] = "Edit successful!";
                            return RedirectToAction("GetUsetyperList", "Admin");
                        }
                        else
                        {
                            TempData["Alert"] = "Edit failed! Maybe you don't change anything, if this error still exists after changed, please report to us!";
                            return RedirectToAction("GetUsetyperList", "Admin");
                        }
                    }

                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public void LockUsertype(int id) {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Usertypes.Where(u => u.IDUT == id).FirstOrDefault<Usertype>();
                ex.StatusUT = false;
                ctx.SaveChanges();
            }
        }

        public void UnlockUsertype(int id) {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Usertypes.Where(u => u.IDUT == id).FirstOrDefault<Usertype>();
                ex.StatusUT = true;
                ctx.SaveChanges();
            }
        }*/

        public ActionResult GetCategoryList() {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    List<CategoryView> cv = null;
                    using (var ctx = new ArtGalleryEntities()) {
                        cv = ctx.Categories.Select(c => new CategoryView()
                        {
                            IDC = c.IDC,
                            NameC = c.NameC,
                            StatusC = c.StatusC,
                            Status = c.StatusC == true ? "Unlock" : "Lock",
                            Statusbtn = c.StatusC == true ? "Lock" : "Unlock"
                        }).ToList<CategoryView>();
                    }
                    return View(cv);
                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult CreateNewCategory() {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    return View();
                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult CreateNewCategoryDB(CategoryView cv) {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    using (var ctx = new ArtGalleryEntities())
                    {
                        ctx.Categories.Add(new Category
                        {
                            NameC = cv.NameC,
                            StatusC = true
                        });
                        ctx.SaveChanges();
                    }
                    TempData["Alert"] = "Create success!";
                    return RedirectToAction("GetCategoryList", "Admin");
                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult EditCategory(int id = 0)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    CategoryView utv = null;
                    using (var db = new ArtGalleryEntities())
                    {
                        utv = db.Categories.Where(u => u.IDC == id).Select(u => new CategoryView()
                        {
                            IDC = u.IDC,
                            NameC = u.NameC,
                        }).FirstOrDefault();
                    }
                    return View(utv);

                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult EditCategoryDB(int id, CategoryView utv)
        {
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    using (var ctx = new ArtGalleryEntities())
                    {

                        var ex = ctx.Categories.Where(u => u.IDC == id).FirstOrDefault<Category>();
                        ex.NameC = utv.NameC;
                        if (ctx.SaveChanges() > 0)
                        {
                            TempData["Alert"] = "Edit successful!";
                            return RedirectToAction("GetCategoryList", "Admin");
                        }
                        else
                        {
                            TempData["Alert"] = "Edit failed! Maybe you don't change anything, if this error still exists after changed, please report to us!";
                            return RedirectToAction("GetCategoryList", "Admin");
                        }
                    }

                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public void LockCategory(int id)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Categories.Where(u => u.IDC == id).FirstOrDefault<Category>();
                ex.StatusC = false;
                ctx.SaveChanges();
            }
        }

        public void UnlockCategory(int id)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Categories.Where(u => u.IDC == id).FirstOrDefault<Category>();
                ex.StatusC = true;
                ctx.SaveChanges();
            }
        }

        public ActionResult GetArttypeList()
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    List<ArttypeView> cv = null;
                    using (var ctx = new ArtGalleryEntities())
                    {
                        cv = ctx.Arttypes.Select(c => new ArttypeView()
                        {
                            IDAT = c.IDAT,
                            NameAT = c.NameAT,
                            StatusA = c.StatusA,
                            Status = c.StatusA == true ? "Unlock" : "Lock",
                            Statusbtn = c.StatusA == true ? "Lock" : "Unlock"
                        }).ToList<ArttypeView>();
                    }
                    return View(cv);
                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult CreateNewArttype()
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    return View();
                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult CreateNewArttypeDB(ArttypeView cv)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    using (var ctx = new ArtGalleryEntities())
                    {
                        ctx.Arttypes.Add(new Arttype
                        {
                            NameAT = cv.NameAT,
                            StatusA = true
                        });
                        ctx.SaveChanges();
                    }
                    TempData["Alert"] = "Create success!";
                    return RedirectToAction("GetArttypeList", "Admin");
                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult EditArttype(int id = 0)
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    ArttypeView utv = null;
                    using (var db = new ArtGalleryEntities())
                    {
                        utv = db.Arttypes.Where(u => u.IDAT == id).Select(u => new ArttypeView()
                        {
                            IDAT = u.IDAT,
                            NameAT = u.NameAT,
                        }).FirstOrDefault();
                    }
                    return View(utv);

                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult EditArttypeDB(int id, ArttypeView utv)
        {
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    using (var ctx = new ArtGalleryEntities())
                    {

                        var ex = ctx.Arttypes.Where(u => u.IDAT == id).FirstOrDefault<Arttype>();
                        ex.NameAT = utv.NameAT;
                        if (ctx.SaveChanges() > 0)
                        {
                            TempData["Alert"] = "Edit successful!";
                            return RedirectToAction("GetArttypeList", "Admin");
                        }
                        else
                        {
                            TempData["Alert"] = "Edit failed! Maybe you don't change anything, if this error still exists after changed, please report to us!";
                            return RedirectToAction("GetArttypeList", "Admin");
                        }
                    }

                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public void LockArttype(int id)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Arttypes.Where(u => u.IDAT == id).FirstOrDefault<Arttype>();
                ex.StatusA = false;
                ctx.SaveChanges();
            }
        }

        public void UnlockArttype(int id)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Arttypes.Where(u => u.IDAT == id).FirstOrDefault<Arttype>();
                ex.StatusA = true;
                ctx.SaveChanges();
            }
        }

        public ActionResult GetAllArt() {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    using (var ctx = new ArtGalleryEntities())
                    {
                        List<ArtView> av = new List<ArtView>();
                        av = ctx.Arts.Select(a => new ArtView()
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
                            Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                            WidthA = a.WidthA
                        }).ToList<ArtView>();
                        return View(av);
                    }

                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public void LockArt(int id)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Arts.Where(u => u.IDA == id).FirstOrDefault<Art>();
                ex.StatusA = false;
                ctx.MessageLists.Add(new MessageList
                {
                    IDMT = ctx.Messagetypes.Where(mt => mt.NameMT == "Message").Select(mt => mt.IDMT).FirstOrDefault(),
                    IDUS = int.Parse(Session["IDU"].ToString()),
                    IDUR = ex.IDU,
                    StatusM = false,
                    ContentM = "Your art has been lock by admin for violating our laws, please check this art here and change to comply with our laws!",
                    IDA = id
                });
                ctx.SaveChanges();
            }
        }

        public void UnlockArt(int id)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Arts.Where(u => u.IDA == id).FirstOrDefault<Art>();
                ex.StatusA = true;
                ctx.MessageLists.Add(new MessageList
                {
                    IDMT = ctx.Messagetypes.Where(mt => mt.NameMT == "Message").Select(mt => mt.IDMT).FirstOrDefault(),
                    IDUS = int.Parse(Session["IDU"].ToString()),
                    IDUR = ex.IDU,
                    StatusM = false,
                    ContentM = "Your art has been unlock by admin, be careful and do not be lock again!",
                    IDA = id
                });
                ctx.SaveChanges();
            }
        }

        public void DeleteArt(int id)
        {
            using (var ctx = new ArtGalleryEntities())
            {
                var ex = ctx.Arts.Where(u => u.IDA == id).FirstOrDefault<Art>();
                string path = ex.FilePath;
                FileInfo file = new FileInfo(path);
                file.Delete();
                ctx.Entry(ex).State = System.Data.Entity.EntityState.Deleted;
                var el = ctx.LikeLists.Where(l => l.IDA == id);
                ctx.LikeLists.RemoveRange(el);
                ctx.SaveChanges();

            }
        }

        public ActionResult ArtDetail(int id = 0) {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    using (var ctx = new ArtGalleryEntities())
                    {
                        ArtView av = new ArtView();
                        av = ctx.Arts.Where(a => a.IDA == id).Select(a => new ArtView()
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
                            Statusbtn = a.StatusA == true ? "Lock" : "Unlock",
                            WidthA = a.WidthA
                        }).FirstOrDefault();
                        return View(av);
                    }

                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult GetTranshistoryList() {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    using (var ctx = new ArtGalleryEntities())
                    {
                        List<TranshistoryView> tv = null;
                        tv = ctx.Transhistories.Select(t => new TranshistoryView()
                        {
                            DaycreateTH = t.DaycreateTH,
                            IDTH = t.IDTH,
                            IDU = t.IDU,
                            NameU = ctx.Users.Where(u => u.IDU == t.IDU).Select(u => u.FirstnameU + u.LastnameU).FirstOrDefault(),
                            Total = t.Total,
                            UIDTH = t.UIDTH,
                        }).ToList<TranshistoryView>();
                        return View(tv);
                    }

                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult GetTransdetailByID(int uid) {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
                {
                    TransView trans = new TransView();
                    using (var ctx = new ArtGalleryEntities())
                    {
                        List<TransdetailView> tv = null;
                        tv = ctx.Transdetails.Where(t => t.UIDTH == uid).Select(t => new TransdetailView()
                        {
                            IDA = t.IDA,
                            UIDTH = t.UIDTH,
                            IDTD = t.IDTD,
                            NameA = ctx.Arts.Where(a => a.IDA == t.IDA).Select(a => a.NameA).FirstOrDefault(),
                            PriceA = ctx.Arts.Where(a => a.IDA == t.IDA).Select(a => a.PriceA).FirstOrDefault()
                        }).ToList<TransdetailView>();
                        TranshistoryView th = null;
                        th = ctx.Transhistories.Where(t => t.UIDTH == uid).Select(t => new TranshistoryView()
                        {
                            IDTH = t.IDTH,
                            UIDTH = t.UIDTH,
                            DaycreateTH = t.DaycreateTH,
                            IDU = t.IDU,
                            NameU = ctx.Users.Where(u => u.IDU == t.IDU).Select(u => u.FirstnameU + u.LastnameU).FirstOrDefault(),
                            Total = t.Total
                        }).FirstOrDefault();
                        trans.tdv = tv;
                        trans.thv = th;
                        return View(trans);
                    }

                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult GetAllArtByID(int id) {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            if (Session["Usertype"] != null)
            {
                if (Session["Usertype"].ToString() == "Admin")
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
                        return View(av);
                    }

                }
                else
                {
                    TempData["Alert"] = "You don't have permission to access this page! ONLY FOR ADMIN!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Alert"] = "Please login by admin account to acces this page!";
                return RedirectToAction("Login", "Login");
            }
        }
    }
}
