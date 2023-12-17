"use strict";
var KTTransactionList = function () {
    return {
        init: function () {
            o = document.getElementById("kt_table_transactions");
            o && (o.querySelectorAll("tbody tr").forEach((e => {
                const t = e.querySelectorAll("td")
                const l = moment(t[2].innerHTML, "MM-DD-YYYY, LT").format();
                t[2].setAttribute("data-order", l)
            }
            )),
                (e = $(o).DataTable({
                    info: !1,
                    order: [],
                    pageLength: 10,
                    lengthChange: !1,
                    columnDefs: [{
                        orderable: !1,
                        targets: 0
                    }]
                }))
            )
        }
    }
}();
KTUtil.onDOMContentLoaded((function () {
    KTTransactionList.init()
}
));
