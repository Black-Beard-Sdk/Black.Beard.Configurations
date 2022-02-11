using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Microsoft.Extensions.Configuration;


namespace Bb.Storages.ConfigurationProviders.SqlServer
{


    public class SqlServerConfigurationSource : IConfigurationSource
    {


        public SqlServerConfigurationSource(SqlServerConfigurationDataAccess dataAccess, int refreshInterval)
        {
            this.DataAccess = dataAccess;
            this._refreshInterval = refreshInterval;
        }


        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            var result = new SqlServerConfigurationProvider(this.DataAccess);
            this.DataAccess.Sql.InitializeWatcher(this._refreshInterval, result.Load);
            return result;
        }


        public SqlServerConfigurationDataAccess DataAccess { get; set; }

        private readonly int _refreshInterval;
    }



}