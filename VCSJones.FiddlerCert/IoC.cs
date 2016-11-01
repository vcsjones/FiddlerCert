using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace VCSJones.FiddlerCert
{
    public static class Container
    {
        public static IoC Instance { get; } = new IoC();
    }

    public class IoC : IDisposable
    {
        private ConcurrentDictionary<Type, object> _runningObjectTable = new ConcurrentDictionary<Type, object>();
        private ConcurrentDictionary<Type, Type> _registrations = new ConcurrentDictionary<Type, Type>();

        private IoC Parent { get; }

        public IoC()
        {
        }

        private IoC(IoC parent)
        {
            Parent = parent;
        }

        public void Register<T>()
        {
            if (!_registrations.TryAdd(typeof(T), null))
            {
                throw new InvalidOperationException($"Type {typeof(T)} is already registered.");
            }
        }

        public void Register<U, T>() where T : U
        {
            if (!_registrations.TryAdd(typeof(U), typeof(T)))
            {
                throw new InvalidOperationException($"Type {typeof(T)} is already registered.");
            }
        }

        public IoC Child() => new IoC(this);

        public object Resolve(Type type)
        {
            return _runningObjectTable.GetOrAdd(type, t =>
            {
                if (_registrations.ContainsKey(t))
                {
                    var concrete = _registrations[t];
                    Type toResolve;
                    if (concrete == null)
                    {
                        if (!t.IsClass)
                            throw new InvalidOperationException($"Cannot resolve type {t} as an interface without a concrete implementation.");
                        toResolve = t;
                    }
                    else
                    {
                        toResolve = concrete;
                    }
                    return CreateLiveObject(toResolve);
                }
                else if (Parent != null)
                {
                    return Parent.Resolve(type);
                }
                throw new InvalidOperationException("Type is not registered.");
            });
        }

        public T Resolve<T>() => (T)Resolve(typeof(T));

        public void Reset()
        {
            _runningObjectTable.Clear();
            _registrations.Clear();
        }

        private object CreateLiveObject(Type type)
        {
            var ctor = FindBestConstructor(type);
            var parameters = ctor.GetParameters();
            object[] arguments = new object[parameters.Length];
            for(var i = 0; i < parameters.Length; i++)
            {
                arguments[i] = Resolve(parameters[i].ParameterType);
            }
            return ctor.Invoke(arguments);
        }

        private ConstructorInfo FindBestConstructor(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            var sortedConstructors = constructors.OrderByDescending(c => c.GetParameters().Length).ToList();
            if (!sortedConstructors.Any())
            {
                throw new InvalidOperationException($"Type {type} does not have any resolvable constructors.");
            }
            foreach(var constructor in sortedConstructors)
            {
                var parameters = constructor.GetParameters();
                if (parameters.All(p => _registrations.ContainsKey(p.ParameterType)))
                {
                    return constructor;
                }
            }
            return null;
        }

        public void Dispose()
        {
            _registrations.Clear();
            _runningObjectTable.Clear();
        }
    }
}
