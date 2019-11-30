using GalaSoft.MvvmLight.Command;
using MFormat.Model;
using MFormat.View;
using MFORMATSLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MFormat.ViewModel
{
    public class RecordingSettingsDialogViewModel : INotifyPropertyChanged
    {
        //Reference to Recording Settings Dialog view
        public RecordingSettingsDialog view;

        //Video Formats List (Binding Property)
        private ObservableCollection<eMVideoFormat> videoFormatsList = null;
        public ObservableCollection<eMVideoFormat> VideoFormatsList {
            get
            {
                return this.videoFormatsList;
            }
            set
            {
                this.videoFormatsList = value;
                OnPropertyChanged("VideoFormatsList");
            }
        }

        //Video file formats list (Binding Property)
        private ObservableCollection<string> formatsList = null;
        public ObservableCollection<string> FormatsList
        {
            get
            {
                return this.formatsList;
            }
            set
            {
                this.formatsList = value;
                OnPropertyChanged("FormatsList");
            }
        }

        //Audio Codec List (Binding Property)
        private ObservableCollection<string> audioCodecList = null;
        public ObservableCollection<string> AudioCodecList
        {
            get
            {
                return this.audioCodecList;
            }
            set
            {
                this.audioCodecList = value;
                OnPropertyChanged("AudioCodecList");
            }
        }
        //Video Codec List (Binding Property)
        private ObservableCollection<string> videoCodecList = null;
        public ObservableCollection<string> VideoCodecList
        {
            get
            {
                return this.videoCodecList;
            }
            set
            {
                this.videoCodecList = value;
                OnPropertyChanged("VideoCodecList");
            }
        }
        //Selected Video Format (Binding Property)
        private eMVideoFormat selectedVideoFormat = eMVideoFormat.eMVF_Custom;
        public eMVideoFormat SelectedVideoFormat
        {
            get
            {
                return this.selectedVideoFormat;
            }
            set
            {
                this.selectedVideoFormat = value;
                this.avProps.vidProps.eVideoFormat = value;
                OnPropertyChanged("SelectedVideoFormat");
            }
        }
        //Selected Format Index (Binding Property)
        private int selectedFormatIndex = 0;
        public int SelectedFormatIndex
        {
            get
            {
                return this.selectedFormatIndex;
            }
            set
            {
                this.selectedFormatIndex = value;
                IMFProps props;
                this.recorder.WriterOptionSetByIndex(eMFWriterOption.eMFWO_Format, value, out props);
                InitializeVideoCodecs();
                InitializeAudioCodecs();
                OnPropertyChanged("SelectedFormatIndex");
            }
        }
        //Selected Video Codec Index (Binding Property)
        private int selectedVideoCodecIndex = 0;
        public int SelectedVideoCodecIndex
        {
            get
            {
                return this.selectedVideoCodecIndex;
            }
            set
            {
                this.selectedVideoCodecIndex = value;
                IMFProps props;
                this.recorder.WriterOptionSetByIndex(eMFWriterOption.eMFWO_VideoCodec, value, out props);
                OnPropertyChanged("SelectedVideoCodecIndex");
            }
        }
        //Selected Audio Codec Index (Binding Property)
        private int selectedAudioCodecIndex = 0;
        public int SelectedAudioCodecIndex
        {
            get
            {
                return this.selectedAudioCodecIndex;
            }
            set
            {
                this.selectedAudioCodecIndex = value;
                IMFProps props;
                this.recorder.WriterOptionSetByIndex(eMFWriterOption.eMFWO_AudioCodec, value, out props);
                OnPropertyChanged("SelectedAudioCodecIndex");
            }
        }
        //Destination File (Binding Property)
        private string destinationFile = "";
        public string DestinationFile
        {
            get
            {
                return this.destinationFile;
            }
            set
            {
                this.destinationFile = value;
                OnPropertyChanged("DestinationFile");
            }
        }

        //New instance for MFWriter
        private MFWriterClass recorder = new MFWriterClass();
        //Reference for broadcastPlayer for setting writer directly to broadcastManager 
        public BroadcastManager broadcastPlayer;
        //AudioVideo recording options
        private M_AV_PROPS avProps = new M_AV_PROPS();
        public RelayCommand OpenSaveFileDialogCommand { get; set; }
        public RelayCommand SetRecorderCommand { get; set; }

        public RecordingSettingsDialogViewModel()
        {
            OpenSaveFileDialogCommand = new RelayCommand(() => OpenSaveFileDialog());
            SetRecorderCommand = new RelayCommand(() => SetRecorder());
            InitializeVideoFormats();
            InitializeFormats();
            InitializeVideoCodecs();
            InitializeAudioCodecs();
        }
        //Initializes Video Formats Combo Box
        private void InitializeVideoFormats()
        {
            if(this.VideoFormatsList != null)
            {
                this.VideoFormatsList.Clear();
            }
            this.VideoFormatsList = new ObservableCollection<eMVideoFormat>();

            this.VideoFormatsList.Add(eMVideoFormat.eMVF_Custom);

            this.VideoFormatsList.Add(eMVideoFormat.eMVF_NTSC);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_NTSC_2398);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_NTSC_16x9);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_PAL);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_PAL_16x9);

            this.VideoFormatsList.Add(eMVideoFormat.eMVF_HD720_50p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_HD720_5994p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_HD720_60p);

            this.VideoFormatsList.Add(eMVideoFormat.eMVF_HD1080_2398p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_HD1080_24p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_HD1080_25p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_HD1080_2997p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_HD1080_30p);

            this.VideoFormatsList.Add(eMVideoFormat.eMVF_HD1080_50i);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_HD1080_5994i);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_HD1080_60i);

            this.VideoFormatsList.Add(eMVideoFormat.eMVF_HD1080_50p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_HD1080_5994p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_HD1080_60p);

            this.VideoFormatsList.Add(eMVideoFormat.eMVF_2K_DCI_2398p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_2K_DCI_24p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_2K_DCI_25p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_2K_DCI_50p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_2K_DCI_5994p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_2K_DCI_60p);

            this.VideoFormatsList.Add(eMVideoFormat.eMVF_2K_2398p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_2K_24p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_2K_25p);

            this.VideoFormatsList.Add(eMVideoFormat.eMVF_4K_DCI_2398p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_4K_DCI_24p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_4K_DCI_25p);

            this.VideoFormatsList.Add(eMVideoFormat.eMVF_4K_UHD_50i);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_4K_UHD_5994i);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_4K_UHD_60i);

            this.VideoFormatsList.Add(eMVideoFormat.eMVF_4K_UHD_50p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_4K_UHD_5994p);
            this.VideoFormatsList.Add(eMVideoFormat.eMVF_4K_UHD_60p);
            OnPropertyChanged("VideoFormatsList");

            this.SelectedVideoFormat = this.VideoFormatsList[0];

        }
        //Initializes Formats Combo Box
        private void InitializeFormats()
        {
            int formatCount;
            this.recorder.WriterOptionGetCount(eMFWriterOption.eMFWO_Format, out formatCount);
            if(this.FormatsList != null)
            {
                this.FormatsList.Clear();
            }
            this.FormatsList = new ObservableCollection<string>();

            for (int i = 0; i < formatCount; i++)
            {
                string shortname, longname;
                this.recorder.WriterOptionGetByIndex(eMFWriterOption.eMFWO_Format, i, out shortname, out longname);
                this.FormatsList.Add(longname);
            }
            this.SelectedFormatIndex = 0;
            OnPropertyChanged("FormatsList");
        }
        //Initializes Video Codecs Combo Box
        private void InitializeVideoCodecs()
        {
            if(this.VideoCodecList != null)
            {
                this.VideoCodecList.Clear();
            }
            this.VideoCodecList = new ObservableCollection<string>();

            int videoCodecsCount;
            this.recorder.WriterOptionGetCount(eMFWriterOption.eMFWO_VideoCodec, out videoCodecsCount);

            for (int i = 0; i < videoCodecsCount; i++)
            {
                string shortname, longname;
                this.recorder.WriterOptionGetByIndex(eMFWriterOption.eMFWO_VideoCodec, i, out shortname, out longname);
                this.VideoCodecList.Add(longname);
            }
            this.SelectedVideoCodecIndex = 0;
            OnPropertyChanged("VideoCodecList");
        }
        //Initializes Audio Codecs
        private void InitializeAudioCodecs()
        {
            if (this.AudioCodecList != null)
            {
                this.AudioCodecList.Clear();
            }
            this.AudioCodecList = new ObservableCollection<string>();

            int audioCodecsCount;
            this.recorder.WriterOptionGetCount(eMFWriterOption.eMFWO_AudioCodec, out audioCodecsCount);

            for (int i = 0; i < audioCodecsCount; i++)
            {
                string shortname, longname;
                this.recorder.WriterOptionGetByIndex(eMFWriterOption.eMFWO_AudioCodec, i, out shortname, out longname);
                this.AudioCodecList.Add(longname);
            }
            this.SelectedAudioCodecIndex = 0;
            OnPropertyChanged("AudioCodecList");
        }

        //Set destination of file for recording
        private void OpenSaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            IMFProps props;
            string format;
            this.recorder.WriterOptionGet(eMFWriterOption.eMFWO_Format, out format, out props);

            string extentions;
            props.PropsGet("extensions", out extentions);

            if(format == "Auto Select")
            {
                saveFileDialog.Filter = "MPEG files (*.mp4)|*.mp4|All Files (*.*)|*.*";
            }
            else
            {
                string[] arrExt = extentions.Split(',');
                string strFilterExt = ""; 
                for (int i = 0; i < arrExt.Length; i++)
                {
                    if (strFilterExt.Length > 0)
                        strFilterExt += ";*." + arrExt[i];
                    else
                        strFilterExt = "*." + arrExt[i];
                }

                saveFileDialog.Filter = format + " Files (" + strFilterExt + ")|" + strFilterExt + "|All Files (*.*)|*.*";
            }
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.DestinationFile = saveFileDialog.FileName;
            }
        }
        //Set recorder to video player
        private void SetRecorder()
        {
            if(this.view.recordFile.Text == "")
            {
                MessageBox.Show("Select destination file first");
                return;
            }

            IMFProps useless;
            string format;
            this.recorder.WriterOptionGet(eMFWriterOption.eMFWO_Format, out format, out useless);

            string target, options; 
            if(format.Equals("Auto Select"))
            {
                options = " format='mp4' video::codec='mpeg4' video::b='5M' audio::codec='aac'";
            } else
            {
                this.recorder.WriterGet(out target, out options);
            }
            this.broadcastPlayer.SetRecorder(this.recorder, this.avProps, this.DestinationFile, options);
            view.DialogResult = true;
            view.Close();
        }


        public event PropertyChangedEventHandler PropertyChanged; // default implementation of INotifyPropertyChanged interface

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
