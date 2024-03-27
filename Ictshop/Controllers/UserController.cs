using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;
namespace Ictshop.Controllers
{
    public class UserController : Controller
    {
        string tempMail;
        Qlbanhang db = new Qlbanhang();
        [HttpPost]
        public ActionResult DoiMk([Bind(Include = "MaNguoiDung,Hoten,Email,Dienthoai,Matkhau,IDQuyen, Anhdaidien,Diachi")] Nguoidung nguoidung)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nguoidung).State = EntityState.Modified;
                db.SaveChanges();
                return View("./Dangnhap");
            }
            return View("./Doimatkhau");
        }

        public ActionResult Sendmail()
        {
            return View("./Quenmatkhau");
        }

        [HttpPost]
        public ActionResult Sendmail(MailModel _objModelMail)
        {
            if (Session["temp"] == null)
            {
                Random rnd = new Random();
                int randNum = rnd.Next(100000, 999999);
                Session["temp"] = randNum;
            }
            if (ModelState.IsValid && db.Nguoidungs.SingleOrDefault(x => x.Email.Equals(_objModelMail.To)) != null)
            {
                if ((int)Session["temp"] == _objModelMail.code)
                {
                    //Nguoidung temp = (Nguoidung)from x in db.Nguoidungs where x.Email == _objModelMail.To select x;
                    List<Nguoidung> temp = db.Nguoidungs.Where(x => x.Email.Equals(_objModelMail.To)).ToList();
                    Session["tempEmail"] = _objModelMail.To;
                    tempMail = _objModelMail.To;
                    Session["temp"] = null;
                    return View("./Doimatkhau", temp[0]);
                }
                MailMessage mail = new MailMessage();
                mail.To.Add(_objModelMail.To);
                mail.From = new MailAddress(_objModelMail.From = "thuvienhanoi97@gmail.com");
                mail.Subject = _objModelMail.Subject = "Laptop36 gửi mã xác nhận thay đổi mật khẩu";
                string Body = _objModelMail.Body = "Mã xác nhận thay đổi mật khẩu là: "+ (int)Session["temp"];
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("thuvienhanoi97@gmail.com", "lxnx syzz smac vjvv"); // Enter seders User name and password  
                smtp.EnableSsl = true;
                smtp.Send(mail);
                ViewBag.codeSent = true;
                return View("./Quenmatkhau", _objModelMail);
            }
            else
            {
                return View("./Quenmatkhau");
            }
        }

        // ĐĂNG KÝ
        public ActionResult Dangky()
        {
            return View();
        }

        // ĐĂNG KÝ PHƯƠNG THỨC POST
        [HttpPost]
        public ActionResult Dangky(Nguoidung nguoidung)
        {
            try
            {
                Session["userReg"] = nguoidung;

                // Thêm người dùng  mới
                db.Nguoidungs.Add(nguoidung);
                // Lưu lại vào cơ sở dữ liệu
                db.SaveChanges();
                // Nếu dữ liệu đúng thì trả về trang đăng nhập
                if (ModelState.IsValid)
                {
                    //return RedirectToAction("Dangnhap");
                    ViewBag.RegOk = "Đăng kí thành công. Đăng nhập ngay";
                    ViewBag.isReg = true;
                    return View("Dangky");
                }
                else
                {
                    return View("Dangky");
                }
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Dangnhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Dangnhap(LoginModel model)
        {
            string userMail = model.userMail;
            string password = model.password;
            var islogin = db.Nguoidungs.SingleOrDefault(x => x.Email.Equals(userMail) && x.Matkhau.Equals(password));
            if (islogin != null)
            {
                if (userMail == "Admin@gmail.com")
                {
                    Session["use"] = islogin;
                    return RedirectToAction("Index", "Admin/Home");
                }
                else
                {
                    Session["use"] = islogin;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.Fail = "Tài khoản hoặc mật khẩu không chính xác.";
                return View("Dangnhap");
            }
        }
        public ActionResult DangXuat()
        {
            Session["use"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Profile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nguoidung nguoiDung = db.Nguoidungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }
            return View(nguoidung);
        }

        // POST: Admin/Nguoidungs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNguoiDung,Hoten,Email,Dienthoai,Matkhau,IDQuyen, Anhdaidien,Diachi")] Nguoidung nguoidung)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nguoidung).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Profile", new { id = nguoidung.MaNguoiDung });
            }           
            return View(nguoidung);
        }
        public static byte[] encryptData(string data)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashedBytes;
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(data));
            return hashedBytes;
        }
        public static string md5(string data)
        {
            return BitConverter.ToString(encryptData(data)).Replace("-", "").ToLower();
        }
    }
}