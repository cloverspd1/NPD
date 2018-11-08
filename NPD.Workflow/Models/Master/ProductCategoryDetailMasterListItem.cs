namespace NPD.Workflow.Models.Master
{
    using CommonDataContract;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;

    /// <summary>
    /// Product Category Detail Master List Item
    /// </summary>
    [DataContract, Serializable]
    public class ProductCategoryDetailMasterListItem : IMasterItem
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
        /// Gets or sets the Current SKUs.
        /// </summary>
        /// <value>
        /// The Current SKUs.
        /// </value>
        [FieldColumnName("CurrentSKUs")]
        [DataMember]
        public double? CurrentSKUs { get; set; }

        /// <summary>
        /// Gets or sets the Phased Out SKUs.
        /// </summary>
        /// <value>
        /// The Phased Out SKUs.
        /// </value>
        [FieldColumnName("PhasedOutSKUs")]
        [DataMember]
        public double? PhasedOutSKUs { get; set; }

        /// <summary>
        /// Gets or sets the Current Sales.
        /// </summary>
        /// <value>
        /// The Current Sales.
        /// </value>
        [FieldColumnName("CurrentSales")]
        [DataMember]
        public double? CurrentSales { get; set; }

        /// <summary>
        /// Gets or sets the Average Sales.
        /// </summary>
        /// <value>
        /// The Average Sales.
        /// </value>
        [FieldColumnName("AverageSales")]
        [DataMember]
        public double? AverageSales { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        /// 
          [DataMember, FieldColumnName("ProductCategory_x003a_ProductCat", true, false, "Title")]
        //[DataMember, FieldColumnName("Product_x0020_Category_x003a_Pro", true, false, "Title")]
        ////[DataMember, FieldColumnName("")]
        public string ProductCategoryDetail { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        /// 
         [DataMember, FieldColumnName("ProductCategory", true, false, "ProductCategoryCode")]
       /// [DataMember, FieldColumnName("Product_x0020_Category", true, false, "ProductCategoryCode")]
        public string ProductCategoryCode { get; set; }

          
    }
}