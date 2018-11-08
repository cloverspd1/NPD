$(document).ready(function () {

    $('.parentCacheChk').click(function () {
        var isChecked = $(this).prop("checked");
        $('#tblData tr:has(td)').find('input[type="checkbox"]').prop('checked', isChecked);
    });

    $('#tblData tr:has(td)').find('input[type="checkbox"]').click(function () {
        var isChecked = $(this).prop("checked");
        var isHeaderChecked = $(".parentCacheChk").prop("checked");
        if (isChecked == false && isHeaderChecked)
            $(".parentCacheChk").prop('checked', isChecked);
        else {
            $('#tblData tr:has(td)').find('input[type="checkbox"]').each(function () {
                if ($(this).prop("checked") == false)
                    isChecked = false;
            });
            $(".parentCacheChk").prop('checked', isChecked);
        }
    });

    //Generate LSMW start
    $("#btnClearCache").on("click", function () {
        var selectedListIds = '';
        $("#divClearCache input[type='checkbox']:checked").each(function () {
            if (selectedListIds != '') { selectedListIds += ","; }
            selectedListIds += $(this).attr('id');
        });
        if (selectedListIds == '') {
            // alert('Please select atleast one record to generate LSMW.');         
            AlertModal('Error', 'Please select atleast one record to clear cache.');
        }
        else {
            ClearCache(selectedListIds)
        }
    });
    //Generate LSMW start

});

function ClearCache(selectedListIds) {
    if (selectedListIds != 0) {

        AjaxCall({
            url:  "/Admin/ClearCache?ids=" + selectedListIds,
            httpmethod: "Post",
            sucesscallbackfunction: function (result) {
                AlertModal('Success', ParseMessage(result.Messages));
                setTimeout(function () { window.location  = window.location.href; }, 5000);
            }
        });

       // window.open('/Admin/ClearCache' + "?ids=" + selectedListIds);
        //setTimeout(function () { window.location.reload(true); }, 5000);
    }
}




