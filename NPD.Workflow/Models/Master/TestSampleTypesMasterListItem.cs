namespace NPD.Workflow.Models.Master
{
    using CommonDataContract;
    using System;
    using System.Runtime.Serialization;

    [DataContract, Serializable]
    public class TestSampleTypesMasterListItem : IMasterItem
    {
        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        [DataMember, FieldColumnName("Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [DataMember, FieldColumnName("Title")]
        public string Value { get; set; }
    }
}