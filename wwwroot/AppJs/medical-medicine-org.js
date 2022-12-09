function queryMedicalDoctorParams(params) {
    var searchItem = $('#docName').val();
    params.docName = searchItem;
    return params;
}

function loadMedicineFromFile() {
    var input = $('#medicineFile');
    if ($(input).val() === "") {
        ShowMessage(false, "Please select medicine file to load data.");
        return;
    }
    var files = input[0].files;

    var formData = new FormData();

    for (var i = 0; i !== files.length; i++) {
        formData.append("files", files[i]);
    }

    formData.append("medOrgId", $('#MedicalOrgId').val());

    $.ajax({
        url: "Medicine/GetMedicineDataFromFile",
        data: formData,
        processData: false,
        contentType: false,
        type: "POST",
        success: function (data) {

            if (data.rows) {
                $tblLoadMedicine.bootstrapTable('load', data);
            }
            else {
                ShowMessage(data.success, data.message);
            }
        }
    });
}

function saveMedicine() {
    var input = $('#medicineFile');
    if ($(input).val() === "") {
        ShowMessage(false, "Please select medicine file to load data.");
        return;
    }
    //need medicalorg id
    var files = input[0].files;

    var formData = new FormData();

    for (var i = 0; i !== files.length; i++) {
        formData.append("files", files[i]);
    }
    formData.append("medOrgId", $('#MedicalOrgId').val());

    $.ajax({
        url: "MedicalOrg/SaveRelatedMedicine",
        data: formData,
        processData: false,
        contentType: false,
        type: "POST",
        success: function (data) {
            if (data.rows) {
                $('#tblLoadMedicine').bootstrapTable('load', data);
            }
            else {
                ShowMessage(data.success, data.message);
            }
        }
    });
}

function rowStyle(row, index) {

    if (row.validationType === "Duplicates") {
        return {
            classes: 'alert-danger'
        };
    }
    else if (row.validationType === "BrandNewMedicine" || row.validationType === "NewMedicine") {
        return {
            classes: 'alert-success'
        };
    }
    else if (row.validationType === "OverlappedWithSystem") {
        return {
            classes: 'text-success'
        };
    }
    else if (row.validationType === "RemovingMedicine") {
        return {
            classes: 'text-danger'
        };
    }
    return {
        classes: 'alert-danger'
    };
}

function queryMedicines(params) {
    var medOrgId = $('#MedicalOrgId').val();
    params.medName = $('#medAvailName').val();
    params.medOrgId = medOrgId;
    return params;
}
function initLoadMedicine() {
    $tblLoadMedicine = $('#tblLoadMedicine');
    $("#tblLoadMedicine").bootstrapTable({

        beforeContextMenuRow: function (e, row, buttonElement) {
            $tblLoadMedicine.bootstrapTable('uncheckAll');
            $tblLoadMedicine.bootstrapTable('checkBy', { field: "medicalDoctorId", values: [row.medicalDoctorId] });
            $tblLoadMedicine.bootstrapTable('showContextMenu', { event: e, contextMenu: '#context-menu-admin' });
        },
        onContextMenuItem: function (row, el) {
            var command = $(el[0]).attr('data-val');
            if (command === "Delete") {
                deleteMedicalDoctor(row.organizationId);
            }
            else if (command === "Edit") {
                addUpdateMedicalDoctor(row.medicalDoctorId);
            }
            else if (command === "Detail") {
                viewMedicalDoctorDetails(row.organizationId);
            }
        }
    });

    $("#tblLoadMedicine").bootstrapTable('scrollTo', top);
    $('#tblLoadMedicine thead tr').css('background', '#24BBB8');
    $('#tblLoadMedicine thead tr').css('color', '#111111');
}
function initAvailableMedicine() {
    var $tblAvailableMedicine = $('#tblAvailableMedicine');
    $("#tblAvailableMedicine").bootstrapTable({

        beforeContextMenuRow: function (e, row, buttonElement) {
            $tblAvailableMedicine.bootstrapTable('uncheckAll');
            $tblAvailableMedicine.bootstrapTable('checkBy', { field: "drugOrgDetailId", values: [row.drugOrgDetailId] });
            $tblAvailableMedicine.bootstrapTable('showContextMenu', { event: e, contextMenu: '#context-menu-edit-delete' });
        },
        onContextMenuItem: function (row, el) {
            var command = $(el[0]).attr('data-val');
            if (command === "Delete") {
                deleteMedicineOrg(row.drugOrgDetailId);
            }
            else if (command === "Edit") {
                addMedicineToOrg(row.drugOrgDetailId);
            }
        }
    });

    $("#tblAvailableMedicine").bootstrapTable('scrollTo', top);
    $('#tblAvailableMedicine thead tr').css('background', '#24BBB8');
    $('#tblAvailableMedicine thead tr').css('color', '#111111');
}

function addMedicineToOrg(medDetailId) {
    $.ajax({
        url: 'MedicalOrg/AddMedicineToOrg',
        data: { medDetailId: medDetailId },
        type: 'GET',
        success: function (data) {
            $('#addMedPartialView').html(data);
            $('#addMedPartialView').modal('show');
        }
    });
}

function deleteMedicineOrg(medOrgDetailId) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: 'Medicine/DeleteMedicineOrg',
                data: AddAntiForgeryToken({ medOrgDetailId: medOrgDetailId }),
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    $("#tblAvailableMedicine").bootstrapTable('refresh');
                }
            });
        }
    });
}


function onStartAddMedicine() {
    $.validator.unobtrusive.parse("#frmAddMedicine");

}


function formatRepoSelection(item) {
    if (item.display === undefined)
        return item.text;
    return item.display;
}

function closeModal() {
    $('#addMedPartialView').modal('hide');
}

function loadDrugSearch() {
    var drugUrl = $('#medSelect').data('url');

    $('#medSelect').select2({
        minimumInputLength: 2,
        multiple: false,
        placeholder: "Please enter drug title..",
        allowClear: true,
        dataType: 'json',
        ajax: {
            url: drugUrl,
            global: false,
            data: function (params) {
                var query = { medName: params.term };
                return query;
            },
            processResults: function (data) {
                var jData = JSON.parse(data.returnData);
                return {
                    results: $.map(jData, function (item) {
                        return {
                            text: `<div class="text-center text-gray-900">
                                                ${item.MedBrandName} - (${item.MedGeneralName})
                                                  <div class="text-small text-success font-weight-bold"">
                                                    ${item.DoseForm} - ( ${item.Composition})
                                                 </div>
                                          </div>`,
                            id: item.DrugBrandId,
                            display: `<div class="text-center text-gray-900 text-primary">
                                            ${item.MedBrandName} (${item.MedGeneralName} )
                                            </div>`,
                            medGeneralName: item.MedGeneralName,
                            medBrandName: item.MedBrandName,
                            doseForm: item.DoseForm,
                            composition: item.Composition
                        };
                    })
                };
            }
        },
        escapeMarkup: function (markup) { return markup; },
        templateSelection: formatRepoSelection
    });
}

function setDrugOrg(jdata) {
    var s2 = $("#medSelect").data('select2');
    console.log(jdata);
    if (jdata.drugOrgDetailId !== 0) {
        s2.trigger('select', {
            data:
            {
                text: `<div class="text-center text-gray-900 text-primary">
                                            ${jdata.medBrandName} (${jdata.medGeneralName} )
                                            </div>`,
                id: jdata.drugBrandId
            }
        });
    }
}

function saveMedicineManual() {
    $.ajax({
        url: "Medicine/AddMedicineOrg",
        data: AddAntiForgeryToken({ model: getMedModel() }),
        type: "POST",
        success: function (data) {
            ShowMessage(data.success, data.message);
            if (data.success) {
                closeModal();
                $("#tblAvailableMedicine").bootstrapTable('refresh');
            }
        }
    });
}

function getMedModel() {
    var data = {
        MedicineDetailId: $('#MedicineDetailId').val(),
        MedicalOrgId: $('#MedicalOrgId').val(),
        MedGeneralName: $('#MedGeneralName').val(),
        MedBrandName: $('#MedBrandName').val(),
        Composition: $('#Composition').val(),
        DoseForm: $('#DoseForm').val(),
        DrugBrandId: $('#DrugBrandId').val(),
        DrugOrgDetailId: $('#DrugOrgDetailId').val()
    };
    console.log(data);
    return data;
}

function searchAvailableMedicine() {
    $("#tblAvailableMedicine").bootstrapTable('refresh');
}

function runningFormatter(value, row, index) {
    var tableOptions = $('#tblAvailableMedicine').bootstrapTable('getOptions');
    return ((tableOptions.pageNumber - 1) * tableOptions.pageSize) + (1 + index);
}