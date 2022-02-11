using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel;

namespace Bb.Storages.ConfigurationProviders.SqlServer
{


    public class SqlServerConfigurationProvider : ConfigurationProvider, IDisposable
    {

        public SqlServerConfigurationProvider(SqlServerConfigurationDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _dataAccess.HasChanged += ConfiguraitonHasChanged;
        }

        private void ConfiguraitonHasChanged(object? sender, ConfigurationHasChangedEventArgs e)
        {

            var item = e.Item;
            if (item != null)
            {
                lock (_lock)
                {
                    if (Data.ContainsKey(item.SectionName))
                        Data[item.SectionName] = item.Value;
                    else
                        Data.Add(item.SectionName, item.Value);
                }
            }
            else
                Load();

        }

        public override void Load()
        {

            if (Data == null)
                Data = new Dictionary<string, string>();

            lock (_lock)
            {
                var datas = _dataAccess.LoadConfigurations();

                foreach (var item in datas.Values)
                    if (Data.TryGetValue(item.SectionName, out string value))
                        Data[item.SectionName] = item.Value;
                    else
                        Data.Add(item.SectionName, item.Value);
            }

        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {

                if (disposing)
                {
                    _dataAccess.HasChanged -= ConfiguraitonHasChanged;
                    _dataAccess.Dispose();
                }

                // TODO: libérer les ressources non managées (objets non managés) et substituer le finaliseur
                // TODO: affecter aux grands champs une valeur null
                disposedValue = true;

            }
        }

        // // TODO: substituer le finaliseur uniquement si 'Dispose(bool disposing)' a du code pour libérer les ressources non managées
        // ~SqlServerConfigurationProvider()
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

        private readonly SqlServerConfigurationDataAccess _dataAccess;
        private bool disposedValue;
        private volatile object _lock = new object();

    }
}




