$(() => {
    const dataSource = [{
        id: 0,
        icon: 'description',
        title: 'PLC訊息',
        template: `
            <form action="your-action" id="histroy-query-form-container">
                <div id="histroy-query-form-plc-message"></div>
            </form>
            <div class="history-result-grid-plc-message"></div>
        `
     }, {
        id: 1,
        icon: 'taskhelpneeded',
        title: 'PC訊息', 
        template: `
            <form action="your-action" id="histroy-query-form-container">
                <div id="histroy-query-form-pc-message"></div>
            </form>
            <div class="history-result-grid-pc-message"></div>
        `
     }];

    const tabPanel = $('#tabpanel').dxTabPanel({
        dataSource,
        width: '100%',
        height: '100%',
        animationEnabled: true,
        swipeEnabled: true,
        tabsPosition: 'top',
        stylingMode: 'primary',
        iconPosition: 'start',
        itemTemplate: (item) => item.template,
        onSelectionChanged: function(e) {
            if (e.addedItems[0].id === 0) {
                initializePLCForm();
            } else {
                initializePCForm();
            }
        }
    }).dxTabPanel('instance');
    initializePLCForm();

    function initializePLCForm(){
        var historyQueryPLCMessageForm = $('#histroy-query-form-plc-message').dxForm({
            readOnly: false,
            showColonAfterLabel: false,
            validationGroup: 'customerDataA',
            onFieldDataChanged: (e) => {

                if (e.dataField === "StartTime") {
                    const endTimeEditor = historyQueryPLCMessageForm.getEditor('EndTime');
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
                        const startTime = historyQueryPLCMessageForm.getEditor('StartTime').option('value');
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

                        const formInstance = $('#histroy-query-form-plc-message').dxForm('instance');

                        const validateResult = formInstance.validate();
    
                        if (validateResult.isValid) {

                            const formData = formInstance.option('formData');

                            updatePLCHistoryGridData(formData);
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
    
        function updatePLCHistoryGridData(formData) {
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
    
            const equipmentSelectBox = historyQueryPLCMessageForm.getEditor('EquipmentName');
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
                url: '/SystemLog/PLCLog',
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
                    historyGridPLC.option('dataSource', response);
    
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
    
        const historyGridPLC = $('.history-result-grid-plc-message').dxDataGrid({
            dataSource: [],
            height:"90%",
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

                const dataGridInstance = $('.history-result-grid-plc-message').dxDataGrid('instance');

                const dataSource = dataGridInstance.getDataSource();

                const data = dataSource.items();
                if (data.length >= 1) {
                    const equipmentSelectBox = historyQueryPLCMessageForm.getEditor('EquipmentName');
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
                    caption: '類型',
                    dataField: 'level',
                    alignment: 'center'
                },
                {
                    caption: '訊息',
                    dataField: 'message',
                    alignment: 'center'
                },
                {
                    caption: '設備描述',
                    dataField: 'description',
                    alignment: 'center'
                }
            ]
        }).dxDataGrid('instance');
    }

    function initializePCForm(){
        var historyQueryPCMessageForm = $('#histroy-query-form-pc-message').dxForm({
            readOnly: false,
            showColonAfterLabel: false,
            validationGroup: 'customerDataAB',
            onFieldDataChanged: (e) => {
                if (e.dataField === "StartTime") {
                    const endTimeEditor = historyQueryPCMessageForm.getEditor('EndTime');
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
                        const startTime = historyQueryPCMessageForm.getEditor('StartTime').option('value');
                        return !startTime || !e.value || e.value > startTime;
                    }
                }],
            }, {
                itemType: 'button',
                name: 'BatchPUTPC',
                cssClass: 'batch-put-button',
                buttonOptions: {
                    text: '查詢',
                    onClick: () => {

                        const formInstance = $('#histroy-query-form-pc-message').dxForm('instance');

                        const validateResult = formInstance.validate();
    
                        if (validateResult.isValid) {

                            const formData = formInstance.option('formData');

                            updatePCHistoryGridData(formData);
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
    
        function updatePCHistoryGridData(formData) {
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

            const loadPanel = $('.xn-load-panel').dxLoadPanel({
                shadingColor: 'rgba(0,0,0,0.4)',
                message: '載入中...',
                showIndicator: true,
                showPane: true,
                visible: true,
                position: { of: '.history-result-content' }
            }).dxLoadPanel('instance');
    
            const requestData = {
                EquipmentName: "",
                StartTime: moment(formData.StartTime).format('YYYY-MM-DD HH:mm:ss'),
                EndTime: moment(formData.EndTime).format('YYYY-MM-DD HH:mm:ss')
            };
    
            $.ajax({
                url: '/SystemLog/PCLog',
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
                    historyGridPC.option('dataSource', response);
    
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
    
        const historyGridPC = $('.history-result-grid-pc-message').dxDataGrid({
            dataSource: [],
            height:"90%",
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

                const dataGridInstance = $('.history-result-grid-pc-message').dxDataGrid('instance');

                const dataSource = dataGridInstance.getDataSource();

                const data = dataSource.items();
                if (data.length >= 1) {
                    const workbook = new ExcelJS.Workbook();
                    const worksheet = workbook.addWorksheet("系統資訊");
    
                    DevExpress.excelExporter.exportDataGrid({
                        component: e.component,
                        worksheet,
                        autoFilterEnabled: true,
                    }).then(() => {
                        workbook.xlsx.writeBuffer().then((buffer) => {
                            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), "系統資訊" + '.xlsx');
                        });
                    });
                }
 
            },
            onContentReady(e) {
                e.component.option('loadPanel.enabled', false);
            },
            columns: [
                {
                    caption: '時間',
                    dataField: 'dateTime',
                    alignment: 'center'
                },
                {
                    caption: '類型',
                    dataField: 'level',
                    alignment: 'center'
                },
                {
                    caption: '訊息',
                    dataField: 'message',
                    alignment: 'center'
                }
            ]
        }).dxDataGrid('instance');
    }
});