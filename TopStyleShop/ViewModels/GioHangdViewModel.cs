using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TopStyleShop.Models;

namespace TopStyleShop.ViewModels
{
    public class GioHangdViewModel
    {
        dbShopQuanAoDataContext data = new dbShopQuanAoDataContext();

        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public string AnhBia { get; set; }
        public int MaS { get; set; }
        public double DonGia { get; set; }
        public int SoLuong { get; set; }
        public string GhiChu { get; set; }
        public int GiaKhuyenMai { get; set; }
        public List<SanPham> SanPhams { get; set; }
        public string TenS { get; set; }
        public Double ThanhTien
        {
            get { return SoLuong * DonGia; }
        }
        public GioHangdViewModel(int id)
        {
            MaSP = id;
            SanPham sanpham = data.SanPhams.Single(n => n.MaSP == id);
            var sanphamsizes = (from s in data.SanPhamSizes
                                where s.MaSP == id
                                select s).FirstOrDefault();

            var sizes = (from s in data.Sizes
                         where s.MaS == sanphamsizes.MaS
                         select s).FirstOrDefault();
            TenS = sizes.TenS;
            GiaKhuyenMai = (int)sanpham.GiaKhuyenMai;
            TenSP = sanpham.TenSP;
            AnhBia = sanpham.AnhBia;
            MaS = sanphamsizes.MaS;
            DonGia = double.Parse(sanpham.GiaBan.ToString());
            SoLuong = 1;

        }
    }
}