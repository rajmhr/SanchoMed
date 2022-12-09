
function ShowMessage(success, message) {
    if (success) {
        $.notify({
            title: "<strong> Success </strong> ",
            message: message,
            icon: 'fa fa-check',
            offset: {
                x: 500,
                y: 100
            },
            animate: {
                enter: 'animated bounceInDown',
                exit: 'animated bounceOutUp'
            },
            allow_dismiss: true
        }, {
                type: 'success',
                timer: 500,
                z_index: 11000,
                mouse_over: 'pause'
            });
    } else {
        $.notify({
            title: "<strong>Error </strong> ",
            message: message,
            icon: 'fa fa-times',
            allow_dismiss: true,
            animate: {
                enter: 'animated bounceInDown',
                exit: 'animated bounceOutUp'
            }
        },
            {
                type: 'danger',
                timer: 500,
                z_index: 11000,
                mouse_over: 'pause'
            });
    }
}
function ShowReceivedGrpMessage(chatModel) {
    $.notify({
        position: "right",
        icon: 'fas fa-comment',
        title: `<strong>Incomming message.. In <span class='badge badge-info'> ${chatModel.groupName}</span><br/></strong> `,
        message: `<span>${chatModel.message}</span>
                  <span class="badge badge-info">Send by : ${chatModel.userDisplayName}</span>`,
        allow_dismiss: true
    }, {
            type: 'success',
            timer: 500,
            z_index: 11000,
            mouse_over:'pause',
            placement: {
                from: "bottom",
                align: "right"
            }
        });
}

function confirmDialog(message, handler) {
    $(`<div class="modal fade" id="myModal" role="dialog"> 
     <div class="modal-dialog"> 
       <!-- Modal content--> 
        <div class="modal-content"> 
           <div class="modal-body" style="padding:10px;"> 
             <h5 class="text-center">${message}</h5> 
             <div class="text-center"> 
               <a class="btn btn-primary btn-no" style="color:black">No</a> 
               <a class="btn btn-danger btn-yes" style="color:black">Yes</a> 
             </div> 
           </div> 
       </div> 
    </div> 
  </div>`).appendTo('body');

    //Trigger the modal
    $("#myModal").modal({
        backdrop: 'static',
        keyboard: false
    });

    //Pass true to a callback function
    $(".btn-yes").click(function () {
        handler(true);
        $("#myModal").modal("hide");
    });

    //Pass false to callback function
    $(".btn-no").click(function () {
        handler(false);
        $("#myModal").modal("hide");
    });

    //Remove the modal once it is closed.
    $("#myModal").on('hidden.bs.modal', function () {
        $("#myModal").remove();
    });
}

AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
    return data;
};



function validateForm(formId) {
    $(formId).removeData("validator");
    $(formId).removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse(formId);
}
function setTableContextMenu(tableId, event, idField, idValue, menu) {
    var $table = $(tableId);
    $table.bootstrapTable('uncheckAll');;
    $table.bootstrapTable('checkBy', { field: idField, values: [idValue] });
    $table.bootstrapTable('showContextMenu', { event: event, contextMenu: menu });
}

function isEnterPressed(e) {
    return (e.which === 13);
}
function isFormValid(formId) {
    return $(formId).valid();
}


function initTable(tableId) {
    $(tableId).bootstrapTable('scrollTo', top);

    $(tableId).on('all.bs.table', function (e, data) {
        $(`${tableId} thead tr`).css('background', '#4fc3f7');
        $(`${tableId} thead tr`).css('color', '#111111');
    });
}


function setTableWithoutContextMenu(tableId, event, idField, idValue) {
    var $table = $(tableId);
    $table.bootstrapTable('uncheckAll');;
    $table.bootstrapTable('checkBy', { field: idField, values: [idValue] });
}