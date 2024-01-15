using System;

namespace ImpossibleOdds.Xml
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class XmlNestedListElementAttribute : AbstractXmlMemberAttribute
    {
        public const string DefaultListEntryName = "Entry";

        /// <summary>
        /// Array of entry names.
        /// </summary>
        public string[] NestedListEntryNames
        {
            get;
            set;
        }

        public XmlNestedListElementAttribute()
        { }

        public XmlNestedListElementAttribute(string key)
            : base(key)
        { }
    }
}
