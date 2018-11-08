using NPD.CommonDataContract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace NPD.Workflow.Models.NPD
{
    /// <summary>
    /// Product Feature
    /// </summary>
    [DataContract, Serializable]
    public class NPDProductFeature:ITrans
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
        /// Gets or sets the Product Feature.
        /// </summary>
        /// <value>
        /// The Title.
        /// </value>
        [DataMember,Required, FieldColumnName("Title")]
        public string ProductFeature { get; set; }

        /// <summary>
        /// Gets or sets the Bajaj Brand.
        /// </summary>
        /// <value>
        /// The Bajaj Brand.
        /// </value>
        [DataMember, Required]
        public string BajajBrand { get; set; }

        /// <summary>
        /// Gets or sets the Reference Brand.
        /// </summary>
        /// <value>
        /// The Reference Brand.
        /// </value>
        [DataMember,Required]
        public string ReferenceBrand { get; set; }

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
        /// Gets or sets the Request Date.
        /// </summary>
        /// <value>
        /// The Request Date.
        /// </value>
        [DataMember]
        public string RequestBy { get; set; }
    }
}