using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.InventoryPro
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
    public class SerializationModelAttribute : Attribute
    {
        public Type type;

        public SerializationModelAttribute(Type type)
        {
            this.type = type;
        }
    }
}
