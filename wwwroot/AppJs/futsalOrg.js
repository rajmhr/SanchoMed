
function onRegisterBegin() {
    $.validator.unobtrusive.parse("#frmFutsalOrg");
}

function openFutsalOrgModal(orgId, title) {
    $.ajax({
        type: 'GET',
        url: 'FutsalOrg/AddUpdate',
        data: { orgId: orgId, title: title },
        success: function (result) {
            $('#futsalOrgPartialView').html(result);
            $('#futsalOrgPartialView').modal('show');
        }
    });
}


function addUpdateFutsalOrg(orgId, title) {
    openFutsalOrgModal(orgId, title);
}

function deleteFutsalOrg(orgId) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: 'FutsalOrg/DeleteOrg',
                data: { orgId: orgId },
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success) {
                        $("#tblFutsalOrg").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
}


function orgRowStyle(row, index) {
    if (!row.isActive) {
        return {
            classes: 'alert-danger'
        };
    }
    return {
        classes: 'text-grey-900'
    };
}

function searchFutsalOrg() {
    $("#tblFutsalOrg").bootstrapTable('refresh', {
        query: {limit: 10, offset: 0}
    });
}

function queryFutsalOrgParams(params) {
    var searchItem = $('#orgName').val();
    var roleId = $('#configDistrict').val();
    if (roleId === '-- Select District --') {
        roleId = 0;
    }
    params.orgName = searchItem;
    params.configDistrictId = roleId;
    return params;
}

function runningFormatter(value, row, index) {
    var tableOptions = $('#tblFutsalOrg').bootstrapTable('getOptions');
    return ((tableOptions.pageNumber - 1) * tableOptions.pageSize) + (1 + index);
}


function viewInOrgMap() {
    $.ajax({
        type: 'GET',
        url: 'FutsalOrg/ViewInMap',
        success: function (result) {
            $('#orgMapPartialView').html(result);
            $('#orgMapPartialView').modal('show');
        }
    });
}