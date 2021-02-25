using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mad.yt.win
{

    public partial class ytForm : Form
    {
        SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindowsClass();

        public ytForm()
        {
            InitializeComponent();
            if(Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText();
                if(IsValid(clipboardText))
                {
                    RunYoutubeProcess(clipboardText);
                    Clipboard.Clear();
                    Close();
                }
            }
        }

        private void ytButton_Click(object sender, EventArgs e)
        {
            
        }

        private void RunYoutubeProcess(string url)
        {
            string saveLocation = UserConfiguration.Instance.SaveFolder.FullName;
            string fileNameTemplate = UserConfiguration.Instance.SingleFileNameFormat;

            if (url.Contains("/playlist?"))
            {
                fileNameTemplate = UserConfiguration.Instance.PlaylistFileNameFormat;
            }

            Process process = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //startInfo.CreateNoWindow = true;
            startInfo.FileName = "youtube-dl.exe";
            startInfo.Arguments = $"-o \"{saveLocation}\\{fileNameTemplate}\" -f bestaudio \"{url}\"";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            OpenMusicDirectory(saveLocation);
        }

        private void OpenMusicDirectory(string musicDirtectoryPath)
        {
            if (isMusicDirectoryOpen(musicDirtectoryPath))
            {
                return;
            }

            Process.Start("explorer.exe",musicDirtectoryPath);
        }

        private bool isMusicDirectoryOpen(string musicDirectoryPath)
        {
            var musicDirectoryURI = new System.Uri(musicDirectoryPath);

            string filename = "";
            foreach (SHDocVw.InternetExplorer ie in shellWindows)
            {
                filename = Path.GetFileNameWithoutExtension(ie.FullName).ToLower();
                if (filename.Equals("explorer") == false)
                {
                    continue;
                }

                var explorerDirectoryURI = new Uri(ie.LocationURL);
               // explorerDirectory.
                if(musicDirectoryURI == explorerDirectoryURI)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsValid(string text)
        {
            if(text.Length == 0)
            {
                return false;
            }
            if(text.Contains("youtube.com/") == false)
            {
                return false;
            }

            if((text.Contains("watch?v=") || text.Contains("playlist?list=")) == false)
            {
                return false;
            }

            return true;
        }

        private void urlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                Close();
                return;
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
            {
                string pasteText = Clipboard.GetText();
                if (IsValid(pasteText) == false)
                {
                    return;
                }
                RunYoutubeProcess(pasteText);
                Close();

                // cancel the "paste" function
                e.SuppressKeyPress = true;
                return;
            }
        }

        private void urlTextBox_TextChanged(object sender, EventArgs e)
        {
            urlTextBox.Text = "";
        }
    }
}
