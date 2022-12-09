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
                data: AddAntiForgeryToken({ configChoiceId: configChoiceId }),
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