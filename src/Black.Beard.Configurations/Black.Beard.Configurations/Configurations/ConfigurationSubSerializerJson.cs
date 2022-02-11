using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace Bb.Configurations
{


    // [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(ConfigurationJSonSerializer))]
    public class ConfigurationSubSerializerJson : ConfigurationSubSerializer
    {

        public ConfigurationSubSerializerJson(IConfiguration configuration) 
            : base(configuration)
        {

        }


        public override bool CanMap(Type type)
        {
            return true;
        }


        public override bool CanSerialize(Type type)
        {
            return true;
        }


        public override string Serialize(object instance)
        {
            return instance.Serialize(Newtonsoft.Json.Formatting.Indented);
        }


        public override void Map(object instance, string keyMapper)
        {
            var datas = _configuration.GetSection(keyMapper);
            if (datas != null && datas.Value != null)
            {
                try
                {
                    instance.Map(datas.Value);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
            }
        }


    }

}
