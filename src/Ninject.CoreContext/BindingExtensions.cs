using Ninject.Syntax;
using System.Web;

namespace Ninject.CoreContext
{
    /// <summary>
    /// Binding in syntax extensions.
    /// </summary>
    public static class BindingInSyntaxExtensions
    {
        /// <summary>
        /// Set the scope of binding in HttpContext.
        /// </summary>
        /// <returns>The http scope.</returns>
        /// <param name="syntax">Syntax.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IBindingNamedWithOrOnSyntax<T> InHttpScope<T>(this IBindingWhenInNamedWithOrOnSyntax<T> syntax)
        {
            return syntax.InScope(x => HttpContext.Current);
        }
    }
}
