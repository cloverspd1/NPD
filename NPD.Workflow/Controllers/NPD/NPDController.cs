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
    using NPD.Workflow.Models.Master;
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
        public ActionResult Index(int id = 0, bool IsRetrieve = false, Int16 urlAction = 0)
        {
            NPDContract contract = null;
            Logger.Info("Start NPD form and ID = " + id);
            Dictionary<string, string> objDict = new Dictionary<string, string>();
            objDict.Add(Parameter.FROMNAME, FormNameConstants.NPDFORM);
            objDict.Add(Parameter.ITEMID, id.ToString());
            objDict.Add(Parameter.USEREID, this.CurrentUser.UserId);


            ViewBag.UserID = this.CurrentUser.UserId;
            ViewBag.UserName = this.CurrentUser.FullName;
            contract = this.GetNPDDetails(objDict);
            contract.UserDetails = this.CurrentUser;

            if (!IsRetrieve && urlAction == Convert.ToInt16(URLAction.NOACTION))
            {
                if (contract != null && contract.Forms != null && contract.Forms.Count > 0)
                {
                    NPDDetailSection npdDetailSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.NPDDETAILSECTION) as NPDDetailSection;
                    if (id == 0)
                    {
                        ApproverMaster approverlist = npdDetailSection.MasterData.FirstOrDefault(x => x.NameOfMaster.Equals(Masters.APPROVERMASTER)) as ApproverMaster;
                        if (approverlist.Items != null && approverlist.Items.Count > 0)
                        {
                            var Approverlist = approverlist.Items.ToList();
                            List<Tuple<string, string, string, string>> lst = new List<Tuple<string, string, string, string>>();
                            foreach (var item in Approverlist)
                            {
                                ApproverMasterListItem requester = item as ApproverMasterListItem;
                                if (!string.IsNullOrWhiteSpace(requester.Requestor))
                                {
                                    string[] str = requester.Requestor.Split(',');
                                    foreach (string strinner in str)
                                    {
                                        lst.Add(new Tuple<string, string, string, string>(strinner, requester.BusinessUnit, requester.Viewer, requester.ViewerName));
                                    }
                                }


                            }
                            bool isValid = false;
                            foreach (var item in lst)
                            {
                                if (item.Item1 == this.CurrentUser.UserId)
                                {
                                    npdDetailSection.BusinessUnit = item.Item2;
                                    npdDetailSection.RequestViewer = item.Item3;
                                    npdDetailSection.RequestViewerName = item.Item4;
                                    isValid = true;
                                    break;
                                }
                            }
                            if (!isValid)
                                return this.RedirectToAction("NotAuthorize", "Master");
                        }
                        else
                        {
                            return this.RedirectToAction("NotAuthorize", "Master");
                        }
                    }



                    if (npdDetailSection != null && npdDetailSection.IsActive)
                    {
                        this.SetTranListintoTempData<NPDProductFeature>(npdDetailSection.NPDProductFeature, TempKeys.NPDProductFeature.ToString() + "_" + id);
                        this.SetTranListintoTempData<NPDProductCompetitor>(npdDetailSection.NPDProductCompetitor, TempKeys.NPDProductCompetitor.ToString() + "_" + id);
                        this.SetTranListintoTempData<NPDMarketInformation>(npdDetailSection.NPDMarketInformation, TempKeys.NPDMarketInformation.ToString() + "_" + id);
                        this.SetTranListintoTempData<NPDCumulativeSalesProjection>(npdDetailSection.NPDCumulativeSalesProjection, TempKeys.NPDCumulativeSalesProjection.ToString() + "_" + id);
                        this.SetTranListintoTempData<NPDTargetRLP>(npdDetailSection.NPDTargetRLP, TempKeys.NPDTargetRLP.ToString() + "_" + id);
                    }
                    return this.View(contract);
                }
                else
                {
                    return this.RedirectToAction("NotAuthorize", "Master");
                }
            }

            else if (IsRetrieve && urlAction == Convert.ToInt16(URLAction.NOACTION))
            {
                if (contract != null && contract.Forms != null && contract.Forms.Count != 0)
                {
                    contract.Forms[0].FormStatus = string.Empty;

                    Button btn = new Button();
                    btn.Name = "Submit";
                    btn.ButtonStatus = ButtonActionStatus.NextApproval;
                    btn.JsFunction = "ConfirmSubmit";
                    btn.ToolTip = "Request will move to the next approval level.";
                    btn.IsVisible = true;
                    btn.Icon = "fa fa-share";
                    contract.Forms[0].Buttons.Add(btn);
                    if (contract.Forms[0].Buttons.FirstOrDefault(p => p.Name == "Print") != null)
                    {
                        contract.Forms[0].Buttons.FirstOrDefault(p => p.Name == "Print").IsVisible = false;
                    }

                    var npdDetailSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.NPDDETAILSECTION) as NPDDetailSection;
                    if (npdDetailSection != null)
                    {
                        ////If Login user is not ABS Admin then error Unauthorized.                      
                        string[] strABSAdmin = npdDetailSection.ApproversList.FirstOrDefault(p => p.Role == NPDRoles.ABSADMIN).Approver.Split(',');
                        if (Array.IndexOf(strABSAdmin, this.CurrentUser.UserId) == -1)
                        {
                            return this.RedirectToAction("NotAuthorize", "Master");
                        }
                        npdDetailSection.OldProjectNo = npdDetailSection.ProjectNo;
                        npdDetailSection.OldNPDCreatedDate = npdDetailSection.RequestDate;
                        ////dcrDetailSection.OldDCRRejectedDate = dcrDetailSection.DCRRejectedDate;
                        npdDetailSection.OldNPDId = id;
                        npdDetailSection.ProposedBy = CurrentUser.UserId;
                        npdDetailSection.ProposedByName = CurrentUser.FullName;
                        npdDetailSection.ProjectNo = string.Empty;
                        npdDetailSection.RequestDate = null;
                        npdDetailSection.WorkflowStatus = string.Empty;
                        npdDetailSection.IsActive = true;
                        npdDetailSection.ListDetails[0].ItemId = 0;
                        npdDetailSection.CurrentApprover.Approver = CurrentUser.UserId;
                        npdDetailSection.CurrentApprover.ApproverName = CurrentUser.FullName;
                        //  dcrDetailSection.ApproversList[0].Approver = dcrProcessIcSection.ApproversList[0].Approver;

                        if (npdDetailSection.NPDProductFeature != null && npdDetailSection.NPDProductFeature.Count > 0)
                        {
                            foreach (var item in npdDetailSection.NPDProductFeature)
                            {
                                item.RequestID = 0;
                                item.ID = 0;
                                item.ItemAction = ItemActionStatus.NEW;
                            }
                        }
                        if (npdDetailSection.NPDProductCompetitor != null && npdDetailSection.NPDProductCompetitor.Count > 0)
                        {
                            foreach (var item in npdDetailSection.NPDProductCompetitor)
                            {
                                item.RequestID = 0;
                                item.ID = 0;
                                item.ItemAction = ItemActionStatus.NEW;
                            }
                        }
                        if (npdDetailSection.NPDMarketInformation != null && npdDetailSection.NPDMarketInformation.Count > 0)
                        {
                            foreach (var item in npdDetailSection.NPDMarketInformation)
                            {
                                item.RequestID = 0;
                                item.ID = 0;
                                item.ItemAction = ItemActionStatus.NEW;
                            }
                        }
                        if (npdDetailSection.NPDCumulativeSalesProjection != null && npdDetailSection.NPDCumulativeSalesProjection.Count > 0)
                        {
                            foreach (var item in npdDetailSection.NPDCumulativeSalesProjection)
                            {
                                item.RequestID = 0;
                                item.ID = 0;
                                item.ItemAction = ItemActionStatus.NEW;
                            }
                        }
                        if (npdDetailSection.NPDTargetRLP != null && npdDetailSection.NPDTargetRLP.Count > 0)
                        {
                            foreach (var item in npdDetailSection.NPDTargetRLP)
                            {
                                item.RequestID = 0;
                                item.ID = 0;
                                item.ItemAction = ItemActionStatus.NEW;
                            }
                        }

                        if (npdDetailSection.Files != null && npdDetailSection.Files.Count > 0)
                        {
                            foreach (var item in npdDetailSection.Files)
                            {
                                item.Status = FileStatus.New;
                            }
                        }
                        if (!string.IsNullOrEmpty(npdDetailSection.CreatorAttachment))
                        {
                            var modifiedJsonString = "";
                            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(npdDetailSection.CreatorAttachment);
                            if (jsonObject != null)
                            {
                                for (int i = 0; i < ((Newtonsoft.Json.Linq.JContainer)(jsonObject)).Count; i++)
                                {
                                    jsonObject[i].Status = FileStatus.New;
                                }
                                 modifiedJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
                            }
                            npdDetailSection.CreatorAttachment = modifiedJsonString;
                        }
                        if (!string.IsNullOrEmpty(npdDetailSection.SpecificationSheet) || npdDetailSection.SpecificationSheet != null)
                        {
                            dynamic jsonObject1 = Newtonsoft.Json.JsonConvert.DeserializeObject(npdDetailSection.SpecificationSheet);
                            if (jsonObject1 != null)
                            {
                                for (int i = 0; i < ((Newtonsoft.Json.Linq.JContainer)(jsonObject1)).Count; i++)
                                {
                                    jsonObject1[i].Status = FileStatus.New;
                                }
                            }
                            var modifiedJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject1);
                            npdDetailSection.SpecificationSheet = modifiedJsonString;
                        }

                        if (!string.IsNullOrEmpty(npdDetailSection.TechnicalDataSheet) || npdDetailSection.TechnicalDataSheet != null)
                        {
                            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(npdDetailSection.TechnicalDataSheet);
                            if (jsonObject != null)
                            {
                                for (int i = 0; i < ((Newtonsoft.Json.Linq.JContainer)(jsonObject)).Count; i++)
                                {
                                    jsonObject[i].Status = FileStatus.New;
                                }
                            }
                            var modifiedJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
                            npdDetailSection.TechnicalDataSheet = modifiedJsonString;
                        }

                        if (!string.IsNullOrEmpty(npdDetailSection.ProductImage) || npdDetailSection.ProductImage != null)
                        {
                            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(npdDetailSection.ProductImage);
                            if (jsonObject != null)
                            {
                                for (int i = 0; i < ((Newtonsoft.Json.Linq.JContainer)(jsonObject)).Count; i++)
                                {
                                    jsonObject[i].Status = FileStatus.New;
                                }
                            }
                            var modifiedJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
                            npdDetailSection.ProductImage = modifiedJsonString;
                        }

                        if (!string.IsNullOrEmpty(npdDetailSection.TestReportsAttachment) || npdDetailSection.TestReportsAttachment != null)
                        {
                            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(npdDetailSection.TestReportsAttachment);
                            if (jsonObject != null)
                            {
                                for (int i = 0; i < ((Newtonsoft.Json.Linq.JContainer)(jsonObject)).Count; i++)
                                {
                                    jsonObject[i].Status = FileStatus.New;
                                }
                            }
                            var modifiedJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
                            npdDetailSection.TestReportsAttachment = modifiedJsonString;
                        }

                        this.SetTranListintoTempData<NPDProductFeature>(npdDetailSection.NPDProductFeature, TempKeys.NPDProductFeature.ToString() + "_" + id);
                        this.SetTranListintoTempData<NPDProductCompetitor>(npdDetailSection.NPDProductCompetitor, TempKeys.NPDProductCompetitor.ToString() + "_" + id);
                        this.SetTranListintoTempData<NPDMarketInformation>(npdDetailSection.NPDMarketInformation, TempKeys.NPDMarketInformation.ToString() + "_" + id);
                        this.SetTranListintoTempData<NPDCumulativeSalesProjection>(npdDetailSection.NPDCumulativeSalesProjection, TempKeys.NPDCumulativeSalesProjection.ToString() + "_" + id);
                        this.SetTranListintoTempData<NPDTargetRLP>(npdDetailSection.NPDTargetRLP, TempKeys.NPDTargetRLP.ToString() + "_" + id);
                    }
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.NPDAPPROVER1SECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.NPDAPPROVER2SECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.NPDAPPROVER3SECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.NPDABSADMINSECTION));

                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == SectionNameConstant.APPLICATIONSTATUS));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.ACTIVITYLOG));
                }
                else
                {
                    return this.RedirectToAction("NotAuthorize", "Master");
                }
                return this.View(contract);
            }
            else if (urlAction == Convert.ToInt16(URLAction.COPY))
            {
                if (contract != null && contract.Forms != null && contract.Forms.Count != 0)
                {
                    contract.Forms[0].FormStatus = string.Empty;
                    contract.Forms[0].Buttons = new List<Button>();

                    Button btn = new Button();
                    btn.Name = ButtonCaption.SAVEASDRAFTCOPY;
                    btn.ToolTip = "Copy request will be saved as draft mode and pending with you only.";
                    btn.ButtonStatus = ButtonActionStatus.SaveAsDraft;
                    btn.JsFunction = "SubmitNoRedirect";
                    btn.IsVisible = true;
                    btn.Icon = "fa fa-save";
                    contract.Forms[0].Buttons.Add(btn);

                    btn = new Button();
                    btn.Name = ButtonCaption.SUBMITCOPY;
                    btn.ToolTip = "Copy request will move to the next approval level.";
                    btn.ButtonStatus = ButtonActionStatus.NextApproval;
                    btn.JsFunction = "ConfirmSubmit";
                    btn.IsVisible = true;
                    btn.Icon = "fa fa-save";
                    contract.Forms[0].Buttons.Add(btn);

                    var npdDetailSection = contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.NPDDETAILSECTION) as NPDDetailSection;
                    if (npdDetailSection != null)
                    {
                        ////npdDetailSection.OldProjectNo = npdDetailSection.ProjectNo;
                        ////npdDetailSection.OldNPDCreatedDate = npdDetailSection.RequestDate;
                        ////dcrDetailSection.OldDCRRejectedDate = dcrDetailSection.DCRRejectedDate;
                        ////npdDetailSection.OldNPDId = id;
                        npdDetailSection.ProposedBy = CurrentUser.UserId;
                        npdDetailSection.ProposedByName = CurrentUser.FullName;
                        npdDetailSection.ProjectNo = string.Empty;
                        npdDetailSection.RequestDate = null;
                        npdDetailSection.WorkflowStatus = string.Empty;
                        npdDetailSection.IsActive = true;
                        npdDetailSection.ListDetails[0].ItemId = 0;
                        npdDetailSection.CurrentApprover.Approver = CurrentUser.UserId;
                        npdDetailSection.CurrentApprover.ApproverName = CurrentUser.FullName;
                        npdDetailSection.CopyFrom = id;
                        //  dcrDetailSection.ApproversList[0].Approver = dcrProcessIcSection.ApproversList[0].Approver;

                        if (npdDetailSection.NPDProductFeature != null && npdDetailSection.NPDProductFeature.Count > 0)
                        {
                            npdDetailSection.NPDProductFeature = npdDetailSection.NPDProductFeature.Select(c => { c.ID = 0; c.RequestID = 0; c.ItemAction = ItemActionStatus.NEW; return c; }).ToList();
                            this.SetTranListintoTempData<NPDProductFeature>(npdDetailSection.NPDProductFeature, TempKeys.NPDProductFeature.ToString() + "_0");
                        }
                        if (npdDetailSection.NPDProductCompetitor != null && npdDetailSection.NPDProductCompetitor.Count > 0)
                        {
                            npdDetailSection.NPDProductCompetitor = npdDetailSection.NPDProductCompetitor.Select(c => { c.ID = 0; c.RequestID = 0; c.ItemAction = ItemActionStatus.NEW; return c; }).ToList();
                            this.SetTranListintoTempData<NPDProductCompetitor>(npdDetailSection.NPDProductCompetitor, TempKeys.NPDProductCompetitor.ToString() + "_0");
                        }
                        if (npdDetailSection.NPDMarketInformation != null && npdDetailSection.NPDMarketInformation.Count > 0)
                        {
                            npdDetailSection.NPDMarketInformation = npdDetailSection.NPDMarketInformation.Select(c => { c.ID = 0; c.RequestID = 0; c.ItemAction = ItemActionStatus.NEW; return c; }).ToList();
                            this.SetTranListintoTempData<NPDMarketInformation>(npdDetailSection.NPDMarketInformation, TempKeys.NPDMarketInformation.ToString() + "_0");
                        }
                        if (npdDetailSection.NPDCumulativeSalesProjection != null && npdDetailSection.NPDCumulativeSalesProjection.Count > 0)
                        {
                            npdDetailSection.NPDCumulativeSalesProjection = npdDetailSection.NPDCumulativeSalesProjection.Select(c => { c.ID = 0; c.ItemAction = ItemActionStatus.NEW; c.RequestID = 0; return c; }).ToList();
                            this.SetTranListintoTempData<NPDCumulativeSalesProjection>(npdDetailSection.NPDCumulativeSalesProjection, TempKeys.NPDCumulativeSalesProjection.ToString() + "_0");
                        }
                        if (npdDetailSection.NPDTargetRLP != null && npdDetailSection.NPDTargetRLP.Count > 0)
                        {
                            npdDetailSection.NPDTargetRLP = npdDetailSection.NPDTargetRLP.Select(c => { c.ID = 0; c.ItemAction = ItemActionStatus.NEW; c.RequestID = 0; return c; }).ToList();
                            this.SetTranListintoTempData<NPDTargetRLP>(npdDetailSection.NPDTargetRLP, TempKeys.NPDTargetRLP.ToString() + "_" + id);
                        }

                        if (npdDetailSection.Files != null && npdDetailSection.Files.Count > 0)
                        {
                            npdDetailSection.Files = npdDetailSection.Files.Select(c => { c.Status = FileStatus.New; return c; }).ToList();
                        }
                        npdDetailSection.FileNameList = npdDetailSection.Files != null && npdDetailSection.Files.Count > 0 ? JsonConvert.SerializeObject(npdDetailSection.Files) : string.Empty;
                        this.SetTranListintoTempData<NPDProductFeature>(npdDetailSection.NPDProductFeature, TempKeys.NPDProductFeature.ToString() + "_" + id);
                        this.SetTranListintoTempData<NPDProductCompetitor>(npdDetailSection.NPDProductCompetitor, TempKeys.NPDProductCompetitor.ToString() + "_" + id);
                        this.SetTranListintoTempData<NPDMarketInformation>(npdDetailSection.NPDMarketInformation, TempKeys.NPDMarketInformation.ToString() + "_" + id);
                        this.SetTranListintoTempData<NPDCumulativeSalesProjection>(npdDetailSection.NPDCumulativeSalesProjection, TempKeys.NPDCumulativeSalesProjection.ToString() + "_" + id);
                        this.SetTranListintoTempData<NPDTargetRLP>(npdDetailSection.NPDTargetRLP, TempKeys.NPDTargetRLP.ToString() + "_" + id);
                    }
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.NPDAPPROVER1SECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.NPDAPPROVER2SECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.NPDAPPROVER3SECTION));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.NPDABSADMINSECTION));

                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == SectionNameConstant.APPLICATIONSTATUS));
                    contract.Forms[0].SectionsList.Remove(contract.Forms[0].SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.ACTIVITYLOG));
                }
                else
                {
                    return this.RedirectToAction("NotAuthorize", "Master");
                }
                return this.View(contract);
            }
            else
            {
                return this.RedirectToAction("NotAuthorize", "Master");
            }
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
        public ActionResult SaveNPDDetailSection(NPDDetailSection model)
        {
            ActionStatus status = new ActionStatus();
            if (model != null && !string.IsNullOrWhiteSpace(model.Channels))
            {
                if (!(model.Channels.Contains("Others")))
                {
                    ModelState.Remove("OtherChannels");
                }
            }
            if (model != null && model.SampleRequired == false)
            {
                ModelState.Remove("CreatorAttachment");
            }

            if (model != null && this.ValidateModelState(model))
            {
                if (model.ApproversList != null)
                {
                    model.ABSAdmin = model.ApproversList.FirstOrDefault(p => p.Role == NPDRoles.ABSADMIN).Approver;
                    model.ABSAdminName = model.ApproversList.FirstOrDefault(p => p.Role == NPDRoles.ABSADMIN).ApproverName;
                }
                model.Files = new List<FileDetails>();
                model.Files.AddRange(FileListHelper.GenerateFileBytes(model.CreatorAttachment));  //For Save Attachemennt
                model.CreatorAttachment = string.Join(",", FileListHelper.GetFileNames(model.CreatorAttachment));
                if (!string.IsNullOrWhiteSpace(model.BusinessUnit) && (model.BusinessUnit.ToUpper() == "CP-LTG" || model.BusinessUnit.ToUpper() == "LUM"))
                {
                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.ProductImage));  //For Save Attachemennt
                    model.ProductImage = string.Join(",", FileListHelper.GetFileNames(model.ProductImage));

                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.SpecificationSheet));  //For Save Attachemennt
                    model.SpecificationSheet = string.Join(",", FileListHelper.GetFileNames(model.SpecificationSheet));

                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.TechnicalDataSheet));  //For Save Attachemennt
                    model.TechnicalDataSheet = string.Join(",", FileListHelper.GetFileNames(model.TechnicalDataSheet));

                    model.Files.AddRange(FileListHelper.GenerateFileBytes(model.TestReportsAttachment));  //For Save Attachemennt
                    model.TestReportsAttachment = string.Join(",", FileListHelper.GetFileNames(model.TestReportsAttachment));
                }
                //for Add Product Feature Grid Data to Model
                var list = this.GetTempData<List<NPDProductFeature>>(TempKeys.NPDProductFeature.ToString() + "_" + GetFormIdFromUrl());
                if (model.ActionStatus == ButtonActionStatus.NextApproval && (list == null || list.Count == 0 || !list.Any(m => m.ItemAction != ItemActionStatus.DELETED) || (!IsValidProductFeature(list))))
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
                if (model.ActionStatus == ButtonActionStatus.NextApproval && (cumulativesalesprojection == null || cumulativesalesprojection.Count == 0 || !cumulativesalesprojection.Any(m => m.ItemAction != ItemActionStatus.DELETED) || (!IsValidCumulativeSalesProjection(cumulativesalesprojection))))
                {
                    status.IsSucceed = false;
                    status.Messages = new List<string>() { this.GetResourceValue("Text_CumulativeSalesProjectionRequired", System.Web.Mvc.Html.ResourceNames.NPD) };
                    return this.Json(status);
                }
                model.NPDCumulativeSalesProjection = cumulativesalesprojection.ToList<ITrans>();

                //for Add Cumulative Sales Projection Grid Data to Model
                var targetRLP = this.GetTempData<List<NPDTargetRLP>>(TempKeys.NPDTargetRLP.ToString() + "_" + GetFormIdFromUrl());
                model.NPDTargetRLP = targetRLP.ToList<ITrans>();

                Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId, model.ActionStatus.ToString(), model.ButtonCaption);
                status = this.SaveSection(model, objDict);
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

        #region "Save NPD Approver1 Section"
        //[HttpPost, ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveApprover1Section(NPDApprover1Section model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null && this.ValidateModelState(model))
            {
                Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId,
                    model.ActionStatus.ToString(), model.ButtonCaption);
                model.Files = new List<FileDetails>();
                model.Files.AddRange(FileListHelper.GenerateFileBytes(model.Approver1Attachment));
                model.Approver1Attachment =
                    string.Join(",", FileListHelper.GetFileNames(model.Approver1Attachment));
                if (!string.IsNullOrEmpty(model.SendBackTo))
                {
                    objDict[Parameter.SENDTOLEVEL] = model.SendBackTo;
                }

                if (model.ActionStatus == ButtonActionStatus.Rejected)
                {
                    model.OldNPDRejectedDate = DateTime.Now;
                }
                status = this.SaveSection(model, objDict);
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

        #region "Save NPD Approver2 Section"
        //[HttpPost, ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveApprover2Section(NPDApprover2Section model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null && this.ValidateModelState(model))
            {
                Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId,
                    model.ActionStatus.ToString(), model.ButtonCaption);
                model.Files = new List<FileDetails>();
                model.Files.AddRange(FileListHelper.GenerateFileBytes(model.Approver2Attachment));
                model.Approver2Attachment =
                    string.Join(",", FileListHelper.GetFileNames(model.Approver2Attachment));

                if (!string.IsNullOrEmpty(model.SendBackTo))
                {
                    objDict[Parameter.SENDTOLEVEL] = model.SendBackTo;
                }

                if (model.ActionStatus == ButtonActionStatus.Rejected)
                {
                    model.OldNPDRejectedDate = DateTime.Now;
                }
                status = this.SaveSection(model, objDict);
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

        #region "Save NPD Approver3 Section"
        //[HttpPost, ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveApprover3Section(NPDApprover3Section model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null && this.ValidateModelState(model))
            {
                Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId,
                    model.ActionStatus.ToString(), model.ButtonCaption);
                model.Files = new List<FileDetails>();
                model.Files.AddRange(FileListHelper.GenerateFileBytes(model.Approver3Attachment));
                model.Approver3Attachment =
                    string.Join(",", FileListHelper.GetFileNames(model.Approver3Attachment));
                if (!string.IsNullOrEmpty(model.SendBackTo))
                {
                    objDict[Parameter.SENDTOLEVEL] = model.SendBackTo;
                }

                if (model.ActionStatus == ButtonActionStatus.Rejected)
                {
                    model.OldNPDRejectedDate = DateTime.Now;
                }
                status = this.SaveSection(model, objDict);
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

        #region "Save NPD ABS Admin Section"
        //[HttpPost, ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveABSAdminSection(NPDABSAdminSection model)
        {
            ActionStatus status = new ActionStatus();

            if (model != null && this.ValidateModelState(model))
            {
                Dictionary<string, string> objDict = this.GetSaveDataDictionary(this.CurrentUser.UserId,
                    model.ActionStatus.ToString(), model.ButtonCaption);

                model.Files = new List<FileDetails>();
                model.Files.AddRange(FileListHelper.GenerateFileBytes(model.ABSAdminAttachment));
                model.ABSAdminAttachment = string.Join(",", FileListHelper.GetFileNames(model.ABSAdminAttachment));
                if (!string.IsNullOrEmpty(model.SendBackTo))
                {
                    objDict[Parameter.SENDTOLEVEL] = model.SendBackTo;
                }

                if (model.ActionStatus == ButtonActionStatus.Rejected)
                {
                    model.OldNPDRejectedDate = DateTime.Now;
                }
                status = this.SaveSection(model, objDict);
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


        #region "CRUD Product Feature Detail"

        /// <summary>
        /// Adds the edit Product Feature item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult AddProductFeature(int index = 0)
        {
            List<NPDProductFeature> list = new List<NPDProductFeature>();
            list = this.GetTempData<List<NPDProductFeature>>(TempKeys.NPDProductFeature.ToString() + "_" + GetFormIdFromUrl());
            NPDProductFeature item = null;
            if (index == 0)
            {
                item = new NPDProductFeature() { Index = 0, RequestDate = DateTime.Now, RequestBy = this.CurrentUser.UserId };
            }
            else
            {
                item = list.FirstOrDefault(x => x.Index == index);
            }
            return this.PartialView("_AddProductFeature", item);
        }

        /// <summary>
        /// Saves the  Product Feature detail item.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Action Result</returns>
        //[HttpPost, ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveProductFeature(NPDProductFeature model)
        {

            ActionStatus status = new ActionStatus() { IsSucceed = true };
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    List<NPDProductFeature> list = new List<NPDProductFeature>();
                    list = this.GetTempData<List<NPDProductFeature>>(TempKeys.NPDProductFeature.ToString() + "_" + GetFormIdFromUrl());
                    if (model.Index == 0)
                    {
                        model.Index = list.Count + 1;
                        model.ItemAction = ItemActionStatus.NEW;
                    }
                    else
                    {
                        list.RemoveAll(x => x.Index == model.Index);
                    }
                    if (model.ID > 0)
                    {
                        model.ItemAction = ItemActionStatus.UPDATED;
                    }
                    list.Add(model);
                    this.SetTempData<List<NPDProductFeature>>(TempKeys.NPDProductFeature.ToString() + "_" + GetFormIdFromUrl(), list.OrderBy(x => x.Index).ToList());
                    status.Messages.Add(this.GetResourceValue("Text_NPDProductFeatureSave", System.Web.Mvc.Html.ResourceNames.NPD));
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.NPD);

                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.NPD);
                }
            }
            return this.Json(status);
        }

        /// <summary>
        /// Gets the  Product Feature details grid.
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetProductFeatureGrid()
        {
            List<NPDProductFeature> list = new List<NPDProductFeature>();
            list = this.GetTempData<List<NPDProductFeature>>(TempKeys.NPDProductFeature.ToString() + "_" + GetFormIdFromUrl()).Where(x => x.ItemAction != ItemActionStatus.DELETED).ToList();
            return this.PartialView("_ProductFeatureGrid", list.ToList<ITrans>());
        }

        /// <summary>
        /// Deletes the Product Feature detail.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        public ActionResult DeleteProductFeature(int index)
        {

            ActionStatus status = new ActionStatus() { IsSucceed = true };
            List<NPDProductFeature> list = new List<NPDProductFeature>();
            list = this.GetTempData<List<NPDProductFeature>>(TempKeys.NPDProductFeature.ToString() + "_" + GetFormIdFromUrl());
            NPDProductFeature item = list.FirstOrDefault(x => x.Index == index);
            list.RemoveAll(x => x.Index == index);
            if (item != null && item.ID > 0)
            {
                item.ItemAction = ItemActionStatus.DELETED;
                list.Add(item);
            }
            this.SetTempData<List<NPDProductFeature>>(TempKeys.NPDProductFeature, list.OrderBy(x => x.Index).ToList());
            status.Messages.Add(this.GetResourceValue("Text_ProductFeatureDeleted", System.Web.Mvc.Html.ResourceNames.NPD));
            return this.Json(status, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "CRUD Competitor Information Of Category"

        /// <summary>
        /// Adds the edit Product Feature item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult AddProductCompetitor(int index = 0)
        {
            List<NPDProductCompetitor> list = new List<NPDProductCompetitor>();
            list = this.GetTempData<List<NPDProductCompetitor>>(TempKeys.NPDProductCompetitor.ToString() + "_" + GetFormIdFromUrl());
            NPDProductCompetitor item = null;
            if (index == 0)
            {
                item = new NPDProductCompetitor() { Index = 0, RequestDate = DateTime.Now, RequestBy = this.CurrentUser.UserId };
            }
            else
            {
                item = list.FirstOrDefault(x => x.Index == index);
            }
            return this.PartialView("_AddProductCompetitor", item);
        }

        /// <summary>
        /// Saves the  Product Feature detail item.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Action Result</returns>
        //[HttpPost, ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveProductCompetitor(NPDProductCompetitor model)
        {

            ActionStatus status = new ActionStatus() { IsSucceed = true };
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    List<NPDProductCompetitor> list = new List<NPDProductCompetitor>();
                    list = this.GetTempData<List<NPDProductCompetitor>>(TempKeys.NPDProductCompetitor.ToString() + "_" + GetFormIdFromUrl());
                    if (model.Index == 0)
                    {
                        model.Index = list.Count + 1;
                        model.ItemAction = ItemActionStatus.NEW;
                    }
                    else
                    {
                        list.RemoveAll(x => x.Index == model.Index);
                    }
                    if (model.ID > 0)
                    {
                        model.ItemAction = ItemActionStatus.UPDATED;
                    }
                    list.Add(model);
                    this.SetTempData<List<NPDProductCompetitor>>(TempKeys.NPDProductCompetitor.ToString() + "_" + GetFormIdFromUrl(), list.OrderBy(x => x.Index).ToList());
                    status.Messages.Add(this.GetResourceValue("Text_NPDProductCompetitorSave", System.Web.Mvc.Html.ResourceNames.NPD));
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.NPD);

                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.NPD);
                }
            }
            return this.Json(status);
        }

        /// <summary>
        /// Gets the  Product Feature details grid.
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetProductCompetitorGrid()
        {
            List<NPDProductCompetitor> list = new List<NPDProductCompetitor>();
            list = this.GetTempData<List<NPDProductCompetitor>>(TempKeys.NPDProductCompetitor.ToString() + "_" + GetFormIdFromUrl()).Where(x => x.ItemAction != ItemActionStatus.DELETED).ToList();
            return this.PartialView("_ProductCompetitorInformationGrid", list.ToList<ITrans>());
        }

        /// <summary>
        /// Deletes the Product Feature detail.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        public ActionResult DeleteProductCompetitor(int index)
        {

            ActionStatus status = new ActionStatus() { IsSucceed = true };
            List<NPDProductCompetitor> list = new List<NPDProductCompetitor>();
            list = this.GetTempData<List<NPDProductCompetitor>>(TempKeys.NPDProductCompetitor.ToString() + "_" + GetFormIdFromUrl());
            NPDProductCompetitor item = list.FirstOrDefault(x => x.Index == index);
            list.RemoveAll(x => x.Index == index);
            if (item != null && item.ID > 0)
            {
                item.ItemAction = ItemActionStatus.DELETED;
                list.Add(item);
            }
            this.SetTempData<List<NPDProductCompetitor>>(TempKeys.NPDProductCompetitor, list.OrderBy(x => x.Index).ToList());
            status.Messages.Add(this.GetResourceValue("Text_NPDProductCompetitorDeleted", System.Web.Mvc.Html.ResourceNames.NPD));
            return this.Json(status, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region "CRUD Cumulative Sales Projection"

        /// <summary>
        /// Adds the edit Product Feature item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult AddCumulativeSalesProjection(int index = 0)
        {
            List<NPDCumulativeSalesProjection> list = new List<NPDCumulativeSalesProjection>();
            list = this.GetTempData<List<NPDCumulativeSalesProjection>>(TempKeys.NPDCumulativeSalesProjection.ToString() + "_" + GetFormIdFromUrl());
            NPDCumulativeSalesProjection item = null;
            if (index == 0)
            {
                item = new NPDCumulativeSalesProjection() { Index = 0, RequestDate = DateTime.Now, RequestBy = this.CurrentUser.UserId };
            }
            else
            {
                item = list.FirstOrDefault(x => x.Index == index);
            }
            return this.PartialView("_AddCumulativeSalesProjection", item);
        }

        /// <summary>
        /// Saves the  Product Feature detail item.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Action Result</returns>
        //[HttpPost, ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveCumulativeSalesProjection(NPDCumulativeSalesProjection model)
        {

            ActionStatus status = new ActionStatus() { IsSucceed = true };
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    List<NPDCumulativeSalesProjection> list = new List<NPDCumulativeSalesProjection>();
                    list = this.GetTempData<List<NPDCumulativeSalesProjection>>(TempKeys.NPDCumulativeSalesProjection.ToString() + "_" + GetFormIdFromUrl());
                    if (model.Index == 0)
                    {
                        model.Index = list.Count + 1;
                        model.ItemAction = ItemActionStatus.NEW;
                    }
                    else
                    {
                        list.RemoveAll(x => x.Index == model.Index);
                    }
                    if (model.ID > 0)
                    {
                        model.ItemAction = ItemActionStatus.UPDATED;
                    }
                    list.Add(model);
                    this.SetTempData<List<NPDCumulativeSalesProjection>>(TempKeys.NPDCumulativeSalesProjection.ToString() + "_" + GetFormIdFromUrl(), list.OrderBy(x => x.Index).ToList());
                    status.Messages.Add(this.GetResourceValue("Text_CumulativeSalesProjectionSave", System.Web.Mvc.Html.ResourceNames.NPD));
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.NPD);

                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.NPD);
                }
            }
            return this.Json(status);
        }

        /// <summary>
        /// Gets the  Product Feature details grid.
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetCumulativeSalesProjectionGrid()
        {
            List<NPDCumulativeSalesProjection> list = new List<NPDCumulativeSalesProjection>();
            list = this.GetTempData<List<NPDCumulativeSalesProjection>>(TempKeys.NPDCumulativeSalesProjection.ToString() + "_" + GetFormIdFromUrl()).Where(x => x.ItemAction != ItemActionStatus.DELETED).ToList();
            return this.PartialView("_CumulativeSalesProjectionGrid", list.ToList<ITrans>());
        }

        /// <summary>
        /// Deletes the Product Feature detail.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        public ActionResult DeleteCumulativeSalesProjection(int index)
        {

            ActionStatus status = new ActionStatus() { IsSucceed = true };
            List<NPDCumulativeSalesProjection> list = new List<NPDCumulativeSalesProjection>();
            list = this.GetTempData<List<NPDCumulativeSalesProjection>>(TempKeys.NPDCumulativeSalesProjection.ToString() + "_" + GetFormIdFromUrl());
            NPDCumulativeSalesProjection item = list.FirstOrDefault(x => x.Index == index);
            list.RemoveAll(x => x.Index == index);
            if (item != null && item.ID > 0)
            {
                item.ItemAction = ItemActionStatus.DELETED;
                list.Add(item);
            }
            this.SetTempData<List<NPDCumulativeSalesProjection>>(TempKeys.NPDCumulativeSalesProjection, list.OrderBy(x => x.Index).ToList());
            status.Messages.Add(this.GetResourceValue("Text_CumulativeSalesProjectionDeleted", System.Web.Mvc.Html.ResourceNames.NPD));
            return this.Json(status, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "CRUD Target RLP"

        /// <summary>
        /// Adds the edit Product Feature item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult AddTargetRLP(int index = 0)
        {
            List<NPDTargetRLP> list = new List<NPDTargetRLP>();
            list = this.GetTempData<List<NPDTargetRLP>>(TempKeys.NPDTargetRLP.ToString() + "_" + GetFormIdFromUrl());
            NPDTargetRLP item = null;
            if (index == 0)
            {
                item = new NPDTargetRLP() { Index = 0, RequestDate = DateTime.Now, RequestBy = this.CurrentUser.UserId };
            }
            else
            {
                item = list.FirstOrDefault(x => x.Index == index);
            }
            return this.PartialView("_AddTargetRLP", item);
        }

        /// <summary>
        /// Saves the  Product Feature detail item.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Action Result</returns>
        //[HttpPost, ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveTargetRLP(NPDTargetRLP model)
        {

            ActionStatus status = new ActionStatus() { IsSucceed = true };
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    List<NPDTargetRLP> list = new List<NPDTargetRLP>();
                    list = this.GetTempData<List<NPDTargetRLP>>(TempKeys.NPDTargetRLP.ToString() + "_" + GetFormIdFromUrl());
                    if (model.Index == 0)
                    {
                        model.Index = list.Count + 1;
                        model.ItemAction = ItemActionStatus.NEW;
                    }
                    else
                    {
                        list.RemoveAll(x => x.Index == model.Index);
                    }
                    if (model.ID > 0)
                    {
                        model.ItemAction = ItemActionStatus.UPDATED;
                    }
                    list.Add(model);
                    this.SetTempData<List<NPDTargetRLP>>(TempKeys.NPDTargetRLP.ToString() + "_" + GetFormIdFromUrl(), list.OrderBy(x => x.Index).ToList());
                    status.Messages.Add(this.GetResourceValue("Text_NPDTargetRLPSave", System.Web.Mvc.Html.ResourceNames.NPD));
                    status = this.GetMessage(status, System.Web.Mvc.Html.ResourceNames.NPD);

                }
                else
                {
                    status.IsSucceed = false;
                    status.Messages = this.GetErrorMessage(System.Web.Mvc.Html.ResourceNames.NPD);
                }
            }
            return this.Json(status);
        }

        /// <summary>
        /// Gets the  Product Feature details grid.
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GetTargetRLPGrid()
        {
            List<NPDTargetRLP> list = new List<NPDTargetRLP>();
            list = this.GetTempData<List<NPDTargetRLP>>(TempKeys.NPDTargetRLP.ToString() + "_" + GetFormIdFromUrl()).Where(x => x.ItemAction != ItemActionStatus.DELETED).ToList();
            return this.PartialView("_TargetRLPGrid", list.ToList<ITrans>());
        }

        /// <summary>
        /// Deletes the Product Feature detail.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Action Result</returns>
        [HttpPost]
        public ActionResult DeleteTargetRLP(int index)
        {

            ActionStatus status = new ActionStatus() { IsSucceed = true };
            List<NPDTargetRLP> list = new List<NPDTargetRLP>();
            list = this.GetTempData<List<NPDTargetRLP>>(TempKeys.NPDTargetRLP.ToString() + "_" + GetFormIdFromUrl());
            NPDTargetRLP item = list.FirstOrDefault(x => x.Index == index);
            list.RemoveAll(x => x.Index == index);
            if (item != null && item.ID > 0)
            {
                item.ItemAction = ItemActionStatus.DELETED;
                list.Add(item);
            }
            this.SetTempData<List<NPDTargetRLP>>(TempKeys.NPDTargetRLP, list.OrderBy(x => x.Index).ToList());
            status.Messages.Add(this.GetResourceValue("Text_NPDTargetRLPDeleted", System.Web.Mvc.Html.ResourceNames.NPD));
            return this.Json(status, JsonRequestBehavior.AllowGet);
        }

        #endregion

        private bool IsValidProductFeature(List<NPDProductFeature> lst)
        {
            bool Isvalid = true;
            if (lst.Count > 0)
            {
                if (lst.FindAll(x => x.ProductFeature == "Mandatory" && x.ItemAction != ItemActionStatus.DELETED).Count == 0 || lst.FindAll(x => x.ProductFeature == "Should not have" && x.ItemAction != ItemActionStatus.DELETED).Count == 0||lst.FindAll(x=>x.ProductFeature== "Preferred" && x.ItemAction!=ItemActionStatus.DELETED).Count==0)
                {
                    Isvalid = false;
                }
            }
            else
                Isvalid = false;

            return Isvalid;
        }
        private bool IsValidCumulativeSalesProjection(List<NPDCumulativeSalesProjection> lst)
        {
            bool Isvalid = true;
            if (lst.Count > 0)
            {
                if (lst.FindAll(x => x.Periods == "6 months" && x.ItemAction != ItemActionStatus.DELETED).Count == 0 || lst.FindAll(x => x.Periods == "12 months" && x.ItemAction != ItemActionStatus.DELETED).Count == 0 || lst.FindAll(x => x.Periods == "24 months" && x.ItemAction != ItemActionStatus.DELETED).Count == 0)
                {
                    Isvalid = false;
                }
            }
            else
                Isvalid = false;

            return Isvalid;
        }

        [HttpPost]
        public JsonResult KeepSessionAlive()
        {
            return new JsonResult { Data = "Success" };
        }





    }
}