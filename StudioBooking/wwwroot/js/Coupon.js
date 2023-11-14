"use strict";
function opencouponModal() {
    $('#mdlCouponBody').empty();
    $.get("/Admin/Coupon/CreateCouponModal")
        .done(function (res) {
            $('#mdlCouponBody').append(res);
            $("#kt_modal_coupon").modal('show');
            addFormValidation();
            KTCouponModal.init();
            $(".coupon-header").text("Add Coupon");
        })
        .fail(function (err) {
            toastr.error(err);
            console.log(err);
        });
}

function addFormValidation() {
    var i = FormValidation.formValidation($("#kt_modal_coupon_form")[0], {
        fields: {
            "Name": {
                validators: {
                    notEmpty: {
                        message: "Name is required"
                    }
                }
            },
            "Code": {
                validators: {
                    notEmpty: {
                        message: "Coupon code is required"
                    }
                }
            },
            //"DiscountType": {
            //    validators: {
            //        notEmpty: {
            //            message: "Discount Type is required"
            //        }
            //    }
            //},
            "Discount": {
                validators: {
                    notEmpty: {
                        message: "Discount is required"
                    }
                }
            }           
        },
        plugins: {
            trigger: new FormValidation.plugins.Trigger,
            bootstrap: new FormValidation.plugins.Bootstrap({
                rowSelector: ".fv-row",
                eleInvalidClass: "",
                eleValidClass: ""
            })
        }
    });
    const a = $('[data-kt-coupon-modal-action="submit"]')[0];
    a.addEventListener("click", (function (t) {
        t.preventDefault(),
            i && i.validate().then((function (s) {
                console.log("validated!"),
                    "Valid" == s ? (
                        $.ajaxSetup({
                            headers: {
                                'Content-Type': 'application/json',
                                'Accept': 'application/json'
                            }
                        }),
                        Swal.fire({
                            text: "Confirm to " + (parseInt($("#Id").val()) > 0 ? 'Edit' : 'Create') + " Coupon!",
                            icon: "warning",
                            showCancelButton: !0,
                            buttonsStyling: !1,
                            confirmButtonText: "Yes",
                            cancelButtonText: "No",
                            customClass: {
                                confirmButton: "btn fw-bold btn-success",
                                cancelButton: "btn fw-bold btn-dander"
                            }
                        }).then((function (t) {
                            if (t.isConfirmed) {
                                $.post("/Admin/Coupon/" + (parseInt($("#Id").val()) > 0 ? 'EditCoupon' : 'AddCoupon') + "", getCouponRequest()).done(function (res) {
                                    if (res.result) {
                                        $("#kt_datatable").KTDatatable().reload();
                                        $("#kt_modal_coupon").modal('hide');
                                        toastr.success("Coupon " + (parseInt($("#Id").val()) > 0 ? 'updated' : 'created') + " successfully.");
                                    }
                                    else {
                                        Swal.fire({
                                            text: "Sorry, looks like there are some errors detected, please try again.",
                                            icon: "error",
                                            buttonsStyling: !1,
                                            confirmButtonText: "Okay!",
                                            customClass: {
                                                confirmButton: "btn btn-primary"
                                            }
                                        })
                                    };
                                }, (err) => {
                                    console.log(err);
                                })
                            }
                        }
                        )))
                        : Swal.fire({
                            text: "Please enter data in all mandatory fields.",
                            icon: "error",
                            buttonsStyling: !1,
                            confirmButtonText: "Okay!",
                            customClass: {
                                confirmButton: "btn btn-primary"
                            }
                        })
            }));
    }
    ));
}

var KTCouponModal = function () {
    const r = document.getElementById("kt_modal_coupon")
    return {
        init: function () {
            (() => {
                let e = r.querySelector("#kt_modal_coupon_form");
                let n = new bootstrap.Modal(r);               
                r.querySelector('[data-kt-coupon-modal-action="close"]').addEventListener("click", (t => {
                    t.preventDefault();
                    $("#kt_modal_coupon").modal('hide');
                }
                ));
                r.querySelector('[data-kt-coupon-modal-action="cancel"]').addEventListener("click", (t => {
                    t.preventDefault();
                    $("#kt_modal_coupon").modal('hide');
                }
                ));
            }
            )()
        }
    }
}();


var CouponDataTable = {
    init: function () {
        var t;
        t = $("#kt_datatable").KTDatatable({
            data: {
                type: "remote",
                source: {
                    read: {
                        url: location.protocol + "//" + location.host + "/Admin/Coupon/GetCoupons",
                        method: 'GET',
                        map: function (t) {
                            var e = t;
                            return void 0 !== t.data && (e = t.data),
                                e;
                        }
                    }
                },
                pageSize: 10,
                serverPaging: !1,
                serverFiltering: !1,
                serverSorting: !1
            },
            layout: {
                scroll: true,
                footer: !1
            },
            order: [0],
            columnDefs: [{
                orderable: !1,
                targets: 9
            }, {
                orderable: !1,
                targets: 10
            }],
            noRecords: 'No records found',
            sortable: !0,
            pagination: !0,
            search: {
                input: $("#kt_datatable_search_query"),
                key: "generalSearch"
            },
            columns: [
                
                {
                    field: "id",
                    title: "ID",
                    sortable: "desc",
                    selector: !1,
                    autoHide: !1,
                    textAlign: "center",
                    template: function (t) {
                        return t.id;
                    }
                },
                {
                    field: "name",
                    title: "Name",
                    autoHide: !1,
                    template: function (t) {
                        return t.name;
                    }
                },
                {
                    field: "code",
                    title: "Coupon Code",
                    autoHide: !1,
                    template: function (t) {
                        return t.code;
                    }
                },               
                //{
                //    field: "discountType",
                //    title: "Discount Type",
                //    autoHide: !1,
                //    textAlign: "center",
                //    template: function (t) {
                //        var e = {
                //            "1": {
                //                title: "Amount",
                //                class: "info"
                //            },
                //            "2": {
                //                title: "Percentage",
                //                class: "info"
                //            }
                //        };
                //        return '<span class="label label-' + e[t.discountType].class + ' label-pill label-inline">' + e[t.discountType].title + '</span>';
                //    }
                //},
                {
                    field: "discount",
                    title: "Discount",
                    autoHide: !1,
                    template: function (t) {
                        return t.discount;
                    }
                },
                {
                    field: "isActive",
                    title: "Status",
                    autoHide: !1,
                    textAlign: "center",
                    template: function (t) {
                        var e = {                            
                            "true": {
                                title: "Activate",
                                class: "success"
                            },                           
                            "false": {
                                title: "Deactivate",
                                class: "danger"
                            }                            
                        };
                        return '<span class="label label-' + e[t.isActive].class + ' label-pill label-inline">' + e[t.isActive].title + '</span>';
                    }
                },                
                {
                    field: "Actions",
                    title: "",
                    sortable: !1,
                    width: 30,
                    autoHide: !1,
                    template: function (t) {
                        let html = '<div class="dropdown dropdown-inline dropleft" data-toggle="kt-tooltip" title="Actions" data-placement="left">';
                        html += '<a href="javascript:;" class="btn btn-light-primary btn-icon btn-sm" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
                        html += '<i class="ki ki-bold-more-ver"></i></a>';
                        html += '<div class="dropdown-menu dropdown-menu-sm">'; 
                        html += '<a href="javascript:;" class="dropdown-item" title="Edit Details" onClick="editCoupon(' + t.id + ')"><i class="flaticon2-edit"></i><span style="margin: 3px;">Edit</span></a>';
                        if (t.isActive) {
                            html += '<a href="javascript:;" class="dropdown-item" title="Deactivate"" onClick="updateCouponStatus(' + t.id + ',' + 0 + ')"><i class="flaticon2-check-mark"></i><span style="margin: 3px;">Deactivate</span></a>';
                                  }
                        if (!t.isActive) {
                            html += '<a href="javascript:;" class="dropdown-item" title="Activate"" onClick="updateCouponStatus(' + t.id + ',' + 1 + ')"><i class="flaticon2-check-mark"></i><span style="margin: 3px;">Activate</span></a>';
                        }
                        //html += '<a href="javascript:;" id="btnDelete" class="dropdown-item" data-target="' + t.booking.id + '" controller="Admin/Booking" title="Delete"><i class="flaticon2-trash"></i><span style="margin: 3px;">Delete</span></a>';
                        html += '</div></div>';
                        return html;
                    }
                }]
        });
    }   
};

function updateCouponStatus(id, status) {
    $.ajaxSetup({
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }
    }),
        Swal.fire({
            text: "Are you sure you want to " + (status === 1 ? 'Activate' : 'Deactivate') + " coupon?",
            icon: "warning",
            showCancelButton: !0,
            buttonsStyling: !1,
            confirmButtonText: "Yes",
            cancelButtonText: "No",
            customClass: {
                confirmButton: "btn fw-bold btn-success",
                cancelButton: "btn fw-bold btn-dander"
            }
        }).then((function (t) {
            if (t.isConfirmed) {
                $.get("/Admin/Coupon/UpdateCouponStatus?id=" + id + "&status=" + status)
                    .done(function (data) {
                        if (data.result) {
                            $("#kt_datatable").KTDatatable().reload();
                            toastr.success("Coupon " + (status === 1 ? 'Activate' : 'Deactivate') + " successfully.");
                        }
                        else {
                            toastr.error("Error: " + data.errorMsg);
                        }
                    })
                    .fail(function (error) {
                        toastr.error(defaultErrorMessage + error);
                    });
            }
        }));

}
function getCouponRequest() {      
       
    let request = {
        Id: $("#Id").val(),
        Name: $("#Name").val(),
        Code: $("#Code").val(),
       // DiscountType: document.querySelector('[name="DiscountType"]').value,
        Discount: $("#Discount").val()        
    }
    return JSON.stringify(request);
}
function editCoupon(id) {
    $('#mdlCouponBody').empty();
    $.get("/Admin/Coupon/CreateCouponModal/" + id)
        .done(function (res) {
            $('#mdlCouponBody').append(res);
            $("#kt_modal_coupon").modal('show');
            addFormValidation();
            KTCouponModal.init();
            $(".coupon-header").text("Edit Coupon");
            $("#Id").attr('disabled', 'disabled');
            $("#Name").attr('disabled', 'disabled');
            $("#Code").attr('disabled', 'disabled'); 
        })
        .fail(function (err) {
            toastr.error(err);
            console.log(err);
        });
}