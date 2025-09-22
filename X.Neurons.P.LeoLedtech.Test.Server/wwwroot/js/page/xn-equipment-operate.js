$(() => {

    function showProductionCountDialog(equipmentName) {
        let numberBox;

        const popup = $('.xn-show-production-count-dialog').dxPopup({
            title: '設定生產計數',
            width: "auto",
            height: "auto",
            showCloseButton: true,
            dragEnabled: true,
            resizeEnabled: false,
            showTitle: true,
            visible: false,
            contentTemplate: function (contentElement) {
                // 創建內容結構
                const container = $('<div>').css('padding', '20px');

                // 設備名稱
                const equipmentInfo = $('<div>').css({
                    'margin-bottom': '15px',
                    'font-size': '14px',
                    'font-weight': 'bold'
                }).text(`設備：${equipmentName}`);

                // 說明文字
                const description = $('<div>').css({
                    'margin-bottom': '15px',
                    'font-size': '14px'
                }).text('請輸入要設定的生產計數：');

                // NumberBox 容器
                const numberBoxContainer = $('<div>').css('margin-bottom', '20px');

                // 按鈕容器
                const buttonContainer = $('<div>').css('text-align', 'right');
                const confirmBtnContainer = $('<span>').css('margin-right', '10px');
                const cancelBtnContainer = $('<span>');

                // 組裝結構
                container.append(equipmentInfo)
                    .append(description)
                    .append(numberBoxContainer)
                    .append(buttonContainer.append(confirmBtnContainer).append(cancelBtnContainer));

                contentElement.append(container);

                // 初始化 NumberBox
                numberBox = numberBoxContainer.dxNumberBox({
                    value: 0,
                    min: 0,
                    format: '#',
                    showSpinButtons: true,
                    width: '100%',
                    placeholder: '請輸入整數...'
                }).dxNumberBox('instance');

                // 初始化確定按鈕
                confirmBtnContainer.dxButton({
                    text: '確定',
                    type: 'default',
                    onClick: function () {
                        const inputValue = numberBox.option('value');

                        // 驗證輸入值
                        if (inputValue === null || inputValue === undefined || !Number.isInteger(inputValue) || inputValue < 0) {
                            DevExpress.ui.notify({
                                message: '請輸入有效的正整數',
                                position: {
                                    my: 'center top',
                                    at: 'center top',
                                },
                            }, 'warning', 3000);
                            return;
                        }

                        // 關閉 popup
                        popup.hide();

                        // 顯示確認對話框
                        DevExpress.ui.dialog.confirm(
                            `確定要將 ${equipmentName} 的生產計數設定為 ${inputValue} 嗎？`,
                            "確認設定"
                        ).then((dialogResult) => {
                            if (dialogResult) {
                                resetProductionCount(equipmentName, inputValue);
                            }
                        });
                    }
                });

                // 初始化取消按鈕
                cancelBtnContainer.dxButton({
                    text: '取消',
                    onClick: function () {
                        popup.hide();
                    }
                });
            },
            onShown: function () {
                // popup 顯示後讓 NumberBox 獲得焦點
                setTimeout(() => {
                    if (numberBox) {
                        const input = numberBox.$element().find('input');
                        input.focus().select();
                    }
                }, 100);
            }
        }).dxPopup('instance');

        // 顯示 popup
        popup.show();
    }

    const operateGrid = $('.xn-equipment-operate-grid').dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: 'name',
            loadUrl: '/EquipmentOperate',
            onBeforeSend: function(operation, ajaxSettings) {
                const token = localStorage.getItem('token');
                ajaxSettings.headers = {
                    'Authorization': `Bearer ${token}`
                };
            }
        }),
        showBorders: true,
        columnAutoWidth: true,
        columns: [
            {
                caption: '設備名稱',
                dataField: 'name',
                alignment: 'center'
            },
            {
                caption: '設備描述',
                dataField: 'description',
                alignment: 'center'
            },
            {
                caption: '最後操作時間',
                dataField: 'lastOperateTime',
                alignment: 'center'
            },
            {
                caption: '生產計數設定',
                alignment: 'center',
                width: 120,
                cellTemplate: function (container, options) {
                    $('<div>').dxButton({
                        icon: 'edit',
                        text: '設定',
                        type: 'default',
                        onClick: function () {
                            showProductionCountDialog(options.row.data.name);
                        }
                    }).appendTo(container);
                }
            }
        ],
        loadPanel: {
            enabled: true,
            text: '載入中...'
        }
    }).dxDataGrid('instance');

    function resetProductionCount(equipmentName, countValue) {
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
            message: '處理中...',
            showIndicator: true,
            showPane: true,
            visible: true,
            position: { of: '.xn-equipment-operate-grid' }
        }).dxLoadPanel('instance');

        $.ajax({
            url: '/EquipmentOperate',
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            data: JSON.stringify({
                name: equipmentName,
                countValue: countValue  // 新增的數值參數
            }),
            success: function (response) {
                DevExpress.ui.notify({
                    message: `設定成功，生產計數已設為 ${countValue}`,
                    position: {
                        my: 'center top',
                        at: 'center top',
                    },
                }, 'success', 3000);

                operateGrid.refresh();
            },
            error: function (xhr, status, error) {
                let errorMessage = '操作失敗';

                if (xhr.status === 401) {
                    errorMessage = '授權已過期，請重新登入';
                } else if (xhr.status === 403) {
                    errorMessage = '沒有權限執行此操作';
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
            },
            complete: function () {
                loadPanel.hide();
            }
        });
    }
});