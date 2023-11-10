'use strict';
var KTModalAddOn = function () {
    var t,
        e,
        n,
        o,
        a,
        r,
        i,
        h,
        l,
        p,
        c;
    return {
        init: function () {
            t = document.querySelector('#kt_modal_addon'),
                c = new bootstrap.Modal(t),
                l = t.querySelector('#kt_modal_addon_form'),
                e = l.querySelector('#kt_modal_addon_submit'),
                n = l.querySelector('#kt_modal_addon_cancel'),
                o = t.querySelector('#kt_modal_addon_close'),
                h = t.querySelector('[name="adjustment"]'),
                function () {
                    const e = t.querySelector('[kt-modal-adjust-balance="current_balance"]');
                    i = t.querySelector('[kt-modal-adjust-balance="new_balance"]'),
                        r = document.getElementById('kt_modal_inputmask');
                    let n,
                        o = parseFloat(e.innerHTML.replace(/[^0-9.]/g, '').replace(',', ''));
                    r.addEventListener('focusout', (function (x) {
                        if (!h.value)
                            return
                        n = parseFloat(r.value.replace(/[^0-9.]/g, '').replace(',', '')),
                            isNaN(n) && (n = 0),
                            i.innerHTML = h.value == '1' ? (n + o).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,') : (o - n).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,')
                    })),
                        h.addEventListener('change', (function (x) {
                            if (!h.value)
                                return
                            n = parseFloat(r.value.replace(/[^0-9.]/g, '').replace(',', '')),
                                isNaN(n) && (n = 0),
                                i.innerHTML = h.value == '1' ? (n + o).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,') : (o - n).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,')
                        }))
                }(),
                a = FormValidation.formValidation(l, {
                    fields: {
                        adjustment: {
                            validators: {
                                notEmpty: {
                                    message: 'Adjustment type is required'
                                }
                            }
                        },
                        Name: {
                            validators: {
                                notEmpty: {
                                    message: 'Name is required'
                                }
                            }
                        },
                        Amount: {
                            validators: {
                                notEmpty: {
                                    message: 'Amount is required'
                                }
                            }
                        },
                        description: {
                            validators: {
                                notEmpty: {
                                    message: 'Description reason is required'
                                }
                            }
                        },
                    },
                    plugins: {
                        trigger: new FormValidation.plugins.Trigger,
                        bootstrap: new FormValidation.plugins.Bootstrap({
                            rowSelector: '.form-group',
                            eleInvalidClass: 'is-invalid',
                            eleValidClass: 'is-valid'
                        })
                    }
                }),
                $(t.querySelector('[name="adjustment"]')).on('change', (function () {
                    a.revalidateField('adjustment')
                })),
                e.addEventListener('click', (function (s) {
                    s.preventDefault(),
                        a && a.validate().then((function (g) {
                            console.log('validated!'),
                                'Valid' == g ? (e.disabled = !0,
                                    $.ajaxSetup({
                                        headers: {
                                            'Content-Type': 'application/x-www-form-urlencoded; charset=utf-8',
                                            'Accept': 'application/json'
                                        }
                                    }),
                                    Swal.fire({
                                        text: "Are you sure you want to add new booking addon?",
                                        icon: "warning",
                                        showCancelButton: !0,
                                        buttonsStyling: !1,
                                        confirmButtonText: "Yes",
                                        cancelButtonText: "No",
                                        customClass: {
                                            confirmButton: "btn fw-bold btn-success",
                                            cancelButton: "btn fw-bold btn-dander"
                                        }
                                    }).then((function (h) {
                                        e.removeAttribute("data-kt-indicator");
                                        e.disabled = !1;
                                        if (h.isConfirmed) {
                                            $.post("/Admin/Booking/AddBookingAddon", { __RequestVerificationToken: $('input[name="__RequestVerificationToken"]', l).val(), Id: $("#Id").val(), BookingId: $("#BookingId").val(), Name: $("#Name").val(), Amount: parseFloat($("#kt_modal_inputmask").val().replace(/[^0-9.]/g, '').replace(',', '')), Description: $("#description").val(), AdjustmentType: t.querySelector('[name="adjustment"]').value }).done(function (res) {
                                                if (res.result) {
                                                    h.isConfirmed && ($("#kt_modal_addon").hide(), e.disabled = !1, l.reset(), i.innerHTML = '--');
                                                    Swal.fire({
                                                        text: "Booking addon created successfully!",
                                                        icon: "success",
                                                        buttonsStyling: !1,
                                                        confirmButtonText: "Okay!",
                                                        customClass: {
                                                            confirmButton: "btn btn-primary"
                                                        }
                                                    }).then((function (h) { location.reload(); }));
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
                                        text: 'Sorry, looks like there are some errors detected, please try again.',
                                        icon: 'error',
                                        buttonsStyling: !1,
                                        confirmButtonText: 'Ok, got it!',
                                        customClass: {
                                            confirmButton: 'btn btn-primary'
                                        }
                                    })
                        }))
                })),
                n.addEventListener('click', (function (z) {
                    z.preventDefault(),
                        Swal.fire({
                            text: 'Are you sure you would like to cancel?',
                            icon: 'warning',
                            showCancelButton: !0,
                            buttonsStyling: !1,
                            confirmButtonText: 'Yes, cancel it!',
                            cancelButtonText: 'No, return',
                            customClass: {
                                confirmButton: 'btn btn-primary',
                                cancelButton: 'btn btn-active-light'
                            }
                        }).then((function (x) {
                            l.reset(); $("#kt_modal_addon").hide();
                        }))
                })),
                o.addEventListener('click', (function (z) {
                    z.preventDefault(),
                        Swal.fire({
                            text: 'Are you sure you would like to cancel?',
                            icon: 'warning',
                            showCancelButton: !0,
                            buttonsStyling: !1,
                            confirmButtonText: 'Yes, cancel it!',
                            cancelButtonText: 'No, return',
                            customClass: {
                                confirmButton: 'btn btn-primary',
                                cancelButton: 'btn btn-active-light'
                            }
                        }).then((function (x) {
                            l.reset(); $("#kt_modal_addon").hide();
                        }))
                }));
        }
    }
}();