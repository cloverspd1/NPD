using NPD.CommonDataContract;
using NPD.Workflow.Models.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NPD.Workflow.Models.NPD
{
    /// <summary>
    /// DR Form
    /// </summary>
    [DataContract, Serializable]
    public class NPDForm : IForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NPDForm"/> class.
        /// </summary>
        public NPDForm()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NPDForm"/> class.
        /// </summary>
        /// <param name="setSections">if set to <c>true</c> [set sections].</param>
        public NPDForm(bool setSections)
        {
            if (setSections)
            {
                this.ApprovalMatrixListName = NPDListNames.NPDAPPAPPROVALMATRIX;
                this.SectionsList = new List<ISection>();
                this.SectionsList.Add(new NPDDetailSection(true));
                this.SectionsList.Add(new NPDApprover1Section(true));
                this.SectionsList.Add(new NPDApprover2Section(true));
                this.SectionsList.Add(new NPDApprover3Section(true));
                this.SectionsList.Add(new NPDABSAdminSection(true));
                this.SectionsList.Add(new ApplicationStatusSection(true) { SectionName = SectionNameConstant.APPLICATIONSTATUS });
                this.SectionsList.Add(new ActivityLogSection(NPDListNames.NPDACTIVITYLOG));
                this.Buttons = new List<Button>();
                this.MainListName = NPDListNames.NPDLIST;
            }
        }

        /// <summary>
        /// Gets the name of the form.
        /// </summary>
        /// <value>
        /// The name of the form.
        /// </value>
        [DataMember]
        public string FormName
        {
            get { return FormNames.NPDFORM; }
        }

        /// <summary>
        /// Gets or sets the form status.
        /// </summary>
        /// <value>
        /// The form status.
        /// </value>
        [DataMember]
        public string FormStatus { get; set; }

        /// <summary>
        /// Gets or sets the form approval level.
        /// </summary>
        /// <value>
        /// The form approval level.
        /// </value>
        [DataMember]
        public int FormApprovalLevel { get; set; }

        /// <summary>
        /// Gets or sets the total approval required.
        /// </summary>
        /// <value>
        /// The total approval required.
        /// </value>
        [DataMember]
        public int TotalApprovalRequired { get; set; }

        /// <summary>
        /// Gets or sets the sections list.
        /// </summary>
        /// <value>
        /// The sections list.
        /// </value>
        [DataMember]
        public List<ISection> SectionsList { get; set; }

        /// <summary>
        /// Gets or sets the buttons.
        /// </summary>
        /// <value>
        /// The buttons.
        /// </value>
        [DataMember]
        public List<Button> Buttons { get; set; }

        /// <summary>
        /// Gets or sets the name of the approval matrix list.
        /// </summary>
        /// <value>
        /// The name of the approval matrix list.
        /// </value>
        [DataMember]
        public string ApprovalMatrixListName { get; set; }

        /// <summary>
        /// Gets or sets the name of the main list.
        /// </summary>
        /// <value>
        /// The name of the main list.
        /// </value>
        [DataMember]
        public string MainListName { get; set; }
    }
}