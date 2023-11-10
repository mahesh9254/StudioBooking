"use strict";
var ServiceGalleryDataTable = {
    init: function () {
        var t;
        t = $(".kt-datatable").KTDatatable({
            data: {
                type: "remote",
                source: {
                    read: {
                        url: location.protocol + "//" + location.host + "/Admin/Website/ServiceGalleryList",
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
                field: "image",
                title: "Image",
                template: function (t) {
                    if (t.image) { return '<img alt="Gallery Image" style="max-width:100px;" src="' + t.image + '" />' } else return "";
                }
            }, {
                field: "orderId",
                title: "order",
                sortable: "asc",
                width: 30,
                type: "number",
                selector: !1
            },
            {
                field: "title",
                title: "Title",
                autoHide: !1
            }, {
                field: "subTitle",
                title: "SubTitle",
                autoHide: !1
            }, {
                field: "isActive",
                title: "IsActive",
                template: function (t) {
                    if (t.isActive) { return "Yes" } else return "No";
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
                    html += '<a href="/Admin/Website/EditServiceGallery/' + t.id + '" class="dropdown-item" title="Edit Details"><i class="flaticon2-edit"></i><span style="margin: 3px;">Edit</span></a>';
                    html += '<a href="javascript:;" id="btnDelete" class="dropdown-item" data-target="' + t.id + '" controller="Admin/Website" action="DeleteServiceGallery" title="Delete"><i class="flaticon2-trash"></i><span style="margin: 3px;">Delete</span></a>';
                    html += '</div></div>';
                    return html;
                }
            }]
        });
    }
};
