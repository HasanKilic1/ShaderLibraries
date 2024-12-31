using System;

namespace HKAnnotations
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ExecuteByButtonClick : Attribute
    {
        public string Description;
        public ExecuteByButtonClick() { }
        public ExecuteByButtonClick(string description)
        {
            Description = description;
        }
    }
}

