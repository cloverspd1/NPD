
$(document).ready(function () {
    $('#divPhotometricTest').hide();
    SessionUpdater.Setup('KeepSessionAlive');
    
    if (jQuery.browser.safari)
        $('.iosEnable').show();
    else
        $('.iosEnable').hide();
    $(".sectionDetailType").change();



    //BindUserTags("");
    if ($("#CreatorAttachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachFile',
            Params: {},
            Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: false,
            CallBack: "OnFileUploaded"
        });
        uploadedFiles = BindFileList("CreatorAttachment", "AttachFile");
    }
    if ($("#Approver1Attachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentApprover1', Params: {}, Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedApprover1"
        });
        uploadedFiles1 = BindFileList("Approver1Attachment", "AttachmentApprover1");
    }
    if ($("#Approver2Attachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentApprover2', Params: {}, Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedApprover2"
        });
        uploadedFiles2 = BindFileList("Approver2Attachment", "AttachmentApprover2");
    }
    if ($("#Approver3Attachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentApprover3', Params: {}, Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedApprover3"
        });
        uploadedFiles3 = BindFileList("Approver3Attachment", "AttachmentApprover3");
    }
    if ($("#ABSAdminAttachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentABSAdmin', Params: {}, Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedABSAdmin"
        });
        uploadedFiles4 = BindFileList("ABSAdminAttachment", "AttachmentABSAdmin");
    }
    if ($("#StageGate1Attachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentStageGate1', Params: {}, Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedStageGate1"
        });
        uploadedFiles4 = BindFileList("StageGate1Attachment", "AttachmentStageGate1");
    }
    if ($("#StageGate2Attachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentStageGate2', Params: {}, Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedStageGate2"
        });
        uploadedFiles4 = BindFileList("StageGate2Attachment", "AttachmentStageGate2");
    }
    if ($("#StageGate3Attachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentStageGate3', Params: {}, Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedStageGate3"
        });
        uploadedFiles4 = BindFileList("StageGate3Attachment", "AttachmentStageGate3");
    }

    if ($("#ProductImage").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentProductImage',
            Params: {},
            Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: false,
            CallBack: "OnFileUploadedProductImage"
        });
        uploadedFiles5 = BindFileList("ProductImage", "AttachmentProductImage");
    }

    if ($("#SpecificationSheet").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentSpecificationSheet',
            Params: {},
            Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: false,
            CallBack: "OnFileUploadedSpecificationSheet"
        });
        uploadedFiles6 = BindFileList("SpecificationSheet", "AttachmentSpecificationSheet");
    }
    if ($("#TechnicalDataSheet").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentTechnicalDataSheet',
            Params: {},
            Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: false,
            CallBack: "OnFileUploadedTechnicalDataSheet"
        });
        uploadedFiles7 = BindFileList("TechnicalDataSheet", "AttachmentTechnicalDataSheet");
    }

    if ($("#TestReportsAttachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentTestReports',
            Params: {},
            Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: false,
            CallBack: "OnFileUploadedTestReportsAttachment"
        });
        uploadedFiles8 = BindFileList("TestReportsAttachment", "AttachmentTestReports");
    }

    //Mulit Select Dropdown Bind Start
    $('#GenderList').multiselect({
        includeSelectAllOption: true
    });



    $('#IncomeGroupList').multiselect({
        includeSelectAllOption: true
    });

    $('#AgeGroupList').multiselect({
        includeSelectAllOption: true
    });

    $('#RegionsList').multiselect({
        includeSelectAllOption: true
    });

    $('#ChannelsList').multiselect({
        includeSelectAllOption: true
    });
    //Mulit Select Dropdown Bind End

    var selectedGender = $("#GenderList").attr("data-selected");
    if ($.trim(selectedGender) != "") {
        $('#GenderList').multiselect('select', selectedGender.split(","));
    }

    var selectedIncomeGroup = $("#IncomeGroupList").attr("data-selected");
    if ($.trim(selectedIncomeGroup) != "") {
        $('#IncomeGroupList').multiselect('select', selectedIncomeGroup.split(","));
    }

    var selectedAgeGroup = $("#AgeGroupList").attr("data-selected");
    if ($.trim(selectedAgeGroup) != "") {
        $('#AgeGroupList').multiselect('select', selectedAgeGroup.split(","));
    }

    var selectedRegion = $("#RegionsList").attr("data-selected");
    if ($.trim(selectedRegion) != "") {
        $('#RegionsList').multiselect('select', selectedRegion.split(","));
    }

    var selectedChannels = $("#ChannelsList").attr("data-selected");
    if ($.trim(selectedChannels) != "") {
        $('#ChannelsList').multiselect('select', selectedChannels.split(","));
    }

    $("#ProjectType").html("<option value=''>Select</option>");

    $(ProjectTypeList).each(function (i, item) {

        var opt = $("<option/>");
        opt.text(item.Title + " (" + item.Value + ")");
        opt.attr("value", item.Value);
        opt.appendTo("#ProjectType");

    });



    $("#ProjectType").val($('#ProjectType').attr('data-selected'));

    //Approver USer based on Business Unit Chage Start


    $("select#BusinessUnit").off("change").on("change", function () {
        var BusinessUnitvalue = $("#BusinessUnit option:selected").val();
        clearProductCategoryDetails();
        if (BusinessUnitvalue != undefined) {
            $('#divPhotometricTest').hide();
            if (BusinessUnitvalue.toString().toUpperCase() == "CP-LTG" || BusinessUnitvalue.toString().toUpperCase() == "LUM") {
                $('#divPhotometricTest').show();
            }
            $("#ProductCategory").html("<option value=''>Select</option>");

            $(ProductCategoryList).each(function (i, item) {
                if (item.BusinessUnit == BusinessUnitvalue) {
                    var opt = $("<option/>");
                    opt.text(item.Value + ' - ' + item.Title);
                    opt.attr("value", item.Value);
                    opt.appendTo("#ProductCategory");
                }
            });


            $(Approverlist).each(function (i, item) {
                debugger;
                if (item.BusinessUnit == BusinessUnitvalue) {
                    $(".approver").each(function () {
                        var role = $(this).attr("data-dept");
                        if (item[role + "Name"])
                            $("td[data-dept$='" + role + "']").find("span").text(item[role + "Name"]);
                        else {
                            if (role == 'Creator')
                                $("td[data-dept$='" + role + "']").find("span").text(item[role + "Name"]);
                            else
                                $("td[data-dept$='" + role + "']").find("span").text('NA');
                        }

                        $("td[data-dept$='" + role + "']").find("input.hiddenuser").val(item[role]);
                        $("td[data-dept$='" + role + "']").find("input.hiddenusername").val(item[role + "Name"]);
                    });


                }
            });


            var selectedValue = $("#ProductCategory").attr("data-selected");
            if ($("#ProductCategory").find("option[value='" + selectedValue + "']").length > 0) {
                $("#ProductCategory").val(selectedValue).change();
            } else {
                $("#ProductCategory").val('').change();
            }
        }

    }).change();
    //Approver USer based on Business Unit Chage End

    //Product Category Details based on BU and Product category
    $("select#ProductCategory").off("change").on("change", function () {
        var strProductCategoryDetail = $("#ProductCategory option:selected").text();
        strProductCategoryDetail = $.trim(strProductCategoryDetail.substr(strProductCategoryDetail.indexOf('-') + 1));

        var ProductCategoryvalue = $("#ProductCategory option:selected").val();

        if (strProductCategoryDetail && ProductCategoryvalue) {
            var returnedData = $.grep(ProductCategoryDetailList, function (item, index) {
                return item.ProductCategoryDetail == strProductCategoryDetail && item.ProductCategoryCode == ProductCategoryvalue;
            });
            if (returnedData[0]) {
                if (!isNaN(returnedData[0]["CurrentSales"])) {
                    var currentSale = returnedData[0]["CurrentSales"] / 100000;
                    currentSale = currentSale.toFixed(3);
                    if (currentSale.indexOf('.') != -1) {
                        currentSale = Math.round(currentSale);
                    }
                    $('.CurrentSales').text(currentSale);
                    $('#CurrentSales').val(currentSale);
                }
                if (!isNaN(returnedData[0]["AverageSales"])) {
                    var averageSale = returnedData[0]["AverageSales"] / 100000;
                    averageSale = averageSale.toFixed(3);
                    if (averageSale.indexOf('.') != -1) {
                        averageSale = Math.round(averageSale);
                    }
                    $('.AverageSales').text(averageSale);
                    $('#AverageSales').val(averageSale);
                }
                $('.CurrentSKUs').text(returnedData[0]["CurrentSKUs"]);
                $('#CurrentSKUs').val(returnedData[0]["CurrentSKUs"]);
                $('.PhasedOutSKUs').text(returnedData[0]["PhasedOutSKUs"]);
                $('#PhasedOutSKUs').val(returnedData[0]["PhasedOutSKUs"]);
                //$('.CurrentSales').text(returnedData[0]["CurrentSales"]);
                //$('#CurrentSales').val(returnedData[0]["CurrentSales"]);
                //$('.AverageSales').text(returnedData[0]["AverageSales"]);
                //$('#AverageSales').val(returnedData[0]["AverageSales"]);
            }
            else {
                $('.CurrentSKUs').text("");
                $('#CurrentSKUs').val("");
                $('.PhasedOutSKUs').text('');
                $('#PhasedOutSKUs').val('');
                $('.CurrentSales').text('');
                $('#CurrentSales').val('');
                $('.AverageSales').text('');
                $('#AverageSales').val('');
            }

            //$(ProductCategoryDetailList).each(function (i, item) {

            //if (item.BusinessUnit == BusinessUnitvalue && item.ProductCategory == ProductCategoryvalue) {

            //    $('.CurrentSKUs').text(item.CurrentSKUs);
            //    $('#CurrentSKUs').val(item.CurrentSKUs);
            //    $('.PhasedOutSKUs').text(item.PhasedOutSKUs);
            //    $('#PhasedOutSKUs').val(item.PhasedOutSKUs);
            //    $('.CurrentSales').text(item.CurrentSales);
            //    $('#CurrentSales').val(item.CurrentSales);
            //    $('.AverageSales').text(item.AverageSales);
            //    $('#AverageSales').val(item.AverageSales);

            //}
            //});


        }
        else
            clearProductCategoryDetails();
    }).change();

    function clearProductCategoryDetails() {
        $('.CurrentSKUs').text('');
        $('#CurrentSKUs').val('');
        $('.PhasedOutSKUs').text('');
        $('#PhasedOutSKUs').val('');
        $('.CurrentSales').text('');
        $('#CurrentSales').val('');
        $('.AverageSales').text('');
        $('#AverageSales').val('');
    }

    $("#ChannelsList").on("change", function () {
        var isOtherChannelsVisible = false;
        $($('#ChannelsList').val()).each(function (index, value) {
            if (value == 'Others')
                isOtherChannelsVisible = true;
        });
        if (isOtherChannelsVisible) {

            $('.otherChannels').show();
        }
        else {
            $('.otherChannels').hide();
        }


    }).change();


    $("#AgeGroupList").on("change", function () {

        $('#AgeGroup').val($("#AgeGroupList").val());


    }).change();

    $("#RegionsList").on("change", function () {

        $('#Regions').val($("#RegionsList").val());


    }).change();

    $("#IncomeGroupList").on("change", function () {

        $('#IncomeGroup').val($("#IncomeGroupList").val());


    }).change();

    $("#GenderList").on("change", function () {

        $('#Gender').val($("#GenderList").val());


    }).change();

    $("#ChannelsList").on("change", function () {

        $('#Channels').val($("#ChannelsList").val());


    }).change();


    $("input[name='SampleRequired']").on("change", function () {

        if ($('#SampleRequired1')[0].checked == true) {
            $('.attachment').removeClass("hide");
        }
        else {
            $('.attachment').addClass("hide");
        }

    }).change();

    //}
    //$("#DivisionCode").off("change").on("change", function () {
    //    var value = $("#DivisionCode option:selected").text();
    //    $("#Division").val(value);
    //    $("#tbodyid .token-input-delete-token").click();
    //    if (value != 'Select') {
    //        GetDivisionApproverforDR(value);
    //    }
    //});
    //$("#Division").change();


    //$("#DivisionCode").off("change").on("change", function () {
    //    if ($("#DivisionCode").val() != '') {
    //        $("#Division").val($.trim($("#DivisionCode option:selected").text().split(' - ')[1]));
    //    } else {
    //        $("#Division").val('');
    //    }
    //    var value = $("#Division").val();
    //    $("#SalesGroupCode").html("<option value=''>Select</option>");
    //    if ($.trim(value) != "") {
    //        $(GroupList).each(function (i, item) {
    //            if (item.RelatedTo == value) {
    //                var opt = $("<option/>");
    //                opt.text(item.Value + ' - ' + item.Title);
    //                opt.attr("value", item.Value);
    //                opt.appendTo("#SalesGroupCode");
    //            }
    //        });
    //    }
    //    var selectedSection = $("#SalesGroupCode").attr("data-selected");

    //    if ($("#SalesGroupCode option[value='" + selectedSection + "']").length > 0) {
    //        $("#SalesGroupCode").val(selectedSection).change();
    //    } else {
    //        $("#SalesGroupCode").val('').change();
    //    }
    //}).change();
    //$("#SalesGroupCode").on("change", function () {
    //    if ($("#SalesGroupCode").val() != '') {
    //        var divisioncode = $("#DivisionCode option:selected").val();
    //        var salesgroupcode = $("#SalesGroupCode option:selected").val();
    //        var email = $("#RequestBy").val();
    //        $("#SalesGroup").val($.trim($("#SalesGroupCode option:selected").text()));
    //        GetDivisionApproverforDR(divisioncode, salesgroupcode, email);
    //    } else {
    //        $("#SalesGroup").val('');
    //    }
    //}).change();


});

var uploadedFiles = [], uploadedFiles1 = [], uploadedFiles2 = [], uploadedFiles3 = [], uploadedFiles4 = [], uploadedFiles5 = [], uploadedFiles6 = [], uploadedFiles7 = [], uploadedFiles8 = [];
function OnFileUploaded(result) {
    uploadedFiles.push(result);
    $("#CreatorAttachment").val(JSON.stringify(uploadedFiles)).blur();
}

function OnFileUploadedApprover1(result) {
    uploadedFiles1.push(result);
    $("#Approver1Attachment").val(JSON.stringify(uploadedFiles1)).blur();
}

function OnFileUploadedApprover2(result) {
    uploadedFiles2.push(result);
    $("#Approver2Attachment").val(JSON.stringify(uploadedFiles2)).blur();
}

function OnFileUploadedApprover3(result) {
    uploadedFiles3.push(result);
    $("#Approver3Attachment").val(JSON.stringify(uploadedFiles3)).blur();
}

function OnFileUploadedABSAdmin(result) {
    uploadedFiles4.push(result);
    $("#ABSAdminAttachment").val(JSON.stringify(uploadedFiles4)).blur();
}

function OnFileUploadedStageGate1(result) {
    uploadedFiles5.push(result);
    $("#StageGate1Attachment").val(JSON.stringify(uploadedFiles5)).blur();
}

function OnFileUploadedStageGate2(result) {
    uploadedFiles6.push(result);
    $("#StageGate2Attachment").val(JSON.stringify(uploadedFiles6)).blur();
}

function OnFileUploadedStageGate3(result) {
    uploadedFiles7.push(result);
    $("#StageGate3Attachment").val(JSON.stringify(uploadedFiles7)).blur();
}


function OnFileUploadedProductImage(result) {
    uploadedFiles5.push(result);
    $("#ProductImage").val(JSON.stringify(uploadedFiles5)).blur();
}

function OnFileUploadedSpecificationSheet(result) {
    uploadedFiles6.push(result);
    $("#SpecificationSheet").val(JSON.stringify(uploadedFiles6)).blur();
}

function OnFileUploadedTechnicalDataSheet(result) {
    uploadedFiles7.push(result);
    $("#TechnicalDataSheet").val(JSON.stringify(uploadedFiles7)).blur();
}

function OnFileUploadedTestReportsAttachment(result) {
    uploadedFiles8.push(result);
    $("#TestReportsAttachment").val(JSON.stringify(uploadedFiles8)).blur();
}

function AttachmentProductImageRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();
    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/NPD/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles5).each(function (i, item) {
                if (idx == -1 && item.FileId == id) {
                    idx = i;
                    if (item.Status == 0) {
                        item.Status = 2;
                        tmpList.push(item);
                    }
                } else {
                    tmpList.push(item);
                }
            });
            if (idx >= 0) {
                uploadedFiles5 = tmpList;
                li.remove();
                if (uploadedFiles5.length == 0) {
                    $("#ProductImage").val("").blur();
                } else {
                    $("#ProductImage").val(JSON.stringify(uploadedFiles5)).blur();
                }
            }
        }
    });
}

function AttachmentSpecificationSheetRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();
    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/NPD/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles6).each(function (i, item) {
                if (idx == -1 && item.FileId == id) {
                    idx = i;
                    if (item.Status == 0) {
                        item.Status = 2;
                        tmpList.push(item);
                    }
                } else {
                    tmpList.push(item);
                }
            });
            if (idx >= 0) {
                uploadedFiles6 = tmpList;
                li.remove();
                if (uploadedFiles6.length == 0) {
                    $("#SpecificationSheet").val("").blur();
                } else {
                    $("#SpecificationSheet").val(JSON.stringify(uploadedFiles6)).blur();
                }
            }
        }
    });
}


function AttachmentTechnicalDataSheetRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();
    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/NPD/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles7).each(function (i, item) {
                if (idx == -1 && item.FileId == id) {
                    idx = i;
                    if (item.Status == 0) {
                        item.Status = 2;
                        tmpList.push(item);
                    }
                } else {
                    tmpList.push(item);
                }
            });
            if (idx >= 0) {
                uploadedFiles7 = tmpList;
                li.remove();
                if (uploadedFiles7.length == 0) {
                    $("#TechnicalDataSheet").val("").blur();
                } else {
                    $("#TechnicalDataSheet").val(JSON.stringify(uploadedFiles7)).blur();
                }
            }
        }
    });
}

function AttachmentTestReportsRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();
    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/NPD/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles8).each(function (i, item) {
                if (idx == -1 && item.FileId == id) {
                    idx = i;
                    if (item.Status == 0) {
                        item.Status = 2;
                        tmpList.push(item);
                    }
                } else {
                    tmpList.push(item);
                }
            });
            if (idx >= 0) {
                uploadedFiles8 = tmpList;
                li.remove();
                if (uploadedFiles8.length == 0) {
                    $("#TestReportsAttachment").val("").blur();
                } else {
                    $("#TestReportsAttachment").val(JSON.stringify(uploadedFiles8)).blur();
                }
            }
        }
    });
}



function AttachFileRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();
    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/NPD/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles).each(function (i, item) {
                if (idx == -1 && item.FileId == id) {
                    idx = i;
                    if (item.Status == 0) {
                        item.Status = 2;
                        tmpList.push(item);
                    }
                } else {
                    tmpList.push(item);
                }
            });
            if (idx >= 0) {
                uploadedFiles = tmpList;
                li.remove();
                if (uploadedFiles.length == 0) {
                    $("#CreatorAttachment").val("").blur();
                } else {
                    $("#CreatorAttachment").val(JSON.stringify(uploadedFiles)).blur();
                }
            }
        }
    });
}

function AttachmentApprover1RemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();
    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/NPD/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles1).each(function (i, item) {
                if (idx == -1 && item.FileId == id) {
                    idx = i;
                    if (item.Status == 0) {
                        item.Status = 2;
                        tmpList.push(item);
                    }
                } else {
                    tmpList.push(item);
                }
            });
            if (idx >= 0) {
                uploadedFiles1 = tmpList;
                li.remove();
                if (uploadedFiles1.length == 0) {
                    $("#Approver1Attachment").val("").blur();
                } else {
                    $("#Approver1Attachment").val(JSON.stringify(uploadedFiles1)).blur();
                }
            }
        }
    });
}

function AttachmentApprover2RemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();
    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/NPD/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles2).each(function (i, item) {
                if (idx == -1 && item.FileId == id) {
                    idx = i;
                    if (item.Status == 0) {
                        item.Status = 2;
                        tmpList.push(item);
                    }
                } else {
                    tmpList.push(item);
                }
            });
            if (idx >= 0) {
                uploadedFiles2 = tmpList;
                li.remove();
                if (uploadedFiles2.length == 0) {
                    $("#Approver2Attachment").val("").blur();
                } else {
                    $("#Approver2Attachment").val(JSON.stringify(uploadedFiles2)).blur();
                }
            }
        }
    });
}

function AttachmentApprover3RemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();
    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/NPD/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles3).each(function (i, item) {
                if (idx == -1 && item.FileId == id) {
                    idx = i;
                    if (item.Status == 0) {
                        item.Status = 2;
                        tmpList.push(item);
                    }
                } else {
                    tmpList.push(item);
                }
            });
            if (idx >= 0) {
                uploadedFiles3 = tmpList;
                li.remove();
                if (uploadedFiles3.length == 0) {
                    $("#Approver3Attachment").val("").blur();
                } else {
                    $("#Approver3Attachment").val(JSON.stringify(uploadedFiles3)).blur();
                }
            }
        }
    });
}

function AttachmentABSAdminRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();
    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/NPD/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles4).each(function (i, item) {
                if (idx == -1 && item.FileId == id) {
                    idx = i;
                    if (item.Status == 0) {
                        item.Status = 2;
                        tmpList.push(item);
                    }
                } else {
                    tmpList.push(item);
                }
            });
            if (idx >= 0) {
                uploadedFiles4 = tmpList;
                li.remove();
                if (uploadedFiles4.length == 0) {
                    $("#ABSAdminAttachment").val("").blur();
                } else {
                    $("#ABSAdminAttachment").val(JSON.stringify(uploadedFiles4)).blur();
                }
            }
        }
    });
}

function OnProductFeatureaddSuccess(data, status, xhr) {
    if (data.IsSucceed) {
        $("#addeditProductFeatureModel").modal('hide');
        AlertModal('Success', ParseMessage(data.Messages));
        AjaxCall({
            url: "/NPD/GetProductFeatureGrid",
            httpmethod: "GET",
            sucesscallbackfunction: function (result) {
                $("#divProductFeatureGrid").html(result);
            }
        });
    } else {
        AlertModal('Error', ParseMessage(data.Messages));
    }
}

function ProductFeatureDelete(index) {
    ConfirmationDailog({
        title: "Delete Product Feature",
        message: "Are you sure want to delete?",
        url: "/NPD/DeleteProductFeature?index=" + index,
        okCallback: function (id, data) {
            OnProductFeatureaddSuccess(data);
        }
    });
}

function OnProductCompetitoraddSuccess(data, status, xhr) {
    if (data.IsSucceed) {
        $("#addeditProductCompetitorModel").modal('hide');
        AlertModal('Success', ParseMessage(data.Messages));
        AjaxCall({
            url: "/NPD/GetProductCompetitorGrid",
            httpmethod: "GET",
            sucesscallbackfunction: function (result) {
                $("#divProductCompetitorInfoGrid").html(result);
            }
        });
    } else {
        AlertModal('Error', ParseMessage(data.Messages));
    }
}

function ProductCompetitorDelete(index) {
    ConfirmationDailog({
        title: "Delete Product Competitor Information",
        message: "Are you sure want to delete?",
        url: "/NPD/DeleteProductCompetitor?index=" + index,
        okCallback: function (id, data) {
            OnProductCompetitoraddSuccess(data);
        }
    });
}

function OnCumulativeSalesProjectionaddSuccess(data, status, xhr) {
    if (data.IsSucceed) {
        $("#addeditCumulativeSalesProjectionModel").modal('hide');
        AlertModal('Success', ParseMessage(data.Messages));
        AjaxCall({
            url: "/NPD/GetCumulativeSalesProjectionGrid",
            httpmethod: "GET",
            sucesscallbackfunction: function (result) {
                $("#divCumulativeSalesProjectionGrid").html(result);
            }
        });
    } else {
        AlertModal('Error', ParseMessage(data.Messages));
    }
}

function CumulativeSalesProjectionDelete(index) {
    ConfirmationDailog({
        title: "Delete Cumulative Sales Projection",
        message: "Are you sure want to delete?",
        url: "/NPD/DeleteCumulativeSalesProjection?index=" + index,
        okCallback: function (id, data) {
            OnCumulativeSalesProjectionaddSuccess(data);
        }
    });
}

function OnTargetRLPaddSuccess(data, status, xhr) {
    if (data.IsSucceed) {
        $("#addeditTargetRLP").modal('hide');
        AlertModal('Success', ParseMessage(data.Messages));
        AjaxCall({
            url: "/NPD/GetTargetRLPGrid",
            httpmethod: "GET",
            sucesscallbackfunction: function (result) {
                $("#divTargetRLPGrid").html(result);
            }
        });
    } else {
        AlertModal('Error', ParseMessage(data.Messages));
    }
}

function TargetRLPDelete(index) {
    ConfirmationDailog({
        title: "Delete Target RLP",
        message: "Are you sure want to delete?",
        url: "/NPD/DeleteTargetRLP?index=" + index,
        okCallback: function (id, data) {
            OnTargetRLPaddSuccess(data);
        }
    });
}

function SetToHold(selectedListIds) {
    ConfirmationDailog({
        title: "Set To Hold",
        message: "Are you sure want to Hold selected item?",
        url: "~/Base/ChangeHoldReleaseStatus?ids=" + selectedListIds + "&isHold=" + true,
        okCallback: function (id, data) {
            window.location.reload();
        }
    });
}

function SetToRelease(selectedListIds) {
    /// <summary>
    /// Sets to release.
    /// </summary>
    /// <param name="selectedListIds">The selected list ids.</param>
    /// <returns></returns>
    ConfirmationDailog({
        title: "Set To Release",
        message: "Are you sure want to Release selected item?",
        url: "~/Base/ChangeHoldReleaseStatus?ids=" + selectedListIds + "&isHold=" + false,
        okCallback: function (id, data) {
            window.location.reload();
        }
    });
}