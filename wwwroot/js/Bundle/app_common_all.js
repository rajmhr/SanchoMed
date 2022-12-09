function onStartConfigChoice() {
    $.validator.unobtrusive.parse("#frmAddEditConfigChoice");
}

function onSaveCategorySuccess(data) {
    ShowMessage(data.success, data.message);
    if (data.success) {
        $('#configPartialView').modal('hide');
        $("#tblConfigChoice").bootstrapTable('refresh');
    }
}

function openConfigChoiceModal(configChoiceId) {
    $.ajax({
        type: 'GET',
        url: 'ConfigChoice/AddUpdate',
        data: { configChoiceId: configChoiceId },
        success: function (result) {
            $('#configPartialView').html(result);
            $('#configPartialView').modal('show');
        }
    });
}

function addUpdateConfigChoice(configChoiceId) {
    openConfigChoiceModal(configChoiceId);
}

function saveConfigChoice() {
    $('#frmAddEditConfigChoice').submit();
}

function deleteConfigChoice(configChoiceId) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: 'ConfigChoice/DeleteConfigChoice',
                data: { configChoiceId: configChoiceId },
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success) {
                        $("#tblConfigChoice").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
}

function tableConfigchoiceActions(value, row) {
    return [
        '<a class="like" href="javascript:void(0)" title="Edit">',
        '<i class="fas fa-fw fa-edit" data-bookCopyId=' + row.configChoiceId + '  onclick="UpdateConfigChoiceAction(' + row.configChoiceId + ')"></i>',
        '</a> ',
        '<a class="danger remove" href="javascript:void(0)" onclick="DeleteConfigChoiceAction(' + row.configChoiceId + ')"  data-toggle="modal" data-target="#VisitorDelete" title="Remove">',
        '<i class="fas fa-trash"></i>',
        '</a>'
    ].join('');
}

function loadConfigChoices() {
    $("#tblConfigChoice").bootstrapTable('refresh');
}

function addExtraParam(param) {
    param.configChoiceCategory = $('#ConfigChoiceCategoryId').val();
    return param;
}
function onConfigCategoryBegin() {
    $.validator.unobtrusive.parse("#frmAddEditCategory");
}

function openCategoryModal(categoryId) {
    $.ajax({
        type: 'GET',
        url: 'ConfigChoiceCategory/AddUpdate',
        data: { configChoiceCategoryId: categoryId },
        success: function (result) {
            $('#categoryPartialView').html(result);
            $('#categoryPartialView').modal('show');
        }
    });
}

function saveConfigCategory() {
    $('#frmAddEditCategory').submit();
}

function addUpdateConfigCategory(categoryId) {
    openCategoryModal(categoryId);
}

function deleteCategory(categoryId) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: 'ConfigChoiceCategory/DeleteConfigChoiceCategory',
                data: { configChoiceCategoryId: categoryId },
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success) {
                        $("#tblConfigCategory").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
}

function tableActions(value, row) {
    return [
        '<a class="like" href="javascript:void(0)" title="Edit">',
        '<i class="fas fa-fw fa-edit" onclick="UpdateCategoryAction(' + row.configChoiceCategoryId + ')"  ></i>',
        '</a> ',
        '<a class="danger remove" href="javascript:void(0)" title="Remove">',
        '<i class="fas fa-trash" onclick="DeleteCategoryAction(' + row.configChoiceCategoryId + ')"></i>',
        '</a>'
    ].join('');
}

function onSaveCategorySuccess(data) {
    ShowMessage(data.success, data.message);
    if (data.success) {
        $('#categoryPartialView').modal('hide');
        $("#tblConfigCategory").bootstrapTable('refresh');
    }
}

function addExtraParam(param) {
    param.configChoiceRoleId = $('#ConfigChoiceRoleId').val();
    return param;
}

function onStartControllerAction() {
    $.validator.unobtrusive.parse("#frmAddEditControllerAction");
}

function onSaveControllerActionSuccess(data) {
    ShowMessage(data.success, data.message);
    if (data.success) {
        $('#contrillerActionPartialView').modal('hide');
        $("#tblControllerAction").bootstrapTable('refresh');
    }
}

function openControllerActionModal(controllerActionId) {
    $.ajax({
        type: 'GET',
        url: '/ControllerAction/AddUpdate',
        data: { controllerActionId: controllerActionId },
        success: function (result) {
            $('#contrillerActionPartialView').html(result);
            $('#contrillerActionPartialView').modal('show');
        }
    });
}

function addUpdateControllerAction(controlerActionRoleId) {
    openControllerActionModal(controlerActionRoleId);
}

function saveControllerActionModal() {
    $('#frmAddEditConfigChoice').submit();
}

function deleteControllerAction(controllerActionId) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: '/ControllerAction/DeleteControllerAction',
                data: { controllerActionId: controllerActionId },
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success) {
                        $("#tblControllerAction").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
}
function loadControllerAction() {
    $("#tblControllerAction").bootstrapTable('refresh');
}

function tableConfigchoiceActions(value, row) {
    return [
        '<a class="like" href="javascript:void(0)" title="Edit">',
        '<i class="fas fa-fw fa-edit" data-bookCopyId=' + row.configChoiceId + 'onclick="addUpdateControllerAction(' + row.controllerActionId + ')"></i>',
        '</a> ',
        '<a class="danger remove" href="javascript:void(0)" onclick="deleteControllerAction(' + row.controllerActionId + ')"  data-toggle="modal" data-target="#VisitorDelete" title="Remove">',
        '<i class="fas fa-trash"></i>',
        '</a>'
    ].join('');
}
function assignFormatter(value, row, index) {
    var data = $("#ConfigChoiceRoleId").val();
    if (data === "-- Select Role --" || data === undefined) {
        return {
            disabled: true
        };
    }
}
function saveControllerActionToRole() {

    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: "/ControllerAction/SaveControllerActionMapping",
                data: { models: getControllerActionRows() },
                type: "POST",
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success) {
                        $("#tblControllerAction").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
}
function getControllerActionRows() {
    var rows = $('#tblControllerAction').bootstrapTable('getSelections');
    if (rows.length === 0) {
        var result = new Array();
        var data = {
            configRoleId: $('#ConfigChoiceRoleId').val(),
            controllerName: "",
            actionName: ""
        };
        result.push(data);
        return result;
    }
    else {
        return rows;
    }

}
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


function onRegisterBegin() {
    $.validator.unobtrusive.parse("#frmAdminRegister");
}

function onUserSuccessRegister(data) {
    ShowMessage(data.success, data.message);
    if (data.success) {
        $('#userPartialView').modal('hide');
        $("#tblUserDetail").bootstrapTable('refresh');
    }
}

function openUserDetailModal(userDetailId) {
    $.ajax({
        type: 'GET',
        url: 'UserDetail/AddUpdate',
        data: { userDetailId: userDetailId },
        success: function (result) {
            $('#userPartialView').html(result);
            $('#userPartialView').modal('show');
        }
    });
}

function saveUser() {
    $('#frmAdminRegister').submit();
}

function addUpdateUserDetail(userDetailId) {
    openUserDetailModal(userDetailId);
}

function deleteUserDetail(userDetailId) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: 'UserDetail/Delete',
                data: { userDetailId: userDetailId },
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success) {
                        $("#tblUserDetail").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
}

function queryUserDetailParams(params) {
    var searchItem = $('#userSearchId').val();
    var roleId = $('#ConfigChoiceRoleId').val();
    params.search = searchItem;
    params.roleId = roleId;
    return params;
}

function searchUser() {
    $("#tblUserDetail").bootstrapTable('refresh');
}

function tableUserDetailActions(value, row) {
    return [
        '<a class="like" href="javascript:void(0)" title="Edit">',
        '<i class="fas fa-edit" data-bookCopyId=' + row.userDetailId + '  onclick="UpdateUserDetailAction(' + row.userDetailId + ')"></i>',
        '</a> ',
        '<a class="danger remove" href="javascript:void(0)" onclick="deleteUserDetail(' + row.userDetailId + ')"  data-toggle="modal" title="Remove">',
        '<i class="fas fa-trash"></i>',
        '</a>'
    ].join('');
}

function onUserSuccessApprove(data) {
    ShowMessage(data.success, data.message);
    if (data.success) {
        $("#tblUserDetail").bootstrapTable('refresh');
        revDlg.close();
    }
}

function runningFormatter(value, row, index) {
    var tableOptions = $('#tblUserDetail').bootstrapTable('getOptions');
    return ((tableOptions.pageNumber - 1) * tableOptions.pageSize) + (1 + index);
}
function dateFormatter(date) {
    if (date !== null) {
        var data = date.substring(0, 10);
        return data;
    }
}

function fullDateFormatter(date) {
    if (date !== null) {
        var data = date.substring(0, 16);
        return data;
    }
}

function booleanFormatter(data) {
    if (data === true)
        return "Yes";
    return "No";
}