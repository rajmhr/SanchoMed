
function onStartMedicalDoctors() {
    $.validator.unobtrusive.parse("#frmMedicalDoctor");
}

function onSaveDoctorSuccess(data) {
    ShowMessage(data.success, data.message);
    if (data.success) {
        $('#medicalDoctorPartialView').modal('hide');
        $("#tblMedicalDoctor").bootstrapTable('refresh');
    }
}

function openMedicalDoctorModal(medDoctorId) {
    $.ajax({
        type: 'GET',
        url: 'MedicalDoctor/AddUpdate',
        data: { medDoctorId: medDoctorId },
        success: function (result) {
            $('#medicalDoctorPartialView').html(result);
            $('#medicalDoctorPartialView').modal('show');
        }
    });
}


function addUpdateMedicalDoctor(medDoctorId) {
    openMedicalDoctorModal(medDoctorId);
}

function deleteMedicalDoctor(mDocId) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: 'MedicalDoctor/DeleteMedicalDoctor',
                data: { docId: mDocId },
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success) {
                        $("#tblMedicalDoctor").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
}

function searchMedicalDoctor() {
    $("#tblMedicalDoctor").bootstrapTable('refresh');
}

function queryMedicalDoctorParams(params) {
    var searchItem = $('#docName').val();
    var roleId = $('#configDistrict').val();
    if (roleId === '-- Select District --') {
        roleId = 0;
    }
    params.docName = searchItem;
    params.configDistrictId = roleId;
    return params;
}

function runningFormatter(value, row, index) {
    var tableOptions = $('#tblMedicalDoctor').bootstrapTable('getOptions');
    return ((tableOptions.pageNumber - 1) * tableOptions.pageSize) + (1 + index);
}


function loadDocSearch() {
    var bookSerachUrl = $('#docSpeSelect').data('url');

    $('#docSpeSelect').select2({
        minimumInputLength: 2,
        multiple: true,
        placeholder: "Please type specialization.",
        dropdownCssClass: 'bigdrop',
        allowClear: true,
        dataType: 'json',
        ajax: {
            url: bookSerachUrl,
            global: false,
            data: function (params) {
                var query = { query: params.term };
                return query;
            },
            processResults: function (data) {
                var jData = JSON.parse(data.returnData);
                return {
                    results: $.map(jData, function (item) {
                        return {
                            text: `<div class="text-center text-gray-900">${item.ChoiceCodeDesc}</div>`,
                            id: item.ConfigChoiceId
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

function setSpecialist(jdata) {
    var s2 = $("#docSpeSelect").data('select2');
    if (jdata) {
        $.each(jdata, function (index, item) {
            console.log(item);
            s2.trigger('select', {
                data:
                {
                    text: item.choiceCodeDesc,
                    id: item.configChoiceId
                }
            });
        });
    }
}
