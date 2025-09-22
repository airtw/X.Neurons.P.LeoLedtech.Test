$(() => {
    // 全域變數
    let currentTestStepHead = null;
    let channelData = [];
    let testStepBodyData = [];

    // 初始化 TestStepHead 主要 DataGrid
    const testStepHeadGrid = $('.teststep-head-grid').dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: 'id',
            loadUrl: '/TestStep/TestStepHead',
            insertUrl: '/TestStep/TestStepHead',
            updateUrl: '/TestStep/TestStepHead',
            deleteUrl: '/TestStep/TestStepHead',
            onBeforeSend: function (operation, ajaxSettings) {
                const token = localStorage.getItem('token');
                if (token) {
                    ajaxSettings.headers = {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    };
                }
            }
        }),

        showBorders: true,
        showRowLines: true,
        showColumnLines: true,
        rowAlternationEnabled: true,
        columnAutoWidth: true,

        paging: {
            pageSize: 20
        },

        filterRow: {
            visible: true
        },
        editing: {
            mode: 'popup',
            allowAdding: true,
            allowUpdating: true,
            allowDeleting: true,
            useIcons: true,
            popup: {
                title: '測試步驟設定',
                showTitle: true,
                width: 400,
                height: 350
            },
            form: {
                items: [
                    {
                        dataField: 'name',
                        label: { text: '測試名稱' },
                        validationRules: [{ type: 'required', message: '測試名稱為必填' }]
                    },
                    {
                        dataField: 'isPCB',
                        label: { text: '是否為PCB板測試' },
                        editorType: 'dxCheckBox'
                    },
                    {
                        dataField: 'description',
                        label: { text: '描述' },
                        editorType: 'dxTextArea',
                        editorOptions: { height: 80 }
                    }
                ]
            }
        },

        columns: [
            {
                dataField: 'id',
                caption: 'ID',
                visible: false
            },
            {
                dataField: 'name',
                caption: '測試名稱',
                alignment: 'center'
            },
            {
                dataField: 'isPCB',
                caption: '是否為PCB板測試',
                alignment: 'center',
                lookup: {
                    dataSource: [
                        { id: true, name: '是' },
                        { id: false, name: '否' }
                    ],
                    valueExpr: 'id',
                    displayExpr: 'name'
                }
            },
            {
                dataField: 'description',
                caption: '描述',
                alignment: 'left'
            },
            {
                type: 'buttons',
                width: 150,
                buttons: [
                    {
                        name: 'edit',
                        text: '編輯設定',
                        icon: 'edit',
                        onClick: function (e) {
                            openTestStepEditor(e.row.data);
                        }
                    },
                    'delete'
                ]
            }
        ],

        onCellPrepared: function (e) {
            if (e.rowType === 'data') {
                e.cellElement.css('vertical-align', 'middle');
            }
        }
    });

    // 打開測試步驟編輯器
    function openTestStepEditor(testStepHeadData) {
        currentTestStepHead = testStepHeadData;

        // 載入相關資料
        loadChannelData(testStepHeadData.id);
        loadTestStepBodyData(testStepHeadData.id);

        // 顯示編輯器 popup
        $('.teststep-editor-popup').dxPopup({
            title: `編輯測試步驟: ${testStepHeadData.name}`,
            width: '95%',
            height: '90%',
            visible: true,
            showCloseButton: true,
            contentTemplate: createTestStepEditorContent,
            onHidden: function () {
                currentTestStepHead = null;
                channelData = [];
                testStepBodyData = [];
            }
        });
    }

    // 創建測試步驟編輯器內容
    function createTestStepEditorContent(contentElement) {
        const $container = $('<div>').css({
            'display': 'flex',
            'flex-direction': 'column',
            'height': '100%',
            'gap': '20px'
        });

        // 建立 Channel 管理區域
        const $channelSection = createChannelSection();

        // 建立 TestStepBody 管理區域  
        const $bodySection = createTestStepBodySection();

        $container.append($channelSection).append($bodySection);
        $(contentElement).append($container);

        return contentElement;
    }

    // 創建 Channel 管理區域
    function createChannelSection() {
        const $section = $('<div>').css({
            'border': '1px solid #ddd',
            'border-radius': '4px',
            'padding': '15px',
            'flex': '0 0 300px'
        });

        $section.append($('<h5>').text('通道設定').css('margin-bottom', '15px'));

        const $channelGrid = $('<div>').addClass('channel-grid').dxDataGrid({
            dataSource: channelData,
            showBorders: true,
            showRowLines: true,
            showColumnLines: true,
            height: 250,

            editing: {
                mode: 'cell',
                allowAdding: true,
                allowDeleting: true,
                allowUpdating: true
            },

            columns: [
                {
                    dataField: 'channel',
                    caption: '通道號',
                    alignment: 'center',
                    dataType: 'number',
                    validationRules: [{ type: 'required' }]
                },
                {
                    dataField: 'name',
                    caption: '通道名稱',
                    alignment: 'center',
                    validationRules: [{ type: 'required' }]
                },
                {
                    dataField: 'cableColor',
                    caption: '線色',
                    alignment: 'center',
                    lookup: {
                        dataSource: [
                            { id: 'black', name: '黑色' },
                            { id: 'black_white', name: '黑白色' },
                            { id: 'blue', name: '藍色' },
                            { id: 'brown', name: '棕色' },
                            { id: 'green', name: '綠熱' },
                            { id: 'green_yellow', name: '綠黃色' }
                        ],
                        valueExpr: 'id',
                        displayExpr: 'name'
                    }
                },
                {
                    dataField: 'description',
                    caption: '描述',
                    alignment: 'left'
                }
            ],

            onRowInserted: function (e) {
                e.data.testStepHeadId = currentTestStepHead.id;
                saveChannelData(e.data, 'insert');
                checkTestStepBodyAvailability();
            },

            onRowUpdated: function (e) {
                saveChannelData(e.data, 'update');
            },

            onRowRemoved: function (e) {
                saveChannelData(e.data, 'delete');
                checkTestStepBodyAvailability();
            },

            onCellPrepared: function (e) {
                if (e.rowType === 'data') {
                    e.cellElement.css('vertical-align', 'middle');
                }
            }
        });

        $section.append($channelGrid);
        return $section;
    }

    // 創建 TestStepBody 管理區域
    function createTestStepBodySection() {
        const $section = $('<div>').css({
            'border': '1px solid #ddd',
            'border-radius': '4px',
            'padding': '15px',
            'flex': '1'
        });

        $section.append($('<h5>').text('測試步驟設定').css('margin-bottom', '15px'));

        const $bodyGrid = $('<div>').addClass('teststep-body-grid').dxDataGrid({
            dataSource: testStepBodyData,
            showBorders: true,
            showRowLines: true,
            showColumnLines: true,

            // 啟用行拖拽功能
            rowDragging: {
                allowReordering: true,
                onReorder: function (e) {
                    const { fromIndex, toIndex } = e;

                    console.log(`移動從位置 ${fromIndex} 到位置 ${toIndex}`);

                    // 手動重新排序 testStepBodyData 陣列
                    const movedItem = testStepBodyData[fromIndex];

                    // 創建新的排序陣列
                    const newOrder = [...testStepBodyData];
                    newOrder.splice(fromIndex, 1);  // 移除原位置的項目
                    newOrder.splice(toIndex, 0, movedItem);  // 插入到新位置

                    // 更新全域陣列
                    testStepBodyData.length = 0;
                    testStepBodyData.push(...newOrder);

                    // 根據新順序更新 step
                    testStepBodyData.forEach((item, index) => {
                        const newStep = index + 1;
                        console.log(`更新 id=${item.id} 從 step=${item.step} 到 step=${newStep}`);
                        // item.step = newStep;
                        updateTestStepBodyStep(item.id, newStep);
                    });

                    // 更新 DataGrid 的資料來源
                    e.component.option('dataSource', testStepBodyData);
                    e.component.refresh();
                }
            },

            // Master-Detail 設定
            masterDetail: {
                enabled: true,
                template: function (container, options) {
                    createTestStepBodyLimitDetail(container, options);
                }
            },

            editing: {
                mode: 'popup',
                allowAdding: true,
                allowUpdating: true,
                allowDeleting: true,
                useIcons: true,
                popup: {
                    title: '測試步驟詳細設定',
                    width: 500,
                    height: 550
                },
                form: {
                    colCount: 1, // 改為一行一行顯示
                    items: [
                        {
                            dataField: 'name',
                            label: { text: '測試名稱' },
                            validationRules: [{ type: 'required' }]
                        },
                        {
                            dataField: 'step',
                            label: { text: '步驟' },
                            dataType: 'number',
                            editorOptions: { readOnly: true }
                        },
                        {
                            dataField: 'voltage',
                            label: { text: '輸出電壓(V)' },
                            dataType: 'number',
                            validationRules: [{ type: 'required' }]
                        },
                        {
                            dataField: 'current',
                            label: { text: '輸出電流(A)' },
                            dataType: 'number',
                            validationRules: [{ type: 'required' }]
                        },
                        {
                            dataField: 'collectionDelayTime',
                            label: { text: '採集延遲時間(ms)' },
                            dataType: 'number',
                            editorOptions: {
                                min: 100
                            },
                            validationRules: [{
                                type: 'range',
                                min: 100,
                                message: '採集延遲時間最少為 100ms'
                            }]
                        },
                        {
                            dataField: 'dleayTime',
                            label: { text: '延遲時間(ms)' },
                            dataType: 'number',
                            editorOptions: {
                                min: 100
                            },
                            validationRules: [{
                                type: 'range',
                                min: 100,
                                message: '延遲時間最少為 100ms'
                            }]
                        },
                        {
                            dataField: 'description',
                            label: { text: '描述' },
                            editorType: 'dxTextArea',
                            editorOptions: { height: 80 }
                        }
                    ]
                }
            },
            columns: [
                {
                    dataField: 'step',
                    caption: '步驟',
                    alignment: 'center',
                    width: 60,
                    allowEditing: false
                },
                {
                    dataField: 'name',
                    caption: '測試名稱',
                    alignment: 'center'
                },
                {
                    dataField: 'voltage',
                    caption: '輸出電壓(V)',
                    alignment: 'center',
                    dataType: 'number',
                    format: '#0.##'
                },
                {
                    dataField: 'current',
                    caption: '輸出電流(A)',
                    alignment: 'center',
                    dataType: 'number',
                    format: '#0.##'
                },
                {
                    dataField: 'collectionDelayTime',
                    caption: '採集延遲(ms)',
                    alignment: 'center',
                    dataType: 'number'
                },
                {
                    dataField: 'dleayTime',
                    caption: '延遲時間(ms)',
                    alignment: 'center',
                    dataType: 'number'
                },
                {
                    dataField: 'description',
                    caption: '描述',
                    alignment: 'left'
                }
            ],

            onRowInserted: function (e) {
                e.data.testStepHeadId = currentTestStepHead.id;
                e.data.step = testStepBodyData.length;
                saveTestStepBodyData(e.data, 'insert');
            },

            onRowUpdated: function (e) {
                saveTestStepBodyData(e.data, 'update');
            },

            onRowRemoved: function (e) {
                saveTestStepBodyData(e.data, 'delete');
            },

            onCellPrepared: function (e) {
                if (e.rowType === 'data') {
                    e.cellElement.css('vertical-align', 'middle');
                }
            }
        });

        $section.append($bodyGrid);
        return $section;
    }

    // 創建 TestStepBodyLimit 詳細區域
    function createTestStepBodyLimitDetail(container, options) {
        const testStepBodyId = options.data.id;

        const $detailGrid = $('<div>').dxDataGrid({
            dataSource: new DevExpress.data.CustomStore({
                key: 'id',

                load: function () {
                    return $.ajax({
                        url: `/TestStep/TestStepBodyLimit/${testStepBodyId}`,
                        method: 'GET',
                        headers: {
                            'Authorization': `Bearer ${localStorage.getItem('token')}`
                        }
                    });
                },

                insert: function (values) {
                    // 加入 testStepBodyId
                    values.testStepBodyId = testStepBodyId;

                    return $.ajax({
                        url: '/TestStep/TestStepBodyLimit',
                        method: 'POST',
                        headers: {
                            'Authorization': `Bearer ${localStorage.getItem('token')}`,
                            'Content-Type': 'application/json'
                        },
                        data: JSON.stringify(values)
                    });
                },

                update: function (key, values) {
                    return $.ajax({
                        url: '/TestStep/TestStepBodyLimit',
                        method: 'PUT',
                        headers: {
                            'Authorization': `Bearer ${localStorage.getItem('token')}`,
                            'Content-Type': 'application/json'
                        },
                        data: JSON.stringify({
                            id: key,
                            ...values
                        })
                    });
                },

                remove: function (key) {
                    return $.ajax({
                        url: `/TestStep/TestStepBodyLimit/${key}`,
                        method: 'DELETE',
                        headers: {
                            'Authorization': `Bearer ${localStorage.getItem('token')}`
                        }
                    });
                }
            }),


            showBorders: true,
            showRowLines: true,
            showColumnLines: true,
            height: 200,

            editing: {
                mode: 'cell',
                allowAdding: true,
                allowUpdating: true,
                allowDeleting: true
            },


            columns: [
                {
                    dataField: 'channel',
                    caption: '通道',
                    alignment: 'center',
                    dataType: 'number',
                    lookup: {
                        dataSource: channelData,
                        valueExpr: 'channel',
                        displayExpr: 'name'
                    },
                    validationRules: [{ type: 'required' }]
                },
                {
                    dataField: 'hh',
                    caption: '高高限',
                    alignment: 'center',
                    dataType: 'number',
                    format: '#0.####',
                    validationRules: [{
                        type: 'custom',
                        validationCallback: function (options) {
                            return validateLimit(options.data, 'hh', options.value);
                        },
                        message: '高高限設定錯誤'
                    }]
                },
                {
                    dataField: 'h',
                    caption: '高限',
                    alignment: 'center',
                    dataType: 'number',
                    format: '#0.####',
                    validationRules: [{
                        type: 'custom',
                        validationCallback: function (options) {
                            return validateLimit(options.data, 'h', options.value);
                        },
                        message: '高限設定錯誤'
                    }]
                },
                {
                    dataField: 'l',
                    caption: '低限',
                    alignment: 'center',
                    dataType: 'number',
                    format: '#0.####',
                    validationRules: [{
                        type: 'custom',
                        validationCallback: function (options) {
                            return validateLimit(options.data, 'l', options.value);
                        },
                        message: '低限設定錯誤'
                    }]
                },
                {
                    dataField: 'll',
                    caption: '低低限',
                    alignment: 'center',
                    dataType: 'number',
                    format: '#0.####',
                    validationRules: [{
                        type: 'custom',
                        validationCallback: function (options) {
                            return validateLimit(options.data, 'll', options.value);
                        },
                        message: '低低限設定錯誤'
                    }]
                }
            ],

            onRowInserted: function (e) {
                e.data.testStepBodyId = testStepBodyId;
            },

            onCellPrepared: function (e) {
                if (e.rowType === 'data') {
                    e.cellElement.css('vertical-align', 'middle');
                }
            }
        });

        $(container).append($detailGrid);
    }

    // 驗證函數
    function validateLimit(data, field, value) {
        // 如果值為 0、null 或 undefined，則跳過驗證
        if (value === 0 || value === null || value === undefined) {
            return true;
        }

        // 獲取所有限制值
        const limits = {
            hh: field === 'hh' ? value : (data?.hh || 0),
            h: field === 'h' ? value : (data?.h || 0),
            l: field === 'l' ? value : (data?.l || 0),
            ll: field === 'll' ? value : (data?.ll || 0)
        };

        // 驗證邏輯：hh > h > l > ll
        switch (field) {
            case 'hh':
                return (limits.h === 0 || value > limits.h) &&
                    (limits.l === 0 || value > limits.l) &&
                    (limits.ll === 0 || value > limits.ll);
            case 'h':
                return (limits.hh === 0 || value < limits.hh) &&
                    (limits.l === 0 || value > limits.l) &&
                    (limits.ll === 0 || value > limits.ll);
            case 'l':
                return (limits.hh === 0 || value < limits.hh) &&
                    (limits.h === 0 || value < limits.h) &&
                    (limits.ll === 0 || value > limits.ll);
            case 'll':
                return (limits.hh === 0 || value < limits.hh) &&
                    (limits.h === 0 || value < limits.h) &&
                    (limits.l === 0 || value < limits.l);
            default:
                return true;
        }
    }

    // 載入 Channel 資料
    function loadChannelData(testStepHeadId) {
        $.ajax({
            url: `/TestStep/TestStepBodyChannel/${testStepHeadId}`,
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).done(function (data) {
            channelData = data || [];

            // 更新 Channel DataGrid
            const channelGrid = $('.channel-grid').dxDataGrid('instance');
            if (channelGrid) {
                channelGrid.option('dataSource', channelData);
                channelGrid.refresh();
            }

            checkTestStepBodyAvailability();
        });
    }

    // 載入 TestStepBody 資料
    function loadTestStepBodyData(testStepHeadId) {
        $.ajax({
            url: `/TestStep/TestStepBody/${testStepHeadId}`,
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        }).done(function (data) {
            testStepBodyData = data || [];
            const stepBodyGrid = $('.teststep-body-grid').dxDataGrid('instance');
            if (stepBodyGrid) {
                stepBodyGrid.option('dataSource', testStepBodyData);
                stepBodyGrid.refresh();
            }
        });
    }

    // 檢查 TestStepBody 可用性
    function checkTestStepBodyAvailability() {
        const canAddTestStepBody = channelData.length > 1;

        // 啟用/禁用 TestStepBody grid 的新增功能
        const bodyGrid = $('.teststep-editor-popup .dx-datagrid').last().dxDataGrid('instance');
        if (bodyGrid) {
            bodyGrid.option('editing.allowAdding', canAddTestStepBody);
        }

        // 顯示提示訊息
        if (!canAddTestStepBody && channelData.length <= 1) {
            DevExpress.ui.notify({
                message: '需要至少 2 個通道才能開始新增測試步驟',
                position: { my: 'center top', at: 'center top' },
            }, 'info', 3000);
        }
    }

    // 儲存 Channel 資料
    function saveChannelData(data, operation) {
        let url = '/TestStep/TestStepBodyChannel';
        let method = 'POST';

        if (operation === 'update') {
            method = 'PUT';
        } else if (operation === 'delete') {
            method = 'DELETE';
            url += `/${data.id}`;
        }

        $.ajax({
            url: url,
            method: method,
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`,
                'Content-Type': 'application/json'
            },
            data: operation === 'delete' ? undefined : JSON.stringify(data),
            success: function (response) {
                // 成功後重新載入 Channel 資料並刷新測試步驟設定
                loadChannelData(currentTestStepHead.id);

                // 重新渲染測試步驟設定區域
                refreshTestStepBodySection();
            },
            error: function (xhr, status, error) {
                console.error('Channel 資料操作失敗:', error);
                DevExpress.ui.notify({
                    message: 'Channel 資料操作失敗',
                    position: { my: 'center top', at: 'center top' },
                }, 'error', 3000);
            }
        });
    }

    // 新增函數：重新渲染測試步驟設定區域
    function refreshTestStepBodySection() {
        // 重新載入 TestStepBody 資料並刷新 DataGrid
        loadTestStepBodyData(currentTestStepHead.id);
    }
    // 新增函數：刷新所有已展開的詳細 DataGrid
    function refreshAllDetailGrids() {
        // 找到所有已展開的 master-detail 行
        const bodyGrid = $('.teststep-body-grid').dxDataGrid('instance');
        if (bodyGrid) {
            const visibleRows = bodyGrid.getVisibleRows();
            visibleRows.forEach(row => {
                if (row.isExpanded) {
                    // 重新載入該行的 master-detail 內容
                    bodyGrid.collapseRow(row.key);
                    setTimeout(() => {
                        bodyGrid.expandRow(row.key);
                    }, 100);
                }
            });
        }
    }

    // 儲存 TestStepBody 資料
    function saveTestStepBodyData(data, operation) {
        let url = '/TestStep/TestStepBody';
        let method = 'POST';

        if (operation === 'update') {
            method = 'PUT';
        } else if (operation === 'delete') {
            method = 'DELETE';
            url += `/${data.id}`;
        }

        $.ajax({
            url: url,
            method: method,
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`,
                'Content-Type': 'application/json'
            },
            data: operation === 'delete' ? undefined : JSON.stringify(data)
        });
    }

    // 更新 TestStepBody 的 Step
    function updateTestStepBodyStep(id, newStep) {
        $.ajax({
            url: `/TestStep/TestStepBody/${id}/Step`,
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`,
                'Content-Type': 'application/json'
            },
            data: JSON.stringify({ step: newStep })
        });
    }

    // 提供全域存取
    window.testStepHeadGrid = testStepHeadGrid;
});