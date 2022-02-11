using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;

namespace Bb.Configurations
{

    // [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(ConfigurationSerializer), LifeCycle = IocScopeEnum.Singleton)]
    public class ConfigurationSerializer
    {

        public ConfigurationSerializer(IConfiguration configuration, IServiceProvider services)
        {
            this._configuration = configuration;
            this._services = services;
            _mappers = new List<ConfigurationSubSerializer>();
            this._defaultMapper = (ConfigurationSubSerializer)services.GetService(typeof(ConfigurationSubSerializerJson));
        }


        public T? Get<T>(Type? type)
        {

            if (type != null)
            {
                var item = (T)TypeDescriptor.CreateInstance(null, type, new Type[] { }, new object[] { });
                var key = GetConfigurationKey(type);
                Map(type, item, key);
                return item;
            }

            return default(T);

        }


        public string Serialize(Type type, object instance)
        {
            var mapper = GetMapper(type);
            return mapper.Serialize(instance);
        }


        public void Map(Type type, object instance, string keyMapper)
        {
            var mapper = GetMapper(type);
            mapper.Map(instance, keyMapper);
        }


        private ConfigurationSubSerializer GetMapper(Type type)
        {

            if (!_DicMappers.TryGetValue(type, out var mapper))
                lock (_lock1)
                    if (!_DicMappers.TryGetValue(type, out mapper))
                        _DicMappers.Add(type, mapper = ResolveMapper(type));

            return mapper;

        }

        private ConfigurationSubSerializer ResolveMapper(Type type)
        {

            var mappers = GetMappers();

            foreach (var item in mappers)
                if (item.CanMap(type))
                    return item;

            return this._defaultMapper;

        }

        private List<ConfigurationSubSerializer> GetMappers()
        {

            if (_mappers.Count != this.TypeMappers.Count)
                lock (_lock2)
                    if (_mappers.Count != this.TypeMappers.Count)
                    {
                        _mappers.Clear();
                        _DicMappers.Clear();
                        foreach (var item in TypeMappers)
                        {
                            var mapper = (ConfigurationSubSerializer)_services.GetService(item);
                            _mappers.Add(mapper);
                        }
                    }

            return _mappers;

        }

        public List<Type> TypeMappers { get; set; } = new List<Type>() { typeof(ConfigurationSubSerializerSection) };


        public string GetConfigurationKey(Type type)
        {

            var keyMapper = type.GetAttributes<ConfigurationAttribute>()
                    .FirstOrDefault()?.ConfigurationKey
                ?? CleanName(type.Name);

            return keyMapper;

        }

        private static string CleanName(string txt)
        {

            if (txt.Contains('`'))
            {
                var index = txt.IndexOf("`");
                return txt[..index];
            }

            return txt;

        }

        private Dictionary<Type, ConfigurationSubSerializer> _DicMappers = new Dictionary<Type, ConfigurationSubSerializer>();
        protected readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IServiceProvider _services;
        private readonly List<ConfigurationSubSerializer> _mappers;
        private readonly ConfigurationSubSerializer _defaultMapper;
        private volatile object _lock1 = new object();
        private volatile object _lock2 = new object();

    }



}
