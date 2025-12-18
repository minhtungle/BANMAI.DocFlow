'use strict'
/**
 * main
 * */
class QuanLyTaiKhoan {
    constructor() {
        this.page;
        this.pageGroup;
        this.taiKhoan = {}
    }
    init() {
        var quanLyTaiKhoan = this;
        var idNguoiDung_DangSuDung = $("#input-idnguoidung-dangsudung").val();
        quanLyTaiKhoan.page = $("#page-quanlytaikhoan");

        quanLyTaiKhoan.taiKhoan = {
            ...quanLyTaiKhoan.taiKhoan,
            dataTable: null,
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyTaiKhoan/getList_TaiKhoan",
                        type: "GET", // Phải là POST để gửi JSON
                        //contentType: "application/json; charset=utf-8",  // Định dạng JSON
                        //dataType: "json",
                    }),
                    //contentType: false,
                    //processData: false,
                    success: function (res) {
                        $("#taikhoan-getList-container").html(res);
                        quanLyTaiKhoan.taiKhoan.dataTable = new DataTableCustom({
                            name: "taikhoan-getList",
                            table: $("#taikhoan-getList"),
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
            xacThucTaiKhoan: function () {
                var $modal = $("#taikhoan-crud #kiemtra");
                var modalValidtion = htmlEl.activeValidationStates($modal);
                if (modalValidtion) {
                    var input = {
                        Id: $("#input-id", $modal).val().trim(),
                        AccessToken: $("#input-accesstoken", $modal).val().trim(),
                        IdNenTang: $("#select-nentang", $modal).val(),
                        LoaiTaiKhoan: $("#select-loaitaikhoan", $modal).val(),
                    };
                    $.ajax({
                        ...ajaxDefaultProps({
                            //url: "/QuanLyTaiKhoan/getUserAndPages",
                            url: input.LoaiTaiKhoan.toLowerCase() == "page"
                                ? "/QuanLyTaiKhoan/getPageInfo"
                                : "/QuanLyTaiKhoan/getUserAndPages",
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            data: { input },
                        }),
                        success: function (res) {
                            if (res.status == "success") {
                                $("#thongtinxacthuc-container", $modal).html(res.html);
                                sys.alert({ status: res.status, mess: res.mess });
                            } else {
                                sys.alert({ status: res.status, mess: "Kết nối thất bại" });
                            }
                        }
                    })
                }
            },
            chonNenTang: function () {
                var $modal = $("#taikhoan-crud #thongtin");

                var idNenTang = $("#select-nentang", $modal).val();

                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyTaiKhoan/chonNenTang",
                        type: "GET",
                        data: { input: idNenTang },
                    }),
                    success: function (res) {
                        $("#yeucaunentang-container", $modal).html(res);
                    }
                })
            },
            chonLoaiTaiKhoan: function () {
                var $modal = $("#taikhoan-crud #thongtin");

                var loaiTaiKhoan = $("#select-loaitaikhoan", $modal).val();
                if (loaiTaiKhoan.toLowerCase() == "user") // User
                {
                    // Nếu là User thì khog cần nhập page_id, page_access_token
                    $("#input-page_id", $modal).removeAttr("required");
                    $("#input-page_accesss_token", $modal).removeAttr("required");

                    // Bỏ class "required" trên label
                    $("label[for='input-page_id']", $modal).removeClass("required");
                    $("label[for='input-page_accesss_token']", $modal).removeClass("required");
                } else {
                    $("#input-page_id", $modal).attr("required", "required");
                    $("#input-page_accesss_token", $modal).attr("required", "required");

                    // Thêm class "required" cho label
                    $("label[for='input-page_id']", $modal).addClass("required");
                    $("label[for='input-page_accesss_token']", $modal).addClass("required");
                }
            },
            displayModal_CRUD: function (loai = "", idTaiKhoan = '00000000-0000-0000-0000-000000000000') {
                var idTaiKhoans = [];
                if (loai == "create") idTaiKhoans.push(idTaiKhoan);
                else {
                    if (idTaiKhoan != '00000000-0000-0000-0000-000000000000')
                        idTaiKhoans.push(idTaiKhoan);
                    else {
                        quanLyTaiKhoan.taiKhoan.dataTable.rows().iterator('row', function (context, index) {
                            var $row = $(this.row(index).node());
                            if ($row.has("input.checkRow-taikhoan-getList:checked").length > 0) {
                                idTaiKhoans.push($row.attr('id'));
                            };
                        });
                        if (idTaiKhoans.length != 1) {
                            sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                            return;
                        }
                    }
                }

                var input = {
                    Loai: loai,
                    IdTaiKhoan: idTaiKhoans[0],
                };
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyTaiKhoan/displayModal_CRUD_TaiKhoan",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: {
                            input
                        },
                    }),
                    success: function (res) {
                        $("#taikhoan-crud").html(res);
                        quanLyTaiKhoan.taiKhoan.chonLoaiTaiKhoan();
                        /**
                          * Gán các thuộc tính
                          */
                        sys.displayModal({
                            name: '#taikhoan-crud'
                        });
                    }
                })
            },
            save: function (loai) {
                let $modal = $("#taikhoan-crud #thongtin");
                var modalValidtion = htmlEl.activeValidationStates($modal);
                if (modalValidtion) {
                    let taiKhoan = {
                        IdTaiKhoan: $("#input-idtaikhoan", $modal).val(),
                        IdNenTang: $("#select-nentang", $modal).val(),
                        LoaiTaiKhoan: $("#select-loaitaikhoan", $modal).val(),
                        User_Id: $("#input-user_id", $modal).val().trim(),
                        User_Access_Token: $("#input-user_access_token", $modal).val().trim(),
                        Page_Id: $("#input-page_id", $modal).val().trim(),
                        Page_Access_Token: $("#input-page_access_token", $modal).val().trim(),
                        GhiChu: $("#input-ghichu", $modal).val().trim(),
                    };
                    sys.confirmDialog({
                        mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                        callback: function () {
                            var formData = new FormData();
                            formData.append("taiKhoan", JSON.stringify(taiKhoan));

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: loai == "create"
                                        ? "/QuanLyTaiKhoan/create_TaiKhoan"
                                        : "/QuanLyTaiKhoan/update_TaiKhoan",
                                    type: "POST",
                                    data: formData,
                                }),
                                //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.status == "success") {
                                        quanLyTaiKhoan.taiKhoan.getList();
                                        sys.displayModal({
                                            name: '#taikhoan-crud',
                                            displayStatus: "hide"
                                        });
                                        sys.alert({ status: res.status, mess: res.mess });
                                    } else {
                                        if (res.status == "warning") {
                                            htmlEl.inputValidationStates(
                                                $("#input-accesstoken"),
                                                "#taikhoan-crud",
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
            delete: function (loai, idTaiKhoan = '00000000-0000-0000-0000-000000000000') {
                var idTaiKhoans = [];
                // Lấy id
                if (loai == "single") {
                    idTaiKhoans.push(idTaiKhoan)
                } else {
                    quanLyTaiKhoan.taiKhoan.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-taikhoan-getList:checked").length > 0) {
                            idTaiKhoans.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra id
                if (idTaiKhoans.length > 0) {
                    var f = new FormData();
                    f.append("idTaiKhoans", JSON.stringify(idTaiKhoans));
                    sys.confirmDialog({
                        mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyTaiKhoan/delete_TaiKhoans",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    sys.alert({ status: res.status, mess: res.mess })
                                    quanLyTaiKhoan.taiKhoan.getList();
                                }
                            })
                        }
                    })
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                }
            },
        };
        quanLyTaiKhoan.taiKhoan.getList();
        sys.activePage({
            page: quanLyTaiKhoan.page.attr("id"),
            pageGroup: quanLyTaiKhoan.pageGroup
        });
    }
};