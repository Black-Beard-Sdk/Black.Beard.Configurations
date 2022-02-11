namespace Bb.Storages.ConfigurationProviders.SqlServer
{
    public class ConfigurationHasChangedEventArgs : EventArgs
    {
        public ConfigurationSettings? Item { get; internal set; }
    }


}


