"use strict";
var BookingDataTable = {
    init: function () {
        var t;
        t = $("#kt_datatable").KTDatatable({
            data: {
                type: "remote",
                source: {
                    read: {
                        url: location.protocol + "//" + location.host + "/Admin/Booking/GetBookings",
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
                    field: "booking.bookingId",
                    title: "ID",
                    sortable: "desc",
                    selector: !1,
                    autoHide: !1,
                    textAlign: "center",
                    template: function (t) {
                        return t.booking.bookingId;
                    }
                },
                {
                    field: "customer.name",
                    title: "Name",
                    autoHide: !1,
                    template: function (t) {
                        return t.customer.name;
                    }
                },
                {
                    field: "servicePrice.categoryName",
                    title: "Studio",
                    autoHide: !1,
                    template: function (t) {
                        return t.servicePrice.categoryName;
                    }
                },
                {
                    field: "servicePrice.serviceName",
                    title: "Service",
                    autoHide: !1,
                    template: function (t) {
                        return t.servicePrice.serviceName;
                    }
                },
                {
                    field: "booking.bookingDate",
                    title: "Start Date",
                    type: 'date',
                    format: 'dd-MM-yyyy',
                    autoHide: !1,
                    template: function (t) {
                        return t.booking.bookingDate;
                    }
                },
                {
                    field: "booking.startTime",
                    title: "Start Time",
                    autoHide: !1,
                    template: function (t) {
                        return t.booking.startTime;
                    }
                },
                {
                    field: "booking.bookingEndDate",
                    title: "End Date",
                    type: 'date',
                    format: 'dd-MM-yyyy',
                    autoHide: !1,
                    template: function (t) {
                        return t.booking.bookingEndDate;
                    }
                },
                {
                    field: "booking.endTime",
                    title: "End Time",
                    autoHide: !1,
                    template: function (t) {
                        return  t.booking.endTime;
                    }
                },
                {
                    field: "totalhours",
                    title: "Hours",
                    autoHide: !1,
                    template: function (t) {
                        return t.booking.totalHours;
                    }
                },
                {
                    field: "ratePerHour",
                    title: "Rate Per Hour",
                    autoHide: !1,
                    template: function (t) {
                        return t.booking.ratePerHour;
                    }
                },
                {
                    field: "total",
                    title: "Total",
                    autoHide: !1,
                    template: function (t) {
                        return t.booking.total;
                    }
                },
                {
                    field: "BookingStatus",
                    title: "Status",
                    autoHide: !1,
                    textAlign: "center",
                    template: function (t) {
                        var e = {
                            "0": {
                                title: "WaitingForApproval",
                                class: "light"
                            },
                            "1": {
                                title: "Pending",
                                class: "warning"
                            },
                            "2": {
                                title: "OnHold",
                                class: "primary"
                            },
                            "3": {
                                title: "Confirmed",
                                class: "success"
                            },
                            "4": {
                                title: "ReScheduled",
                                class: "info"
                            },
                            "5": {
                                title: "WaitingForCancellation",
                                class: "light"
                            },
                            "6": {
                                title: "Cancelled",
                                class: "danger"
                            },
                            "7": {
                                title: "Failed",
                                class: "danger"
                            },
                            "8": {
                                title: "Reserved",
                                class: "primary"
                            }
                        };
                        return '<span class="label label-' + e[t.booking.bookingStatus].class + ' label-pill label-inline">' + e[t.booking.bookingStatus].title + '</span>';
                    }
                },
                {
                    field: "paymentStatus",
                    title: "Payment",
                    width: 120,
                    autoHide: !1,
                    template: function (t) {
                        var e = {
                            "0": {
                                title: "No Payment",
                                class: "warning"
                            },
                            "1": {
                                title: "Advance Paid",
                                class: "warning"
                            },
                            "2": {
                                title: "Full Paid",
                                class: "success"
                            },
                            "3": {
                                title: "Refund Pending",
                                class: "info"
                            },
                            "4": {
                                title: "Refund Completed",
                                class: "success"
                            }
                        };
                        return '<span class="label label-' + e[t.booking.paymentStatus].class + ' label-pill label-inline">' + e[t.booking.paymentStatus].title + '</span>';
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
                        if (!t.booking.isBookingExpired && t.booking.bookingStatus !== 6) {
                            if (t.booking.bookingStatus === 0) {
                                html += '<a href="javascript:;" class="dropdown-item" title="Approve Booking"" onClick="updateBookingStatus(' + t.booking.id + ',' + 1 + ')"><i class="flaticon2-check-mark"></i><span style="margin: 3px;">Approve Booking</span></a>';
                            }
                            else if (t.booking.bookingStatus === 5) {
                                html += '<a href="javascript:;" class="dropdown-item" title="Approve Cancellation"" onClick="openCancelModal(' + t.booking.id + ',' + 1 + ')"><i class="flaticon2-check-mark"></i><span style="margin: 3px;">Approve</span></a>';
                                html += '<a href="javascript:;" class="dropdown-item" title="Reject Cancellation"" onClick="approveCancellationRequest(' + t.booking.id + ',' + 2 + ')"><i class="flaticon2-delete"></i><span style="margin: 3px;">Reject</span></a>';
                            }
                            else {
                                if (t.booking.bookingStatus !== 2 && t.booking.paymentStatus !== 0)
                                    html += '<a href="javascript:;" class="dropdown-item" title="On-Hold"" onClick="updateBookingStatus(' + t.booking.id + ',' + 2 + ')"><i class="flaticon2-hourglass-1"></i><span style="margin: 3px;">On-Hold</span></a>';
                                html += '<a href="javascript:;" class="dropdown-item" title="Edit Details" onClick="editBooking(' + t.booking.id + ')"><i class="flaticon2-edit"></i><span style="margin: 3px;">Edit</span></a>';
                                html += '<a href="javascript:;" class="dropdown-item" title="Add Addon" onClick="openAddOnModal(' + t.booking.id + ')"><i class="flaticon2-plus"></i><span style="margin: 3px;">Add Addon</span></a>';                                
                            }
                            if (t.booking.bookingStatus !== 5) {
                                html += '<a href="javascript:;" class="dropdown-item" title="Cancel Booking" onClick="openCancelModal(' + t.booking.id + ',' + 0 + ')"><i class="flaticon2-delete"></i><span style="margin: 3px;">Cancel</span></a>';
                            }
                        } else {
                            if (t.booking.bookingStatus === 6) {
                                html += '<a href="javascript:;" class="dropdown-item" title="Cancel Booking" onClick="openCancelModal(' + t.booking.id + ',' + 3 + ')">View Cancel Request</span></a>';
                            }
                        }
                        if (t.booking.paymentStatus !== 2) {
                            html += '<a href="javascript:;" class="dropdown-item" title="Complete Payment" onClick="openPaymentModal(' + t.booking.id + ')"><i class="flaticon2-correct"></i><span style="margin: 3px;">Complete Payment</span></a>';
                            html += '<a href="javascript:;" class="dropdown-item copytext" title="Payment Link" bookingId="' + t.booking.id + '"><i class="flaticon2-copy"></i><span style="margin: 3px;"> Payment Link</span></a>';
                        }
                        if (t.booking.bookingStatus === 3 || t.booking.bookingStatus === 4) {
                            html += '<a href="/Booking/PaymentReciept/' + t.booking.id + '" target="_blank" class="dropdown-item" title="View Reciept"><i class="flaticon2-file"></i><span style="margin: 3px;">View Reciept</span></a>';
                        }
                        //html += '<a href="javascript:;" id="btnDelete" class="dropdown-item" data-target="' + t.booking.id + '" controller="Admin/Booking" title="Delete"><i class="flaticon2-trash"></i><span style="margin: 3px;">Delete</span></a>';
                        html += '</div></div>';
                        return html;
                    }
                }]
        });
    },
    ApproveBooking: function (id, bookingid) {
        bootbox.confirm({
            title: "Confirm",
            message: "Approve booking and send payment link for booking id: " + bookingid + " ?",
            buttons: {
                cancel: {
                    label: '<i class="fa fa-times"></i> No'
                },
                confirm: {
                    label: '<i class="fa fa-check"></i> Yes'
                }
            },
            callback: function (result) {
                if (result) {
                    $.get("/Admin/Booking/ApproveBooking?id=" + id)
                        .done(function (data) {
                            if (data.result) {
                                $(".kt-datatable").KTDatatable().reload();
                                toastr.success("Booking approved successfully.");
                            }
                            else {
                                toastr.error("Error: " + data.errorMsg);
                            }
                        })
                        .fail(function (error) {
                            toastr.error(defaultErrorMessage + error);
                        });
                }
            }
        });
    }
};
var KTBookingModal = function () {
    const r = document.getElementById("kt_modal_booking")
    return {
        init: function () {
            (() => {
                let e = r.querySelector("#kt_modal_booking_form");
                let n = new bootstrap.Modal(r);
                let serviceData = [];
                $("#Booking_BookingDate").datepicker({
                    enableTime: false,
                    format: "dd-mm-yyyy",
                   // startDate: '+1d'
                });
                $("#Booking_BookingEndDate").datepicker({
                    enableTime: false,
                    format: "dd-mm-yyyy",
                    //startDate: '+1d'                    
                });
                $("#ddlCategory").select2();
                $("#Booking_CustomerId").select2();
                $("#Booking_ServicePriceId").select2();
                r.querySelector('[data-kt-booking-modal-action="close"]').addEventListener("click", (t => {
                    t.preventDefault();
                    $("#kt_modal_booking").modal('hide');
                }
                ));
                r.querySelector('[data-kt-booking-modal-action="cancel"]').addEventListener("click", (t => {
                    t.preventDefault();
                    $("#kt_modal_booking").modal('hide');
                }
                ));
                $(e.querySelector('#ddlCategory')).on("change", (function () {
                    if (this.value) {
                        $.get("/Admin/ServicePrice/GetServices/" + this.value).done((res) => {
                            if (res.data.length) {
                                serviceData = res.data;
                                let services = [];
                                $.each(res.data, function (i, e) {
                                    services.push({
                                        id: e.id,
                                        text: e.serviceName
                                    });
                                });
                                if ($('#Booking_ServicePriceId').hasClass("select2-hidden-accessible")) $("#Booking_ServicePriceId").select2('destroy').empty().select2({ data: services }).trigger('change');
                                else $("#Booking_ServicePriceId").empty().select2({ data: services }).trigger('change');
                            }
                            else {
                                $("#Booking_ServicePriceId").empty().select2({ data: [] }).trigger('change');
                            }
                        }).fail((err) => {
                            console.log(err);
                        });
                    }
                    else {
                        $("#Booking_ServicePriceId").empty().select2({ data: [] }).trigger('change');
                    }
                }
                ));
                $(e.querySelector('#Booking_ServicePriceId')).on("change", (function () {
                    if (this.value) {
                        let servicePriceId = parseInt(this.value);
                        var selectedValue = {};
                        serviceData.forEach(function (e) {
                            if (servicePriceId == e.id) selectedValue = e;
                        });
                        let sDate = $("#Booking_BookingDate").val();
                        if (sDate) { 
                            $.get("/Service/GetBookedStartEndTimeSlots" + "?id=" + $("#ddlCategory").val() + "&date=" + sDate).done((res) => {

                            setStartTimeSlotsBooking(selectedValue.startTime, selectedValue.endTime, 1, res);
                            setEndTimeSlotsBooking(selectedValue.startTime, selectedValue.endTime, 1, res);

                        }).fail((err) => {
                            console.log(err);
                        });
                        }
                    } else {
                        $("#StartTime").empty().select2().trigger('change');
                        $("#EndTime").empty().select2().trigger('change')
                    }
                }
                ));
                $(e.querySelector('#Booking_BookingDate')).on("change", (function () {
                    if (this.value) {
                        $("#Booking_BookingEndDate").val(this.value) 
                        $("#Booking_BookingEndDate").datepicker('setStartDate', this.value);
                    }
                    else $("#Booking_BookingEndDate").val(null)

                    let servicePriceId = parseInt($("#Booking_ServicePriceId").val());
                    var selectedValue = {};
                    var tymValue = this.value;
                    $.get("/Admin/ServicePrice/GetServices/" + $("#ddlCategory").val()).done((res) => {
                        if (res.data.length) {
                            serviceData = res.data;
                            let services = [];
                            $.each(res.data, function (i, e) {
                                services.push({
                                    id: e.id,
                                    text: e.serviceName
                                });
                            });
                            if (serviceData) {
                                serviceData.forEach(function (e) {
                                    if (servicePriceId == e.id) selectedValue = e;
                                });
                                $("#Booking_StartTime").val(null); $("#StartTime").val(null);
                                $("#Booking_EndTime").val(null); $("#EndTime").val(null);

                                
                            }
                        }
                    }).fail((err) => {
                        console.log(err);
                    });

                    $.get("/Service/GetBookedStartEndTimeSlots" + "?id="+$("#ddlCategory").val() + "&date=" + this.value).done((res) => {
                        
                        setStartTimeSlotsBooking(selectedValue.startTime, selectedValue.endTime, 1, res);
                        setEndTimeSlotsBooking(selectedValue.startTime, selectedValue.endTime, 1, res);
                   
                }).fail((err) => {
                    console.log(err);
                });

                }));

                $(e.querySelector('#Booking_BookingEndDate')).on("change", (function () {
                    if (this.value) {
                        //$("#Booking_EndTime").val(''); $("#EndTime").val('');
                        //let servicePriceId = parseInt($("#Booking_ServicePriceId").val());
                        //var selectedValue = {};
                        //if (serviceData.length > 0) {
                        //    serviceData.forEach(function (e) {
                        //        if (servicePriceId == e.id) selectedValue = e;
                        //    });
                        //    $("#Booking_StartTime").val(null); $("#StartTime").val(null);
                        //    setEndTimeSlots(selectedValue.startTime, selectedValue.endTime, 1);
                        //}
                        //else {
                        //    $.get("/Admin/ServicePrice/GetServices/" + $("#ddlCategory").val()).done((res) => {
                        //        if (res.data.length) {
                        //            serviceData = res.data;
                        //            let services = [];
                        //            $.each(res.data, function (i, e) {
                        //                services.push({
                        //                    id: e.id,
                        //                    text: e.serviceName
                        //                });
                        //            });
                        //            if (serviceData) {
                        //                serviceData.forEach(function (e) {
                        //                    if (servicePriceId == e.id) selectedValue = e;
                        //                });
                        //                $("#Booking_StartTime").val(null); $("#StartTime").val(null);

                        //                setEndTimeSlots(selectedValue.startTime, selectedValue.endTime, 1);
                        //            }
                        //        }
                        //    }).fail((err) => {
                        //        console.log(err);
                        //    });

                        //}

                        let sDate = new Date($("#Booking_BookingDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
                        let eDate = new Date($("#Booking_BookingEndDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
                        if (Date.parse(sDate) < Date.parse(eDate)) {

                            let servicePriceId = parseInt($("#Booking_ServicePriceId").val());
                            var selectedValue = {};
                            var tymValue = this.value;
                            $.get("/Admin/ServicePrice/GetServices/" + $("#ddlCategory").val()).done((res) => {
                                if (res.data.length) {
                                    serviceData = res.data;
                                    let services = [];
                                    $.each(res.data, function (i, e) {
                                        services.push({
                                            id: e.id,
                                            text: e.serviceName
                                        });
                                    });
                                    if (serviceData) {
                                        serviceData.forEach(function (e) {
                                            if (servicePriceId == e.id) selectedValue = e;
                                        });
                                        //$("#Booking_StartTime").val(null); $("#StartTime").val(null);
                                        $("#Booking_EndTime").val(null); $("#EndTime").val(null);

                                    }
                                }
                            }).fail((err) => {
                                console.log(err);
                            });

                            $.get("/Service/GetBookedStartEndTimeSlots" + "?id=" + $("#ddlCategory").val() + "&date=" + this.value).done((res) => {
                                
                                setEndTimeSlotsBooking(selectedValue.startTime, selectedValue.endTime, 1, res);

                            }).fail((err) => {
                                console.log(err);
                            });
                            
                        }
                       
                    }
                    
                }));
                $(e.querySelector('#StartTime')).on("change", (function () {
                    //if (this.value) $("#Booking_StartTime").val(this.value)
                    //else $("#Booking_StartTime").val(null)
                    if (this.value) {
                    let servicePriceId = parseInt($("#Booking_ServicePriceId").val());
                    var selectedValue = {};
                    var tymValue = this.value;
                    $.get("/Admin/ServicePrice/GetServices/" + $("#ddlCategory").val()).done((res) => {
                            if (res.data.length) {
                                serviceData = res.data;
                                let services = [];
                                $.each(res.data, function (i, e) {
                                    services.push({
                                        id: e.id,
                                        text: e.serviceName
                                    });
                                });
                                if (serviceData) {
                                    serviceData.forEach(function (e) {
                                        if (servicePriceId == e.id) selectedValue = e;
                                    }); 
                                }
                            }
                        }).fail((err) => {
                            console.log(err);
                        });
                        $.get("/Service/GetBookedStartEndTimeSlots" + "?id=" + $("#ddlCategory").val() + "&date=" + $("#Booking_BookingEndDate").val()).done((res) => {

                            setEndTimeSlotsBookingwithTym(selectedValue.startTime, selectedValue.endTime, 1, res, tymValue);

                        }).fail((err) => {
                            console.log(err);
                        });
                    
                    
                        let sDate = new Date($("#Booking_BookingDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
                        let eDate = new Date($("#Booking_BookingEndDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
                        let sTime = parseTime(this.value); 
                        let eTime = parseTime($("#Booking_EndTime").val());
                        if (Date.parse(sDate) == Date.parse(eDate) && eTime) {
                            if (sTime < eTime) $("#Booking_StartTime").val(this.value)
                            else { $("#Booking_StartTime").val(null); $("#StartTime").val(null) }
                        }
                        else {
                            $("#Booking_StartTime").val(this.value);
                        }
                    }
                    else { $("#Booking_StartTime").val(null); $("#StartTime").val(null); }
                }));
                $(e.querySelector('#EndTime')).on("change", (function () {
                    if (this.value) {
                        let sDate = new Date($("#Booking_BookingDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
                        let eDate = new Date($("#Booking_BookingEndDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
                        let sTime = parseTime($("#Booking_StartTime").val());
                        let eTime = parseTime(this.value);
                        if (Date.parse(sDate) == Date.parse(eDate)) {
                            if (sTime < eTime) { $("#Booking_EndTime").val(this.value) }
                            else { $("#Booking_EndTime").val(null); $("#EndTime").val(null); }
                        }
                        else {
                            $("#Booking_EndTime").val(this.value); $("#EndTime").val(this.value);
                        }
                       
                        
                    }
                    else { $("#Booking_EndTime").val(null); $("#EndTime").val(null); }
                }));
            }
            )()
        }
    }
}();

$(document).on('click', '.copytext', function () {
    var thistooltip = $(this);
    var thistext = location.protocol + "//" + location.host + '/User/Booking/PayNow/' + $(this).attr('bookingId');
    navigator.clipboard.writeText(thistext);
    $(this).attr('title', 'copied');
    setTimeout(function () { $(thistooltip).tooltip("toggle"); }, 800);
});

function addFormValidation() {
    var i = FormValidation.formValidation($("#kt_modal_booking_form")[0], {
        fields: {
            "Booking.CustomerId": {
                validators: {
                    notEmpty: {
                        message: "Customer is required"
                    }
                }
            },
            "Booking.BookingDate": {
                validators: {
                    notEmpty: {
                        message: "Booking date is required"
                    }
                }
            },
            "Booking.BookingEndDate": {
                validators: {
                    notEmpty: {
                        message: "Booking end date is required"
                    }
                }
            },
            "Booking.StartTime": {
                validators: {
                    notEmpty: {
                        message: "Start Time is required"
                    }
                }
            },
            "Booking.EndTime": {
                validators: {
                    notEmpty: {
                        message: "End Time is required"
                    }
                }
            },
            "Booking.ServicePriceId": {
                validators: {
                    notEmpty: {
                        message: "Service is required"
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
    const a = $('[data-kt-booking-modal-action="submit"]')[0];
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
                            text: "Confirm to " + (parseInt($("#Booking_Id").val()) > 0 ? 'Re-Schedule' : 'Create') + " booking!",
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
function setStartTimeSlots(startTime, endTime, minhrs) {
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
    });
    if ($('#StartTime').hasClass("select2-hidden-accessible")) $("#StartTime").select2('destroy').empty().select2({ data: starttimedata }).val(convertTime12to24($("#Booking_StartTime").val())).trigger('change');
    else $("#StartTime").empty().select2({ data: starttimedata }).val(convertTime12to24($("#Booking_StartTime").val())).trigger('change');
    
}
function setEndTimeSlots(startTime, endTime, minhrs) {
    let sTime = parseTime(convertTime12to24(startTime));
    let eTime = parseTime(convertTime12to24(endTime));
    let timeSlots = calculate_time_slot(sTime, eTime, 60);
    let firstTimeSlot = "";
    var starttimedata = [];
    var endtimedata = [];
    $.each(timeSlots, function (i, e) {
        if (i === 0)
            firstTimeSlot = e.id;
       // let disableStartTime = parseTime(convertTime12to24(endTime)) - parseTime(e.id) < (minhrs * 60);
        //let disableEndTime = (parseTime(e.id) - parseTime(firstTimeSlot)) < (minhrs * 60);
        let disableEndTime = parseTime(e.id) <= parseTime(minhrs);
        //starttimedata.push({
        //    id: e.id,
        //    text: convertFrom24To12(e.id),
        //    //disabled: disableStartTime
        //});
        endtimedata.push({
            id: e.id,
            text: convertFrom24To12(e.id),
            disabled: disableEndTime
        });
    });     
    if ($('#EndTime').hasClass("select2-hidden-accessible")) $("#EndTime").select2('destroy').empty().select2({ data: endtimedata }).val(convertTime12to24($("#Booking_EndTime").val())).trigger('change');
    else $("#EndTime").empty().select2({ data: endtimedata }).val(convertTime12to24($("#Booking_EndTime").val())).trigger('change');
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
       // let disableStartTime = parseTime(convertTime12to24(endTime)) - parseTime(e.id) < (minhrs * 60);
        //let disableEndTime = (parseTime(e.id) - parseTime(firstTimeSlot)) < (minhrs * 60);
        starttimedata.push({
            id: e.id,
            text: convertFrom24To12(e.id),
            //disabled: disableStartTime
        });
        endtimedata.push({
            id: e.id,
            text: convertFrom24To12(e.id),
           // disabled: disableEndTime
        });
        //$(".startTime").append('<button type="button" class="btn btn-sm font-weight-bold mr-2 mt-2 ' + (disableStartTime ? 'disabled btn-light-dark' : 'btn-rbstudio') + '">' + e.id + '</button>');
        //$(".endTime").append('<button type="button" class="btn btn-sm font-weight-bold mr-2 mt-2 ' + (disableEndTime ? 'disabled btn-light-dark' : 'btn-rbstudio') + '">' + e.id + '</button>');
    });
    //var bookingStartTime = 
    if ($('#StartTime').hasClass("select2-hidden-accessible")) $("#StartTime").select2('destroy').empty().select2({ data: starttimedata }).val(convertTime12to24($("#Booking_StartTime").val())).trigger('change');
    else $("#StartTime").empty().select2({ data: starttimedata }).val(convertTime12to24($("#Booking_StartTime").val())).trigger('change');

    if ($('#EndTime').hasClass("select2-hidden-accessible")) $("#EndTime").select2('destroy').empty().select2({ data: endtimedata }).val(convertTime12to24($("#Booking_EndTime").val())).trigger('change');
    else $("#EndTime").empty().select2({ data: endtimedata }).val(convertTime12to24($("#Booking_EndTime").val())).trigger('change');
}

function updateBookingStatus(id, status) {
    $.ajaxSetup({
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }
    }),
        Swal.fire({
            text: "Are you sure you want to " + (status === 1 ? 'Approve' : 'On-Hold') + " booking?",
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
                $.get("/Admin/Booking/UpdateBookingStatus?id=" + id + "&status=" + status)
                    .done(function (data) {
                        if (data.result) {
                            $(".kt-datatable").KTDatatable().reload();
                            toastr.success("Booking " + (status === 1 ? 'Approve' : 'On-Hold') + " successfully.");
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

function approveCancellationRequest(id, status) {
    $.ajaxSetup({
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }
    }),
        Swal.fire({
            text: "Are you sure you want to " + (status === 1 ? 'Approve' : 'Cancel') + " request?",
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
                $.get("/Admin/Booking/ApproveCancellationRequest?id=" + id + "&status=" + status)
                    .done(function (data) {
                        if (data.result) {
                            $(".kt-datatable").KTDatatable().reload();
                            $("#kt_modal_cancel_booking").modal('hide');
                            toastr.success("Request " + (status === 1 ? 'approved' : 'rejected') + " successfully.");
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

function editBooking(id) {
    $('#mdlBookingBody').empty();
    $.get("/Admin/Booking/CreateBookingModal/" + id)
        .done(function (res) {
            $('#mdlBookingBody').append(res);
            $("#kt_modal_booking").modal('show');
            addFormValidation();
            KTBookingModal.init();
            $(".booking-header").text("Edit Booking");
            $("#Booking_CustomerId").attr('disabled', 'disabled');
            $("#ddlCategory").attr('disabled', 'disabled');
            $("#Booking_ServicePriceId").attr('disabled', 'disabled');
            // setTimeSlots($("#ServiceStartTime").val(), $("#ServiceEndTime").val(), 1);
            

            $.get("/Service/GetBookedStartEndTimeSlots" + "?id=" + $("#ddlCategory").val() + "&date=" + $("#Booking_BookingDate").val()).done((res) => {

                setStartTimeSlotsBookingEdit($("#ServiceStartTime").val(), $("#ServiceEndTime").val(), $("#Booking_StartTime").val(), $("#Booking_EndTime").val(), res);
                setEndTimeSlotsBookingEdit($("#ServiceStartTime").val(), $("#ServiceEndTime").val(), $("#Booking_StartTime").val() ,$("#Booking_EndTime").val(), res);

            }).fail((err) => {
                console.log(err);
            });
           // $("#StartTime").val($("#Booking_StartTime").val()).trigger('change');
           // $("#EndTime").val($("#Booking_EndTime").val()).trigger('change')
        })
        .fail(function (err) {
            toastr.error(err);
            console.log(err);
        });
}

function openBookingModal() {
    $('#mdlBookingBody').empty();
    $.get("/Admin/Booking/CreateBookingModal")
        .done(function (res) {
            $('#mdlBookingBody').append(res);
            $("#kt_modal_booking").modal('show');
            addFormValidation();
            KTBookingModal.init();
            $(".booking-header").text("Add Booking");
        })
        .fail(function (err) {
            toastr.error(err);
            console.log(err);
        });
}

function getBookingRequest() {
    let request = {
        Id: $("#Booking_Id").val(),
        ServicePriceId: $("#Booking_ServicePriceId").val(),
        CustomerId: $("#Booking_CustomerId").val(),
        BookingDate: $("#Booking_BookingDate").val(),
        BookingEndDate: $("#Booking_BookingEndDate").val(),
        StartTime: $("#Booking_StartTime").val(),
        EndTime: $("#Booking_EndTime").val(),
    }
    return JSON.stringify(request);
}

function openPaymentModal(id) {
    $('#mdlPaymentReceiptBody').empty();
    $.get("/Admin/Booking/PaymentReceiptModal/" + id)
        .done(function (res) {
            $('#mdlPaymentReceiptBody').append(res);
            $("#kt_modal_paymentReceipt").modal('show');
            $("#PaymentReceipt_PaymentDate").datepicker({
                enableTime: false,
                dateFormat: "d-m-Y"
            });
            $("#PaymentReceipt_PaymentProvider").select2();
            $("#PaymentReceipt_PaymentStatus").select2();
            addPayemntFormValidation();
        })
        .fail(function (err) {
            toastr.error(err);
            console.log(err);
        });
}

function addPayemntFormValidation() {
    const r = document.getElementById("kt_modal_paymentReceipt");
    var i = FormValidation.formValidation($("#kt_modal_payment_form")[0], {
        fields: {
            "PaymentReceipt.PaymentProvider": {
                validators: {
                    notEmpty: {
                        message: "Payment Provider is required"
                    }
                }
            },
            "PaymentReceipt.PaymentStatus": {
                validators: {
                    notEmpty: {
                        message: "Payment status is required"
                    }
                }
            },
            "PaymentReceipt.PaymentDate": {
                validators: {
                    notEmpty: {
                        message: "Payment date is required"
                    }
                }
            },
            "PaymentReceipt.Amount": {
                validators: {
                    notEmpty: {
                        message: "Amount is required"
                    }
                }
            },
            "PaymentReceipt.ReferenceId": {
                validators: {
                    notEmpty: {
                        message: "ReferenceId is required"
                    }
                }
            },
            "PaymentReceipt.Remarks": {
                validators: {
                    notEmpty: {
                        message: "Remarks is required"
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
    r.querySelector('[data-kt-payment-modal-action="close"]').addEventListener("click", (t => {
        t.preventDefault();
        $("#kt_modal_paymentReceipt").modal('hide');
    }
    ));
    r.querySelector('[data-kt-payment-modal-action="cancel"]').addEventListener("click", (t => {
        t.preventDefault();
        $("#kt_modal_paymentReceipt").modal('hide');
    }
    ));
    const a = $('[data-kt-payment-modal-action="submit"]')[0];
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
                            text: "Confirm to save payment receipt!",
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
                                $.post("/Admin/Booking/AddPayment", getPaymentRequest()).done(function (res) {
                                    if (res.result) {
                                        $("#kt_modal_paymentReceipt").modal('hide');
                                        $(".kt-datatable").KTDatatable().reload();
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

function getPaymentRequest() {
    let request = {
        BookingId: $("#PaymentReceipt_BookingId").val(),
        PaymentProvider: $("#PaymentReceipt_PaymentProvider").val(),
        PaymentStatus: $("#PaymentReceipt_PaymentStatus").val(),
        PaymentDate: $("#PaymentReceipt_PaymentDate").val(),
        Amount: $("#PaymentReceipt_Amount").val(),
        ReferenceId: $("#PaymentReceipt_ReferenceId").val(),
        Remarks: $("#PaymentReceipt_Remarks").val()
    }
    return JSON.stringify(request);
}

function openCancelModal(id, type) {
    $('#mdlCancelBookingBody').empty();
    $.get("/Admin/Booking/GetCancelModal?id=" + id + "&type=" + type)
        .done(function (res) {
            $('#mdlCancelBookingBody').append(res);
            $("#kt_modal_cancel_booking").modal('show');
            if (!type)
                addFormValidationCancelModal();
        })
        .fail(function (err) {
            toastr.error(err.responseText);
            console.log(err);
        });
}

function openAddOnModal(id) {
    $.ajaxSetup({
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }
    });
    $.get(location.protocol + "//" + location.host + "/Admin/Booking/GetBookingAddonModal/" + id).done(function (res) {
        $('#AddOnModal').empty();
        $('#AddOnModal').append(res);
        KTModalAddOn.init();
        $("#kt_modal_addon").show();
    }).fail(function (err) {
        toastr.error(err.responseText);
        console.log(err);
    });;
}


function addFormValidationCancelModal() {
    var i = FormValidation.formValidation($("#kt_modal_cancel_form")[0], {
        fields: {
            "PaymentProvider": {
                validators: {
                    notEmpty: {
                        message: "PaymentProvider is required"
                    }
                }
            },
            "Name": {
                validators: {
                    notEmpty: {
                        message: "Name is required"
                    }
                }
            },
            "Account": {
                validators: {
                    notEmpty: {
                        message: "Account/UPI Number is required"
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
                            }), $.post(location.protocol + "//" + location.host + "/Admin/Booking/CancelBooking", JSON.stringify({
                                BookingId: parseInt($("#CancelBookingId").val()),
                                RequestType: 2,
                                PaymentProvider: $("#PaymentProvider").val(),
                                Name: $("#Name").val(),
                                Account: $("#Account").val(),
                                IFSC: $("#IFSC").val()
                            })).done(function (res) {
                                if (res.result) {
                                    Swal.fire({
                                        text: "Booking cancelled successfully!",
                                        icon: "success",
                                        buttonsStyling: !1,
                                        confirmButtonText: "Ok, got it!",
                                        customClass: {
                                            confirmButton: "btn fw-bold btn-primary"
                                        }
                                    }).then((function () {
                                        $("#kt_modal_cancel_booking").modal("hide");
                                        $(".kt-datatable").KTDatatable().reload();
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
    //const b = $('[data-kt-cancel-modal-action="close"]')[0];
    //b.addEventListener("click", (function (t) { $("#kt_modal_cancel_booking").modal("hide"); }));
    const c = $('[data-kt-cancel-modal-action="cancel"]')[0];
    c.addEventListener("click", (function (t) { $("#kt_modal_cancel_booking").modal("hide"); }));
}

function setStartTimeSlotsBooking(startTime, endTime, minhrs, res) {
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
            let sDate = new Date($("#Booking_BookingDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            let eDate = new Date($("#Booking_BookingEndDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
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
    if ($('#StartTime').hasClass("select2-hidden-accessible")) $("#StartTime").select2('destroy').empty().select2({ data: starttimedata }).val(convertTime12to24($("#Booking_StartTime").val())).trigger('change');
    else $("#StartTime").empty().select2({ data: starttimedata }).val(convertTime12to24($("#Booking_StartTime").val())).trigger('change');
}

function setEndTimeSlotsBooking(startTime, endTime, minhrs, eres) {

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
            let sDate = new Date($("#Booking_BookingDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
            let eDate = new Date($("#Booking_BookingEndDate").val().replace(/(\d{2})-(\d{2})-(\d{4})/, "$2/$1/$3"));
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
    if ($('#EndTime').hasClass("select2-hidden-accessible")) $("#EndTime").select2('destroy').empty().select2({ data: endtimedata }).val(convertTime12to24($("#Booking_EndTime").val())).trigger('change');
    else $("#EndTime").empty().select2({ data: endtimedata }).val(convertTime12to24($("#Booking_EndTime").val())).trigger('change');
}


function setStartTimeSlotsBookingEdit(startTime, endTime, bookingstart, bookingend, res) {
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
            if (parseTime(e.id) >= parseTime(re.start) && parseTime(e.id) <= parseTime(re.end)) {
                disableStartTime = true;
            }
        });
        if (parseTime(e.id) >= parseTime(bookingstart) && parseTime(e.id) <= parseTime(bookingend)) {
            disableStartTime = false;
        }
        starttimedata.push({
            id: e.id,
            text: convertFrom24To12(e.id),
            disabled: disableStartTime
        });

    });
    if ($('#StartTime').hasClass("select2-hidden-accessible")) $("#StartTime").select2('destroy').empty().select2({ data: starttimedata }).val(convertTime12to24($("#Booking_StartTime").val())).trigger('change');
    else $("#StartTime").empty().select2({ data: starttimedata }).val(convertTime12to24($("#Booking_StartTime").val())).trigger('change');
}

function setEndTimeSlotsBookingEdit(startTime, endTime, bookingstart, bookingend, eres) {

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
            if (parseTime(e.id) >= parseTime(re.start) && parseTime(e.id) <= parseTime(re.end)) {
                disableEndTime = true;
            }
            //if (parseTime(re) == parseTime(e.id)) {
            //    disableEndTime = true;
            //}
        });
        if (parseTime(e.id) >= parseTime(bookingstart) && parseTime(e.id) <= parseTime(bookingend)) {
            disableEndTime = false;
        }
        endtimedata.push({
            id: e.id,
            text: convertFrom24To12(e.id),
            disabled: disableEndTime
        });
    });
    if ($('#EndTime').hasClass("select2-hidden-accessible")) $("#EndTime").select2('destroy').empty().select2({ data: endtimedata }).val(convertTime12to24($("#Booking_EndTime").val())).trigger('change');
    else $("#EndTime").empty().select2({ data: endtimedata }).val(convertTime12to24($("#Booking_EndTime").val())).trigger('change');
}

function setEndTimeSlotsBookingwithTym(startTime, endTime, minhrs, eres, tym) {

    let sTime = parseTime(convertTime12to24(startTime));
    let eTime = parseTime(convertTime12to24(endTime));
    let timeSlots = calculate_time_slot(sTime, eTime, 60);
    let firstTimeSlot = "";
    var endtimedata = [];
    $.each(timeSlots, function (i, e) {
        //if (i === 0)
        //    firstTimeSlot = e.id;
        let disableEndTime = false;

         disableEndTime = parseTime(e.id) <= parseTime(tym);
        // let disableEndTime = (parseTime(e.id) - parseTime(firstTimeSlot)) < (minhrs * 60);

        $.each(eres, function (ri, re) {
            if (parseTime(e.id) >= parseTime(re.start) && parseTime(e.id) <= parseTime(re.end) && parseTime(re.start) != parseTime(tym) && parseTime(re.end) != parseTime($("#Booking_EndTime").val())) {
                disableEndTime = true;
            }            
        });
        endtimedata.push({
            id: e.id,
            text: convertFrom24To12(e.id),
            disabled: disableEndTime
        });
    });
    if ($('#EndTime').hasClass("select2-hidden-accessible")) $("#EndTime").select2('destroy').empty().select2({ data: endtimedata }).val(convertTime12to24($("#Booking_EndTime").val())).trigger('change');
    else $("#EndTime").empty().select2({ data: endtimedata }).val(convertTime12to24($("#Booking_EndTime").val())).trigger('change');
}