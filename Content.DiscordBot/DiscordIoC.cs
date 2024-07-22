// using System.Runtime.CompilerServices;
// using JetBrains.Annotations;
// using Robust.Server;
// using Robust.Shared.IoC;

// namespace Content.DiscordBot;

// public static class DiscordIoC
// {
//     public static IDependencyCollection Instance = new DependencyCollection();

//     /// <summary>
//     /// Ensures that the <see cref="IDependencyCollection"/> instance exists for this thread.
//     /// </summary>
//     /// <remarks>
//     /// This will create a new instance of a <see cref="IDependencyCollection"/> for this thread,
//     /// otherwise it will do nothing if one already exists.
//     /// </remarks>
//     /// <returns>The dependency collection for this thread.</returns>
//     public static IDependencyCollection Init()
//     {
//         IoCManager.InitThread(Instance, true);
//         ServerIoC.RegisterIoC(Instance);

//         return Instance;
//     }

//     /// <summary>
//     /// Registers an interface to an implementation, to make it accessible to <see cref="Resolve{T}"/>
//     /// </summary>
//     /// <typeparam name="TInterface">The type that will be resolvable.</typeparam>
//     /// <typeparam name="TImplementation">The type that will be constructed as implementation.</typeparam>
//     /// <param name="overwrite">
//     /// If true, do not throw an <see cref="InvalidOperationException"/> if an interface is already registered,
//     /// replace the current implementation instead.
//     /// </param>
//     /// <exception cref="InvalidOperationException">
//     /// Thrown if <paramref name="overwrite"/> is false and <typeparamref name="TInterface"/> has been registered before,
//     /// or if an already instantiated interface (by <see cref="BuildGraph"/>) is attempting to be overwritten.
//     /// </exception>
//     public static void Register<TInterface, TImplementation>(bool overwrite = false)
//         where TImplementation : class, TInterface
//         where TInterface : class
//     {
//         Instance.Register<TInterface, TImplementation>(overwrite);
//     }

//     /// <summary>
//     /// Register an implementation, to make it accessible to <see cref="Resolve{T}"/>
//     /// </summary>
//     /// <typeparam name="T">The type that will be resolvable and implementation.</typeparam>
//     /// <param name="overwrite">
//     /// If true, do not throw an <see cref="InvalidOperationException"/> if an interface is already registered,
//     /// replace the current implementation instead.
//     /// </param>
//     /// <exception cref="InvalidOperationException">
//     /// Thrown if <paramref name="overwrite"/> is false and <typeparamref name="T"/> has been registered before,
//     /// or if an already instantiated interface (by <see cref="BuildGraph"/>) is attempting to be overwritten.
//     /// </exception>
//     public static void Register<T>(bool overwrite = false) where T : class
//     {
//         Register<T, T>(overwrite);
//     }

//     /// <summary>
//     /// Registers an interface to an implementation, to make it accessible to <see cref="Resolve{T}"/>
//     /// <see cref="BuildGraph"/> MUST be called after this method to make the new interface available.
//     /// </summary>
//     /// <typeparam name="TInterface">The type that will be resolvable.</typeparam>
//     /// <typeparam name="TImplementation">The type that will be constructed as implementation.</typeparam>
//     /// <param name="factory">A factory method to construct the instance of the implementation.</param>
//     /// <param name="overwrite">
//     /// If true, do not throw an <see cref="InvalidOperationException"/> if an interface is already registered,
//     /// replace the current implementation instead.
//     /// </param>
//     /// <exception cref="InvalidOperationException">
//     /// Thrown if <paramref name="overwrite"/> is false and <typeparamref name="TInterface"/> has been registered before,
//     /// or if an already instantiated interface (by <see cref="BuildGraph"/>) is attempting to be overwritten.
//     /// </exception>
//     public static void Register<TInterface, TImplementation>(DependencyFactoryDelegate<TImplementation> factory, bool overwrite = false)
//         where TImplementation : class, TInterface
//         where TInterface : class
//     {
//         Instance.Register<TInterface, TImplementation>(factory, overwrite);
//     }

//     /// <summary>
//     ///     Registers an interface to an existing instance of an implementation,
//     ///     making it accessible to <see cref="IDependencyCollection.Resolve{T}"/>.
//     ///     Unlike <see cref="IDependencyCollection.Register{TInterface, TImplementation}"/>,
//     ///     <see cref="IDependencyCollection.BuildGraph"/> does not need to be called after registering an instance
//     ///     if deferredInject is false.
//     /// </summary>
//     /// <typeparam name="TInterface">The type that will be resolvable.</typeparam>
//     /// <param name="implementation">The existing instance to use as the implementation.</param>
//     /// <param name="overwrite">
//     ///     If true, do not throw an <see cref="InvalidOperationException"/> if an interface is already registered,
//     ///     replace the current implementation instead.
//     /// </param>
//     public static void RegisterInstance<TInterface>(object implementation, bool overwrite = false)
//         where TInterface : class
//     {
//         Instance.RegisterInstance<TInterface>(implementation, overwrite);
//     }

//     /// <summary>
//     /// Clear all services and types.
//     /// Use this between unit tests and on program shutdown.
//     /// If a service implements <see cref="IDisposable"/>, <see cref="IDisposable.Dispose"/> will be called on it.
//     /// </summary>
//     public static void Clear()
//     {
//         Instance.Clear();
//     }

//     /// <summary>
//     /// Resolve a dependency manually.
//     /// </summary>
//     /// <exception cref="UnregisteredTypeException">Thrown if the interface is not registered.</exception>
//     /// <exception cref="InvalidOperationException">
//     /// Thrown if the resolved type hasn't been created yet
//     /// because the object graph still needs to be constructed for it.
//     /// </exception>
//     [System.Diagnostics.Contracts.Pure]
//     public static T Resolve<T>()
//     {
//         return Instance.Resolve<T>();
//     }

//     /// <inheritdoc cref="Resolve{T}()"/>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public static void Resolve<T>([NotNull] ref T? instance)
//     {
//         // Do not call into IDependencyCollection immediately for this,
//         // avoids thread local lookup if instance is already given.
//         instance ??= Resolve<T>()!;
//     }

//     /// <inheritdoc cref="Resolve{T}(ref T?)"/>
//     /// <summary>
//     /// Resolve two dependencies manually.
//     /// </summary>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public static void Resolve<T1, T2>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2)
//     {
//         Instance.Resolve(ref instance1, ref instance2);
//     }

//     /// <inheritdoc cref="Resolve{T1, T2}(ref T1?, ref T2?)"/>
//     /// <summary>
//     /// Resolve three dependencies manually.
//     /// </summary>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public static void Resolve<T1, T2, T3>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2, [NotNull] ref T3? instance3)
//     {
//         Instance.Resolve(ref instance1, ref instance2, ref instance3);
//     }

//     /// <inheritdoc cref="Resolve{T1, T2, T3}(ref T1?, ref T2?, ref T3?)"/>
//     /// <summary>
//     /// Resolve four dependencies manually.
//     /// </summary>
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public static void Resolve<T1, T2, T3, T4>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2, [NotNull] ref T3? instance3, [NotNull] ref T4? instance4)
//     {
//         Instance.Resolve(ref instance1, ref instance2, ref instance3, ref instance4);
//     }

//     /// <summary>
//     /// Resolve a dependency manually.
//     /// </summary>
//     /// <exception cref="UnregisteredTypeException">Thrown if the interface is not registered.</exception>
//     /// <exception cref="InvalidOperationException">
//     /// Thrown if the resolved type hasn't been created yet
//     /// because the object graph still needs to be constructed for it.
//     /// </exception>
//     [System.Diagnostics.Contracts.Pure]
//     public static object ResolveType(Type type)
//     {
//         return Instance.ResolveType(type);
//     }

//     /// <summary>
//     /// Initializes the object graph by building every object and resolving all dependencies.
//     /// </summary>
//     /// <seealso cref="InjectDependencies{T}"/>
//     public static void BuildGraph()
//     {
//         Instance.BuildGraph();
//     }

//     /// <summary>
//     ///     Injects dependencies into all fields with <see cref="DependencyAttribute"/> on the provided object.
//     ///     This is useful for objects that are not IoC created, and want to avoid tons of IoC.Resolve() calls.
//     /// </summary>
//     /// <remarks>
//     ///     This does NOT initialize IPostInjectInit objects!
//     /// </remarks>
//     /// <param name="obj">The object to inject into.</param>
//     /// <exception cref="UnregisteredDependencyException">
//     ///     Thrown if a dependency field on the object is not registered.
//     /// </exception>
//     /// <seealso cref="BuildGraph"/>
//     public static T InjectDependencies<T>(T obj) where T : notnull
//     {
//         Instance.InjectDependencies(obj);
//         return obj;
//     }
// }
