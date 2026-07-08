using System;

namespace ERP_sys.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuditActionAttribute : Attribute
    {
        public string Description { get; }

        public AuditActionAttribute(string description)
        {
            Description = description;
        }
    }
}