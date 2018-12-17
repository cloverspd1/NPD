namespace NPD.NWorkflow.BusinessLayer
{
    using CommonDataContract;
    using NPD.Workflow.Models.NPD;
    using NPD.Workflow.Models.Common;
    using NPD.DataAccessLayer;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Workflow.Models;
    using Newtonsoft.Json;

    public class NPDBusinessLayer
    {
        private static readonly Lazy<NPDBusinessLayer> lazy =
         new Lazy<NPDBusinessLayer>(() => new NPDBusinessLayer());

        public static NPDBusinessLayer Instance { get { return lazy.Value; } }

        // <summary>
        /// The padlock
        /// </summary>
        private static readonly object Padlock = new object();

        /// <summary>
        /// The context
        /// </summary>
        private ClientContext context = null;

        /// <summary>
        /// The web
        /// </summary>
        private Web web = null;

        private NPDBusinessLayer()
        {
            string siteURL = BELDataAccessLayer.Instance.GetSiteURL(SiteURLs.NPDSITEURL);
            if (!string.IsNullOrEmpty(siteURL))
            {
                if (this.context == null)
                {
                    this.context = BELDataAccessLayer.Instance.CreateClientContext(siteURL);
                }
                if (this.web == null)
                {
                    this.web = BELDataAccessLayer.Instance.CreateWeb(this.context);
                }
            }
        }

        #region "GET DATA"
        /// <summary>
        /// Gets the NPD details.
        /// </summary>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>byte array</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "test")]
        public NPDContract GetNPDDetails(IDictionary<string, string> objDict)
        {
            NPDContract contract = new NPDContract();
            if (objDict != null && objDict.ContainsKey(Parameter.FROMNAME) && objDict.ContainsKey(Parameter.ITEMID) && objDict.ContainsKey(Parameter.USEREID))
            {
                string formName = objDict[Parameter.FROMNAME];
                int itemId = Convert.ToInt32(objDict[Parameter.ITEMID]);
                string userID = objDict[Parameter.USEREID];
                IForm npdForm = new NPDForm(true);
                npdForm = BELDataAccessLayer.Instance.GetFormData(this.context, this.web, ApplicationNameConstants.NPDAPP, formName, itemId, userID, npdForm);
                if (npdForm != null && npdForm.SectionsList != null && npdForm.SectionsList.Count > 0)
                {
                    var sectionDetails = npdForm.SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.NPDDETAILSECTION) as NPDDetailSection;
                    var approvalMatrix = npdForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(SectionNameConstant.APPLICATIONSTATUS)) as ApplicationStatusSection;
                    if (sectionDetails != null)
                    {
                        if (itemId == 0)
                        {

                        }
                        else
                        {
                            sectionDetails.ApproversList.Remove(sectionDetails.ApproversList.FirstOrDefault(p => p.Role == UserRoles.VIEWER));
                        }

                        sectionDetails.CreatorAttachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.CreatorAttachment) && sectionDetails.CreatorAttachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;

                        var approver1Section = npdForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(NPDSectionName.NPDAPPROVER1SECTION)) as NPDApprover1Section;
                        if (approver1Section != null)
                        {
                            approver1Section.Approver1Attachment = approver1Section.Files != null && approver1Section.Files.Count > 0 ? JsonConvert.SerializeObject(approver1Section.Files.Where(x => !string.IsNullOrEmpty(approver1Section.Approver1Attachment) && approver1Section.Approver1Attachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        }

                        var approver2Section = npdForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(NPDSectionName.NPDAPPROVER2SECTION)) as NPDApprover2Section;
                        if (approver2Section != null)
                        {
                            approver2Section.Approver2Attachment = approver2Section.Files != null && approver2Section.Files.Count > 0 ? JsonConvert.SerializeObject(approver2Section.Files.Where(x => !string.IsNullOrEmpty(approver2Section.Approver2Attachment) && approver2Section.Approver2Attachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        }

                        var approver3Section = npdForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(NPDSectionName.NPDAPPROVER3SECTION)) as NPDApprover3Section;
                        if (approver3Section != null)
                        {
                            approver3Section.Approver3Attachment = approver3Section.Files != null && approver3Section.Files.Count > 0 ? JsonConvert.SerializeObject(approver3Section.Files.Where(x => !string.IsNullOrEmpty(approver3Section.Approver3Attachment) && approver3Section.Approver3Attachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        }

                        var absAdminSection = npdForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(NPDSectionName.NPDABSADMINSECTION)) as NPDABSAdminSection;
                        if (absAdminSection != null)
                        {
                            absAdminSection.ABSAdminAttachment = absAdminSection.Files != null && absAdminSection.Files.Count > 0 ? JsonConvert.SerializeObject(absAdminSection.Files.Where(x => !string.IsNullOrEmpty(absAdminSection.ABSAdminAttachment) && absAdminSection.ABSAdminAttachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        }

                        var stagegate1Section = npdForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(NPDSectionName.NPDSTAGEGATE1SECTION)) as NPDStageGate1;
                        if (stagegate1Section != null)
                        {
                            stagegate1Section.StageGate1Attachment = stagegate1Section.Files != null && stagegate1Section.Files.Count > 0 ? JsonConvert.SerializeObject(stagegate1Section.Files.Where(x => !string.IsNullOrEmpty(stagegate1Section.StageGate1Attachment) && stagegate1Section.StageGate1Attachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        }

                        var stagegate2Section = npdForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(NPDSectionName.NPDSTAGEGATE2SECTION)) as NPDStageGate2;
                        if (stagegate2Section != null)
                        {
                            stagegate2Section.StageGate2Attachment = stagegate2Section.Files != null && stagegate2Section.Files.Count > 0 ? JsonConvert.SerializeObject(stagegate2Section.Files.Where(x => !string.IsNullOrEmpty(stagegate2Section.StageGate2Attachment) && stagegate2Section.StageGate2Attachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        }

                        var stagegate3Section = npdForm.SectionsList.FirstOrDefault(f => f.SectionName.Equals(NPDSectionName.NPDSTAGEGATE3SECTION)) as NPDStageGate3;
                        if (stagegate3Section != null)
                        {
                            stagegate3Section.StageGate3Attachment = stagegate3Section.Files != null && stagegate3Section.Files.Count > 0 ? JsonConvert.SerializeObject(stagegate3Section.Files.Where(x => !string.IsNullOrEmpty(stagegate3Section.StageGate3Attachment) && stagegate3Section.StageGate3Attachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        }

                        if (!string.IsNullOrWhiteSpace(sectionDetails.BusinessUnit) && (sectionDetails.BusinessUnit.ToUpper() == "CP-LTG" || sectionDetails.BusinessUnit.ToUpper() == "LUM"))
                        {
                            sectionDetails.ProductImage = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.ProductImage) && sectionDetails.ProductImage.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                            sectionDetails.SpecificationSheet = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.SpecificationSheet) && sectionDetails.SpecificationSheet.Split(',').Contains(x.FileName)).ToList()) : string.Empty;

                            sectionDetails.TechnicalDataSheet = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.TechnicalDataSheet) && sectionDetails.TechnicalDataSheet.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                            sectionDetails.TestReportsAttachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.TestReportsAttachment) && sectionDetails.TestReportsAttachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        }

                        //if ((sectionDetails.Status == FormStatus.COMPLETED || sectionDetails.Status == FormStatus.REJECTED) && sectionDetails.ApproversList.Any(p => p.Role == NPDRoles.ABSADMIN && p.Approver != userID))
                        if ((sectionDetails.Status == FormStatus.COMPLETED || sectionDetails.Status == FormStatus.REJECTED) && !approvalMatrix.ApplicationStatusList.Any(p => p.Role == NPDRoles.STAGEGATE3 && !String.IsNullOrEmpty(p.Approver) && p.Approver.Split(',').Contains(userID.Trim())))
                        {
                            if (npdForm.Buttons.FirstOrDefault(p => p.Name == "Print") != null)
                            {
                                npdForm.Buttons.FirstOrDefault(p => p.Name == "Print").IsVisible = false;
                            }
                        }
                    }
                    contract.Forms.Add(npdForm);
                }
            }
            return contract;

        }

        #endregion

        #region "SAVE DATA"
        /// <summary>
        /// Saves the by section.
        /// </summary>
        /// <param name="sections">The sections.</param>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>return status</returns>
        public ActionStatus SaveBySection(ISection sectionDetails, Dictionary<string, string> objDict)
        {
            lock (Padlock)
            {
                ActionStatus status = new ActionStatus();
                NPDNoCount currentNPDNo = null;
                //BELDataAccessLayer helper = new BELDataAccessLayer();
                if (sectionDetails != null && objDict != null)
                {
                    objDict[Parameter.ACTIVITYLOG] = NPDListNames.NPDACTIVITYLOG;
                    objDict[Parameter.APPLICATIONNAME] = ApplicationNameConstants.NPDAPP;
                    objDict[Parameter.FROMNAME] = FormNameConstants.NPDFORM;
                    NPDDetailSection section = null;
                    if (sectionDetails.SectionName == NPDSectionName.NPDDETAILSECTION)
                    {
                        section = sectionDetails as NPDDetailSection;
                        if (sectionDetails.ActionStatus == ButtonActionStatus.NextApproval && string.IsNullOrEmpty(section.ProjectNo))
                        {
                            section.RequestDate = DateTime.Now;
                            currentNPDNo = GetNPDNo(section.BusinessUnit);
                            if (currentNPDNo != null)
                            {
                                currentNPDNo.CurrentValue = currentNPDNo.CurrentValue + 1;
                                Logger.Info("NPD Current Value + 1 = " + currentNPDNo.CurrentValue);
                                // section.ProjectNo = string.Format("NPD-{0}-{1}-{2}", currentNPDNo.BusinessUnit, DateTime.Today.Year, string.Format("{0:0000}", currentNPDNo.CurrentValue));
                                section.ProjectNo = string.Format("{0}-{1}-{2}-{3}-{4}", section.ProjectType, section.BusinessUnit, section.ProductCategory, section.RequestDate.Value.ToString("yyyyMMdd"), string.Format("{0:00000}", currentNPDNo.CurrentValue));
                                Logger.Info("NPD No is " + section.ProjectNo);
                                status.ExtraData = section.ProjectNo;
                            }
                        }

                    }
                    List<ListItemDetail> objSaveDetails = BELDataAccessLayer.Instance.SaveData(this.context, this.web, sectionDetails, objDict);
                    ListItemDetail itemDetails = objSaveDetails.Where(p => p.ListName.Equals(NPDListNames.NPDLIST)).FirstOrDefault<ListItemDetail>();
                    if (sectionDetails.SectionName == NPDSectionName.NPDDETAILSECTION)
                    {
                        if (!string.IsNullOrEmpty(section.OldProjectNo))
                        {
                            Dictionary<string, dynamic> values = new Dictionary<string, dynamic>();
                            values.Add("IsNPDRetrieved", true);
                            BELDataAccessLayer.Instance.SaveFormFields(this.context, this.web, NPDListNames.NPDLIST, section.OldNPDId, values);
                        }

                        if (itemDetails.ItemId > 0 && currentNPDNo != null)
                        {
                            UpdateNPDNoCount(currentNPDNo);
                            Logger.Info("Update DCR No " + section.ProjectNo);
                        }
                    }

                    if (itemDetails.ItemId > 0)
                    {
                        status.IsSucceed = true;
                        switch (sectionDetails.ButtonCaption.Trim())
                        {
                            case ButtonCaption.SAVEASDRAFT:
                            case ButtonCaption.SAVEASDRAFTCOPY:
                                status.Messages.Add("Text_SaveDraftSuccess");
                                status.ItemID = itemDetails.ItemId;
                                break;
                            case ButtonCaption.SUBMIT:
                            case ButtonCaption.SUBMITCOPY:
                                status.Messages.Add(ApplicationConstants.SUCCESSMESSAGE);
                                break;
                            case ButtonCaption.APPROVE:
                                if (sectionDetails.ActionStatus == ButtonActionStatus.Complete)
                                {
                                    status.Messages.Add("Text_CompletedSuccess");
                                }
                                else
                                {
                                    status.Messages.Add("Text_ApproveSuccess");
                                }
                                break;
                            case ButtonCaption.REWORK:
                                status.Messages.Add("Text_ReworkSuccess");
                                break;
                            case ButtonCaption.REJECT:
                                status.Messages.Add("Text_RejectedSuccess");
                                break;
                            case ButtonCaption.HOLD:
                                status.Messages.Add("Text_HoldSuccess");
                                status.ItemID = itemDetails.ItemId;
                                break;
                            case ButtonCaption.RESUME:
                                status.Messages.Add("Text_ResumeSuccess");
                                status.ItemID = itemDetails.ItemId;
                                break;
                            default:
                                status.Messages.Add(ApplicationConstants.SUCCESSMESSAGE);
                                break;
                        }
                    }
                    else
                    {
                        status.IsSucceed = false;
                        status.Messages.Add(ApplicationConstants.ERRORMESSAGE);
                    }

                }
                return status;
            }
        }

        /// <summary>
        /// Get NPD No Logic
        /// </summary>
        /// <param name="context">Site context</param>
        /// <param name="web">Web Object</param>
        /// <returns>NPD No</returns>
        public string GetNPDNo()
        {
            try
            {
                int dcrno = 0;
                List spList = this.web.Lists.GetByTitle(NPDListNames.NPDLIST);
                CamlQuery query = new CamlQuery();
                string startDate = (new DateTime(DateTime.Now.Year, 1, 1)).ToString("yyyy-MM-ddTHH:mm:ssZ");
                string endDate = (new DateTime(DateTime.Now.Year, 12, 31)).ToString("yyyy-MM-ddTHH:mm:ssZ");
                query.ViewXml = @"<View><Query>
                                      <Where>
                                        <And>
                                          <Geq>
                                            <FieldRef Name='Created' />
                                              <Value IncludeTimeValue='FALSE' Type='DateTime'>" + startDate + @"</Value>
                                          </Geq>
                                          <Leq>
                                            <FieldRef Name='Created' />
                                            <Value IncludeTimeValue='FALSE' Type='DateTime'>" + endDate + @"</Value>
                                          </Leq>
                                        </And>
                                      </Where>
                                    </Query>
                                         <ViewFields><FieldRef Name='ID' /></ViewFields>   </View>";
                //query.ViewXml = @"<View><ViewFields><FieldRef Name='ID' /></ViewFields></View>";
                ListItemCollection items = spList.GetItems(query);
                this.context.Load(items);
                this.context.ExecuteQuery();
                if (items != null && items.Count != 0)
                {
                    dcrno = items.Count;
                }
                return dcrno.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return string.Empty;
            }


        }

        /// <summary>
        /// Get NPD No Logic
        /// </summary>
        /// <param name="businessunit">Business Unit</param>
        /// <returns>NPD No Count</returns>
        public NPDNoCount GetNPDNo(string businessunit)
        {
            try
            {
                List<NPDNoCount> lstNPDCount = new List<NPDNoCount>();
                List spList = this.web.Lists.GetByTitle(NPDListNames.NPDNOCOUNT);
                CamlQuery query = new CamlQuery();
                query.ViewXml = @"<View><ViewFields><FieldRef Name='Title' /><FieldRef Name='Year' /><FieldRef Name='CurrentValue' /></ViewFields>   </View>";
                ListItemCollection items = spList.GetItems(query);
                this.context.Load(items);
                this.context.ExecuteQuery();
                if (items != null && items.Count != 0)
                {
                    foreach (ListItem item in items)
                    {
                        NPDNoCount obj = new NPDNoCount();
                        obj.ID = item.Id;
                        obj.BusinessUnit = Convert.ToString(item["Title"]);
                        obj.Year = Convert.ToInt32(item["Year"]);
                        obj.CurrentValue = Convert.ToInt32(item["CurrentValue"]);
                        if (obj.Year != DateTime.Today.Year)
                        {
                            obj.Year = DateTime.Today.Year;
                            obj.CurrentValue = 0;
                        }

                        lstNPDCount.Add(obj);
                    }
                }

                if (lstNPDCount != null)
                {
                    return lstNPDCount.FirstOrDefault(p => businessunit.Contains(p.BusinessUnit) && p.Year == DateTime.Today.Year);
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Update NPD No Count
        /// </summary>
        /// <param name="currentValue">Current Value</param>
        public void UpdateNPDNoCount(NPDNoCount currentValue)
        {
            if (currentValue != null && currentValue.ID != 0)
            {
                try
                {

                    Logger.Info("Aync update NPD Current value currentValue : " + currentValue.CurrentValue + " BusinessUnit : " + currentValue.BusinessUnit);
                    List spList = this.web.Lists.GetByTitle(NPDListNames.NPDNOCOUNT);
                    ListItem item = spList.GetItemById(currentValue.ID);
                    item["CurrentValue"] = currentValue.CurrentValue;
                    item["Year"] = currentValue.Year;
                    item.Update();
                    //context.Load(item);
                    context.ExecuteQuery();

                }
                catch (Exception ex)
                {
                    Logger.Error("Error while Update NPD no Current Value");
                    Logger.Error(ex);
                }
            }
        }
        #endregion

        #region "GET Admin DATA"
        /// <summary>
        /// Gets the NPD details.
        /// </summary>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>byte array</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "test")]
        public NPDContract GetNPDAdminDetails(IDictionary<string, string> objDict)
        {
            NPDContract contract = new NPDContract();
            if (objDict != null && objDict.ContainsKey(Parameter.FROMNAME) && objDict.ContainsKey(Parameter.ITEMID) && objDict.ContainsKey(Parameter.USEREID))
            {
                string formName = objDict[Parameter.FROMNAME];
                int itemId = Convert.ToInt32(objDict[Parameter.ITEMID]);
                string userID = objDict[Parameter.USEREID];
                IForm npdForm = new NPDAdminForm(true);
                npdForm = BELDataAccessLayer.Instance.GetFormData(this.context, this.web, ApplicationNameConstants.NPDAPP, formName, itemId, userID, npdForm);
                if (npdForm != null && npdForm.SectionsList != null && npdForm.SectionsList.Count > 0)
                {
                    var sectionDetails = npdForm.SectionsList.FirstOrDefault(f => f.SectionName == NPDSectionName.NPDADMINSECTION) as NPDAdminDetailSection;
                    if (sectionDetails != null)
                    {
                        if (itemId == 0)
                        {

                        }
                        else
                        {
                            sectionDetails.ApproversList.Remove(sectionDetails.ApproversList.FirstOrDefault(p => p.Role == UserRoles.VIEWER));
                        }
                        // sectionDetails.FileNameList = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files) : string.Empty;
                        sectionDetails.CreatorAttachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.CreatorAttachment) && sectionDetails.CreatorAttachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        sectionDetails.Approver1Attachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.Approver1Attachment) && sectionDetails.Approver1Attachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        sectionDetails.Approver2Attachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.Approver2Attachment) && sectionDetails.Approver2Attachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        sectionDetails.Approver3Attachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.Approver3Attachment) && sectionDetails.Approver3Attachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        sectionDetails.ABSAdminAttachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.ABSAdminAttachment) && sectionDetails.ABSAdminAttachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        if (!string.IsNullOrWhiteSpace(sectionDetails.BusinessUnit) && (sectionDetails.BusinessUnit.ToUpper() == "CP-LTG" || sectionDetails.BusinessUnit.ToUpper() == "LUM"))
                        {
                            sectionDetails.ProductImage = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.ProductImage) && sectionDetails.ProductImage.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                            sectionDetails.SpecificationSheet = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.SpecificationSheet) && sectionDetails.SpecificationSheet.Split(',').Contains(x.FileName)).ToList()) : string.Empty;

                            sectionDetails.TechnicalDataSheet = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.TechnicalDataSheet) && sectionDetails.TechnicalDataSheet.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                            sectionDetails.TestReportsAttachment = sectionDetails.Files != null && sectionDetails.Files.Count > 0 ? JsonConvert.SerializeObject(sectionDetails.Files.Where(x => !string.IsNullOrEmpty(sectionDetails.TestReportsAttachment) && sectionDetails.TestReportsAttachment.Split(',').Contains(x.FileName)).ToList()) : string.Empty;
                        }

                    }
                    contract.Forms.Add(npdForm);
                }
            }
            return contract;

        }

        #endregion

        #region "SAVE Admin DATA"
        /// <summary>
        /// Saves the by section.
        /// </summary>
        /// <param name="sections">The sections.</param>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>return status</returns>
        public ActionStatus SaveAdminDataBySection(ISection sectionDetails, Dictionary<string, string> objDict)
        {
            lock (Padlock)
            {
                ActionStatus status = new ActionStatus();

                //BELDataAccessLayer helper = new BELDataAccessLayer();
                if (sectionDetails != null && objDict != null)
                {
                    objDict[Parameter.ACTIVITYLOG] = NPDListNames.NPDACTIVITYLOG;
                    objDict[Parameter.APPLICATIONNAME] = ApplicationNameConstants.NPDAPP;
                    objDict[Parameter.FROMNAME] = FormNameConstants.NPDADMINFORM;
                    //NPDAdminDetailSection section = null;
                    //if (sectionDetails.SectionName == NPDSectionName.NPDADMINSECTION)
                    //{
                    //    section = sectionDetails as NPDAdminDetailSection;
                    //}
                    List<ListItemDetail> objSaveDetails = BELDataAccessLayer.Instance.SaveData(this.context, this.web, sectionDetails, objDict);
                    ListItemDetail itemDetails = objSaveDetails.Where(p => p.ListName.Equals(NPDListNames.NPDLIST)).FirstOrDefault<ListItemDetail>();
                    if (sectionDetails.SectionName == NPDSectionName.NPDADMINSECTION)
                    {
                        //if (!string.IsNullOrEmpty(section.OldDCRNo))
                        //{
                        //    Dictionary<string, dynamic> values = new Dictionary<string, dynamic>();
                        //    values.Add("IsDCRRetrieved", true);
                        //    BELDataAccessLayer.Instance.SaveFormFields(this.context, this.web, DCRDCNListNames.DCRLIST, section.OldDCRId, values);
                        //}


                    }

                    if (itemDetails.ItemId > 0)
                    {
                        status.IsSucceed = true;
                        switch (sectionDetails.ActionStatus)
                        {
                            case ButtonActionStatus.SaveAndNoStatusUpdate:
                                status.Messages.Add("Text_SaveSuccess");
                                break;
                            case ButtonActionStatus.NextApproval:
                                status.Messages.Add(ApplicationConstants.SUCCESSMESSAGE);
                                break;
                            case ButtonActionStatus.Delegate:
                                status.Messages.Add("Text_DelegatedSuccess");
                                break;
                            case ButtonActionStatus.Complete:
                                status.Messages.Add("Text_CompleteSuccess");
                                break;
                            case ButtonActionStatus.Rejected:
                                status.Messages.Add("Text_RejectedSuccess");
                                break;
                            default:
                                status.Messages.Add(ApplicationConstants.SUCCESSMESSAGE);
                                break;
                        }
                    }
                    else
                    {
                        status.IsSucceed = false;
                        status.Messages.Add(ApplicationConstants.ERRORMESSAGE);
                    }

                }
                return status;
            }
        }
        #endregion

        public bool IsAdminUser(string userid)
        {
            return BELDataAccessLayer.Instance.IsGroupMember(context, web, userid, UserRoles.ADMIN);
        }

        public bool IsCreatorUser(string userid)
        {
            return BELDataAccessLayer.Instance.IsGroupMember(context, web, userid, UserRoles.CREATOR);
        }
    }
}