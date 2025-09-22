$(() => {
    var SystemConfigForm = $('#system-config-form').dxForm({
        formData: {},
        readOnly: false,
        showColonAfterLabel: false,
        items: [{
            dataField: 'rate',
            label: {
                text: "輪播速率",
                location: 'top'
            },
            editorType: 'dxNumberBox',
            editorOptions: {
                min: 0,
                showSpinButtons: true,
                value: 0,
                format: "#0 秒"
            },
            validationRules: [{
                type: 'required',
                message: '輪播速率為必填',
            }],
        }, {
            dataField: 'clickHold',
            label: {
                text: '點選停止時間',
                location: 'top'
            },
            editorType: 'dxNumberBox',
            editorOptions: {
                min: 0,
                showSpinButtons: true,
                value: 0,
                format: "#0 秒"
            },
            validationRules: [{
                type: 'required',
                message: '點選停止時間為必填',
            }],
        }, {
            dataField: 'hold',
            label: {
                text: '滑動站台停止時間',
                location: 'top'
            },
            editorType: 'dxNumberBox',
            editorOptions: {
                min: 0,
                showSpinButtons: true,
                value: 0,
                format: "#0 秒"
            },
            validationRules: [{
                type: 'required',
                message: '滑動站台停止時間為必填',
            }],
        }, {
            itemType: 'button',
            buttonOptions: {
                text: '保存',
                onClick: () => {

                    const formInstance = $('#system-config-form').dxForm('instance');

                    const validateResult = formInstance.validate();

                    if (validateResult.isValid) {

                        const formData = formInstance.option('formData');
                        saveSystemConfig(formData);
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
                useSubmitBehavior: false,
                width: 'auto',
                height: '2.2rem',
            },
        }]
    }).dxForm('instance');

    function loadSystemConfig() {
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

        $.ajax({
            url: '/SystemConfig',
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`
            },
            success: function (response) {

                SystemConfigForm.option('formData', response);
            },
            error: function (xhr) {
                let errorMessage = '載入系統設定失敗';

                if (xhr.status === 401) {
                    errorMessage = '授權已過期，請重新登入';
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

    function saveSystemConfig(formData) {
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
            message: '儲存中...',
            showIndicator: true,
            showPane: true,
            visible: true
        }).dxLoadPanel('instance');

        $.ajax({
            url: '/SystemConfig',
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            data: JSON.stringify({
                Rate: parseInt(formData.rate),
                Hold: parseInt(formData.hold),
                ClickHold: parseInt(formData.clickHold)
            }),
            complete: function () {
                loadPanel.hide();
            },
            success: function (response) {
                if (response === true) {
                    DevExpress.ui.notify({
                        message: '儲存成功',
                        position: {
                            my: 'center top',
                            at: 'center top',
                        },
                    }, 'success', 3000);
                }
            },
            error: function (xhr) {
                let errorMessage = '儲存失敗';

                if (xhr.status === 401) {
                    errorMessage = '授權已過期，請重新登入';
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

    loadSystemConfig();

    var SystemUserForm = $('#system-config-user-form').dxForm({
        formData: {
            Name: localStorage.getItem('user')
        },
        readOnly: false,
        showColonAfterLabel: false,
        items: [{
            dataField: 'OldPassword',
            label: {
                text: "舊密碼",
                location: 'top'
            },
            editorType: 'dxTextBox',
            editorOptions: {
                mode: 'password',
                buttons: [{
                    name: 'password',
                    location: 'after',
                    options: {
                        icon: 'fas fa-eye',
                        type: 'default',
                        onClick: function (e) {
                            const textBox = SystemUserForm.getEditor('OldPassword');
                            const isPassword = textBox.option('mode') === 'password';
                            textBox.option('mode', isPassword ? 'text' : 'password');
                            e.component.option('icon', isPassword ? 'fas fa-eye-slash' : 'fas fa-eye');
                        }
                    }
                }]
            },
            validationRules: [{
                type: 'required',
                message: '舊密碼為必填',
            }],
        }, {
            dataField: 'NewPassword',
            label: {
                text: '新密碼',
                location: 'top'
            },
            editorType: 'dxTextBox',
            editorOptions: {
                mode: 'password',
                buttons: [{
                    name: 'password',
                    location: 'after',
                    options: {
                        icon: 'fas fa-eye',
                        type: 'default',
                        onClick: function (e) {
                            const textBox = SystemUserForm.getEditor('NewPassword');
                            const isPassword = textBox.option('mode') === 'password';
                            textBox.option('mode', isPassword ? 'text' : 'password');
                            e.component.option('icon', isPassword ? 'fas fa-eye-slash' : 'fas fa-eye');
                        }
                    }
                }]

            },
            validationRules: [{
                type: 'required',
                message: '新密碼為必填',
            }],
        }, {
            dataField: 'ConfirmPassword',
            label: {
                text: '再次輸入新密碼',
                location: 'top'
            },
            editorType: 'dxTextBox',
            editorOptions: {
                mode: 'password',
                buttons: [{
                    name: 'password',
                    location: 'after',
                    options: {
                        icon: 'fas fa-eye',
                        type: 'default',
                        onClick: function (e) {
                            const textBox = SystemUserForm.getEditor('ConfirmPassword');
                            const isPassword = textBox.option('mode') === 'password';
                            textBox.option('mode', isPassword ? 'text' : 'password');
                            e.component.option('icon', isPassword ? 'fas fa-eye-slash' : 'fas fa-eye');
                        }
                    }
                }]
            },
            validationRules: [{
                type: 'required',
                message: '請再次輸入新密碼',
            }, {
                type: 'custom',
                message: '兩次輸入的密碼不一致',
                validationCallback: function (e) {
                    const newPassword = SystemUserForm.getEditor('NewPassword').option('value');
                    return e.value === newPassword;
                }
            }],
        }, {
            itemType: 'button',
            buttonOptions: {
                text: '保存',
                onClick: () => {
                    const formInstance = $('#system-config-user-form').dxForm('instance');
                    const validateResult = formInstance.validate();

                    if (validateResult.isValid) {
                        const formData = formInstance.option('formData');
                        updateUserPassword(formData);
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
                useSubmitBehavior: false,
                width: 'auto',
                height: '2.2rem',
            },
        }]
    }).dxForm('instance');

    function updateUserPassword(formData) {
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
            message: '儲存中...',
            showIndicator: true,
            showPane: true,
            visible: true,
            position: { of: '#section' }
        }).dxLoadPanel('instance');

        const requestData = {
            Name: formData.Name,
            OldPassword: formData.OldPassword,
            NewPassword: formData.NewPassword
        };

        $.ajax({
            url: '/SystemConfig/User',
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
                if (response === true) {
                    DevExpress.ui.notify({
                        message: '密碼修改成功',
                        position: {
                            my: 'center top',
                            at: 'center top',
                        },
                    }, 'success', 3000);

                    SystemUserForm.resetValues();
                    localStorage.removeItem('token');
                    localStorage.removeItem('user');

                }
            },
            error: function (xhr) {
                let errorMessage = '密碼修改失敗';

                if (xhr.status === 401) {
                    errorMessage = '授權已過期，請重新登入';
                } else if (xhr.status === 400) {
                    errorMessage = '舊密碼錯誤';
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
});