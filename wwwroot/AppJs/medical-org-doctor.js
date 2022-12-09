

function mapDoctorToOrg(mDocId) {
    var medOrgId = $('#MedicalOrgId').val();
    $.ajax({
        url: 'MedicalDoctor/MapDoctor',
        data: AddAntiForgeryToken({ mDocId: mDocId, medOrgId: medOrgId }),
        type: 'POST',
        success: function (data) {
            ShowMessage(data.success, data.message);
            if (data.success) {
                $("#tblMappedDoctor").bootstrapTable('refresh');

            }
        }
    });
}

function removeMappedDoctor(mDocId) {
    $.ajax({
        url: 'MedicalDoctor/UnMapDoctor',
        data: AddAntiForgeryToken({ orgMDocId: mDocId }),
        type: 'POST',
        success: function (data) {
            ShowMessage(data.success, data.message);
            if (data.success) {
                $("#tblMappedDoctor").bootstrapTable('refresh');
            }
        }
    });
}
function updateSchedule(mDocId) {
    $.ajax({
        url: 'MedicalDoctor/UpdateSchedule',
        data: AddAntiForgeryToken({ medOrgDocId: mDocId }),
        type: 'POST',
        success: function (data) {
            $('#schedulePartialView').html(data);
            $("#schedulePartialView").show();
        }
    });
}
function initMappedDoctor() {
    var $tableMapped = $('#tblMappedDoctor');
    $("#tblMappedDoctor").bootstrapTable({

        beforeContextMenuRow: function (e, row, buttonElement) {
            $tableMapped.bootstrapTable('uncheckAll');
            $tableMapped.bootstrapTable('checkBy', { field: "orgMedicalDoctorId", values: [row.orgMedicalDoctorId] });
            $tableMapped.bootstrapTable('showContextMenu', { event: e, contextMenu: '#context-menu-doctors-mapped' });
        },
        onContextMenuItem: function (row, el) {
            var command = $(el[0]).attr('data-val');
            if (command === "Remove") {
                removeMappedDoctor(row.orgMedicalDoctorId);
            } else if (command === "Schedule") {
                updateSchedule(row.orgMedicalDoctorId);
            }
        }
    });

    $tableMapped.bootstrapTable('scrollTo', top);
    $('#tblMappedDoctor thead tr').css('background', '#24BBB8');
    $('#tblMappedDoctor thead tr').css('color', '#111111');
}

function initAvailableDoctor() {
    var $tableAvailable = $('#tblAvailDoctor');
    $("#tblAvailDoctor").bootstrapTable({

        beforeContextMenuRow: function (e, row, buttonElement) {
            $tableAvailable.bootstrapTable('uncheckAll');;
            $tableAvailable.bootstrapTable('checkBy', { field: "medicalDoctorId", values: [row.medicalDoctorId] });
            $tableAvailable.bootstrapTable('showContextMenu', { event: e, contextMenu: '#context-menu-doctors-available' });
        },
        onContextMenuItem: function (row, el) {
            var command = $(el[0]).attr('data-val');
            if (command === "Map") {
                mapDoctorToOrg(row.medicalDoctorId);
            }
        }
    });

    $tableAvailable.bootstrapTable('scrollTo', top);
    $('#tblAvailDoctor thead tr').css('background', '#24BBB8');
    $('#tblAvailDoctor thead tr').css('color', '#111111');
}
function searchAvailDoctor() {
    $("#tblAvailDoctor").bootstrapTable('refresh');
}

function searchMappedDoctor() {
    $("#tblMappedDoctor").bootstrapTable('refresh');
}


function queryAvailableDoctorParams(params) {
    var searchItem = $('#availDocName').val();
    params.docName = searchItem;
    return params;
}



