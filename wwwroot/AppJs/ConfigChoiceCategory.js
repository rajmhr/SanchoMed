function onConfigCategoryBegin() {
    $.validator.unobtrusive.parse("#frmAddEditCategory");
}

function openCategoryModal(categoryId) {
    $.ajax({
        type: 'GET',
        url: 'ConfigChoiceCategory/AddUpdate',
        data: { configChoiceCategoryId: categoryId },
        success: function (result) {
            $('#categoryPartialView').html(result);
            $('#categoryPartialView').modal('show');
        }
    });
}

function saveConfigCategory() {
    $('#frmAddEditCategory').submit();
}

function addUpdateConfigCategory(categoryId) {
    openCategoryModal(categoryId);
}

function deleteCategory(categoryId) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                url: 'ConfigChoiceCategory/DeleteConfigChoiceCategory',
                data: AddAntiForgeryToken({ configChoiceCategoryId: categoryId }),
                type: 'POST',
                success: function (data) {
                    ShowMessage(data.success, data.message);
                    if (data.success) {
                        $("#tblConfigCategory").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
}

function tableActions(value, row) {
    return [
        '<a class="like" href="javascript:void(0)" title="Edit">',
        '<i class="fas fa-fw fa-edit" onclick="UpdateCategoryAction(' + row.configChoiceCategoryId + ')"  ></i>',
        '</a> ',
        '<a class="danger remove" href="javascript:void(0)" title="Remove">',
        '<i class="fas fa-trash" onclick="DeleteCategoryAction(' + row.configChoiceCategoryId + ')"></i>',
        '</a>'
    ].join('');
}

function onSaveCategorySuccess(data) {
    ShowMessage(data.success, data.message);
    if (data.success) {
        $('#categoryPartialView').modal('hide');
        $("#tblConfigCategory").bootstrapTable('refresh');
    }
}