using TopStyleShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;

namespace ShopQuanAoLite.Controllers
{
    public class NguoiDungController : Controller
    {
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();

        #region Function
        //ma hoa md5
        private string mahoamd5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var dulieu = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                var builder = new StringBuilder();

                for (int i = 0; i < dulieu.Length; i++)
                {
                    builder.Append(dulieu[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        //check ký tự 
        public static bool checkkitu(string input)
        {
            char[] specialChar = { ' ', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '{', '}', '[', ']', '|', '\\', ':', ';', '\"', '\'', '<', '>', ',', '.', '?', '/' };
            foreach (char item in input)
            {
                if (specialChar.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }
        //check khoangtrang
        public static bool checkkhoangtrang(string input)
        {
            return input.Contains(" ");
        }
        //Check có tài khoản chưa 
        private bool checktk(string Username)
        {
            return data.KhachHangs.Count(x => x.TenDangNhap == Username) > 0;
        }
        #endregion
        // GET: NguoiDung
        public ActionResult Index()
        {
            return View();
        }

        //Đăng ký 
        #region Đăng ký
   
                [HttpGet]
                public PartialViewResult DangKy()
                {
                    return PartialView();
                }
                [HttpPost]
                public ActionResult Dangky(FormCollection collection, KhachHang kh)
                {
                    // Gán các giá tị người dùng nhập liệu cho các biến 
                    var tendangnhap = collection["TenDangnhap"];
                    var matkhau = collection["MatKhau"];
                    var nhaplaimatkhau = collection["NhapLaiMatkhau"];
                    var hoten = collection["HoTen"];
                    var diachi = collection["Diachi"];
                    var email = collection["Email"];
                    var sodienthoai = collection["SoDienthoai"];
                    if (String.IsNullOrEmpty(hoten))
                    {
                        ViewData["hoten"] = "Họ tên khách hàng không được để trống";
                    }
                    else if (String.IsNullOrEmpty(tendangnhap))
                    {
                        ViewData["tendangnhap"] = "Phải nhập tên đăng nhập";
                    }
                    else if (checkkhoangtrang(tendangnhap))
                    {
                        ViewData["tendangnhap"] = "Tên đăng nhập có khoảng trắng";
                    }
                    else if (String.IsNullOrEmpty(matkhau))
                     {
                      ViewData["matkhau"] = "Phải nhập mật khẩu";
                     }
                    else if (checkkhoangtrang(matkhau))
                    {
                        ViewData["matkhau"] = "Mật khẩu có khoảng trắng";
                    }
                     else if (String.IsNullOrEmpty(nhaplaimatkhau))
                    {
                        ViewData["nhaplaimatkhau"] = "Phải nhập lại mật khẩu";
                    }
                    else if (nhaplaimatkhau != matkhau)
                    {
                        ViewData["nhaplaimatkhau"] = "Không khớp với mật khẩu";
                    }

                     if (String.IsNullOrEmpty(email))
                    {
                        ViewData["email"] = "Email không được bỏ trống";
                    }

                    if (String.IsNullOrEmpty(sodienthoai))
                    {
                        ViewData["sodienthoai"] = "Phải nhập điện thoại";
                    }
                    else
                    {
                        //Gán giá trị cho đối tượng được tạo mới (kh)
                        kh.TenDangNhap = tendangnhap;
                        kh.MatKhau = mahoamd5(matkhau);
                        kh.HoTen = hoten;
                        kh.DiaChi = diachi;
                        kh.Email = email;
                        kh.SoDienThoai = sodienthoai;
                        data.KhachHangs.InsertOnSubmit(kh);
                        data.SubmitChanges();
                        return RedirectToAction("DangNhap");
                    }
                    return this.DangKy();
                }

        #endregion

        //Đăng nhập 
        #region Đăng nhập
        [HttpGet]

        public PartialViewResult Dangnhap()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            // Gán các giá trị người dùng nhập liệu cho các biến 
            var tendangnhap = collection["Tendangnhap_dangnhap"];
            var matkhau = collection["Matkhau_dangnhap"];
            if (String.IsNullOrEmpty(tendangnhap))
            {
                ViewData["tendangnhap"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["matkhau"] = "Phải nhập mật khẩu";
            }
            else
            {
                //Gán giá trị cho đối tượng được tạo mới (kh)
                KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.TenDangNhap == tendangnhap && n.MatKhau == mahoamd5(matkhau));
                if (kh != null)
                {
                    Session["Taikhoan"] = kh;
                    Session["tenkhachhang"] = kh.HoTen;
                    ViewBag.Thongbao = "Đăng nhập thành công";
                    return RedirectToAction("Index", "Home");   
                }
                else
                    ViewData["thongbao"] = "Sai tài khoản hoặc mật khẩu";
            }
            return this.Dangnhap();

        }
        public ActionResult LoginPartial()
        {
            return PartialView("_LoginPartial");
        }

        #endregion
        //Đăng xuất 
        #region Đăng xuất 
        //Đăng xuất 
        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        #endregion
        //Profile 
        #region
        public ActionResult Profile(KhachHang khachhang)
        {
            Session["profile"] = null;
            return View(khachhang);
        }
        #endregion

    }

}