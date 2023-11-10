"use strict";
var ServiceDataTable = {
    init: function () {
        var t;
        t = $(".kt-datatable").KTDatatable({
            data: {
                type: "remote",
                source: {
                    read: {
                        url: location.protocol + "//" + location.host + "/Admin/Service/GetServiceList",
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
                field: "name",
                title: "Name",
                autoHide: !1
            },
            {
                field: "enableTwoStepBooking",
                title: "Two Step Booking",
                autoHide: !1,
                template: function (t) {
                    var e = {
                        false: {
                            title: "No",
                            class: "info"
                        },
                        true: {
                            title: "Yes",
                            class: "warning"
                        }
                    };
                    return '<span class="label label-' + e[t.enableTwoStepBooking].class + ' label-pill label-inline">' + e[t.enableTwoStepBooking].title + '</span>';
                }
            },
            {
                field: "serviceTypeName",
                title: "Type",
                autoHide: !1
            }, {
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
                    html += '<a href="/Admin/Service/Edit/' + t.id + '" class="dropdown-item" title="Edit Details"><i class="flaticon2-edit"></i><span style="margin: 3px;">Edit</span></a>';
                    html += '<a href="javascript:;" id="btnDelete" class="dropdown-item" data-target="' + t.id + '" controller="Admin/Service" action="Delete" title="Delete"><i class="flaticon2-trash"></i><span style="margin: 3px;">Delete</span></a>';
                    html += '</div></div>';
                    return html;
                }
            }]
        });
    }
};
