using NPD.CommonDataContract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace NPD.Workflow.Models.NPD
{
    public class NPDCumulativeSalesProjection : ITrans
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember, IsListColumn(false)]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        [DataMember, FieldColumnName("RequestID", true, false, "ID")]
        public int RequestID { get; set; }

        /// <summary>
        /// Gets or sets the Request Date.
        /// </summary>
        /// <value>
        /// The Request Date.
        /// </value>
        [DataMember]
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        [DataMember, IsListColumn(false)]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the item action.
        /// </summary>
        /// <value>
        /// The item action.
        /// </value>
        [DataMember, IsListColumn(false)]
        public ItemActionStatus ItemAction { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        /// <value>
        /// The Status.
        /// </value>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the Periods.
        /// </summary>
        /// <value>
        /// The Periods.
        /// </value>
        [DataMember,FieldColumnName("Title"),Required]
        public string Periods { get; set; }

        /// <summary>
        /// Gets or sets the Value.
        /// </summary>
        /// <value>
        /// The Value.
        /// </value>
        [DataMember,Required]
        public double? Value { get; set; }

        /// <summary>
        /// Gets or sets the Desired Throughput.
        /// </summary>
        /// <value>
        /// The Desired Throughput.
        /// </value>
        [DataMember, Required, Range(0.0, 100.0)]
        public double? DesiredThroughput { get; set; }

        /// <summary>
        /// Gets or sets the Business Saliency.
        /// </summary>
        /// <value>
        /// The Business Saliency.
        /// </value>
        [DataMember, Required, Range(0.0, 100.0)]

        public double? BusinessSaliency { get; set; }
        /// <summary>
        /// Gets or sets the Request Date.
        /// </summary>
        /// <value>
        /// The Request Date.
        /// </value>
        [DataMember]
        public string RequestBy { get; set; }
    }
}