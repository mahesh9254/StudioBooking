"use strict";
var ServicePriceDataTable = {
    init: function () {
        var t;
        t = $(".kt-datatable").KTDatatable({
            data: {
                type: "remote",
                source: {
                    read: {
                        url: location.protocol + "//" + location.host + "/Admin/ServicePrice/GetServicePriceList",
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
                scroll: !1,
                footer: !1
            },
            noRecords: 'No records found',
            sortable: true,
            pagination: true,
            search: {
                input: $("#kt_datatable_search_query"),
                key: "generalSearch"
            },
            columns: [{
                field: "id",
                title: "#",
                sortable: "asc",
                width: 30,
                type: "number",
                selector: !1,
                textAlign: "center"
            }, {
                field: "categoryName",
                title: "Studio Name",
                autoHide: !1
            },
            {
                field: "serviceName",
                title: "Service Name",
                autoHide: !1
            },
            {
                field: "price",
                title: "Price",
                autoHide: !1
            },
            {
                field: "minHours",
                title: "MinHours",
                autoHide: !1
            },
            {
                field: "eventColorId",
                title: "EventColor",
                template: function (t) { return '<span class="label label-xl label-inline label-rounded" style="background-color:' + t.eventColorId + ';width:28px"></span>'; }
            },
            {
                field: "disableBooking",
                title: "Booking Status",
                autoHide: !1,
                template: function (t) {
                    var e = {
                        true: {
                            title: "Disabled",
                            class: "danger"
                        },
                        false: {
                            title: "Enabled",
                            class: "success"
                        }
                    };
                    return '<span class="label label-' + e[t.disableBooking].class + ' label-pill label-inline" onClick="ServicePriceDataTable.ToggleBookingStatus(' + t.id + ',' + t.disableBooking + ')">' + e[t.disableBooking].title + '</span>';
                }
            },
            {
                field: "createdDate",
                sortable: true,
                title: "CreatedDate"
            }, {
                field: "Actions",
                title: "",
                sortable: !1,
                width: 30,
                overflow: "visible",
                autoHide: !1,
                template: function (t) {
                    let html = '<div class="dropdown dropdown-inline dropleft" data-toggle="kt-tooltip" title="Actions" data-placement="left">';
                    html += '<a href="javascript:;" class="btn btn-light-primary btn-icon btn-sm" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
                    html += '<i class="ki ki-bold-more-ver"></i></a>';
                    html += '<div class="dropdown-menu dropdown-menu-sm">';
                    html += '<a href="/Admin/ServicePrice/Edit/' + t.id + '" class="dropdown-item" title="Edit Details"><i class="flaticon2-edit"></i><span style="margin: 3px;">Edit</span></a>';
                    html += '<a href="javascript:;" id="btnDelete" class="dropdown-item" data-target="' + t.id + '" controller="Admin/ServicePrice" action="Delete" title="Delete"><i class="flaticon2-trash"></i><span style="margin: 3px;">Delete</span></a>';
                    html += '</div></div>';
                    return html;
                }
            }]
        });
    },
    ToggleBookingStatus: function (id, status) {
        bootbox.confirm({
            title: "Confirm",
            message: "Are you sure you want to " + (status == true ? 'enable' : 'disable') + " booking?",
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
                    $.get("/Admin/ServicePrice/ToggleBookingStatus?id=" + id + "&status=" + status)
                        .done(function (data) {
                            if (data.result) {
                                $(".kt-datatable").KTDatatable().reload();
                                toastr.success("Booking " + status == true ? 'disabled' : 'enabled' + " successfully.");
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
