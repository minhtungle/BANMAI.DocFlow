'use strict'
/**
 * main
 * */
class QuanLyBenhNhan {
    constructor() {
        this.page;
        this.pageGroup;
        this.benhNhan = {}
    }
    init() {
        var quanLyBenhNhan = this;
        quanLyBenhNhan.page = $("#page-quanlybenhnhan");
        htmlEl = new HtmlElement();

        quanLyBenhNhan.benhNhan = {
            ...quanLyBenhNhan.benhNhan,
            dataTable: null,
            getList: function () {
                var $timKiem = $("#benhnhan-timkiem-collapse");
                var input = {
                    LocThongTin: {
                        ThoiGianBatDau: $("#input-thoigianbatdau", $timKiem).val().trim(),
                        ThoiGianKetThuc: $("#input-thoigianketthuc", $timKiem).val().trim(),

                        //IdBenhNhans: [
                        //    $("#select-idbenhnhan", $timKiem).val(),
                        //],
                        HoTen: $("#select-hoten", $timKiem).val(),
                        GioiTinh: $("#select-gioitinh", $timKiem).val() == 1 ? true : false,
                        NgaySinh: $("#input-ngaysinh", $timKiem).val().trim(),
                        SoDienThoai: $("#input-sodienthoai", $timKiem).val().trim(),
                        Email: $("#input-email", $timKiem).val().trim(),
                        CCCD: $("#input-cccd", $timKiem).val().trim(),
                        NgheNghiep: $("#input-nghenghiep", $timKiem).val().trim(),
                        DiaChi: $("#input-diachi", $timKiem).val().trim(),
                        TienSuBenh: $("#input-tiensubenh", $timKiem).val().trim(),
                        GhiChu: $("#input-ghichu", $timKiem).val().trim(),
                    }
                };
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyBenhNhan/getList_BenhNhan",
                        type: "POST", // Phải là POST để gửi JSON
                        contentType: "application/json; charset=utf-8",  // Định dạng JSON
                        data: { input }
                        //dataType: "json",
                    }),
                    //contentType: false,
                    //processData: false,
                    success: function (res) {
                        $("#benhnhan-getList-container").html(res);
                        quanLyBenhNhan.benhNhan.dataTable = new DataTableCustom({
                            name: "benhnhan-getList",
                            table: $("#benhnhan-getList"),
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
            displayModal_CRUD: function (loai = "", idBenhNhan = '00000000-0000-0000-0000-000000000000') {
                var idBenhNhans = [];
                if (loai == "create") idBenhNhans.push(idBenhNhan);
                else {
                    if (idBenhNhan != '00000000-0000-0000-0000-000000000000')
                        idBenhNhans.push(idBenhNhan);
                    else {
                        quanLyBenhNhan.benhNhan.dataTable.rows().iterator('row', function (context, index) {
                            var $row = $(this.row(index).node());
                            if ($row.has("input.checkRow-benhnhan-getList:checked").length > 0) {
                                idBenhNhans.push($row.attr('id'));
                            };
                        });
                        if (idBenhNhans.length != 1) {
                            sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                            return;
                        }
                    }
                };
                var input = {
                    Loai: loai,
                    IdBenhNhan: idBenhNhans[0],
                };

                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyBenhNhan/displayModal_CRUD_BenhNhan",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: {
                            input
                        },
                    }),
                    success: function (res) {
                        $("#benhnhan-crud").html(res);
                        /**
                          * Gán các thuộc tính
                          */
                        sys.displayModal({
                            name: '#benhnhan-crud'
                        });
                    }
                })
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#benhnhan-crud");
                if (modalValidtion) {
                    let $modal = $("#benhnhan-crud");
                    var nguoiThans = [];
                    var benhNhan = {
                        BenhNhan: {
                            IdBenhNhan: $("#input-idbenhnhan", $modal).val(),
                            HoTen: $("#input-hoten", $modal).val().trim(),
                            GioiTinh: $("#select-gioitinh", $modal).val() == 1 ? true : false,
                            NgaySinh: $("#input-ngaysinh", $modal).val().trim(),
                            SoDienThoai: $("#input-sodienthoai", $modal).val().trim(),
                            Email: $("#input-email", $modal).val().trim(),
                            CCCD: $("#input-cccd", $modal).val().trim(),
                            NgheNghiep: $("#input-nghenghiep", $modal).val().trim(),
                            DiaChi: $("#input-diachi", $modal).val().trim(),
                            TienSuBenh: $("#input-tiensubenh", $modal).val().trim(),
                            GhiChu: $("#input-ghichu", $modal).val().trim(),
                        },
                        NguoiThans: nguoiThans
                    }
                    sys.confirmDialog({
                        mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                        callback: function () {
                            var formData = new FormData();
                            formData.append("benhNhan", JSON.stringify(benhNhan));
                            formData.append("loai", loai);

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: loai == "create" ? "/QuanLyBenhNhan/create_BenhNhan" : "/QuanLyBenhNhan/update_BenhNhan",
                                    type: "POST",
                                    data: formData,
                                }),
                                //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.status == "success") {
                                        quanLyBenhNhan.benhNhan.getList();

                                        sys.displayModal({
                                            name: '#benhnhan-crud',
                                            displayStatus: "hide"
                                        });
                                        sys.alert({ status: res.status, mess: res.mess });
                                    } else {
                                        if (res.status == "warning") {
                                            htmlEl.inputValidationStates(
                                                $("#input-cccd"),
                                                "#benhnhan-crud",
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
            delete: function (loai, idBenhNhan = '00000000-0000-0000-0000-000000000000') {
                var idBenhNhans = [];
                // Lấy id
                if (loai == "single") {
                    idBenhNhans.push(idBenhNhan)
                } else {
                    quanLyBenhNhan.benhNhan.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-benhnhan-getList:checked").length > 0) {
                            idBenhNhans.push($row.attr('id'));
                        };
                    });
                };
                // Kiểm tra id
                if (idBenhNhans.length > 0) {
                    var f = new FormData();
                    f.append("idBenhNhans", JSON.stringify(idBenhNhans));
                    sys.confirmDialog({
                        mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyBenhNhan/delete_BenhNhans",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    sys.alert({ status: res.status, mess: res.mess })
                                    quanLyBenhNhan.benhNhan.getList();
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
                    var benhNhan = {
                        BenhNhan: {
                            IdBenhNhan: $("#input-idbenhnhan", $tab).val(),
                            HoTen: $("#input-hoten", $tab).val().trim(),
                            GioiTinh: $("#select-gioitinh", $tab).val() == 1 ? true : false,
                            NgaySinh: $("#input-ngaysinh", $tab).val().trim(),
                            SoDienThoai: $("#input-sodienthoai", $tab).val().trim(),
                            Email: $("#input-email", $tab).val().trim(),
                            CCCD: $("#input-cccd", $tab).val().trim(),
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
                            formData.append("benhNhan", JSON.stringify(benhNhan));

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyBenhNhan/update_BenhNhan",
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
                                            quanLyBenhNhan.benhNhan.xemChiTiet.showTab();
                                        }, 1000);
                                    } else {
                                        if (res.status == "warning") {
                                            htmlEl.inputValidationStates(
                                                $("#input-cccd"),
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

            lichHen: {
                dataTable: null,
                displayModal_CRUD: function (loai = "", idLichHen = '00000000-0000-0000-0000-000000000000') {
                    var idLichHens = [];
                    if (loai == "create") idLichHens.push(idLichHen);
                    else {
                        if (idLichHen != '00000000-0000-0000-0000-000000000000')
                            idLichHens.push(idLichHen);
                        else {
                            quanLyBenhNhan.benhNhan.lichHen.dataTable.rows().iterator('row', function (context, index) {
                                var $row = $(this.row(index).node());
                                if ($row.has("input.checkRow-lichhen-getList:checked").length > 0) {
                                    idLichHens.push($row.attr('id'));
                                };
                            });
                            if (idLichHens.length != 1) {
                                sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                                return;
                            }
                        }
                    };
                    var input = {
                        Loai: loai,
                        IdLichHen: idLichHens[0],
                    };

                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyBenhNhan/displayModal_CRUD_LichHen",
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            data: {
                                input
                            },
                        }),
                        success: function (res) {
                            $("#lichhen-crud").html(res);
                            /**
                              * Gán các thuộc tính
                              */
                            sys.displayModal({
                                name: '#lichhen-crud'
                            });
                        }
                    })
                },
                save: function (loai) {
                    var modalValidtion = htmlEl.activeValidationStates("#lichhen-crud");
                    if (modalValidtion) {
                        let $modal = $("#lichhen-crud");
                        var lichHen = {
                            LichHen: {
                                IdLichHen: $("#input-idlichhen", $modal).val(),
                                IdBenhNhan: $("#select-danhsachbenhnhan").val(),
                                IdBacSy: $("#select-bacsy", $modal).val(),
                                LoaiDieuTri: $("#select-loaidieutri", $modal).val(),
                                LoaiThoiGian: $("#select-loaithoigian", $modal).val(),
                                NgayHen: $("#input-ngayhen", $modal).val(),
                                ThoiGianBatDau: $("#input-thoigianbatdau", $modal).val(),
                                ThoiGianKetThuc: $("#input-thoigianketthuc", $modal).val(),
                                NoiDungKham: $("#input-noidungkham", $modal).val().trim(),
                                GhiChu: $("#input-ghichu", $modal).val().trim(),
                            }
                        }
                        sys.confirmDialog({
                            mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                            callback: function () {
                                var formData = new FormData();
                                formData.append("lichHen", JSON.stringify(lichHen));
                                formData.append("loai", loai);

                                $.ajax({
                                    ...ajaxDefaultProps({
                                        url: loai == "create" ? "/QuanLyBenhNhan/create_LichHen" : "/QuanLyBenhNhan/update_LichHen",
                                        type: "POST",
                                        data: formData,
                                    }),
                                    //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                    contentType: false,
                                    processData: false,
                                    success: function (res) {
                                        if (res.status == "success") {
                                            quanLyBenhNhan.benhNhan.xemChiTiet.showTab();

                                            sys.displayModal({
                                                name: '#lichhen-crud',
                                                displayStatus: "hide"
                                            });
                                        };
                                        sys.alert({ status: res.status, mess: res.mess });

                                    }
                                });
                            }
                        });
                    };
                },
                delete: function (loai, idLichHen = '00000000-0000-0000-0000-000000000000') {
                    var idLichHens = [];
                    // Lấy id
                    if (loai == "single") {
                        idLichHens.push(idLichHen)
                    } else {
                        quanLyBenhNhan.lichhen.dataTable.rows().iterator('row', function (context, index) {
                            var $row = $(this.row(index).node());
                            if ($row.has("input.checkRow-lichhen-getList:checked").length > 0) {
                                idLichHens.push($row.attr('id'));
                            };
                        });
                    };
                    // Kiểm tra id
                    if (idLichHens.length > 0) {
                        var f = new FormData();
                        f.append("idLichHens", JSON.stringify(idLichHens));
                        sys.confirmDialog({
                            mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                            callback: function () {
                                $.ajax({
                                    ...ajaxDefaultProps({
                                        url: "/QuanLyBenhNhan/delete_LichHens",
                                        type: "POST",
                                        data: f,
                                    }),
                                    contentType: false,
                                    processData: false,
                                    success: function (res) {
                                        sys.alert({ status: res.status, mess: res.mess })
                                        quanLyBenhNhan.benhNhan.xemChiTiet.showTab();
                                    }
                                })
                            }
                        })
                    } else {
                        sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                    }
                },
            },

            phieuKham: {
                dataTable: null,
                displayModal_CRUD: function (loai = "", idPhieuKham = '00000000-0000-0000-0000-000000000000') {
                    var idPhieuKhams = [];
                    if (loai == "create") idPhieuKhams.push(idPhieuKham);
                    else {
                        if (idPhieuKham != '00000000-0000-0000-0000-000000000000')
                            idPhieuKhams.push(idPhieuKham);
                        else {
                            quanLyBenhNhan.benhNhan.phieukham.dataTable.rows().iterator('row', function (context, index) {
                                var $row = $(this.row(index).node());
                                if ($row.has("input.checkRow-phieukham-getList:checked").length > 0) {
                                    idPhieuKhams.push($row.attr('id'));
                                };
                            });
                            if (idPhieuKhams.length != 1) {
                                sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                                return;
                            }
                        }
                    };
                    var input = {
                        Loai: loai,
                        IdPhieuKham: idPhieuKhams[0],
                    };

                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyBenhNhan/displayModal_CRUD_PhieuKham",
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            data: {
                                input
                            },
                        }),
                        success: function (res) {
                            $("#phieukham-crud").html(res);
                            /**
                              * Gán các thuộc tính
                              */
                            sys.displayModal({
                                name: '#phieukham-crud'
                            });
                        }
                    })
                },
                save: function (loai) {
                    var modalValidtion = htmlEl.activeValidationStates("#phieukham-crud");
                    if (modalValidtion) {
                        let $modal = $("#phieukham-crud");
                        var phieuKham = {
                            PhieuKham: {
                                IdBenhNhan: $("#select-danhsachbenhnhan").val(),
                                GhiChu: $("#input-ghichu", $modal).val().trim(),
                            }
                        }
                        sys.confirmDialog({
                            mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                            callback: function () {
                                var formData = new FormData();
                                formData.append("phieuKham", JSON.stringify(phieuKham));
                                formData.append("loai", loai);

                                $.ajax({
                                    ...ajaxDefaultProps({
                                        url: loai == "create" ? "/QuanLyBenhNhan/create_PhieuKham" : "/QuanLyBenhNhan/update_PhieuKham",
                                        type: "POST",
                                        data: formData,
                                    }),
                                    //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                    contentType: false,
                                    processData: false,
                                    success: function (res) {
                                        if (res.status == "success") {
                                            quanLyBenhNhan.benhNhan.xemChiTiet.showTab();

                                            sys.displayModal({
                                                name: '#phieukham-crud',
                                                displayStatus: "hide"
                                            });
                                        };
                                        sys.alert({ status: res.status, mess: res.mess });

                                    }
                                });
                            }
                        });
                    };
                },
                delete: function (loai, idLichHen = '00000000-0000-0000-0000-000000000000') {
                    var idLichHens = [];
                    // Lấy id
                    if (loai == "single") {
                        idLichHens.push(idLichHen)
                    } else {
                        quanLyBenhNhan.lichhen.dataTable.rows().iterator('row', function (context, index) {
                            var $row = $(this.row(index).node());
                            if ($row.has("input.checkRow-lichhen-getList:checked").length > 0) {
                                idLichHens.push($row.attr('id'));
                            };
                        });
                    };
                    // Kiểm tra id
                    if (idLichHens.length > 0) {
                        var f = new FormData();
                        f.append("idLichHens", JSON.stringify(idLichHens));
                        sys.confirmDialog({
                            mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                            callback: function () {
                                $.ajax({
                                    ...ajaxDefaultProps({
                                        url: "/QuanLyBenhNhan/delete_LichHens",
                                        type: "POST",
                                        data: f,
                                    }),
                                    contentType: false,
                                    processData: false,
                                    success: function (res) {
                                        sys.alert({ status: res.status, mess: res.mess })
                                        quanLyBenhNhan.benhNhan.xemChiTiet.showTab();
                                    }
                                })
                            }
                        })
                    } else {
                        sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                    }
                },
                update: function () {
                    var modalValidtion = htmlEl.activeValidationStates("#tab-phieukham");
                    if (modalValidtion) {
                        let $tab = $("#tab-phieukham");
                        var phieuKham = {
                            PhieuKham: {
                                IdPhieuKham: $("#input-idphieukham", $tab).val().trim(),
                                GhiChu: $("#input-ghichu", $tab).val().trim(),
                            }
                        }
                        sys.confirmDialog({
                            mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                            callback: function () {
                                var formData = new FormData();
                                formData.append("phieuKham", JSON.stringify(phieuKham));

                                $.ajax({
                                    ...ajaxDefaultProps({
                                        url: "/QuanLyBenhNhan/update_PhieuKham",
                                        type: "POST",
                                        data: formData,
                                    }),
                                    //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                    contentType: false,
                                    processData: false,
                                    success: function (res) {
                                        if (res.status == "success") {
                                            quanLyBenhNhan.benhNhan.xemChiTiet.showTab();
                                        };
                                        sys.alert({ status: res.status, mess: res.mess });

                                    }
                                });
                            }
                        });
                    };
                },
            },

            lichDieuTri: {
                dataTable: null,
                tienTrinhDieuTri: {
                    dataTable: null,
                    themDongTienTrinhDieuTri: function (loai, idLichDieuTri = '00000000-0000-0000-0000-000000000000') {
                        var input = {
                            Loai: loai,
                            IdLichDieuTri: idLichDieuTri,
                        };

                        $.ajax({
                            ...ajaxDefaultProps({
                                url: "/QuanLyBenhNhan/themDong_TienTrinhDieuTri",
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                data: {
                                    input
                                },
                            }),
                            success: function (res) {
                                var $modal = $("#lichdieutri-crud");
                                var $table = $("#tientrinhdieutri-getList", $modal);
                                $("tbody", $table).prepend(res);
                                $("tbody tr td.dataTables_empty", $table).closest("tr").empty(); // Xóa dòng default empty

                                htmlEl = new HtmlElement();
                                htmlEl.activeValidationStates($("tbody tr:first", $table)); // Valid cho dòng đó luôn
                            }
                        })
                    },
                    xoaDongTienTrinhDieuTri: function (e) {
                        var $tr = $(e).closest("tr");
                        $tr.remove();

                        //var $modal = $("#lichdieutri-crud");
                        //var $table = $("#tientrinhdieutri-getList", $modal);
                        //quanLyBenhNhan.benhNhan.lichDieuTri.dataTable.row($tr).remove().draw();
                    },
                    displayModal_ChonRang: function (e) {
                        var $tr = $(e).closest("tr");
                        var idTienTrinhDieuTri = $tr.attr("fake-id") ?? $tr.attr("id");
                        var soRangDaChons = $(`#input-rangso-${idTienTrinhDieuTri}`, $tr).val().trim().split(',').filter(x => x != "");

                        var input = {
                            IdTienTrinhDieuTri: idTienTrinhDieuTri,
                            SoRangDaChons: soRangDaChons,
                        };

                        $.ajax({
                            ...ajaxDefaultProps({
                                url: "/QuanLyBenhNhan/displayModal_ChonRang",
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                data: { input },
                            }),
                            success: function (res) {
                                $("#tientrinhdieutri-chonrang").html(res);
                                /**
                                  * Gán các thuộc tính
                                  */
                                sys.displayModal({
                                    name: "#tientrinhdieutri-chonrang",
                                    level: 2
                                });
                            }
                        })
                    },
                    chonRang: function () {
                        var soRangDaChon = [];
                        var $modal = $("#tientrinhdieutri-chonrang");
                        var idTienTrinhDieuTri = $("#input-idtientrinhdieutri", $modal).val();

                        $("#table-sodohamrang input.checkbox-rang:checked").each(function () {
                            soRangDaChon.push($(this).data("vitri"));
                        });

                        var rangSo = soRangDaChon.join(', ');
                        var soLuongRang = soRangDaChon.length;
                        var $modalLichDieuTriCRUD = $("#lichdieutri-crud");
                        $(`#input-rangso-${idTienTrinhDieuTri}`, $modalLichDieuTriCRUD).val(rangSo);
                        $(`#input-rangso-${idTienTrinhDieuTri}`, $modalLichDieuTriCRUD).change();
                        $(`#input-soluong-${idTienTrinhDieuTri}`, $modalLichDieuTriCRUD).val(soLuongRang);
                        $(`#input-soluong-${idTienTrinhDieuTri}`, $modalLichDieuTriCRUD).change();
                        $(`#label-rangso-${idTienTrinhDieuTri}`, $modalLichDieuTriCRUD).text(rangSo);

                        sys.displayModal({
                            name: "#tientrinhdieutri-chonrang",
                            level: 2,
                            displayStatus: "hide"
                        });
                    },
                    chonTinhTrangRang: function () { },
                    chonThuThuat: function (e) {
                        var $tr = $(e).closest("tr");
                        var idTienTrinhDieuTri = $tr.attr("fake-id") ?? $tr.attr("id");
                        var idThuThuat = $(e).val();
                        $.ajax({
                            ...ajaxDefaultProps({
                                url: "/QuanLyBenhNhan/chonThuThuat",
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                data: { idThuThuat: idThuThuat },
                            }),
                            success: function (res) {
                                $(`#input-giatien-${idTienTrinhDieuTri}`, $tr).val(res.GiaTien);
                                $(`#input-giatien-${idTienTrinhDieuTri}`, $tr).change();
                                $(`#input-noidungthuthuat-${idTienTrinhDieuTri}`, $tr).val(res.NoiDung);
                            }
                        })

                        //var giaTien = $(e).find("option:selected").data("giatien"),
                        //    noiDung = $(e).find("option:selected").data("noidung");

                        //$(`#input-giatien-${idTienTrinhDieuTri}`, $tr).val(giaTien);
                        //$(`#input-giatien-${idTienTrinhDieuTri}`, $tr).change();
                        //$(`#input-noidungthuthuat-${idTienTrinhDieuTri}`, $tr).val(noiDung);
                    },
                    tinhTongChiPhi: function (e) {
                        var $tr = $(e).closest("tr");
                        var idTienTrinhDieuTri = $tr.attr("fake-id") ?? $tr.attr("id");
                        var giaTien = parseInt($(`#input-giatien-${idTienTrinhDieuTri}`, $tr).val().replaceAll(' ', '') ?? 0),
                            phanTramGiamGia = parseInt($(`#input-phantramgiamgia-${idTienTrinhDieuTri}`, $tr).val() ?? 0),
                            soLuong = parseInt($(`#input-soluong-${idTienTrinhDieuTri}`, $tr).val()) ?? 0;

                        if (phanTramGiamGia < 0 || phanTramGiamGia > 100) {
                            $(`#input-tongchiphi-${idTienTrinhDieuTri}`, $tr).val(0);
                        } else {
                            var tongChiPhi = (giaTien - (giaTien * phanTramGiamGia / 100)) * soLuong;
                            $(`#input-tongchiphi-${idTienTrinhDieuTri}`, $tr).val(tongChiPhi);
                        };
                        $(`#input-tongchiphi-${idTienTrinhDieuTri}`, $tr).change();
                    },
                },
                displayModal_CRUD: function (loai = "", idLichDieuTri = '00000000-0000-0000-0000-000000000000') {
                    var idLichDieuTris = [];
                    if (loai == "create") idLichDieuTris.push(idLichDieuTri);
                    else {
                        if (idLichDieuTri != '00000000-0000-0000-0000-000000000000')
                            idLichDieuTris.push(idLichDieuTri);
                    };
                    var input = {
                        Loai: loai,
                        IdLichDieuTri: idLichDieuTris[0],
                    };

                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyBenhNhan/displayModal_CRUD_LichDieuTri",
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            data: {
                                input
                            },
                        }),
                        success: function (res) {
                            $("#lichdieutri-crud").html(res);
                            quanLyBenhNhan.benhNhan.lichDieuTri.tienTrinhDieuTri.dataTable = new DataTableCustom({
                                name: "tientrinhdieutri-getList",
                                table: $("#tientrinhdieutri-getList"),
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
                            /**
                              * Gán các thuộc tính
                              */
                            sys.displayModal({
                                name: '#lichdieutri-crud'
                            });
                        }
                    })
                },

                save: function (loai) {
                    var modalValidtion = htmlEl.activeValidationStates("#lichdieutri-crud", false);
                    if (modalValidtion) {
                        let $tab = $("#tab-phieukham");
                        let $modal = $("#lichdieutri-crud");
                        var tienTrinhDieuTris = [];
                        var $tienTrinhDieuTriRows = $("#tientrinhdieutri-getList tbody tr.tr-tientrinhdieutri", $modal);
                        $.each($tienTrinhDieuTriRows, function (index, $row) {
                            var idTienTrinhDieuTri = $(this).attr("fake-id") ?? $(this).attr("id");

                            var tienTrinhDieuTri = {
                                TienTrinhDieuTri: {
                                    IdTienTrinhDieuTri: $(this).attr("id"),
                                    IdLichDieuTri: $(this).attr("id"),
                                    IdPhieuKham: $("#input-idphieukham", $tab).val().trim(),

                                    RangSo: $(`#input-rangso-${idTienTrinhDieuTri}`, $(this)).val().trim(),
                                    IdTinhTrangRang: $(`#select-tinhtrangrang-${idTienTrinhDieuTri}`, $(this)).val(),
                                    IdThuThuat: $(`#select-thuthuat-${idTienTrinhDieuTri}`, $(this)).val(),
                                    NoiDungThuThuat: $(`#input-noidungthuthuat-${idTienTrinhDieuTri}`, $(this)).val().trim(),
                                    IdBacSy: $(`#select-bacsy-${idTienTrinhDieuTri}`, $(this)).val(),
                                    IdPhuTa: $(`#select-phuta-${idTienTrinhDieuTri}`, $(this)).val(),
                                    SoLuong: $(`#input-soluong-${idTienTrinhDieuTri}`, $(this)).val(),
                                    GiaTien: $(`#input-giatien-${idTienTrinhDieuTri}`, $(this)).val().replaceAll(' ', ''),
                                    PhanTramGiamGia: $(`#input-phantramgiamgia-${idTienTrinhDieuTri}`, $(this)).val(),
                                    TongChiPhi: $(`#input-tongchiphi-${idTienTrinhDieuTri}`, $(this)).val().replaceAll(' ', ''),
                                    TrangThaiDieuTri: $(`#select-trangthaidieutri-${idTienTrinhDieuTri}`, $(this)).val(),
                                    GhiChu: $(`#input-ghichu-${idTienTrinhDieuTri}`, $(this)).val().trim(),
                                },
                            };
                            tienTrinhDieuTris.push(tienTrinhDieuTri);
                        });
                        var lichDieuTri = {
                            LichDieuTri: {
                                IdPhieuKham: $("#input-idphieukham", $("#tab-phieukham")).val(),

                                IdLichDieuTri: $("#input-idlichdieutri", $modal).val(),
                                NgayDieuTri: $("#input-ngaydieutri", $modal).val(),
                                GhiChu: $("#input-ghichu", $modal).val().trim(),
                            },
                            TienTrinhDieuTris: tienTrinhDieuTris,
                        }
                        sys.confirmDialog({
                            mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                            callback: function () {
                                var formData = new FormData();
                                formData.append("lichDieuTri", JSON.stringify(lichDieuTri));
                                formData.append("loai", loai);

                                $.ajax({
                                    ...ajaxDefaultProps({
                                        url: loai == "create" ? "/QuanLyBenhNhan/create_LichDieuTri" : "/QuanLyBenhNhan/update_LichDieuTri",
                                        type: "POST",
                                        data: formData,
                                    }),
                                    //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                    contentType: false,
                                    processData: false,
                                    success: function (res) {
                                        if (res.status == "success") {
                                            quanLyBenhNhan.benhNhan.xemChiTiet.showTab();

                                            sys.displayModal({
                                                name: '#lichdieutri-crud',
                                                displayStatus: "hide"
                                            });
                                        };
                                        sys.alert({ status: res.status, mess: res.mess });

                                    }
                                });
                            }
                        });
                    };
                },
                delete: function (loai, idLichHen = '00000000-0000-0000-0000-000000000000') {
                    var idLichHens = [];
                    // Lấy id
                    if (loai == "single") {
                        idLichHens.push(idLichHen)
                    } else {
                        quanLyBenhNhan.lichhen.dataTable.rows().iterator('row', function (context, index) {
                            var $row = $(this.row(index).node());
                            if ($row.has("input.checkRow-lichhen-getList:checked").length > 0) {
                                idLichHens.push($row.attr('id'));
                            };
                        });
                    };
                    // Kiểm tra id
                    if (idLichHens.length > 0) {
                        var f = new FormData();
                        f.append("idLichHens", JSON.stringify(idLichHens));
                        sys.confirmDialog({
                            mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                            callback: function () {
                                $.ajax({
                                    ...ajaxDefaultProps({
                                        url: "/QuanLyBenhNhan/delete_LichHens",
                                        type: "POST",
                                        data: f,
                                    }),
                                    contentType: false,
                                    processData: false,
                                    success: function (res) {
                                        sys.alert({ status: res.status, mess: res.mess })
                                        quanLyBenhNhan.benhNhan.xemChiTiet.showTab();
                                    }
                                })
                            }
                        })
                    } else {
                        sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                    }
                },
            },

            xemChiTiet: {
                init: function () {
                    quanLyBenhNhan.benhNhan.xemChiTiet.showTab();
                },
                showTab: function () {
                    var input = {
                        TabName: $("#select-loaithongtin").val(),
                        IdBenhNhan: $("#select-danhsachbenhnhan").val(),
                    };
                    $.ajax({
                        ...ajaxDefaultProps({
                            url: "/QuanLyBenhNhan/showTab_BenhNhan",
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            data: { input },
                        }),
                        success: function (res) {
                            $("#benhnhan-xemchitiet-container").html(res);
                            htmlEl = new HtmlElement(); // Chạy lại hàm khởi tạo htmlEl

                            // Bản đồ tên tab -> hàm xử lý
                            var tabHandlers = {
                                "thongtincoban": "showTab_ThongTinCoBan",
                                "lichhen": "showTab_LichHen",
                                "phieukham": "showTab_PhieuKham",
                                "lichchamsoc": "showTab_LichChamSoc",
                                "donthuoc": "showTab_DonThuoc",
                                "thuvienanh": "showTab_ThuVienAnh",
                                "xuongvattu": "showTab_XuongVatTu",
                                "thanhtoan": "showTab_ThanhToan",
                                "bieumau": "showTab_BieuMau",
                                "lichsuthaotac": "showTab_LicHSuThaoTac"
                            };

                            var funcName = tabHandlers[input.TabName];
                            if (funcName && typeof quanLyBenhNhan.benhNhan.xemChiTiet[funcName] === "function") {
                                quanLyBenhNhan.benhNhan.xemChiTiet[funcName](); // Gọi hàm tương ứng
                            } else {
                                console.warn("Không tìm thấy hàm xử lý cho tab:", input.TabName);
                            }
                        }
                    })
                },
                showTab_ThongTinCoBan: function () { },
                showTab_LichHen: function () {
                    quanLyBenhNhan.benhNhan.lichHen.dataTable = new DataTableCustom({
                        name: "lichhen-getList",
                        table: $("#lichhen-getList"),
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
                },
                showTab_PhieuKham: function () {
                    quanLyBenhNhan.benhNhan.lichDieuTri.dataTable = new DataTableCustom({
                        name: "lichdieutri-getList",
                        table: $("#lichdieutri-getList"),
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
                },
                showTab_LichChamSoc: function () { },
                showTab_DonThuoc: function () { },
                showTab_ThuVienAnh: function () { },
                showTab_XuongVatTu: function () { },
                showTab_ThanhToan: function () { },
                showTab_BieuMau: function () { },
                showTab_LicHSuThaoTac: function () { },
            }
        };
    }
};