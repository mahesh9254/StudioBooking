'use strict';
var KTModalAdjustBalance = function () {
    var t,
        e,
        n,
        o,
        a,
        r,
        i,
        l,
        p,
        c;
    return {
        init: function () {
            t = document.querySelector('#kt_modal_adjust_balance'),
                c = new bootstrap.Modal(t),
                p = document.querySelector('#btn_modal_adjust_balance'),
                p.addEventListener('click', (function (w) {
                    $.ajaxSetup({
                        headers: {
                            'Content-Type': 'application/json',
                            'Accept': 'application/json'
                        }
                    }),
                        $.get(location.protocol + "//" + location.host + "/Admin/Account/GetWalletBalanceUpdateModal?userId=" + w.target.getAttribute('data-id')).done(function (res) {
                            $('#kt_modal_adjust_balance_body').empty(),
                                $('#kt_modal_adjust_balance_body').append(res),
                                l = t.querySelector('#kt_modal_adjust_balance_form'),
                                e = l.querySelector('#kt_modal_adjust_balance_submit'),
                                n = l.querySelector('#kt_modal_adjust_balance_cancel'),
                                o = t.querySelector('#kt_modal_adjust_balance_close'),
                                Inputmask('9,999,999', {
                                    numericInput: !0
                                }).mask('#kt_modal_inputmask'),
                                //document.getElementById('adjustment').select2(),
                                function () {
                                    const e = t.querySelector('[kt-modal-adjust-balance="current_balance"]');
                                    i = t.querySelector('[kt-modal-adjust-balance="new_balance"]'),
                                        r = document.getElementById('kt_modal_inputmask');
                                    let n,
                                        o = parseFloat(e.innerHTML.replace(/[^0-9.]/g, '').replace(',', ''));
                                    r.addEventListener('focusout', (function (t) {
                                        n = parseFloat(t.target.value.replace(/[^0-9.]/g, '').replace(',', '')),
                                            isNaN(n) && (n = 0),
                                            i.innerHTML = (n + o).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,')
                                    }))
                                }(),
                                a = FormValidation.formValidation(l, {
                                    fields: {
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
                                            rowSelector: '.fv-row',
                                            eleInvalidClass: '',
                                            eleValidClass: ''
                                        })
                                    }
                                }),
                                $(l.querySelector('[name="adjustment"]')).on('change', (function () {
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
                                                        text: "Are you sure you want to update wallet balance?",
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
                                                            $.post("/Admin/Account/UpdateWallet", { __RequestVerificationToken: $('input[name="__RequestVerificationToken"]', l).val(), Id: $("#Id").val(), CustomerId: $("#CustomerId").val(), TransactionType: $("#adjustment").val(), Amount: parseFloat($("#kt_modal_inputmask").val().replace(/[^0-9.]/g, '').replace(',', '')), Description: $("#description").val() }).done(function (res) {
                                                                if (res.result) {
                                                                    h.isConfirmed && ($("#kt_modal_adjust_balance").hide(), e.disabled = !1, l.reset(), i.innerHTML = '--');
                                                                    Swal.fire({
                                                                        text: "Wallet coins balance adjusted successfully!",
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
                                            l.reset(); $("#kt_modal_adjust_balance").hide();
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
                                            l.reset(); $("#kt_modal_adjust_balance").hide();
                                        }))
                                }))
                        });
                    //t.show();
                }));
        }
    }
}();