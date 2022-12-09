
function onStartMedicines() {
    $.validator.unobtrusive.parse("#frmMedicine");
}


function onStartGenerics() {
    $.validator.unobtrusive.parse("#frmGeneric");
}

function onSaveDoctorSuccess(data) {
    ShowMessage(data.success, data.message);
    if (data.success) {
        $('#medicinePartialView').modal('hide');
        $("#tblMedicine").bootstrapTable('refresh');
    }
}
function onSaveGenericSuccess(data) {
    ShowMessage(data.success, data.message);
    if (data.success) {
        $('#genericPartialView').modal('hide');
        $("#tblGeneric").bootstrapTable('refresh');
    }
}

function openMedicineModal(medDetailId) {
    $.ajax({
        type: 'GET',
        url: 'Medicine/AddUpdate',
        data: { medId: medDetailId },
        success: function (result) {
            $('#medicinePartialView').html(result);
            $('#medicinePartialView').modal('show');
        }
    });
}

function openGenericModal(genericId) {
    $.ajax({
        type: 'GET',
        url: '/Medicine/AddUpdateGeneric',
        data: { genericId: genericId },
        success: function (result) {
            $('#genericPartialView').html(result);
            $('#genericPartialView').modal('show');
        }
    });
}


function addUpdateMedicine(medDetailId) {
    openMedicineModal(medDetailId);
}

function addUpdateGeneric(genericId) {
    openGenericModal(genericId);
}

function deleteMedicine(medId) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: 'Medicine/DeleteMedicine',
                data: AddAntiForgeryToken({ medId: medId }),
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success) {
                        $("#tblMedicine").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
}

function deleteGeneric(genericId) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: '/Medicine/DeleteGeneric',
                data: AddAntiForgeryToken({ genericId: genericId }),
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success) {
                        $("#tblGeneric").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
}

function searchMedicine() {
    $("#tblMedicine").bootstrapTable('refresh');
}

function searchGeneric() {
    $("#tblGeneric").bootstrapTable('refresh');
}

function queryMedicineParams(params) {
    var searchItem = $('#medName').val();
    params.medName = searchItem;
    return params;
}
function queryGenericParams(params) {
    var searchItem = $('#genericName').val();
    params.medName = searchItem;
    return params;
}

function runningFormatter(value, row, index) {
    var tableOptions = $('#tblMedicine').bootstrapTable('getOptions');
    return ((tableOptions.pageNumber - 1) * tableOptions.pageSize) + (1 + index);
}

function runningFormatterGeneric(value, row, index) {
    var tableOptions = $('#tblGeneric').bootstrapTable('getOptions');
    return ((tableOptions.pageNumber - 1) * tableOptions.pageSize) + (1 + index);
}


function loadDrugGeneric() {
    var medOrgUrl = $('#genericSelect').data('url');

    $('#genericSelect').select2({
        minimumInputLength: 3,
        multiple: false,
        placeholder: "Please type drug generic..",
        dropdownCssClass: 'bigdrop',
        allowClear: true,
        dataType: 'json',
        ajax: {
            url: medOrgUrl,
            global: false,
            data: function (params) {
                var query = { generics: params.term };
                return query;
            },
            processResults: function (data) {
                console.log(data);
                return {
                    results: $.map(data.returnObject, function (item) {
                        return {
                            text: `<div class="text-center text-gray-900 text-small">${item.genericName}</div>`,
                            id: item.drugGenericId
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

function setDrugGeneric(jdata) {
    var s2 = $("#genericSelect").data('select2');
    console.log(jdata);
    if (jdata.drugBrandId !== 0) {
        s2.trigger('select', {
            data:
            {
                text: `${jdata.medGeneralName}`,
                id: jdata.drugGenericId
            }
        });
    }
}
