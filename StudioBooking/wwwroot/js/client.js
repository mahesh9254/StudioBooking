"use strict";
var ClientDataTable = {
    init: function () {
        var t;
        t = $(".kt-datatable").KTDatatable({
            data: {
                type: "remote",
                source: {
                    read: {
                        url: location.protocol + "//" + location.host + "/Admin/Client/GetClients",
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
                field: "order",
                title: "#",
                sortable: "asc",
                width: 30,
                type: "number",
                selector: !1,
                textAlign: "center"
            },
            {
                field: "name",
                title: "Name",
                autoHide: !1
            },
            {
                field: "image",
                title: "Image",
                template: function (t) {
                    if (t.image) { return '<img alt="Client Image" style="max-width:100px;" src="' + t.image + '" />' } else return "";
                }
            },
            {
                field: "isActive",
                title: "Status",
                template: function (t) {
                    var e = {
                        false: {
                            title: "InActive",
                            class: "danger"
                        },
                        true: {
                            title: "Active",
                            class: "success"
                        }
                    };
                    return '<span class="label label-' + e[t.isActive].class + ' label-pill label-inline">' + e[t.isActive].title + '</span>';
                }
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
                    html += '<a href="/Admin/Client/Edit/' + t.id + '" class="dropdown-item" title="Edit"><i class="flaticon2-edit"></i><span style="margin: 3px;">Edit</span></a>';
                    html += '<a href="javascript:;" id="btnDelete" class="dropdown-item" data-target="' + t.id + '" controller="Admin/Client" action="Delete" title="Delete"><i class="flaticon2-trash"></i><span style="margin: 3px;">Delete</span></a>';
                    html += '</div></div>';
                    return html;
                }
            }]
        });
    }
};
