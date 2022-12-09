function onSuccessRegister(response) {
    ShowMessageLogin(response.success, response.message);
    if (response.success) {
        clearForm();
    }
}
function clearForm() {
    $('#DisplayName').val("");
    $('#Email').val("");
    $('#Phone').val("");
    $('#UserName').val("");
    $('#Password').val("");
}

function onSuccessLogin(response) {
    if (response.success) {
        location.href = "/DashBoard";
    } else {
        ShowMessageLogin(response.success, response.message);
    }
}

function LoadRoleAuthentication() {
    $.ajax({

        url: '/User/GetRoleAuthentication',
        data: { roleId: $("#RoleId").val() },
        type: 'GET',
        success: function (node) {
            $("#sectionRole").empty().html(node);
        }
    });

}
function onSuccessSaveAuthentication(response) {
    ShowMessage(response.success, response.message);
}

function onSuccessChangePassword(response) {
    ShowMessage(response.success, response.message);
    if (response.success) {
        $('#OldPassword').val('');
        $('#Password').val('');
        $('#RePassword').val('');
    }
}

function onCreateAccount() {
    location.href = "/User/Register";
}

function onUserSuccessRegister() {
    ShowMessageLogin(data.success, data.message);
}

function onRegisterBegin() {
    $.validator.unobtrusive.parse("#frmRegisterUser");
}
