using GalaSoft.MvvmLight.Command;
using MFormat.Model;
using MFormat.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MFormat.ViewModel
{
    public class LoadHighligtsDialogViewModel : INotifyPropertyChanged
    {
        public LoadHighlightsDialog view;
        public string RecordingFile { get; set; } = "";
        public string HighlightsFile { get; set; } = "";
        public RelayCommand RecordsBrowseCommand { get; set; }
        public RelayCommand HighlightsBrowseCommand { get; set; }
        public RelayCommand LoadHighlightsCommand { get; set; }
        public LoadHighligtsDialogViewModel()
        {
            RecordsBrowseCommand = new RelayCommand(() => RecordsBrowse());
            HighlightsBrowseCommand = new RelayCommand(() => HighlightsBrowse());
            LoadHighlightsCommand = new RelayCommand(() => LoadHighlights());
        }

        public void RecordsBrowse()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All Media Files|*.wav;*.aac;*.wma;*.wmv;*.avi;*.mpg;*.mpeg;*.m1v;*.mp2;*.mp3;*.mpa;*.mpe;*.m3u;*.mp4;*.mov;*.3g2;*.3gp2;*.3gp;*.3gpp;*.m4a;*.cda;*.aif;*.aifc;*.aiff;*.mid;*.midi;*.rmi;*.mkv;*.WAV;*.AAC;*.WMA;*.WMV;*.AVI;*.MPG;*.MPEG;*.M1V;*.MP2;*.MP3;*.MPA;*.MPE;*.M3U;*.MP4;*.MOV;*.3G2;*.3GP2;*.3GP;*.3GPP;*.M4A;*.CDA;*.AIF;*.AIFC;*.AIFF;*.MID;*.MIDI;*.RMI;*.MKV;";
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                RecordingFile = dialog.FileName;
                OnPropertyChanged("RecordingFile");
            }
        }

        public void HighlightsBrowse()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "XML files |*.xml;*.XML";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                HighlightsFile = dialog.FileName;
                OnPropertyChanged("HighlightsFile");
            }
        }

        public void LoadHighlights()
        { 
            if(HighlightsFile == "" || RecordingFile == "")
            {
                MessageBox.Show("Please enter highlights file and recording file");
                return;
            }
            Actions.Instance.LoadHighlights(RecordingFile, HighlightsFile);
            view.DialogResult = true;
            view.Close();

        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
