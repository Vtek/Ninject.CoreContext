using Ninject.Modules;
using Ninject.Planning.Bindings;

namespace Ninject.CoreContext
{
    /// <summary>
    /// Auto injection module.
    /// </summary>
    public abstract class AutoInjectionModule : NinjectModule
    {
        /// <summary>
        /// Registers the specified binding.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        public override void AddBinding(IBinding binding)
        {
            AutoInjection.AddTypeToInject(binding.Service);
            base.AddBinding(binding);
        }
    }
}
