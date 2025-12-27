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
                        IdTaiLieu: $("#select-tailieu", $timKiem).val(),
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
            ...quanLyTaiLieu.taiLieu,

            displayModal_Create: function (loai = "") {
                var $modal = $("#tailieu-crud");
                var input = {
                    Loai: loai,
                    IdNhaCungCap: idNhaCungCaps[0],
                };

                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyNhaCungCap/displayModal_Create_NhaCungCap",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: { input },
                    }),
                    success: function (res) {
                        $("#nhacungcap-crud").html(res.html);
                        // Tạo modal crud
                        quanLyTaiLieu.createModalCRUD_NhaCungCap();
                        // Tự động kích hoạt nút import
                        $("#select-file", $modal).trigger("click");
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

            
        };
    }
    createModalCRUD_TaiLieu() {
        var quanLyTaiLieu = this;

        quanLyTaiLieu.handleModal_CRUD = {
            dataTable: new DataTableCustom({
                name: "tailieu-getList",
                table: $("#tailieu-getList", $("#tailieu-crud")),
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
                var rows = quanLyTaiLieu.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-tailieu-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    sys.displayModal({
                        name: '#tailieu-crud-capnhattruong',
                        level: 2
                    });
                };
            },
            updateSingleCell: function (el) {
                var rows = quanLyTaiLieu.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $div = $(el).closest(".tailieu-read"),
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
                var $modal = $("#tailieu-crud"),
                    endName = 'capnhattruong';

                var $modal_CapNhatTruong = $(`#tailieu-crud-${endName}`);

                var idTaiKhoan = $(`#select-taikhoan-${endName}`, $modal_CapNhatTruong).val(),
                    idAITool = $(`#select-aitool-${endName}`, $modal_CapNhatTruong).val(),
                    idAIBot = $(`#select-aibot-${endName}`, $modal_CapNhatTruong).val(),
                    idChienDich = $(`#select-chiendich-${endName}`, $modal_CapNhatTruong).val();

                var isCheck_TaiKhoan = $(`#checkbox-taikhoan-${endName}`, $modal_CapNhatTruong).is(":checked"),
                    isCheck_AIBot = $(`#checkbox-aibot-${endName}`, $modal_CapNhatTruong).is(":checked"),
                    isCheck_AITool = $(`#checkbox-aitool-${endName}`, $modal_CapNhatTruong).is(":checked"),
                    isCheck_ChienDich = $(`#checkbox-chiendich-${endName}`, $modal_CapNhatTruong).is(":checked");

                var rows = quanLyTaiLieu.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-tailieu-getList:checked`, rows);
                if ($rowChecks.length == 0) {
                    sys.alert({ status: "warning", mess: "Bạn chưa chọn bản ghi nào" })
                } else {
                    $.each($rowChecks, function () {
                        var $rowCheck = $(this).closest("tr"),
                            rowNumber = $rowCheck.attr("row"),
                            $div = $(`.tailieu-read[row=${rowNumber}]`, $modal);
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
                        name: '#tailieu-crud-capnhattruong',
                        displayStatus: "hide",
                        level: 2,
                    });
                };
            },
            import: function () {
                var $modal = $("#tailieu-crud");
                var $fileInput = $(`#select-file`, $modal).get(0);

                var kiemTra = true,
                    mess = "Thêm tệp thành công";

                var formData = new FormData();

                $.each($fileInput, function (idx, f) {
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
                });

                if (!kiemTra) {
                    sys.alert({
                        status: "error",
                        mess,
                        timeout: 5000
                    });
                } else {
                    $.each($fileInput, function (idx, f) {
                        formData.append("files", f); // Dùng khi save()
                    });

                    sys.alert({
                        status: "success",
                        mess: "Đã thêm tệp thành công",
                        timeout: 5000
                    });
                };

                $fileInput.value = ''; // xóa giá trị của input file
                quanLyTaiLieu.handleModal_CRUD.addBanGhi(formData);
            },
            addBanGhi: function (input) {
                var $modal = $("#tailieu-crud");
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyTaiLieu/addBanGhi_Modal_CRUD",
                        type: "POST",
                        data: f,
                    }),
                    contentType: false,
                    processData: false,
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
                            quanLyTaiLieu.handleModal_CRUD.dataTable.destroy();
                            // Tạo bản ghi mới
                            $("#tailieu-getList tbody", $modal).prepend(res.html_tailieu_row);
                            $("#tailieu-read-container", $modal).prepend(res.html_tailieu_read);
                            // Tạo dataTable
                            //if (!quanLyTaiLieu.handleModal_CRUD.dataTable) {
                            quanLyTaiLieu.handleModal_CRUD.dataTable = new DataTableCustom({
                                name: "tailieu-getList",
                                table: $("#tailieu-getList", $modal),
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
                            //quanLyTaiLieu.handleModal_CRUD.dataTable.row($(res.html_tailieu_row)).invalidate().draw(false);
                            // Chọn bản ghi đó
                            var rows_NEW = quanLyTaiLieu.handleModal_CRUD.dataTable.rows().nodes().toArray(); // Chọn phần thử đầu tiên của bảng
                            quanLyTaiLieu.handleModal_CRUD.readRow($(rows_NEW[0]));
                            $.each($(".tailieu-read", $modal), function () {
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
                                name: '#tailieu-crud'
                            });

                            setTimeout(function () {
                                var containerHeight = $("#tailieu-crud .modal-body").height() - 10;
                                $("#tailieu-read-container", $("#tailieu-crud")).height(containerHeight);
                            }, 500)
                        }
                    }
                })
                //#endregion
            },
            deleteBanGhi: function () {
                var $modal = $("#tailieu-crud");
                var rows = quanLyTaiLieu.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $rowChecks = $(`.checkRow-tailieu-getList:checked`, rows);

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
                        $div = $(`.tailieu-read[row=${rowNumber}]`, $modal);
                    // Xóa bản ghi trong div
                    quanLyTaiLieu.handleModal_CRUD.dataTable.row($rowCheck).remove().draw();
                    $div.remove();
                    // Xóa tệp trong mảng
                    quanLyTaiLieu.taiLieu.handleTaiLieu.arrTaiLieu = quanLyTaiLieu.taiLieu.handleTaiLieu.arrTaiLieu
                        .filter(function (anh) {
                            return anh.rowNumber != rowNumber;
                        });
                });

                // Lấy lại các dòng mới sau khi xóa
                var rows_NEW = quanLyTaiLieu.handleModal_CRUD.dataTable.rows().nodes().toArray();

                if (rows_NEW.length > 0) {
                    quanLyTaiLieu.handleModal_CRUD.readRow($(rows_NEW[0]));
                } else {
                    $modal.find(".tailieu-read").empty();
                }
            },
            readRow: function (el) {
                var $modal = $("#tailieu-crud");
                var rowNumber = $(el).attr("row"),
                    rows = quanLyTaiLieu.handleModal_CRUD.dataTable.rows().nodes().toArray(),
                    $divs = $(".tailieu-read", $modal),
                    $div = $(`.tailieu-read[row=${rowNumber}]`, $modal);

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
                var taiLieus = [];
                $.each($(".tailieu-read", $("#tailieu-crud")), function () {
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
                    var idTaiLieuCha = $(`#select-tailieucha-${rowNumber}`, $div).val();
                    var idTaiLieu = $(`#input-idtailieu`, $div).val();

                    var taiLieu = {
                        RowNumber: rowNumber,
                        TaiLieu: {
                            IdTaiLieu: idTaiLieu,
                            IdTaiLieuCha: idTaiLieuCha,

                            //MaTaiLieu: $("#input-matailieu", $div).val().trim(),
                            TenTaiLieu: $("#input-tentailieu", $div).val().trim(),
                            TenMatHang: $("#input-tenmathang", $div).val().trim(),
                            SoDienThoai: $("#input-sodienthoai", $div).val().trim(),
                            Email: $("#input-email", $div).val().trim(),
                            DiaChi: $("#input-diachi", $div).val().trim(),
                            GhiChu: $("#input-ghichu", $div).val().trim(),
                        },
                        TruongHocs: idTruongHocs,
                        //TaiLieus: taiLieus,
                    };

                    taiLieus.push(taiLieu);
                });

                sys.confirmDialog({
                    mess: loai == 'create'
                        ? `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`
                        : loai == "update"
                            ? `<p>Bạn có thực sự muốn cập nhật bản ghi này hay không ?</p>`
                            : `<p>Bản ghi sẽ được lưu nháp cho lần sử dụng tiếp theo ?</p>`,
                    callback: function () {
                        var formData = new FormData();
                        formData.append("taiLieus", JSON.stringify(taiLieus));
                        formData.append("loai", loai);

                        $.each(quanLyTaiLieu.taiLieu.handleTaiLieu.arrTaiLieu, function (idx, anh) {
                            formData.append("files", anh.file);
                            formData.append("rowNumbers", anh.rowNumber);
                        });

                        var url = "/QuanLyTaiLieu/create_TaiLieu";
                        //if (loai == "create" || loai == "draft")
                        if (loai == "draftToSave" || loai == "update") url = "/QuanLyTaiLieu/update_TaiLieu";
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
                                    quanLyTaiLieu.taiLieu.getList();

                                    sys.displayModal({
                                        name: '#tailieu-crud',
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
                            name: '#tailieu-crud',
                            displayStatus: "hide"
                        });
                    }
                });
            },
            excel: {
                import: function () {
                    var $modal = $("#tailieu-crud");
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
                            url: "/QuanLyTaiLieu/importPreview_TaiLieu_Excel",
                            type: "POST",
                            data: formData
                        }),
                        contentType: false,
                        processData: false,
                        success: function (res) {

                            // ❌ Có lỗi -> tự tải file lỗi về ngay
                            if (res.status == "error" && res.downloadToken) {
                                sys.alert({ status: "error", mess: res.mess, timeout: 4000 });

                                // trigger download dialog (không cần base64)
                                window.location = "/QuanLyTaiLieu/downloadImportError_TaiLieu_Excel?downloadToken=" + res.downloadToken;
                                return;
                            }
                            quanLyTaiLieu.handleModal_CRUD.excel.save(res.downloadToken);
                            //// ✅ OK -> trả data preview để user xem và bấm lưu
                            //if (res.status == "success") {
                            //    quanLyTaiLieu.taiLieu.previewData = res.data;
                            //    sys.alert({ status: "success", mess: res.mess });

                            //    // mở modal preview để user chọn lưu
                            //    sys.displayModal({ name: "#ncc-import-preview", displayStatus: "show" });
                            //    return;
                            //}

                            sys.alert({ status: res.status, mess: res.mess });
                        }
                    });

                },
                export: function () {
                    window.location = "/QuanLyTaiLieu/exportTemplate_TaiLieu_Excel";
                },
                save: function (downloadToken) {
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyTaiLieu/saveImport_TaiLieu_Excel",
                            type: "POST",
                            data: { downloadToken }
                        }),
                        success: function (res) {
                            if (res.status == "success") {
                                quanLyTaiLieu.taiLieu.getList();

                                sys.displayModal({
                                    name: '#tailieu-crud',
                                    displayStatus: "hide"
                                });
                            };
                            sys.alert({ status: res.status, mess: res.mess });
                        }
                    });
                },
            }
        };
    }
};