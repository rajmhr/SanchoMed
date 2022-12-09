
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
function loadFacilities(orgId) {
    $.ajax({
        url: "Facilities/GetFacilitiesView",
        data: { Organizationid: orgId },
        type: "GET",
        success: function (data) {
            $('#facilities').html(data);
        }
    });
}


function loadSchedule(orgId) {
    $.ajax({
        url: "OrgTimeSchedule/GetOrgTimeSchedule",
        data: AddAntiForgeryToken({ orgId: orgId }),
        type: "POST",
        success: function (data) {
            $('#schedule').html(data);
        }
    });
}

function loadBasicInfo(orgId) {
    $.ajax({
        url: "FutsalOrg/LoadBasicInfo",
        data: AddAntiForgeryToken({ orgId: orgId }),
        type: "POST",
        success: function (data) {
            $('#futsalBasicInfo').html(data);
        }
    });
}

function loadFutsalImages(orgId) {
    $.ajax({
        url: "FutsalOrg/LoadFutsalImages",
        data: AddAntiForgeryToken({ orgId: orgId }),
        type: "POST",
        success: function (data) {
            $('#futsalImages').html(data);
        }
    });
}

function goToStep(step) {
    stepper.to(step);
}


function saveBasicInfo() {
    $.ajax({
        url: "FutsalOrg/SaveFutsalOrg",
        data: AddAntiForgeryToken({ model: getBasicModel() }),
        type: "POST",
        success: function (data) {
            if (data.success) {
                $('#OrganizationId').val(data.returnObject.organizationId);
                stepper.next();
            }
            else {
                ShowMessage(data.success, data.message);
            }
        }
    });
}

function getBasicModel() {
    var data = {
        OrganizationId: $('#OrganizationId').val(),
        Description: $('#Description').val(),
        Address: $('#Address').val(),
        IsActive: $('#IsActive').is(":checked"),
        Title: $('#Title').val(),
        Pricing: $('#Pricing').val(),
        LatLocation: $('#LatLocation').val(),
        LonLocation: $('#LonLocation').val(),
        UserNote: $('#UserNote').val(),
        FacebookLink: $('#FacebookLink').val(),
        ConfigDistrictId: $('#ConfigDistrictId').val(),
        WebSite: $('#WebSite').val(),
        OfficePhone: $('#OfficePhone').val(),
        MobileNo: $('#MobileNo').val(),
        ConfigAppTypeId: $('#ConfigAppTypeId').val(),
    }
    return data;
}

function fileLoaded(input) {
    readURL(input);
}

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('.noimage').remove();
            $('.carousel-inner').append(`<div class="carousel-item active">
                                            <img class="d-block w-100 imgSize" src=${e.target.result} alt="New Image">
                                           </div>`);
        };
        reader.readAsDataURL(input.files[0]);
    }
}
function uploadImage() {
    var input = $('#imgFutsal');
    if ($(input).val() === "") {
        ShowMessage(false, "Please select image to upload.");
        return;
    }
    var files = input[0].files;
    var formData = new FormData();

    for (var i = 0; i !== files.length; i++) {
        formData.append("files", files[i]);
    }

    formData.append("OrganizationId", $('#OrganizationId').val());
    formData.append("__RequestVerificationToken", $('input[name=__RequestVerificationToken]').val());

    $.ajax({
        url: "FutsalOrg/UploadImage",
        data: formData,
        processData: false,
        contentType: false,
        type: "POST",
        success: function (data) {
            ShowMessage(data.success, data.message);
        }
    });
}
function onStartFacilities() {
    $.validator.unobtrusive.parse("#frmFacilities");
}

function onSaveFacilities(data) {
    ShowMessage(data.success, data.message);

}
function saveFacilities() {
    $.ajax({
        url: 'Facilities/SaveFacilities',
        data: AddAntiForgeryToken({ model: getFacilityModel() }),
        type: 'POST',
        success: function (data) {
            ShowMessage(data.success, data.message);
            if (data.success) {
                $('.availablefacilitiesList').append(`<ul class="list-group list-group-flush  col-xl-6 mt-2">

                            <li class="list-group-item list-group-item-info rounded-pill d-flex justify-content-between">
                                <span class="badge badge-danger badge-pill text-center" onclick="deleteFacilities(${data.returnObject.facilitiesId}, this)"> 
                                    <i class="fas fa-trash "></i>
                                </span>
                                ${data.returnObject.description}
                                <span class="badge badge-success badge-pill" onclick="addToOrgFacilities(${data.returnObject.facilitiesId}, '${data.returnObject.description}')">
                                    <i class="fas fa-plus"></i>
                                </span>
                            </li>

                        </ul>`);
            }
        }
    });
}
function getFacilityModel() {
    var data = {
        OrganizationId: $('#OrganizationId').val(),
        Description: $('#inputDescription').val()
    };
    return data;
}

function deleteFacilities(facilitiesId, obj) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: 'Facilities/DeleteFacilities',
                data: AddAntiForgeryToken({ fId: facilitiesId }),
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    $(obj).closest('ul').remove();
                }
            });
        }
    });
}

function addToOrgFacilities(facilityId, title) {
    var orgId = $('#OrganizationId').val();

    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: 'Facilities/AddToOrgFacilities',
                data: AddAntiForgeryToken({ facilityId: facilityId, orgId: orgId, title: title }),
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success) {
                        $('.mappedFacilitiesList').append(`<ul class="list-group list-group-flush  col-xl-6 mt-2">

                        <li class="list-group-item list-group-item-info rounded-pill d-flex justify-content-between">
                           ${data.returnObject.description}
                            <span class="badge badge-danger badge-pill text-center" onclick="unlinkFacilities(${data.returnObject.orgFacilitiesId}, this)">
                                <i class="fas fa-trash "></i>
                            </span>

                        </li>

                    </ul>`);
                    }
                }
            });
        }
    });
}

function unlinkFacilities(orgFacilityId, obj) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: 'Facilities/RemoveFromOrgFacilities',
                data: AddAntiForgeryToken({ orgFacilityId: orgFacilityId }),
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success)
                        $(obj).closest('ul').remove();
                }
            });
        }
    });
}



function getAvailableFacilities(data) {
    $.ajax({
        url: "Facilities/GetAvailableFacilities",
        type: "POST",
        data: AddAntiForgeryToken({ search: data }),
        success: function (data) {
            $('#availableFacilities').html(data);
        }
    });
}


function getOrgFacilities() {
    $.ajax({
        url: "Facilities/GetMappedFacilities",
        type: "POST",
        data: AddAntiForgeryToken({ orgId: $('#OrganizationId').val() }),
        success: function (data) {
            $('#mappedFacilities').html(data);
        }
    });
}

function addUpdateFacilities(facilitesId) {
    $.ajax({
        url: "Facilities/AddUpdate",
        type: "POST",
        data: AddAntiForgeryToken({ facilitesId: facilitesId }),
        success: function (data) {
            $('#addupdateFacilitesPartialView').html(data);
            $('#addupdateFacilitesPartialView').show();
        }
    });
}


function searchFacilites(search) {
    search = $('#facilitesSearch').val();
    getAvailableFacilities(search);
}


