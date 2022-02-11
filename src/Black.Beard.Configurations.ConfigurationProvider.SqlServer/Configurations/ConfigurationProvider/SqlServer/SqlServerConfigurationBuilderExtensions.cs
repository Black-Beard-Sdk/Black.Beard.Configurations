using Microsoft.Extensions.Configuration;


namespace Bb.Storages.ConfigurationProviders.SqlServer
{

    public static class SqlServerConfigurationBuilderExtensions
    {
     
        public static SqlServerConfigurationSource GetSqlServerProvider(BaseConfiguration configurationConnexion)
        {
            var dataAccess = new SqlServerConfigurationDataAccess(configurationConnexion);
            var source = new SqlServerConfigurationSource(dataAccess, configurationConnexion.RefreshInterval);
            return source;

        }
    }

}