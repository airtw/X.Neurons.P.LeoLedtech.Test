$(() => {

    var UPDATE_INTERVAL = 1 * 1000;

    function formatPercentage(value) {
        if (value == null || isNaN(value)) {
            return "-";
        }
        return value + " %";
    }


    function formatProductionSpeed(value) {
        if (value == null || isNaN(value)) {
            return "-";
        }
        if (value < 60) {

            return value + " Sec / PCS";;
        } else if (value < 3600) {

            const minutes = (value / 60).toFixed(1);
            return minutes + " Min / PCS";
        } else {

            const hours = (value / 3600).toFixed(1);
            return hours + " Hour / PCS";
        }
    }


    const realtimeGrid = $('.xn-realtime-grid').dxDataGrid({
        dataSource: [],
        showBorders: true,
        columnAutoWidth: true,
        paging: {
            enabled: false
        },
        columns: [
            {
                caption: '設備名稱',
                dataField: 'name',
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
            }
        ]
    }).dxDataGrid('instance');

    const getStatusText = (status) => {
        const statusMap = {
            0: '離線',
            1: '運行',
            2: '異常',
            3: '待機'
        };
        return statusMap[status] || '未知';
    };


    function processDataForPieChart(data) {

        const statusCount = {};
        data.forEach(item => {
            const status = item.equipmentStatus;
            const statusText = getStatusText(status);
            statusCount[statusText] = (statusCount[statusText] || 0) + 1;
        });

        return Object.entries(statusCount).map(([status, count]) => ({
            status: status,
            count: count
        }));
    }
    async function updateData(data) {
        try {
            if (!isConnectionReady(connection)) {
                await waitForConnection(connection);
            }

            const failureRateData = data.map(item => ({
                name: item.name,
                failureRate: item.utilizationInfo.failureRate
            }));

            equipmentErrorPiechart.option('dataSource', failureRateData);

        } catch (error) {
            console.error('更新數據失敗:', error);
        }
    }


    const equipmentStatusPiechart = $('.xn-piechart-equipment-status').dxPieChart({
        resolveLabelOverlapping: "shift",
        sizeGroup: "piesGroup",
        animation: {
            enabled: false,
            duration: 500
        },
        legend: {
            visible: false
        },
        series: [{
            argumentField: "status",
            valueField: "count",
            label: {
                visible: true,
                position: "outside",
                backgroundColor: "transparent",
                connector: {
                    visible: true,
                    width: 1
                },
                font: {
                    size: "1vh",
                    color: "var(--text-color)",
                },
                customizeText: function (arg) {
                    if (window.innerWidth < 768) {
                        return `
                        ${arg.argumentText} 數量: ${arg.valueText}\n
                        `;
                    }
                    return `
                        ${arg.argumentText}\n數量: ${arg.valueText}\n占比: ${(arg.percent * 100).toFixed(1)}%
                    `;
                }
            }
        }],
        tooltip: {
            enabled: false
        },
        customizePoint: function (arg) {
            // 根据状态定义颜色
            switch (arg.argument) {
                case "運行":
                    return { color: "#4ECDC4" }; // 绿色
                case "異常":
                    return { color: "#FF6B6B" }; // 红色
                case "離線":
                    return { color: "#999999" }; // 灰色
                case "待機":
                    return { color: "yellow" }; // 浅绿色
                default:
                    return {}; // 使用默认颜色
            }
        }
    }).dxPieChart('instance');

    const equipmentErrorPiechart = $('.xn-piechart-equipment-error').dxPieChart({
        palette: ['#FF6B6B', '#4ECDC4'],
        resolveLabelOverlapping: "shift",
        sizeGroup: "piesGroup",
        animation: {
            enabled: true,
            duration: 500
        },
        legend: {
            verticalAlignment: 'bottom',
            horizontalAlignment: 'center',
            itemTextPosition: 'right',
            font: {
                size: "1vh",
                color: "var(--text-color)",
            }
        },
        series: [{
            argumentField: "name",
            valueField: "failureRate",


        }],
        tooltip: {
            enabled: false
        }
    }).dxPieChart('instance');

    const sessionService = window.sessionDashboardService;
    const connection = sessionService.getConnection();

    function isConnectionReady(connection) {
        return connection && connection.state === "Connected";
    }

    async function renderStationInfo() {
        try {
            if (isConnectionReady(connection)) {
                const response = await connection.invoke("StationsInfo");
                if (!response) {
                    throw new Error('無效的站點資料格式');
                }

                realtimeGrid.option('dataSource', response);

                const newPieData = processDataForPieChart(response);
                const currentPieData = equipmentStatusPiechart.option('dataSource') || [];
    
                if (!arePieDataEqual(currentPieData, newPieData)) {
                    // 更新數據時才啟用動畫
                    equipmentStatusPiechart.option('animation.enabled', true);
                    equipmentStatusPiechart.option('dataSource', newPieData);
                } else {
                    // 數據沒變化時關閉動畫
                    equipmentStatusPiechart.option('animation.enabled', false);
                }


                updateData(response);
            }

        } catch (error) {
            handleError(error, 'renderStationInfo');
        }
    }

    function arePieDataEqual(data1, data2) {
        // 如果兩個都是空陣列或 null，視為相等
        if ((!data1 || data1.length === 0) && (!data2 || data2.length === 0)) {
            return true;
        }
    
        // 如果只有一個是空的，視為不相等
        if (!data1 || !data2 || data1.length !== data2.length) {
            return false;
        }
    
        return data1.every((item1) => {
            const item2 = data2.find(item => item.status === item1.status);
            return item2 && item1.count === item2.count;
        });
    }


    function setupIntervalUpdates() {

        setInterval(async () => {
            try {
                await renderStationInfo();
            } catch (error) {
                handleError(error, 'stationInfoInterval');
            }
        }, UPDATE_INTERVAL);
    }


    function handleError(error, context) {
        console.error(`Error in ${context}:`, error);
    }


    async function initialize() {
        try {

            await Promise.all([
                renderStationInfo()
            ]);

            setupIntervalUpdates();

        } catch (error) {
            handleError(error, 'initialize');
            throw error;
        }
    }

    initialize().catch(error => {
        console.error('程式初始化失敗:', error);
    });
});