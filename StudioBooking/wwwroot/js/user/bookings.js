"use strict";

var KTBookingList = function (name) {
    var e, o = document.getElementById(name);
    o && (o.querySelectorAll("tbody tr").forEach((e => {
        const t = e.querySelectorAll("td")
        const date = moment(t[0].innerHTML, "DD/MM/YYYY, LT").format();
        t[0].setAttribute("data-order", date);
        t[1].setAttribute("data-order", t[1].id);
    }
    )),
        (e = $(o).DataTable({
            info: 1,
            order: [0],
            pageLength: 10,
            lengthChange: !1,
            columnDefs: [{
                orderable: !1,
                targets: 9
            }, {
                orderable: !1,
                targets: 10
            }, {
                orderable: !1,
                targets: 4
            }, {
                orderable: !1,
                targets: 5
            }, {
                orderable: !1,
                targets: 6
            }]
        }))
    )
}

var KTReSchedule = function () {
    const r = document.getElementById("kt_modal_reschedule")
        , e = r.querySelector("#kt_modal_reschedule_form")
        , n = new bootstrap.Modal(r);
    return {
        init: function () {
            (() => {

                $("#RequestDate").flatpickr({
                    enableTime: false,
                    dateFormat: "d-m-Y",
                    enable: [
                        {
                            from: "today",
                            to: new Date().fp_incr(180) // 180 days from now
                        }
                    ]
                });
                $("#RequestEndDate").flatpickr({
                    enableTime: false,
                    dateFormat: "d-m-Y",
                    enable: [
                        {
                            from: "today",
                            to: new Date().fp_incr(180) // 180 days from now
                        }
                    ]
                });

                //$(".booking-date").datepicker({
                //    todayHighlight: !0,
                //    format: "dd-mm-yyyy",
                //    startDate: '+1d'
                //});
                var i = FormValidation.formValidation(e, {
                    fields: {
                        RequestDate: {
                            validators: {
                                notEmpty: {
                                    message: "Booking date is required"
                                }
                            }
                        },
                        RequestStartTime: {
                            validators: {
                                notEmpty: {
                                    message: "Start Time is required"
                                }
                            }
                        },
                        RequestEndTime: {
                            validators: {
                                notEmpty: {
                                    message: "End Time is required"
                                }
                            }
                        },
                        Description: {
                            validators: {
                                notEmpty: {
                                    message: "Reason is required"
                                }
                            }
                        }
                    },
                    plugins: {
                        trigger: new FormValidation.plugins.Trigger,
                        bootstrap: new FormValidation.plugins.Bootstrap5({
                            rowSelector: ".fv-row",
                            eleInvalidClass: "",
                            eleValidClass: ""
                        })
                    }
                });
                $(e.querySelector('[name="RequestDate"]')).on("change", (function () {
                    if (this.value) {
                        enableSubmit();
                        $.get("/Service/GetBookedStartEndTimeSlots?id=" + $("#CategoryId").val() + "&date=" + this.value + "&type=0").done((res) => {
                            var startTime = $("#hdfStartTime-" + $("#BookingId").val()).val();
                            var endTime = $("#hdfEndTime-" + $("#BookingId").val()).val();
                            var minHours = $("#hdfMinHours-" + $("#BookingId").val()).val();
                            // setTimeSlots(startTime, endTime, parseInt(minHours));
                            setStartTimeSlots("8:00 AM", "11:00 PM", minHours, res);
                            setEndTimeSlots(startTime, endTime, minHours, res);

                            $("#RequestStartTime").val(null).trigger('change');
                            $("#RequestEndTime").val(null).trigger('change');


                            $("#RequestEndDate").flatpickr({
                                enableTime: false,
                                dateFormat: "d-m-Y",
                                disable: [
                                    {
                                        from: new Date(new Date(this.value.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")).getTime() + (2 * 24 * 60 * 60 * 1000)),
                                        to: new Date().fp_incr(180) // 180 days from now
                                    }
                                ]
                            });

                            /*$("#RequestEndTime").datepicker('setEndDate', new Date(new Date(this.value.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")).getTime() + (1 * 24 * 60 * 60 * 1000)));*/
                            // $("#RequestEndDate").set('maxDate', new Date(this.value).fp_incr(-1))
                            // $("#RequestEndDate").set('maxDate', new Date(new Date(this.value.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")).getTime() + (1 * 24 * 60 * 60 * //1000)));
                            //$("#StartTime").val(null);
                            //$("#EndTime").val(null);
                            //let startTimeSlots = $('#RequestStartTime').find('option');
                            //let endTimeSlots = $('#RequestEndTime').find('option');
                            //startTimeSlots.removeClass('booked');
                            //endTimeSlots.removeClass('booked');
                            //$.each(res, function (i, e) {
                            //    let time = parseTime(e);
                            //    startTimeSlots.filter(function () {
                            //        return parseTime(this.innerText) == time;
                            //    }).addClass('booked').attr("disabled", "disabled");
                            //    endTimeSlots.filter(function () {
                            //        return parseTime(this.innerText) == time;
                            //    }).addClass('booked').attr("disabled", "disabled");
                            //});
                            i.revalidateField("RequestDate");
                        }).fail((err) => {
                            console.log(err);
                        });


                        $("#RequestEndDate").val(this.value)

                    }
                }
                )),
                    $(e.querySelector('[name="RequestEndDate"]')).on("change", (function () {

                        let sDate = new Date($("#RequestDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
                        let eDate = new Date($("#RequestEndDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
                        if (sDate <= eDate) {


                            if (this.value) {
                                enableSubmit();
                                $.get("/Service/GetBookedStartEndTimeSlots?id=" + $("#CategoryId").val() + "&date=" + this.value + "&type=0").done((res) => {
                                    var startTime = $("#hdfStartTime-" + $("#BookingId").val()).val();
                                    var endTime = $("#hdfEndTime-" + $("#BookingId").val()).val();
                                    var minHours = $("#hdfMinHours-" + $("#BookingId").val()).val();

                                    setEndTimeSlots(startTime, endTime, minHours, res);
                                    $("#RequestEndTime").val(null).trigger('change');;

                                    i.revalidateField("RequestDate");
                                }).fail((err) => {
                                    console.log(err);
                                });


                                $("#RequestEndDate").val(this.value)

                            }
                        }
                        else {

                            $("#RequestEndDate").val(null)
                        }
                    }
                    
                    )),
                    r.querySelector('[data-kt-users-modal-action="close"]').addEventListener("click", (t => {
                        t.preventDefault(),
                            Swal.fire({
                                text: "Are you sure you would like to cancel?",
                                icon: "warning",
                                showCancelButton: !0,
                                buttonsStyling: !1,
                                confirmButtonText: "Yes, cancel it!",
                                cancelButtonText: "No, return",
                                customClass: {
                                    confirmButton: "btn btn-primary",
                                    cancelButton: "btn btn-active-light"
                                }
                            }).then((function (t) {
                                t.value ? (e.reset(),
                                    n.hide()) : "cancel" === t.dismiss && Swal.fire({
                                        text: "Your booking has not been rescheduled!",
                                        icon: "error",
                                        buttonsStyling: !1,
                                        confirmButtonText: "Ok, got it!",
                                        customClass: {
                                            confirmButton: "btn btn-primary"
                                        }
                                    })
                            }
                            ))
                    }
                    )),
                    r.querySelector('[data-kt-users-modal-action="cancel"]').addEventListener("click", (t => {
                        t.preventDefault(),
                            Swal.fire({
                                text: "Are you sure you would like to cancel?",
                                icon: "warning",
                                showCancelButton: !0,
                                buttonsStyling: !1,
                                confirmButtonText: "Yes, cancel it!",
                                cancelButtonText: "No, return",
                                customClass: {
                                    confirmButton: "btn btn-primary",
                                    cancelButton: "btn btn-active-light"
                                }
                            }).then((function (t) {
                                t.value ? (e.reset(),
                                    n.hide()) : "cancel" === t.dismiss && Swal.fire({
                                        text: "Your booking has not been rescheduled!",
                                        icon: "error",
                                        buttonsStyling: !1,
                                        confirmButtonText: "Ok, got it!",
                                        customClass: {
                                            confirmButton: "btn btn-primary"
                                        }
                                    })
                            }
                            ))
                    }
                    ));
                const a = r.querySelector('[data-kt-users-modal-action="submit"]');
                a.disabled = !0;
                a.addEventListener("click", (function (t) {
                    t.preventDefault(),
                        i && i.validate().then((function (s) {
                            console.log("validated!"),
                                "Valid" == s ? (a.setAttribute("data-kt-indicator", "on"),
                                    a.disabled = !0,
                                    $.ajaxSetup({
                                        headers: {
                                            'Content-Type': 'application/json',
                                            'Accept': 'application/json'
                                        }
                                    }),
                                    Swal.fire({
                                        text: "Are you sure you want to rechedule booking?",
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
                                        a.removeAttribute("data-kt-indicator");
                                        a.disabled = !1;
                                        if (t.isConfirmed) {
                                            $.post("/User/Booking/ScheduleChangeRequest", getReScheduleRequest(1)).done(function (res) {
                                                if (res.result) {
                                                    if (res.data.redirectToPayment)
                                                        window.location.href = '/User/Booking/PayNow?id=' + res.data.bookingId + '&scheduleRequestId=' + res.data.id;
                                                    else {
                                                        t.isConfirmed && n.hide();
                                                        Swal.fire({
                                                            text: "Booking Rescheduled Successfully!",
                                                            icon: "success",
                                                            buttonsStyling: !1,
                                                            confirmButtonText: "Okay!",
                                                            customClass: {
                                                                confirmButton: "btn btn-primary"
                                                            }
                                                        }).then((function () {
                                                            location.reload();
                                                        }));
                                                    }
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
                        }
                        ))
                }
                ))
            }
            )()
        }
    }
}();
KTUtil.onDOMContentLoaded((function () {
    KTBookingList("kt_table_upcomingbookings");
    KTBookingList("kt_table_pastbookings");
    KTReSchedule.init();
}
));

function addFormValidation() {
    var i = FormValidation.formValidation($("#kt_modal_cancel_form")[0], {
        fields: {
            "PaymentProvider": {
                validators: {
                    notEmpty: {
                        message: "Payment Provider is required"
                    }
                }
            },
            "Name": {
                validators: {
                    notEmpty: {
                        message: "This field is required"
                    }
                }
            },
            "Account": {
                validators: {
                    notEmpty: {
                        message: "This field is required"
                    }
                }
            }
        },
        plugins: {
            trigger: new FormValidation.plugins.Trigger,
            bootstrap: new FormValidation.plugins.Bootstrap5({
                rowSelector: ".fv-row",
                eleInvalidClass: "",
                eleValidClass: ""
            })
        }
    });
    const a = $('[data-kt-cancel-modal-action="submit"]')[0];
    a.addEventListener("click", (function (t) {
        t.preventDefault(),
            i && i.validate().then((function (s) {
                console.log("validated!"),
                    "Valid" == s ? (
                        Swal.fire({
                            text: "Are you sure you want to cancel booking?",
                            icon: "warning",
                            showCancelButton: !0,
                            buttonsStyling: !1,
                            confirmButtonText: "Yes, cancel!",
                            cancelButtonText: "No, cancel",
                            customClass: {
                                confirmButton: "btn fw-bold btn-danger",
                                cancelButton: "btn fw-bold btn-primary"
                            }
                        }).then((function (t) {
                            t.value ? ($.ajaxSetup({
                                headers: {
                                    'Content-Type': 'application/json',
                                    'Accept': 'application/json'
                                }
                            }), $.post(location.protocol + "//" + location.host + "/User/Booking/ScheduleChangeRequest", JSON.stringify({
                                BookingId: parseInt($("#CancelBookingId").val()),
                                RequestType: 2,
                                PaymentProvider: $("#PaymentProvider").val(),
                                Name: $("#Name").val(),
                                Account: $("#Account").val(),
                                IFSC: $("#IFSC").val()
                            })).done(function (res) {
                                if (res.result) {
                                    Swal.fire({
                                        text: "Cancellation request created successfully and pending for approval.",
                                        icon: "success",
                                        buttonsStyling: !1,
                                        confirmButtonText: "Ok, got it!",
                                        customClass: {
                                            confirmButton: "btn fw-bold btn-primary"
                                        }
                                    }).then((function () {
                                        location.reload();
                                    }));
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
                            })) : "cancel" === t.dismiss && Swal.fire({
                                text: r + " was not cancelled.",
                                icon: "error",
                                buttonsStyling: !1,
                                confirmButtonText: "Ok, got it!",
                                customClass: {
                                    confirmButton: "btn fw-bold btn-primary"
                                }
                            })
                        }
                        ))
                    ) : Swal.fire({
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
    const b = $('[data-kt-cancel-modal-action="close"]')[0];
    b.addEventListener("click", (function (t) { $("#kt_modal_cancel_booking").modal("hide"); }));
    const c = $('[data-kt-cancel-modal-action="cancel"]')[0];
    c.addEventListener("click", (function (t) { $("#kt_modal_cancel_booking").modal("hide"); }));
}

function cancelBooking(id, status) {
    if (status !== 0) {
        $.get(location.protocol + "//" + location.host + "/User/Booking/GetCancelModal/" + parseInt(id)).done(function (res) {
            $('#mdlCancelBookingBody').empty();
            $('#mdlCancelBookingBody').append(res);
            $(".name").hide();
            $(".account").hide();
            $(".ifsc").hide();
            $("#kt_modal_cancel_booking").modal('show');
            addFormValidation();
        });
    }
    else {
        Swal.fire({
            text: "Are you sure you want to cancel booking?",
            icon: "warning",
            showCancelButton: !0,
            buttonsStyling: !1,
            confirmButtonText: "Yes, cancel!",
            cancelButtonText: "No, cancel",
            customClass: {
                confirmButton: "btn fw-bold btn-danger",
                cancelButton: "btn fw-bold btn-primary"
            }
        }).then((function (t) {
            t.value ? ($.ajaxSetup({
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                }
            }), $.post(location.protocol + "//" + location.host + "/User/Booking/ScheduleChangeRequest", JSON.stringify({
                BookingId: parseInt(id),
                RequestType: 2,
                PaymentProvider: null,
                Name: null,
                Account: null,
                IFSC: null
            })).done(function (res) {
                if (res.result) {
                    Swal.fire({
                        text: "Booking cancelled successfully.",
                        icon: "success",
                        buttonsStyling: !1,
                        confirmButtonText: "Ok, got it!",
                        customClass: {
                            confirmButton: "btn fw-bold btn-primary"
                        }
                    }).then((function () {
                        location.reload();
                    }));
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
            })) : "cancel" === t.dismiss && Swal.fire({
                text: "Booking was not cancelled.",
                icon: "error",
                buttonsStyling: !1,
                confirmButtonText: "Ok, got it!",
                customClass: {
                    confirmButton: "btn fw-bold btn-primary"
                }
            })
        }
        ))
    }
}

function reSchedule(id, categoryId) {
    resetReScheduleForm();
    $.get(location.protocol + "//" + location.host + "/User/Booking/GetBooking/" + id).then((res) => {
        if (res.booking) {
            var startTime = $("#hdfStartTime-" + id).val();
            var endTime = $("#hdfEndTime-" + id).val();
            var minHours = $("#hdfMinHours-" + id).val();
            // setTimeSlots(startTime, endTime, parseInt(minHours));

            $("#spnBookingId").text(res.booking.bookingId);
            $("#BookingId").val(id);
            $("#RequestType").val(1);
            $("#CategoryId").val(categoryId);
            $("#StartTime").val(res.booking.startTime);
            $("#EndTime").val(res.booking.endTime);
            $("#RequestStartTime").val(convertTime12to24(res.booking.startTime)).trigger('change');
            $("#RequestEndTime").val(convertTime12to24(res.booking.endTime)).trigger('change');
            $("#BookingDate").val(res.booking.bookingDate);
            $("#RequestDate").val(res.booking.bookingDate);//.trigger("change");
            $("#BookingEndDate").val(res.booking.bookingEndDate);
            $("#RequestEndDate").val(res.booking.bookingEndDate);//.trigger("change");
            $.get("/Service/GetBookedStartEndTimeSlots?id=" + $("#CategoryId").val() + "&date=" + $("#RequestDate").val() + "&type=0").done((res) => {
                var startTime = $("#hdfStartTime-" + $("#BookingId").val()).val();
                var endTime = $("#hdfEndTime-" + $("#BookingId").val()).val();
                var minHours = $("#hdfMinHours-" + $("#BookingId").val()).val();

                setStartTimeSlots("8:00 AM", "11:00 PM", minHours, res);
                setEndTimeSlotsValidation(startTime, endTime, 2, res);
                let startTimeSlots = $('#RequestStartTime').find('option');
                let endTimeSlots = $('#RequestEndTime').find('option');

            }).fail((err) => {
                console.log(err);
            });
            $("#RequestEndDate").flatpickr({
                enableTime: false,
                dateFormat: "d-m-Y",
                disable: [
                    {
                        from: new Date(new Date(res.booking.bookingDate.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")).getTime() + (2 * 24 * 60 * 60 * 1000)),
                        to: new Date().fp_incr(1180) // 180 days from now
                    }
                ]
            });
            $("#kt_modal_reschedule").modal("show");
        }
        else {
            Swal.fire({
                text: "Something went wrong!",
                icon: "error",
                buttonsStyling: !1,
                confirmButtonText: "Ok",
                customClass: {
                    confirmButton: "btn btn-danger"
                }
            });
        }
    });
}

function setTimeSlots(startTime, endTime, minhrs) {
    let sTime = parseTime(convertTime12to24(startTime));
    let eTime = parseTime(convertTime12to24(endTime));
    let timeSlots = calculate_time_slot(sTime, eTime, 60);
    let firstTimeSlot = "";
    var starttimedata = [];
    var endtimedata = [];
    $.each(timeSlots, function (i, e) {
        if (i === 0)
            firstTimeSlot = e.id;
        let disableStartTime = parseTime(convertTime12to24(endTime)) - parseTime(e.id) < (minhrs * 60);
        let disableEndTime = (parseTime(e.id) - parseTime(firstTimeSlot)) < (minhrs * 60);
        starttimedata.push({
            id: e.id,
            text: convertFrom24To12(e.id),
            disabled: disableStartTime
        });
        endtimedata.push({
            id: e.id,
            text: convertFrom24To12(e.id),
            disabled: disableEndTime
        });
        //$(".startTime").append('<button type="button" class="btn btn-sm font-weight-bold mr-2 mt-2 ' + (disableStartTime ? 'disabled btn-light-dark' : 'btn-rbstudio') + '">' + e.id + '</button>');
        //$(".endTime").append('<button type="button" class="btn btn-sm font-weight-bold mr-2 mt-2 ' + (disableEndTime ? 'disabled btn-light-dark' : 'btn-rbstudio') + '">' + e.id + '</button>');
    });
    if ($('#RequestStartTime').hasClass("select2-hidden-accessible")) $("#RequestStartTime").select2('destroy').empty().select2({ data: starttimedata }).val(convertTime12to24($("#StartTime").val())).trigger('change');
    else $("#RequestStartTime").empty().select2({ data: starttimedata }).val(convertTime12to24($("#StartTime").val())).trigger('change');

    if ($('#RequestEndTime').hasClass("select2-hidden-accessible")) $("#RequestEndTime").select2('destroy').empty().select2({ data: endtimedata }).val(convertTime12to24($("#EndTime").val())).trigger('change');
    else $("#RequestEndTime").empty().select2({ data: endtimedata }).val(convertTime12to24($("#EndTime").val())).trigger('change');
}


function setStartTimeSlots(startTime, endTime, minhrs, res) {
    let sTime = parseTime(convertTime12to24(startTime));
    let eTime = parseTime(convertTime12to24(endTime));
    let timeSlots = calculate_time_slot(sTime, eTime, 60);
    var starttimedata = [];
    $.each(timeSlots, function (i, e) {

        let disableStartTime = false;


        //let disableStartTime = ((parseTime(convertTime12to24(endTime)) - parseTime(e.id)) < (minhrs * 60));

        $.each(res, function (ri, re) {
            //if (parseTime(re) == parseTime(e.id)) {
            //    disableStartTime = true;
            //}

            let start = new Date(re.startDate.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            let end = new Date(re.endDate.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            let sDate = new Date($("#RequestDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            let eDate = new Date($("#RequestEndDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            if (start < sDate && end > eDate) {
                disableStartTime = true;
            }
            if (Date.parse(start) == Date.parse(sDate) && Date.parse(end) != Date.parse(eDate)) {

                if (parseTime(e.id) >= parseTime(re.start) && parseTime(e.id) <= eTime) {
                    disableStartTime = true;
                }
            }
            if (Date.parse(start) != Date.parse(sDate) && Date.parse(end) == Date.parse(eDate)) {

                if (parseTime(e.id) >= sTime && parseTime(e.id) <= parseTime(re.end)) {
                    disableStartTime = true;
                }
            }
            if (Date.parse(start) == Date.parse(sDate) && Date.parse(end) == Date.parse(eDate)) {

                if (parseTime(e.id) >= parseTime(re.start) && parseTime(e.id) <= parseTime(re.end)) {
                    disableStartTime = true;
                }
            }
        });

        starttimedata.push({
            id: e.id,
            text: convertFrom24To12(e.id),
            disabled: disableStartTime
        });

    });
    if ($('#RequestStartTime').hasClass("select2-hidden-accessible")) $("#RequestStartTime").select2('destroy').empty().select2({ data: starttimedata }).val(convertTime12to24($("#StartTime").val())).trigger('change');
    else $("#RequestStartTime").empty().select2({ data: starttimedata }).val(convertTime12to24($("#StartTime").val())).trigger('change');
}

function setEndTimeSlots(startTime, endTime, minhrs, eres) {

    let sTime = parseTime(convertTime12to24(startTime));
    let eTime = parseTime(convertTime12to24(endTime));
    let timeSlots = calculate_time_slot(sTime, eTime, 60);
    let firstTimeSlot = "";
    var endtimedata = [];
    $.each(timeSlots, function (i, e) {
        //if (i === 0)
        //    firstTimeSlot = e.id;
        let disableEndTime = false;


        // let disableEndTime = (parseTime(e.id) - parseTime(firstTimeSlot)) < (minhrs * 60);

        $.each(eres, function (ri, re) {
            let start = new Date(re.startDate.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            let end = new Date(re.endDate.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            let sDate = new Date($("#RequestDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            let eDate = new Date($("#RequestEndDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            if (start < sDate && end > eDate) {
                disableEndTime = true;
            }
            if (Date.parse(start) == Date.parse(sDate) && Date.parse(end) != Date.parse(eDate)) {

                if (parseTime(e.id) >= parseTime(re.start) && parseTime(e.id) <= eTime) {
                    disableEndTime = true;
                }
            }
            if (Date.parse(start) != Date.parse(sDate) && Date.parse(end) == Date.parse(eDate)) {

                if (parseTime(e.id) >= sTime && parseTime(e.id) <= parseTime(re.end)) {
                    disableEndTime = true;
                }
            }
            if (Date.parse(start) == Date.parse(sDate) && Date.parse(end) == Date.parse(eDate)) {

                if (parseTime(e.id) >= parseTime(re.start) && parseTime(e.id) <= parseTime(re.end)) {
                    disableEndTime = true;
                }
            }

        });
        endtimedata.push({
            id: e.id,
            text: convertFrom24To12(e.id),
            disabled: disableEndTime
        });
    });
    if ($('#RequestEndTime').hasClass("select2-hidden-accessible")) $("#RequestEndTime").select2('destroy').empty().select2({ data: endtimedata }).val(convertTime12to24($("#EndTime").val())).trigger('change');
    else $("#RequestEndTime").empty().select2({ data: endtimedata }).val(convertTime12to24($("#EndTime").val())).trigger('change');
}

function setEndTimeSlotsValidation(startTime, endTime, minhrs, eres) {

    let sTime = parseTime(convertTime12to24(startTime));
    let eTime = parseTime(convertTime12to24(endTime));
    let timeSlots = calculate_time_slot(sTime, eTime, 60);
    let staTime = parseTime(convertTime12to24($("#RequestStartTime").val()));
    let firstTimeSlot = "";
    var endtimedata = [];
    $.each(timeSlots, function (i, e) {
        //if (i === 0)
        //    firstTimeSlot = e.id;
        let disableEndTime = false;


        

        if (Date.parse(sDate) < Date.parse(eDate)) {
            disableEndTime = (parseTime(e.id) - staTime) >= 0;
        }
        else {
            disableEndTime = (parseTime(e.id) - staTime) < (minhrs * 60);
        }
        $.each(eres, function (ri, re) {
            let start = new Date(re.startDate.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            let end = new Date(re.endDate.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            let sDate = new Date($("#RequestDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            let eDate = new Date($("#RequestEndDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            if (start < sDate && end > eDate) {
                disableEndTime = true;
            }
            if (Date.parse(start) == Date.parse(sDate) && Date.parse(end) != Date.parse(eDate)) {

                if (parseTime(e.id) >= parseTime(re.start) && parseTime(e.id) <= eTime) {
                    disableEndTime = true;
                }
            }
            if (Date.parse(start) != Date.parse(sDate) && Date.parse(end) == Date.parse(eDate)) {

                if (parseTime(e.id) >= sTime && parseTime(e.id) <= parseTime(re.end)) {
                    disableEndTime = true;
                }
            }
            if (Date.parse(start) == Date.parse(sDate) && Date.parse(end) == Date.parse(eDate)) {

                if (parseTime(e.id) >= parseTime(re.start) && parseTime(e.id) <= parseTime(re.end)) {
                    disableEndTime = true;
                }
            }

        });
        endtimedata.push({
            id: e.id,
            text: convertFrom24To12(e.id),
            disabled: disableEndTime
        });
    });
    if ($('#RequestEndTime').hasClass("select2-hidden-accessible")) $("#RequestEndTime").select2('destroy').empty().select2({ data: endtimedata }).val(convertTime12to24($("#EndTime").val())).trigger('change');
    else $("#RequestEndTime").empty().select2({ data: endtimedata }).val(convertTime12to24($("#EndTime").val())).trigger('change');
}
$(document).on('change', '#PaymentProvider', function (e) {
    if (this.value === '') {
        $(".name").hide();
        $(".account").hide();
        $(".ifsc").hide();
    }
    else {
        if (this.value === '1') {
            $(".name").show();
            $(".account").show();
            $("#nameLabel").text("UPI ID");
            $("#Name").attr('placeholder', 'Enter UPI ID');
            $("#accountLabel").text("Phone Number");
            $("#Account").attr('placeholder', 'Enter Phone Number');
            $(".ifsc").hide();
        }
        else {
            $(".name").show();
            $(".account").show();
            $(".ifsc").show();
            $("#nameLabel").text("Bank Name");
            $("#Name").attr('placeholder', 'Enter Bank Name');
            $("#accountLabel").text("Account Number");
            $("#Account").attr('placeholder', 'Enter Account Number');
        }
    }
});

function disableTimeSlots(startTime) {
    var minInterval = $("#hdfMinHours-" + $("#BookingId").val()).val();
    var targets = $('#RequestEndTime').find('option');
    $.each(targets, function (i, e) {
        if (!e.classList.length)
            $(e).removeAttr('disabled');
        else {
            if (checkBookingConflicts($('#RequestStartTime').find('option:selected')[0].index, e.index))
                $(e).removeAttr('disabled');
        }
        let endTime = parseTime(convertTime12to24(e.textContent));
        let disable = endTime < (parseTime(startTime) + (parseInt(minInterval) * 60));
        if (disable)
            $(e).attr('disabled', 'disabled');
    });
    if ($("#RequestEndTime").val())
        if ($('#RequestStartTime').find('option:selected')[0].index > $('#RequestEndTime').find('option:selected')[0].index) $("#RequestEndTime").val(null);
    if ($('#RequestEndTime').hasClass("select2-hidden-accessible")) $("#RequestEndTime").trigger('change');
    else $("#RequestEndTime").trigger('change');
}

$(document).on('select2:select', '#RequestStartTime', function (e) {
    if (e.target.options[e.target.options.selectedIndex + (parseInt($("#hdfMinHours-" + $("#BookingId").val()).val()) - 1)].className == 'booked' && e.target.options.selectedIndex != -1) {//$(this).hasClass('booked')
        Swal.fire({
            text: "Selected timeslot conflicts with booked timeslot. Please select another timeslots",
            icon: "warning",
            buttonsStyling: !1,
            confirmButtonText: "Ok",
            customClass: {
                confirmButton: "btn fw-bold btn-primary"
            }
        });
        e.preventDefault();
        $("#RequestStartTime").val(convertTime12to24($("#StartTime").val())).trigger('change');
        return false;
    }
    if (e.target.options.selectedIndex != -1)
        disableTimeSlots(e.target.options[e.target.options.selectedIndex].value);
    $("#RequestEndTime").val(null).trigger('change');
});

$(document).on('select2:select', '#RequestEndTime', function (e) {
    var minInterval = $("#hdfMinHours-" + $("#BookingId").val()).val();
    let endTime = parseTime($('#RequestEndTime').val());
    if (endTime >= (parseTime($('#RequestStartTime').val()) + (parseInt(minInterval) * 60))) {
        if (e.target.options.selectedIndex != -1) {//$(this).hasClass('booked')
            //e.target.options[e.target.options.selectedIndex - 1].className == 'booked'        
            if (!checkBookingConflicts($('#RequestStartTime').find('option:selected')[0].index, e.target.options.selectedIndex)) {
                Swal.fire({
                    text: "Selected timeslot conflicts with booked timeslot. Please select another timeslots",
                    icon: "warning",
                    buttonsStyling: !1,
                    confirmButtonText: "Ok",
                    customClass: {
                        confirmButton: "btn fw-bold btn-primary"
                    }
                });
                e.preventDefault();
                $("#RequestEndTime").val(convertTime12to24($("#EndTime").val())).trigger('change');
                return false;
            }
        }
    } else {
        Swal.fire({
            text: "Selected timeslots is less than booked timeslot interval. Please select another timeslots",
            icon: "warning",
            buttonsStyling: !1,
            confirmButtonText: "Ok",
            customClass: {
                confirmButton: "btn fw-bold btn-primary"
            }
        }); $("#RequestEndTime").val(null).trigger('change');
    }
});

$(document).on('change', '#RequestEndTime', function (e) {
    enableSubmit();
});

$(document).on('change', '#RequestStartTime', function (e) {


    $.get("/Service/GetBookedStartEndTimeSlots?id=" + $("#CategoryId").val() + "&date=" + $("#RequestDate").val() + "&type=0").done((res) => {
        var startTime = $("#hdfStartTime-" + $("#BookingId").val()).val();
        var endTime = $("#hdfEndTime-" + $("#BookingId").val()).val();        
        setEndTimeSlotsValidation(startTime, endTime, 2, res);
        let startTimeSlots = $('#RequestStartTime').find('option');
        let endTimeSlots = $('#RequestEndTime').find('option');

    }).fail((err) => {
        console.log(err);
    });

    enableSubmit();
});

function checkBookingConflicts(start, end) {
    let endTimeSlots = $('#RequestEndTime').find('option');
    let isValid = true;
    for (var i = start; i < end; i++) {
        if (endTimeSlots[i].className == 'booked')
            isValid = false;
    }
    return isValid;
}

function enableSubmit() {
    const r = document.getElementById("kt_modal_reschedule")
    const a = r.querySelector('[data-kt-users-modal-action="submit"]');
    var disabled = true;
    if (convertTime12to24($("#StartTime").val()) !== $("#RequestStartTime").val() && ($("#StartTime").val() && $("#RequestStartTime").val()))
        disabled = false;
    if (convertTime12to24($("#EndTime").val()) !== $("#RequestEndTime").val() && ($("#EndTime").val() && $("#RequestEndTime").val()))
        disabled = false;
    if (($("#BookingDate").val() !== $("#RequestDate").val()) && $("#RequestDate").val())
        disabled = false;
    a.disabled = disabled;
}

function resetReScheduleForm() {
    $("#spnBookingId").text(null);
    $("#BookingId").val(0);
    $("#RequestType").val(1);
    $("#CategoryId").val(0);
    $("#StartTime").val(null);
    $("#EndTime").val(null);
    $("#RequestStartTime").val(null).trigger('change');
    $("#RequestEndTime").val(null).trigger('change');
    $("#RequestDate").val(null).trigger("change");
    enableSubmit();
}

function getReScheduleRequest(type) {
    //var requestDate = $("#RequestDate").val().split('/');
    var data = {
        bookingId: $("#BookingId").val(),
        requestType: type,
        //requestDate: [requestDate[2], requestDate[1], requestDate[0]].join('-'),
        requestDate: $("#RequestDate").val(),
        requestStartTime: $("#RequestStartTime").val(),
        requestEndTime: $("#RequestEndTime").val(),
        requestEndDate: $("#RequestEndDate").val()
    };
    return JSON.stringify(data);
}

function exportAllBooking() {
    window.open("/Admin/Booking/ExportAllBooking");
}