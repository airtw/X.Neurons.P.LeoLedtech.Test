$(() => {


    var historyQueryForm = $('#histroy-query-form').dxForm({
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
            dataField: 'EquipmentName',
            label: {
                text: "生產監控站",
                location: 'top'
            },
            editorType: 'dxSelectBox',
            editorOptions: {
                dataSource: DevExpress.data.AspNet.createStore({
                    key: 'id',
                    loadUrl: '/EquipmentInfo',
                    onBeforeSend: function (operation, ajaxSettings) {
                        const token = localStorage.getItem('token');
                        ajaxSettings.headers = {
                            'Authorization': `Bearer ${token}`
                        };
                    }
                }),
                displayExpr: 'name',
                valueExpr: 'id',
                searchEnabled: true,
                onLoadError: function (error) {
                    DevExpress.ui.notify({
                        message: '載入設備列表失敗',
                        position: {
                            my: 'center top',
                            at: 'center top',
                        },
                    }, 'error', 3000);
                }
            },
            validationRules: [{
                type: 'required',
                message: '生產監控站為必填',
            }],
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

        const equipmentSelectBox = historyQueryForm.getEditor('EquipmentName');
        const selectedItem = equipmentSelectBox.option('selectedItem');
        const equipmentName = selectedItem ? selectedItem.name : '';

        const loadPanel = $('.xn-load-panel').dxLoadPanel({
            shadingColor: 'rgba(0,0,0,0.4)',
            message: '載入中...',
            showIndicator: true,
            showPane: true,
            visible: true,
            position: { of: '.history-result-content' }
        }).dxLoadPanel('instance');

        const requestData = {
            EquipmentName: equipmentName,
            StartTime: moment(formData.StartTime).format('YYYY-MM-DD HH:mm:ss'),
            EndTime: moment(formData.EndTime).format('YYYY-MM-DD HH:mm:ss')
        };

        $.ajax({
            url: '/History',
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

    function formatPercentage(value) {
        if (value == null || isNaN(value)) {
            return "-";
        }
        return value + " %"; 
    }


    function formatProductionSpeed(value) {
        if (value == null || isNaN(value)) {
            return "-"; // 處理空值或非數字
        }
        if (value < 60) {
            // 小於 1 分鐘，顯示秒
            return value + " Sec / PCS";;
        } else if (value < 3600) {
            // 小於 1 小時，顯示分鐘
            const minutes = (value / 60).toFixed(1); // 保留 1 位小數
            return minutes + " Min / PCS";
        } else {
            // 大於等於 1 小時，顯示小時
            const hours = (value / 3600).toFixed(1); // 保留 1 位小數
            return hours + " Hour / PCS";
        }
    }

    const historyGrid = $('.history-result-grid').dxDataGrid({
        dataSource: [],
        showBorders: true,
        columnAutoWidth: true,
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
        columns: [
            {
                caption: '設備名稱',
                dataField: 'name',
                alignment: 'center'
            },
            {
                caption: '時間',
                dataField: 'dateTime',
                alignment: 'center'
            },
            {
                caption: '設備狀態',
                dataField: 'equipmentStatus',
                alignment: 'center',
                cellTemplate: function (container, options) {
                    const statusMap = {
                        0: { text: "離線", color: "gray" },
                        1: { text: "運行", color: "green" },
                        2: { text: "異常", color: "red" },
                        3: { text: "待機", color: "yellow" }
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
                }
            },
            {
                caption: '生產計數',
                dataField: 'productionInfo.productionCount',
                alignment: 'center'
            },
            {
                caption: '生產速度',
                dataField: 'productionInfo.productionRate',
                alignment: 'center',
                format: {
                    type: 'fixedPoint',
                    precision: 2
                },
                cellTemplate: function (container, options) {
                    const percentage = formatProductionSpeed(options.value);
                    container.text(percentage);
                }
            },
            {
                caption: '稼動率',
                dataField: 'utilizationInfo.equipmentUtilizationRate',
                alignment: 'center',
                cellTemplate: function (container, options) {
                    const percentage = formatPercentage(options.value);
                    container.text(percentage);
                }
            },
            {
                caption: '異常率',
                dataField: 'utilizationInfo.failureRate',
                alignment: 'center',
                cellTemplate: function (container, options) {
                    const percentage = formatPercentage(options.value);
                    container.text(percentage);
                }
            },
            {
                caption: '待機率',
                dataField: 'utilizationInfo.standbyRate',
                alignment: 'center',
                cellTemplate: function (container, options) {
                    const percentage = formatPercentage(options.value);
                    container.text(percentage);
                }
            },
            {
                caption: '設備描述',
                dataField: 'description',
                alignment: 'center'
            }
        ]
    }).dxDataGrid('instance');
});
