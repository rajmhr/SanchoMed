function addUpdateDiseaseAI(DiseaseAIId) {
    $.ajax({
        type: 'GET',
        url: 'DiseaseAI/AddUpdate',
        data: { DiseaseAIId: DiseaseAIId },
        success: function (result) {
            $('#diseaseAIPartialView').html(result);
            $('#diseaseAIPartialView').modal('show');
        }
    });
}

function deleteDiseaseAI(DiseaseAIId) {
    confirmDialog("Are you sure?", (ans) => {
        if (ans) {
            $.ajax({
                type: 'POST',
                url: 'DiseaseAI/DeleteDisease',
                data: AddAntiForgeryToken({ dId: DiseaseAIId }),
                success: function (result) {
                    ShowMessage(result.success, result.message);
                    if (result.success) {
                        $("#tblDiseaseAI").bootstrapTable('refresh');
                    }
                }
            });
        }
    });
}

function queryDiseaseAIParams(param) {
    param.search = $('#diseaseAITitle').val();
    return param;
}
function dRunningFormatter(value, row, index) {
    var tableOptions = $('#tblDiseaseAI').bootstrapTable('getOptions');
    return ((tableOptions.pageNumber - 1) * tableOptions.pageSize) + (1 + index);
}
function searchDiseaseAI() {
    $("#tblDiseaseAI").bootstrapTable('refresh');
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
    $('#DiseaseAIRelatedVideo_DiseaseAIRelatedVideoId').val(rId);
}

function clearVideo() {
    editObj = null;
    isEdit = false;
    $('#VideoTitle').val("");
    $('#VideoUrl').val("");
    $('#DiseaseAIRelatedVideo_DiseaseAIRelatedVideoId').val(0);
}

function onSaveDiseaseAISuccess(data) {
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
                                                                    <button class="btn btn-sm btn-danger ml-1 text-small float-right rounded" type="button" onclick="deleteRelatedVideo(${data.returnObject.DiseaseAIRelatedVideoId}, this)"><i class="fas fa-trash mr-1"></i>Remove</button>
                                                                    <button class="btn btn-sm btn-secondary ml-1 text-small float-right rounded" type="button" data-url="${data.returnObject.videoUrl}" data-title="${data.returnObject.videoTitle}" onclick="editRelatedVideo(${data.returnObject.DiseaseAIRelatedVideoId}, this)"><i class="fas fa-edit mr-1"></i> Edit</button>
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
                url: 'DiseaseAI/DeleteRelatedVideo',
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
