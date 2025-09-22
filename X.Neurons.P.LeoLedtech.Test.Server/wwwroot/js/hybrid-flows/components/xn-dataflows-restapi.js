function createRestApi(componentName) {
    // #region::創建元素
    var popupContent = initializeConfigPopup(componentName);
    const $requestSection = $("<div>").addClass("xn-restfulapi-request-section");
    const $methodSelector = $("<div>").addClass("xn-restfulapi-method-selector");
    const $urlInput = $("<div>").addClass("xn-restfulapi-request-url");
    const $urlInputWrapper = $("<div>").addClass("xn-restfulapi-request-url-wrapper").append($urlInput);
    $requestSection.append($methodSelector, $urlInputWrapper);
    const $tabPanel = $("<div>").addClass("xn-restfulapi-config-tab-panel");
    popupContent.append($requestSection, $tabPanel);
    // #endregion::創建元素

    // #region::定義設定內容
    // HTTP Method Selector
    $methodSelector.dxSelectBox({
        items: ["GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS"],
        value: "GET",
        width: 120
    });
    // URL Input
    $urlInput.dxTextBox({
        placeholder: t('data_flows_components_restfulapi_url_input_placeholder'),
        value: "",
        width: "100%"
    });

    // TabPanael
    $tabPanel.dxTabPanel({
        items: [{
            title: t('data_flows_components_restfulapi_tab_panel_title_config'),
            template: createRestfulApiConfig
        }, {
            title: t('data_flows_components_restfulapi_tab_panel_title_params'),
            template: createRestfulApiParams
        }, {
            title: t('data_flows_components_restfulapi_tab_panel_title_authorization'),
            template: createRestfulApiAuthorization
        }, {
            title: t('data_flows_components_restfulapi_tab_panel_title_headers'),
            template: createRestfulApiHeaders
        }, {
            title: t('data_flows_components_restfulapi_tab_panel_title_body'),
            template: createRestfulApiBody
        }, {
            title: t('data_flows_components_restfulapi_tab_panel_title_test'),
            template: createRestfulApiTest
        }],
        deferRendering: false,
        height: 600
    });
    // #endregion::定義設定內容

    // #region::創建Config內容
    function createRestfulApiConfig() {
        const $container = $("<div>")
            .addClass("xn-restfulapi-config-container");

        // General Section
        const $generalSection = $("<div>")
            .addClass("xn-restfulapi-config-general-container");

        // General 標題
        $("<h2>")
            .text(t('data_flows_components_restfulapi_config_general_title'))
            .appendTo($generalSection);

        // Request timeout
        const $timeoutSection = $("<div>")
            .addClass("xn-restfulapi-config-request-timeout");

        // 左側：標題和描述
        const $leftContent = $("<div>")
            .addClass("xn-restfulapi-config-request-timeout-header-wrapper");

        // 標題
        $("<div>")
            .addClass("xn-restfulapi-config-request-timeout-title")
            .text(t('data_flows_components_restfulapi_config_general_timeout_title'))
            .appendTo($leftContent);

        // 描述
        $("<div>")
            .addClass("xn-restfulapi-config-request-timeout-descript")
            .text(t('data_flows_components_restfulapi_config_general_timeout_description'))
            .appendTo($leftContent);

        // 右側：輸入框和單位
        const $rightContent = $("<div>")
            .addClass("xn-restfulapi-config-request-timeout-value-wrapper");

        // 數字輸入框
        $("<div>")
            .addClass("xn-restfulapi-config-request-timeout-value")
            .dxNumberBox({
                value: 0,
                min: 0,
                showSpinButtons: true,
                showZero: true,
                stylingMode: "filled",
                format: "#0",
                height: "32px"
            })
            .appendTo($rightContent);

        // 單位標籤
        $("<span>")
            .text("ms")
            .appendTo($rightContent);

        // 組合左右兩側內容
        $timeoutSection.append($leftContent, $rightContent);

        // Certificates Section
        const $certsSection = $("<div>")
            .addClass("xn-restfulapi-config-certificates-container");

        $("<h2>")
            .addClass("xn-restfulapi-config-certificates-title")
            .text(t('data_flows_components_restfulapi_config_certificates_title'))
            .appendTo($certsSection);

        // CA certificates
        const $caSection = createCertificateSection(
            t('data_flows_components_restfulapi_config_certificates_ca_title'),
            t('data_flows_components_restfulapi_config_certificates_ca_description')
        );

        // Client certificates
        const $clientSection = createCertificateSection(
            t('data_flows_components_restfulapi_config_certificates_client_title'),
            t('data_flows_components_restfulapi_config_certificates_client_description'),
            true
        );

        // 組合所有元素
        $generalSection.append($timeoutSection);
        $certsSection.append($caSection, $clientSection);
        $container.append($generalSection, $certsSection);

        function createFileUploadButton(fileType, onFileSelected) {
            const $container = $("<div>")
                .addClass("xn-restfulapi-config-certificates-upload-button")
                .css({
                    display: "flex",
                    alignItems: "center"
                });

            // 創建隱藏的文件輸入
            const $fileInput = $("<input>")
                .attr({
                    type: "file",
                    accept: fileType === "PEM File" ? ".pem" :
                        fileType === "CRT File" ? ".crt" :
                            fileType === "KEY File" ? ".key" :
                                fileType === "PFX File" ? ".pfx" : "",
                    style: "display: none"
                })
                .on("change", function (e) {
                    const file = e.target.files[0];
                    if (file) {
                        // 模擬上傳動作
                        console.log(`Selected file: ${file.name}`);

                        // 假設這是上傳 API
                        mockUploadFile(file).then(() => {
                            // 成功後調用回調
                            onFileSelected && onFileSelected(file.name);
                        });
                    }
                });

            // 創建按鈕
            $("<div>").dxButton({
                text: t('data_flows_components_restfulapi_config_certificate_upload_button_text'),
                onClick: function () {
                    $fileInput.click();
                }
            }).appendTo($container);

            $container.append($fileInput);
            return $container;
        }

        // 模擬上傳文件的函數
        function mockUploadFile(file) {
            return new Promise((resolve) => {
                console.log(`Uploading ${file.name}...`);
                // 模擬上傳延遲
                setTimeout(() => {
                    console.log(`Upload complete: ${file.name}`);
                    resolve();
                }, 1000);
            });
        }
        // 輔助函數：創建證書部分
        function createCertificateSection(title, description, isClient = false) {
            const $section = $("<div>")
                .addClass("xn-restfulapi-config-certificates-file-wrapper");

            // Header with Switch
            const $header = $("<div>")
                .addClass("xn-restfulapi-config-certificates-header-wrapper");

            $("<div>")
                .addClass("xn-restfulapi-config-certificates-title")
                .text(title)
                .appendTo($header);

            $("<div>")
                .addClass("xn-restfulapi-config-certificates-enable-switch")
                .dxSwitch({
                    value: false,
                    switchedOnText: t('data_flows_components_restfulapi_config_certificate_enable'),
                    switchedOffText: t('data_flows_components_restfulapi_config_certificate_disable')
                })
                .appendTo($header);

            $("<div>")
                .addClass("xn-restfulapi-config-certificates-discript")
                .text(description)
                .appendTo($section);

            // File Selection Area
            const fileTypes = isClient ? ["CRT File", "KEY File", "PFX File"] : ["PEM File"];

            fileTypes.forEach(type => {
                var functionName = type.replace(/\s+/g, '-');
                const $row = $("<div>")
                    .addClass("xn-restfulapi-config-certificates-file");

                // 選擇框實例
                const selectBox = $("<div>")
                    .addClass("xn-restfulapi-config-certificates-file-selectBox")
                    .dxSelectBox({
                        items: [],
                        placeholder: type.split(' ')[0] + " " + t('data_flows_components_restfulapi_config_certificate_file_suffix'),
                        noDataText: t('data_flows_components_restfulapi_config_certificate_passphrase_noData'),
                        stylingMode: "filled",
                        height: "32px"
                    })
                    .dxSelectBox('instance');

                $row.append(selectBox.element());

                // 使用新的文件上傳按鈕
                const $uploadButton = createFileUploadButton(type, (fileName) => {
                    // 更新選擇框的項目
                    const currentItems = selectBox.option('items');
                    selectBox.option('items', [...currentItems, fileName]);
                    selectBox.option('value', fileName);
                });

                $row.append($uploadButton);
                $section.append($row);
            });

            // Passphrase (只有 Client certificates 需要)
            if (isClient) {
                const $passphrase = $("<div>")
                    .addClass("xn-restfulapi-config-certificates-passphrase-wrapper");

                $("<div>")
                    .addClass("xn-restfulapi-config-certificates-passphrase-textBox")
                    .dxTextBox({
                        placeholder: t('data_flows_components_restfulapi_config_certificate_passphrase_placeholder'),
                        mode: "password",
                        height: "32px",
                        width: "100%",
                        stylingMode: "filled",
                        buttons: [{
                            name: "password",
                            location: "after",
                            options: {
                                template: function (data, container) {
                                    const textBox = $(container).closest('.dx-textbox').dxTextBox('instance');
                                    const isPassword = textBox.option('mode') === 'password';

                                    return $("<div>").append(
                                        isPassword ?
                                            `<svg width="16" height="16" viewBox="0 0 16 16">
                                            <path d="M8 3.33333C11.4 3.33333 14.3333 5.86667 15.4667 9.33333C14.3333 12.8 11.4 15.3333 8 15.3333C4.6 15.3333 1.66667 12.8 0.533333 9.33333C1.66667 5.86667 4.6 3.33333 8 3.33333ZM8 13.6667C10.2667 13.6667 12.1333 11.8 12.1333 9.33333C12.1333 6.86667 10.2667 5 8 5C5.73333 5 3.86667 6.86667 3.86667 9.33333C3.86667 11.8 5.73333 13.6667 8 13.6667ZM8 11.6667C6.8 11.6667 5.86667 10.7333 5.86667 9.33333C5.86667 7.93333 6.8 7 8 7C9.2 7 10.1333 7.93333 10.1333 9.33333C10.1333 10.7333 9.2 11.6667 8 11.6667Z" fill="currentColor"/>
                                         </svg>` :
                                            `<svg width="16" height="16" viewBox="0 0 16 16">
                                            <path d="M12.5133 12.1467L11.2667 10.9C11.6 10.4267 11.8 9.90667 11.8 9.33333C11.8 7.93333 10.8667 7 9.66667 7C9.09333 7 8.57333 7.2 8.1 7.53333L7.06667 6.5C7.66667 6.13333 8.4 5.93333 9.2 5.93333C11.4667 5.93333 13.3333 7.8 13.3333 10.2667C13.3333 10.9 13.1333 11.5333 12.7667 12.1333L15.4667 14.8333L14.3333 16L1.66667 3.33333L2.8 2.2L4.48667 3.85333C3.42 4.68667 2.53333 5.8 2 7.13333C3.13333 10.6 6.06667 13.1333 9.46667 13.1333C10.2667 13.1333 11.0667 12.9333 11.7667 12.6L12.5133 12.1467ZM9.66667 11.6667C9.09333 11.6667 8.57333 11.4667 8.1 11.1333L10.7 8.53333C11.0333 9 11.2333 9.52 11.2333 10.0933C11.2333 11.4933 10.3 12.4267 9.1 12.4267H9.66667V11.6667Z" fill="currentColor"/>
                                            <path d="M2.8 3.33333L4.48667 4.98667C3.42 5.82 2.53333 6.93333 2 8.26667C3.13333 11.7333 6.06667 14.2667 9.46667 14.2667C10.2667 14.2667 11.0667 14.0667 11.7667 13.7333L12.5133 13.28L15.4667 16L14.3333 14.8333L1.66667 2.2L2.8 3.33333Z" fill="currentColor"/>
                                        </svg>`
                                    );
                                },
                                onClick: function (e) {
                                    const textBox = $(e.element).closest('.dx-textbox').dxTextBox('instance');
                                    const newMode = textBox.option('mode') === 'text' ? 'password' : 'text';
                                    textBox.option('mode', newMode);
                                    // 重新渲染按鈕
                                    const button = $(e.element).dxButton('instance');
                                    button.repaint();
                                }
                            }
                        }]
                    })
                    .appendTo($passphrase);

                $section.append($passphrase);
            }

            $section.prepend($header);
            return $section;
        }
        return $container;
    }
    // #endregion::創建Config內容

    // #region::創建Params內容
    function createRestfulApiParams() {
        const $container = $("<div>")
            .addClass("xn-restfulapi-params-container");

        // 創建單選按鈕組並添加間距樣式
        const $radioGroupContainer = $("<div>")
            .addClass("xn-restfulapi-params-radio-group-container");
        // 創建內容區域，設定為填滿剩餘空間
        const $content = $("<div>")
            .addClass("xn-restfulapi-params-content");
        const radioData = [
            { text: t('data_flows_components_restfulapi_params_radio_group_none'), value: 'none' },
            { text: t('data_flows_components_restfulapi_params_radio_group_custom'), value: 'custom' },
            { text: t('data_flows_components_restfulapi_params_radio_group_dataSource'), value: 'dataSource' }
        ];
        const $radioGroup = $("<div>")
            .addClass("xn-restfulapi-params-radio-group")
            .dxRadioGroup({
                items: radioData,
                valueExpr: 'value',
                value: 'none',
                layout: "horizontal",
                onInitialized: function (e) {
                    // 初始化後的動作
                    updateContent(e.component.option('value'));
                },
                onValueChanged: function (e) {
                    updateContent(e.value);
                }
            });

        $radioGroupContainer.append($radioGroup);

        // 將元素添加到容器中
        $container.append($radioGroupContainer, $content);
        // 根據單選按鈕選擇更新內容的函數
        function updateContent(selectedValue) {

            // 隱藏所有現有內容
            $content.children().hide();

            // 重置content的樣式
            $content.css({
                display: "block",
                position: "relative"
            });

            switch (selectedValue) {
                case "custom":
                    // let $grid = $("#xn-restfulapi-customize-params");
                    let $grid = $content.find('.xn-restfulapi-params-grid');
                    if ($grid.length === 0) {
                        $grid = createRestfulApiParamsGrid();
                        $content.append($grid);
                    } else {
                        $grid.show();
                    }
                    break;
                case "dataSource":
                    let $test2Content = $("#test2-content");
                    if ($test2Content.length === 0) {
                        $test2Content = createTest2Content()
                            .attr("id", "test2-content")
                            .css({
                                height: "100%",
                                width: "100%"
                            });
                        $content.append($test2Content);
                    } else {
                        $test2Content.show();
                    }
                    break;
                case "none":
                default:
                    // 設定content為flex容器以實現垂直置中
                    $content.css({
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        height: "100%"
                    });

                    let $defaultContent = $("#xn-restfulapi-default-params");
                    if ($defaultContent.length === 0) {
                        $defaultContent = $("<p>")
                            .addClass('xn-restfulapi-params-default-text')
                            .text(t('data_flows_components_restfulapi_params_content_none'));
                        $content.append($defaultContent);
                    } else {
                        $defaultContent.show();
                    }
            }
        }
        function createRestfulApiParamsGrid() {
            return $("<div>")
                .addClass('xn-restfulapi-params-grid')
                .dxDataGrid({
                    dataSource: [{
                        enabled: true,
                        slot: '',
                        key: '',
                        value: '',
                        description: ''
                    }],
                    columns: [{
                        dataField: "enabled",
                        caption: "",
                        dataType: "boolean",
                        width: 40,
                        allowSorting: false
                    }, {
                        dataField: "slot",
                        caption: "Slot",
                        allowSorting: false,
                        visible: false
                    }, {
                        dataField: "key",
                        caption: t('data_flows_components_restfulapi_params_customize_dataGrid_caption_key'),
                        allowSorting: false
                    }, {
                        dataField: "value",
                        caption: t('data_flows_components_restfulapi_params_customize_dataGrid_caption_value'),
                        allowSorting: false
                    }, {
                        dataField: "description",
                        caption: t('data_flows_components_restfulapi_params_customize_dataGrid_caption_description'),
                        allowSorting: false
                    }, {
                        type: "buttons",
                        width: 50,
                        buttons: [{
                            icon: "trash",
                            onClick(e) {
                                const grid = e.component;
                                const allData = grid.getDataSource().items();
                                const currentRowIndex = e.row.rowIndex;

                                // 如果這是最後一行且是空的，不允許刪除
                                if (currentRowIndex === allData.length - 1 &&
                                    !e.row.data.key &&
                                    !e.row.data.value &&
                                    !e.row.data.description) {
                                    return;
                                }

                                // 刪除行
                                grid.deleteRow(e.row.rowIndex);

                                // 重新構建 URL
                                const urlInput = $urlInput.dxTextBox('instance');
                                let newUrl = '';

                                // 獲取基本 URL（問號之前的部分）
                                const baseUrl = urlInput.option('value').split('?')[0];
                                newUrl = baseUrl;

                                // 從剩餘的資料中構建參數，只取有 value 的參數
                                const remainingData = allData.filter((item, index) => index !== currentRowIndex);
                                const params = remainingData
                                    .filter(item =>
                                        item.enabled &&
                                        item.key &&
                                        item.value)  // 只有同時有 key 和 value 的才加入
                                    .map(item => `${item.key}=${item.value}`);

                                // 如果有參數，添加到 URL
                                if (params.length > 0) {
                                    newUrl += '?' + params.join('&');
                                }

                                // 更新 URL 輸入框
                                urlInput.option('value', newUrl);
                            }
                        }]
                    }],
                    editing: {
                        mode: "cell",
                        allowUpdating: true,
                        allowDeleting: false,
                        allowAdding: false,
                        confirmDelete: false // 禁用刪除確認框
                    },
                    onFocusedCellChanged: function (e) {
                        // 記住當前焦點的列資訊
                        e.component.focusedCellInfo = {
                            rowIndex: e.rowIndex,
                            columnIndex: e.columnIndex,
                            dataField: e.column.dataField
                        };
                    },
                    onRowUpdated: function (e) {
                        const grid = e.component;
                        const rowIndex = grid.getRowIndexByKey(e.key);
                        const totalRows = grid.getDataSource().items().length;

                        if (rowIndex === -1) {
                            console.error('Row index not found!');
                            return;
                        }

                        // 檢查是否為最後一行且有輸入值
                        if (rowIndex === totalRows - 1 && e.data.key !== null) {
                            // 判斷是否有輸入資料時自動啟用
                            if ((e.data.key || e.data.value) && !e.data.enabled) {
                                // 更新 enabled 狀態
                                grid.cellValue(rowIndex, 'enabled', true);
                            }
                            // 記住當前編輯的單元格位置
                            const focusedInfo = grid.focusedCellInfo;
                            // 插入新行
                            grid.getDataSource().store().insert({
                                enabled: false,
                                key: '',
                                value: '',
                                description: ''
                            }).then(() => {
                                grid.refresh();

                                grid.option('onContentReady', function onContentReadyHandler() {
                                    grid.option('onContentReady', null);

                                    requestAnimationFrame(() => {
                                        grid.option("focusedRowIndex", focusedInfo.rowIndex);
                                        if (focusedInfo.rowIndex !== undefined) {
                                            grid.editCell(focusedInfo.rowIndex, focusedInfo.columnIndex);
                                        }
                                    });
                                });
                            }).catch(error => {
                                console.error("Error adding row:", error);
                            });
                            grid.editCell(focusedInfo.rowIndex, focusedInfo.columnIndex);
                        } else {
                            // URL 更新邏輯
                            if (e.data.key || e.data.value) {
                                const urlInput = $urlInput.dxTextBox('instance');
                                const currentUrl = urlInput.option('value');
                                let newUrl = currentUrl;

                                const paramString = `${e.data.key}=${e.data.value}`;

                                if (e.data.enabled) {
                                    // 檢查 key 是否存在於 URL 中
                                    const keyExists = new RegExp(`[?&]${e.data.key}=`).test(currentUrl);

                                    if (keyExists) {
                                        // 如果 key 已存在，更新其值
                                        const regex = new RegExp(`(${e.data.key}=)[^&]*`);
                                        newUrl = currentUrl.replace(regex, `$1${e.data.value}`);
                                    } else {
                                        // 如果 key 不存在且 value 不為空，則添加新參數
                                        if (e.data.value) {  // 只有當 value 不為空時才添加
                                            if (!currentUrl.includes('?')) {
                                                newUrl = `${currentUrl}?${paramString}`;
                                            } else {
                                                newUrl = currentUrl.endsWith('?') ?
                                                    `${currentUrl}${paramString}` :
                                                    `${currentUrl}&${paramString}`;
                                            }
                                        }
                                    }

                                    urlInput.option('value', newUrl);
                                } else {
                                    // enabled 為 false 時的刪除邏輯保持不變
                                    if (currentUrl.includes(paramString)) {
                                        newUrl = currentUrl.replace(`&${paramString}`, '');
                                        newUrl = newUrl.replace(`?${paramString}&`, '?');
                                        newUrl = newUrl.replace(`?${paramString}`, '');

                                        if (newUrl.endsWith('?')) {
                                            newUrl = newUrl.slice(0, -1);
                                        }

                                        urlInput.option('value', newUrl);
                                    }
                                }
                            }
                        }
                    },
                    rowDragging: {
                        allowReordering: true,
                        dropFeedbackMode: "push",
                        onReorder: function (e) {
                            const visibleRows = e.component.getVisibleRows();
                            const toIndex = visibleRows[e.toIndex].dataIndex;
                            const fromIndex = visibleRows[e.fromIndex].dataIndex;

                            const dataSource = e.component.getDataSource();
                            const data = [...dataSource.items()];

                            // 移動資料
                            const item = data[fromIndex];
                            data.splice(fromIndex, 1);
                            data.splice(toIndex, 0, item);

                            // 更新 DataSource
                            dataSource.store().clear();
                            data.forEach(item => {
                                dataSource.store().insert(item);
                            });

                            // 重建 URL
                            const urlInput = $urlInput.dxTextBox('instance');
                            const baseUrl = urlInput.option('value').split('?')[0];
                            let newUrl = baseUrl;

                            // 只取有 value 的參數
                            const params = data
                                .filter(item =>
                                    item.enabled &&
                                    item.key &&
                                    item.value)  // 只有同時有 key 和 value 的才加入
                                .map(item => `${item.key}=${item.value}`);

                            if (params.length > 0) {
                                newUrl += '?' + params.join('&');
                            }

                            urlInput.option('value', newUrl);

                            dataSource.reload();
                        }
                    },
                    showBorders: true,
                    showRowLines: true,
                    height: "100%"
                });
        }
        return $container;
    }
    // #endregion::創建Params內容

    // #region::創建Authorization內容
    function createRestfulApiAuthorization() {
        const $container = $("<div>")
            .addClass("xn-restfulapi-auth-container");

        // 左側橘色區塊
        const $leftPanel = $("<div>")
            .addClass("xn-restfulapi-auth-panel-left");

        // 右側藍色區塊
        const $rightPanel = $("<div>")
            .addClass("xn-restfulapi-auth-panel-right");

        // 建立說明文字區域
        const $description = $("<div>")
            .addClass("xn-restfulapi-auth-description");

        // 選單資料
        const menuData = [
            {
                value: "NoAuth",
                text: t('data_flows_components_restfulapi_authorization_selectBox_no_auth'),
                description: "",
                content: t('data_flows_components_restfulapi_authorization_no_auth_content')
            },
            {
                value: "BasicAuth",
                text: t('data_flows_components_restfulapi_authorization_selectBox_basic_auth'),
                description: t('data_flows_components_restfulapi_authorization_basic_auth_description'),
                content: ""
            },
            {
                value: "BearerToken",
                text: t('data_flows_components_restfulapi_authorization_selectBox_bearer_token'),
                description: t('data_flows_components_restfulapi_authorization_bearer_token_description'),
                content: ""
            }
        ];

        // 建立下拉選單
        const $select = $("<div>")
            .addClass("xn-restfulapi-auth-panel-left-select")
            .appendTo($leftPanel);
        // 建立右側內容區域
        const $content = $("<div>")
            .addClass("xn-restfulapi-auth-panel-right-content")
            .appendTo($rightPanel);
        // 添加說明文字區域到左側面板
        $description.appendTo($leftPanel);

        $select.dxSelectBox({
            items: menuData,
            displayExpr: "text",
            valueExpr: "value",
            width: "100%",
            value: "NoAuth",
            onInitialized: function (e) {
                // 初始化後的動作
                updateContent(e.component.option('value'));
            },
            onValueChanged: function (e) {
                updateContent(e.value);
            }
        });

        // 更新內容的函數
        function updateContent(value) {
            const selectedItem = menuData.find(item => item.value === value);
            if (selectedItem) {
                $description.text(selectedItem.description);

                // 清空原有內容
                $content.empty();

                if (value === "BasicAuth") {
                    // Basic Auth 的內容 - 維持原本的靠上對齊
                    $content.css({
                        display: "block",
                        alignItems: "initial",
                        justifyContent: "initial"
                    });

                    // 創建 Username 輸入框
                    const $usernameLabel = $("<div>")
                        .addClass("xn-restfulapi-auth-basic-username")
                        // .attr("id", "xn-restfulapi-auth-basic-username")
                        .text(t('data_flows_components_restfulapi_authorization_basic_auth_username'));

                    const $usernameBox = $("<div>")
                        .addClass("xn-restfulapi-auth-basic-username-textbox")
                        // .attr("id", "xn-restfulapi-auth-basic-username-textbox")
                        .dxTextBox({
                            placeholder: t('data_flows_components_restfulapi_authorization_basic_auth_username_placeholder')
                        });

                    // 創建 Password 輸入框 
                    const $passwordLabel = $("<div>")
                        .addClass("xn-restfulapi-auth-basic-password")
                        // .attr("id", "xn-restfulapi-auth-basic-password")
                        .text(t('data_flows_components_restfulapi_authorization_basic_auth_password'));
                    const $passwordBox = $("<div>")
                        .addClass("xn-restfulapi-auth-basic-password-textbox")
                        // .attr("id", "xn-restfulapi-auth-basic-password-textbox")
                        .dxTextBox({
                            placeholder: t('data_flows_components_restfulapi_authorization_basic_auth_password_placeholder')
                        });

                    // 添加所有元素到內容區
                    $content.append($usernameLabel, $usernameBox, $passwordLabel, $passwordBox);
                } else if (value === "BearerToken") {
                    // Bearer Token 的內容 - 維持原本的靠上對齊
                    $content.css({
                        display: "block",
                        alignItems: "initial",
                        justifyContent: "initial"
                    });

                    // 創建 Token 輸入框
                    const $usernameLabel = $("<div>")
                        .addClass("xn-restfulapi-auth-bearertoken-token")
                        // .attr("id", "xn-restfulapi-auth-bearertoken-token")
                        .text(t('data_flows_components_restfulapi_authorization_bearer_token'));
                    const $usernameBox = $("<div>")
                        .addClass("xn-restfulapi-auth-bearertoken-token-textbox")
                        .attr("id", "xn-restfulapi-auth-bearertoken-token-textbox")
                        .dxTextBox({
                            placeholder: t('data_flows_components_restfulapi_authorization_bearer_token_placeholder')
                        });

                    // 添加所有元素到內容區
                    $content.append($usernameLabel, $usernameBox);
                } else {
                    // 純文字內容 - 設置垂直置中
                    $content.css({
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center"
                    }).text(selectedItem.content);
                }
            } else {
                // 初始狀態也使用垂直置中
                $content.css({
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center"
                }).text(t('data_flows_components_restfulapi_authorization_no_auth_content'));
            }
        }

        // 組裝面板
        $leftPanel.appendTo($container);
        $rightPanel.appendTo($container);

        return $container;
    }
    // #endregion::創建Authorization內容

    // #region::創建Headers內容
    function createRestfulApiHeaders() {
        return $("<div id='xn-restfulapi-headers'>").dxDataGrid({
            dataSource: [{
                enabled: true,
                key: '',
                value: '',
                description: ''
            }],
            columns: [{
                dataField: "enabled",
                caption: "",
                dataType: "boolean",
                width: 40,
                allowSorting: false
            }, {
                dataField: "key",
                caption: t('data_flows_components_restfulapi_headers_dataGrid_caption_key'),
                allowSorting: false
            }, {
                dataField: "value",
                caption: t('data_flows_components_restfulapi_headers_dataGrid_caption_value'),
                allowSorting: false
            }, {
                dataField: "description",
                caption: t('data_flows_components_restfulapi_headers_dataGrid_caption_description'),
                allowSorting: false
            }, {
                type: "buttons",
                width: 50,
                buttons: [{
                    icon: "trash",
                    onClick(e) {
                        const grid = e.component;
                        const data = grid.getDataSource().items();
                        const currentRowIndex = e.row.rowIndex;
                        // 如果這是最後一行且是空的，不允許刪除
                        if (currentRowIndex === data.length - 1 && !e.row.data.key && !e.row.data.value && !e.row.data.description) {
                            return;
                        }
                        grid.deleteRow(e.row.rowIndex);
                    }
                }]
            }],
            editing: {
                mode: "cell",
                allowUpdating: true,
                allowDeleting: false,
                allowAdding: false,
                confirmDelete: false // 禁用刪除確認框
            },
            onFocusedCellChanged: function (e) {
                // 記住當前焦點的列資訊
                e.component.focusedCellInfo = {
                    rowIndex: e.rowIndex,
                    columnIndex: e.columnIndex,
                    dataField: e.column.dataField
                };
            },
            onRowUpdated: function (e) {
                const grid = e.component;
                const rowIndex = grid.getRowIndexByKey(e.key);
                const totalRows = grid.getDataSource().items().length;

                if (rowIndex === -1) {
                    console.error('Row index not found!');
                    return;
                }

                // 檢查是否為最後一行且有輸入值
                if (rowIndex === totalRows - 1 && e.data.key !== null) {
                    // 判斷是否有輸入資料時自動啟用
                    if ((e.data.key || e.data.value) && !e.data.enabled) {
                        // 更新 enabled 狀態
                        grid.cellValue(rowIndex, 'enabled', true);
                    }
                    // 記住當前編輯的單元格位置
                    const focusedInfo = grid.focusedCellInfo;
                    // 插入新行
                    grid.getDataSource().store().insert({
                        enabled: false,
                        key: '',
                        value: '',
                        description: ''
                    }).then(() => {
                        grid.refresh();

                        grid.option('onContentReady', function onContentReadyHandler() {
                            grid.option('onContentReady', null);

                            requestAnimationFrame(() => {
                                grid.option("focusedRowIndex", focusedInfo.rowIndex);
                                if (focusedInfo.rowIndex !== undefined) {
                                    grid.editCell(focusedInfo.rowIndex, focusedInfo.columnIndex);
                                }
                            });
                        });
                    }).catch(error => {
                        console.error("Error adding row:", error);
                    });
                    grid.editCell(focusedInfo.rowIndex, focusedInfo.columnIndex);
                }
            },
            rowDragging: {
                allowReordering: true,
                dropFeedbackMode: "push",
                onReorder: function (e) {
                    const visibleRows = e.component.getVisibleRows();
                    const toIndex = visibleRows[e.toIndex].dataIndex;
                    const fromIndex = visibleRows[e.fromIndex].dataIndex;

                    const dataSource = e.component.getDataSource();
                    const data = [...dataSource.items()];

                    // 移動資料
                    const item = data[fromIndex];
                    data.splice(fromIndex, 1);
                    data.splice(toIndex, 0, item);

                    // 更新 DataSource
                    dataSource.store().clear();
                    data.forEach(item => {
                        dataSource.store().insert(item);
                    });


                    dataSource.reload();
                }
            },
            showBorders: true,
            showRowLines: true,
            height: "100%"
        });
    }
    // #endregion::創建Headers內容

    // #region::創建Body內容
    function createRestfulApiBody() {
        let editor = null;  // 移到函數內部

        const $container = $("<div>")
            .addClass("xn-restfulapi-body-container");

        const $radioGroupContainer = $("<div>")
            .addClass("xn-restfulapi-body-container-radioGroup-container");

        // 選單資料
        const menuData = [
            {
                value: "none",
                text: t('data_flows_components_restfulapi_body_format_none'),
            },
            {
                value: "Customize",
                text: t('data_flows_components_restfulapi_body_format_customize'),
            },
            {
                value: "JSON",
                text: t('data_flows_components_restfulapi_body_format_json'),
            },
            {
                value: "XML",
                text: t('data_flows_components_restfulapi_body_format_xml'),
            }
        ];

        const $radioGroup = $("<div>").dxRadioGroup({
            items: menuData,
            displayExpr: "text",
            valueExpr: "value",
            value: "none",
            layout: "horizontal",
            onValueChanged: function (e) {
                updateFormatContent(e.value);  // 改用內部函數
            }
        });

        $radioGroupContainer.append($radioGroup);

        // 創建空內容顯示區
        const $emptyContent = $("<div>")
            .addClass("xn-restfulapi-body-empty-content")
            .text("內容為空");

        // 創建編輯器容器
        const $editorWrapper = $("<div>")
            .addClass("xn-restfulapi-body-editor-wrapper");
        const $editorContainer = $("<textarea>")
            .addClass("xn-restfulapi-body-editor-container");

        $editorWrapper.append($editorContainer);
        $container.append($radioGroupContainer, $emptyContent, $editorWrapper);
        // 初始化編輯器
        function initEditor() {
            if (!editor) {
                const savedTheme = localStorage.getItem('theme') === 'dark' ? 'material-darker' : 'default';
                editor = CodeMirror.fromTextArea($editorContainer[0], {
                    lineNumbers: true,
                    theme: savedTheme,
                    autoCloseBrackets: true,
                    readOnly: false,
                    lineWrapping: true,
                    foldGutter: true,
                    gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
                    matchBrackets: true,
                    autoCloseTags: true,
                    foldOptions: {
                        widget: '...',  // 摺疊時顯示的圖示
                        minFoldSize: 1
                    },
                    extraKeys: {
                        "Ctrl-Q": function (cm) {
                            cm.foldCode(cm.getCursor());
                        }
                    },
                    line: true,
                    styleActiveLine: true
                });
                editor.setSize("100%", "100%");

            }
            // 為 CodeMirror 包裝器添加圓角
            const wrapper = editor.getWrapperElement();
            wrapper.style.borderRadius = '4px';
            wrapper.style.border = '1px solid var(--border-color)';

            // 確保內部元素也有圓角
            const scroller = wrapper.querySelector('.CodeMirror-scroll');
            if (scroller) {
                scroller.style.borderRadius = '4px';
            }
            return editor;
        }


        // 內部更新函數
        function updateFormatContent(selectedValue) {
            if (selectedValue === "none") {
                // 顯示空內容，隱藏編輯器
                $emptyContent.show();
                $editorWrapper.hide();
                return;
            }

            // 隱藏空內容，顯示編輯器
            $emptyContent.hide();
            $editorWrapper.show();

            const editor = initEditor();
            editor.getWrapperElement().style.display = '';

            try {
                switch (selectedValue) {
                    case "Customize":
                        editor.setOption("mode", "text/plain");
                        editor.setValue("");
                        break;
                    case "JSON":
                        const jsonData = {
                            name: "John",
                            age: 30,
                            hobbies: ["reading", "swimming", "cycling"],
                            addresses: [{
                                city: "New York",
                                country: "USA",
                                details: {
                                    street: "Broadway",
                                    zipCode: "10001"
                                }
                            }]
                        };

                        editor.setOption("mode", "application/json");
                        editor.setValue(JSON.stringify(jsonData, null, 2));
                        break;
                    case "XML":
                        const xmlData = `<?xml version="1.0" encoding="UTF-8"?>
        <root>
            <person>
                <name>John</name>
                <age>30</age>
                <hobbies>
                    <hobby>reading</hobby>
                    <hobby>swimming</hobby>
                    <hobby>cycling</hobby>
                </hobbies>
            </person>
        </root>`;

                        editor.setOption("mode", "xml");
                        editor.setValue(xmlData);
                        break;
                }
                editor.refresh();
            } catch (error) {
                console.error('Error updating editor content:', error);
                editor.setOption("mode", "text/plain");
                editor.setValue("發生錯誤，請重試");
            }
        }

        // 初始化顯示
        setTimeout(() => updateFormatContent("none"), 0);

        return $container;
    }
    // #endregion::創建Body內容

    // #region::創建Test內容
    function createRestfulApiTest() {

        let isTest = false;

        const $container = $("<div>").addClass("xn-restfulapi-test-container");

        // Top Control Area
        const $controlArea = $("<div>").addClass("xn-restfulapi-test-control-area");

        // Left controls group
        const $leftControls = $("<div>").addClass("xn-restfulapi-test-left-controls");

        const $testButton = $("<div>")
            .addClass("xn-restfulapi-test-test-button")
            .dxButton({
                icon: "fas fa-play",
                text: t('data_flows_components_restfulapi_test_execute_button'),
                type: "default",
                onClick: function () {
                    executeTest();
                }
            });

        const $formatGroup = $("<div>").addClass("xn-restfulapi-test-test-format-group");

        $("<span>")
            .addClass("xn-restfulapi-test-response-format")
            .text(t('data_flows_components_restfulapi_test_response_format'))
            .appendTo($formatGroup);

        const $formatSelector = $("<div>")
            .addClass("xn-restfulapi-test-test-format-selector")
            .dxSelectBox({
                items: [
                    { text: "Auto", value: "auto" },
                    { text: "JSON", value: "application/json" },
                    { text: "XML", value: "application/xml" },
                    { text: "String", value: "text/plain" },
                    { text: "HTML", value: "text/html" }
                ],
                value: "auto",
                displayExpr: "text",
                valueExpr: "value",
                width: "auto",
                onValueChanged: function (e) {
                    updateEditorMode(e.value);
                }
            });

        $formatGroup.append($formatSelector);
        $leftControls.append($testButton, $formatGroup);

        // Right stats group
        const $statsArea = $("<div>").addClass("xn-restfulapi-test-stats");
        const $popoverContainer = $("<div>").addClass("xn-restfulapi-test-stats-popover-container")
        $statsArea.appendTo($popoverContainer);

        const statsItems = [
            { icon: "fas fa-check-circle", label: t('data_flows_components_restfulapi_test_status_code'), id: "statusCode" },
            { icon: "fas fa-clock", label: t('data_flows_components_restfulapi_test_response_time'), id: "responseTime" },
            { icon: "fas fa-database", label: t('data_flows_components_restfulapi_test_response_size'), id: "responseSize" }
        ];

        statsItems.forEach(item => {
            const $stat = $("<div>")
                // .addClass("xn-restfulapi-test-stats-" + item.id)
                .addClass("xn-restfulapi-test-stats-content");

            $("<i>")
                .addClass(item.icon)
                .appendTo($stat);

            $("<span>")
                .addClass("xn-restfulapi-test-stats-caption")
                .text(item.label + ": ")
                .appendTo($stat);

            $("<span>")
                .attr("id", `xn-restfulapi-test-${item.id}`)
                .addClass("xn-restfulapi-test-stat-value")
                .appendTo($stat);

            $stat.appendTo($statsArea);

            const $tooltip = $("<div>")
                .addClass(`xn-restfulapi-test-stats-tooltip`)
                .addClass(item.id === "responseTime" ? 'xn-restfulapi-test-stats-tooltip-response-time' : 'xn-restfulapi-test-stats-tooltip-default')
                .appendTo($stat);

            // 根據不同類型填充內容
            switch (item.id) {
                case "statusCode":
                    createStatusCodeTooltip($tooltip);
                    break;
                case "responseTime":
                    createResponseTimeTooltip($tooltip);
                    break;
                case "responseSize":
                    createResponseSizeTooltip($tooltip);
                    break;
            }

            // 添加懸浮事件
            $stat.css("position", "relative")
                .hover(
                    function () {
                        if (isTest) {
                            const position = calculateTooltipPosition($stat, $tooltip);
                            $tooltip.css(position).fadeIn(200);
                        }

                    },
                    function () {
                        if (isTest) {
                            $tooltip.fadeOut(200);
                        }

                    }
                );
        });

        $controlArea.append($leftControls, $statsArea);


        // Response Editor Area
        const $editorArea = $("<div>")
            .addClass("xn-restfulapi-test-editor-wrapper");

        const $editorContainer = $("<textarea>")
            .addClass("xn-restfulapi-test-editor")
            .appendTo($editorArea);

        // Initialize CodeMirror
        const editor = CodeMirror.fromTextArea($editorContainer[0], {
            lineNumbers: true,
            theme: localStorage.getItem('theme') === 'dark' ? 'material-darker' : 'default',
            readOnly: true,
            lineWrapping: true,
            foldGutter: true,
            gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
            mode: "application/json",
            viewportMargin: Infinity
        });

        // Loading Indicator
        // Create a loading overlay that covers the entire tab panel
        const $loadingOverlay = $("<div>")
            .addClass("xn-restfulapi-test-loading-overlay");

        // 創建一個容器來包含 loading indicator 和文字
        const $loadingContent = $("<div>")
            .addClass("xn-restfulapi-test-loading-content")
            .appendTo($loadingOverlay);

        // 添加 loading indicator
        const $loadIndicator = $("<div>")
            .appendTo($loadingContent)
            .dxLoadIndicator({
                visible: true
            });

        // 添加描述文字
        const $loadingText = $("<div>")
            .addClass("xn-restfulapi-test-loading-text")
            .text(t('data_flows_components_restfulapi_test_loading_text')) // "執行測試中..."
            .appendTo($loadingContent);

        // Append all elements to container
        $container.append($controlArea, $editorArea);

        // Function to format size
        function formatSize(bytes) {
            if (bytes === 0) return '0 B';
            const k = 1024;
            const sizes = ['B', 'KB', 'MB', 'GB'];
            const i = Math.floor(Math.log(bytes) / Math.log(k));
            return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
        }

        // Function to update status code style
        function updateStatusStyle(code, $statusElement) {
            const $container = $statusElement.closest('div');
            $container.css({
                backgroundColor: code >= 200 && code < 300 ? "var(--success-bg)" :
                    (code >= 400 ? "var(--error-bg)" : "var(--base-bg)")
            });
            $statusElement.css({
                color: code >= 200 && code < 300 ? "var(--success-color)" :
                    (code >= 400 ? "var(--error-color)" : "var(--text-color)")
            });
        }

        // Function to update editor mode based on content type
        function updateEditorMode(mode) {
            if (mode === "auto") {
                // Try to detect format from content
                const content = editor.getValue();
                try {
                    JSON.parse(content);
                    editor.setOption("mode", "application/json");
                    return;
                } catch (e) {
                    if (content.trim().startsWith("<?xml") || content.trim().startsWith("<")) {
                        editor.setOption("mode", "application/xml");
                        return;
                    }
                }
                editor.setOption("mode", "text/plain");
            } else {
                editor.setOption("mode", mode);
            }
            editor.refresh();
        }

        // Function to execute test
        function executeTest() {
            // Lock tab panel
            const tabPanelInstance = $tabPanel.dxTabPanel("instance");
            tabPanelInstance.option("disabled", true);
            $urlInput.dxTextBox("instance").option("disabled", true); // 禁用 URL 輸入框

            // Append loading overlay to tab panel container and show it
            const $tabPanelElement = $tabPanel.closest('.dx-tabpanel');
            $loadingOverlay.appendTo($tabPanelElement).css("display", "flex");

            // Simulate API call
            const startTime = performance.now();
            setTimeout(() => {
                const endTime = performance.now();
                const responseTime = Math.round(endTime - startTime);

                // Simulate response
                const response = {
                    status: "success",
                    data: {
                        id: 1,
                        name: "Test Response",
                        timestamp: new Date().toISOString(),
                        details: {
                            category: "test",
                            priority: "high"
                        }
                    }
                };

                // Update stats
                const $statusElement = $("#xn-restfulapi-test-statusCode");
                $statusElement.text("200 OK");
                updateStatusStyle(200, $statusElement);

                $("#xn-restfulapi-test-responseTime").text(`${responseTime} ms`);

                const responseText = JSON.stringify(response, null, 2);
                $("#xn-restfulapi-test-responseSize").text(formatSize(new Blob([responseText]).size));

                // Update editor
                editor.setValue(responseText);
                updateEditorMode($formatSelector.dxSelectBox("instance").option("value"));

                // 在所有數據更新完成後設置 isTest 為 true
                isTest = true;

                // 更新 stats 和其他 UI
                $statsArea.fadeIn(200);

                // Hide loading and unlock tab panel
                $loadingOverlay.fadeOut(200, function () {
                    $(this).detach(); // Remove from DOM when hidden
                });
                tabPanelInstance.option("disabled", false);
                $urlInput.dxTextBox("instance").option("disabled", false);

            }, 5000); // Simulate 2 second delay
        }
        // 添加一個計算最佳位置的輔助函數
        function calculateTooltipPosition($stat, $tooltip) {
            const SAFETY_MARGIN = 15; // 與邊界保持的安全距離

            const tabPanel = $('.xn-restfulapi-config-tab-panel');
            const tabPanelOffset = tabPanel.offset();
            const tabPanelWidth = tabPanel.width();
            const tabPanelRight = tabPanelOffset.left + tabPanelWidth - SAFETY_MARGIN;
            const tabPanelLeft = tabPanelOffset.left + SAFETY_MARGIN;

            const statOffset = $stat.offset();
            const tooltipWidth = $tooltip.outerWidth();

            // 計算 tooltip 在默認位置時的左右邊界
            const tooltipLeft = statOffset.left - (tooltipWidth / 2);
            const tooltipRight = statOffset.left + (tooltipWidth / 2);

            let position = {
                top: '100%',
                transform: 'translateX(-50%)'
            };

            // 如果會超出右邊界
            if (tooltipRight > tabPanelRight) {
                // 改為靠右對齊，並取消 transform
                position.left = 'auto';
                position.right = '0';
                position.transform = 'none';
            }
            // 如果會超出左邊界
            else if (tooltipLeft < tabPanelLeft) {
                // 改為靠左對齊，並取消 transform
                position.left = '0';
                position.transform = 'none';
            }
            else {
                // 維持居中
                position.left = '50%';
            }

            return position;
        }
        // 建立狀態碼內容
        function createStatusCodeTooltip($tooltip) {
            const $header = $("<div>").addClass("xn-restfulapi-test-stats-statusCode");

            $("<i>")
                .addClass("fas fa-check-circle")
                .css("color", "var(--success-color)")
                .appendTo($header);

            $("<span>")
                .addClass("xn-restfulapi-test-stats-statusCode-value")
                .text("200 OK")
                .appendTo($header);

            const $description = $("<div>")
                .addClass("xn-restfulapi-test-stats-statusCode-description")
                .text("Request successful. The server has responded as required.");

            $tooltip.append($header, $description);
        }
        // 建立回應時間內容
        function createResponseTimeTooltip($tooltip) {
            const $content = $("<div>").addClass("xn-restfulapi-test-stats-tooltip-content");

            const timelineData = [
                { phase: "Socket Initializtion", duration: 0.67, start: 0 },
                { phase: "Waiting (TTFB)", duration: 24.24, start: 0.67 },
                { phase: "SSL Handshake", duration: 2.01, start: 24.91 },
                { phase: "Download", duration: 1.30, start: 26.92 },

            ];

            // 計算總持續時間
            const totalDuration = timelineData.reduce((sum, item) => sum + item.duration, 0);

            // 創建標題行
            const $headerRow = $("<div>").addClass("xn-restfulapi-test-stats-tooltip-header-row");

            $("<div>")
                .addClass("xn-restfulapi-test-stats-tooltip-header-icon-cell")
                .append($("<i>")
                    .addClass("fas fa-clock")
                    .css("color", "var(--accent-color)"))
                .appendTo($headerRow);

            $("<div>")
                .addClass("xn-restfulapi-test-stats-tooltip-header-title-cell")
                .text("Response Time")
                .appendTo($headerRow);

            $("<div>")
                .addClass("xn-restfulapi-test-stats-tooltip-header-empty-cell")
                .appendTo($headerRow);

            $("<div>")
                .addClass("xn-restfulapi-test-stats-tooltip-header-total-cell")
                .text("28.35 ms")
                .appendTo($headerRow);

            $content.append($headerRow);

            // 創建數據行
            timelineData.forEach(item => {
                const $row = $("<div>")
                    .addClass("xn-restfulapi-test-stats-tooltip-data-row");

                $("<div>")
                    .addClass("xn-restfulapi-test-stats-tooltip-data-empty-cell")
                    .appendTo($row);

                $("<div>")
                    .addClass("xn-restfulapi-test-stats-tooltip-data-phase-cell")
                    .text(item.phase)
                    .appendTo($row);

                const $timelineCell = $("<div>")
                    .addClass("xn-restfulapi-test-stats-tooltip-data-timeline-cell");

                const $timeline = $("<div>")
                    .addClass("xn-restfulapi-test-stats-tooltip-data-timeline");

                // 計算開始位置和時間線長度的百分比
                const startPercentage = (item.start / totalDuration) * 100;
                const widthPercentage = (item.duration / totalDuration) * 100;

                $("<div>")
                    .addClass("xn-restfulapi-test-stats-tooltip-data-timeline-fill")
                    .css({
                        left: startPercentage + "%",
                        width: widthPercentage + "%",
                        backgroundColor: "#dc3545",
                    })
                    .appendTo($timeline);

                $timelineCell.append($timeline);
                $row.append($timelineCell);

                $("<div>")
                    .addClass("xn-restfulapi-test-stats-tooltip-data-duration-cell")
                    .text(item.duration + " ms")
                    .appendTo($row);

                $content.append($row);
            });

            $tooltip.append($content);
        }
        // 建立回應大小內容
        function createResponseSizeTooltip(container) {
            const $content = $("<div>")
                .addClass("xn-restfulapi-test-stats-responseSize");

            function createSizeSection(title, data, isResponse = true) {
                const $section = $("<div>")
                    .addClass("xn-restfulapi-test-stats-responseSize-section")
                    .css({
                        marginBottom: isResponse ? "15px" : "0"
                    });

                const $header = $("<div>")
                    .addClass("xn-restfulapi-test-stats-responseSize-header");

                $("<i>")
                    .addClass(isResponse ? "fas fa-download" : "fas fa-upload")
                    .css("color", "var(--accent-color)")
                    .appendTo($header);

                $("<span>")
                    .addClass("xn-restfulapi-test-stats-responseSize-caption")
                    .text(title)
                    .appendTo($header);

                $("<span>")
                    .addClass("xn-restfulapi-test-stats-responseSize-value")
                    .text(data.total)
                    .appendTo($header);

                const $details = $("<div>")
                    .addClass("xn-restfulapi-test-stats-responseSize-details-content");

                Object.entries(data.details).forEach(([key, value]) => {
                    const $row = $("<div>").addClass("xn-restfulapi-test-stats-responseSize-details");

                    $("<span>").text(key).appendTo($row);
                    $("<span>").text(value).appendTo($row);
                    $details.append($row);
                });

                $section.append($header, $details);

                if (isResponse) {
                    $section.append(
                        $("<div>").addClass("xn-restfulapi-test-stats-responseSize-details-response"));
                }

                return $section;
            }

            const responseData = {
                total: "532 B",
                details: {
                    "Headers": "148 B",
                    "Body": "384 B"
                }
            };

            const requestData = {
                total: "216 B",
                details: {
                    "Headers": "216 B",
                    "Body": "0 B"
                }
            };

            $content
                .append(createSizeSection("Response Size", responseData))
                .append(createSizeSection("Request Size", requestData, false));

            container.append($content);
        }
        return $container;
    }
    // #endregion::創建Test內容

}