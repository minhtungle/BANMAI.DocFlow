using Applications.QuanLyBaiDang.Dtos;
using Applications.QuanLyBaiDang.Enums;
using Applications.QuanLyBaiDang.Interfaces;
using Applications.QuanLyBaiDang.Models;
using Applications.SocialApi._Instagram.Dtos;
using Applications.SocialApi.Dtos;
using Applications.SocialApi.Interfaces;
using EDM_DB;
using Infrastructure.Interfaces;
using Newtonsoft.Json;
using Public.AppServices;
using Public.Helpers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TrangThaiDangBai_BaiDang = Applications.QuanLyBaiDang.Enums.TrangThaiDangBaiEnum;

namespace Applications.QuanLyBaiDang.Serivices
{
    public class QuanLyBaiDangService : BaseService, IQuanLyBaiDangService
    {
        private readonly IRepository<tbBaiDang, Guid> _baiDangRepo;
        private readonly IRepository<tbChienDich, Guid> _chienDichRepo;
        private readonly IRepository<tbTepDinhKem, Guid> _tepDinhKemRepo;
        private readonly IRepository<tbBaiDangTepDinhKem, Guid> _baiDangTepDinhKemRepo;
        private readonly IRepository<tbNenTang, Guid> _nenTangRepo;
        private readonly IRepository<tbNguoiDung, Guid> _nguoiDungRepo;
        private readonly IRepository<tbAIBot, Guid> _aiBotRepo;
        private readonly IRepository<tbAITool, Guid> _aiToolRepo;
        private readonly IRepository<tbTaiKhoan, Guid> _taiKhoanRepo;
        private readonly ISocialApiService _socialApiService;

        private readonly IRepository<tbApiCredential, Guid> _apiCredentialRepo;

        public QuanLyBaiDangService(
            IUserContext userContext,
            IUnitOfWork unitOfWork,
            IRepository<tbBaiDang, Guid> baiDangRepo,
            IRepository<tbChienDich, Guid> chienDichRepo,
            IRepository<tbTepDinhKem, Guid> tepDinhKemRepo,
            IRepository<tbBaiDangTepDinhKem, Guid> baiDangTepDinhKemRepo,
            IRepository<tbNenTang, Guid> nenTangRepo,
            IRepository<tbNguoiDung, Guid> nguoiDungRepo,
            IRepository<tbAIBot, Guid> aiBotRepo,
            IRepository<tbAITool, Guid> aiToolRepo,
            IRepository<tbTaiKhoan, Guid> taiKhoanRepo,
            ISocialApiService socialApiService,

            IRepository<tbApiCredential, Guid> apiCredentialRepo)
            : base(userContext, unitOfWork)
        {
            _baiDangRepo = baiDangRepo;
            _chienDichRepo = chienDichRepo;
            _tepDinhKemRepo = tepDinhKemRepo;
            _baiDangTepDinhKemRepo = baiDangTepDinhKemRepo;
            _nenTangRepo = nenTangRepo;
            _nguoiDungRepo = nguoiDungRepo;
            _aiBotRepo = aiBotRepo;
            _aiToolRepo = aiToolRepo;
            _taiKhoanRepo = taiKhoanRepo;
            _apiCredentialRepo = apiCredentialRepo;
            _socialApiService = socialApiService;
        }
        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);

        public async Task<Index_OutPut_Dto> Index_OutPut()
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyBaiDang");
            var nenTangs = await _nenTangRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung).ToListAsync();
            var nguoiTaos = await _nguoiDungRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung).ToListAsync();
            var chienDichs = await _chienDichRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung).ToListAsync();
            var aiTools = await _aiToolRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung).ToListAsync();
            var aiBots = await _aiBotRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung).ToListAsync();
            var taiKhoans = await _taiKhoanRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung).ToListAsync();

            var trangThaiDangBaiEnums = EnumHelper.GetEnumInfoList<TrangThaiDangBaiEnum>();

            return new Index_OutPut_Dto
            {
                ThaoTacs = thaoTacs,
                TaiKhoans = taiKhoans,
                NenTangs = nenTangs,
                NguoiTaos = nguoiTaos,
                ChienDichs = chienDichs,
                AIBots = aiBots,
                AITools = aiTools,
                TrangThaiDangBais = trangThaiDangBaiEnums,
            };
        }
        public async Task<FormAddBaiDangDto> AddBanGhi_Modal_CRUD_Output(AddBanGhi_Modal_CRUD_Input_Dto input)
        {
            var nenTangs = await _nenTangRepo.Query()
                .Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .ToListAsync();
            var chienDichs = await _chienDichRepo.Query()
                .Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .ToListAsync();
            var aiTools = await _aiToolRepo.Query()
                .Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .ToListAsync();
            var aiBots = await _aiBotRepo.Query()
                .Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .ToListAsync();

            var taiKhoansRaw = await _taiKhoanRepo.Query()
                .Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .ToListAsync();

            var taiKhoans = taiKhoansRaw
                .Join(nenTangs,
                    tk => tk.IdNenTang,
                    nt => nt.IdNenTang,
                    (tk, nt) => new tbTaiKhoan
                    {
                        IdTaiKhoan = tk.IdTaiKhoan,
                        Name = $"{nt.TenNenTang} - {tk.Name}", // lúc này dùng $"" được vì đang là LINQ thường
                        TrangThai = tk.TrangThai,
                        IdNenTang = tk.IdNenTang,
                        MaDonViSuDung = tk.MaDonViSuDung
                    })
                .ToList();

            var output = new AddBanGhi_Modal_CRUD_Output_Dto
            {
                Loai = input.Loai,
            };
            if (input.Loai == "create")
            {
                output.BaiDangs = new List<tbBaiDangExtend>
                {
                    new tbBaiDangExtend()
                };
            }
            else if (input.Loai == "update" || input.Loai == "detail")
            {
                var baiDang = await GetDetail_BaiDangs(idBaiDangs: input.IdBaiDangs);
                // Chỉ lấy những bài đăng có trạng thái (chờ đăng)
                output.BaiDangs = baiDang.ToList();
            }
            else if (input.Loai == "draftToSave")
            {
                var baiDang = await GetDetail_BaiDangs(idBaiDangs: input.IdBaiDangs);
                // Chỉ lấy những bài đăng có trạng thái (nháp)
                output.BaiDangs = baiDang
                    .Where(x => x.BaiDang.TrangThaiDangBai == (int?)TrangThaiDangBai_BaiDang.Draft)
                    .ToList();
            }

            return new FormAddBaiDangDto
            {
                Data = output,
                ChienDichs = chienDichs,
                TaiKhoans = taiKhoans,
                NenTangs = nenTangs,
                AIBots = aiBots,
                AITools = aiTools,
            };
        }
        public async Task<XemChiTiet_Output_Dto> XemChiTiet_BaiDang(Guid input)
        {
            var data = await AddBanGhi_Modal_CRUD_Output(input: new AddBanGhi_Modal_CRUD_Input_Dto
            {
                IdBaiDangs = new List<Guid> { input },
                Loai = "detail"
            });
            if (data.Data.BaiDangs == null || !data.Data.BaiDangs.Any())
                throw new Exception("Không tìm thấy bài đăng.");
            var baiDang = data.Data.BaiDangs.FirstOrDefault();
            if (baiDang.BaiDang.TrangThaiDangBai == (int)TrangThaiDangBai_BaiDang.Success)
            {
                #region không dùng
                //// Gọi webhook phân tích cmt
                ////var json = await _http.GetStringAsync($"https://postpilot1.app.n8n.cloud/webhook/test-webhook-json?post_id={baiDang.BaiDang.IdBaiDang_TrenNenTang}");
                //var json = await _http.GetStringAsync($"https://postpilot1.app.n8n.cloud/webhook/test-webhook-json?post_id={baiDang.BaiDang.IdBaiDang_TrenNenTang}");
                //if (string.IsNullOrWhiteSpace(json))
                //    throw new Exception("Không có dữ liệu phân tích từ webhook.");
                //var data = JsonConvert.DeserializeObject<PhanTichBinhLuan_Dto>(json); // tạm dùng chung DTO Facebook cho tất cả
                //return new XemChiTiet_Output_Dto
                //{
                //    BaiDang = baiDang,
                //    PhanTichBinhLuan = data,
                //};
                #endregion
                var binhLuans = await _socialApiService.GetPostCommentsAsync(input: new SocialApi_Input_Dto
                {
                    PostId = baiDang.BaiDang.IdBaiDang_TrenNenTang,
                    TenNenTang = baiDang.NenTang?.TenNenTang,
                    AccessToken = baiDang.TaiKhoan?.Page_Access_Token,
                });

                return new XemChiTiet_Output_Dto
                {
                    BaiDang = baiDang,
                    BinhLuans = binhLuans,
                    AIBots = data.AIBots,
                    AITools = data.AITools,
                    ChienDichs = data.ChienDichs,
                    NenTangs = data.NenTangs,
                    TaiKhoans = data.TaiKhoans
                };
            }
            return new XemChiTiet_Output_Dto
            {
                BaiDang = baiDang,
                BinhLuans = null, // Không có dữ liệu phân tích nếu chưa đăng thành công
            };
        }
        public async Task<PhanTichBinhLuan_BaiDang_Output_Dto> PhanTichBinhLuan_BaiDang(PhanTichBinhLuan_BaiDang_Input_Dto input)
        {
            var _baiDang = await GetDetail_BaiDangs(idBaiDangs: new List<Guid> { input.IdBaiDang });
            // Chỉ lấy những bài đăng có trạng thái (chờ đăng)
            var baiDang = _baiDang.FirstOrDefault();
            if (baiDang.BaiDang.TrangThaiDangBai == (int)TrangThaiDangBai_BaiDang.Success)
            {
                var binhLuans = await _socialApiService.GetPostCommentsAsync(input: new SocialApi_Input_Dto
                {
                    PostId = baiDang.BaiDang.IdBaiDang_TrenNenTang,
                    TenNenTang = baiDang.NenTang?.TenNenTang,
                    AccessToken = baiDang.TaiKhoan?.Page_Access_Token,
                });

                if (binhLuans == null || !binhLuans.Any())
                    throw new Exception("Không có dữ liệu bình luận từ API.");

                // Gọi webhook phân tích cmt
                var cmts = binhLuans.Select(x => x.Message).ToList();
                var prompt = $@"
Bạn là chuyên gia hỗ trợ nhà sáng tạo nội dung tối ưu tương tác trên mạng xã hội.

Hãy phân tích nội dung dưới đây để giúp quản trị viên hiểu bài viết và phản hồi bình luận hiệu quả hơn.

Dữ liệu đầu vào:
- Nội dung bài đăng: {baiDang.BaiDang.NoiDung}
- Danh sách bình luận: {JsonConvert.SerializeObject(cmts)}
- Ghi chú từ quản trị viên (nếu có): {input.Content}

Yêu cầu đầu ra (trả về đúng định dạng JSON sau, không giải thích thêm):

```json
{{
  ""DanhGiaTongQuanBaiDang"": ""string"", // Tóm tắt cảm xúc, chủ đề, khả năng tạo tương tác
  ""BinhLuanDaPhanTichs"": [
    {{
      ""BinhLuanGoc"": ""string"",         // Nội dung bình luận gốc
      ""DanhGiaBinhLuan"": ""string"",     // Phân tích nội dung & cảm xúc (tích cực / tiêu cực / trung lập)
      ""GoiYTraLoi"": ""string""           // Gợi ý trả lời phù hợp, giữ tương tác
    }}
  ]
}}";
                var _rs = await _socialApiService.AnalyzePostAsync(input: new AnalyzePost_Input_Dto
                {
                    IdAITool = input.IdAITool,
                    Prompt = prompt
                });
                var rs = JsonConvert.DeserializeObject<PhanTichBinhLuan_BaiDang_Output_Dto>(_rs);
                return rs;
            };
            throw new Exception("Không có dữ liệu bài đăng.");
        }
        public async Task<List<tbBaiDangExtend>> GetDetail_BaiDangs(
            List<Guid> idBaiDangs = null)
        {
            var query = _baiDangRepo.Query()
                .Where(x =>
                    idBaiDangs.Contains(x.IdBaiDang) &&
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);

            // 1. Lấy danh sách bài đăng đã join các bảng liên quan (chưa gán TepDinhKems)
            var tempResult = await (
                from bd in query
                join nd in _nguoiDungRepo.Query() on bd.IdNguoiTao equals nd.IdNguoiDung into ndGroup
                from nd in ndGroup.DefaultIfEmpty()
                join cd in _chienDichRepo.Query() on bd.IdChienDich equals cd.IdChienDich into cdGroup
                from cd in cdGroup.DefaultIfEmpty()
                join tk in _taiKhoanRepo.Query() on bd.IdTaiKhoan equals tk.IdTaiKhoan into tkGroup
                from tk in tkGroup.DefaultIfEmpty()
                join nt in _nenTangRepo.Query() on tk.IdNenTang equals nt.IdNenTang into ntGroup
                from nt in ntGroup.DefaultIfEmpty()
                select new tbBaiDangExtend
                {
                    BaiDang = bd,
                    NguoiTao = nd,
                    ChienDich = cd,
                    TaiKhoan = tk,
                    NenTang = nt,
                    //TepDinhKems = new List<tbTepDinhKem>() // Tạm để trống
                }
            )
            .OrderByDescending(x => x.BaiDang.ThoiGian)
            .ToListAsync();

            // 2. Lấy IdBaiDang
            var baiDangIds = tempResult.Select(x => x.BaiDang.IdBaiDang).ToList();

            // 3. Truy vấn bảng liên kết + bảng tệp đính kèm
            var tepLienKet = await (
                from lk in _baiDangTepDinhKemRepo.Query()
                join tep in _tepDinhKemRepo.Query() on lk.IdTepDinhKem equals tep.IdTep
                where baiDangIds.Contains(lk.IdBaiDang.Value)
                select new { lk.IdBaiDang, Tep = tep }
            ).ToListAsync();

            // 4. Gán TepDinhKem vào từng BaiDang
            foreach (var item in tempResult)
            {
                item.TepDinhKems = tepLienKet
                    .Where(x => x.IdBaiDang == item.BaiDang.IdBaiDang)
                    .Select(x => x.Tep)
                    .ToList();
            }

            return tempResult;
        }
        public async Task<List<tbBaiDangExtend>> GetBaiDangs(
          string loai = "all",
          List<Guid> idBaiDangs = null,
          LocThongTinDto locThongTin = null)
        {
            var query = _baiDangRepo.Query()
                .Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);

            // Áp dụng lọc trước khi join để tối ưu
            if (locThongTin != null)
            {
                if (!string.IsNullOrWhiteSpace(locThongTin.NoiDung))
                    query = query.Where(x => x.NoiDung.Contains(locThongTin.NoiDung));

                if (locThongTin.IdChienDich.HasValue)
                    query = query.Where(x => x.IdChienDich == locThongTin.IdChienDich.Value);

                if (locThongTin.TrangThaiDangBai.HasValue)
                    query = query.Where(x => x.TrangThaiDangBai == locThongTin.TrangThaiDangBai.Value);

                if (locThongTin.IdNguoiTao.HasValue)
                    query = query.Where(x => x.IdNguoiTao == locThongTin.IdNguoiTao.Value);

                //if (locThongTin.IdNenTang.HasValue)
                //    query = query.Where(x => x.IdNenTang == locThongTin.IdNenTang.Value);

                var ngayTaoRange = DateHelper.ParseThangNam(locThongTin.NgayTao);
                if (ngayTaoRange.Start.HasValue && ngayTaoRange.End.HasValue)
                {
                    query = query.Where(x =>
                        x.NgayTao >= ngayTaoRange.Start.Value &&
                        x.NgayTao <= ngayTaoRange.End.Value);
                }

                var ngayDangRange = DateHelper.ParseThangNam(locThongTin.NgayDangBai);
                if (ngayDangRange.Start.HasValue && ngayDangRange.End.HasValue)
                {
                    query = query.Where(x =>
                        x.ThoiGian.HasValue &&
                        x.ThoiGian.Value >= ngayDangRange.Start.Value &&
                        x.ThoiGian.Value <= ngayDangRange.End.Value);
                }

            }

            if (loai == "single" && idBaiDangs != null && idBaiDangs.Any())
            {
                query = query.Where(x => idBaiDangs.Contains(x.IdBaiDang));
            }

            var data = await (
               from bd in query

               join nd in _nguoiDungRepo.Query() on bd.IdNguoiTao equals nd.IdNguoiDung into ndGroup
               from nd in ndGroup.DefaultIfEmpty()

               join cd in _chienDichRepo.Query() on bd.IdChienDich equals cd.IdChienDich into cdGroup
               from cd in cdGroup.DefaultIfEmpty()

               join tk in _taiKhoanRepo.Query() on bd.IdTaiKhoan equals tk.IdTaiKhoan into tkGroup
               from tk in tkGroup.DefaultIfEmpty()

               join nt in _nenTangRepo.Query() on tk.IdNenTang equals nt.IdNenTang into ntGroup
               from nt in ntGroup.DefaultIfEmpty()


               select new tbBaiDangExtend
               {
                   BaiDang = bd,
                   NguoiTao = nd,
                   ChienDich = cd,
                   NenTang = nt,
                   TaiKhoan = tk,
               }
           )
           .OrderByDescending(x => x.BaiDang.ThoiGian)
           .ToListAsync();

            return data;
        }

        public async Task<FreeImageUploadResponse> UploadToFreeImageHost(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            var _apiKey = await GetDecryptedCredential("FreeImage", "ApiKey");

            using (var ms = new MemoryStream())
            {
                if (file.InputStream.CanSeek)
                    file.InputStream.Position = 0;

                file.InputStream.CopyTo(ms);
                ms.Position = 0;

                using (var httpClient = new HttpClient())
                using (var formData = new MultipartFormDataContent())
                using (var streamContent = new StreamContent(ms))
                {
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                    formData.Add(streamContent, "source", file.FileName);
                    formData.Add(new StringContent("upload"), "action");
                    formData.Add(new StringContent(_apiKey), "key");

                    var response = await httpClient.PostAsync("https://freeimage.host/api/1/upload", formData);
                    var json = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic result = JsonConvert.DeserializeObject<FreeImageUploadResponse>(json);
                        //return JObject.Parse(json);
                        return result;
                    }
                    else
                    {
                        dynamic errorResult = JsonConvert.DeserializeObject<FreeImageUploadResponse>(json);
                        //return JObject.Parse(json);
                        return errorResult; // Hoặc trả về null, hoặc ném exception tùy nhu cầu
                    }
                }
            }
        }
        public async Task<string> GetDecryptedCredential(
            string serviceName,
            string credentialType,
            Guid? userId = null)
        {
            var cred = await _apiCredentialRepo.Query()
                .FirstOrDefaultAsync(x => x.ServiceName == serviceName
                                  && x.CredentialType == credentialType
                                  //&& (userId == null || x.IdNguoiDung == userId)
                                  );

            if (cred == null) throw new Exception("Không tìm thấy dữ liệu!");

            return CryptoHelper.Decrypt(cred.KeyJson);
        }
        public async Task Create_BaiDang(
            string loai,
            List<tbBaiDangExtend> baiDangs,
            HttpPostedFileBase[] files,
            Guid[] rowNumbers)
        {
            if (baiDangs == null || !baiDangs.Any())
                throw new ArgumentException("Danh sách bài đăng không được để trống.");

            var tepDinhKemMappings = new List<(tbBaiDang baiDang, List<tbTepDinhKem> teps)>();

            // 1. Upload ảnh trước khi transaction
            foreach (var baiDang_NEW in baiDangs)
            {
                var tepList = new List<tbTepDinhKem>();

                if (files != null && (baiDang_NEW.BaiDang.TuTaoAnhAI == false))
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (baiDang_NEW.RowNumber == rowNumbers[i])
                        {
                            var file = files[i];
                            if (file == null || file.ContentLength <= 0) continue;

                            var result = await UploadToFreeImageHost(file);
                            if (result == null || result.StatusCode != 200)
                                throw new Exception("Upload ảnh thất bại hoặc không có phản hồi từ server.");

                            var tep = new tbTepDinhKem
                            {
                                IdTep = Guid.NewGuid(),
                                FileName = Path.GetFileNameWithoutExtension(file.FileName),
                                DuongDanTepOnline = result.Image.Url,
                                TrangThai = 1,
                                IdNguoiTao = CurrentUserId,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung
                            };

                            tepList.Add(tep);
                        }
                    }
                }
                var trangThaiDangBai = chuyenTrangThai_BaiDang(
                        loai: loai,
                        trangThaiBanDau: (int)TrangThaiDangBai_BaiDang.WaitToPost);
                var baiDang = new tbBaiDang
                {
                    IdBaiDang = Guid.NewGuid(),
                    IdChienDich = baiDang_NEW.BaiDang.IdChienDich,
                    IdTaiKhoan = baiDang_NEW.BaiDang.IdTaiKhoan,
                    NoiDung = baiDang_NEW.BaiDang.NoiDung,
                    ThoiGian = baiDang_NEW.BaiDang.ThoiGian,
                    TuTaoAnhAI = baiDang_NEW.BaiDang.TuTaoAnhAI,
                    TrangThaiDangBai = trangThaiDangBai,
                    TrangThai = 1,
                    IdNguoiTao = CurrentUserId,
                    NgayTao = DateTime.Now,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung
                };
                tepDinhKemMappings.Add((baiDang, tepList));
            }

            // 2. Transaction: lưu bài đăng, file đính kèm và liên kết
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                foreach (var (baiDang, tepList) in tepDinhKemMappings)
                {
                    await _unitOfWork.InsertAsync<tbBaiDang, Guid>(baiDang);

                    foreach (var tep in tepList)
                    {
                        await _unitOfWork.InsertAsync<tbTepDinhKem, Guid>(tep);

                        var baiDangTep = new tbBaiDangTepDinhKem
                        {
                            IdBaiDangTepDinhKem = Guid.NewGuid(),
                            IdBaiDang = baiDang.IdBaiDang,
                            IdTepDinhKem = tep.IdTep
                        };

                        await _unitOfWork.InsertAsync<tbBaiDangTepDinhKem, Guid>(baiDangTep);
                    }
                }
            });
        }
        public async Task Update_BaiDang(
            string loai,
            List<tbBaiDangExtend> baiDangs,
            HttpPostedFileBase[] files,
            Guid[] rowNumbers)
        {
            if (baiDangs == null || !baiDangs.Any())
                throw new ArgumentException("Danh sách bài đăng không được để trống.");

            var tepDinhKemMappings = new List<(tbBaiDangExtend baiDang, List<tbTepDinhKem> teps)>();
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var baiDangs_ThemMoi = new List<tbBaiDangExtend>();
                /**
                 * B1. Cập nhật đồng thời chọn ra các bản ghi trùng IdBaiDang để thêm mới
                 * B2. Thêm mới các bản ghi và dùng luôn tệp đính kèm của bản ghi trùng IdBaiDang đã tồn tại
                 */

                #region B1
                /**
                    * 1. Tạo list bản ghi đã dùng (nhận biết bản ghi cùng id) [baiDangs_DaSuDung]
                    * 2. Kiểm tra tồn tại trong [baiDangs_DaSuDung]
                    *      !null: Lưu vào baiDangs_ThemMoi - sử dụng TrangThaiDangBai của [baiDangs_DaSuDung]
                    *      null: Cập nhật - lưu [baiDang_NEW] vào [baiDangs_DaSuDung]
                    */

                // 1.
                var baiDangs_DaSuDung = new List<tbBaiDang>();

                foreach (var baiDang_NEW in baiDangs)
                {

                    // 2.
                    var _bd = baiDangs_DaSuDung.FirstOrDefault(x => x.IdBaiDang == baiDang_NEW.BaiDang.IdBaiDang);
                    if (_bd != null)
                    {
                        baiDang_NEW.BaiDang.TrangThaiDangBai = _bd.TrangThaiDangBai; // Sử dụng TrangThaiDangBai của bản ghi đã sử dụng
                        baiDangs_ThemMoi.Add(baiDang_NEW); // Thêm vào danh sách tạo mới
                    }
                    else
                    {
                        var tepList = new List<tbTepDinhKem>();

                        if (files != null && baiDang_NEW.BaiDang.TuTaoAnhAI == false)
                        {
                            for (int i = 0; i < files.Length; i++)
                            {
                                if (baiDang_NEW.RowNumber == rowNumbers[i])
                                {
                                    var file = files[i];
                                    if (file == null || file.ContentLength <= 0) continue;

                                    var result = await UploadToFreeImageHost(file);
                                    if (result == null || result.StatusCode != 200)
                                        throw new Exception("Upload ảnh thất bại hoặc không có phản hồi từ server.");

                                    var tep = new tbTepDinhKem
                                    {
                                        IdTep = Guid.NewGuid(),
                                        FileName = Path.GetFileNameWithoutExtension(file.FileName),
                                        DuongDanTepOnline = result.Image.Url,
                                        TrangThai = 1,
                                        IdNguoiTao = CurrentUserId,
                                        NgayTao = DateTime.Now,
                                        MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung
                                    };

                                    tepList.Add(tep);
                                }
                            }
                        }

                        var baiDang_OLD = await _baiDangRepo.GetByIdAsync(baiDang_NEW.BaiDang.IdBaiDang);
                        if (baiDang_OLD == null)
                            throw new Exception($"Bài đăng với Id {baiDang_NEW.BaiDang.IdBaiDang} không tồn tại.");

                        baiDang_NEW.BaiDang.TrangThaiDangBai = chuyenTrangThai_BaiDang(
                            loai: loai,
                            trangThaiBanDau: (int)baiDang_OLD.TrangThaiDangBai);

                        // 2.1
                        baiDang_OLD.IdChienDich = baiDang_NEW.BaiDang.IdChienDich;
                        baiDang_OLD.IdTaiKhoan = baiDang_NEW.BaiDang.IdTaiKhoan;
                        baiDang_OLD.NoiDung = baiDang_NEW.BaiDang.NoiDung;
                        baiDang_OLD.ThoiGian = baiDang_NEW.BaiDang.ThoiGian;
                        baiDang_OLD.TuTaoAnhAI = baiDang_NEW.BaiDang.TuTaoAnhAI;
                        baiDang_OLD.TrangThaiDangBai = baiDang_NEW.BaiDang.TrangThaiDangBai;

                        baiDang_OLD.TrangThai = 1;
                        baiDang_OLD.IdNguoiSua = CurrentUserId;
                        baiDang_OLD.NgaySua = DateTime.Now;

                        _baiDangRepo.Update(baiDang_OLD);

                        tepDinhKemMappings.Add((baiDang_NEW, tepList));

                        // 2.2
                        baiDangs_DaSuDung.Add(baiDang_NEW.BaiDang);
                    }
                }

                // Cập nhật tệp đính kèm cho các bản ghi [baiDangs_DaSuDung]
                foreach (var (baiDang, tepList) in tepDinhKemMappings)
                {
                    // Xóa têp đính kèm cũ không còn trong danh sách mới
                    var a = await _baiDangTepDinhKemRepo.Query()
                        .ToListAsync();
                    var baiDangTepDinhKems_Delete = a
                        .Where(x => x.IdBaiDang == baiDang.BaiDang.IdBaiDang
                            && !baiDang.TepDinhKems.Any(t => t.IdTep == x.IdTepDinhKem))
                        .ToList();

                    foreach (var baiDangTep in baiDangTepDinhKems_Delete)
                    {
                        var tepToDelete = await _tepDinhKemRepo.GetByIdAsync((Guid)baiDangTep.IdTepDinhKem);
                        if (tepToDelete != null) _tepDinhKemRepo.Delete(tepToDelete);
                        _baiDangTepDinhKemRepo.Delete(baiDangTep);
                    }

                    // Cập nhật hoặc thêm tệp đính kèm mới
                    foreach (var tep in tepList)
                    {
                        await _unitOfWork.InsertAsync<tbTepDinhKem, Guid>(tep);

                        var baiDangTep = new tbBaiDangTepDinhKem
                        {
                            IdBaiDangTepDinhKem = Guid.NewGuid(),
                            IdBaiDang = baiDang.BaiDang.IdBaiDang,
                            IdTepDinhKem = tep.IdTep
                        };

                        await _unitOfWork.InsertAsync<tbBaiDangTepDinhKem, Guid>(baiDangTep);
                    }
                }
                await _unitOfWork.SaveChangesAsync(); // Xác nhận lưu dữ liệu
                #endregion

                #region B2
                foreach (var baiDang_NEW in baiDangs_ThemMoi)
                {
                    // Lấy danh sách mapping của bài cùng id
                    var baiDangTepDinhKems_COPY = await _baiDangTepDinhKemRepo.Query()
                     .Where(x => x.IdBaiDang == baiDang_NEW.BaiDang.IdBaiDang)
                     .ToListAsync();

                    var baiDang = new tbBaiDang
                    {
                        IdBaiDang = Guid.NewGuid(),
                        IdChienDich = baiDang_NEW.BaiDang.IdChienDich,
                        IdTaiKhoan = baiDang_NEW.BaiDang.IdTaiKhoan,
                        NoiDung = baiDang_NEW.BaiDang.NoiDung,
                        ThoiGian = baiDang_NEW.BaiDang.ThoiGian,
                        TuTaoAnhAI = baiDang_NEW.BaiDang.TuTaoAnhAI,
                        TrangThaiDangBai = baiDang_NEW.BaiDang.TrangThaiDangBai,
                        TrangThai = 1,
                        IdNguoiTao = CurrentUserId,
                        NgayTao = DateTime.Now,
                        MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung
                    };
                    await _unitOfWork.InsertAsync<tbBaiDang, Guid>(baiDang);

                    foreach (var baiDangTep_COPY in baiDangTepDinhKems_COPY)
                    {
                        var tep_COPY = await _tepDinhKemRepo.GetByIdAsync(baiDangTep_COPY.IdTepDinhKem.Value);
                        if (tep_COPY != null)
                        {
                            var tep_NEW = new tbTepDinhKem
                            {
                                IdTep = Guid.NewGuid(),
                                FileName = tep_COPY.FileName,
                                DuongDanTepOnline = tep_COPY.DuongDanTepOnline,

                                TrangThai = 1,
                                IdNguoiTao = CurrentUserId,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung
                            };
                            await _unitOfWork.InsertAsync<tbTepDinhKem, Guid>(tep_NEW);

                            var baiDangTep_NEW = new tbBaiDangTepDinhKem
                            {
                                IdBaiDangTepDinhKem = Guid.NewGuid(),
                                IdBaiDang = baiDang.IdBaiDang,
                                IdTepDinhKem = tep_NEW.IdTep,
                            };

                            await _unitOfWork.InsertAsync<tbBaiDangTepDinhKem, Guid>(baiDangTep_NEW);
                        }
                    }
                }
                #endregion
            });
        }
        public async Task _Update_BaiDang(
            string loai,
            List<tbBaiDangExtend> baiDangs,
            HttpPostedFileBase[] files,
            Guid[] rowNumbers)
        {
            if (baiDangs == null || !baiDangs.Any())
                throw new ArgumentException("Danh sách bài đăng không được để trống.");

            var tepDinhKemMappings = new List<(tbBaiDangExtend baiDang, List<tbTepDinhKem> teps)>();
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                foreach (var baiDang_NEW in baiDangs)
                {
                    var tepList = new List<tbTepDinhKem>();

                    if (files != null && baiDang_NEW.BaiDang.TuTaoAnhAI == false)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            if (baiDang_NEW.RowNumber == rowNumbers[i])
                            {
                                var file = files[i];
                                if (file == null || file.ContentLength <= 0) continue;

                                var result = await UploadToFreeImageHost(file);
                                if (result == null || result.StatusCode != 200)
                                    throw new Exception("Upload ảnh thất bại hoặc không có phản hồi từ server.");

                                var tep = new tbTepDinhKem
                                {
                                    IdTep = Guid.NewGuid(),
                                    FileName = Path.GetFileNameWithoutExtension(file.FileName),
                                    DuongDanTepOnline = result.Image.Url,
                                    TrangThai = 1,
                                    IdNguoiTao = CurrentUserId,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung
                                };

                                tepList.Add(tep);
                            }
                        }
                    }
                    // Cùng IdBaiDang nhưng khác nền tảng
                    var baiDang = await _baiDangRepo.GetByIdAsync(baiDang_NEW.BaiDang.IdBaiDang);
                    if (baiDang == null)
                        throw new Exception($"Bài đăng với Id {baiDang_NEW.BaiDang.IdBaiDang} không tồn tại.");

                    baiDang.IdChienDich = baiDang_NEW.BaiDang.IdChienDich;
                    baiDang.IdTaiKhoan = baiDang_NEW.BaiDang.IdTaiKhoan;
                    baiDang.NoiDung = baiDang_NEW.BaiDang.NoiDung;
                    baiDang.ThoiGian = baiDang_NEW.BaiDang.ThoiGian;
                    baiDang.TuTaoAnhAI = baiDang_NEW.BaiDang.TuTaoAnhAI;
                    baiDang.TrangThaiDangBai = loai == "draftToSave"
                        ? (int?)TrangThaiDangBai_BaiDang.WaitToPost
                        : baiDang.TrangThaiDangBai;

                    baiDang.TrangThai = 1;
                    baiDang.IdNguoiSua = CurrentUserId;
                    baiDang.NgaySua = DateTime.Now;

                    _baiDangRepo.Update(baiDang);

                    tepDinhKemMappings.Add((baiDang_NEW, tepList));
                }

                foreach (var (baiDang, tepList) in tepDinhKemMappings)
                {
                    // Xóa têp đính kèm cũ không còn trong danh sách mới
                    var baiDangTepDinhKems_Delete = await _baiDangTepDinhKemRepo.Query()
                        .Where(x => x.IdBaiDang == baiDang.BaiDang.IdBaiDang
                            && !baiDang.TepDinhKems.Any(t => t.IdTep == x.IdTepDinhKem))
                        .ToListAsync();

                    foreach (var baiDangTep in baiDangTepDinhKems_Delete)
                    {
                        var tepToDelete = await _tepDinhKemRepo.GetByIdAsync((Guid)baiDangTep.IdTepDinhKem);
                        if (tepToDelete != null) _tepDinhKemRepo.Delete(tepToDelete);
                        _baiDangTepDinhKemRepo.Delete(baiDangTep);
                    }

                    // Cập nhật hoặc thêm tệp đính kèm mới
                    foreach (var tep in tepList)
                    {
                        await _unitOfWork.InsertAsync<tbTepDinhKem, Guid>(tep);

                        var baiDangTep = new tbBaiDangTepDinhKem
                        {
                            IdBaiDangTepDinhKem = Guid.NewGuid(),
                            IdBaiDang = baiDang.BaiDang.IdBaiDang,
                            IdTepDinhKem = tep.IdTep
                        };

                        await _unitOfWork.InsertAsync<tbBaiDangTepDinhKem, Guid>(baiDangTep);
                    }
                }
            });
        }
        public async Task Delete_BaiDangs(List<Guid> idBaiDangs)
        {
            if (idBaiDangs == null || !idBaiDangs.Any())
                throw new ArgumentException("Danh sách bài đăng không được để trống.");

            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                foreach (var id in idBaiDangs)
                {
                    var baiDang = await _baiDangRepo.GetByIdAsync(id);
                    if (baiDang == null) continue;

                    // Cập nhật trạng thái bài đăng
                    if (baiDang.TrangThaiDangBai == (int?)TrangThaiDangBai_BaiDang.WaitToPost) // Đang chờ đăng thì xóa luôn
                        baiDang.TrangThaiDangBai = (int?)TrangThaiDangBai_BaiDang.Deleted;
                    else baiDang.TrangThaiDangBai = (int?)TrangThaiDangBai_BaiDang.WaitToDelete; // Đã đăng thì chuyển sang trạng thái chờ xóa
                    baiDang.TrangThai = 0;
                    baiDang.IdNguoiSua = CurrentUserId;
                    baiDang.NgaySua = DateTime.Now;
                    _baiDangRepo.Update(baiDang);

                    // Lấy các bản ghi liên kết Tệp - Bài đăng
                    var baiDangTepDinhKems = await _baiDangTepDinhKemRepo.Query()
                        .Where(x => x.IdBaiDang == baiDang.IdBaiDang)
                        .ToListAsync();

                    if (!baiDangTepDinhKems.Any()) continue;

                    var tepIds = baiDangTepDinhKems.Select(x => x.IdTepDinhKem).Distinct().ToList();

                    var tepDinhKems = await _tepDinhKemRepo.Query()
                        .Where(x => tepIds.Contains(x.IdTep))
                        .ToListAsync();

                    foreach (var tep in tepDinhKems)
                    {
                        tep.TrangThai = 0;
                        tep.IdNguoiSua = CurrentUserId;
                        tep.NgaySua = DateTime.Now;
                        _tepDinhKemRepo.Update(tep);

                        // Gợi ý nâng cao: Nếu bạn cần xoá file vật lý trên server, gọi FileService tại đây
                        // await _fileService.DeleteFileIfExistsAsync(tep.DuongDanTepVatLy);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
            });
        }

        private int? chuyenTrangThai_BaiDang(string loai, int? trangThaiBanDau)
        {
            trangThaiBanDau = trangThaiBanDau ?? (int)TrangThaiDangBai_BaiDang.WaitToPost;
            var mapping = new Dictionary<string, int>
            {
                { "create", (int)TrangThaiDangBai_BaiDang.WaitToPost },
                { "update", trangThaiBanDau.Value },
                { "draft", (int)TrangThaiDangBai_BaiDang.Draft },
                { "draftToSave", (int)TrangThaiDangBai_BaiDang.WaitToPost }
            };

            if (mapping.TryGetValue(loai, out var trangThai))
            {
                return (int)trangThai;
            }

            return trangThaiBanDau; // hoặc ném ngoại lệ nếu muốn kiểm soát lỗi kỹ hơn
        }

    }
}