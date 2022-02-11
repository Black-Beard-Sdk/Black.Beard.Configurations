namespace Bb.Configurations
{

    public abstract class ConfigurationSubSerializer
    {

        public ConfigurationSubSerializer(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            this._configuration = configuration;
        }


        public abstract void Map(object instance, string keyMapper);


        public abstract string Serialize(object instance);


        protected readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public abstract bool CanMap(Type type);

        public abstract bool CanSerialize(Type type);

    }

  

}
