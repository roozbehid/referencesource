//------------------------------------------------------------------------------
// <copyright file="DesignerSerializerAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.ComponentModel.Design.Serialization {
    using System.Security.Permissions;

    /// <devdoc>
    ///     This attribute can be placed on a class to indicate what serialization
    ///     object should be used to serialize the class at design time.
    /// </devdoc>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public sealed class DesignerSerializerAttribute : Attribute {
        private string serializerTypeName;
        private string serializerBaseTypeName;
        private string typeId;
    
        /// <devdoc>
        ///     Creates a new designer serialization attribute.
        /// </devdoc>
        public DesignerSerializerAttribute(Type serializerType, Type baseSerializerType) {
            this.serializerTypeName = serializerType.AssemblyQualifiedName;
            this.serializerBaseTypeName = baseSerializerType.AssemblyQualifiedName;
        }
    
        /// <devdoc>
        ///     Creates a new designer serialization attribute.
        /// </devdoc>
        public DesignerSerializerAttribute(string serializerTypeName, Type baseSerializerType) {
            this.serializerTypeName = serializerTypeName;
            this.serializerBaseTypeName = baseSerializerType.AssemblyQualifiedName;
        }
    
        /// <devdoc>
        ///     Creates a new designer serialization attribute.
        /// </devdoc>
        public DesignerSerializerAttribute(string serializerTypeName, string baseSerializerTypeName) {
            this.serializerTypeName = serializerTypeName;
            this.serializerBaseTypeName = baseSerializerTypeName;
        }
    
        /// <devdoc>
        ///     Retrieves the fully qualified type name of the serializer.
        /// </devdoc>
        public string SerializerTypeName {
            get {
                return serializerTypeName;
            }
        }
    
        /// <devdoc>
        ///     Retrieves the fully qualified type name of the serializer base type.
        /// </devdoc>
        public string SerializerBaseTypeName {
            get {
                return serializerBaseTypeName;
            }
        }
    }
}

