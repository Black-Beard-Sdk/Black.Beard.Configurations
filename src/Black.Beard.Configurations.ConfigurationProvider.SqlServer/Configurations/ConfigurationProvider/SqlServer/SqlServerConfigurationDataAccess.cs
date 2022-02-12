using Bb.Sql;
using System.Data.Common;
using System.Data.SqlClient;

namespace Bb.Storages.ConfigurationProviders.SqlServer
{

    public class SqlServerConfigurationDataAccess : IDisposable
    {

        public SqlServerConfigurationDataAccess(ProviderSqlServerBaseConfiguration connection)
        {
            this._tableName = connection.TableName;
            var cnx = connection.GetConnection() ?? throw new NullReferenceException(nameof(connection));
            Sql = SqlProcessor.GetSqlProcessor(cnx);
        }

        public DateTimeOffset? LastUpdate { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Sql?.Dispose();
                }

                disposedValue = true;
            }
        }

        // // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour libérer les ressources non managées
        // ~SqlServerConfigurationDataAccess()
        // {
        //     // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        public ConfigurationSettings GetNew(string sectionName, string context, string kind)
        {
            return new ConfigurationSettings()
            {
                SectionName = sectionName,
                Context = context,
                Kind = kind,
            };
        }


        public bool InsertConfiguration(ConfigurationSettings settings)
        {

            var results = Sql.ExecuteNonQuery(
                   GetSql(_sql_history_Insert),
                    Sql.GetParameter("sectionName", settings.SectionName),
                    Sql.GetParameter("context", settings.Context),
                    Sql.GetParameter("kind", settings.Kind),
                    Sql.GetParameter("version", 1),
                    Sql.GetParameter("value", settings.Value)
                   );

            if (HasChanged != null)
                HasChanged(this, new ConfigurationHasChangedEventArgs() { Item = null });

            return results.InpactedObject > 0;

        }


        public bool UpdateConfiguration(ConfigurationSettings settings)
        {

            var results = Sql.ExecuteNonQuery(
                GetSql(_sql_history_Update),
                Sql.GetParameter("sectionName", settings.SectionName),
                Sql.GetParameter("value", settings.Value),
                Sql.GetParameter("version", settings.Version)
                );

            if (results.InpactedObject > 0)
            {

                var newConfig = LoadConfiguration(settings.SectionName);
                if (newConfig != null)
                {
                    settings.LastUpdate = newConfig.LastUpdate;
                    settings.Version = newConfig.Version;
                    settings.Value = newConfig.Value;
                    settings.IsDirty = false;
                    
                    if (HasChanged != null)
                        HasChanged(this, new ConfigurationHasChangedEventArgs() { Item = newConfig });

                    return true;
                }
            }

            return false;

        }

        public EventHandler<ConfigurationHasChangedEventArgs> HasChanged { get; set; }


        public ConfigurationSettings? LoadConfiguration(string sectionName)
        {

            var queryString = GetSql(_sql_history_selectAll) + " WHERE [SectionName] = @sectionName";
            var argument = Sql.GetParameter("sectionName", sectionName);

            foreach (var item in Sql.Read(queryString, argument))
            {

                var row = new ConfigurationSettings()
                {
                    SectionName = item.GetString(item.GetOrdinal("SectionName")),
                    Context = item.GetString(item.GetOrdinal("Context")),
                    Kind = item.GetString(item.GetOrdinal("Kind")),
                    Version = item.GetInt32(item.GetOrdinal("Version")),
                    Value = item.GetString(item.GetOrdinal("Value")),
                    CreationDtm = item.GetDateTime(item.GetOrdinal("CreationDtm")),
                    LastUpdate = item.GetDateTime(item.GetOrdinal("LastUpdate")),
                };

                CheckLastDate(row);

                return row;

            }

            return null;

        }


        public Dictionary<string, ConfigurationSettings> LoadConfigurations()
        {

            var datas = new Dictionary<string, ConfigurationSettings>();

            var query = GetSql(_sql_history_selectAll);

            DbParameter? parameter = null;
            if (LastUpdate.HasValue)
            {
                query += " WHERE [LastUpdate] = @lastUpdate";
                parameter = Sql.GetParameter("lastUpdate", LastUpdate.Value);
            }

            foreach (var item in Sql.Read(query, parameter))
            {

                var row = new ConfigurationSettings()
                {
                    SectionName = item.GetString(item.GetOrdinal("SectionName")),
                    Context = item.GetString(item.GetOrdinal("Context")),
                    Kind = item.GetString(item.GetOrdinal("Kind")),
                    Version = item.GetInt32(item.GetOrdinal("Version")),
                    Value = item.GetString(item.GetOrdinal("Value")),
                    CreationDtm = item.GetDateTime(item.GetOrdinal("CreationDtm")),
                    LastUpdate = item.GetDateTime(item.GetOrdinal("LastUpdate")),
                };

                CheckLastDate(row);

                datas.Add(row.SectionName, row);

            }

            return datas;
        }

        //public bool CreateTables()
        //{
        //    var results = Sql.ExecuteNonQuery(GetSql(_sql_tables_create));
        //    return results.Success;
        //}

        private string GetSql(string sql)
        {
            return sql.Replace("%TableName%", this._tableName);
        }

        private void CheckLastDate(ConfigurationSettings row)
        {

            if (row.LastUpdate > this.LastUpdate || !this.LastUpdate.HasValue)
                this.LastUpdate = row.LastUpdate;

        }

        private readonly string _tableName;
        public SqlProcessor Sql { get; }

        private string _sql_history_Insert = @"

INSERT INTO [dbo].[%TableName%] ([SectionName], [Context], [Kind], [Version], [Value], [CreationDtm], [LastUpdate]) 
VALUES (@sectionName, @context, @kind, @version, @value, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET())
GO
INSERT INTO [dbo].[%TableName%_history] ([SectionName], [Context], [Kind], [Version], [Value], [CreationDtm]) 
VALUES (@sectionName, @context, @kind, @version, @value, SYSDATETIMEOFFSET())

";
        private string _sql_history_Update = "UPDATE [dbo].[%TableName%] SET [Value] = @value, [LastUpdate] = SYSDATETIMEOFFSET(), [Version] = @version + 1  WHERE [SectionName]=@sectionName AND [Version] = @version";
        private string _sql_history_selectAll = "SELECT [SectionName], [Context], [Kind], [Version], [Value], [CreationDtm], [LastUpdate] FROM [%TableName%] WITH (NOLOCK)";
        public const string Sql_tables_create =
 @"

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[%TableName%](
	[SectionName] [varchar](100) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[Context] [varchar](100) NOT NULL,
	[Kind] [varchar](20) NOT NULL,
	[Version] [int] NOT NULL,
	[CreationDtm] [datetime] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_%TableName%] PRIMARY KEY CLUSTERED 
(
	[SectionName] ASC
)WITH 
	(PAD_INDEX = OFF
	, STATISTICS_NORECOMPUTE = OFF
	, IGNORE_DUP_KEY = OFF
	, ALLOW_ROW_LOCKS = ON
	, ALLOW_PAGE_LOCKS = ON
	, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
	) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [NonClusteredIndexLastUpdate] ON [dbo].[%TableName%]
(
	[LastUpdate] ASC
)WITH 
    ( PAD_INDEX = OFF
    , STATISTICS_NORECOMPUTE = OFF
    , SORT_IN_TEMPDB = OFF
    , DROP_EXISTING = OFF
    , ONLINE = OFF
    , ALLOW_ROW_LOCKS = ON
    , ALLOW_PAGE_LOCKS = ON
    , OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[%TableName%_history](
	[_id_history] [bigint] IDENTITY(1,1) NOT NULL,
	[SectionName] [varchar](100) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[Context] [varchar](100) NOT NULL,
	[Kind] [varchar](20) NOT NULL,
	[Version] [int] NOT NULL,
	[CreationDtm] [datetime] NOT NULL,
 CONSTRAINT [PK_settings_history] PRIMARY KEY CLUSTERED 
(
	[_id_history] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

";

        private bool disposedValue;


    }


}


