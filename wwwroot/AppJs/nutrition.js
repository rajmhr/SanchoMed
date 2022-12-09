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

