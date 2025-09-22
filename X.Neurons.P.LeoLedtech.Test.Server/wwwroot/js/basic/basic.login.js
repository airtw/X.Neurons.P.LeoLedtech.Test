// basic.login.js
$(document).ready(function () {

    const newUrl = window.location.href.replace('.html', '');
    history.replaceState({}, '', newUrl);
    
    // 初始化 DevExpress 元件
    const usernameTextBox = $("#username").dxTextBox({
        name: "username",
        label: t('login_username_label'),
        labelMode: "floating",
        showClearButton: true,
    }).dxValidator({
        validationRules: [{
            type: "required",
            message: t('login_username_validationRules_message')
        }]
    }).dxTextBox("instance");

    const passwordTextBox = $("#password").dxTextBox({
        name: "password",
        label: t('login_password_label'),
        labelMode: "floating",
        mode: "password",
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
                    const button = $(e.element).dxButton('instance');
                    button.repaint();
                }
            }
        }]
    }).dxValidator({
        validationRules: [{
            type: "required",
            message: t('login_password_validationRules_message')
        }]
    }).dxTextBox("instance");

    $(".xn-login-button").dxButton({
        text: t('login_login_label'),
        type: "default",
        useSubmitBehavior: true,
        width: "100%"
    });

    // 初始化 toast
    const toast = $(".message-toast").dxToast({
        displayTime: 3000,
        animation: {
            show: { type: "fade", duration: 400 },
            hide: { type: "fade", duration: 400 }
        }
    }).dxToast("instance");

    // Loading panel
    const loadPanel = $("<div>").dxLoadPanel({
        position: { of: ".xn-login-container" },
        visible: false,
        showIndicator: true,
        showPane: true,
        shading: true,
    }).dxLoadPanel("instance");


    // Form submit handler
    $(".xn-login-form").on("submit", async function (e) {
        e.preventDefault();

        const username = usernameTextBox.option("value");
        const password = passwordTextBox.option("value");

        // 顯示載入中
        loadPanel.show();

        try {
            // 使用 SignalR 進行登入
            const connection = window.sessionService.getConnection();
            const response = await connection.invoke("Login", {
                username: username,
                password: password
            });

            if (response.success) {
                
                // 儲存 token
                localStorage.setItem("token", response.token);
                
                // 儲存 user
                localStorage.setItem("user", response.user.username);

                localStorage.setItem("role", response.user.role);

                localStorage.setItem("sessionID", response.connectionID);

                // 顯示成功訊息
                toast.option({
                    type: "success",
                    message: "登入成功! 正在導向管理頁面..."
                });
                toast.show();

                // 獲取 returnUrl 參數
                // const urlParams = new URLSearchParams(window.location.search);
                // const returnUrl = urlParams.get('returnUrl');

                // 延遲後導向
                setTimeout(() => {
                    window.location.href = "/index.html";
                }, 1500);
                console.log('登入成功!');
            } else {
                // 顯示錯誤訊息
                toast.option({
                    type: "error",
                    message: response.message || "Login failed"
                });
                toast.show();
            }
        } catch (error) {
            console.error("Login Error:", error);
            toast.option({
                type: "error",
                message: "Login failed. Please try again."
            });
            toast.show();
        } finally {
            loadPanel.hide();
        }
    });
});