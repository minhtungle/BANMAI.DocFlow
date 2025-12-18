using Applications.QuanLyAIBot.Dtos;
using Applications.QuanLyAIBot.Interfaces;
using Applications.QuanLyAIBot.Models;
using Applications.QuanLyAITool.Dtos;
using Applications.QuanLyAITool.Interfaces;
using EDM_DB;
using Newtonsoft.Json;
using Public.Controllers;
using Public.Helpers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QuanLyAIBot.Controllers
{
    [CustomAuthorize]
    public class QuanLyAIBotController : Controller
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyAIBot";
        public readonly IQuanLyAIBotService _quanLyAIBotService;
        private readonly IQuanLyAIToolService _quanLyAIToolService;
        public QuanLyAIBotController(
            IQuanLyAIBotService quanLyAIBotAppService,
            IQuanLyAIToolService quanLyAIToolAppService)
        {
            _quanLyAIBotService = quanLyAIBotAppService;
            _quanLyAIToolService = quanLyAIToolAppService;
        }
        #endregion

        public async Task<ActionResult> Index()
        {
            var output = await _quanLyAIBotService.Index_OutPut();

            return View($"{VIEW_PATH}/aibot.cshtml", output);
        }
        [HttpPost]
        public async Task<JsonResult> taoNoiDungAI(TaoNoiDungAI_Input_Dto input)
        {
            try
            {
                string noiDung = await _quanLyAIBotService.TaoNoiDungAI(input: input);
                return Json(new
                {
                    NoiDung = noiDung,
                    status = "success",
                    mess = "Đã tạo nội dung AI",
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    NoiDung = "",
                    status = "error",
                    mess = ex.ToString(),
                });
            }
         ;
        }
        [HttpPost]
        public async Task<JsonResult> testNoiDungAI(TestNoiDungAI_Input_Dto input)
        {
            try
            {
                string noiDung = await _quanLyAIBotService.TestNoiDungAI(input: input);
                return Json(new
                {
                    NoiDung = noiDung,
                    status = "success",
                    mess = "Đã tạo nội dung AI",
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    NoiDung = "",
                    status = "error",
                    mess = ex.ToString(),
                });
            }
         ;
        }

        #region AIBot
        [HttpGet]
        public async Task<ActionResult> getList_AIBot()
        {
            var data = await _quanLyAIBotService.GetAIBots(loai: "all");
            var output = new GetList_AIBot_Output_Dto
            {
                AIBots = data.ToList(),
                ThaoTacs = _quanLyAIBotService.GetThaoTacs(maChucNang: "QuanLyAIBot"),
            };
            return PartialView($"{VIEW_PATH}/quanlyaibot-tab/aibot/aibot-getList.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_AIBot(DisplayModel_CRUD_AIBot_Input_Dto input)
        {
            var output = await _quanLyAIBotService.DisplayModel_CRUD_AIBot_Output(input: input);
            return PartialView($"{VIEW_PATH}/quanlyaibot-tab/aibot/aibot-crud.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> displayModal_Test_AIBot(Guid input)
        {
            var output = await _quanLyAIBotService.DisplayModel_Test_AIBot_Output(input: input);
            return PartialView($"{VIEW_PATH}/quanlyaibot-tab/aibot/aibot-test.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> create_AIBot()
        {
            try
            {
                var aiBot_NEW = JsonConvert.DeserializeObject<tbAIBotExtend>(Request.Form["aiBot"]);
                if (aiBot_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyAIBotService.IsExisted_AIBot(
                    aiBot: aiBot_NEW.AIBot);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyAIBotService.Create_AIBot(aiBot: aiBot_NEW);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
       ;
        }
        [HttpPost]
        public async Task<ActionResult> update_AIBot()
        {
            try
            {
                var aiBot_NEW = JsonConvert.DeserializeObject<tbAIBotExtend>(Request.Form["aiBot"]);
                if (aiBot_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyAIBotService.IsExisted_AIBot(
                    aiBot: aiBot_NEW.AIBot);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyAIBotService.Update_AIBot(aiBot: aiBot_NEW);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
       ;
        }
        [HttpPost]
        public async Task<JsonResult> delete_AIBot()
        {
            try
            {
                var idAIBots = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idAIBots"]);
                if (idAIBots == null || idAIBots.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                // Gọi AppService xử lý logic chính
                await _quanLyAIBotService.Delete_AIBot(idAIBots: idAIBots);

                return Json(new { status = "success", mess = "Xóa bản ghi thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Loại AIBot
        [HttpGet]
        public async Task<ActionResult> getList_LoaiAIBot()
        {
            var data = await _quanLyAIBotService.GetLoaiAIBots(loai: "all");
            var output = new GetList_LoaiAIBot_Output_Dto
            {
                LoaiAIBots = data.ToList(),
                ThaoTacs = _quanLyAIBotService.GetThaoTacs(maChucNang: "QuanLyAIBot"),
            };
            return PartialView($"{VIEW_PATH}/quanlyaibot-tab/loaiaibot/loaiaibot-getList.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_LoaiAIBot(DisplayModel_CRUD_LoaiAIBot_Input_Dto input)
        {
            var output = await _quanLyAIBotService.DisplayModel_CRUD_LoaiAIBot_Output(input: input);
            return PartialView($"{VIEW_PATH}/quanlyaibot-tab/loaiaibot/loaiaibot-crud.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> create_LoaiAIBot()
        {
            try
            {
                var loaiAiBot_NEW = JsonConvert.DeserializeObject<tbLoaiAIBot>(Request.Form["loaiAiBot"]);
                if (loaiAiBot_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyAIBotService.IsExisted_LoaiAIBot(loaiAIBot: loaiAiBot_NEW);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyAIBotService.Create_LoaiAIBot(loaiAIBot: loaiAiBot_NEW);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            ;
        }
        [HttpPost]
        public async Task<ActionResult> update_LoaiAIBot()
        {
            try
            {
                var loaiAIBot_NEW = JsonConvert.DeserializeObject<tbLoaiAIBot>(Request.Form["loaiAIBot"]);
                if (loaiAIBot_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyAIBotService.IsExisted_LoaiAIBot(
                    loaiAIBot: loaiAIBot_NEW);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyAIBotService.Update_LoaiAIBot(loaiAIBot: loaiAIBot_NEW);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
       ;
        }
        [HttpPost]
        public async Task<JsonResult> delete_LoaiAIBot()
        {
            try
            {
                var idLoaiAIBots = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idLoaiAIBots"]);
                if (idLoaiAIBots == null || idLoaiAIBots.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                // Gọi AppService xử lý logic chính
                await _quanLyAIBotService.Delete_LoaiAIBot(idLoaiAIBots: idLoaiAIBots);

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