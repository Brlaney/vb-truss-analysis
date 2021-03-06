// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System;
using System.Reflection;

namespace ExtendedXmlSerialization.Cache
{
    internal class PropertieDefinition
    {
        public PropertieDefinition(Type type, PropertyInfo propertyInfo, string name)
        {
            Name = string.IsNullOrEmpty(name) ? propertyInfo.Name : name;
            TypeDefinition = TypeDefinitionCache.GetDefinition(propertyInfo.PropertyType);
	        IsWritable = propertyInfo.CanWrite;
            _getter = ObjectAccessors.CreatePropertyGetter(type, propertyInfo.Name);
            _propertySetter = CreateSetter( type, propertyInfo.Name );
        }

        public PropertieDefinition(Type type, FieldInfo fieldInfo, string name)
        {
            Name = string.IsNullOrEmpty(name) ? fieldInfo.Name : name;
            TypeDefinition = TypeDefinitionCache.GetDefinition(fieldInfo.FieldType);
	        IsWritable = !fieldInfo.IsInitOnly;
            _getter = ObjectAccessors.CreatePropertyGetter(type, fieldInfo.Name);
            _propertySetter = CreateSetter( type, fieldInfo.Name );
        }

        ObjectAccessors.PropertySetter CreateSetter( Type type, string name )
        {
            if ( IsWritable )
            {
                return ObjectAccessors.CreatePropertySetter(type, name);
            }

            var result = TypeDefinition.MethodAddToCollection != null ? new ObjectAccessors.PropertySetter( TypeDefinition.MethodAddToCollection ) : null;
            return result;
        }

        private readonly ObjectAccessors.PropertyGetter _getter;
        private readonly ObjectAccessors.PropertySetter _propertySetter;
        
        public string Name { get; private set; }
        public TypeDefinition TypeDefinition { get; }
		public bool IsWritable { get; }
        public int Order { get; set; } = -1;
        public int MetadataToken { get; set; }
        public object GetValue(object obj)
        {
            return _getter(obj);
        }

        public void SetValue(object obj, object value)
        {
            _propertySetter?.Invoke(obj, value);
        }
    }
}
