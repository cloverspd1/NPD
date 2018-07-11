namespace NPD.Workflow.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Linq;
    using Newtonsoft.Json;
    using CommonDataContract;
    using Common;
    using Models.NPD;
    using Models.Common;
    using System.IO;
    using System.Text;
    using System.Web.UI;
    using BusinessLayer;
    using NPD.NWorkflow;
    using NPD.NWorkflow.BusinessLayer;

    /// <summary>
    /// PR Controller used for saving Market visit report
    /// </summary>
    public partial class NPDController : NPDBaseController
    {
        #region "Index"

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// Index View
        /// </returns>      
        [SharePointContextFilter]
        public ActionResult NPDAdmin(int id = 0, Int16 urlAction = 0)
        {
            if (urlAction == Convert.ToInt16(URLAction.NOACTION))
            {
                if (id > 0 && NPDBusinessLayer.Instance.IsAdminUser(this.CurrentUser.UserId))
                {
                    Logger.Info("Start NPD form and ID = " + id);
                    Dictionary<string, string> objDict = new Dictionary<string, string>();
                    objDict.Add(Parameter.FROMNAME, FormNameConstants.NPDADMINFORM);
                    objDict.Add(Parameter.ITEMID, id.ToString());
                    objDict.Add(Parameter.USEREID, this.CurrentUser.UserId);
                    ViewBag.UserID = this.CurrentUser.UserId;
                    ViewBag.UserName = this.CurrentUser.FullName;
                    NPDContract contract = this.GetNPDAdminDetails(objDict);
                    contract.UserDetails = this.CurrentUser;
                    if (contract != null && contract.Forms != null && contract.Forms.Count > 0)
                    {
                        NPDAdminDetailSection npdDetailSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.NPDADMINSECTION) as NPDAdminDetailSection;
                        ApplicationStatusSection npdApprovalSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == SectionNameConstant.APPLICATIONSTATUS) as ApplicationStatusSection;

                        if (npdApprovalSection != null)
                        {
                            npdDetailSection.ApproversList = npdApprovalSection.ApplicationStatusList;
                            npdDetailSection.Approver1Comments = npdDetailSection.ApproversList.FirstOrDefault(p => p.Role == NPDRoles.APPROVER1).Comments;
                            npdDetailSection.Approver2Comments = npdDetailSection.ApproversList.FirstOrDefault(p => p.Role == NPDRoles.APPROVER2).Comments;
                            npdDetailSection.Approver3Comments = npdDetailSection.ApproversList.FirstOrDefault(p => p.Role == NPDRoles.APPROVER3).Comments;
                            npdDetailSection.ABSAdmincomments = npdDetailSection.ApproversList.FirstOrDefault(p => p.Role == NPDRoles.ABSADMIN).Comments;
                        }


                        if (npdDetailSection != null)
                        {
                            this.SetTranListintoTempData<NPDProductFeature>(npdDetailSection.NPDProductFeature, TempKeys.NPDProductFeature.ToString() + "_" + id);
                            this.SetTranListintoTempData<NPDProductCompetitor>(npdDetailSection.NPDProductCompetitor, TempKeys.NPDProductCompetitor.ToString() + "_" + id);
                            this.SetTranListintoTempData<NPDMarketInformation>(npdDetailSection.NPDMarketInformation, TempKeys.NPDMarketInformation.ToString() + "_" + id);
                            this.SetTranListintoTempData<NPDCumulativeSalesProjection>(npdDetailSection.NPDCumulativeSalesProjection, TempKeys.NPDCumulativeSalesProjection.ToString() + "_" + id);
                            this.SetTranListintoTempData<NPDTargetRLP>(npdDetailSection.NPDTargetRLP, TempKeys.NPDTargetRLP.ToString() + "_" + id);
                        }

                    }
                    else
                    {
                        return this.RedirectToAction("NotAuthorize", "Master");
                    }

                    return this.View("Admin/NPDAdminIndex", contract);
                }
                else
                {
                    return this.RedirectToAction("NotAuthorize", "Master");
                }
            }
            else if (id > 0 && urlAction == Convert.ToInt16(URLAction.ROLLBACK))
            {

                if (id > 0 && NPDBusinessLayer.Instance.IsCreatorUser(this.CurrentUser.UserId))
                {
                    Logger.Info("Start NPD form and ID = " + id);
                    Dictionary<string, string> objDict = new Dictionary<string, string>();
                    objDict.Add(Parameter.FROMNAME, FormNameConstants.NPDADMINFORM);
                    objDict.Add(Parameter.ITEMID, id.ToString());
                    objDict.Add(Parameter.USEREID, this.CurrentUser.UserId);
                    ViewBag.UserID = this.CurrentUser.UserId;
                    ViewBag.UserName = this.CurrentUser.FullName;
                    NPDContract contract = this.GetNPDAdminDetails(objDict);
                    contract.UserDetails = this.CurrentUser;
                    if (contract != null && contract.Forms != null && contract.Forms.Count > 0)
                    {
                        contract.Forms[0].Buttons = new List<Button>();
                        Button btn = new Button();
                        btn.Name = ButtonCaption.SAVEASDRAFT;
                        btn.ButtonStatus = ButtonActionStatus.SaveAndNoStatusUpdate;
                        btn.ToolTip = "You can save all updated fields.";
                        btn.JsFunction = "ConfirmSubmit";
                        btn.IsVisible = true;
                        btn.Icon = "fa fa-save";
                        contract.Forms[0].Buttons.Add(btn);

                        NPDAdminDetailSection npdDetailSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.NPDADMINSECTION) as NPDAdminDetailSection;
                        ApplicationStatusSection npdApprovalSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == SectionNameConstant.APPLICATIONSTATUS) as ApplicationStatusSection;

                        if (npdApprovalSection != null)
                        {
                            if (npdDetailSection.ProposedBy != this.CurrentUser.UserId)
                            {
                                return this.RedirectToAction("NotAuthorize", "Master");
                            }
                            npdDetailSection.ApproversList = npdApprovalSection.ApplicationStatusList;
                            npdDetailSection.IsCreatorEdit = true;
                        }

                        if (npdDetailSection != null)
                        {
                            this.SetTranListintoTempData<NPDProductFeature>(npdDetailSection.NPDProductFeature, TempKeys.NPDProductFeature.ToString() + "_" + id);
                            this.SetTranListintoTempData<NPDProductCompetitor>(npdDetailSection.NPDProductCompetitor, TempKeys.NPDProductCompetitor.ToString() + "_" + id);
                            this.SetTranListintoTempData<NPDMarketInformation>(npdDetailSection.NPDMarketInformation, TempKeys.NPDMarketInformation.ToString() + "_" + id);
                            this.SetTranListintoTempData<NPDCumulativeSalesProjection>(npdDetailSection.NPDCumulativeSalesProjection, TempKeys.NPDCumulativeSalesProjection.ToString() + "_" + id);
                            this.SetTranListintoTempData<NPDTargetRLP>(npdDetailSection.NPDTargetRLP, TempKeys.NPDTargetRLP.ToString() + "_" + id);
                        }
                    }
                    else
                    {
                        return this.RedirectToAction("NotAuthorize", "Master");
                    }

                    return this.View("Admin/NPDAdminIndex", contract);
                }
                else
                {
                    return this.RedirectToAction("NotAuthorize", "Master");
                }

            }
            return this.RedirectToAction("NotAuthorize", "Master");
            //return this.View(contract,"/Admin/NPDAdminIndex");

        }
        #endregion

        #region "Save NPD Detail Section"
        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Content result
        /// </returns>
        //[HttpPost, ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveAdminNPDDetailSection(NPDAdminDetailSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null && !string.IsNullOrWhiteSpace(model.Channels))
            {
                if (model != null && !(model.Channels.Contains("Others")))
                {
                    ModelState.Remove("OtherChannels");
                }
            }

            if (model != null && model.SampleRequired == false)
            {
                ModelState.Remove("CreatorAttachment");
            }




            if (model != null && ModelState.IsValid)
            {

               // model.Files = FileListHelper.GenerateFileBytes(model.FileNameList);  //For Save Attachemennt
                model.Files = FileListHelper.GenerateFileBytes(model.CreatorAttachment);  //For Save Attachemennt
                model.CreatorAttachment = string.Join(",", FileListHelper.GetFileNames(model.CreatorAttachment));

                model.Files.AddRange(FileListHelper.GenerateFileBytes(model.Approver1Attachment));
                model.Approver1Attachment = string.Join(",", FileListHelper.GetFileNames(model.Approver1Attachment));

                model.Files.AddRange(FileListHelper.GenerateFileBytes(model.Approver2Attachment));
                model.Approver2Attachment = string.Join(",", FileListHelper.GetFileNames(model.Approver2Attachment));

                model.Files.AddRange(FileListHelper.GenerateFileBytes(model.Approver3Attachment));
                model.Approver3Attachment = string.Join(",", FileListHelper.GetFileNames(model.Approver3Attachment));

                model.Files.AddRange(FileListHelper.GenerateFileBytes(model.ABSAdminAttachment));
                model.ABSAdminAttachment = string.Join(",", FileListHelper.GetFileNames(model.ABSAdminAttachment));


                //for Add Product Feature Grid Data to Model
                var list = this.GetTempData<List<NPDProductFeature>>(TempKeys.NPDProductFeature.ToString() + "_" + GetFormIdFromUrl());
                if (model.ActionStatus == ButtonActionStatus.SaveAndNoStatusUpdate && (list == null || list.Count == 0 || !list.Any(m => m.ItemAction != ItemActionStatus.DELETED) || (!IsValidProductFeature(list))))
                {
                    status.IsSucceed = false;
                    status.Messages = new List<string>() { this.GetResourceValue("Text_ProductFeatureRequired", System.Web.Mvc.Html.ResourceNames.NPD) };
                    return this.Json(status);
                }
                model.NPDProductFeature = list.ToList<ITrans>();

                //for Add Product Competitor Grid Data to Model
                var listcompetitor = this.GetTempData<List<NPDProductCompetitor>>(TempKeys.NPDProductCompetitor.ToString() + "_" + GetFormIdFromUrl());
                model.NPDProductCompetitor = listcompetitor.ToList<ITrans>();

                //for Add Cumulative Sales Projection Grid Data to Model
                var cumulativesalesprojection = this.GetTempData<List<NPDCumulativeSalesProjection>>(TempKeys.NPDCumulativeSalesProjection.ToString() + "_" + GetFormIdFromUrl());
                if (model.ActionStatus == ButtonActionStatus.SaveAndNoStatusUpdate && (cumulativesalesprojection == null || cumulativesalesprojection.Count == 0 || !cumulativesalesprojection.Any(m => m.ItemAction != ItemActionStatus.DELETED) || (!IsValidCumulativeSalesProjection(cumulativesalesprojection))))
                {
                    status.IsSucceed = false;
                    status.Messages = new List<string>() { this.GetResourceValue("Text_CumulativeSalesProjectionRequired", System.Web.Mvc.Html.ResourceNames.NPD) };
                    return this.Json(status);
                }
                model.NPDCumulativeSalesProjection = cumulativesalesprojection.ToList<ITrans>();

                //for Add Cumulative Sales Projection Grid Data to Model
                var targetRLP = this.GetTempData<List<NPDTargetRLP>>(TempKeys.NPDTargetRLP.ToString() + "_" + GetFormIdFromUrl());
                model.NPDTargetRLP = targetRLP.ToList<ITrans>();

                if (model.ApproversList != null && !model.IsCreatorEdit)
                {
                    model.ApproversList.FirstOrDefault(p => p.Role == NPDRoles.APPROVER1).Comments = model.Approver1Comments;
                    model.ApproversList.FirstOrDefault(p => p.Role == NPDRoles.APPROVER2).Comments = model.Approver2Comments;
                    model.ApproversList.FirstOrDefault(p => p.Role == NPDRoles.APPROVER3).Comments = model.Approver3Comments;
                    model.ApproversList.FirstOrDefault(p => p.Role == NPDRoles.ABSADMIN).Comments = model.ABSAdmincomments;

                }
                if(model.IsCreatorEdit)
                {
                    model.SectionName = "NPD Detail Section Modification";
                }
                Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);

                status = this.SaveAdminDetailSection(model, objDict);
                status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.NPD);
            }
            else
            {
                status.IsSucceed = false;
                status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.NPD);
            }


            return this.Json(status);
        }
        #endregion


    }
}