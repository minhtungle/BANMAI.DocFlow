'use strict'
/**
 * main
 * */
class QuanLyDonVi {
    constructor() {
        this.page;
        this.pageGroup;
        this.donVi = {}
    }
    init() {
        var quanLyDonVi = this;
        var idNguoiDung_DangSuDung = $("#input-idnguoidung-dangsudung").val();
        quanLyDonVi.page = $("#page-quanlydonvi");

        quanLyDonVi.donVi = {
            ...quanLyDonVi.donVi,
            dataTable: null,
            thietLapGiaoDien: {
                logo: null,
            },
            handleImg: {
                add: function () {
                    var $modal = $("#donvi-crud");
                    var $selectLogo = $("#input-logo", $modal).get(0),
                        kiemTra = true;
                    $.each($selectLogo.files, function (idx, f) {
                        var extension = f.type;
                        if (/\.(png|jpg|jpeg|ico)$/i.test(f.name)) {
                            quanLyDonVi.donVi.thietLapGiaoDien.logo = f;
                        } else {
                            kiemTra = false;
                        };
                    });
                    // Xóa bộ nhớ đệm để upload file trong lần tiếp theo
                    $selectLogo.value = ''; // xóa giá trị của input file
                    if (!kiemTra) {
                        sys.alert({
                            status: "error",
                            mess: `Tồn tại tệp không thuộc định dạng cho phép [pdf|png|jpg|jpeg|ico]`,
                            timeout: 5000
                        });
                    } else {
                        // Tạo URL đại diện cho đối tượng dữ liệu
                        var imageUrl = URL.createObjectURL(quanLyDonVi.donVi.thietLapGiaoDien.logo);
                        $("#demo-logo", $modal).attr("src", imageUrl);
                    };
                },
                delete: function () {
                    var $modal = $("#donvi-crud");
                    $("#demo-logo", $modal).attr("src", "/Assets/images/logo-img.png");
                    quanLyDonVi.donVi.thietLapGiaoDien.logo = null;
                },
            },
            getList: function () {
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyDonVi/getList_DonVi",
                        type: "GET", // Phải là POST để gửi JSON
                        //contentType: "application/json; charset=utf-8",  // Định dạng JSON
                        //dataType: "json",
                    }),
                    //contentType: false,
                    //processData: false,
                    success: function (res) {
                        $("#donvi-getList-container").html(res);
                        quanLyDonVi.donVi.dataTable = new DataTableCustom({
                            name: "donvi-getList",
                            table: $("#donvi-getList"),
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
            displayModal_CRUD: function (loai = "", idDonVi = '00000000-0000-0000-0000-000000000000') {
                var idDonVis = [];
                if (loai == "create") idDonVis.push(idDonVi);
                else {
                    if (idDonVi != '00000000-0000-0000-0000-000000000000')
                        idDonVis.push(idDonVi);
                    else {
                        quanLyDonVi.donVi.dataTable.rows().iterator('row', function (context, index) {
                            var $row = $(this.row(index).node());
                            if ($row.has("input.checkRow-donvi-getList:checked").length > 0) {
                                idDonVis.push($row.attr('id'));
                            };
                        });
                        if (idDonVis.length != 1) {
                            sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                            return;
                        }
                    }
                }

                var input = {
                    Loai: loai,
                    IdDonVi: idDonVis[0],
                };
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyDonVi/displayModal_CRUD_DonVi",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: {
                            input
                        },
                    }),
                    success: function (res) {
                        $("#donvi-crud").html(res);
                        /**
                          * Gán các thuộc tính
                          */
                        sys.displayModal({
                            name: '#donvi-crud'
                        });
                    }
                })
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#donvi-crud");
                if (modalValidtion) {
                    let $modal = $("#donvi-crud");
                    let donVi = {
                        MaDonViSuDung: $("#input-madonvisudung", $modal).val(),
                        TenMien: $("#input-tenmien", $modal).val().trim(),
                        Logo: quanLyDonVi.donVi.thietLapGiaoDien.logo == null ? "/Assets/images/logo-img.png" : "",
                        TenDonViSuDung: $("#input-tendonvisudung", $modal).val().trim(),
                        TieuDeTrangChu: $("#input-tieudetrangchu", $modal).val().trim(),
                        DiaChi: $("#input-diachi", $modal).val().trim(),
                        Email: $("#input-email", $modal).val().trim(),
                        SoDienThoai: $("#input-sodienthoai", $modal).val().trim(),
                        GiaoDien: $("#input-giaodien", $modal).val().trim(),
                        GhiChu: $("#input-ghichu", $modal).val().trim(),
                    };
                    sys.confirmDialog({
                        mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                        callback: function () {
                            var formData = new FormData();
                            formData.append("donVi", JSON.stringify(donVi));
                            formData.append("logo", quanLyDonVi.donVi.thietLapGiaoDien.logo);

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: loai == "create" ? "/QuanLyDonVi/create_DonVi" : "/QuanLyDonVi/update_DonVi",
                                    type: "POST",
                                    data: formData,
                                }),
                                //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.status == "success") {
                                        quanLyDonVi.donVi.getList();
                                        sys.displayModal({
                                            name: '#donvi-crud',
                                            displayStatus: "hide"
                                        });
                                        sys.alert({ status: res.status, mess: res.mess });
                                    } else {
                                        if (res.status == "warning") {
                                            htmlEl.inputValidationStates(
                                                $("#input-tenmien"),
                                                "#donvi-crud",
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
            delete: function (loai, idDonVi = '00000000-0000-0000-0000-000000000000') {
                var idDonVis = [];
                // Lấy id
                if (loai == "single") {
                    idDonVis.push(idDonVi)
                } else {
                    quanLyDonVi.donVi.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-donvi-getList:checked").length > 0) {
                            idDonVis.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra id
                if (idDonVis.length > 0) {
                    var f = new FormData();
                    f.append("idDonVis", JSON.stringify(idDonVis));
                    sys.confirmDialog({
                        mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyDonVi/delete_DonVis",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    sys.alert({ status: res.status, mess: res.mess })
                                    quanLyDonVi.donVi.getList();
                                }
                            })
                        }
                    })
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                }
            },
        };

        quanLyDonVi.nguoiDung = {

        };
        quanLyDonVi.donVi.getList();
        sys.activePage({
            page: quanLyDonVi.page.attr("id"),
            pageGroup: quanLyDonVi.pageGroup
        });
    }
};