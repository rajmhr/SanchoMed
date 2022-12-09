
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

function loadMedicines(medOrgId) {
    $.ajax({
        url: "MedicalOrg/LoadMedicine",
        data: { orgMedId: medOrgId },
        type: "GET",
        success: function (data) {
            $('#medicineList').html(data);
        }
    });
}

function loadMedicalAdditionalInfo(medOrgId) {
    $.ajax({
        url: "MedicalOrg/LoadAdditionalMedicalInfo",
        data: { orgId: medOrgId },
        type: "GET",
        success: function (data) {
            $('#medicalAdditionalInfo').html(data);
        }
    });
}

function loadMedicalOrgDoctors(medOrgId) {
    $.ajax({
        url: "MedicalOrg/LoadMedicalDoctor",
        data: AddAntiForgeryToken({ orgId: medOrgId }),
        type: "POST",
        success: function (data) {
            $('#doctorList').html(data);
        }
    });
}

function goToStep(step) {
    stepper.to(step);
}

function saveMedicalInfo() {
    
    $.ajax({
        url: "MedicalOrg/SaveMedicalOrg",
        data: AddAntiForgeryToken({ model: getMediaSaveModel() }),
        type: "POST",
        success: function (data) {
            ShowMessage(data.success, data.message);
            if (data.success) {
                $('#MedicalOrgId').val(data.returnObject.medicalOrgId);
                stepper.next();
                $("#btnNext").blur();
            }
        }
    });
}

function getMediaSaveModel() {
    var data = {
        IsDoctorsAvailable: $('#IsDoctorsAvailable').is(":checked"),
        IsDeliveryAvailable: $('#IsDeliveryAvailable').is(":checked"),
        DeliveryNotes: $('#DeliveryNotes').val(),
        ConfigMedTypeId: $('#ConfigMedTypeId').val(),
        MedicalOrgId: $('#MedicalOrgId').val(),
        OrganizationId: $('#OrganizationId').val(),
        DiscountCode: $('#DiscountCode').val(),
        DiscountPercent: $('#DiscountPercent').val(),
        IsDiscountAvailable: $('#IsDiscountAvailable').is(":checked")
    };
    return data;
}


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




function addUpdateDisease(diseaseId) {
    $.ajax({
        type: 'GET',
        url: 'Disease/AddUpdate',
        data: { diseaseId: diseaseId },
        success: function (result) {
            $('#diseasePartialView').html(result);
            $('#diseasePartialView').modal('show');
        }
    });
}

function deleteDisease(diseaseId) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                type: 'POST',
                url: 'Disease/DeleteDisease',
                data: AddAntiForgeryToken({ dId: diseaseId }),
                success: function (result) {
                    ShowMessage(result.success, result.message);
                    if (result.success) {
                        $("#tblDisease").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
}

function queryDiseaseParams(param) {
    param.search = $('#diseaseTitle').val();
    return param;
}
function dRunningFormatter(value, row, index) {
    var tableOptions = $('#tblDisease').bootstrapTable('getOptions');
    return ((tableOptions.pageNumber - 1) * tableOptions.pageSize) + (1 + index);
}
function searchDisease() {
    $("#tblDisease").bootstrapTable('refresh');
}

function btnShowHide(obj) {
    $(obj).closest('.row').next('.vId').fadeToggle();
}

var isEdit = false;
var editObj;
function editRelatedVideo(rId, obj) {
    isEdit = true;
    editObj = obj;
    $('#VideoTitle').val($(obj).attr('data-title'));
    $('#VideoUrl').val($(obj).attr('data-url'));
    $('#DiseaseRelatedVideo_DiseaseRelatedVideoId').val(rId);
}

function clearVideo() {
    editObj = null;
    isEdit = false;
    $('#VideoTitle').val("");
    $('#VideoUrl').val("");
    $('#DiseaseRelatedVideo_DiseaseRelatedVideoId').val(0);
}

function onSaveDiseaseSuccess(data) {
    ShowMessage(data.success, data.message);
}

function onSaveRelatedVideoSuccess(data) {
    ShowMessage(data.success, data.message);
    console.log(data);

    if (data.success) {
        if (isEdit) {
            $(editObj).closest('.list-group-item').remove();
        }

        var html = `<li class="list-group-item">
                                                <div class="row form-group">
                                                    <label class="col-sm-8">${data.returnObject.videoTitle}</label>
                                                    <div class="col-sm-4 float-right btn-group">
                                                                    <button class="btn btn-sm btn-primary text-small float-right rounded" type="button" onclick="btnShowHide(this)"><i class="fas fa-toggle-on mr-1"></i>View</button>
                                                                    <button class="btn btn-sm btn-danger ml-1 text-small float-right rounded" type="button" onclick="deleteRelatedVideo(${data.returnObject.diseaseRelatedVideoId}, this)"><i class="fas fa-trash mr-1"></i>Remove</button>
                                                                    <button class="btn btn-sm btn-secondary ml-1 text-small float-right rounded" type="button" data-url="${data.returnObject.videoUrl}" data-title="${data.returnObject.videoTitle}" onclick="editRelatedVideo(${data.returnObject.diseaseRelatedVideoId}, this)"><i class="fas fa-edit mr-1"></i> Edit</button>
                                                    </div>
                                                </div>
                                                <div class="form-group vId" style="display:none">
                                                    <div class="embed-responsive embed-responsive-16by9">
                                                        <iframe class="embed-responsive-item" src="${data.returnObject.videoUrl}" allowfullscreen></iframe>
                                                    </div>
                                                </div>
                                            </li>`;
        $('.rVideos').append(html);
        clearVideo();
    }
}

function deleteRelatedVideo(rId, obj) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: 'Disease/DeleteRelatedVideo',
                data: AddAntiForgeryToken({ rId: rId }),
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success) {
                        $(obj).closest('.list-group-item').remove();
                    }
                }
            });
        }
    });
}

function addUpdateNutrition(nutritionId) {
    $.ajax({
        type: 'GET',
        url: 'Nutrition/AddUpdate',
        data: { nutId: nutritionId },
        success: function (result) {
            $('#nutritionPartialView').html(result);
            $('#nutritionPartialView').modal('show');
        }
    });
}

function deleteDisease(nutrition) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                type: 'POST',
                url: 'Disease/DeleteNutrition',
                data: AddAntiForgeryToken({ dId: nutrition }),
                success: function (result) {
                    ShowMessage(result.success, result.message);
                    if (result.success) {
                        $("#tblNutrition").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
}
function fileNutrition(input) {
    readURL(input);
}
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#nutImage').html(`
                <img src="${e.target.result}" width="100" height="100" alt="/images/no_image.png/" />
                `);
        };

        $('#fileName').text(input.files[0].name);
        reader.readAsDataURL(input.files[0]);
    }
}
function nutRowStyle(row, index) {
    if (!row.isActive) {
        return {
            classes: 'alert-danger'
        };
    }
    return {
        classes: 'text-grey-900'
    };
}

function queryNutritions(param) {
    param.title = $('#nutritionTitle').val();
    return param;
}
function dRunningFormatter(value, row, index) {
    var tableOptions = $('#tblNutrition').bootstrapTable('getOptions');
    return ((tableOptions.pageNumber - 1) * tableOptions.pageSize) + (1 + index);
}
function searchNutrition() {
    $("#tblNutrition").bootstrapTable('refresh');
}

function btnShowHide(obj) {
    $(obj).closest('.row').next('.vId').fadeToggle();
}


function onSaveDiseaseSuccess(data) {
    ShowMessage(data.success, data.message);
}

function deleteNutrition(nutId) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: 'Nutrition/DeleteNutrition',
                data: AddAntiForgeryToken({ nutId: nutId }),
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success) {
                        $("#tblNutrition").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
}
function saveNutrition() {
    var input = $('#imgNutrition');

    var files = input[0].files;
    var formData = new FormData();

    for (var i = 0; i !== files.length; i++) {
        formData.append("files", files[i]);
    }

    formData.append("NutritionId", $('#NutritionId').val());
    formData.append("Title", $('#Title').val());
    formData.append("ImageUrl", $('#ImageUrl').val());
    formData.append("ConfigFoodTypeId", $('#ConfigFoodTypeId').val());
    formData.append("IsActive", $('#IsActive').is(":checked"));
    formData.append("Nutritions", $('#nutDetail').summernote('code'));
    formData.append("Description", $('#description').summernote('code'));
    formData.append("__RequestVerificationToken", $('input[name=__RequestVerificationToken]').val());

    $.ajax({
        url: "Nutrition/SaveNutrition",
        data: formData,
        processData: false,
        contentType: false,
        type: "POST",
        success: function (data) {
            ShowMessage(data.success, data.message);
            if (data.success) {
                $('#nutritionPartialView').modal('hide');
                $("#tblNutrition").bootstrapTable('refresh');
            }
        }
    });
}



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