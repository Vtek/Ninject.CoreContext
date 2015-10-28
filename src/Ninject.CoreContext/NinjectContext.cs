using Ninject.Modules;
using Ninject.Selection.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninject.CoreContext
{
    /// <summary>
    /// Ninject context.
    /// </summary>
    public sealed class NinjectContext
    {
        /// <summary>
        /// The synchronize root object.
        /// </summary>
        static readonly object SyncRoot = new object();

        /// <summary>
        /// Single instance of the NinjectContext
        /// </summary>
        static readonly NinjectContext _instance = new NinjectContext();

        /// <summary>
		/// Ninject module to use in the context.
		/// </summary>
        readonly IList<INinjectModule> _modules = new List<INinjectModule>();

        /// <summary>
        /// True if NinjectContext was initialized, otherwise false
        /// </summary>
        static bool _initialized;

        /// <summary>
        /// Tue if AutoInjection is using
        /// </summary>
        bool _withAutoInjection;

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
        public bool Initialized
        {
            get
            {
                lock (SyncRoot)
                    return _initialized;
            }
        }

        /// <summary>
        /// Gets the modules.
        /// </summary>
        /// <value>The modules.</value>
        public IList<INinjectModule> Modules
        {
            get
            {
                lock (SyncRoot)
                    return _modules;
            }
        }

        /// <summary>
        /// Gets the inject types.
        /// </summary>
        /// <value>The inject types.</value>
        public IList<Type> InjectTypes
        {
            get
            {
                lock (SyncRoot)
                    return AutoInjection.ShouldInjectPropertyTypes;
            }
        }

        /// <summary>
        /// Use stack which be execute during initialize method
        /// </summary>
        private IList<Action<IKernel>> _useStack = new List<Action<IKernel>>();

        /// <summary>
        /// Adds the module.
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public NinjectContext AddModule<T>() where T : class, INinjectModule
        {
            lock (SyncRoot)
            {
                if (Initialized) return this;

                var module = (T)Activator.CreateInstance(typeof(T));

                if (_modules.Any(x => x.GetType().Equals(module.GetType())))
                    return this;

                _modules.Add(module);
                return this;
            }
        }

        /// <summary>
        /// Activate the AutoInjection
        /// </summary>
        public NinjectContext WithAutoInjection()
        {
            lock (SyncRoot)
            {
                if (Initialized) return this;

                _withAutoInjection = true;
                return this;
            }
        }

        /// <summary>
        /// Use current action in the initialize method
        /// </summary>
        /// <param name="use">Use action</param>
        public void Use(Action<IKernel> use)
        {
            _useStack.Add(use);
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        public void Initialize()
        {
            lock (SyncRoot)
            {
                if (Initialized) return;

                var kernel = new StandardKernel(_modules.ToArray());
                if (_withAutoInjection)
                    kernel.Components.Add<IInjectionHeuristic, AutoInjection>();

                foreach (var use in _useStack)
                    use(kernel);

                _initialized = true;
            }
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static NinjectContext Get()
        {
            lock (SyncRoot)
                return _instance;
        }
    }
}
