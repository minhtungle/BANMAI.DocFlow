'use strict'
/**
 * main
 * */
class QuanLyLichHen {
    constructor() {
        this.page;
        this.pageGroup;
        this.lichHen = {}
    }
    init() {
        var quanLyLichHen = this;
        quanLyLichHen.page = $("#page-quanlylichhen");
        htmlEl = new HtmlElement();

        quanLyLichHen.lichHen = {
            ...quanLyLichHen.lichHen,
            dataTable: null,
            getList: function () {
                var $timKiem = $("#lichhen-timkiem-collapse");
                var input = {
                    LocThongTin: {
                        ThoiGianBatDau: $("#input-thoigianbatdau", $timKiem).val().trim(),
                        ThoiGianKetThuc: $("#input-thoigianketthuc", $timKiem).val().trim(),

                        IdBacSy: $("#select-bacsy", $timKiem).val(),
                        IdBenhNhan: $("#select-benhnhan", $timKiem).val(),
                        LoaiDieuTri: $("#select-loaidieutri", $timKiem).val(),
                        LoaiThoiGian: $("#select-loaithoigian", $timKiem).val(),
                        NgayHen: $("#input-ngayhen", $timKiem).val(),
                        NoiDungKham: $("#input-noidungkham", $timKiem).val().trim(),
                        GhiChu: $("#input-ghichu", $timKiem).val().trim(),
                    }
                };
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyLichHen/getList_LichHen",
                        type: "POST", // Phải là POST để gửi JSON
                        contentType: "application/json; charset=utf-8",  // Định dạng JSON
                        data: { input }
                        //dataType: "json",
                    }),
                    //contentType: false,
                    //processData: false,
                    success: function (res) {
                        $("#lichhen-getList-container").html(res);
                        quanLyLichHen.lichHen.dataTable = new DataTableCustom({
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
                    }
                });
            },
            displayModal_CRUD: function (loai = "", idLichHen = '00000000-0000-0000-0000-000000000000') {
                var idLichHens = [];
                if (loai == "create") idLichHens.push(idLichHen);
                else {
                    if (idLichHen != '00000000-0000-0000-0000-000000000000')
                        idLichHens.push(idLichHen);
                    else {
                        quanLyLichHen.lichHen.dataTable.rows().iterator('row', function (context, index) {
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
                        url: "/QuanLyLichHen/displayModal_CRUD_LichHen",
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
                        IdLichHen: $("#input-idlichhen", $modal).val(),

                        IdBacSy: $("#select-bacsy", $modal).val(),
                        LoaiDieuTri: $("#select-loaidieutri", $modal).val(),
                        LoaiThoiGian: $("#select-loaithoigian", $modal).val(),
                        NgayHen: $("#input-ngayhen", $modal).val(),
                        ThoiGianBatDau: $("#input-thoigianbatdau", $modal).val(),
                        ThoiGianKetThuc: $("#input-thoigianketthuc", $modal).val(),
                        NoiDungKham: $("#input-noidungkham", $modal).val().trim(),
                        GhiChu: $("#input-ghichu", $modal).val().trim(),
                    }
                    sys.confirmDialog({
                        mess: `<p>Bạn có thực sự muốn thêm bản ghi này hay không ?</p>`,
                        callback: function () {
                            var formData = new FormData();
                            formData.append("lichHen", JSON.stringify(lichHen));
                            formData.append("loai", loai);

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: loai == "create" ? "/QuanLyLichHen/create_LichHen" : "/QuanLyLichHen/update_LichHen",
                                    type: "POST",
                                    data: formData,
                                }),
                                //contentType: "application/json; charset=utf-8",  // Chỉ định kiểu nội dung là JSON
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.status == "success") {
                                        quanLyLichHen.lichHen.getList();

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
                    quanLyLichHen.lichhen.dataTable.rows().iterator('row', function (context, index) {
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
                                    url: "/QuanLyLichHen/delete_LichHens",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    sys.alert({ status: res.status, mess: res.mess })
                                    quanLyLichHen.lichHen.getList();
                                }
                            })
                        }
                    })
                } else {
                    sys.alert({ mess: "Bạn chưa chọn bản ghi nào", status: "warning", timeout: 1500 })
                }
            },
        };
    }
};