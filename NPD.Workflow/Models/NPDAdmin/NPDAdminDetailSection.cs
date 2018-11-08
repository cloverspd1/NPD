using NPD.CommonDataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using NPD.Workflow.Models.Common;
using NPD.Workflow.Models.NPD;
using System.ComponentModel.DataAnnotations;
using NPD.Workflow.Models.Master;

namespace NPD.Workflow.Models.NPD
{
    /// <summary>
    /// NPD Detail Section
    /// </summary>
    [DataContract, Serializable]
    public class NPDAdminDetailSection : ISection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NPDDetailSection"/> class.
        /// </summary>
        public NPDAdminDetailSection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NPDDetailSection"/> class.
        /// </summary>
        /// <param name="isSet">if set to <c>true</c> [is set].</param>
        public NPDAdminDetailSection(bool isSet)
        {
            if (isSet)
            {
                this.ListDetails = new List<ListItemDetail>() { new ListItemDetail(NPDListNames.NPDLIST, true) };
                this.SectionName = NPDSectionName.NPDADMINSECTION;
                this.NPDProductFeature = new List<ITrans>();
                this.NPDProductCompetitor = new List<ITrans>();
                this.NPDMarketInformation = new List<ITrans>();
                this.NPDCumulativeSalesProjection = new List<ITrans>();
                this.NPDTargetRLP = new List<ITrans>();

                this.ApproversList = new List<ApplicationStatus>();
                //this.VendorNPDList = new List<ITrans>();
                this.CurrentApprover = new ApplicationStatus();

                this.MasterData = new List<IMaster>();
                this.MasterData.Add(new ApproverMaster());
                this.MasterData.Add(new BusinessUnitMaster());
                this.MasterData.Add(new ProjectTypeMaster());
                this.MasterData.Add(new ProductCategoryMaster());
                this.MasterData.Add(new ProductCategoryDetailMaster());
                this.MasterData.Add(new ProductPositioningMaster());
                this.MasterData.Add(new ProjectTriggerMaster());
                this.MasterData.Add(new IncomeGroupMaster());
                this.MasterData.Add(new AgeGroupMaster());
                this.MasterData.Add(new ChannelsMaster());
                this.MasterData.Add(new RegionsMaster());
                this.MasterData.Add(new TestSampleTypesMaster());
                this.MasterData.Add(new DriverTypeMaster());
                this.MasterData.Add(new LightSourceMaster());
                //this.MasterData.Add(new prodcu());
                //this.MasterData.Add(new DesignchangeproposedMaster());

                this.Files = new List<FileDetails>();
            }
        }

        /// <summary>
        /// Gets or sets the master data.
        /// </summary>
        /// <value>
        /// The master data.
        /// </value>
        [DataMember, IsListColumn(false), ContainsMasterData(true)]
        public List<IMaster> MasterData { get; set; }


        /// <summary>
        /// Gets or sets the NPD Product Feature List.
        /// </summary>
        /// <value>
        /// The NPD Product Feature List
        /// </value>
        [DataMember, IsListColumn(false), IsTran(true, NPDListNames.NPDPRODUCTFEATURELIST, typeof(NPDProductFeature))]
        public List<ITrans> NPDProductFeature { get; set; }

        ///// <summary>
        ///// Gets or sets the NPD Product Feature List.
        ///// </summary>
        ///// <value>
        ///// The NPD Product Feature List
        ///// </value>
        //[DataMember, IsListColumn(false), IsTran(true, Masters.PRODUCTCATEGORYDDETAIL, typeof(NPDProductCategoryDetail))]
        //public List<ITrans> NPDProductCategoryDetails { get; set; }

        /// <summary>
        /// Gets or sets the NPD Product Feature List.
        /// </summary>
        /// <value>
        /// The NPD Product Feature List
        /// </value>
        [DataMember, IsListColumn(false), IsTran(true, NPDListNames.NPDPRODUCTCOMPETITORLIST, typeof(NPDProductCompetitor))]
        public List<ITrans> NPDProductCompetitor { get; set; }

        /// <summary>
        /// Gets or sets the NPD Product Feature List.
        /// </summary>
        /// <value>
        /// The NPD Product Feature List
        /// </value>
        [DataMember, IsListColumn(false), IsTran(true, NPDListNames.NPDMARKETINFORMATIONLIST, typeof(NPDMarketInformation))]
        public List<ITrans> NPDMarketInformation { get; set; }

        /// <summary>
        /// Gets or sets the NPD Product Feature List.
        /// </summary>
        /// <value>
        /// The NPD Product Feature List
        /// </value>
        [DataMember, IsListColumn(false), IsTran(true, NPDListNames.NPDCUMULATIVESALESPROJECTIONLIST, typeof(NPDCumulativeSalesProjection))]
        public List<ITrans> NPDCumulativeSalesProjection { get; set; }

        /// <summary>
        /// Gets or sets the NPD Product Feature List.
        /// </summary>
        /// <value>
        /// The NPD Product Feature List
        /// </value>
        [DataMember, IsListColumn(false), IsTran(true, NPDListNames.NPDTARGETRLPLIST, typeof(NPDTargetRLP))]
        public List<ITrans> NPDTargetRLP { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember, IsListColumn(false)]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false), IsViewer]
        public string ProposedBy { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember, IsPerson(true, false, true), FieldColumnName("ProposedBy"), IsViewer]
        public string ProposedByName { get; set; }

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        [DataMember]
        public DateTime? RequestDate { get; set; }

        /// <summary>
        /// Gets or sets the workflow status.
        /// </summary>
        /// <value>
        /// The workflow status.
        /// </value>
        [DataMember]
        public string WorkflowStatus { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the name of the section.
        /// </summary>
        /// <value>
        /// The name of the section.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string SectionName { get; set; }

        /// <summary>
        /// Gets or sets the form belong to.
        /// </summary>
        /// <value>
        /// The form belong to.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string FormBelongTo { get; set; }

        /// <summary>
        /// Gets or sets the application belong to.
        /// </summary>
        /// <value>
        /// The application belong to.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string ApplicationBelongTo { get; set; }

        /// <summary>
        /// Gets or sets the list details.
        /// </summary>
        /// <value>
        /// The list details.
        /// </value>
        [DataMember, IsListColumn(false)]
        public List<ListItemDetail> ListDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        [DataMember, IsListColumn(false)]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the action status.
        /// </summary>
        /// <value>
        /// The action status.
        /// </value>
        [DataMember, IsListColumn(false)]
        public ButtonActionStatus ActionStatus { get; set; }

        /// <summary>
        /// Gets or sets the current approver.
        /// </summary>
        /// <value>
        /// The current approver.
        /// </value>
        [DataMember, IsListColumn(false), IsApproverDetails(true, NPDListNames.NPDAPPAPPROVALMATRIX)]
        public ApplicationStatus CurrentApprover { get; set; }

        /// <summary>
        /// Gets or sets the approvers list.
        /// </summary>
        /// <value>
        /// The approvers list.
        /// </value>
        [DataMember, IsListColumn(false), IsApproverMatrixField(true, NPDListNames.NPDAPPAPPROVALMATRIX)]
        public List<ApplicationStatus> ApproversList { get; set; }

        /// <summary>
        /// Gets or sets the button caption.
        /// </summary>
        /// <value>
        /// The button caption.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string ButtonCaption { get; set; }

        /// <summary>
        /// Gets or sets the Business Unit.
        /// </summary>
        /// <value>
        /// The business unit.
        /// </value>
        [DataMember, Required, RequiredOnDraft]
        public string BusinessUnit { get; set; }

        /// <summary>
        /// Gets or sets the Project Name.
        /// </summary>
        /// <value>
        /// The project name.
        /// </value>
        [DataMember, Required, RequiredOnDraft, MaxLength(150)]
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the Project Type.
        /// </summary>
        /// <value>
        /// The project type.
        /// </value>
        [DataMember, Required, RequiredOnDraft]
        public string ProjectType { get; set; }

        /// <summary>
        /// Gets or sets the Product Category.
        /// </summary>
        /// <value>
        /// The prdouct category.
        /// </value>
        [DataMember, Required, RequiredOnDraft]
        public string ProductCategory { get; set; }

        /// <summary>
        /// Gets or sets the reference brand model.
        /// </summary>
        /// <value>
        /// The reference brand model.
        /// </value>
        [DataMember, Required, RequiredOnDraft, MaxLength(150)]
        public string ReferenceBrandAndModel { get; set; }

        /// <summary>
        /// Gets or sets the Sample Required.
        /// </summary> 
        /// <value>
        /// The reference sample required.
        /// </value>
        [DataMember, Required]
        public bool SampleRequired { get; set; }

        /// <summary>
        /// Gets or sets the Gender.
        /// </summary>
        /// <value>
        /// The reference gender.
        /// </value>
        [DataMember,Required]
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the Income Group.
        /// </summary>
        /// <value>
        /// The income group.
        /// </value>
        [DataMember, Required]
        public string IncomeGroup { get; set; }

        /// <summary>
        /// Gets or sets the Age Group.
        /// </summary>
        /// <value>
        /// The age group.
        /// </value>
        [DataMember, Required]
        public string AgeGroup { get; set; }

        /// <summary>
        /// Gets or sets the Project No.
        /// </summary>
        /// <value>
        /// The project no.
        /// </value>
        [DataMember,]
        public string ProjectNo { get; set; }

      

        /// <summary>
        /// Gets or sets the Project Trigger.
        /// </summary>
        /// <value>
        /// The project trigger.
        /// </value>
        [DataMember, Required]
        public string ProjectTrigger { get; set; }

        /// <summary>
        /// Gets or sets the Regions.
        /// </summary>
        /// <value>
        /// The regions.
        /// </value>
        [DataMember, Required]
        public string Regions { get; set; }

        /// <summary>
        /// Gets or sets the Channels.
        /// </summary>
        /// <value>
        /// The channels.
        /// </value>
        [DataMember, Required]
        public string Channels { get; set; }

        /// <summary>
        /// Gets or sets the Other Channels.
        /// </summary>
        /// <value>
        /// The other channels.
        /// </value>
        [DataMember, MaxLength(150)]
        public string OtherChannels { get; set; }

        /// <summary>
        /// Gets or sets the Product Positioning.
        /// </summary>
        /// <value>
        /// The product positioning.
        /// </value>
        [DataMember, Required]
        public string ProductPositioning { get; set; }

        /// <summary>
        /// Gets or sets the Intended LaunchDate.
        /// </summary>
        /// <value>
        /// The intended launch date.
        /// </value>
        [DataMember, Required, DataType(DataType.Date)]
        public DateTime? IntendedLaunchDate { get; set; }

        /// <summary>
        /// Gets or sets the Requestor Comments.
        /// </summary>
        /// <value>
        /// The requestor comments.
        /// </value>
        [DataMember, Required, MaxLength(255)]
        public string RequestorComments { get; set; }

  

        /// <summary>
        /// Gets or sets the market previous year.
        /// </summary>
        /// <value>
        /// The prdouct category.
        /// </value>
        [DataMember, Required]
        public double? mktPreviousYear1 { get; set; }

        /// <summary>
        /// Gets or sets the market previous year.
        /// </summary>
        /// <value>
        /// The prdouct category.
        /// </value>
        [DataMember, Required]
        public double? mktPreviousYear2 { get; set; }

        /// <summary>
        /// Gets or sets the market current year.
        /// </summary>
        /// <value>
        /// The prdouct category.
        /// </value>
        [DataMember, Required]
        public double? mktCurrentYear1 { get; set; }

        /// <summary>
        /// Gets or sets the market current year.
        /// </summary>
        /// <value>
        /// The prdouct category.
        /// </value>
        [DataMember, Required]
        public double? mktCurrentYear2 { get; set; }

        /// <summary>
        /// Gets or sets the market next year.
        /// </summary>
        /// <value>
        /// The prdouct category.
        /// </value>
        [DataMember, Required]
        public double? mktNextYear1 { get; set; }

        /// <summary>
        /// Gets or sets the market next year.
        /// </summary>
        /// <value>
        /// The prdouct category.
        /// </value>
        [DataMember, Required]
        public double? mktNextYear2 { get; set; }

        /// <summary>
        /// Gets or sets the Current SKUs.
        /// </summary>
        /// <value>
        /// The Current SKUs.
        /// </value>
        [FieldColumnName("CurrentSKUs")]
        [DataMember]
        public double? CurrentSKUs { get; set; }

        /// <summary>
        /// Gets or sets the Phased Out SKUs.
        /// </summary>
        /// <value>
        /// The Phased Out SKUs.
        /// </value>
        [FieldColumnName("PhasedOutSKUs")]
        [DataMember]
        public double? PhasedOutSKUs { get; set; }

        /// <summary>
        /// Gets or sets the Current Sales.
        /// </summary>
        /// <value>
        /// The Current Sales.
        /// </value>
        [FieldColumnName("CurrentSales")]
        [DataMember]
        public double? CurrentSales { get; set; }

        /// <summary>
        /// Gets or sets the Average Sales.
        /// </summary>
        /// <value>
        /// The Average Sales.
        /// </value>
        [FieldColumnName("AverageSales")]
        [DataMember]
        public double? AverageSales { get; set; }

        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        [DataMember, IsFile(true)]
        public List<FileDetails> Files { get; set; }

        /// <summary>
        /// Gets or sets the file name list.
        /// </summary>
        /// <value>
        /// The file name list.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string FileNameList { get; set; }

        /// <summary>
        /// Gets or sets the file name list.
        /// </summary>
        /// <value>
        /// The file name list.
        /// </value>
        [DataMember, Required]
        public string CreatorAttachment { get; set; }

        /// <summary>
        /// Gets or sets the file name list.
        /// </summary>
        /// <value>
        /// The file name list.
        /// </value>
        [DataMember]
        public string Approver1Attachment { get; set; }

        /// <summary>
        /// Gets or sets the file name list.
        /// </summary>
        /// <value>
        /// The file name list.
        /// </value>
        [DataMember]
        public string Approver2Attachment { get; set; }

        /// <summary>
        /// Gets or sets the file name list.
        /// </summary>
        /// <value>
        /// The file name list.
        /// </value>
        [DataMember]
        public string Approver3Attachment { get; set; }

        /// <summary>
        /// Gets or sets the file name list.
        /// </summary>
        /// <value>
        /// The file name list.
        /// </value>
        [DataMember]
        public string ABSAdminAttachment { get; set; }

        /// <summary>
        /// Gets or sets the file name list.
        /// </summary>
        /// <value>
        /// The file name list.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string Approver1Comments { get; set; }

        /// <summary>
        /// Gets or sets the file name list.
        /// </summary>
        /// <value>
        /// The file name list.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string Approver2Comments { get; set; }

        /// <summary>
        /// Gets or sets the file name list.
        /// </summary>
        /// <value>
        /// The file name list.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string Approver3Comments { get; set; }

        /// <summary>
        /// Gets or sets the file name list.
        /// </summary>
        /// <value>
        /// The file name list.
        /// </value>
        [DataMember, IsListColumn(false)]
        public string ABSAdmincomments { get; set; }


        /// <summary>
        /// Old DCR ID
        /// </summary>
        [DataMember, IsListColumn(false)]
        public int OldNPDId { get; set; }

        /// <summary>
        /// Gets or sets the Old DCR No.
        /// </summary>
        /// <value>
        /// The Old DCR No.
        /// </value>
        [DataMember]
        public string OldProjectNo { get; set; }

        /// <summary>
        /// Gets or sets the Old DCR Created Date.
        /// </summary>
        /// <value>
        /// The Old DCR Created Date.
        /// </value>
        [DataMember]
        public DateTime? OldNPDCreatedDate { get; set; }

        /// <summary>
        /// NPD Rejected Date
        /// </summary>
        [DataMember]
        public DateTime? OldNPDRejectedDate { get; set; }

        /// <summary>
        /// ABS Admin User
        /// </summary>
        [DataMember, IsPerson(true, true, false), IsViewer]
        public string RequestViewer { get; set; }

        /// <summary>
        /// ABS Admin User
        /// </summary>
        [DataMember, IsPerson(true, true, true), IsViewer, FieldColumnName("RequestViewer")]
        public string RequestViewerName { get; set; }

        /// <summary>
        /// Gets or sets the Vendor Name.
        /// </summary>
        /// <value>
        /// The Vendor Name.
        /// </value>
        [DataMember, MaxLength(255)]
        public string VendorName { get; set; }

        /// <summary>
        /// Gets or sets the Vendor Address.
        /// </summary>
        /// <value>
        /// The Vendor Address
        /// </value>
        [DataMember, MaxLength(1000)]
        public string VendorAddress { get; set; }

        /// <summary>
        /// Gets or sets the Vendor Address.
        /// </summary>
        /// <value>
        /// The Vendor Address
        /// </value>
        [DataMember,IsListColumn(false)]
        public bool IsCreatorEdit { get; set; }

        /// <summary>
        /// Gets or sets the test sample.
        /// </summary>
        /// <value>
        /// The test sample.
        /// </value>
        [DataMember]
        public string TestSample { get; set; }

        /// <summary>
        /// Gets or sets the total no of identical test sample.
        /// </summary>
        /// <value>
        /// The total no of identical test sample.
        /// </value>
        [DataMember]
        public double? TotalNoOfIdenticalTestSample { get; set; }

        /// <summary>
        /// Gets or sets the product description.
        /// </summary>
        /// <value>
        /// The product description.
        /// </value>
        [DataMember]
        public string ProductDescription { get; set; }

        /// <summary>
        /// Gets or sets the product image.
        /// </summary>
        /// <value>
        /// The product image.
        /// </value>
        [DataMember]
        public string ProductImage { get; set; }

        /// <summary>
        /// Gets or sets the type of light source.
        /// </summary>
        /// <value>
        /// The type of light source.
        /// </value>
        [DataMember]
        public string TypeOfLightSource { get; set; }

        /// <summary>
        /// Gets or sets the no of lamp led.
        /// </summary>
        /// <value>
        /// The no of lamp led.
        /// </value>
        [DataMember]
        public double? NoOfLampLED { get; set; }

        /// <summary>
        /// Gets or sets the lamp led make.
        /// </summary>
        /// <value>
        /// The lamp led make.
        /// </value>
        [DataMember]
        public string LampLEDMake { get; set; }

        /// <summary>
        /// Gets or sets the led part number.
        /// </summary>
        /// <value>
        /// The led part number.
        /// </value>
        [DataMember]
        public string LEDPartNumber { get; set; }

        /// <summary>
        /// Gets or sets the led bin.
        /// </summary>
        /// <value>
        /// The led bin.
        /// </value>
        [DataMember]
        public string LEDBin { get; set; }

        /// <summary>
        /// Gets or sets the nominal CCT.
        /// </summary>
        /// <value>
        /// The nominal CCT.
        /// </value>
        [DataMember]
        public string NominalCCT { get; set; }

        /// <summary>
        /// Gets or sets the nominal cri.
        /// </summary>
        /// <value>
        /// The nominal cri.
        /// </value>
        [DataMember]
        public string NominalCRI { get; set; }

        /// <summary>
        /// Gets or sets the type of the driver.
        /// </summary>
        /// <value>
        /// The type of the driver.
        /// </value>
        [DataMember]
        public string DriverType { get; set; }

        /// <summary>
        /// Gets or sets the driver make.
        /// </summary>
        /// <value>
        /// The driver make.
        /// </value>
        [DataMember]
        public string DriverMake { get; set; }

        /// <summary>
        /// Gets or sets the driver output voltage.
        /// </summary>
        /// <value>
        /// The driver output voltage.
        /// </value>
        [DataMember]
        public double? DriverOutputVoltage { get; set; }

        /// <summary>
        /// Gets or sets the driver nominal cri.
        /// </summary>
        /// <value>
        /// The driver nominal cri.
        /// </value>
        [DataMember]
        public double? DriverNominalCRI { get; set; }

        /// <summary>
        /// Gets or sets the led configuration.
        /// </summary>
        /// <value>
        /// The led configuration.
        /// </value>
        [DataMember]
        public string LEDConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the led driving current.
        /// </summary>
        /// <value>
        /// The led driving current.
        /// </value>
        [DataMember]
        public string LEDDrivingCurrent { get; set; }

        /// <summary>
        /// Gets or sets the purpose of testing.
        /// </summary>
        /// <value>
        /// The purpose of testing.
        /// </value>
        [DataMember]
        public string PurposeOfTesting { get; set; }

        /// <summary>
        /// Gets or sets the test specification.
        /// </summary>
        /// <value>
        /// The test specification.
        /// </value>
        [DataMember]
        public string TestSpecification { get; set; }

        /// <summary>
        /// Gets or sets the photometric comment.
        /// </summary>
        /// <value>
        /// The photometric comment.
        /// </value>
        [DataMember]
        public string PhotometricComment { get; set; }

        /// <summary>
        /// Gets or sets the specification sheet.
        /// </summary>
        /// <value>
        /// The specification sheet.
        /// </value>
        [DataMember]
        public string SpecificationSheet { get; set; }

        /// <summary>
        /// Gets or sets the technical data sheet.
        /// </summary>
        /// <value>
        /// The technical data sheet.
        /// </value>
        [DataMember]
        public string TechnicalDataSheet { get; set; }

        /// <summary>
        /// Gets or sets the test reports attachment.
        /// </summary>
        /// <value>
        /// The test reports attachment.
        /// </value>
        [DataMember]
        public string TestReportsAttachment { get; set; }
    }
}