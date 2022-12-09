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