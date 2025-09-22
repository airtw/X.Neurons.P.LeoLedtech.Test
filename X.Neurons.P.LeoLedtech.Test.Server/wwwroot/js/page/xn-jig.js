$(function () {
    // 可選：載入中面板
    const $loadPanel = $(".xn-load-panel").dxLoadPanel({
        shading: true,
        message: "資料載入中…",
        visible: false
    }).dxLoadPanel("instance");

    // API 端點
    const API_BASE = "/WorkOder";

    // 主表 CustomStore — AJAX CRUD
    const store = new DevExpress.data.CustomStore({
        key: "id",

        load: function () {
            $loadPanel.show();
            return $.ajax({
                url: "/Jig",
                method: "GET",
                dataType: "json"
            }).always(() => $loadPanel.hide());
        },

        insert: function (values) {
            return $.ajax({
                url: "/Jig",
                method: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(values)
            });
        },

        update: function (key, values) {
            // 將 key 合併到 values 中
            const data = Object.assign({ id: key }, values);
            
            return $.ajax({
                url: `/Jig`,
                method: "PUT",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(data)
            });
        },

        remove: function (key) {
            return $.ajax({
                url: `Jig/${encodeURIComponent(key)}`,
                method: "DELETE",
                dataType: "json"
            });
        }
    });

    const productModelStore = new DevExpress.data.CustomStore({
        key: "id",

        // 載入全部清單（最簡單：後端一次回傳所有型號）
        load: function () {
            return $.ajax({
                url: "/Product",
                method: "GET",
                dataType: "json"
            });
        },

        // 編輯時根據 value 顯示對應文字會用到
        byKey: function (key) {
            return $.ajax({
                url: `/WorkOder/ProductModels/${encodeURIComponent(key)}`,
                method: "GET",
                dataType: "json"
            });
        }
    });

    // 初始化主 Grid（含 Master-Detail）
    const $grid = $(".workorder-grid").dxDataGrid({
        dataSource: store,
        keyExpr: "id",
        showBorders: true,
        height: "auto",
        columnAutoWidth: true,
        rowAlternationEnabled: true,
        allowColumnResizing: true,
        columnResizingMode: "widget",
        // sorting: { mode: "multiple" },
        headerFilter: { visible: true },
        filterRow: { visible: true },
        // searchPanel: { visible: true, placeholder: "搜尋…" },
        paging: { pageSize: 30 },
        pager: { showInfo: true, showNavigationButtons: true },

        editing: {
            mode: "row",                // "row" | "cell" | "popup"
            allowAdding: true,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true
        },

        // 主表欄位：依你給的 JSON
        columns: [
            {
                dataField: "id",
                caption: "ID",
                dataType: "number",
                allowEditing: false,
                width: 90,
                visible: false
            },
            {
                dataField: "number",
                caption: "治具序號",
                dataType: "string", // 後端若改 ISO，可以把這改成 "datetime" 並設定 format
            },
            {
                dataField: "productId",
                caption: "產品型號",
                dataType: "number",
                validationRules: [{ type: "required" }],
                lookup: {
                    dataSource: {
                        store: productModelStore,   // 用上面 AJAX store
                        // 若清單不大，以下可省略；若很多可設定分頁
                        paginate: false
                    },
                    valueExpr: "id",
                    displayExpr: "model"
                },
                // 讓下拉可以搜尋/清除
                editorOptions: {
                    searchEnabled: true,
                    showClearButton: true,
                    minSearchLength: 0,
                    placeholder: "選擇型號"
                }
            },
            { 
                dataField: "description",
                caption: "說明", 
                dataType: "string" 
            }
        ],

        onSaving: function (e) {
            e.cancel = false;
        },
        onDataErrorOccurred: function (e) {
            DevExpress.ui.notify(e.error?.message || "資料存取發生錯誤", "error", 4000);
        }
    });
});
