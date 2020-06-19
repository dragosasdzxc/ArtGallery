using ArtGallery.Models.Entities;
using ArtGallery.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace ArtGallery.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            if (TempData["Alert"] != null)
            {
                ViewBag.Alert = TempData["Alert"];
                TempData.Remove("Alert");
            }
            return View();
        }

        public ActionResult LoginDB(UserView uv) {
            using (var ctx = new ArtGalleryEntities()) {
                
                string pwcv = Encrypt(uv.PasswordU);
                if (Session["IDU"] == null)
                {
                    var checkus= ctx.Users.Where(a => a.UsernameU.Equals(uv.UsernameU)).FirstOrDefault();
                    if (checkus != null)
                    {
                        var obj = ctx.Users.Where(a => a.UsernameU.Equals(uv.UsernameU) && a.PasswordU.Equals(pwcv)).FirstOrDefault();
                        if (obj != null)
                        {
                            var ut = ctx.Usertypes.Where(t => t.IDUT == obj.IDUT).Select(t => t.NameUT).FirstOrDefault().ToString();
                            var sta = obj.StatusU == true ? "Unlock" : "Lock";
                            if (sta == "Unlock")
                            {
                                if (ut == "Admin")
                                {
                                    Session["IDU"] = obj.IDU;
                                    Session["UsernameU"] = obj.UsernameU;
                                    Session["Usertype"] = ctx.Usertypes.Where(utt => utt.IDUT == obj.IDUT).Select(utt => utt.NameUT).FirstOrDefault();
                                    //TempData["Alert"] = "Welcome admin!";
                                    return RedirectToAction("AdminHomePage", "Admin");
                                }
                                else
                                {
                                    Session["IDU"] = obj.IDU;
                                    Session["UsernameU"] = obj.UsernameU;
                                    Session["Usertype"] = ctx.Usertypes.Where(utt => utt.IDUT == obj.IDUT).Select(utt => utt.NameUT).FirstOrDefault();
                                    //TempData["Alert"] = "Have a nice day!";
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                            else
                            {
                                TempData["Alert"] = "Your account has been locked!";
                                return RedirectToAction("Login", "Login");
                            }
                        }
                        else
                        {
                            TempData["Alert"] = "Your password is wrong!";
                            return RedirectToAction("Login", "Login");
                        }
                    }
                    else {
                        TempData["Alert"] = "Your account not exist!";
                        return RedirectToAction("Login", "Login");
                    }
                    
                }
                else
                {
                    TempData["Alert"] = "Please log out to be able to log in with another account!";
                    return RedirectToAction("Index", "Home");
                }                
            }
        }

        public ActionResult LogoutDB() {
            if (Session["IDU"] != null)
            {
                Session["IDU"] = null;
                Session["UsernameU"] = null;
                Session["Usertype"] = null;
                //TempData["Alert"] = "Logout successful!";
                return RedirectToAction("Index", "Home");
            }
            else {
                TempData["Alert"] = "You not log in with any account!";
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
    }
}