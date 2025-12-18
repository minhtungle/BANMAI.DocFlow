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
            getList: function () {
                var $timKiem = $("#nhacungcap-timkiem-collapse");
                var input = {
                    LocThongTin: {
                        //ThoiGianBatDau: $("#input-thoigianbatdau", $timKiem).val().trim(),
                        //ThoiGianKetThuc: $("#input-thoigianketthuc", $timKiem).val().trim(),

                        //IdNhaCungCaps: [
                        //    $("#select-idnhacungcap", $timKiem).val(),
                        //],
                        MaNhaCungCap: $("#select-manhacungcap", $timKiem).val(),
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
                        $("#nhacungcap-crud").html(res);
                        /**
                          * Gán các thuộc tính
                          */
                        sys.displayModal({
                            name: '#nhacungcap-crud'
                        });
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
                            MaNhaCungCap: $("#input-manhacungcap", $modal).val().trim(),
                            TenNhaCungCap: $("#input-tennhacungcap", $modal).val().trim(),
                            TenMatHang: $("#input-tenmathang", $modal).val().trim(),
                            SoDienThoai: $("#input-sodienthoai", $modal).val().trim(),
                            Email: $("#input-email", $modal).val().trim(),
                            NgheNghiep: $("#input-nghenghiep", $modal).val().trim(),
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
                                                $("#input-manhacungcap"),
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
                            MaNhaCungCap: $("#input-manhacungcap", $tab).val().trim(),
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
};