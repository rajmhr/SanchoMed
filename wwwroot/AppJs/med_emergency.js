
function addUpdateEmergency(emergencyId) {
    $.ajax({
        type: 'GET',
        url: 'MedicalEmergency/AddUpdate',
        data: { emergencyId: emergencyId },
        success: function (result) {
            $('#emergencyPartialView').html(result);
            $('#emergencyPartialView').modal('show');
        }
    });
}

function completeNotified() {
    confirmDialog("Are you sure to run the secret job ?", (ans) => {
        if (ans) {
            $.ajax({
                type: 'POST',
                url: 'MedicalEmergency/CompleteExpiredRequests',
                data: AddAntiForgeryToken({}),
                success: function (result) {
                    ShowMessage(result.success, result.message)
                }
            });
        }
    })
}

function viewInMap(lat, lon) {
    $.ajax({
        type: 'POST',
        url: 'MedicalEmergency/ViewInMap',
        data: AddAntiForgeryToken({ lat: lat, lon: lon}),
        success: function (result) {
            $('#mapPartialView').html(result);
            $('#mapPartialView').modal('show');
        }
    });
} function removeLocation() {
    $('#Lattitude').val(0);
    $('#Longitude').val(0);
    $('#Location').val(0.0 + " -- " + 0.0);

}

function processNotify(row) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                type: 'POST',
                url: 'MedicalEmergency/DeliverMsg',
                data: AddAntiForgeryToken({ model: row }),
                success: function (result) {
                    ShowMessage(result.success, result.message);
                    if (result.success)
                        $("#tblEmergency").bootstrapTable('refresh');
                }
            });
        }
    });
}

function onSaveEmergencySuccess(data) {
    ShowMessage(data.success, data.message);
    if (data.success) {
        $('#emergencyPartialView').modal('hide');
        $("#tblEmergency").bootstrapTable('refresh');
    }
}

function eRowStyle(row, index) {
    if (row.emergencyMsgStatus === "Pending") {
        return {
            css: {
                color: 'blue'
            }
        };
    }
    else if (row.emergencyMsgStatus === "Approved") {
        return {
            css: {
                color: 'green'
            }
        };
    }

    else if (row.emergencyMsgStatus === "Notified") {
        return {
            classes: 'alert-success'
        };
    }
    else if (row.emergencyMsgStatus === "Rejected") {
        return {
            css: {
                color: 'red'
            }
        };
    }
    else if (row.emergencyMsgStatus === "Terminate") {
        return {
            classes: 'alert-danger'
        };
    }
    else if (row.emergencyMsgStatus === "Completed") {
        return {
            classes: 'alert-primary'
        };
    }
    return {
        classes: 'text-grey-900'
    };
}

function queryEmergency(param) {
    param.title = $('#emergencyTitle').val();
    param.emergencyDeliveryTypeId = $('#deliveryTypeId').val();
    param.emergencyTypeId = $('#requestTypeId').val();
    return param;
}
function dRunningFormatter(value, row, index) {
    var tableOptions = $('#tblEmergency').bootstrapTable('getOptions');
    return ((tableOptions.pageNumber - 1) * tableOptions.pageSize) + (1 + index);
}
function searchEmergency() {
    $("#tblEmergency").bootstrapTable('refresh');
}

function btnShowHide(obj) {
    $(obj).closest('.row').next('.vId').fadeToggle();
}


function onSaveDiseaseSuccess(data) {
    ShowMessage(data.success, data.message);
}

function deleteEmergency(nutId) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: 'MedicalEmergency/DeleteMedicalEmergency',
                data: AddAntiForgeryToken({ nutId: nutId }),
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success) {
                        $("#tblEmergency").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
}

