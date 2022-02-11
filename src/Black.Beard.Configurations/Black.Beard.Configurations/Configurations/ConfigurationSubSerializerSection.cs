

namespace Bb.Configurations
{

    // [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(ConfigurationSectionSerializer))]
    public class ConfigurationSubSerializerSection : ConfigurationSubSerializer
    {

        public ConfigurationSubSerializerSection(Microsoft.Extensions.Configuration.IConfiguration configuration) 
            : base(configuration)
        {

        }
        public override bool CanMap(Type type)
        {
            return typeof(Microsoft.Extensions.Configuration.ConfigurationSection).IsAssignableFrom(type);
        }

        public override bool CanSerialize(Type type)
        {
            return false;
        }

        public override string Serialize(object instance)
        {
            throw new NotImplementedException();
        }

        public override void Map(object instance, string keyMapper)
        {
            var i = (Microsoft.Extensions.Configuration.ConfigurationSection)instance;
            i.Map(_configuration.GetSection(keyMapper).Value);
        }


    }

}
