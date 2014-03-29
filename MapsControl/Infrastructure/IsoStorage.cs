using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MapsControl.Infrastructure
{
    public interface IIsoStorage
    {
        bool FileExists(string relativePath);

        string GetIsoStorageAbsolutePath(string relativePath);
    }

    public class IsoStorage : IIsoStorage
    {
        #region Fields

        private readonly StorageFolder _localStorageFolder = ApplicationData.Current.LocalFolder;

        #endregion
        
        #region IIsoStorage

        public bool FileExists(string relativePath)
        {
            using (var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return isolatedStorage.FileExists(relativePath);
            }
        }

        public string GetIsoStorageAbsolutePath(string relativePath)
        {
            string localFolderPath = _localStorageFolder.Path;
            return Path.Combine(localFolderPath, relativePath);
        }

        #endregion
    }
}
