using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopStyleShop.Models;
using System.Data;
using System.Configuration;
using System.Diagnostics;
using TopStyleShop.ViewModels;
using PagedList;
using PagedList.Mvc;



namespace TopStyleShop.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();
        public List<SanPham> Laymoi(int count)
        {

            return data.SanPhams.OrderByDescending(a => a.ngayNhapHang).Take(count).ToList();
        }
        public List<SanPham> Layhet()
        {

            return data.SanPhams.ToList();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IndexLayhet(int? Page)
        {
            int pageSize = 5;
            //Tao bien so trang
            int pageNum = (Page ?? 1);
            //Lấy top 5 Album bán chạy nhất
            var layhet = Layhet();
            return PartialView(layhet.ToPagedList(pageNum, pageSize));
        }
        public PartialViewResult IndexLaymoi()
        {
            var quanaomoi = Laymoi(5);
            return PartialView(quanaomoi);
        }
        public PartialViewResult SPTheoLoai(int id)
        {
            var sanpham = from s in data.SanPhams where s.MaL == id select s;
            return PartialView(sanpham);
        }
        public PartialViewResult LoaiSP()
        {
            var loaisp = from cd in data.LoaiSanPhams select cd;
            return PartialView(loaisp);
        }

        // Id = MaGioiTinh
        public PartialViewResult LoaiSPTheoGioiTinhNam()
        {
            int nam = 1;

            var loaisp = from cd in data.LoaiSanPhams
                         where cd.MaGioiTinh == nam
                         select cd;
            return PartialView(loaisp);
        }
        public PartialViewResult LoaiSPTheoGioiTinhNu()
        {
            int nu = 0;
            var loaisp = from cd in data.LoaiSanPhams
                         where cd.MaGioiTinh == nu
                         select cd;

            return PartialView(loaisp);
        }
        public PartialViewResult Details(int id)
        {
            var sanpham = (from s in data.SanPhams
                           where s.MaSP == id
                           select s).FirstOrDefault();

            var sanPhamCungLoai = data.SanPhams
                .Where(sp => sp.LoaiSanPham == sanpham.LoaiSanPham)
                .Take(3);

            var binhluan = data.BinhLuans
                .Where(bl => bl.MaSP == id);
            var hinhanh = data.HinhAnhs
                .Where(ha => ha.MaSP == id);
            /* var size = from sz in data.Sizes select sz;*/
            var sanphamsize = data.SanPhamSizes
                .Where(sps => sps.MaSP == id);
            var size = from sz in data.Sizes select sz;

            // Debugging
            binhluan.ToList().ForEach(bl => Debug.WriteLine(bl.NoiDung));

            var detailView = new DetailedViewModel
            {
                SanPham = sanpham,
                SanPhamCungloai = sanPhamCungLoai,
                BinhLuan = binhluan,
                HinhAnh = hinhanh,
                SanPhamSize = sanphamsize,
                Size = size,
            };
            return PartialView(detailView);
        }
        public PartialViewResult DanhMucBST()
        {
            var bosuutap = from cd in data.BoSuuTaps select cd;

            return PartialView(bosuutap);
        }
        public PartialViewResult SanPhamTheoBST(int id)
        {
            var sanphamtheobst = from cd in data.SanPhams
                                 where cd.MaBST == id
                                 select cd;

            return PartialView(sanphamtheobst);
        }


        [HttpPost]
        public PartialViewResult Search(string pid)
        {
            int x;
            bool resl = int.TryParse(pid, out x);
            if (resl != false)
            {
                var det = (from d in data.SanPhams
                           where d.MaSP == x
                           select d).ToList();
                return PartialView(det);
            }
            else
            {
                var det = (from d in data.SanPhams
                           where d.TenSP == pid
                           select d).ToList();
                return PartialView(det);
            }

        }
    }

}