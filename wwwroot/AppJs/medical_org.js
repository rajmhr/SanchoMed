
function onRegisterBegin() {
    $.validator.unobtrusive.parse("#frmMedicalOrg");
}

function openMedicalOrgModal(orgId, medOrgId, title) {
    $.ajax({
        type: 'GET',
        url: 'MedicalOrg/AddUpdate',
        data: { orgId: orgId, medOrgId: medOrgId, title: title },
        success: function (result) {
            $('#medicalOrgPartialView').html(result);
            $('#medicalOrgPartialView').modal('show');
        }
    });
}


function addUpdateMedicalOrg(orgId, medOrgId ,title) {
    openMedicalOrgModal(orgId, medOrgId, title);
}

function deleteMedicalOrg(orgId) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: 'MedicalOrg/Delete',
                data: { orgId: orgId },
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success) {
                        $("#tblMedicalOrg").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
}

function searchMedicalOrg() {
    $("#tblMedicalOrg").bootstrapTable('refresh', {
            query: { limit: 10, offset: 0 }
    });
}

function queryMedicalOrgParams(params) {
    var searchItem = $('#orgName').val();
    var districtId = $('#configDistrict').val();
    if (districtId === '-- Select District --') {
        districtId = 0;
    }
    params.orgName = searchItem;
    params.districtId = districtId;
    return params;
}

function medRunningFormatter(value, row, index) {
    var tableOptions = $('#tblMedicalOrg').bootstrapTable('getOptions');
    return ((tableOptions.pageNumber - 1) * tableOptions.pageSize) + (1 + index);
}
