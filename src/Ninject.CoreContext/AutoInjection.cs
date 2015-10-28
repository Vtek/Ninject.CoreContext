using Ninject.Components;
using Ninject.Selection.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ninject.CoreContext
{
    /// <summary>
    /// Auto injection heuristic.
    /// </summary>
    sealed class AutoInjection : NinjectComponent, IInjectionHeuristic
    {
        /// <summary>
        /// Initializes the AutoInjection class.
        /// </summary>
        static AutoInjection()
        {
            ShouldInjectPropertyTypes = new List<Type>();
        }

        /// <summary>
        /// Gets or sets the should inject property types.
        /// </summary>
        /// <value>The should inject property types.</value>
        internal static List<Type> ShouldInjectPropertyTypes { get; private set; }

        /// <summary>
        /// Adds the type to inject.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <remarks>If check for childs type, parent must be in the same assembly</remarks>
        internal static void AddTypeToInject(Type type)
        {
            if (!ShouldInjectPropertyTypes.Contains(type))
                ShouldInjectPropertyTypes.Add(type);
        }

        /// <summary>
        /// Returns a value indicating whether the specified member should be injected.
        /// </summary>
        /// <param name="member">The member in question.</param>
        /// <returns>True</returns>
        /// <c>false</c>
        public bool ShouldInject(MemberInfo member)
        {
            var propertyInfo = member as PropertyInfo;

            if (member == null || propertyInfo == null || !propertyInfo.PropertyType.IsInterface)
                return false;

            return propertyInfo.CanWrite && ShouldInjectPropertyTypes.Any(x => x == propertyInfo.PropertyType || x.IsAssignableFrom(propertyInfo.PropertyType));
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        public override void Dispose(bool disposing)
        {
            ShouldInjectPropertyTypes.Clear();
            base.Dispose(disposing);
        }
    }
}
