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
