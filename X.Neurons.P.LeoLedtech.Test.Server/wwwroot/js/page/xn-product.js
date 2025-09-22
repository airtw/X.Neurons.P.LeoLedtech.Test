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
                url: "/Product",
                method: "GET",
                dataType: "json"
            }).always(() => $loadPanel.hide());
        },

        insert: function (values) {
            return $.ajax({
                url: "/Product",
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
                url: `/Product`,
                method: "PUT",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(data)
            });
        },

        remove: function (key) {
            return $.ajax({
                url: `Product/${encodeURIComponent(key)}`,
                method: "DELETE",
                dataType: "json"
            });
        }
    });

 const testStepStore = DevExpress.data.AspNet.createStore({
    key: 'id',
    loadUrl: '/TestStep/TestStepHead',
    onBeforeSend: function (operation, ajaxSettings) {
        const token = localStorage.getItem('token');
        if (token) {
            ajaxSettings.headers = {
                'Authorization': `Bearer ${token}`
            };
        }
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
                dataField: "model",
                caption: "產品名稱",
                dataType: "string",
                allowEditing: true,
            },
            {
                dataField: "testStepId",
                caption: "測試模式",
                dataType: "number",
                validationRules: [{ type: "required" }],
                lookup: {
                    dataSource: {
                        store: testStepStore,   // 用上面 AJAX store
                        // 若清單不大，以下可省略；若很多可設定分頁
                        paginate: false
                    },
                    valueExpr: "id",
                    displayExpr: "name"
                },
                // 讓下拉可以搜尋/清除
                editorOptions: {
                    searchEnabled: true,
                    showClearButton: true,
                    minSearchLength: 0,
                    placeholder: "選擇模式"
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
