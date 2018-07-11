using NPD.CommonDataContract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace NPD.Workflow.Models.NPD
{
    public class NPDProductCompetitor:ITrans
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
        /// Gets or sets the Brand.
        /// </summary>
        /// <value>
        /// The Brand.
        /// </value>
        [DataMember,FieldColumnName("Title"),Required]
        public string Brand { get; set; }

        /// <summary>
        /// Gets or sets the Sales Volume.
        /// </summary>
        /// <value>
        /// The Sales Volume.
        /// </value>
        [DataMember, Required]
        public double? SalesVolume { get; set; }

        /// <summary>
        /// Gets or sets the Market Share.
        /// </summary>
        /// <value>
        /// The Market Share.
        /// </value>
        [DataMember, Required, Range(0.0,100.0)]
        public double? MarketShare { get; set; }

        /// <summary>
        /// Gets or sets the Sales Value.
        /// </summary>
        /// <value>
        /// The Sales Value.
        /// </value>
        [DataMember, Required]
        public double? SalesValue { get; set; }

        /// <summary>
        /// Gets or sets the RLP.
        /// </summary>
        /// <value>
        /// The RLP.
        /// </value>
        [DataMember, Required]
        public double? RLP { get; set; }

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