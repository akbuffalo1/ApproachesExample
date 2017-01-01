using System;

namespace TigerApp
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class ApiResourcePathAttribute : Attribute
    {
        public readonly string URI;

        public ApiResourcePathAttribute(string uri)
        {
            this.URI = uri;
        }
    }
}
