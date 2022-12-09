
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
                data: AddAntiForgeryToken({ controllerActionId: controllerActionId }),
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
                data: AddAntiForgeryToken({ models: getControllerActionRows() }),
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