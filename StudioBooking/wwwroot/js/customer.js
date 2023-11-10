"use strict";
var UserDataTable = {
    init: function () {
        var t;
        t = $(".kt-datatable").KTDatatable({
            data: {
                type: "remote",
                source: {
                    read: {
                        url: location.protocol + "//" + location.host + "/Admin/Account/CustomerList",
                        method: 'GET',
                        map: function (t) {
                            var e = t;
                            return void 0 !== t.users && (e = t.users),
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
            sortable: !0,
            pagination: !0,
            search: {
                input: $("#kt_datatable_search_query"),
                key: "generalSearch"
            },
            columns: [
                {
                    field: "Customer ID",
                    title: "Customer ID",
                    autoHide: !1,
                    template: function (t) { return t.user.customerId }
                },
                {
                field: "Name",
                title: "Name",
                autoHide: !1,
                template: function (t) { return t.user.name }
            }, {
                field: "Email",
                title: "Email",
                template: function (t) { return t.user.email }
            }, {
                field: "Mobile",
                title: "Mobile",
                template: function (t) { return t.user.mobile === undefined ? "" : t.user.mobile }
            },
            {
                field: "CreatedDate",
                title: "Created Date",
                template: function (t) { return t.user.createdDate === undefined ? "" : t.user.createdDate }
            },
            {
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
                    html += '<a href="/Admin/Account/Update/' + t.user.userId + '" class="dropdown-item" title="Edit Details"><i class="flaticon2-edit"></i><span style="margin: 3px;">Edit</span></a>';
                    html += '<a href="javascript:;" id="btnDelete" class="dropdown-item" data-target="' + t.user.userId + '" controller="Admin/Account" action="Delete" title="Delete"><i class="flaticon2-trash"></i><span style="margin: 3px;">Delete</span></a>';
                    html += '</div></div>';
                    return html;
                }
            }]
        });
    }
};
