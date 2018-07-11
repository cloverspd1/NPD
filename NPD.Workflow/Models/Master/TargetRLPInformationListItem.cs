namespace NPD.Workflow.Models.Master
{
    using CommonDataContract;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;

    /// <summary>
    /// Target RLP Information List Item
    /// </summary>
    [DataContract, Serializable]
    public class TargetRLPInformationListItem : IMasterItem
    {
        /// <summary>
        /// Gets or sets the market information category.
        /// </summary>
        /// <value>
        /// The market information category.
        /// </value>  
        [FieldColumnName("Title")]
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [FieldColumnName("Title")]
        [DataMember]
        public string Value { get; set; }


        /// <summary>
        /// Gets or sets the Bajaj Brand.
        /// </summary>
        /// <value>
        /// The Bajaj Brand Value.
        /// </value>
        [FieldColumnName("TargetRLP")]
        [DataMember]
        public decimal TargetRLP { get; set; }

        /// <summary>
        /// Gets or sets the Reference Brand.
        /// </summary>
        /// <value>
        /// The Reference Brand.
        /// </value>
        [FieldColumnName("TargetTransferPrice")]
        [DataMember]
        public decimal TargetTransferPrice { get; set; }

        /// <summary>
        /// Gets or sets the Request ID.
        /// </summary>
        /// <value>
        /// The RequestID.
        /// </value>
        [FieldColumnName("RequestID")]
        [DataMember]
        public int RequestID { get; set; }
    }
}