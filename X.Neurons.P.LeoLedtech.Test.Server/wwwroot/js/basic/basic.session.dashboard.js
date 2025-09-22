// basic.session.dashboard.js
class SessionDashboardService {
    static instance = null;

    constructor() {
        if (SessionDashboardService.instance) {
            return SessionDashboardService.instance;
        }

        // 設定連線保持時間和重試策略
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/dashboardHub")
            .withAutomaticReconnect([0, 2000, 5000, 10000]) // 重試間隔（毫秒）
            .build();

        // 等待 DOM 準備好後初始化 icons
        document.addEventListener('DOMContentLoaded', () => {
            this.initializeIcons();
        });

        this.setupEventHandlers();
        this.setupHeartbeat();
        SessionDashboardService.instance = this;
    }

    initializeIcons() {
        this.icons = {
            connected: document.querySelector('.connection-icon.connected'),
            disconnected: document.querySelector('.connection-icon.disconnected'),
            error: document.querySelector('.connection-icon.error')
        };
        this.currentStatus = 'disconnected';
        this.setStatus('disconnected'); // 設定初始狀態
    }

    setStatus(status) {
        if (!this.icons) {
            // console.warn('Icons not initialized yet');
            return;
        }
        // 隱藏所有圖標
        Object.values(this.icons).forEach(icon => {
            if (icon) {
                icon.style.display = 'none';
            }
        });
        
        // 顯示對應狀態的圖標
        switch(status) {
            case 'connected':
                if (this.icons.connected) {
                    this.icons.connected.style.display = 'block';
                }
                break;
            case 'disconnected':
                if (this.icons.disconnected) {
                    this.icons.disconnected.style.display = 'block';
                }
                break;
            case 'error':
                if (this.icons.error) {
                    this.icons.error.style.display = 'block';
                }
                break;
            default:
                console.warn('Unknown connection status:', status);
                return;
        }
        
        this.currentStatus = status;
    }

    getStatus() {
        return this.currentStatus;
    }

    setupEventHandlers() {
        this.connection.onreconnecting(error => {
            console.log('正在重新連線:', error);
            this.setStatus('disconnected');
            const systemTimeElement = document.querySelector('.system-time');
            systemTimeElement.textContent = '-';
            this.showMessage("正在嘗試重新連線...", "warning", 0, true);
        });

        this.connection.onreconnected(connectionId => {
            console.log('已重新連線:', connectionId);
            this.setStatus('connected');
            this.showMessage("已重新連線", "success", 20000, false);
        });

        this.connection.onclose(error => {
            console.log('連線已關閉:', error);
            this.setStatus('disconnected');
            // 嘗試重新連線
            this.start();
        });

        this.connection.on("HeartbeatResponse", () => {
            // console.log('收到心跳響應');
            this.setStatus('connected');
            this.lastHeartbeat = Date.now();
        });

        this.connection.on("SystemTime", (timeValue) => {
            // console.log("收到系統時間:", timeValue);
        
            // 更新到某個 DOM 元素（假設有元素 .system-time）
            const systemTimeElement = document.querySelector('.system-time');
            if (systemTimeElement) {
                systemTimeElement.textContent = timeValue;
            }
        });
    }

    setupHeartbeat() {
        this.heartbeatInterval = setInterval(async () => {
            if (this.connection.state === signalR.HubConnectionState.Connected) {
                try {
                    await this.connection.invoke("Heartbeat");
                    await this.connection.invoke("SystemTime");
                } catch (error) {
                    console.error('心跳檢測失敗:', error);
                    const systemTimeElement = document.querySelector('.system-time');
                    systemTimeElement.textContent = '-';
                    // 嘗試重新連線
                    await this.start();
                }
            }
        }, 1000); // 改為 30 秒
    }

    showMessage(message, type, displayTime, isCancel) {
        if (this.toast) {
            this.toast.option({
                message: message,
                type: type,
                displayTime: displayTime,
                onHiding: function (e) {
                    e.cancel = isCancel;
                }
            });
            this.toast.show();
        }
    }

    dispose() {
        if (this.heartbeatInterval) {
            clearInterval(this.heartbeatInterval);
            this.heartbeatInterval = null;
        }
        if (this.connection) {
            this.connection.stop();
        }
    }

    async start() {
        if (this.connection.state === "Connected") {
            return;
        }
        try {
            await this.connection.start();
            console.log("SignalR Connected");
        } catch (err) {
            console.error("SignalR Connection Error:", err);
            // 如果連線失敗，5秒後重試
            setTimeout(() => this.start(), 5000);
        }
    }

    getConnection() {
        return this.connection;
    }
}

// 創建全局實例
window.sessionDashboardService = new SessionDashboardService();