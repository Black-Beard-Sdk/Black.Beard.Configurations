using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.DataAnnotations;
using Bb.Sql;
using System.ComponentModel;

namespace Bb.Storages.ConfigurationProviders.SqlServer
{

    [ExposeClass(ConstantsCore.Initialization, LifeCycle = IocScopeEnum.Transiant)]
    [Configuration(ConfigurationKey = "TranslationsConfiguration", TypeSerialisation = ConfigurationAttribute.TypeSerialisationToJson)]
    [TranslationKey(BaseConfiguration.MenuList, "p:TypeName,k:Configuration,l:en-us,d:Manage configuration")]
    public class BaseConfiguration
    {

        public BaseConfiguration(ConnectionStringSetting initialeConnectionStringSetting)
        {
            this._settings = initialeConnectionStringSetting;
        }


        [Description("p:BaseConnection,k:TableName,l:en-us,d:Name of the table That contains translations")]
        [DefaultValue("Server=.;Database=BaseWebsite;Integrated Security=SSPI;Encrypt=true; TrustServerCertificate=true;")]
        public string TableName { get; set; } = "settings";


        [Description("p:BaseConnection,k:RefreshInterval,l:en-us,d:Name of the table That contains translations")]
        public int RefreshInterval { get; set; } = 1 * 60;

        public ConnectionStringSetting GetConnection()
        {
            return _settings;
        }

        internal const string MenuList = "MenuList";
        private readonly ConnectionStringSetting _settings;

    }


}


