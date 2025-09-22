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
                url: "/WorkOder/WorkOrderHead",
                method: "GET",
                dataType: "json"
            }).always(() => $loadPanel.hide());
        },

        insert: function (values) {
            return $.ajax({
                url: "/WorkOder/AddWorkOrder",
                method: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(values)
            });
        },

        update: function (key, values) {
            return $.ajax({
                url: `${API_BASE}/${encodeURIComponent(key)}`,
                method: "PUT",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(values)
            });
        },

        remove: function (key) {
            return $.ajax({
                url: `${API_BASE}/${encodeURIComponent(key)}`,
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
                dataField: "createDateTime",
                caption: "建立時間",
                dataType: "string", // 後端若改 ISO，可以把這改成 "datetime" 並設定 format
                allowEditing: false,
                width: 200
            },
            {
                dataField: "number",
                caption: "單號",
                dataType: "string",
                validationRules: [{ type: "required" }]
            },
            {
                dataField: "productModel",
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
                dataField: "prodcutNumberHead",
                caption: "序號標頭",
                dataType: "string",
            },
            {
                dataField: "prodcutQuantity", // 注意拼字與後端一致
                caption: "數量",
                dataType: "number",
                editorOptions: { min: 0 },
                validationRules: [{ type: "required" }, { type: "range", min: 0 }]
            },
            { 
                dataField: "description",
                caption: "說明", 
                dataType: "string" 
            },
            { 
                dataField: "create", 
                caption: "建立者", 
                dataType: "number", 
                allowEditing: false, 
                width: 100,
                 visible: false
            }
        ],

        // ★ Master-Detail：用列的 id 叫子資料
        masterDetail: {
            enabled: true,
            template: function (detailContainer, detailInfo) {
                // detailInfo.data 就是該列資料，這裡取主鍵 id 當 headId
                const headId = detailInfo.data.id;

                // 建立內層容器
                const $inner = $("<div>").addClass("detail-grid-wrapper").appendTo(detailContainer);

                // 子表資料來源（讀取用 headId 過濾）
                const detailStore = new DevExpress.data.CustomStore({
                    key: "id", // 如果子表主鍵不是 id，請改成正確的欄位名
                    load: function () {
                        return $.ajax({
                            url: "/WorkOder/OneWorkOrderBody/" + headId,
                            method: "GET",
                            dataType: "json",
                        });
                    }
                });

                // 子 Grid（預設 read-only；若要 CRUD 可再加 editing 設定與對應 API）
                $inner.dxDataGrid({
                    dataSource: detailStore,
                    showBorders: true,
                    columnAutoWidth: true,
                    wordWrapEnabled: true,
                    noDataText: "無子資料",
                    paging: { pageSize: 5 },
                    pager: { showInfo: true, visible: true },
                    columns: [
                        {
                            caption: 'ID',
                            dataField: 'id',
                            alignment: 'center',
                            visible: false
                        },
                        {
                            caption: '建立時間',
                            dataField: 'createDateTime',
                            alignment: 'center',
                            visible: false
                        },
                        {
                            caption: '建立者',
                            dataField: 'create',
                            alignment: 'center',
                            visible: false
                        },
                        {
                            caption: '產品序號',
                            dataField: 'productNumber',
                            alignment: 'center',
                        },
                        {
                            caption: '是否已測試',
                            dataField: 'isTest',
                            alignment: 'center',
                            lookup: {
                                dataSource: [
                                    { id: 0, name: '未測試' },
                                    { id: 1, name: '已測試' },
                                    { id: 2, name: '未知' }
                                ],
                                valueExpr: 'id',
                                displayExpr: 'name'
                            }
                        },
                        {
                            caption: '最後測試結果',
                            dataField: 'lastIsPass',
                            alignment: 'center',
                            cellTemplate: function (container, options) {
                                const statusMap = {
                                    0: { text: "未通過", color: "red" },
                                    1: { text: "通過", color: "green" },
                                    2: { text: "未知", color: "black" }
                                };

                                const status = statusMap[options.value] || { text: "未知", color: "black" };

                                const $icon = $("<div>")
                                    .css({
                                        display: "inline-block",
                                        width: "10px",
                                        height: "10px",
                                        backgroundColor: status.color,
                                        borderRadius: "50%",
                                        marginRight: "8px"
                                    });

                                $(container)
                                    .append($icon)
                                    .append($("<span>").text(status.text));
                            },
                            lookup: {
                                dataSource: [
                                    { id: true, name: '通過' },
                                    { id: false, name: '未通過' }
                                ],
                                valueExpr: 'id',
                                displayExpr: 'name'
                            }
                        },
                        {
                            caption: '最後測試時間',
                            dataField: 'lastTestDateTime',
                            alignment: 'center',
                        },
                        {
                            caption: '最後測試人員',
                            dataField: 'lastTestUser',
                            alignment: 'center',
                            visible: false
                        },
                        {
                            caption: '描述',
                            dataField: 'description',
                            alignment: 'center'
                        }
                    ]
                });
            }
        },

        onSaving: function (e) {
            e.cancel = false;
        },
        onDataErrorOccurred: function (e) {
            DevExpress.ui.notify(e.error?.message || "資料存取發生錯誤", "error", 4000);
        }
    });
});
