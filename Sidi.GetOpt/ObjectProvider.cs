using System;

namespace Sidi.GetOpt
{
    internal class ObjectProvider : IObjectProvider
    {
        private readonly Func<object> getInstance;

        public ObjectProvider(Type propertyType, Func<object> getInstance)
        {
            Type = propertyType ?? throw new ArgumentNullException(nameof(propertyType));
            this.getInstance = getInstance ?? throw new ArgumentNullException(nameof(getInstance));
        }

        public Type Type { get; }

        public object Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = getInstance();
                }
                return _Instance;
            }
        }

        public static IObjectProvider Create(Type type, Func<object> getInstance)
        {
            return new ObjectProvider(type, getInstance);
        }

        class StaticObjectProvider : IObjectProvider
        {
            private object instance;

            public StaticObjectProvider(object instance)
            {
                this.instance = instance;
            }

            public Type Type => instance.GetType();

            public object Instance => instance;
        }

        public static IObjectProvider Create(object instance)
        {
            return new StaticObjectProvider(instance);
        }

        object _Instance;
    }
}