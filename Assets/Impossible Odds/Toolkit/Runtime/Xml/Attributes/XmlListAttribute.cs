using System;

namespace ImpossibleOdds.Xml
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public sealed class XmlListAttribute : Attribute
    {
        public const string DefaultListEntryName = "Entry";

        private string childElementName = DefaultListEntryName;

        /// <summary>
        /// The name each entry in the list should have.
        /// </summary>
        public string EntryName
        {
            get => childElementName;
            set => childElementName = value;
        }
    }
}
