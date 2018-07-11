using NPD.CommonDataContract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace NPD.Workflow.Models.NPD
{
    public class NPDMarketInformation:ITrans
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
        public DateTime? RequestDate { get; set; }

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
        /// Gets or sets the Market Info Category.
        /// </summary>
        /// <value>
        /// The Market Info Category.
        /// </value>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Previous Year.
        /// </summary>
        /// <value>
        /// The Previous Year.
        /// </value>
        [DataMember]
        public double? PreviousYear { get; set; }

        /// <summary>
        /// Gets or sets the Current Year.
        /// </summary>
        /// <value>
        /// The Current Year.
        /// </value>
        [DataMember]
        public double? CurrentYear { get; set; }

        /// <summary>
        /// Gets or sets the Next Year.
        /// </summary>
        /// <value>
        /// The Next Year.
        /// </value>
        [DataMember]
        public double? NextYear { get; set; }

     
    }
}