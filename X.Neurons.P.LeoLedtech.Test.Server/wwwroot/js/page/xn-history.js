$(() => {
    const initialFormData = {
        StartTime: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000),
        // 其他初始值...
    };

    var historyQueryForm = $('#histroy-query-form').dxForm({
        formData: initialFormData,
        readOnly: false,
        showColonAfterLabel: false,
        validationGroup: 'customerDataA',
        onFieldDataChanged: (e) => {

            if (e.dataField === "StartTime") {
                const endTimeEditor = historyQueryForm.getEditor('EndTime');
                if (endTimeEditor) {
                    endTimeEditor.option('min', e.value);

                    const currentEndTime = endTimeEditor.option('value');
                    if (currentEndTime && currentEndTime < e.value) {
                        endTimeEditor.option('value', null);
                    }
                }
            }
        },
        colCountByScreen: {
            xs: 1,
            sm: 1,
            md: 1,
            lg: 4,
        },
        items: [{
            dataField: 'WorkOderNumber',
            label: {
                text: "工單號",
                location: 'top'
            },
            editorType: 'dxSelectBox',
            editorOptions: {
                dataSource: DevExpress.data.AspNet.createStore({
                    key: 'id',
                    loadUrl: '/WorkOder/WorkOrderHead',
                    onBeforeSend: function (operation, ajaxSettings) {
                        const token = localStorage.getItem('token');
                        ajaxSettings.headers = {
                            'Authorization': `Bearer ${token}`
                        };
                    }
                }),
                displayExpr: 'number',
                valueExpr: 'id',
                searchEnabled: true,
                onValueChanged: function (e) {

                    // 當工單號有值時，禁用產品序號；沒值時啟用產品序號
                    const formInstance = historyQueryForm; // 取得表單實例

                    if (formInstance) {

                        const productionField = formInstance.getEditor('PrdouectionNumber');
                        if (productionField) {
                            productionField.option('disabled', !!e.value);
                            // 如果工單號有值，清空產品序號
                            if (e.value) {
                                formInstance.updateData('PrdouectionNumber', '');
                            }
                        }
                    }
                },
                onLoadError: function (error) {
                    DevExpress.ui.notify({
                        message: '載入工單號列表失敗',
                        position: {
                            my: 'center top',
                            at: 'center top',
                        },
                    }, 'error', 3000);
                }
            }
        }, {
            dataField: 'PrdouectionNumber',
            label: {
                text: "產品序號",
                location: 'top'
            },
            editorType: 'dxTextBox',
            editorOptions: {
                placeholder: '請輸入產品序號',
                onValueChanged: function (e) {
                    // 當產品序號有值時，禁用工單號；沒值時啟用工單號
                    const formInstance = historyQueryForm; // 取得表單實例
                    if (formInstance) {
                        const workOrderField = formInstance.getEditor('WorkOderNumber');
                        if (workOrderField) {
                            workOrderField.option('disabled', !!e.value);
                            // 如果產品序號有值，清空工單號
                            if (e.value) {
                                formInstance.updateData('WorkOderNumber', null);
                            }
                        }
                    }
                }
            }
        }, {
            dataField: 'StartTime',
            label: {
                text: '開始時間',
                location: 'top'
            },
            editorType: 'dxDateBox',
            editorOptions: {
                type: "datetime",
                displayFormat: "yyyy-MM-dd HH:mm:ss",
                dateSerializationFormat: "yyyy-MM-ddTHH:mm:ssZ",
                useMaskBehavior: true,
            },
            validationRules: [{
                type: 'required',
                message: '開始時間為必填',
            }],
        }, {
            dataField: 'EndTime',
            label: {
                text: '結束時間',
                location: 'top'
            },
            editorType: 'dxDateBox',
            editorOptions: {
                type: "datetime",
                displayFormat: "yyyy-MM-dd HH:mm:ss",
                dateSerializationFormat: "yyyy-MM-ddTHH:mm:ssZ",
                useMaskBehavior: true,
            },
            validationRules: [{
                type: 'required',
                message: '結束時間為必填',
            }, {
                type: 'custom',
                message: '結束時間必須晚於開始時間',
                validationCallback: function (e) {
                    const startTime = historyQueryForm.getEditor('StartTime').option('value');
                    return !startTime || !e.value || e.value > startTime;
                }
            }],
        }, {
            itemType: 'button',
            name: 'BatchPUT',
            cssClass: 'batch-put-button',
            buttonOptions: {
                text: '查詢',
                onClick: () => {

                    const formInstance = $('#histroy-query-form').dxForm('instance');

                    const validateResult = formInstance.validate();

                    if (validateResult.isValid) {

                        const formData = formInstance.option('formData');

                        updateHistoryGridData(formData);
                    } else {
                        DevExpress.ui.notify({
                            message: '請填寫完整資料',
                            position: {
                                my: 'center top',
                                at: 'center top',
                            },
                        }, 'error', 3000);
                    }
                },
                type: 'default',
                icon: 'find',
                useSubmitBehavior: false,
                disabled: false,
                width: 'auto',
                height: '2.2rem',
            },
        }
        ]
    }).dxForm('instance');

    function updateHistoryGridData(formData) {
        const token = localStorage.getItem('token');

        if (!token) {
            DevExpress.ui.notify({
                message: '未登入或授權已過期，請重新登入',
                position: {
                    my: 'center top',
                    at: 'center top',
                },
            }, 'error', 3000);
            return;
        }

        const workOderNumber = historyQueryForm.getEditor('WorkOderNumber').option('selectedItem');
        const productNumber = historyQueryForm.getEditor('PrdouectionNumber').option('value');
        // const selectedItem = equipmentSelectBox.option('selectedItem');
        // const equipmentName = selectedItem ? selectedItem.name : '';

        const loadPanel = $('.xn-load-panel').dxLoadPanel({
            shadingColor: 'rgba(0,0,0,0.4)',
            message: '載入中...',
            showIndicator: true,
            showPane: true,
            visible: true,
            position: { of: '.history-result-content' }
        }).dxLoadPanel('instance');

        const requestData = {
            WorkOderNumber: workOderNumber,
            ProductNumber: productNumber,
            StartTime: moment(formData.StartTime).format('YYYY-MM-DD HH:mm:ss'),
            EndTime: moment(formData.EndTime).format('YYYY-MM-DD HH:mm:ss')
        };

        $.ajax({
            url: '/WorkOder/WorkOrderBody',
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            data: JSON.stringify(requestData),
            complete: function () {
                loadPanel.hide();
            },
            success: function (response) {
                historyGrid.option('dataSource', response);

                DevExpress.ui.notify({
                    message: '資料更新成功',
                    position: {
                        my: 'center top',
                        at: 'center top',
                    },
                }, 'success', 3000);
            },
            error: function (xhr, status, error) {
                let errorMessage = '資料載入失敗';

                if (xhr.status === 401) {
                    errorMessage = '授權已過期，請重新登入';
                } else if (xhr.status === 403) {
                    errorMessage = '沒有權限存取此資料';
                } else if (xhr.status === 400) {
                    errorMessage = '請求參數錯誤';
                }

                DevExpress.ui.notify({
                    message: errorMessage,
                    position: {
                        my: 'center top',
                        at: 'center top',
                    },
                }, 'error', 3000);
            }
        });
    }

    const historyGrid = $('.history-result-grid').dxDataGrid({
        dataSource: [],
        showBorders: true,
        showRowLines: true,
        columnAutoWidth: true,
        allowColumnResizing: true,
        loadPanel: {
            enabled: true,
        },
        scrolling: {
            mode: 'virtual',
        },
        filterRow: {
            visible: true
        },
        export: {
            enabled: true,
        },
        onExporting(e) {

            const dataGridInstance = $('.history-result-grid').dxDataGrid('instance');

            const dataSource = dataGridInstance.getDataSource();

            const data = dataSource.items();
            if (data.length >= 1) {
                const equipmentSelectBox = historyQueryForm.getEditor('EquipmentName');
                const selectedItem = equipmentSelectBox.option('selectedItem');
                const equipmentName = selectedItem ? selectedItem.name : '';

                const workbook = new ExcelJS.Workbook();
                const worksheet = workbook.addWorksheet(equipmentName);

                DevExpress.excelExporter.exportDataGrid({
                    component: e.component,
                    worksheet,
                    autoFilterEnabled: true,
                }).then(() => {
                    workbook.xlsx.writeBuffer().then((buffer) => {
                        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), equipmentName + '.xlsx');
                    });
                });
            }



        },
        onContentReady(e) {
            e.component.option('loadPanel.enabled', false);
        },
        onCellPrepared: function (e) {
            if (e.rowType === 'data') {
                e.cellElement.css('vertical-align', 'middle');
            }
        },
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
            },
            {
                caption: '操作',
                alignment: 'center',
                width: 'auto',
                allowFiltering: false,
                allowSorting: false,
                cellTemplate: function (container, options) {
                    // 測試紀錄按鈕
                    $('<div>').dxButton({
                        text: '測試紀錄',
                        type: 'normal',
                        icon: 'doc',
                        onClick: function () {
                            showTestRecordPopup(options.data.productNumber, options.data.id);
                        }
                    }).css('margin-right', '5px').appendTo(container);
                    $('<div>').dxButton({
                        text: '下載報告',
                        type: 'default',
                        icon: 'download',
                        onClick: function () {
                            downloaLastTestReport(options.data.id);
                        }
                    }).appendTo(container);
                }
            }
        ]
    }).dxDataGrid('instance');

    // 下載測試報告
    function downloaLastTestReport(productId) {
        const loadingIndicator = DevExpress.ui.notify({
            message: '正在生成測試報告...',
            position: { my: 'center top', at: 'center top' },
        }, 'info', 0);


        $.ajax({
            url: '/TestOder/LastTestReport/' + productId, // 替換為您的測試報告下載 API 端點
            type: 'GET',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`,
                'Content-Type': 'application/json'
            },
            // data: JSON.stringify(postData),
            xhrFields: {
                responseType: 'blob'
            },
            success: function (data, textStatus, xhr) {
                // loadingIndicator.hide();

                let filename = `test_report_${productId}.xlsx`;
                const contentDisposition = xhr.getResponseHeader('Content-Disposition');
                if (contentDisposition) {
                    const matches = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(contentDisposition);
                    if (matches != null && matches[1]) {
                        filename = matches[1].replace(/['"]/g, '');
                    }
                }

                const blob = new Blob([data], {
                    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
                });
                saveAs(blob, filename);

                DevExpress.ui.notify({
                    message: '測試報告下載成功',
                    position: { my: 'center top', at: 'center top' },
                }, 'success', 3000);
            },
            error: function (xhr, textStatus, errorThrown) {
                loadingIndicator.hide();

                let errorMessage = '測試報告下載失敗';
                try {
                    const errorResponse = JSON.parse(xhr.responseText);
                    errorMessage = errorResponse.message || errorMessage;
                } catch (e) {
                    errorMessage = `下載失敗: ${xhr.status} ${xhr.statusText}`;
                }

                DevExpress.ui.notify({
                    message: errorMessage,
                    position: { my: 'center top', at: 'center top' },
                }, 'error', 5000);
            }
        });
    }

    // 測試紀錄 Popup 的實現
    function showTestRecordPopup(productNumber, productNumberId) {
        // 創建測試紀錄的 popup
        const testRecordPopup = $('.testRecordPopup').dxPopup({
            title: `${productNumber} - 測試紀錄`,
            width: '90%',
            height: '80%',
            visible: true,
            showCloseButton: true,
            contentTemplate: function (contentElement) {
                // 創建測試紀錄的 DataGrid
                const testRecordGrid = $('<div>').dxDataGrid({
                    dataSource: [],
                    showBorders: true,
                    showRowLines: true,
                    columnAutoWidth: true,
                    allowColumnResizing: true,
                    loadPanel: {
                        enabled: true,
                    },
                    scrolling: {
                        mode: 'virtual',
                    },
                    filterRow: {
                        visible: false
                    },
                    paging: {
                        pageSize: 20
                    },
                    onCellPrepared: function (e) {
                        if (e.rowType === 'data') {
                            e.cellElement.css('vertical-align', 'middle');
                        }
                    },
                    columns: [
                        {
                            caption: '測試時間',
                            dataField: 'createDateTime',
                            alignment: 'center',
                            dataType: 'datetime',
                            format: 'yyyy-MM-dd HH:mm:ss'
                        },
                        {
                            caption: '測試工站',
                            dataField: 'station',
                            alignment: 'center',
                            visible: false
                        },
                        {
                            caption: '測試人員',
                            dataField: 'testUser',
                            alignment: 'center',
                            visible: false
                        },
                        {
                            caption: '產品序號',
                            dataField: 'productNumber',
                            alignment: 'center',
                            visible: false
                        },
                        {
                            caption: '測試程式',
                            dataField: 'testModel',
                            alignment: 'center'
                        },
                        {
                            caption: '測試結果',
                            dataField: 'isPass',
                            alignment: 'center',
                            cellTemplate: function (container, options) {
                                const statusMap = {
                                    true: { text: "通過", color: "green" },
                                    false: { text: "未通過", color: "red" },
                                    1: { text: "通過", color: "green" },
                                    0: { text: "未通過", color: "red" }
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
                            caption: '操作',
                            alignment: 'center',
                            width: 'auto',
                            allowFiltering: false,
                            allowSorting: false,
                            cellTemplate: function (container, options) {
                                // 檢閱報告按鈕
                                $('<div>').dxButton({
                                    text: '檢閱報告',
                                    type: 'normal',
                                    icon: 'find',
                                    // width: 80,
                                    onClick: function () {
                                        showDetailReportPopup(options.data);
                                    }
                                }).css('margin-right', '5px').appendTo(container);

                                // 下載報告按鈕
                                $('<div>').dxButton({
                                    text: '下載報告',
                                    type: 'default',
                                    icon: 'download',
                                    // width: 80,
                                    onClick: function () {
                                        downloadTestReport(options.data);
                                    }
                                }).appendTo(container);
                            }
                        }
                    ]
                }).appendTo(contentElement);

                // 載入測試紀錄資料
                loadTestRecordData(productNumberId, testRecordGrid);

                return contentElement;
            },
            onHidden: function () {
                // testRecordPopup.remove();
                $('.testRecordPopup').empty();
            }
        });
    }

    // 載入測試紀錄資料
    function loadTestRecordData(productNumberId, gridElement) {
        const token = localStorage.getItem('token');

        if (!token) {
            DevExpress.ui.notify({
                message: '未登入或授權已過期，請重新登入',
                position: { my: 'center top', at: 'center top' },
            }, 'error', 3000);
            return;
        }

        $.ajax({
            url: '/TestOder/TestOrderHead/' + productNumberId, // 替換為您的測試紀錄 API 端點
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            success: function (response) {
                const gridInstance = gridElement.dxDataGrid('instance');
                gridInstance.option('dataSource', response);
                gridInstance.option('loadPanel.enabled', false);
            },
            error: function (xhr, status, error) {
                let errorMessage = '測試紀錄載入失敗';

                if (xhr.status === 401) {
                    errorMessage = '授權已過期，請重新登入';
                } else if (xhr.status === 403) {
                    errorMessage = '沒有權限存取此資料';
                } else if (xhr.status === 400) {
                    errorMessage = '請求參數錯誤';
                }

                DevExpress.ui.notify({
                    message: errorMessage,
                    position: { my: 'center top', at: 'center top' },
                }, 'error', 3000);
            }
        });
    }

    // 下載測試報告
    function downloadTestReport(testData) {
        console.log(testData);
        const loadingIndicator = DevExpress.ui.notify({
            message: '正在生成測試報告...',
            position: { my: 'center top', at: 'center top' },
        }, 'info', 0);

        const postData = {
            testId: testData.id,
            productNumber: testData.productNumber,
            testDateTime: testData.createDateTime
        };

        $.ajax({
            url: '/TestOder/TestReport/' + testData.id, // 替換為您的測試報告下載 API 端點
            type: 'GET',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`,
                'Content-Type': 'application/json'
            },
            // data: JSON.stringify(postData),
            xhrFields: {
                responseType: 'blob'
            },
            success: function (data, textStatus, xhr) {
                // loadingIndicator.hide();

                let filename = `test_report_${testData.productNumber}.xlsx`;
                const contentDisposition = xhr.getResponseHeader('Content-Disposition');
                if (contentDisposition) {
                    const matches = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(contentDisposition);
                    if (matches != null && matches[1]) {
                        filename = matches[1].replace(/['"]/g, '');
                    }
                }

                const blob = new Blob([data], {
                    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
                });
                saveAs(blob, filename);

                DevExpress.ui.notify({
                    message: '測試報告下載成功',
                    position: { my: 'center top', at: 'center top' },
                }, 'success', 3000);
            },
            error: function (xhr, textStatus, errorThrown) {
                loadingIndicator.hide();

                let errorMessage = '測試報告下載失敗';
                try {
                    const errorResponse = JSON.parse(xhr.responseText);
                    errorMessage = errorResponse.message || errorMessage;
                } catch (e) {
                    errorMessage = `下載失敗: ${xhr.status} ${xhr.statusText}`;
                }

                DevExpress.ui.notify({
                    message: errorMessage,
                    position: { my: 'center top', at: 'center top' },
                }, 'error', 5000);
            }
        });
    }

    function showDetailReportPopup(testData) {
        // 使用已定義的 HTML 元素來創建 popup
        const detailReportPopup = $('.detailReportPopup').dxPopup({
            title: `${testData.productNumber} - 測試詳細報告 (${testData.createDateTime})`,
            width: '95%',
            height: '90%',
            visible: true,
            showCloseButton: true,
            contentTemplate: function (contentElement) {
                // 清空之前的內容
                $(contentElement).empty();

                // 創建詳細報告的 DataGrid
                const detailReportGrid = $('<div>').dxDataGrid({
                    dataSource: [],
                    showBorders: true,
                    showRowLines: true,
                    columnAutoWidth: true,
                    allowColumnResizing: true,
                    loadPanel: {
                        enabled: true,
                    },
                    scrolling: {
                        mode: 'virtual',
                    },
                    filterRow: {
                        visible: false
                    },
                    paging: {
                        pageSize: 20
                    },
                    onCellPrepared: function (e) {
                        if (e.rowType === 'data') {
                            e.cellElement.css('vertical-align', 'middle');
                        }
                    },
                    columns: [
                        {
                            caption: '步驟ID',
                            dataField: 'id',
                            alignment: 'center',
                            width: 80
                        },
                        {
                            caption: '測試時間',
                            dataField: 'dateTime',
                            alignment: 'center',
                            width: 180
                        },
                        {
                            caption: '步驟詳情',
                            dataField: 'stepDetail',
                            alignment: 'center',
                            width: 300,
                            cellTemplate: function (container, options) {
                                const stepDetail = options.value || '';

                                // 解析步驟詳情
                                const stepMatch = stepDetail.match(/步驟:(\d+)/);
                                const voltageMatch = stepDetail.match(/輸出電壓:([0-9.]+)/);
                                const currentMatch = stepDetail.match(/輸出電流:([0-9.]+)/);
                                const passMatch = stepDetail.match(/是否通過:(.+)/);

                                const stepNum = stepMatch ? stepMatch[1] : '';
                                const voltage = voltageMatch ? voltageMatch[1] + 'V' : '';
                                const current = currentMatch ? currentMatch[1] + 'A' : '';
                                const isPass = passMatch ? passMatch[1] : '';

                                // 創建顯示內容
                                const $content = $('<div>').css('text-align', 'left');
                                $content.append($('<div>').text(`步驟: ${stepNum}`));
                                $content.append($('<div>').text(`電壓: ${voltage}`));
                                $content.append($('<div>').text(`電流: ${current}`));

                                // 通過狀態with顏色
                                const $passStatus = $('<div>').css('margin-top', '5px');
                                const passColor = isPass === '通過' ? 'green' : 'red';
                                const $icon = $('<span>').css({
                                    'display': 'inline-block',
                                    'width': '8px',
                                    'height': '8px',
                                    'background-color': passColor,
                                    'border-radius': '50%',
                                    'margin-right': '5px'
                                });
                                $passStatus.append($icon).append($('<span>').text(`狀態: ${isPass}`).css('color', passColor));
                                $content.append($passStatus);

                                $content.appendTo(container);
                            }
                        },
                        {
                            caption: '通道測試結果',
                            dataField: 'testOrderChannel',
                            alignment: 'center',
                            allowSorting: false,
                            cellTemplate: function (container, options) {
                                const channels = options.value || [];

                                if (channels.length === 0) {
                                    $(container).text('無資料');
                                    return;
                                }

                                // 創建通道資料表格
                                const $channelTable = $('<table>').css({
                                    'width': '100%',
                                    'border-collapse': 'collapse',
                                    'margin': '5px 0'
                                });

                                // 表頭
                                const $thead = $('<thead>');
                                const $headerRow = $('<tr>');
                                ['通道', '電壓', '電流', '功率'].forEach(header => {
                                    $headerRow.append(
                                        $('<th>').text(header).css({
                                            'border': '1px solid #ddd',
                                            'padding': '4px 8px',
                                            'background-color': '#f8f9fa',
                                            'font-weight': 'bold',
                                            'text-align': 'center',
                                            'font-size': '12px'
                                        })
                                    );
                                });
                                $thead.append($headerRow);
                                $channelTable.append($thead);

                                // 資料行
                                const $tbody = $('<tbody>');
                                channels.forEach(channel => {
                                    const $row = $('<tr>');

                                    // 通道號
                                    $row.append(
                                        $('<td>').text(`CH${channel.channel}`).css({
                                            'border': '1px solid #ddd',
                                            'padding': '4px 8px',
                                            'text-align': 'center',
                                            'font-size': '12px',
                                            'font-weight': 'bold'
                                        })
                                    );

                                    // 電壓
                                    $row.append(
                                        $('<td>').text(channel.voltage).css({
                                            'border': '1px solid #ddd',
                                            'padding': '4px 8px',
                                            'text-align': 'center',
                                            'font-size': '12px'
                                        })
                                    );

                                    // 電流
                                    $row.append(
                                        $('<td>').text(channel.current).css({
                                            'border': '1px solid #ddd',
                                            'padding': '4px 8px',
                                            'text-align': 'center',
                                            'font-size': '12px'
                                        })
                                    );

                                    // 功率
                                    const powerValue = channel.power && channel.power !== '0' ? channel.power + '' : '0W';
                                    $row.append(
                                        $('<td>').text(powerValue).css({
                                            'border': '1px solid #ddd',
                                            'padding': '4px 8px',
                                            'text-align': 'center',
                                            'font-size': '12px'
                                        })
                                    );

                                    $tbody.append($row);
                                });

                                $channelTable.append($tbody);
                                $channelTable.appendTo(container);
                            }
                        }
                    ],
                    // 設定行高以容納內嵌表格
                    onRowPrepared: function (e) {
                        if (e.rowType === 'data') {
                            e.rowElement.css('height', 'auto');
                        }
                    }
                }).appendTo(contentElement);

                // 載入詳細報告資料
                loadDetailReportData(testData.id, detailReportGrid);

                return contentElement;
            },
            onHidden: function () {
                // 清空內容但不移除元素
                $('.detailReportPopup').empty();
            }
        });
    }

    // 修改 loadDetailReportData 函數來處理新的資料格式
    function loadDetailReportData(testId, gridElement) {
        const token = localStorage.getItem('token');

        if (!token) {
            DevExpress.ui.notify({
                message: '未登入或授權已過期，請重新登入',
                position: { my: 'center top', at: 'center top' },
            }, 'error', 3000);
            return;
        }

        $.ajax({
            url: '/TestOder/TestOrderBody/' + testId, // 替換為您的詳細報告 API 端點
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            // data: JSON.stringify({ testId: testId }),
            success: function (response) {
                const gridInstance = gridElement.dxDataGrid('instance');

                // 直接使用回傳的資料，因為格式已經符合預期
                gridInstance.option('dataSource', response);
                gridInstance.option('loadPanel.enabled', false);
            },
            error: function (xhr, status, error) {
                let errorMessage = '詳細報告載入失敗';

                if (xhr.status === 401) {
                    errorMessage = '授權已過期，請重新登入';
                } else if (xhr.status === 403) {
                    errorMessage = '沒有權限存取此資料';
                } else if (xhr.status === 400) {
                    errorMessage = '請求參數錯誤';
                }

                DevExpress.ui.notify({
                    message: errorMessage,
                    position: { my: 'center top', at: 'center top' },
                }, 'error', 3000);
            }
        });
    }

});
