'use strict'
/**
 * main
 * */
class QuanLyNhaCungCap {
    constructor() {
        this.page;
        this.pageGroup;
        this.nhaCungCap = {}
    }
    init() {
        var quanLyNhaCungCap = this;
        quanLyNhaCungCap.page = $("#page-quanlynhacungcap");
        htmlEl = new HtmlElement();

        quanLyNhaCungCap.nhaCungCap = {
            ...quanLyNhaCungCap.nhaCungCap,
            dataTable: null,
            handleTaiLieu: {
                maxDungLuongTaiLieu: 1024 * 1024 * 300, // 300MB,
                maxSoLuongTaiLieu: 200,
                arrTaiLieu: [],
                add: function (e, rowNumber = '00000000-0000-0000-0000-000000000000') {
                    var $modal = $("#nhacungcap-crud");

                    var $imgContainer = $(`.nhacungcap-read[row='${rowNumber}'] #tailieu-items`, $modal),
                        soAnhDangCo = $(".image-item", $imgContainer).length;

                    var addTr = function (files) {
                        let kiemTra = true,
                            mess = "Thêm tệp thành công";

                        let arrTaiLieu = [];

                        $.each(files, function (idx, f) {
                            // Kiểm tra tệp
                            if (!(/\.(.pdf)$/i.test(f.name))) {
                                mess = `Tồn tại tệp không thuộc định dạng cho phép [.pdf]`;
                                kiemTra = false;
                                return false;
                            };
                            // Kiểm tra dung lượng
                            if (f.size > quanLyNhaCungCap.nhaCungCap.handleTaiLieu.maxDungLuongTaiLieu) {
                                mess = `Tồn tại tệp có kích thước tệp vượt quá giới hạn ${quanLyNhaCungCap.nhaCungCap.handleTaiLieu.maxDungLuongTaiLieu} Mb`;
                                kiemTra = false;
                                return false;
                            };
                            // Kiểm tra tên
                            if (f.name.length > 80) {
                                mess = `Tồn tại tệp có tên vượt quá giới hạn 80 ký tự`;
                                kiemTra = false;
                                return false;
                            };

                            // Thêm tệp vào mảng
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
                                            onclick="quanLyNhaCungCap.nhaCungCap.handleTaiLieu.delete('${loaiAnh}', this, '${rowNumber}')">
                                            &times;
                                        </button>
                                    </div>
                                `
                            };

                            quanLyNhaCungCap.nhaCungCap.handleTaiLieu.arrTaiLieu.push(data);
                            arrTaiLieu.push(data);
                        });

                        if (!kiemTra) {
                            sys.alert({
                                status: "error",
                                mess,
                                timeout: 5000
                            });
                        } else {
                            $.each(arrTaiLieu, function (idx, tl) {
                                //formData.append("files", anh.file); // Dùng khi save()
                                $imgContainer.prepend(tl.html);
                            });

                            sys.alert({
                                status: "success",
                                mess: "Đã thêm tệp thành công",
                                timeout: 5000
                            });
                        };
                    };

                    var $fileInput = null;

                    $fileInput = $(`.nhacungcap-read[row='${rowNumber}'] #image-tailieu-${rowNumber}`, $modal).get(0);
                    if (soAnhDangCo >= quanLyNhaCungCap.nhaCungCap.handleTaiLieu.maxSoLuongTaiLieu) {
                        sys.alert({
                            status: "warning",
                            mess: `Chỉ cho phép tối đa ${quanLyNhaCungCap.nhaCungCap.handleTaiLieu.maxSoLuongTaiLieu} tệp`,
                            timeout: 5000
                        });
                    } else {
                        // Chỉ lấy đủ số tệp quy định
                        let files = Array.from($fileInput.files)
                            .slice(0, (quanLyNhaCungCap.nhaCungCap.handleTaiLieu.maxSoLuongTaiLieu - soAnhDangCo));
                        addTr(files);
                    };
                    $fileInput.value = ''; // xóa giá trị của input file
                },
                delete: function (e, rowNumber) {
                    var $item = $(e).closest(".image-item"),
                        id = $item.attr("data-id"),
                        idTamThoi = $item.attr("data-idtamthoi");

                    // Lấy thẻ <img> để lấy URL tạm
                    var img = $item.find("img")[0];
                    if (img && img.src.startsWith("blob:")) {
                        URL.revokeObjectURL(img.src); // Giải phóng bộ nhớ URL tạm 
                    }

                    // Xóa tệp trên giao diện
                    $item.remove();

                    // Xóa tệp trong mảng tạm
                    quanLyNhaCungCap.nhaCungCap.handleTaiLieu.arrTaiLieu = quanLyNhaCungCap.nhaCungCap.handleTaiLieu.arrTaiLieu
                        .filter(function (tl) {
                            return tl.idTamThoi != idTamThoi;
                        });
                }
            },
            getList: function () {
                var $timKiem = $("#nhacungcap-timkiem-collapse");
                var input = {
                    LocThongTin: {
                        //ThoiGianBatDau: $("#input-thoigianbatdau", $timKiem).val().trim(),
                        //ThoiGianKetThuc: $("#input-thoigianketthuc", $timKiem).val().trim(),

                        //IdNhaCungCaps: [
                        //    $("#select-idnhacungcap", $timKiem).val(),
                        //],
                        Stt: $("#input-stt", $timKiem).val(),
                        TenNhaCungCap: $("#input-tennhacungcap", $timKiem).val().trim(),
                        TenMatHang: $("#input-tenmathang", $timKiem).val().trim(),
                        SoDienThoai: $("#input-sodienthoai", $timKiem).val().trim(),
                        Email: $("#input-email", $timKiem).val().trim(),
                        DiaChi: $("#input-diachi", $timKiem).val().trim(),
                        GhiChu: $("#input-ghichu", $timKiem).val().trim(),
                    }
                };
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyNhaCungCap/getList_NhaCungCap",
                        type: "POST", // Phải là POST để gửi JSON
                        contentType: "application/json; charset=utf-8",  // Định dạng JSON
                        data: { input }
                        //dataType: "json",
                    }),
                    //contentType: false,
                    //processData: false,
                    success: function (res) {
                        $("#nhacungcap-getList-container").html(res);
                        quanLyNhaCungCap.nhaCungCap.dataTable = new DataTableCustom({
                            name: "nhacungcap-getList",
                            table: $("#nhacungcap-getList"),
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
            displayModal_CRUD: function (loai = "", idNhaCungCap = '00000000-0000-0000-0000-000000000000') {
                var idNhaCungCaps = [];
                if (loai == "create") idNhaCungCaps.push(idNhaCungCap);
                else {
                    if (idNhaCungCap != '00000000-0000-0000-0000-000000000000')
                        idNhaCungCaps.push(idNhaCungCap);
                    else {
                        quanLyNhaCungCap.nhaCungCap.dataTable.rows().iterator('row', function (context, index) {
                            var $row = $(this.row(index).node());
                            if ($row.has("input.checkRow-nhacungcap-getList:checked").length > 0) {
                                idNhaCungCaps.push($row.attr('id'));
                            };
                        });
                        if (idNhaCungCaps.length != 1) {
                            sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                            return;
                        }
                    }
                };
                var input = {
                    Loai: loai,
                    IdNhaCungCap: idNhaCungCaps[0],
                };

                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyNhaCungCap/displayModal_CRUD_NhaCungCap",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: {
                            input
                        },
                    }),
                    success: function (res) {
                        $("#nhacungcap-crud").html(res.html);

                        quanLyNhaCungCap.createModalCRUD_NhaCungCap();
                        quanLyNhaCungCap.handleModal_CRUD.addBanGhi(res.output);
                    }
                })
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#nhacungcap-crud");
                if (modalValidtion) {
                    let $modal = $("#nhacungcap-crud");
                    var truongHocs = $("#select-truonghoc", $modal).val();
                    var nhaCungCap = {
                        NhaCungCap: {
                            IdNhaCungCap: $("#input-idnhacungcap", $modal).val(),
                            IdNhaCungCapCha: $("#select-nhacungcapcha", $modal).val(),
                            //MaNhaCungCap: $("#input-manhacungcap", $modal).val().trim(),
                            TenNhaCungCap: $("#input-tennhacungcap", $modal).val().trim(),
                            TenMatHang: $("#input-tenmathang", $modal).val().trim(),
                            SoDienThoai: $("#input-sodienthoai", $modal).val().trim(),
                            Email: $("#input-email", $modal).val().trim(),
                            DiaChi: $("#input-diachi", $modal).val().trim(),
                            GhiChu: $("#input-ghichu", $modal).val().trim(),
                        },
                        TruongHocs: truongHocs?.map(x => ({
                            IdTruongHoc: x
                        })),
                    }
                    sys.confirmDialog({
                        mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                        callback: function () {
                            var formData = new FormData();
                            formData.append("nhaCungCap", JSON.stringify(nhaCungCap));
                            formData.append("loai", loai);

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: loai == "create" ? "/QuanLyNhaCungCap/create_NhaCungCap" : "/QuanLyNhaCungCap/update_NhaCungCap",
                                    type: "POST",
                                    data: formData,
                                }),
                                //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.status == "success") {
                                        quanLyNhaCungCap.nhaCungCap.getList();

                                        sys.displayModal({
                                            name: '#nhacungcap-crud',
                                            displayStatus: "hide"
                                        });
                                        sys.alert({ status: res.status, mess: res.mess });
                                    } else {
                                        if (res.status == "warning") {
                                            htmlEl.inputValidationStates(
                                                $("#input-tennhacungcap"),
                                                "#nhacungcap-crud",
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
            delete: function (loai, idNhaCungCap = '00000000-0000-0000-0000-000000000000') {
                var idNhaCungCaps = [];
                // Lấy id
                if (loai == "single") {
                    idNhaCungCaps.push(idNhaCungCap)
                } else {
                    quanLyNhaCungCap.nhaCungCap.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-nhacungcap-getList:checked").length > 0) {
                            idNhaCungCaps.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra id
                if (idNhaCungCaps.length > 0) {
                    var f = new FormData();
                    f.append("idNhaCungCaps", JSON.stringify(idNhaCungCaps));
                    sys.confirmDialog({
                        mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyNhaCungCap/delete_NhaCungCaps",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    sys.alert({ status: res.status, mess: res.mess })
                                    quanLyNhaCungCap.nhaCungCap.getList();
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
                    var nguoiThans = [];
                    var nhaCungCap = {
                        NhaCungCap: {
                            IdNhaCungCap: $("#input-idnhacungcap", $tab).val(),
                            //MaNhaCungCap: $("#input-manhacungcap", $tab).val().trim(),
                            GioiTinh: $("#select-gioitinh", $tab).val() == 1 ? true : false,
                            TenNhaCungCap: $("#input-tennhacungcap", $tab).val().trim(),
                            SoDienThoai: $("#input-sodienthoai", $tab).val().trim(),
                            Email: $("#input-email", $tab).val().trim(),
                            TenMatHang: $("#input-tenmathang", $tab).val().trim(),
                            NgheNghiep: $("#input-nghenghiep", $tab).val().trim(),
                            DiaChi: $("#input-diachi", $tab).val().trim(),
                            TienSuBenh: $("#input-tiensubenh", $tab).val().trim(),
                            GhiChu: $("#input-ghichu", $tab).val().trim(),
                        },
                        NguoiThans: nguoiThans
                    }
                    sys.confirmDialog({
                        mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                        callback: function () {
                            var formData = new FormData();
                            formData.append("nhaCungCap", JSON.stringify(nhaCungCap));

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyNhaCungCap/update_NhaCungCap",
                                    type: "POST",
                                    data: formData,
                                }),
                                //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.status == "success") {
                                        sys.alert({ status: res.status, mess: res.mess });
                                        setTimeout(() => {
                                            quanLyNhaCungCap.nhaCungCap.xemChiTiet.showTab();
                                        }, 1000);
                                    } else {
                                        if (res.status == "warning") {
                                            htmlEl.inputValidationStates(
                                                $("#input-tenmathang"),
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
    createModalCRUD_NhaCungCap() {
        var quanLyNhaCungCap = this;

        quanLyNhaCungCap.handleModal_CRUD = {
            dataTable: new DataTableCustom({
                name: "nhacungcap-getList",
                table: $("#nhacungcap-getList", $("#nhacungcap-crud")),
                props: {
                    dom: `
                    <'row'<'col-sm-12'rt>>
                    <'row'<'col-sm-12 col-md-6 text-left'i><'col-sm-12 col-md-6 pt-2'p>>`,
                    lengthMenu: [
                        [-1],
                        ['Tất cả'],
                    ],
                }
            }).dataTable,
            displayModal_UpdateMultipleCell: function () {
                var rows = quanLyNhaCungCap.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-nhacungcap-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    sys.displayModal({
                        name: '#nhacungcap-crud-capnhattruong',
                        level: 2
                    });
                };
            },
            updateSingleCell: function (el) {
                var rows = quanLyNhaCungCap.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $div = $(el).closest(".nhacungcap-read"),
                    rowNumber = $div.attr("row"),
                    val = $(el).val();

                var tenTruongDuLieu = $(el).attr("id").split("-")[1];
                $.each(rows, function () {
                    if ($(this).attr("row") == rowNumber) {
                        let $span = $(`span[data-tentruong="${tenTruongDuLieu}"]`, $(this));
                        $span.text(sys.truncateString(val, 12));
                        $span.closest("td").attr("title", val);
                    }
                });
            },
            updateMultipleCell: function () {
                var $modal = $("#nhacungcap-crud"),
                    endName = 'capnhattruong';

                var $modal_CapNhatTruong = $(`#nhacungcap-crud-${endName}`);

                var idTaiKhoan = $(`#select-taikhoan-${endName}`, $modal_CapNhatTruong).val(),
                    idAITool = $(`#select-aitool-${endName}`, $modal_CapNhatTruong).val(),
                    idAIBot = $(`#select-aibot-${endName}`, $modal_CapNhatTruong).val(),
                    idChienDich = $(`#select-chiendich-${endName}`, $modal_CapNhatTruong).val();

                var isCheck_TaiKhoan = $(`#checkbox-taikhoan-${endName}`, $modal_CapNhatTruong).is(":checked"),
                    isCheck_AIBot = $(`#checkbox-aibot-${endName}`, $modal_CapNhatTruong).is(":checked"),
                    isCheck_AITool = $(`#checkbox-aitool-${endName}`, $modal_CapNhatTruong).is(":checked"),
                    isCheck_ChienDich = $(`#checkbox-chiendich-${endName}`, $modal_CapNhatTruong).is(":checked");

                var rows = quanLyNhaCungCap.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-nhacungcap-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.nhacungcap-read[row=${rowNumber}]`, $modal);
                        // Thay đổi value cho những dòng được chọn
                        if (isCheck_TaiKhoan) {
                            $(`#select-taikhoan-${rowNumber}`, $div).val(idTaiKhoan);
                            $(`#select-taikhoan-${rowNumber}`, $div).trigger("change");
                        };
                        if (isCheck_AIBot) {
                            $(`#select-aibot`, $div).val(idAIBot);
                            $(`#select-aibot`, $div).trigger("change");
                        };
                        if (isCheck_AITool) {
                            $(`#select-aitool`, $div).val(idAITool);
                            $(`#select-aitool`, $div).trigger("change");
                        };
                        if (isCheck_ChienDich) {
                            $(`#select-chiendich`, $div).val(idChienDich);
                            $(`#select-chiendich`, $div).trigger("change");
                        };
                    });

                    sys.alert({ status: "success", mess: "Cập nhật trường dữ liệu thành công" })
                    sys.displayModal({
                        name: '#nhacungcap-crud-capnhattruong',
                        displayStatus: "hide",
                        level: 2,
                    });
                };
            },
            addBanGhi: function (input) {
                // Tạo mã guid cho bản ghi
                //var guid = sys.generateGUID();
                //#region Thêm bản ghi
                var $modal = $("#nhacungcap-crud");
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyNhaCungCap/addBanGhi_Modal_CRUD",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: { input: input },
                    }),
                    success: function (res) {
                        if (!res.status) {
                            sys.alert({
                                mess: `Không có bản ghi phù hợp trạng thái ${(res.Loai == "create"
                                    ? "[Thêm mới]"
                                    : res.Loai == "update"
                                        ? "[Cập nhật]"
                                        : "[Chuyển đăng bài]")
                                    }`,
                                status: "warning", timeout: 1500
                            })
                        }
                        else {
                            quanLyNhaCungCap.handleModal_CRUD.dataTable.destroy();
                            // Tạo bản ghi mới
                            $("#nhacungcap-getList tbody", $modal).prepend(res.html_nhacungcap_row);
                            $("#nhacungcap-read-container", $modal).prepend(res.html_nhacungcap_read);
                            // Tạo dataTable
                            //if (!quanLyNhaCungCap.handleModal_CRUD.dataTable) {
                            quanLyNhaCungCap.handleModal_CRUD.dataTable = new DataTableCustom({
                                name: "nhacungcap-getList",
                                table: $("#nhacungcap-getList", $modal),
                                props: {
                                    dom: `
                                <'row'<'col-sm-12'rt>>
                                <'row'<'col-sm-12 col-md-6 text-left'i><'col-sm-12 col-md-6 pt-2'p>>`,
                                    lengthMenu: [
                                        [-1],
                                        ['Tất cả'],
                                    ],
                                }
                            }).dataTable;
                            //    // Nếu đã khởi tạo, chỉ cần thêm dòng mới (đã prepend bên trên rồi) và vẽ lại bảng
                            //}
                            //quanLyNhaCungCap.handleModal_CRUD.dataTable.row($(res.html_nhacungcap_row)).invalidate().draw(false);
                            // Chọn bản ghi đó
                            var rows_NEW = quanLyNhaCungCap.handleModal_CRUD.dataTable.rows().nodes().toArray(); // Chọn phần thử đầu tiên của bảng
                            quanLyNhaCungCap.handleModal_CRUD.readRow($(rows_NEW[0]));
                            $.each($(".nhacungcap-read", $modal), function () {
                                var $div = $(this),
                                    rowNumber = $div.attr("row");
                                // Gán validation
                                htmlEl.validationStates($div);
                                htmlEl.inputMask();
                                var modalValidtion = htmlEl.activeValidationStates($div);
                            });

                            /**
                              * Gán các thuộc tính
                              */
                            sys.displayModal({
                                name: '#nhacungcap-crud'
                            });

                            setTimeout(function () {
                                var containerHeight = $("#nhacungcap-crud .modal-body").height() - 10;
                                $("#nhacungcap-read-container", $("#nhacungcap-crud")).height(containerHeight);
                            }, 500)
                        }
                    }
                })
                //#endregion
            },
            deleteBanGhi: function () {
                var $modal = $("#nhacungcap-crud");
                var rows = quanLyNhaCungCap.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-nhacungcap-getList:checked`, rows);

                if ($rowChecks.length === 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" });
                    return;
                }

                // Nếu số bản ghi được chọn >= tổng số dòng hiện có, ctệp báo và không xóa
                if ($rowChecks.length >= rows.length) {
                    sys.alert({ status: "warning", mess: "Phải giữ lại ít nhất một bản ghi!" });
                    return;
                }

                // Tiến hành xóa các bản ghi được chọn
                $.each($rowChecks, function () {
                    var $rowCheck = $(this).closest("tr"),
                        rowNumber = $rowCheck.attr("row"),
                        $div = $(`.nhacungcap-read[row=${rowNumber}]`, $modal);
                    // Xóa bản ghi trong div
                    quanLyNhaCungCap.handleModal_CRUD.dataTable.row($rowCheck).remove().draw();
                    $div.remove();
                    // Xóa tệp trong mảng
                    quanLyNhaCungCap.nhaCungCap.handleTaiLieu.arrTaiLieu = quanLyNhaCungCap.nhaCungCap.handleTaiLieu.arrTaiLieu
                        .filter(function (anh) {
                            return anh.rowNumber != rowNumber;
                        });
                });

                // Lấy lại các dòng mới sau khi xóa
                var rows_NEW = quanLyNhaCungCap.handleModal_CRUD.dataTable.rows().nodes().toArray();

                if (rows_NEW.length > 0) {
                    quanLyNhaCungCap.handleModal_CRUD.readRow($(rows_NEW[0]));
                } else {
                    $modal.find(".nhacungcap-read").empty();
                }
            },
            readRow: function (el) {
                var $modal = $("#nhacungcap-crud");
                var rowNumber = $(el).attr("row"),
                    rows = quanLyNhaCungCap.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $divs = $(".nhacungcap-read", $modal),
                    $div = $(`.nhacungcap-read[row=${rowNumber}]`, $modal);

                $divs.hide(); $div.show();
                $.each(rows, function () {
                    if ($(this).attr("row") == rowNumber) {
                        $(this).css({
                            "background-color": "var(--bs-table-hover-bg)",
                        })
                    } else {
                        $(this).css({
                            "background-color": "transparent",
                        })
                    };
                });
            },
            save: function (loai) {
                var nhaCungCaps = [];
                $.each($(".nhacungcap-read", $("#nhacungcap-crud")), function () {
                    var $div = $(this),
                        rowNumber = $div.attr("row");

                    //var tepDinhKems = [];
                    //$.each($(`#tailieu-items .image-item`, $div), function () {
                    //    let idTep = $(this).attr("data-id");
                    //    // Chỉ lấy những tệp đã tồn tại trong CSDL
                    //    if (idTep != "00000000-0000-0000-0000-000000000000") {
                    //        tepDinhKems.push({
                    //            IdTep: idTep,
                    //        });
                    //    };
                    //});

                    var idTruongHocs =
                        $(`#select-truonghoc-${rowNumber}`, $div).val()?.map(x => ({
                            IdTruongHoc: x
                        }));
                    var idNhaCungCapCha = $(`#select-nhacungcapcha-${rowNumber}`, $div).val();
                    var idNhaCungCap = $(`#input-idnhacungcap`, $div).val();

                    var nhaCungCap = {
                        RowNumber: rowNumber,
                        NhaCungCap: {
                            IdNhaCungCap: idNhaCungCap,
                            IdNhaCungCapCha: idNhaCungCapCha,

                            //MaNhaCungCap: $("#input-manhacungcap", $div).val().trim(),
                            TenNhaCungCap: $("#input-tennhacungcap", $div).val().trim(),
                            TenMatHang: $("#input-tenmathang", $div).val().trim(),
                            SoDienThoai: $("#input-sodienthoai", $div).val().trim(),
                            Email: $("#input-email", $div).val().trim(),
                            DiaChi: $("#input-diachi", $div).val().trim(),
                            GhiChu: $("#input-ghichu", $div).val().trim(),
                        },
                        TruongHocs: idTruongHocs,
                        //TaiLieus: taiLieus,
                    };

                    nhaCungCaps.push(nhaCungCap);
                });

                sys.confirmDialog({
                    mess: loai == 'create'
                        ? `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`
                        : `<p>Bản ghi sẽ được lưu nháp cho lần sử dụng tiếp theo ?</p>`,
                    callback: function () {
                        var formData = new FormData();
                        formData.append("nhaCungCaps", JSON.stringify(nhaCungCaps));
                        formData.append("loai", loai);

                        $.each(quanLyNhaCungCap.nhaCungCap.handleTaiLieu.arrTaiLieu, function (idx, anh) {
                            formData.append("files", anh.file);
                            formData.append("rowNumbers", anh.rowNumber);
                        });

                        var url = "/QuanLyNhaCungCap/create_NhaCungCap";
                        //if (loai == "create" || loai == "draft")
                        if (loai == "draftToSave" || loai == "update") url = "/QuanLyNhaCungCap/update_NhaCungCap";
                        $.ajax({
                            ...ajaxDefaultProps({
                                url: url,
                                type: "POST",
                                data: formData,
                            }),
                            //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                            contentType: false,
                            processData: false,
                            success: function (res) {
                                if (res.status == "success") {
                                    quanLyNhaCungCap.nhaCungCap.getList();

                                    sys.displayModal({
                                        name: '#nhacungcap-crud',
                                        displayStatus: "hide"
                                    });
                                };
                                sys.alert({ status: res.status, mess: res.mess });
                            }
                        });
                    }
                });
            },
            close: function () {
                sys.confirmDialog({
                    mess: `<span class="font-bold">Bản ghi chưa hoàn thiện</span><br />
                    <p>Bạn có muốn tiếp tục không ?</p>`,
                    callback_no: function () {
                        sys.displayModal({
                            name: "#confirmdialog",
                            displayStatus: "hide",
                            level: 100
                        });
                        sys.displayModal({
                            name: '#nhacungcap-crud',
                            displayStatus: "hide"
                        });
                    }
                });
            },
            excel: {
                import: function () {
                    var $modal = $("#nhacungcap-crud");
                    var $excel = $("#select-excel", $modal).get(0),
                        formData = new FormData();
                    $.each($excel.files, function (idx, f) {
                        var extension = f.type;
                        if (extension.includes("sheet")) {
                            formData.append("files", f);
                        };
                    });
                    // Xóa bộ nhớ đệm để upload file trong lần tiếp theo
                    $excel.value = ''; // xóa giá trị của input file

                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyNhaCungCap/importPreview_NhaCungCap_Excel_Ajax",
                            type: "POST",
                            data: formData
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {

                            // ❌ Có lỗi -> tự tải file lỗi về ngay
                            if (res.status == "error" && res.downloadToken) {
                                sys.alert({ status: "error", mess: res.mess });

                                // trigger download dialog (không cần base64)
                                window.location = "/QuanLyNhaCungCap/downloadImportError_NhaCungCap_Excel?token=" + res.downloadToken;
                                return;
                            }

                            // ✅ OK -> trả data preview để user xem và bấm lưu
                            if (res.status == "success") {
                                quanLyNhaCungCap.nhaCungCap.previewData = res.data;
                                sys.alert({ status: "success", mess: res.mess });

                                // mở modal preview để user chọn lưu
                                sys.displayModal({ name: "#ncc-import-preview", displayStatus: "show" });
                                return;
                            }

                            sys.alert({ status: res.status, mess: res.mess });
                        }
                    });

                },
                export: function () {
                    window.location = "/QuanLyNhaCungCap/exportTemplate_NhaCungCap_Excel";
                }

            }
        };
    }
};