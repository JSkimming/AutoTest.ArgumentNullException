﻿namespace AutoTest.ArgNullEx.Filter
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Filters out the methods that have nullable parameters, but have null defaults.
    /// </summary>
    public class HasNullableParametersWithoutNullDefaults : FilterBase, IMethodFilter
    {
        /// <summary>
        /// Filters out the methods that have nullable parameters, but have null defaults.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="method">The method.</param>
        /// <returns><c>true</c> if the <paramref name="method"/> should be excluded, otherwise <c>false</c>.</returns>
        bool IMethodFilter.ExcludeMethod(Type type, MethodBase method)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (method == null) throw new ArgumentNullException("method");

            return method.GetParameters()
                         .Where(pi => pi.IsNullable())
                         .All(pi => pi.HasNullDefault());
        }
    }
}
