document.addEventListener('DOMContentLoaded', async () => {
    // 全域常量定義
    var UPDATE_INTERVAL = 1000; // 更新間隔（毫秒）
    var SCROLL_PAUSE_DURATION = 5000; // 滾動暫停時間（毫秒）
    var SCROLL_HOLD_DURATION = 10000; // 手動點擊暫停時間（毫秒）
    var SCROLL_ANIMATION_DURATION = 250; // 滾動動畫時間（毫秒）

    // 全域狀態管理
    const state = {
        isPauseAutoScroll: false,
        pauseTimer: null,
        isManualScroll: false,
        currentActiveStation: null
    };

    // 服務初始化
    const sessionService = window.sessionDashboardService;
    const connection = sessionService.getConnection();
    const chartDom = document.getElementById('gaugeChart');
    const myChart = echarts.init(chartDom, null, {
        renderer: 'canvas',
        useDirtyRect: false
    });

    // ECharts 儀表板基礎配置
    const gaugeOptions = {
        series: [{
            type: 'gauge',
            center: ['50%', calculateCenterY()],
            startAngle: 180,
            endAngle: 0,
            min: 0,
            max: 100,
            radius: '120%',
            splitNumber: 10,
            itemStyle: {
                color: '#58D9F9',
                shadowColor: 'rgba(0,138,255,0.45)',
                shadowBlur: 10,
                shadowOffsetX: 2,
                shadowOffsetY: 2
            },
            progress: {
                show: true,
                roundCap: true,
                width: 18
            },
            pointer: {
                icon: 'path://M2090.36389,615.30999 L2090.36389,615.30999 C2091.48372,615.30999 2092.40383,616.194028 2092.44859,617.312956 L2096.90698,728.755929 C2097.05155,732.369577 2094.2393,735.416212 2090.62566,735.56078 C2090.53845,735.564269 2090.45117,735.566014 2090.36389,735.566014 L2090.36389,735.566014 C2086.74736,735.566014 2083.81557,732.63423 2083.81557,729.017692 C2083.81557,728.930412 2083.81732,728.84314 2083.82081,728.755929 L2088.2792,617.312956 C2088.32396,616.194028 2089.24407,615.30999 2090.36389,615.30999 Z',
                length: '75%',
                width: 16,
                offsetCenter: [0, '5%']
            },
            axisLine: {
                roundCap: true,
                lineStyle: { width: 18 }
            },
            axisTick: {
                splitNumber: 2,
                lineStyle: { width: 2, color: '#999' }
            },
            splitLine: {
                length: 12,
                lineStyle: { width: 3, color: '#999' }
            },
            axisLabel: {
                distance: 30,
                color: '#999',
                fontSize: 20
            },
            title: { show: false },
            detail: {
                backgroundColor: 'transparent',
                borderWidth: 0,
                width: '60%',
                lineHeight: 40,
                height: 40,
                borderRadius: 8,
                offsetCenter: [0, '35%'],
                valueAnimation: true,
                formatter: value => `{value|${value.toFixed(0)}}{unit|%}`,
                rich: {
                    value: {
                        fontSize: 60,
                        fontWeight: 'bolder',
                        color: '#4f6483'
                    },
                    unit: {
                        fontSize: 20,
                        color: '#999',
                        padding: [0, 0, -20, 10]
                    }
                }
            },
            data: [{ value: 50 }]
        }]
    };
    // 核心工具函數
    function calculateCenterY() {
        const desktopWidth = 1200;
        const desktopCenterY = 70;
        const currentWidth = window.innerWidth;
        const scale = currentWidth / desktopWidth;
        const adjustedY = Math.min(desktopCenterY, desktopCenterY * scale);
        return `${adjustedY}%`;
    }

    // 狀態指示器相關函數
    function getStatusLightClass(status) {
        const statusMap = {
            1: 'light-normal',   // 正常狀態 - 綠色
            2: 'light-error',    // 錯誤狀態 - 紅色
            3: 'light-warning'   // 警告狀態 - 黃色
        };
        return statusMap[status] || 'light-off';  // 預設為關閉狀態 - 黑色
    }

    function getStatusIndicatorClass(status) {
        const statusMap = {
            1: 'status-normal',  // 正常狀態
            2: 'status-error',   // 錯誤狀態
            3: 'status-warning'  // 警告狀態
        };
        return statusMap[status] || 'status-stop';  // 預設為停止狀態
    }

    // 更新狀態指示器顯示
    function updateStatusIndicators(status) {
        // 定義所有需要更新的元素
        const elements = {
            error: $('#equipment-error-status'),
            wait: $('#equipment-wait-status'),
            stop: $('#equipment-stop-status'),
            run: $('#equipment-run-status')
        };

        // 所有可能的狀態類別
        const allStatuses = ['status-normal', 'status-error',
            'status-warning', 'status-stop'];

        // 清除所有元素的所有狀態
        Object.values(elements).forEach(element => {
            allStatuses.forEach(statusClass => {
                element.removeClass(statusClass);
            });
        });

        // 根據當前狀態更新顯示
        switch (status) {
            case 1: // 運行狀態
                elements.error.addClass('status-stop');
                elements.wait.addClass('status-stop');
                elements.stop.addClass('status-stop');
                elements.run.addClass('status-normal');
                break;
            case 2: // 錯誤狀態
                elements.error.addClass('status-error');
                elements.wait.addClass('status-stop');
                elements.stop.addClass('status-stop');
                elements.run.addClass('status-stop');
                break;
            case 3: // 警告狀態
                elements.error.addClass('status-stop');
                elements.wait.addClass('status-warning');
                elements.stop.addClass('status-stop');
                elements.run.addClass('status-stop');
                break;
            default: // 停止狀態
                Object.values(elements).forEach(element => {
                    element.addClass('status-stop');
                });
                break;
        }
    }

    // 滾動處理輔助函數
    function handleScrollPause() {
        if (state.isManualScroll) return;

        if (state.pauseTimer) {
            clearTimeout(state.pauseTimer);
        }

        state.isPauseAutoScroll = true;
        state.pauseTimer = setTimeout(() => {
            state.isPauseAutoScroll = false;
            state.pauseTimer = null;
        }, SCROLL_PAUSE_DURATION);
    }

    // 錯誤處理輔助函數
    function handleError(error, context) {
        console.error(`Error in ${context}:`, error);
    }

    // 視窗大小變化處理
    function handleResize() {
        try {
            myChart.resize();
            updateChartOptions();
        } catch (error) {
            handleError(error, 'handleResize');
        }
    }

    // 動畫相關輔助函數
    function setTemporaryState(duration, callback) {
        return new Promise((resolve) => {
            setTimeout(() => {
                callback();
                resolve();
            }, duration);
        });
    }
    // 圖表值更新處理
    function updateGaugeValue(newValue) {
        try {
            // 更新圖表數據
            gaugeOptions.series[0].data[0].value = newValue;
            myChart.setOption(gaugeOptions);

            // 更新圖表配置
            updateChartOptions();

            // 更新顯示值
            const gaugeValueElement = document.querySelector('.gauge-value');
            if (gaugeValueElement) {
                gaugeValueElement.textContent = newValue.toFixed(0);
            }
        } catch (error) {
            handleError(error, 'updateGaugeValue');
        }
    }

    // 圖表配置更新
    function updateChartOptions() {
        const screenWidth = window.innerWidth;
        let chartConfig;

        try {
            // 根據螢幕寬度設置不同的配置
            if (screenWidth <= 768) {
                // 手機版配置
                chartConfig = getMobileChartConfig();
            } else if (screenWidth <= 1024) {
                // 平板版配置
                chartConfig = getTabletChartConfig();
            } else {
                // 桌面版配置
                chartConfig = getDesktopChartConfig();
            }

            myChart.setOption(chartConfig);
        } catch (error) {
            handleError(error, 'updateChartOptions');
        }
    }

    // 手機版圖表配置
    function getMobileChartConfig() {
        return {
            series: [{
                center: ['50%', '70%'],
                progress: {
                    show: true,
                    roundCap: true,
                    width: 8
                },
                pointer: {
                    length: '70%',
                    width: 10
                },
                axisLine: {
                    roundCap: true,
                    lineStyle: {
                        width: 8
                    }
                },
                splitLine: {
                    length: 3
                },
                axisLabel: {
                    show: false
                },
                detail: {
                    show: false
                }
            }]
        };
    }

    // 平板版圖表配置
    function getTabletChartConfig() {
        return {
            series: [{
                center: ['50%', '70%'],
                detail: {
                    ...gaugeOptions.series[0].detail,
                    offsetCenter: [0, '30%'],
                    rich: {
                        ...gaugeOptions.series[0].detail.rich,
                        value: {
                            ...gaugeOptions.series[0].detail.rich.value,
                            fontSize: 50
                        }
                    }
                }
            }]
        };
    }

    // 桌面版圖表配置
    function getDesktopChartConfig() {
        return {
            series: [{
                center: ['50%', calculateCenterY()]
            }]
        };
    }

    // 圖表事件處理
    function setupChartEventListeners() {
        // 視窗大小變化監聽
        window.addEventListener('resize', _.debounce(() => {
            try {
                myChart.resize();
                updateChartOptions();
            } catch (error) {
                handleError(error, 'chartResize');
            }
        }, 250));

        // 圖表點擊事件
        myChart.on('click', params => {
            try {
                // 這裡可以添加更多圖表交互邏輯
            } catch (error) {
                handleError(error, 'chartClick');
            }
        });
    }

    // 圖表銷毀處理
    function destroyChart() {
        try {
            if (myChart) {
                myChart.dispose();
            }
        } catch (error) {
            handleError(error, 'destroyChart');
        }
    }
    // 站點點擊處理函數
    async function handleStationClick(station) {
        try {
            // 清除現有計時器
            if (state.pauseTimer) {
                clearTimeout(state.pauseTimer);
            }

            // 更新狀態
            state.isPauseAutoScroll = true;
            state.currentActiveStation = station;

            // 設置新的計時器
            state.pauseTimer = setTimeout(async () => {
                // 重置所有狀態
                state.isPauseAutoScroll = false;
                state.currentActiveStation = null;
                state.pauseTimer = null;

                // 更新顯示
                await Promise.all([
                    renderStationList(),
                    renderStationInfo()
                ]);
            }, SCROLL_HOLD_DURATION);

            // 立即更新顯示
            await Promise.all([
                renderStationList(),
                renderStationInfo()
            ]);
        } catch (error) {
            handleError(error, 'handleStationClick');
        }
    }

    // 站點列表渲染函數
    async function renderStationList() {
        try {
            const response = await connection.invoke("Pages");
            if (!response) {
                throw new Error('無效的站點資料格式');
            }

            const stationList = document.querySelector('.station-list');
            if (!stationList) return;

            // 保存當前活動項目的索引
            const previousActiveIndex = Array.from(stationList.children)
                .findIndex(item => item.classList.contains('active'));

            // 清空列表
            stationList.innerHTML = '';

            // 創建並添加新的站點元素
            let activeItem = null;
            for (const station of response) {
                if (!station.name) continue;

                const stationItem = createStationElement(station);
                stationList.appendChild(stationItem);

                if (isActiveStation(station)) {
                    activeItem = stationItem;
                }
            }

            // 處理自動滾動
            if (activeItem && !state.isPauseAutoScroll) {
                await handleAutoScroll(stationList, activeItem, previousActiveIndex);
            }

        } catch (error) {
            handleError(error, 'renderStationList');
        }
    }

    // 創建站點元素
    function createStationElement(station) {
        const stationItem = document.createElement('div');
        const isActive = isActiveStation(station);
        stationItem.className = `station-item ${isActive ? 'active' : ''}`;

        // 創建狀態指示燈
        const statusLight = document.createElement('div');
        statusLight.className = 'status-light';
        const lightDiv = document.createElement('div');
        lightDiv.className = getStatusLightClass(station.equipmentStatus);
        statusLight.appendChild(lightDiv);

        // 創建站點名稱
        const stationName = document.createElement('span');
        stationName.textContent = station.name;

        // 組裝元素
        stationItem.append(statusLight, stationName);

        // 添加點擊事件
        stationItem.addEventListener('click', () => handleStationClick(station));

        return stationItem;
    }

    // 判斷站點是否為活動狀態
    function isActiveStation(station) {
        return state.currentActiveStation ?
            station.name === state.currentActiveStation.name :
            station.isPageActive;
    }

    // 處理自動滾動
    async function handleAutoScroll(stationList, activeItem, previousActiveIndex) {
        if (!activeItem) return;

        state.isManualScroll = true;
        const currentActiveIndex = Array.from(stationList.children)
            .findIndex(item => item.classList.contains('active'));

        try {
            await (window.innerWidth <= 768 ?
                handleMobileScroll(stationList, activeItem, previousActiveIndex, currentActiveIndex) :
                handleDesktopScroll(stationList, previousActiveIndex, currentActiveIndex));

        } catch (error) {
            handleError(error, 'handleAutoScroll');
        } finally {
            // 確保解除手動滾動狀態
            setTimeout(() => {
                state.isManualScroll = false;
            }, SCROLL_ANIMATION_DURATION);
        }
    }

    // 處理手機版滾動
    async function handleMobileScroll(stationList, activeItem, previousActiveIndex, currentActiveIndex) {
        const itemWidth = activeItem.offsetWidth;
        const containerWidth = stationList.offsetWidth;
        const scrollPosition = activeItem.offsetLeft - (containerWidth / 2) + (itemWidth / 2);

        if (previousActiveIndex === stationList.children.length - 1 && currentActiveIndex === 0) {
            // 從最後回到第一個的處理
            await smoothScroll(stationList, { left: 0 });
        } else {
            // 一般滾動處理
            await smoothScroll(stationList, {
                left: Math.max(0, scrollPosition - 40)
            });
        }
    }

    // 處理桌面版滾動
    async function handleDesktopScroll(stationList, previousActiveIndex, currentActiveIndex) {
        if (previousActiveIndex === stationList.children.length - 1 && currentActiveIndex === 0) {
            // 從最後回到第一個的處理
            await smoothScroll(stationList, { top: 0 });
        } else {
            // 一般滾動處理
            const activeItem = stationList.children[currentActiveIndex];
            if (activeItem) {
                activeItem.scrollIntoView({
                    behavior: 'smooth',
                    block: 'nearest'
                });
            }
        }
    }

    // 平滑滾動處理
    function smoothScroll(element, options) {
        return new Promise(resolve => {
            element.scrollTo({
                ...options,
                behavior: 'smooth'
            });

            // 監聽滾動結束
            const handleScrollEnd = () => {
                element.removeEventListener('scrollend', handleScrollEnd);
                resolve();
            };

            element.addEventListener('scrollend', handleScrollEnd);

            // 設置超時保護
            setTimeout(resolve, SCROLL_ANIMATION_DURATION);
        });
    }
    // 站點資訊更新函數
    async function renderStationInfo() {
        try {
            const response = await connection.invoke("StationsInfo");
            if (!response) {
                throw new Error('無效的站點資料格式');
            }

            // 尋找並更新活動站點的資訊
            for (const station of response) {
                if (!station.name) continue;

                const isActive = isActiveStation(station);
                if (isActive) {
                    updateStationDisplay(station);
                    break; // 找到活動站點後就停止搜尋
                }
            }
        } catch (error) {
            handleError(error, 'renderStationInfo');
        }
    }

    // 更新站點顯示
    function updateStationDisplay(station) {
        try {
            // 更新生產資訊
            $('#production-count-value').text(station.productionInfo.productionCount);
            // $('#production-rate-value').text(station.productionInfo.productionRate);
            formatProductionRate(station.productionInfo.productionRate);
            $('#equipment-station').text(`站別：${station.name}`);

            // 更新狀態指示器
            updateStatusIndicators(station.equipmentStatus);

            // 更新儀表板
            updateGaugeValue(station.utilizationInfo.equipmentUtilizationRate);
        } catch (error) {
            handleError(error, 'updateStationDisplay');
        }
    }
    //production-rate-unit
    function formatProductionRate(rateInSeconds) {
        if (rateInSeconds < 60) {
            // 小於 1 分鐘，顯示秒
            $('#production-rate-value').text(rateInSeconds);
            $('#production-rate-unit').text('Sec / PCS');
        } else if (rateInSeconds < 3600) {
            // 小於 1 小時，顯示分鐘
            const minutes = (rateInSeconds / 60).toFixed(1); // 保留 1 位小數
            $('#production-rate-value').text(minutes);
            $('#production-rate-unit').text('Min / PCS');
        } else {
            // 大於等於 1 小時，顯示小時
            const hours = (rateInSeconds / 3600).toFixed(1); // 保留 1 位小數
            $('#production-rate-value').text(hours);
            $('#production-rate-unit').text('Hour / PCS');
        }
    }

    // 設置事件監聽器
    function setupEventListeners() {
        // 視窗大小變化監聽
        window.addEventListener('resize', _.debounce(handleResize, 250));

        // 滾動事件監聽
        const stationList = document.querySelector('.station-list');
        if (stationList) {
            // 桌面版滾動處理
            stationList.addEventListener('scroll', () => {
                if (!state.isManualScroll && window.innerWidth > 768) {
                    handleScrollPause();
                }
            });

            // 移動版觸控處理
            stationList.addEventListener('touchstart', () => {
                if (!state.isManualScroll) {
                    handleScrollPause();
                }
            });
        }

        // 站點更新事件監聽
        connection.on("StationsUpdated", async () => {
            try {
                console.log('收到站點更新通知');
                if (connection.state === signalR.HubConnectionState.Connected) {
                    await renderStationList();
                }
            } catch (error) {
                handleError(error, 'StationsUpdated');
            }
        });

        // 添加SignalR重連事件監聽
        connection.onreconnected(async connectionId => {
            try {
                console.log('SignalR reconnected:', connectionId);
                // 重新獲取設定
                await updateConfig();
            } catch (error) {
                handleError(error, 'reconnectionHandler');
            }
        });
        // SignalR斷線重連相關事件
        connection.onreconnecting(error => {
            try {
                console.log('SignalR reconnecting:', error);
                clearAllData();
            } catch (error) {
                handleError(error, 'reconnectingHandler');
            }
        });
        connection.onclose(error => {
            try {
                console.log('SignalR connection closed:', error);
                clearAllData();
            } catch (error) {
                handleError(error, 'closeHandler');
            }
        });
    }

    // 設置定時更新
    function setupIntervalUpdates() {
        // 站點列表更新
        setInterval(async () => {
            try {
                await renderStationList();
            } catch (error) {
                handleError(error, 'stationListInterval');
            }
        }, UPDATE_INTERVAL);

        // 站點資訊更新
        setInterval(async () => {
            try {
                await renderStationInfo();
            } catch (error) {
                handleError(error, 'stationInfoInterval');
            }
        }, UPDATE_INTERVAL);
    }

    // 資源清理
    function cleanup() {
        try {
            // 清除計時器
            if (state.pauseTimer) {
                clearTimeout(state.pauseTimer);
            }

            // 銷毀圖表
            destroyChart();

            // 移除事件監聽器
            window.removeEventListener('resize', handleResize);

        } catch (error) {
            handleError(error, 'cleanup');
        }
    }

    async function updateConfig() {
        try {
            const config = await connection.invoke("Config");
            if (config.carousel !== null) {
                SCROLL_PAUSE_DURATION = config.carousel.hold * 1000;
                SCROLL_HOLD_DURATION = config.carousel.clickHold * 1000;
            }
        } catch (error) {
            handleError(error, 'updateConfig');
        }

    }
    function clearAllData() {
        try {
            const stationList = document.querySelector('.station-list');
            if (!stationList) return;
            // 清空列表
            stationList.innerHTML = '';
            // 更新生產資訊
            $('#production-count-value').text('0');
            $('#production-rate-value').text('0');
            $('#equipment-station').text(`站別：`);

            // 更新狀態指示器
            // 定義所有需要更新的元素
            const elements = {
                error: $('#equipment-error-status'),
                wait: $('#equipment-wait-status'),
                stop: $('#equipment-stop-status'),
                run: $('#equipment-run-status')
            };

            // 所有可能的狀態類別
            const allStatuses = ['status-normal', 'status-error',
                'status-warning', 'status-stop'];

            // 清除所有元素的所有狀態
            Object.values(elements).forEach(element => {
                allStatuses.forEach(statusClass => {
                    element.removeClass(statusClass);
                });
            });

            // 更新儀表板
            updateGaugeValue(0);
        } catch (error) {
            handleError(error, 'clearAllData');
        }
    }

    // 主程式初始化
    async function initialize() {
        try {
            // 啟動連接
            await sessionService.start();
            console.log('Session service started successfully.');

            //獲取伺服器設定
            updateConfig();

            // 初始渲染
            await Promise.all([
                renderStationList(),
                renderStationInfo()
            ]);

            // 設置事件監聽器
            setupEventListeners();

            // 設置定時更新
            setupIntervalUpdates();

            // 初始化圖表
            myChart.setOption(gaugeOptions);
            updateChartOptions();

            // 設置清理處理
            window.addEventListener('unload', cleanup);

        } catch (error) {
            handleError(error, 'initialize');
            throw error; // 重拋錯誤以通知調用者初始化失敗
        }
    }

    // 啟動主程式
    initialize().catch(error => {
        console.error('程式初始化失敗:', error);
        // 這裡可以添加更多錯誤處理邏輯，如顯示錯誤提示等
    });

});