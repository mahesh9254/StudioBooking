var defaultErrorMessage = "An error occured when processing your request. If the problem persists, please contact to admin. Error: ";
$(document).on("click", "#btnDelete", function () {
    var button = $(this);
    let resource = JSON.parse(localStorage.getItem("bt-box-resource"));
    bootbox.confirm({
        title: resource.Confirm,
        message: resource.delete_confirm_msg,
        buttons: {
            cancel: {
                label: '<i class="fa fa-times"></i> ' + resource.No
            },
            confirm: {
                label: '<i class="fa fa-check"></i> ' + resource.Yes
            }
        },
        callback: function (result) {
            if (result) {
                $.get("/" + button.attr("controller") + (button.attr("action").length === 0 ? "/Delete?id=" : "/" + button.attr("action") + "?id=") + button.attr("data-target"))
                    .done(function (data) {
                        if (data.result) {
                            $(".kt-datatable").KTDatatable().reload();
                            toastr.success("Record deleted successfully.");
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
});

var baseController = function () {

    actionResult = function (edit, result, errorMessage, url) {

        if (result) {
            bootbox.alert("Data " + (edit === "Update" ? "updated" : "saved") + " successfully.", function () { reload(url); });
        }
        if (errorMessage) {
            toastr.error(defaultErrorMessage + errorMessage);
        }
    },

        reload = function (url) {
            window.location = url;
        };

    return {
        reload: reload,
        actionResult: actionResult
    };
}();

var KTBootstrapDatepicker = function () {
    var t;
    t = KTUtil.isRTL() ? {
        leftArrow: '<i class="la la-angle-right"></i>',
        rightArrow: '<i class="la la-angle-left"></i>'
    } : {
        leftArrow: '<i class="la la-angle-left"></i>',
        rightArrow: '<i class="la la-angle-right"></i>'
    };
    return {
        init: function () {

            $(".datepicker").datepicker({
                rtl: KTUtil.isRTL(),
                todayHighlight: !0,
                format: "dd/mm/yyyy",
                orientation: "bottom left",
                templates: t
            });
        }
    };
}();

var KTImageUpload = function () {
    return {
        init: function (ctrl) {
            var t = new KTImageInput(ctrl);
            t.on("cancel", (function (t) {
                swal.fire({
                    title: "Image successfully canceled !",
                    type: "success",
                    buttonsStyling: !1,
                    confirmButtonText: "Awesome!",
                    confirmButtonClass: "btn btn-primary font-weight-bold"
                })
            }
            )),
                t.on("change", (function (t) {
                    swal.fire({
                        title: "Image successfully changed !",
                        type: "success",
                        buttonsStyling: !1,
                        confirmButtonText: "Awesome!",
                        confirmButtonClass: "btn btn-primary font-weight-bold"
                    })
                }
                )),
                t.on("remove", (function (t) {
                    swal.fire({
                        title: "Image successfully removed !",
                        type: "error",
                        buttonsStyling: !1,
                        confirmButtonText: "Got it!",
                        confirmButtonClass: "btn btn-primary font-weight-bold"
                    })
                }
                ));
        }
    }
}();


function convertHours(mins) {
    var hour = Math.floor(mins / 60);
    var mins = mins % 60;
    var converted = pad(hour, 2) + ':' + pad(mins, 2);
    return converted;
}

function pad(str, max) {
    str = str.toString();
    return str.length < max ? pad("0" + str, max) : str;
}

function calculate_time_slot(start_time, end_time, interval = "30") {
    var i, formatted_time;
    var time_slots = [];
    for (var i = start_time; i <= end_time; i = i + interval) {
        formatted_time = convertHours(i);
        time_slots.push({ id: formatted_time, text: formatted_time });
    }
    return time_slots;
}

function convertTime12to24(time12h) {
    if (time12h) {
        const [time, modifier] = time12h.split(' ');
        if (!modifier)
            return time12h;
        let [hours, minutes] = time.split(':');
        if (hours === '12') {
            hours = '00';
        }
        if (modifier === 'PM') {
            hours = parseInt(hours, 10) + 12;
        }
        return `${hours.length == 1 ? '0' + hours : hours}:${minutes}`;
    }
}

function convertFrom24To12(time24) {
    const [sHours, minutes] = time24.match(/([0-9]{1,2}):([0-9]{2})/).slice(1);
    const period = +sHours < 12 ? 'AM' : 'PM';
    const hours = +sHours % 12 || 12;
    return `${hours}:${minutes} ${period}`;
}

function parseTime(s) {
    var c = s.split(':');
    return parseInt(c[0]) * 60 + parseInt(c[1]);
}