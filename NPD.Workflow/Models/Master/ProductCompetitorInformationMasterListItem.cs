namespace NPD.Workflow.Models.Master
{
     using CommonDataContract;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;

    /// <summary>
    /// Product competitor Master List Item
    /// </summary>
    [DataContract, Serializable]
    public class ProductCompetitorInformationMasterListItem : IMasterItem
    {
        /// <summary>
        /// Gets or sets the Brand.
        /// </summary>
        /// <value>
        /// The Brand.
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
        /// Gets or sets the Sales Volume.
        /// </summary>
        /// <value>
        /// The Sales Volume.
        /// </value>
        [FieldColumnName("SalesVolume")]
        [DataMember]
        public double? SalesVolume { get; set; }

        /// <summary>
        /// Gets or sets the Market Share.
        /// </summary>
        /// <value>
        /// The Market Share.
        /// </value>
        [FieldColumnName("MarketShare")]
        [DataMember]
        public double? MarketShare { get; set; }

        /// <summary>
        /// Gets or sets the Sales Value.
        /// </summary>
        /// <value>
        /// The Sales Value.
        /// </value>
        [FieldColumnName("SalesValue")]
        [DataMember]
        public double? SalesValue { get; set; }

  

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