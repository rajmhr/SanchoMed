
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
                data: AddAntiForgeryToken({ userDetailId: userDetailId }),
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

function loadMedicalOrgs() {
    var medOrgUrl = $('#medOrgSelect').data('url');

    $('#medOrgSelect').select2({
        minimumInputLength: 3,
        multiple: false,
        placeholder: "Please type medical title.",
        dropdownCssClass: 'bigdrop',
        allowClear: true,
        dataType: 'json',
        ajax: {
            url: medOrgUrl,
            global: false,
            data: function (params) {
                var query = { medicalOrgName: params.term };
                return query;
            },
            processResults: function (data) {
                console.log(data);
                return {
                    results: $.map(data.returnObject, function (item) {
                        return {
                            text: `<div class="text-center text-gray-900 text-small">${item.title} -- ${item.address}</div>`,
                            id: item.medicalOrgId
                        };
                    })
                };
            }
        },
        escapeMarkup: function (markup) { return markup; },
        templateSelection: formatRepoSelection
    });
}

function formatRepoSelection(item) {
    if (item.display === undefined)
        return item.text;
    return item.display;
}

function setMedicalOrg(jdata) {
    var s2 = $("#medOrgSelect").data('select2');
    console.log(jdata);
    if (jdata) {
        s2.trigger('select', {
            data:
            {
                text: `${jdata.title} -- ${jdata.address}`,
                id: jdata.medicalOrgId
            }
        });
    }
}