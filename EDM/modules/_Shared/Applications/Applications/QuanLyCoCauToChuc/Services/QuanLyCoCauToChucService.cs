using Applications.QuanLyCoCauToChuc.Interfaces;
using Applications.QuanLyNguoiDung.Interfaces;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Interfaces;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Applications.QuanLyCoCauToChuc.Services
{
    public class QuanLyCoCauToChucService : BaseService, IQuanLyCoCauToChucService
    {
        private readonly IRepository<tbCoCauToChuc, Guid> _coCauToChucRepo;
        public QuanLyCoCauToChucService(
            IUserContext userContext,
            IUnitOfWork unitOfWork,
            IRepository<tbCoCauToChuc, Guid> coCauToChucRepo
            )
            : base(userContext, unitOfWork)
        {
            _coCauToChucRepo = coCauToChucRepo;
        }
        [HttpPost]
        public async Task<Tree<tbCoCauToChuc>> Get_CoCauToChucs_Tree(Guid idCoCau)
        {
            async Task<List<Tree<tbCoCauToChuc>>> makeTree(Guid idCha)
            {
                // Tạo nhánh
                List<Tree<tbCoCauToChuc>> nodes = new List<Tree<tbCoCauToChuc>>();
                // Tìm con
                //List<tbCoCauToChuc> _coCaus = new List<tbCoCauToChuc>();
                //_coCaus = db.tbCoCauToChucs.Where(x => x.MaDonViSuDung == maDonViSuDung && x.TrangThai == 1 && x.IdCha == idCha)
                //    .OrderByDescending(x => x.IdCoCauToChuc).ToList();
                List<tbCoCauToChuc> coCaus = await _coCauToChucRepo.Query()
                    .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung
                        && x.IdCha == idCha)
                    .OrderByDescending(x => x.NgayTao)
                    .ToListAsync();
               
                for (int i = 0; i < coCaus.Count; i++)
                {
                    tbCoCauToChuc coCau = coCaus[i];
                    // 1 node gồm nhiều con hơn
                    Tree<tbCoCauToChuc> tree = new Tree<tbCoCauToChuc>();
                    tree.SoThuTu = (i + 1);
                    tree.root = coCau;
                    tree.nodes = await makeTree(coCau.IdCoCauToChuc);
                    nodes.Add(tree);
                }
                return nodes;
            }
            Tree<tbCoCauToChuc> _tree = new Tree<tbCoCauToChuc>
            {
                nodes = await makeTree(idCoCau),
            };
            return _tree;
        }
        public async Task XuLy_TenCoCauToChuc(
            List<Tree<tbCoCauToChuc>> coCaus_IN, 
            List<tbCoCauToChuc> coCaus_OUT, 
            string mucLuc = "", 
            string khoangCachDauDong = "",
            bool kichHoat = true)
        {
            foreach (Tree<tbCoCauToChuc> coCau_IN in coCaus_IN)
            {
                string mucLuc_NEW = string.Format("{0}{1}.", mucLuc, coCau_IN.SoThuTu);
                int capDo = coCau_IN.root.CapDo ?? 0;
                int capDoCha = capDo >= 1 ? capDo - 1 : capDo;
                string khoangCachDauDong_NEW = String.Join("", Enumerable.Repeat(khoangCachDauDong, capDoCha).ToArray());

                tbCoCauToChuc coCau_OUT = new tbCoCauToChuc
                {
                    IdCoCauToChuc = coCau_IN.root.IdCoCauToChuc,
                    TenCoCauToChuc = kichHoat ? string.Format("{0} {1} {2}", khoangCachDauDong_NEW, mucLuc_NEW, coCau_IN.root.TenCoCauToChuc) : coCau_IN.root.TenCoCauToChuc,
                    CapDo = coCau_IN.root.CapDo,
                    IdCha = coCau_IN.root.IdCha,
                    GhiChu = coCau_IN.root.GhiChu,
                    TrangThai = coCau_IN.root.TrangThai,
                    MaDonViSuDung = coCau_IN.root.MaDonViSuDung,
                    NgayTao = coCau_IN.root.NgayTao,
                    IdNguoiTao = coCau_IN.root.IdNguoiTao,
                    NgaySua = coCau_IN.root.NgaySua,
                    IdNguoiSua = coCau_IN.root.IdNguoiSua
                };
                coCaus_OUT.Add(coCau_OUT);
                await XuLy_TenCoCauToChuc(coCaus_OUT: coCaus_OUT, coCaus_IN: coCau_IN.nodes, mucLuc: mucLuc_NEW, khoangCachDauDong: khoangCachDauDong, kichHoat: kichHoat);
            }
            ;
        }
    }
}