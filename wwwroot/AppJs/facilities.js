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


