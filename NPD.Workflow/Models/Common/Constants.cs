

namespace NPD.Workflow.Models.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Form Names
    /// </summary>
    public static class FormNames
    {
        /// <summary>
        /// The NPDform
        /// </summary>
        public const string NPDFORM = "NPD Form";

        /// <summary>
        /// The NPD Admin form
        /// </summary>
        public const string NPDADMINFORM = "NPD Admin Form";

    }

    /// <summary>
    /// List Names
    /// </summary>
    public static class NPDListNames
    {
        /// <summary>
        /// The NPD list name
        /// </summary>
        public const string NPDLIST = "NPD";

        public const string NPDPRODUCTFEATURELIST = "Product Feature";

        public const string NPDPRODUCTCOMPETITORLIST = "Product Competitor Information";

        public const string NPDMARKETINFORMATIONLIST = "Market Information";

        public const string NPDCUMULATIVESALESPROJECTIONLIST = "Cumulative Sales Projection";

        public const string NPDTARGETRLPLIST = "Target RLP Information";

        

       

        /// <summary>
        /// The NPD activity log
        /// </summary>
        public const string NPDACTIVITYLOG = "NPDActivityLog";

        /// <summary>
        /// The NPD appapproval matrix
        /// </summary>
        public const string NPDAPPAPPROVALMATRIX = "NPDApprovalMatrix";

        /// <summary>
        /// NPD Number Count List Name
        /// </summary>
        public const string NPDNOCOUNT = "NPDNoCount";

       
       
    }

    public static class Masters
    {
        
        /// <summary>
        /// the Product Category Master
        /// </summary>
        public const string PRODUCTCATEGORYMASTER = "ProductCategoryMaster";

        /// <summary>
        /// the product category detail master
        /// </summary>
        public const string PRODUCTCATEGORYDDETAIL = "ProductCategoryDetailMaster";

        /// <summary>
        /// the Approver master
        /// </summary>
        public const string APPROVERMASTER = "ApproverMaster";

        /// <summary>
        /// the Income Group
        /// </summary>
        public const string INCOMEGROUPMASTER = "IncomeGroupMaster";

        /// <summary>
        /// the Age Group master
        /// </summary>
        public const string AGEGROUPMASTER = "AgeGroupMaster";

        /// <summary>
        /// the Project type master
        /// </summary>
        public const string PROJECTTYPEMASTER = "ProjectTypeMaster";

        /// <summary>
        /// the project trigger master
        /// </summary>
        public const string PROJECTTRIGGERMASTER = "ProjectTriggerMaster";

        /// <summary>
        /// the Regions Master
        /// </summary>
        public const string REGIONSMASTER = "RegionsMaster";

        /// <summary>
        /// the Channels Master
        /// </summary>
        public const string CHANNELSMASTER = "ChannelsMaster";

        /// <summary>
        /// the business unit master
        /// </summary>
        public const string BUSINESSUNITMASTER = "BusinessUnitMaster";

        /// <summary>
        /// the business unit master
        /// </summary>
        public const string PRODUCTPOSITIONINGMASTER = "ProductPositioningMaster";

        /// <summary>
        /// the employee master
        /// </summary>
        public const string EMPLOYEEMASTER = "EmployeeMaster";
    }

    /// <summary>
    /// NPD Section Names
    /// </summary>
    public static class NPDSectionName
    {
        /// <summary>
        /// The npd details section
        /// </summary>
        public const string NPDDETAILSECTION = "NPD Detail Section";

        /// <summary>
        /// The NPD approver1 section
        /// </summary>
        public const string NPDAPPROVER1SECTION = "NPD Approver1 Section";

        /// <summary>
        /// The NPD approver2 section
        /// </summary>
        public const string NPDAPPROVER2SECTION = "NPD Approver2 Section";

        /// <summary>
        /// The NPD approver3 section
        /// </summary>
        public const string NPDAPPROVER3SECTION = "NPD Approver3 Section";

        /// <summary>
        /// The NPD approver1 section
        /// </summary>
        public const string NPDABSADMINSECTION = "ABS Admin Section";

        /// <summary>
        /// The NPD approver1 section
        /// </summary>
        public const string NPDADMINSECTION = "NPD Admin Detail Section";


        /// <summary>
        /// The activitylog
        /// </summary>
        public const string ACTIVITYLOG = "Activity Log";

        /// <summary>
        /// The activitylog
        /// </summary>
        public const string APPLICATIONSTATUSSECTION = "Application Status Section";

        /// <summary>
        /// The npd admin details section
        /// </summary>
        public const string NPDDETAILADMINSECTION = "NPD Detail Admin Section";
    }

    /// <summary>
    /// NPD Roles
    /// </summary>
    public static class NPDRoles
    {
        /// <summary>
        /// The creator
        /// </summary>
        public const string CREATOR = "Creator";

        /// <summary>
        /// The viewer
        /// </summary>
        public const string VIEWER = "Viewer";

        /// <summary>
        /// The editor
        /// </summary>
        public const string EDITOR = "Editor";

        /// Approver
        /// </summary>
        public const string APPROVER1 = "Approver1";

        /// Approver
        /// </summary>
        public const string APPROVER2 = "Approver2";

        /// Approver
        /// </summary>
        public const string APPROVER3 = "Approver3";

        /// <summary>
        /// Admin
        /// </summary>
        public const string ADMIN = "Admin";

        /// <summary>
        /// ABS Admin
        /// </summary>
        public const string ABSADMIN = "ABS Admin";
    
    }

    public enum URLAction
    {
        /// <summary>
        /// The noaction
        /// </summary>
        NOACTION = 0,

        /// <summary>
        /// The iscopy
        /// </summary>
        COPY = 1,

        /// <summary>
        /// The isretrieve
        /// </summary>
        RETRIEVE = 2,

        /// <summary>
        /// The rollback
        /// </summary>
        ROLLBACK = 3
    }
}