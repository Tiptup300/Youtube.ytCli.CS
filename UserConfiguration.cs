using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace mad.yt.win
{
    public class UserConfiguration
    {
        #region UserConfiguration Fields
        private DirectoryInfo _savefolder = new DirectoryInfo(@"C:\Users\mdeweese\Google Drive\music-youtube");
        private string _playlistFileNameFormat = @"%(playlist)s/%(playlist_index)s - %(title)s.%(ext)s";
        private string _singleFileNameFormat = "%(title)s.%(ext)s";

        public string SingleFileNameFormat
        {
            get { return _singleFileNameFormat; }
            set { _singleFileNameFormat = value; }
        }


        #endregion

        #region UserConfiguration Properties


        public DirectoryInfo SaveFolder
        {
            get { return _savefolder; }
            set { _savefolder = value; }
        }


        public string PlaylistFileNameFormat
        {
            get { return _playlistFileNameFormat; }
            set { _playlistFileNameFormat = value; }
        }


        #endregion

        public UserConfiguration()
        {
            _configFileName = Directory.GetCurrentDirectory() + "\\config.csv";
            LoadConfigurationFile();
        }

        public UserConfiguration(string configFileName)
        {
            _configFileName = configFileName;
            LoadConfigurationFile();
        }

        private void LoadConfigurationFile()
        {
            if (!File.Exists(_configFileName))
            {
                SaveConfigurationFile();
            }
            if (FileHelpers.IsFileLocked(_configFileName))
            {
                // use defaults
                return;
            }
            string[] fileData = File.ReadAllLines(_configFileName);
            if (fileData[0].ToLower().Trim() != _configCSVHeader)
            {
                SaveConfigurationFile();
                return;
            }
            for (int x = 1; x < fileData.Length; x++)
            {
                if (fileData[x].Trim() == "")
                {
                    continue;
                }
                string[] flds = FileHelpers.ParseCSVRow(fileData[x]);
                FieldInfo fieldInfo = this.GetType().GetField(flds[0], BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo is null)
                {
                    continue;
                }
                if (fieldInfo.FieldType == typeof(bool))
                {
                    bool value = false;
                    if (bool.TryParse(flds[1], out value))
                    {
                        fieldInfo.SetValue(this, value);
                    }
                }
                else if (fieldInfo.FieldType == typeof(int))
                {
                    int value = 0;
                    if (int.TryParse(flds[1], out value))
                    {
                        fieldInfo.SetValue(this, value);
                    }
                }
                else if (fieldInfo.FieldType == typeof(DirectoryInfo))
                {
                    DirectoryInfo value = new DirectoryInfo(flds[1]);
                    if (value.Exists)
                    {
                        fieldInfo.SetValue(this, value);
                    }
                }
                else if (fieldInfo.FieldType == typeof(string))
                {
                    fieldInfo.SetValue(this, flds[1]);
                }
            }
        }

        private void SaveConfigurationFile()
        {
            if (FileHelpers.IsFileLocked(_configFileName))
            {
                // if locked, dont save
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(_configCSVHeader);
            var fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Where(x => x.Name != nameof(_configFileName)).ToList();
            foreach (var field in fields)
            {
                string name = field.Name;
                string value = field.GetValue(this).ToString();
                sb.AppendLine($"{name},{value}");
            }
            File.WriteAllText(_configFileName, sb.ToString());
        }


        public static UserConfiguration Instance;
        private const string _configCSVHeader = "property,value";
        private string _configFileName;
    }
}
