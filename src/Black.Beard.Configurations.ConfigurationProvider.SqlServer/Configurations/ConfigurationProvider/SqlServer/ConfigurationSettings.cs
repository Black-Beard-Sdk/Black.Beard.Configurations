namespace Bb.Storages.ConfigurationProviders.SqlServer
{
    public class ConfigurationSettings
    {

        public ConfigurationSettings()
        {
            
        }

        public string SectionName { get; internal set; }

        public string Context { get; internal set; }

        public string Kind { get; internal set; }

        public int Version { get; internal set; }

        public string Value { get; internal set; }

        public DateTimeOffset? CreationDtm { get; internal set; }

        public DateTimeOffset LastUpdate { get; internal set; }

        public bool IsDirty { get; internal set; } = false;

        //public void Update(object instance)
        //{
        //    var result = ContentHelper.Serialize(instance, Newtonsoft.Json.Formatting.Indented);
        //    if (result != Value)
        //        IsDirty = true;
        //    Value = result; 
        //}


    }


}


/*
 
    SET ANSI_NULLS ON
    GO

    SET QUOTED_IDENTIFIER ON
    GO

CREATE TABLE [dbo].[settings](
	[SectionName] [varchar](100) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[Context] [varchar](100) NOT NULL,
	[Kind] [varchar](20) NOT NULL,
	[CreationDtm] [datetimeoffset](7) NOT NULL,
	[LastUpdate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_settings] PRIMARY KEY CLUSTERED 
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


 */