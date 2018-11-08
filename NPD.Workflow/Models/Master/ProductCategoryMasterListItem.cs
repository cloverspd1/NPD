namespace NPD.Workflow.Models.Master
{
    using CommonDataContract;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;

    /// <summary>
    /// Product Category Master ListItem
    /// </summary>
    [DataContract, Serializable]
    public class ProductCategoryMasterListItem : IMasterItem
    {
        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        [DataMember, FieldColumnName("ProductCategory")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [FieldColumnName("ProductCategoryCode")]
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [DataMember, FieldColumnName("BusinessUnit", true, false, "Code")]
        //[DataMember, FieldColumnName("Business_x0020_Unit", true, false, "Code")]
        public string BusinessUnit { get; set; }
    }
}