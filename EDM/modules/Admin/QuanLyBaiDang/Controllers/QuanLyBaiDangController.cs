using Applications._Others.Interfaces;
using Newtonsoft.Json;
using Public.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LocThongTin_BaiDang = Applications.QuanLyBaiDang.Dtos.LocThongTinDto;
using LocThongTin_ChienDich = Applications.QuanLyChienDich.Dtos.LocThongTinDto;

namespace QuanLyBaiDang.Controllers
{
    [CustomAuthorize]
    public class QuanLyBaiDangController : Controller
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyBaiDang";
        private readonly IQuanLyBaiDangService _quanLyBaiDangAppService;
        private readonly IQuanLyChienDichAppService _quanLyChienDichAppService;
        private readonly IQuanLyAIToolService _quanLyAIToolAppService;
        private readonly IQuanLyAIBotService _quanLyAIBotAppService;
        private readonly IOtherAppService _otherAppService;
        public QuanLyBaiDangController(
            IQuanLyBaiDangService quanLyBaiDangService,
            IQuanLyChienDichAppService quanLyChienDichService,
            IQuanLyAIToolService quanLyAIToolAppService,
            IQuanLyAIBotService quanLyAIBotAppService,
            IOtherAppService otherAppService)
        {
            _quanLyBaiDangAppService = quanLyBaiDangService;
            _quanLyChienDichAppService = quanLyChienDichService;
            _quanLyAIToolAppService = quanLyAIToolAppService;
            _quanLyAIBotAppService = quanLyAIBotAppService;
            _otherAppService = otherAppService;
        }
        #endregion

        public async Task<ActionResult> Index()
        {
            var output = await _quanLyBaiDangAppService.Index_OutPut();
            return View($"{VIEW_PATH}/baidang.cshtml", output);
        }

        #region Bài đăng
        [HttpPost]
        public async Task<ActionResult> getList_BaiDang(LocThongTin_BaiDang input)
        {
            var baiDangs = await _quanLyBaiDangAppService.GetBaiDangs(loai: "all", locThongTin: input);
            var output = new GetList_BaiDang_Output_Dto
            {
                BaiDangs = baiDangs.ToList(),
                ThaoTacs = _quanLyBaiDangAppService.GetThaoTacs(maChucNang: "QuanLyBaiDang"),
            };
            return PartialView($"{VIEW_PATH}/quanlybaidang-tab/baidang/baidang-getList.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_BaiDang(DisplayModel_CRUD_BaiDang_Input_Dto input)
        {
            //var output = await _quanLyBaiDangAppService.DisplayModal_CRUD_BaiDang_Output(input: input);
            var html = Public.Handle.RenderViewToString(
              controller: this,
              viewName: $"{VIEW_PATH}/quanlybaidang-tab/baidang/baidang-crud/baidang-crud.cshtml",
              model: input);
            return Json(new
            {
                html,
                output = input
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> xemChiTiet_BaiDang(Guid idBaiDang)
        {
            var output = await _quanLyBaiDangAppService.XemChiTiet_BaiDang(input: idBaiDang);
            return View($"{VIEW_PATH}/quanlybaidang-tab/baidang/xemchitiet/xemchitiet.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> addBanGhi_Modal_CRUD(AddBanGhi_Modal_CRUD_Input_Dto input)
        {
            var output = await _quanLyBaiDangAppService.AddBanGhi_Modal_CRUD_Output(input: input);

            output.LoaiView = "row";
            var html_baidang_row = Public.Handle.RenderViewToString(
                controller: this,
                viewName: $"{VIEW_PATH}/quanlybaidang-tab/baidang/baidang-crud/form-addbaidang.cshtml",
                model: output);

            output.LoaiView = "read";
            var html_baidang_read = Public.Handle.RenderViewToString(
                controller: this,
                viewName: $"{VIEW_PATH}/quanlybaidang-tab/baidang/baidang-crud/form-addbaidang.cshtml",
                model: output);

            return Json(new
            {
                status = (output.Data.BaiDangs != null && output.Data.BaiDangs.Count > 0),
                Loai = output.Data.Loai,
                html_baidang_row,
                html_baidang_read
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> create_BaiDang(HttpPostedFileBase[] files, Guid[] rowNumbers)
        {
            try
            {
                var baiDang_NEWs = JsonConvert.DeserializeObject<List<tbBaiDangExtend>>(Request.Form["baiDangs"]);
                var loai = Request.Form["loai"];
                if (baiDang_NEWs == null || !baiDang_NEWs.Any())
                {
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);
                }

                await _quanLyBaiDangAppService.Create_BaiDang(loai: loai, baiDangs: baiDang_NEWs, files: files, rowNumbers: rowNumbers);

                return Json(new { status = "success", mess = "Thêm mới bản ghi thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<ActionResult> update_BaiDang(HttpPostedFileBase[] files, Guid[] rowNumbers)
        {
            try
            {
                var baiDangs = JsonConvert.DeserializeObject<List<tbBaiDangExtend>>(Request.Form["baiDangs"]);
                var loai = Request.Form["loai"];
                if (baiDangs == null || !baiDangs.Any())
                {
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);
                }

                await _quanLyBaiDangAppService.Update_BaiDang(loai: loai, baiDangs: baiDangs, files: files, rowNumbers: rowNumbers);

                return Json(new { status = "success", mess = "Cập nhật bản ghi thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> delete_BaiDangs()
        {
            try
            {
                var idBaiDangs = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idBaiDangs"]);
                if (idBaiDangs == null || idBaiDangs.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                // Gọi AppService xử lý logic chính
                await _quanLyBaiDangAppService.Delete_BaiDangs(idBaiDangs: idBaiDangs);

                return Json(new { status = "success", mess = "Xóa bản ghi thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> taoNoiDungAI(TaoNoiDungAI_Input_Dto input)
        {
            string status = "success";
            string mess = "Đã tạo nội dung AI";
            string noiDung = "";
            try
            {
                var aiBot = await _quanLyAIBotAppService.GetAIBots(loai: "single", idAIBots: new List<Guid> { input.IdAIBot });
                if (aiBot == null || !aiBot.Any() || aiBot.FirstOrDefault().AIBot.IdAIBot == Guid.Empty)
                {
                    return Json(new
                    {
                        status = "error",
                        mess = "Không tìm thấy thông tin AI Bot"
                    });
                }
                string prompt = aiBot.FirstOrDefault().AIBot.Prompt += string.Format("\n {0}: {1}",
                     "[THÔNG TIN CUNG CẤP] (nếu không có gì thì bỏ qua)",
                     input.Keywords);
                noiDung = await _quanLyAIToolAppService.WorkWithAITool(input: new WorkWithAITool_Input_Dto
                {
                    IdAITool = input.IdAITool,
                    Prompt = prompt,
                });
            }
            catch (Exception ex)
            {
                status = "error";
                mess = ex.ToString();
            }
            ;
            return Json(new
            {
                NoiDung = noiDung,
                status,
                mess
            });
        }
        [HttpPost]
        public async Task<JsonResult> chonLoaiAIBot(Guid idAIBot)
        {
            try
            {
                var aiBot = await _quanLyAIBotAppService.GetAIBots(loai: "single", idAIBots: new List<Guid> { idAIBot });
                if (aiBot != null && aiBot.FirstOrDefault().AIBot.IdAIBot != Guid.Empty)
                    return Json(new
                    {
                        status = "success",
                        mess = "Lựa chọn AI Bot thành công, hãy nhập keywords để tạo nội dung",
                        Keywords = aiBot.FirstOrDefault().AIBot.Keywords,
                    });
                return Json(new
                {
                    status = "warning",
                    mess = "Không tìm thấy thông tin AI Bot",
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "error",
                    mess = ex.ToString()
                });
            }
            ;

        }
        #endregion

        #region Chiến dịch
        [HttpPost]
        public async Task<ActionResult> getList_ChienDich(LocThongTin_ChienDich input)
        {
            var chienDichs = await _quanLyChienDichAppService.GetChienDichs(loai: "all", locThongTin: input);
            var output = new GetList_ChienDich_Output_Dto
            {
                ChienDichs = chienDichs.ToList(),
                ThaoTacs = _quanLyAIBotAppService.GetThaoTacs(maChucNang: "QuanLyChienDich"),
            };
            return PartialView($"{VIEW_PATH}/quanlybaidang-tab/chiendich/chiendich-getList.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_ChienDich(DisplayModel_CRUD_ChienDich_Input_Dto input)
        {
            var chienDich = await _quanLyChienDichAppService.GetChienDichs(loai: "single", idChienDichs: new List<Guid> { input.IdChienDich });
            var output = new DisplayModel_CRUD_ChienDich_Output_Dto
            {
                Loai = input.Loai,
                ChienDich = chienDich.FirstOrDefault(),
            };
            return PartialView($"{VIEW_PATH}/quanlybaidang-tab/chiendich/chiendich-crud.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> create_ChienDich()
        {
            try
            {
                var chienDich_NEW = JsonConvert.DeserializeObject<tbChienDich>(Request.Form["chienDich"]);
                if (chienDich_NEW == null)
                {
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);
                }

                await _quanLyChienDichAppService.Create_ChienDich(chienDich: chienDich_NEW);

                return Json(new { status = "success", mess = "Thêm mới bản ghi thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> delete_ChienDichs()
        {
            try
            {
                var idChienDichs = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idChienDichs"]);
                if (idChienDichs == null || idChienDichs.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                // Gọi AppService xử lý logic chính
                await _quanLyChienDichAppService.Delete_ChienDichs(idChienDichs: idChienDichs);

                return Json(new { status = "success", mess = "Xóa bản ghi thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}