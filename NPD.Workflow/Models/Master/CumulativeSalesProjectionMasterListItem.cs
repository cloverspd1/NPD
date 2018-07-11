namespace NPD.Workflow.Models.Master
{
    using CommonDataContract;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;
    /// <summary>
    /// Cumulative Sales Projection Master List Item
    /// </summary>
    [DataContract, Serializable]
    public class CumulativeSalesProjectionMasterListItem : IMasterItem
    {
        /// <summary>
        /// Gets or sets the periods.
        /// </summary>
        /// <value>
        /// The periods.
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
        [FieldColumnName("Value")]
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the desired throughput.
        /// </summary>
        /// <value>
        /// The desired throughput.
        /// </value>
        [FieldColumnName("DesiredThroughput")]
        [DataMember]
        public decimal DesiredThroughput { get; set; }

        /// <summary>
        /// Gets or sets the Business Saliency.
        /// </summary>
        /// <value>
        /// The Business Saliency.
        /// </value>
        [FieldColumnName("BusinessSaliency")]
        [DataMember]
        public decimal BusinessSaliency { get; set; }

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