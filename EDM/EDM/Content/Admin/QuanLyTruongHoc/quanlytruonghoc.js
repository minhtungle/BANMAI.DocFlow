'use strict'
/**
 * main
 * */
class QuanLyTruongHoc {
    constructor() {
        this.page;
        this.pageGroup;
        this.truongHoc = {}
    }
    init() {
        var quanLyTruongHoc = this;
        quanLyTruongHoc.page = $("#page-quanlytruonghoc");
        htmlEl = new HtmlElement();

        quanLyTruongHoc.truongHoc = {
            ...quanLyTruongHoc.truongHoc,
            dataTable: null,
            getList: function () {
                var $timKiem = $("#truonghoc-timkiem-collapse");
                var input = {
                    LocThongTin: {
                        MaTruongHoc: $("#select-matruonghoc", $timKiem).val(),
                        TenTruongHoc: $("#input-tentruonghoc", $timKiem).val().trim(),
                        Email: $("#input-email", $timKiem).val().trim(),
                        SoDienThoai: $("#input-sodienthoai", $timKiem).val().trim(),
                        DiaChi: $("#input-diachi", $timKiem).val().trim(),
                        GhiChu: $("#input-ghichu", $timKiem).val().trim(),
                    }
                };
                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyTruongHoc/getList_TruongHoc",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: { input }
                    }),
                    success: function (res) {
                        $("#truonghoc-getList-container").html(res);
                        quanLyTruongHoc.truongHoc.dataTable = new DataTableCustom({
                            name: "truonghoc-getList",
                            table: $("#truonghoc-getList"),
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
            displayModal_CRUD: function (loai = "", idTruongHoc = '00000000-0000-0000-0000-000000000000') {
                var idTruongHocs = [];
                if (loai == "create") idTruongHocs.push(idTruongHoc);
                else {
                    if (idTruongHoc != '00000000-0000-0000-0000-000000000000')
                        idTruongHocs.push(idTruongHoc);
                    else {
                        quanLyTruongHoc.truongHoc.dataTable.rows().iterator('row', function (context, index) {
                            var $row = $(this.row(index).node());
                            if ($row.has("input.checkRow-truonghoc-getList:checked").length > 0) {
                                idTruongHocs.push($row.attr('id'));
                            };
                        });
                        if (idTruongHocs.length != 1) {
                            sys.alert({ mess: "Yêu cầu chọn 1 bản ghi", status: "warning", timeout: 1500 });
                            return;
                        }
                    }
                };
                var input = {
                    Loai: loai,
                    IdTruongHoc: idTruongHocs[0],
                };

                $.ajax({
                    ...ajaxDefaultProps({
                        url: "/QuanLyTruongHoc/displayModal_CRUD_TruongHoc",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: { input },
                    }),
                    success: function (res) {
                        $("#truonghoc-crud").html(res);
                        sys.displayModal({
                            name: '#truonghoc-crud'
                        });
                    }
                })
            },
            save: function (loai) {
                var modalValidtion = htmlEl.activeValidationStates("#truonghoc-crud");
                if (modalValidtion) {
                    let $modal = $("#truonghoc-crud");
                    var truongHoc = {
                        TruongHoc: {
                            IdTruongHoc: $("#input-idtruonghoc", $modal).val(),
                            TenTruongHoc: $("#input-tentruonghoc", $modal).val().trim(),
                            TenVietTat: $("#input-tenviettat", $modal).val().trim(),
                            Slug: $("#input-slug", $modal).val().trim(),
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
                            formData.append("truongHoc", JSON.stringify(truongHoc));
                            formData.append("loai", loai);

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: loai == "create" ? "/QuanLyTruongHoc/create_TruongHoc" : "/QuanLyTruongHoc/update_TruongHoc",
                                    type: "POST",
                                    data: formData,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.status == "success") {
                                        quanLyTruongHoc.truongHoc.getList();

                                        sys.displayModal({
                                            name: '#truonghoc-crud',
                                            displayStatus: "hide"
                                        });
                                        sys.alert({ status: res.status, mess: res.mess });
                                    } else {
                                        if (res.status == "warning") {
                                            htmlEl.inputValidationStates(
                                                $("#input-tentruonghoc"),
                                                "#truonghoc-crud",
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
            delete: function (loai, idTruongHoc = '00000000-0000-0000-0000-000000000000') {
                var idTruongHocs = [];
                if (loai == "single") {
                    idTruongHocs.push(idTruongHoc)
                } else {
                    quanLyTruongHoc.truongHoc.dataTable.rows().iterator('row', function (context, index) {
                        var $row = $(this.row(index).node());
                        if ($row.has("input.checkRow-truonghoc-getList:checked").length > 0) {
                            idTruongHocs.push($row.attr('id'));
                        };
                    });
                };
                if (idTruongHocs.length > 0) {
                    var f = new FormData();
                    f.append("idTruongHocs", JSON.stringify(idTruongHocs));
                    sys.confirmDialog({
                        mess: `Bạn có thực sự muốn xóa bản ghi này hay không ?`,
                        callback: function () {
                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyTruongHoc/delete_TruongHocs",
                                    type: "POST",
                                    data: f,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    sys.alert({ status: res.status, mess: res.mess })
                                    quanLyTruongHoc.truongHoc.getList();
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
                    var truongHoc = {
                        TruongHoc: {
                            IdTruongHoc: $("#input-idtruonghoc", $tab).val(),
                            TenTruongHoc: $("#input-tentruonghoc", $tab).val().trim(),
                            TenVietTat: $("#input-tenviettat", $tab).val().trim(),
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
                            formData.append("truongHoc", JSON.stringify(truongHoc));

                            $.ajax({
                                ...ajaxDefaultProps({
                                    url: "/QuanLyTruongHoc/update_TruongHoc",
                                    type: "POST",
                                    data: formData,
                                }),
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.status == "success") {
                                        sys.alert({ status: res.status, mess: res.mess });
                                        setTimeout(() => {
                                            quanLyTruongHoc.truongHoc.xemChiTiet.showTab();
                                        }, 1000);
                                    } else {
                                        if (res.status == "warning") {
                                            htmlEl.inputValidationStates(
                                                $("#input-tentruonghoc"),
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