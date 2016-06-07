﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lunggo.ApCommon.Product.Constant;
using Lunggo.Framework.Pattern;

namespace Lunggo.ApCommon.Product.Service
{
    public abstract class ProductServiceBase<T> : SingletonBase<T> where T : SingletonBase<T>
    {
        internal abstract void Issue(string rsvNo);

        private static Dictionary<ProductType, Type> _serviceList =  
            (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
             from assemblyType in domainAssembly.GetTypes()
             where typeof(ProductServiceBase<T>).IsAssignableFrom(assemblyType)
             select assemblyType).ToDictionary(
                type => (ProductType) type.GetProperty("Type").GetConstantValue(),
                type => type);

        internal static Type GetService(ProductType type)
        {
            return _serviceList[type];
        }
    }
}