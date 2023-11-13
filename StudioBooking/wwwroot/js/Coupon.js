"use strict";
function openCouponModal() {
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
            "DiscountType": {
                validators: {
                    notEmpty: {
                        message: "Discount Type is required"
                    }
                }
            },
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
                                $.post("/Admin/Booking/" + (parseInt($("#Booking_Id").val()) > 0 ? 'ScheduleChangeRequest' : 'AddBooking') + "", getBookingRequest()).done(function (res) {
                                    if (res.result) {
                                        $("#kt_datatable").KTDatatable().reload();
                                        $("#kt_modal_booking").modal('hide');
                                        toastr.success("Booking " + (parseInt($("#Booking_Id").val()) > 0 ? 'updated' : 'created') + " successfully.");
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
               
            }
            )()
        }
    }
}();