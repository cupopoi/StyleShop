using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopStyleShop.Models;
using TopStyleShop.ViewModels;

namespace TopStyleShop.Controllers
{
    public class GioHangController : Controller
    {
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();
        // GET: GioHang
        public ActionResult Index()
        {
            return View();
        }
        #region Giỏ hàng tạm thời
        public List<GioHangdViewModel> LayGioHang()
        {
            List<GioHangdViewModel> lstGioHang = Session["GioHang"] as List<GioHangdViewModel>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHangdViewModel>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        public ActionResult ThemGioHang(int masp, string strURL)
        {

            //Lay ra Session gio hang
            List<GioHangdViewModel> lstGiohang = LayGioHang();
            //Kiem tra sách này tồn tại trong Session["Giohang"] chưa?
            GioHangdViewModel sanpham = lstGiohang.Find(n => n.MaSP == masp);
            if (sanpham == null)
            {
                sanpham = new GioHangdViewModel(masp);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.SoLuong++;
                return Redirect(strURL);
            }
        }
        //Tong so luong
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHangdViewModel> lstGiohang = Session["GioHang"] as List<GioHangdViewModel>;
            if (lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.SoLuong);
            }
            return iTongSoLuong;
        }
        //Tinh tong tien
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHangdViewModel> lstGiohang = Session["GioHang"] as List<GioHangdViewModel>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.ThanhTien);
            }
            return iTongTien;
        }
        #endregion

        //Xay dung trang Gio hang
        #region Lưu giỏ hàng
        public ActionResult GioHang()
        {
            List<GioHangdViewModel> lstGiohang = LayGioHang();
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Shop");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }
        //Tao Partial view de hien thi thong tin gio hang
        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }
        //Cap nhat Giỏ hàng
        public ActionResult CapnhatGiohang(int masp, FormCollection f)
        {

            //Lay gio hang tu Session
            List<GioHangdViewModel> lstGiohang = LayGioHang();
            //Kiem tra sach da co trong Session["Giohang"]
            GioHangdViewModel sanpham = lstGiohang.SingleOrDefault(n => n.MaSP == masp);
            //Neu ton tai thi cho sua Soluong
            if (sanpham != null)
            {
                sanpham.SoLuong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Giohang");
        }
     
        //Xoa Giohang

        public ActionResult XoaGiohang(int masp)
        {
            //Lay gio hang tu Session
            List<GioHangdViewModel> lstGiohang = LayGioHang();
            //Kiem tra sach da co trong Session["Giohang"]
            GioHangdViewModel sanpham = lstGiohang.SingleOrDefault(n => n.MaSP == masp);
            //Neu ton tai thi cho sua Soluong
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.MaSP == masp);
                return RedirectToAction("GioHang");

            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Shop");
            }
            return RedirectToAction("GioHang");
        }
        //Xoa tat ca thong tin trong Gio hang
        public ActionResult XoaTatcaGiohang()
        {
            //Lay gio hang tu Session
            List<GioHangdViewModel> lstGiohang = LayGioHang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "Shop");
        }

        #endregion

        //Đặt hàng
        #region Đặt hàng
        [HttpGet]
        public ActionResult DatHang()
        {
            //Kiem tra dang nhap
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "Shop");
            }

            //Lay gio hang tu Session
            List<GioHangdViewModel> lstGiohang = LayGioHang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();

            return View(lstGiohang);
        }
        //Xay dung chuc nang Dathang
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            //Them Don hang
            HoaDon ddh = new HoaDon();
            KhachHang kh = (KhachHang)Session["Taikhoan"];
            List<GioHangdViewModel> gh = LayGioHang();
            ddh.MaKH = kh.MaKH;
            ddh.DiaChiGiaoHang = kh.DiaChi;
            ddh.NgayLapHD = DateTime.Now;
            ddh.TrangThai = false;
            ddh.GhiChu = collection["GhiChu"];
            data.HoaDons.InsertOnSubmit(ddh);
            data.SubmitChanges();
            //Them chi tiet don hang            
            foreach (var item in gh)
            {
                ChiTietHoaDon ctdh = new ChiTietHoaDon();
                ctdh.MaHD = ddh.MaHD;
                ctdh.MaSP = item.MaSP;
                ctdh.TenSP = (item.MaSP + " - " + item.TenSP);
                ctdh.MaS = item.MaS;
                ctdh.SoLuong = item.SoLuong;
                ctdh.DonGia = (int)item.DonGia;
                data.ChiTietHoaDons.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }
        public ActionResult Xacnhandonhang()
        {
            return View();
        }
        #endregion
    }

}