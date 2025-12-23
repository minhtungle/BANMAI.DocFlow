'use strict'
/**
 * main
 * */
class QuanLyTaiLieu {
    constructor() {
        this.page;
        this.pageGroup;
        this.taiLieu = {}
    }
    init() {
        var quanLyTaiLieu = this;
        quanLyTaiLieu.page = $("#page-quanlytailieu");
        htmlEl = new HtmlElement();

        quanLyTaiLieu.taiLieu = {
            ...quanLyTaiLieu.taiLieu,
            dataTable: null,
            getList: function () {
                var $timKiem = $("#tailieu-timkiem-collapse");
                var input = {
                    LocThongTin: {
                        IdNhaCungCap: $("#select-nhacungcap", $timKiem).val(),
                        TenTaiLieu: $("#input-tentailieu", $timKiem).val().trim(),
                    }
                };
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyTaiLieu/getList_TaiLieu",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: { input }
                    }),
                    success: function (res) {
                        $("#tailieu-getList-container").html(res);
                        quanLyTaiLieu.taiLieu.dataTable = new DataTableCustom({
                            name: "tailieu-getList",
                            table: $("#tailieu-getList"),
                            props: {
                                dom: `
                                <'row'<'col-sm-12'rt>>
                                <'row'<'col-sm-12 col-md-6 text-left'i><'col-sm-12 col-md-6 pt-2'p>>`,
                                lengthMenu: [
                                    [5, 10],
                                    [5, 10],
                                ],
                            }
                        }).dataTable;
                    }
                });
            },
            ...quanLyNhaCungCap.taiLieu,

            maxDungLuongTep: 1024 * 1024 * 200, // 200MB,
            maxSoLuongTaiLieu: 200,
            arrTaiLieu: [],
            add: function (e, rowNumber = '00000000-0000-0000-0000-000000000000') {
                var $modal = $("#baidang-crud");

                var $imgContainer = $(`.baidang-read[row='${rowNumber}'] #anhmota-items`, $modal),
                    soAnhDangCo = $(".image-item", $imgContainer).length;

                var addTr = function (files) {
                    let kiemTra = true,
                        mess = "Thêm ảnh thành công";

                    let arrTaiLieu = [];

                    $.each(files, function (idx, f) {
                        // Kiểm tra tệp
                        if (!(/\.(pdf)$/i.test(f.name))) {
                            mess = `Tồn tại tệp không thuộc định dạng cho phép [pdf]`;
                            kiemTra = false;
                            return false;
                        };
                        // Kiểm tra dung lượng
                        if (f.size > quanLyBaiDang.baiDang.handleAnhMoTa.maxDungLuongTep) {
                            mess = `Tồn tại tệp có kích thước tệp vượt quá giới hạn ${quanLyBaiDang.baiDang.handleAnhMoTa.maxDungLuongTep} Mb`;
                            kiemTra = false;
                            return false;
                        };
                        // Kiểm tra tên
                        if (f.name.length > 80) {
                            mess = `Tồn tại tệp có tên vượt quá giới hạn 80 ký tự`;
                            kiemTra = false;
                            return false;
                        };

                        // Thêm ảnh vào mảng
                        let idTamThoi = sys.generateGUID();
                        var tempImageUrl = URL.createObjectURL(f);

                        var data = {
                            rowNumber,
                            idTamThoi,
                            file: f,
                            html: `
                                    <div class="image-item" data-id="00000000-0000-0000-0000-000000000000" data-idtamthoi="${idTamThoi}">
                                        <img src="${tempImageUrl}" alt="${f.name}" />
                                        <button class="delete-btn"
                                            onclick="quanLyBaiDang.baiDang.handleAnhMoTa.delete('${loaiAnh}', this, '${rowNumber}')">
                                            &times;
                                        </button>
                                    </div>
                                `
                        };

                        quanLyBaiDang.baiDang.handleAnhMoTa.arrTaiLieu.push(data);
                        arrTaiLieu.push(data);
                    });

                    if (!kiemTra) {
                        sys.alert({
                            status: "error",
                            mess,
                            timeout: 5000
                        });
                    } else {
                        $.each(arrTaiLieu, function (idx, anh) {
                            //formData.append("files", anh.file); // Dùng khi save()
                            $imgContainer.prepend(anh.html);
                        });

                        sys.alert({
                            status: "success",
                            mess: "Đã thêm ảnh thành công",
                            timeout: 5000
                        });
                    };
                };

                var $fileInput = null;

                $fileInput = $(`.baidang-read[row='${rowNumber}'] #image-anhmota-${rowNumber}`, $modal).get(0);
                if (soAnhDangCo >= quanLyBaiDang.baiDang.handleAnhMoTa.maxSoLuongTaiLieu) {
                    sys.alert({
                        status: "warning",
                        mess: `Chỉ cho phép tối đa ${quanLyBaiDang.baiDang.handleAnhMoTa.maxSoLuongTaiLieu} ảnh`,
                        timeout: 5000
                    });
                } else {
                    // Chỉ lấy đủ số ảnh quy định
                    let files = Array.from($fileInput.files)
                        .slice(0, (quanLyBaiDang.baiDang.handleAnhMoTa.maxSoLuongTaiLieu - soAnhDangCo));
                    addTr(files);
                };

                $fileInput.value = ''; // xóa giá trị của input file
            },
            displayModal_CreateByFile: function (loai = "", idTaiLieu = '00000000-0000-0000-0000-000000000000') {
                var idTaiLieus = [];
                if (loai == "create") idTaiLieus.push(idTaiLieu);
                else {
                    if (idTaiLieu != '00000000-0000-0000-0000-000000000000')
                        idTaiLieus.push(idTaiLieu);
                    else {
                        quanLyTaiLieu.taiLieu.dataTable.rows().iterator('row', function (context, index) {
                            var $row = $(this.row(index).node());
                            if ($row.has("input.checkRow-tailieu-getList:checked").length > 0) {
                                idTaiLieus.push($row.attr('id'));
                            };
                        });
                        if (idTaiLieus.length != 1) {
                            sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                            return;
                        }
                    }
                };
                var input = {
                    Loai: loai,
                    IdTaiLieu: idTaiLieus[0],
                };

                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyTaiLieu/displayModal_CRUD_TaiLieu",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: { input },
                    }),
                    success: function (res) {
                        $("#tailieu-crud").html(res);
                        sys.displayModal({
                            name: '#tailieu-crud'
                        });
                    }
                })
            },

            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#tailieu-crud");
                if (modalValidtion) {
                    let $modal = $("#tailieu-crud");
                    var taiLieu = {
                        TaiLieu: {
                            IdTaiLieu: $("#input-idtailieu", $modal).val(),
                            TenTaiLieu: $("#input-tentailieu", $modal).val().trim(),
                            Email: $("#input-email", $modal).val().trim(),
                            SoDienThoai: $("#input-sodienthoai", $modal).val().trim(),
                            DiaChi: $("#input-diachi", $modal).val().trim(),
                            GhiChu: $("#input-ghichu", $modal).val().trim(),
                        }
                    }
                    sys.confirmDialog({
                        mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                        callback: function () {
                            var formData = new FormData();
                            formData.append("taiLieu", JSON.stringify(taiLieu));
                            formData.append("loai", loai);

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: loai == "create" ? "/QuanLyTaiLieu/create_TaiLieu" : "/QuanLyTaiLieu/update_TaiLieu",
                                    type: "POST",
                                    data: formData,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.status == "success") {
                                        quanLyTaiLieu.taiLieu.getList();

                                        sys.displayModal({
                                            name: '#tailieu-crud',
                                            displayStatus: "hide"
                                        });
                                        sys.alert({ status: res.status, mess: res.mess });
                                    } else {
                                        if (res.status == "warning") {
                                            htmlEl.inputValidationStates(
                                                $("#input-tentailieu"),
                                                "#tailieu-crud",
                                                res.mess,
                                                {
                                                    status: true,
                                                    isvalid: false
                                                }
                                            )
                                        };
                                        sys.alert({ status: res.status, mess: res.mess });
                                    };

                                }
                            });
                        }
                    });
                };
            },
            delete: function (loai, idTaiLieu = '00000000-0000-0000-0000-000000000000') {
                var idTaiLieus = [];
                if (loai == "single") {
                    idTaiLieus.push(idTaiLieu)
                } else {
                    quanLyTaiLieu.taiLieu.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-tailieu-getList:checked").length > 0) {
                            idTaiLieus.push($row.attr('id'));
                        };
                    });
                };
                if (idTaiLieus.length > 0) {
                    var f = new FormData();
                    f.append("idTaiLieus", JSON.stringify(idTaiLieus));
                    sys.confirmDialog({
                        mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyTaiLieu/delete_TaiLieus",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    sys.alert({ status: res.status, mess: res.mess })
                                    quanLyTaiLieu.taiLieu.getList();
                                }
                            })
                        }
                    })
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                }
            },

            update: function () {
                var modalValidtion = htmlEl.activeValidationStates("#tab-thongtincoban");
                if (modalValidtion) {
                    let $tab = $("#tab-thongtincoban");
                    var taiLieu = {
                        TaiLieu: {
                            IdTaiLieu: $("#input-idtailieu", $tab).val(),
                            TenTaiLieu: $("#input-tentailieu", $tab).val().trim(),
                            SoDienThoai: $("#input-sodienthoai", $tab).val().trim(),
                            Email: $("#input-email", $tab).val().trim(),
                            DiaChi: $("#input-diachi", $tab).val().trim(),
                            GhiChu: $("#input-ghichu", $tab).val().trim(),
                        }
                    }
                    sys.confirmDialog({
                        mess: `<p>Bạn có thực sự muốn cập nhật bản ghi này hay không ?</p>`,
                        callback: function () {
                            var formData = new FormData();
                            formData.append("taiLieu", JSON.stringify(taiLieu));

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyTaiLieu/update_TaiLieu",
                                    type: "POST",
                                    data: formData,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.status == "success") {
                                        sys.alert({ status: res.status, mess: res.mess });
                                        setTimeout(() => {
                                            quanLyTaiLieu.taiLieu.xemChiTiet.showTab();
                                        }, 1000);
                                    } else {
                                        if (res.status == "warning") {
                                            htmlEl.inputValidationStates(
                                                $("#input-tentailieu"),
                                                "#tab-thongtincoban",
                                                res.mess,
                                                {
                                                    status: true,
                                                    isvalid: false
                                                }
                                            )
                                        };
                                        sys.alert({ status: res.status, mess: res.mess });
                                    };

                                }
                            });
                        }
                    });
                };
            },

            xemChiTiet: {

            }
        };
    }
};