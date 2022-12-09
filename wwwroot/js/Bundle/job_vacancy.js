
function onStartJobVacancy() {
    $.validator.unobtrusive.parse("#frmJobVacancy");
}

function onSaveJobVacancy(data) {
    ShowMessage(data.success, data.message);
    if (data.success) {
        $('#jobVacancyPartialView').modal('hide');
        $("#tblJobVacancy").bootstrapTable('refresh');
    }
}

function addUpdateJobVacancy(jobId) {
    $.ajax({
        type: 'GET',
        url: 'JobVacancy/AddUpdate',
        data: AddAntiForgeryToken({ jobId: jobId}),
        success: function (result) {
            $('#jobVacancyPartialView').html(result);
            $('#jobVacancyPartialView').modal('show');
        }
    });
}


function deleteJobVacancy(jobId) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: 'JobVacancy/DeleteJobVacancy',
                data: AddAntiForgeryToken({ jobId: jobId }),
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success) {
                        $("#tblJobVacancy").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
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

function queryJobVacancyParam(params) {
    params.startDate = $('#StartDate').val();
    params.endDate = $('#EndDate').val();
    params.isActiveOnly = $('#isActiveOnly').val();
    return params;
}

function runningFormatter(value, row, index) {
    var tableOptions = $('#tblJobVacancy').bootstrapTable('getOptions');
    return ((tableOptions.pageNumber - 1) * tableOptions.pageSize) + (1 + index);
}

function setMedicalOrg(jdata) {
    var s2 = $("#medOrgSelect").data('select2');
    console.log(jdata);
    if (jdata) {
        s2.trigger('select', {
            data:
            {
                text: `${jdata.medicalName} -- ${jdata.address}`,
                id: jdata.jobVacancyId
            }
        });
    }
}

function searchJobVacancy() {
    $("#tblJobVacancy").bootstrapTable('refresh', {
        query: { limit: 10, offset: 0 }
    });
}