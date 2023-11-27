
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
    $("#Cart_BookingEndDate").val(this.value);
    //KTbooking.validate();
    $(".endTime").children('button').removeClass('selected');
    $(".startTime").children('button').removeClass('selected');
   // $(".dv-end-time").hide();
   // $(".dv-start-time").show();
    $.get("/Service/GetBookedStartEndTimeSlots?id=" + $('input[name="bookingstudio"]:checked').attr("categoryId") + "&date=" + $("#Cart_BookingDate").val() + "&endDate=" + $("#Cart_BookingEndDate").val()).done((res) => {
        let start_time = $('input[name="bookingstudio"]:checked').attr('start-time');
        let end_time = $('input[name="bookingstudio"]:checked').attr('end-time');
        let minInterval = $('input[name="bookingstudio"]:checked').attr('min-hours');
        setStartTimeSlots("8:00 AM", "11:00 PM", minInterval,  res);
        setEndTimeSlots(start_time, end_time, minInterval, res);
       
        //console.log(new Date(new Date(this.value.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")).getTime() + (5 * 24 * 60 * 60 * 1000)));
        $("#Cart_BookingEndDate").datepicker('setEndDate', new Date(new Date(this.value.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")).getTime() + (1 * 24 * 60 * 60 * 1000)));
        $("#Cart_BookingEndDate").datepicker('setStartDate', new Date(new Date(this.value.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3")).getTime()));
        //$.each(res, function (i, e) {
        //    let time = parseTime(e);//parseTime(convertTime12to24(e));
        //    startTimeSlots.filter(function () {
        //        return parseTime(convertTime12to24(this.innerText)) == time;
        //    }).removeClass('btn-light-success').addClass('booked btn-light-dark');
        //    endTimeSlots.filter(function () {
        //        return parseTime(convertTime12to24(this.innerText)) == time;
        //    }).removeClass('btn-light-success').addClass('booked btn-light-dark');;
        //});
    }).fail((err) => {
        console.log(err);
    });
})

$("#Cart_BookingEndDate").change(function (e) { 
    $.get("/Service/GetBookedStartEndTimeSlots?id=" + $('input[name="bookingstudio"]:checked').attr("categoryId") + "&date=" + $("#Cart_BookingEndDate").val()).done((res) => {               
        let start_time = $('input[name="bookingstudio"]:checked').attr('start-time');
        let end_time = $('input[name="bookingstudio"]:checked').attr('end-time');
        let minInterval = $('input[name="bookingstudio"]:checked').attr('min-hours');
        //let sDate = $("#Cart_BookingDate").val();
        //let eDate = $("#Cart_BookingEndDate").val();
        //if (sDate == eDate) {
        setEndTimeSlotsValidation(start_time, end_time, 2, res);
        //}
        //else {
        //    setEndTimeSlots(start_time, end_time, 0, res);
        //}
    }).fail((err) => {
        console.log(err);
    });
})

$("#StartTime").change(function (e) {
    if (this.value) {
        let sDate = $("#Cart_BookingDate").val();
        let eDate = $("#Cart_BookingEndDate").val();
        let sTime = parseTime(this.value);
        let eTime = parseTime($("#Cart_EndTime").val());
        if (sDate == eDate && eTime) {
            if (sTime < eTime) $("#Cart_StartTime").val(this.value)
            else { $("#Cart_StartTime").val(null); $("#StartTime").val(null) }
        }
        else {
            $("#Cart_StartTime").val(this.value);
        }
    }
    else { $("#Cart_StartTime").val(null); $("#StartTime").val(null); }


    $.get("/Service/GetBookedStartEndTimeSlots?id=" + $('input[name="bookingstudio"]:checked').attr("categoryId") + "&date=" + $("#Cart_BookingEndDate").val()).done((res) => {
        let start_time = $('input[name="bookingstudio"]:checked').attr('start-time');
        let end_time = $('input[name="bookingstudio"]:checked').attr('end-time');    
        setEndTimeSlotsValidation(start_time, end_time, 2, res);
      
    }).fail((err) => {
        console.log(err);
    });
})
$("#EndTime").change(function (e) {
    if (this.value) {
        let sDate = $("#Cart_BookingDate").val();
        let eDate = $("#Cart_BookingEndDate").val();
        let sTime = parseTime($("#Cart_StartTime").val());
        let eTime = parseTime(this.value);
        if (sDate == eDate) {
            if (sTime < eTime) $("#Cart_EndTime").val(this.value)
            else { $("#Cart_EndTime").val(null); $("#EndTime").val(null); }
        }
        else {
            $("#Cart_EndTime").val(this.value);
        }
    }
    else { $("#Cart_EndTime").val(null); $("#EndTime").val(null); }
})
$('input[name="bookingstudio"]').change(function () {
    //var start_time = $(this).attr('start-time');
    //var end_time = $(this).attr('end-time');
    minInterval = $(this).attr('min-hours');
    $("#Cart_ServicePriceId").val($('input[name="bookingstudio"]:checked').val());
    $("#dvServiceName").empty().text($(this).attr('service-name'));
    $("#dvStudioName").empty().text($(this).attr('category'));
    $("#dvStudioTitle").empty().text($(this).attr('title'));
    //$(".dv-end-time").hide();
   // $(".dv-start-time").hide();
    $("#Cart_BookingDate").val(null);
    $("#Cart_BookingEndDate").val(null);
    $("#StartTime").empty();
    $("#EndTime").empty();
    $("#Cart_EndTime").val(null);
    $("#Cart_StartTime").val(null);
   // setStartTimeSlots("8:00 AM", "11:00 PM", 2, []);
    //setEndTimeSlots(start_time, end_time, 2, []);
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

//function setTimeSlots(startTime, endTime, minhrs) {
//    let sTime = parseTime(convertTime12to24(startTime));
//    let eTime = parseTime(convertTime12to24(endTime));
//    let timeSlots = calculate_time_slot(sTime, eTime, 60);
//    let firstTimeSlot = "";
//    $("#Cart_StartTime").val(null);
//    $("#Cart_EndTime").val(null);
//    $(".startTime").empty();
//    $(".endTime").empty();
//    $.each(timeSlots, function (i, e) {
//        if (i === 0)
//            firstTimeSlot = e.id;
//        let disableStartTime = parseTime(convertTime12to24(endTime)) - parseTime(e.id) < (minhrs * 60);
//        let disableEndTime = (parseTime(e.id) - parseTime(firstTimeSlot)) < (minhrs * 60);
//        $(".startTime").append('<button type="button" class="btn btn-md font-weight-bold mr-2 mt-2 ' + (disableStartTime ? 'disabled btn-light-dark' : 'btn-light-success') + '">' + convertFrom24To12(e.id) + '</button>');
//        $(".endTime").append('<button type="button" class="btn btn-md font-weight-bold mr-2 mt-2 ' + (disableEndTime ? 'disabled btn-light-dark' : 'btn-light-success') + '">' + convertFrom24To12(e.id) + '</button>');
//    });
//}

//function setStartTimeSlots(startTime, endTime, minhrs, res) {
//    let sTime = parseTime(convertTime12to24(startTime));
//    let eTime = parseTime(convertTime12to24(endTime));
//    let timeSlots = calculate_time_slot(sTime, eTime, 60);
//    var starttimedata = [];
//    $.each(timeSlots, function (i, e) {
//        let disableStartTime = false;//((parseTime(convertTime12to24(endTime)) - parseTime(e.id)) < (minhrs * 60));

//        $.each(res, function (ri, re) {
//            if (parseTime(re) == parseTime(e.id)) {
//                disableStartTime = true;
//            }
//        });

//        starttimedata.push({
//            id: e.id,
//            text: convertFrom24To12(e.id),
//            disabled: disableStartTime
//        });

//         });
//    if ($('#StartTime').hasClass("select2-hidden-accessible")) $("#StartTime").select2('destroy').empty().select2({ data: starttimedata }).val(convertTime12to24($("#Cart_StartTime").val())).trigger('change');
//    else $("#StartTime").empty().select2({ data: starttimedata }).val(convertTime12to24($("#Cart_StartTime").val())).trigger('change');
//}

//function setEndTimeSlots(startTime, endTime, minhrs, eres) {

//    let sTime = parseTime(convertTime12to24(startTime));
//    let eTime = parseTime(convertTime12to24(endTime));
//    let timeSlots = calculate_time_slot(sTime, eTime, 60);
//    let firstTimeSlot = "";
//    var endtimedata = [];
//    $.each(timeSlots, function (i, e) {
//        if (i === 0)
//            firstTimeSlot = e.id;
//        let disableEndTime = false;// (parseTime(e.id) - parseTime(firstTimeSlot)) < (minhrs * 60);

//        $.each(eres, function (ri, re) {
//            if (parseTime(re) == parseTime(e.id)) {
//                disableEndTime = true;
//            }
//        });
//        endtimedata.push({            id: e.id,
//            text: convertFrom24To12(e.id),
//            disabled: disableEndTime
//        });
//    });
//    if ($('#EndTime').hasClass("select2-hidden-accessible")) $("#EndTime").select2('destroy').empty().select2({ data: endtimedata }).val(convertTime12to24($("#Cart_EndTime").val())).trigger('change');
//    else $("#EndTime").empty().select2({ data: endtimedata }).val(convertTime12to24($("#Cart_EndTime").val())).trigger('change');
//}


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
            let sDate = new Date($("#Cart_BookingDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            let eDate = new Date($("#Cart_BookingEndDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            if (start < sDate && end > eDate) {
                disableStartTime = true;
            }

            if (Date.parse(start) == Date.parse(sDate) && Date.parse(end) == Date.parse(eDate)) {

                if (parseTime(e.id) >= parseTime(re.start) && parseTime(e.id) <= parseTime(re.end)) {
                    disableStartTime = true;
                }
            }
            else { 
            if (Date.parse(start) == Date.parse(sDate)) {

                if (parseTime(e.id) >= parseTime(re.start)) {
                    disableStartTime = true;
                }
            }
            if (Date.parse(end) == Date.parse(start)) {

                if (parseTime(e.id) <= parseTime(re.end)) {
                    disableStartTime = true;
                }
                }  
            }
        });

        starttimedata.push({
            id: e.id,
            text: convertFrom24To12(e.id),
            disabled: disableStartTime
        });

    });
    if ($('#StartTime').hasClass("select2-hidden-accessible")) $("#StartTime").select2('destroy').empty().select2({ data: starttimedata }).val(convertTime12to24($("#Cart_StartTime").val())).trigger('change');
    else $("#StartTime").empty().select2({ data: starttimedata }).val(convertTime12to24($("#Cart_StartTime").val())).trigger('change');
}

function setEndTimeSlotsValidation(startTime, endTime, minhrs, eres) {

    let sTime = parseTime(convertTime12to24(startTime));
    let eTime = parseTime(convertTime12to24(endTime));
    let timeSlots = calculate_time_slot(sTime, eTime, 60);
    let staTime = parseTime(convertTime12to24($("#Cart_StartTime").val()));
    let firstTimeSlot = "";
    var endtimedata = [];
    $.each(timeSlots, function (i, e) {
        
        let disableEndTime = false;

        let sDate = new Date($("#Cart_BookingDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
        let eDate = new Date($("#Cart_BookingEndDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));

        if (Date.parse(sDate) < Date.parse(eDate)) {
            disableEndTime = (parseTime(e.id) - staTime) >= 0;
        }
        else {
            disableEndTime = (parseTime(e.id) - staTime) < (minhrs * 60);
        }
        $.each(eres, function (ri, re) {
            let start = new Date(re.startDate.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            let end = new Date(re.endDate.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));

            if (start < sDate && end > eDate) {
                disableEndTime = true;
            }
            if (Date.parse(start) == Date.parse(sDate) && Date.parse(end) == Date.parse(eDate)) {

                if (parseTime(e.id) >= parseTime(re.start) && parseTime(e.id) <= parseTime(re.end)) {
                    disableEndTime = true;
                }
            }
            else {
                if (Date.parse(end) == Date.parse(eDate)) {

                    if (parseTime(e.id) <= parseTime(re.end)) {
                        disableEndTime = true;
                    }
                }

                if (Date.parse(start) == Date.parse(eDate)) {

                    if (parseTime(e.id) >= parseTime(re.start)) {
                        disableEndTime = true;
                    }
                }
            }
        });
        endtimedata.push({
            id: e.id,
            text: convertFrom24To12(e.id),
            disabled: disableEndTime
        });
    });
    if ($('#EndTime').hasClass("select2-hidden-accessible")) $("#EndTime").select2('destroy').empty().select2({ data: endtimedata }).val(convertTime12to24($("#Cart_EndTime").val())).trigger('change');
    else $("#EndTime").empty().select2({ data: endtimedata }).val(convertTime12to24($("#Cart_EndTime").val())).trigger('change');
}
function setEndTimeSlots(startTime, endTime, minhrs, eres) {

    let sTime = parseTime(convertTime12to24(startTime));
    let eTime = parseTime(convertTime12to24(endTime));
    let timeSlots = calculate_time_slot(sTime, eTime, 60);
    //let iTime = parseTime($("#Cart_StartTime").val());
    let firstTimeSlot = "";
    var endtimedata = [];
    $.each(timeSlots, function (i, e) {
        //if (i === 0)
        //    firstTimeSlot = e.id;
        let disableEndTime = false;

        let sDate = new Date($("#Cart_BookingDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
        let eDate = new Date($("#Cart_BookingEndDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
        //if (sDate == eDate && parseTime(e.id) > iTime) {
        //    disableEndTime = (parseTime(e.id) - iTime) < (minhrs * 60);
        //}
        // let disableEndTime = (parseTime(e.id) - parseTime(firstTimeSlot)) < (minhrs * 60);

        $.each(eres, function (ri, re) {
            let start = new Date(re.startDate.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            let end = new Date(re.endDate.replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            
            if (start < sDate && end > eDate) {
                disableEndTime = true;
            }
            if (Date.parse(start) == Date.parse(sDate) && Date.parse(end) == Date.parse(eDate)) {

                if (parseTime(e.id) >= parseTime(re.start) && parseTime(e.id) <= parseTime(re.end)) {
                    disableEndTime = true;
                }
            }
            else { 
            if ( Date.parse(end) == Date.parse(eDate)) {

                if ( parseTime(e.id) <= parseTime(re.end)) {
                    disableEndTime = true;
                }
            }
           
            if ( Date.parse(start) == Date.parse(eDate)) {

                if (parseTime(e.id) >= parseTime(re.start) ) {
                    disableEndTime = true;
                }
            } 
            }
        });
        endtimedata.push({
            id: e.id,
            text: convertFrom24To12(e.id),
            disabled: disableEndTime
        });
    });
    if ($('#EndTime').hasClass("select2-hidden-accessible")) $("#EndTime").select2('destroy').empty().select2({ data: endtimedata }).val(convertTime12to24($("#Cart_EndTime").val())).trigger('change');
    else $("#EndTime").empty().select2({ data: endtimedata }).val(convertTime12to24($("#Cart_EndTime").val())).trigger('change');
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
   // $(".dv-end-time").show();
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
function diff_hours(edate, sdate) {

    var diff = (edate.getTime() - sdate.getTime()) / 1000;
    diff /= (60 * 60);
    return Math.abs(Math.round(diff));

}
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
                                message: "Booking Start Date is required"
                            }
                        }
                    },
                    "Cart.BookingEndDate": {
                        validators: {
                            notEmpty: {
                                message: "Booking End Date is required"
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
                    const [eday, emonth, eyear] = $("#Cart_BookingEndDate").val().split('-');
                    const [ehours] = String(convertTime12to24($("#Cart_EndTime").val())).split(':');
                    
                    const edate = new Date(
                        +eyear,
                        +emonth-1,
                        +eday,
                        +ehours,
                        +"00",
                        +"00",
                    );
                    const [sday, smonth, syear] = $("#Cart_BookingDate").val().split('-');
                    const [shours] = String(convertTime12to24($("#Cart_StartTime").val())).split(':');
                    
                    const sdate = new Date(
                        +syear,
                        +smonth-1,
                        +sday,
                        +shours,
                        +"00",                       
                        +"00",
                    );
                    
                    
                    if (t.getStep() === 1) {
                        t.goTo(t.getNewStep() - 1);
                    }
                    else if (!(t.getStep() > t.getNewStep())) {
                        var i = e[0];
                        return i && i.validate().then((function (i) {
                            "Valid" == i ? (t.goTo(t.getNewStep()),
                                $("#dvBookingDate").empty().text($("#Cart_BookingDate").val() + " " + $("#Cart_StartTime").val()),
                                $("#dvBookingEndDate").empty().text($("#Cart_BookingEndDate").val() + " " + $("#Cart_EndTime").val()),                              
                                $("#dvBookingHrs").empty().text(diff_hours(edate , sdate) + " " + "hrs"),
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