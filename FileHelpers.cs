using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace mad.yt.win
{
    public static class FileHelpers
    {

        public static bool IsFileLocked(string filePath)
        {
            try
            {
                using (File.Open(filePath, FileMode.Open)) { }
            }
            catch (IOException e)
            {
                var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);

                return errorCode == 32 || errorCode == 33;
            }

            return false;
        }
        public static string[] ParseCSVRow(string csvrow)
        {
            //Define pattern
            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            string[] results = CSVParser.Split(csvrow);

            for (int x = 0; x < results.Length; x++)
            {
                results[x] = results[x].Replace("[DOUBLE-QUOTE]", "\"");
            }

            //Separating columns to array
            return results.ToList().Select(x => x.RemoveApostrophes()).ToArray();
        }
        public static string RemoveApostrophes(this string str)
        {
            if (str.Length < 2)
            {
                return str;
            }

            if (str[0] == '"' && str[str.Length - 1] == '"')
            {
                return str.Substring(1, str.Length - 2);
            }

            return str;
        }
    }
}
