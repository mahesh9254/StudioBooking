'use strict';
var KTProfile = function () {
    var t,
        a,
        p;
    return {
        init: function () {
            t = document.querySelector('#kt_form'),
                p = document.querySelector('#kt_form_submit'),
                a = FormValidation.formValidation(t, {
                    fields: {
                        "User.FirstName": {
                            validators: {
                                notEmpty: {
                                    message: 'First name is required'
                                }
                            }
                        },
                        "User.LastName": {
                            validators: {
                                notEmpty: {
                                    message: 'Last name is required'
                                }
                            }
                        },
                        "User.Email": {
                            validators: {
                                notEmpty: {
                                    message: 'Email is required'
                                }
                            }
                        },
                        "User.Mobile": {
                            validators: {
                                notEmpty: {
                                    message: 'Mobile is required'
                                }
                            }
                        },
                        "Customer.AddressLine1": {
                            validators: {
                                notEmpty: {
                                    message: 'Address Line 1 is required'
                                }
                            }
                        },
                        "Customer.AddressLine2": {
                            validators: {
                                notEmpty: {
                                    message: 'Address Line 2 is required'
                                }
                            }
                        },
                        "Customer.Landmark": {
                            validators: {
                                notEmpty: {
                                    message: 'Landmark is required'
                                }
                            }
                        },  
                        "Customer.City": {
                            validators: {
                                notEmpty: {
                                    message: 'City is required'
                                }
                            }
                        },
                        "Customer.State": {
                            validators: {
                                notEmpty: {
                                    message: 'State is required'
                                }
                            }
                        },
                        "Customer.PinCode": {
                            validators: {
                                notEmpty: {
                                    message: 'Pincode is required'
                                }
                            }
                        }
                    },
                    plugins: {
                        trigger: new FormValidation.plugins.Trigger,
                        bootstrap: new FormValidation.plugins.Bootstrap5({
                            rowSelector: '.fv-row',
                            eleInvalidClass: '',
                            eleValidClass: ''
                        })
                    }
                }),
                p.addEventListener('click', (function (s) {
                    s.preventDefault(),
                        a && a.validate().then((function (g) {
                            console.log('validated!');
                            if ('Valid' == g) {
                                t.submit();
                            }
                        }))
                }));
        }
    }
}();