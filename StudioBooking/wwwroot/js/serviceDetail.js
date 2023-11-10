﻿
$("#Cart_BookingDate").change(function (e) {
    var selectedService = $('input[name="bookingstudio"]:checked').val();
    if (!selectedService) {
        Swal.fire({
            text: "Please select studio for booking!",
            icon: "error",
            buttonsStyling: !1,
            confirmButtonText: "Ok, got it!",
            customClass: {
                confirmButton: "btn font-weight-bold btn-light"
            }
        });
        $("#Cart_BookingDate").val(null);
        e.preventDefault();
        return false;
    }
    KTbooking.validate();
    $(".endTime").children('button').removeClass('selected');
    $(".startTime").children('button').removeClass('selected');
    $(".dv-end-time").hide();
    $(".dv-start-time").show();
    $.get("/Service/GetBookedTimeSlots/" + $('input[name="bookingstudio"]:checked').attr("categoryId") + "?date=" + $("#Cart_BookingDate").val()).done((res) => {
        let start_time = $('input[name="bookingstudio"]:checked').attr('start-time');
        let end_time = $('input[name="bookingstudio"]:checked').attr('end-time');
        let minInterval = $('input[name="bookingstudio"]:checked').attr('min-hours');
        setTimeSlots(start_time, end_time, minInterval);
        let startTimeSlots = $(".startTime").children('button');
        let endTimeSlots = $(".endTime").children('button');
        startTimeSlots.removeClass('booked');
        endTimeSlots.removeClass('booked');
        startTimeSlots.not(".disabled").removeClass('btn-light-dark').addClass("btn-light-success");
        endTimeSlots.not(".disabled").removeClass('btn-light-dark').addClass("btn-light-success")
        $.each(res, function (i, e) {
            let time = parseTime(e);//parseTime(convertTime12to24(e));
            startTimeSlots.filter(function () {
                return parseTime(convertTime12to24(this.innerText)) == time;
            }).removeClass('btn-light-success').addClass('booked btn-light-dark');
            endTimeSlots.filter(function () {
                return parseTime(convertTime12to24(this.innerText)) == time;
            }).removeClass('btn-light-success').addClass('booked btn-light-dark');;
        });
    }).fail((err) => {
        console.log(err);
    });
})

$('input[name="bookingstudio"]').change(function () {
    //var start_time = $(this).attr('start-time');
    //var end_time = $(this).attr('end-time');
    minInterval = $(this).attr('min-hours');
    $("#Cart_ServicePriceId").val($('input[name="bookingstudio"]:checked').val());
    $("#dvServiceName").empty().text($(this).attr('service-name'));
    $("#dvStudioName").empty().text($(this).attr('category'));
    $("#dvStudioTitle").empty().text($(this).attr('title'));
    $(".dv-end-time").hide();
    $(".dv-start-time").hide();
    $("#Cart_BookingDate").val(null);
    //setTimeSlots(start_time, end_time, minInterval);
});

function AttachTimeSlots(timeSlots) {
    $(".startTime").empty();
    $(".endTime").empty();
    $.each(timeSlots, function (i, e) {
        //$(".startTime").append('<a href="javascript:;">' + e.id + '</a>');
        //$(".endTime").append('<a href="javascript:;">' + e.id + '</a>');
        $(".startTime").append('<li>' + e.id + '</li>');
        $(".endTime").append('<li>' + e.id + '</li>');
    });
}

function setTimeSlots(startTime, endTime, minhrs) {
    let sTime = parseTime(convertTime12to24(startTime));
    let eTime = parseTime(convertTime12to24(endTime));
    let timeSlots = calculate_time_slot(sTime, eTime, 60);
    let firstTimeSlot = "";
    $("#Cart_StartTime").val(null);
    $("#Cart_EndTime").val(null);
    $(".startTime").empty();
    $(".endTime").empty();
    $.each(timeSlots, function (i, e) {
        if (i === 0)
            firstTimeSlot = e.id;
        let disableStartTime = parseTime(convertTime12to24(endTime)) - parseTime(e.id) < (minhrs * 60);
        let disableEndTime = (parseTime(e.id) - parseTime(firstTimeSlot)) < (minhrs * 60);
        $(".startTime").append('<button type="button" class="btn btn-md font-weight-bold mr-2 mt-2 ' + (disableStartTime ? 'disabled btn-light-dark' : 'btn-light-success') + '">' + convertFrom24To12(e.id) + '</button>');
        $(".endTime").append('<button type="button" class="btn btn-md font-weight-bold mr-2 mt-2 ' + (disableEndTime ? 'disabled btn-light-dark' : 'btn-light-success') + '">' + convertFrom24To12(e.id) + '</button>');
    });
}



function disableTimeSlots(startTime) {
    $("#Cart_EndTime").val(null);
    var minInterval = $('input[name="bookingstudio"]:checked').attr('min-hours');
    var targets = $(".endTime").children('button');
    $.each(targets, function (i, e) {
        if (!e.getAttribute('class').includes('booked'))
            $(e).removeClass('disabled');
        else {
            if (checkBookingConflicts(convertTime12to24(startTime), i))
                $(e).removeClass('disabled').removeClass('booked').removeClass('btn-light-dark').addClass("btn-light-success");
        }
        let endTime = parseTime(convertTime12to24(e.textContent));
        let disable = endTime < (parseTime(convertTime12to24(startTime)) + (parseInt(minInterval) * 60));
        if (disable)
            $(e).addClass('disabled');
    });
}

function checkBookingConflicts(startTime, endIndex) {
    let timeSlots = $(".startTime").children('button');
    let startIndex = 0;
    for (startIndex = timeSlots.length; startIndex-- > 0 && convertTime12to24(timeSlots[startIndex].innerText) !== convertTime12to24(startTime););
    let isValid = true;
    for (var i = startIndex; i < endIndex; i++) {
        if (timeSlots[i].className.includes("booked") && isValid)
            isValid = false;
    }
    return isValid;
}

$(document).on('click', '.startTime > button', function (e) {
    if ($(this).hasClass('booked')) {
        e.preventDefaults();
        return false;
    }
    var target = $(this).parent().children('button');
    $(target).removeClass('selected');
    $(this).toggleClass('selected');
    $("#Cart_StartTime").val($(this).text());
    KTbooking.validate();
    $(".dv-end-time").show();
    disableTimeSlots($(this).text());
});

$(document).on('click', '.endTime > button', function (e) {
    let selectedSlot = $(this).text();
    let timeSlots = $(".startTime").children('button');
    let target = $(this).parent().children('button');
    $(target).removeClass('selected');
    let endIndex = 0;
    for (endIndex = timeSlots.length; endIndex-- > 0 && convertTime12to24(timeSlots[endIndex].innerText) !== convertTime12to24(selectedSlot););
    if (checkBookingConflicts($("#Cart_StartTime").val(), endIndex)) {
        $(this).toggleClass('selected');
        $("#Cart_EndTime").val(selectedSlot);
    }
    else {
        Swal.fire({
            text: "Sorry, Selected time slots conflicts with another booking. Please select different timeslot!",
            icon: "error",
            buttonsStyling: !1,
            confirmButtonText: "Ok, got it!",
            customClass: {
                confirmButton: "btn font-weight-bold btn-light"
            }
        });
    }
    KTbooking.validate();
});

"use strict";
var KTbooking = function () {
    var t = "spinner spinner-right spinner-white pr-15";
    e = [];
    return {
        init: function () {
            var t, i = KTUtil.getById("kt_booking"), o = KTUtil.getById("kt_booking_form");
            o && (e.push(FormValidation.formValidation(o, {
                fields: {
                    "bookingstudio": {
                        validators: {
                            notEmpty: {
                                message: "Booking Studio is required"
                            }
                        }
                    },
                    "Cart.ServiceId": {
                        validators: {
                            notEmpty: {
                                message: "Service selection is required"
                            }
                        }
                    },
                    "Cart.StartTime": {
                        validators: {
                            notEmpty: {
                                message: "Start Time is required"
                            }
                        }
                    },
                    "Cart.EndTime": {
                        validators: {
                            notEmpty: {
                                message: "End Time is required"
                            }
                        }
                    },
                    "Cart.BookingDate": {
                        validators: {
                            notEmpty: {
                                message: "Booking Date is required"
                            }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger,
                    bootstrap: new FormValidation.plugins.Bootstrap({
                        eleValidClass: ""
                    })
                }
            })),
                (t = new KTWizard(i, {
                    startStep: 1,
                    clickableSteps: !1
                })).on("change", (function (t) {
                    if (t.getStep() === 1) {
                        t.goTo(t.getNewStep() - 1);
                    }
                    else if (!(t.getStep() > t.getNewStep())) {
                        var i = e[0];
                        return i && i.validate().then((function (i) {
                            "Valid" == i ? (t.goTo(t.getNewStep()),
                                $("#dvBookingDate").empty().text($("#Cart_BookingDate").val()),
                                $("#dvBookingTime").empty().text($("#Cart_StartTime").val() + " To " + $("#Cart_EndTime").val()),
                                $("#dvBookingHrs").empty().text(Math.floor((parseTime(convertTime12to24($("#Cart_EndTime").val())) - parseTime(convertTime12to24($("#Cart_StartTime").val()))) / 60) + "hrs"),
                                KTUtil.scrollTop()) : Swal.fire({
                                    text: "Sorry, looks like there are some errors detected, please try again.",
                                    icon: "error",
                                    buttonsStyling: !1,
                                    confirmButtonText: "Ok, got it!",
                                    customClass: {
                                        confirmButton: "btn font-weight-bold btn-light"
                                    }
                                }).then((function () {
                                    KTUtil.scrollTop()
                                }
                                ))
                        }
                        )),
                            !1
                    }
                }
                )),
                t.on("changed", (function (t) {
                    KTUtil.scrollTop()
                }
                )),
                t.on("submit", (function (t) {
                    Swal.fire({
                        html: "This is a non-cancellable booking. Please check our <a href='https://rbstudios.info/Home/Privacy' target='_blank'>cancellation and rescheduling policy</a>. Do you want to acknowledge and proceed to checkout ?",
                        icon: "success",
                        showCancelButton: !0,
                        buttonsStyling: !1,
                        confirmButtonText: "Yes, submit!",
                        cancelButtonText: "No, cancel",
                        customClass: {
                            confirmButton: "btn font-weight-bold btn-primary",
                            cancelButton: "btn font-weight-bold btn-default"
                        }
                    }).then((function (t) {
                        t.value ? o.submit() : "cancel" === t.dismiss
                    }
                    ))
                }
                )))
        },
        validate: function () {
            return e[0].validate();
        }
    }
}();
jQuery(document).ready((function () {
    KTbooking.init()
}
));