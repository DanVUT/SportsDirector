using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MFORMATSLib;

namespace MFormat.View
{
    /// <summary>
    /// Interaction logic for InputStreamDialog.xaml
    /// </summary>
    public partial class InputDeviceDialog : Window
    {
        MFLiveClass liveClass;
        public InputDeviceDialog()
        {
            InitializeComponent();
            InitializeVideoInputSources();
            Loaded += InputDeviceDialog_Loaded;
            Closing += ClosingDialog;
        }

        private void ClosingDialog(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DialogResult == false || this.DialogResult == null)
            {
                if (this.liveClass != null)
                {
                    this.liveClass.DeviceClose();
                }
            }

            if(this.DialogResult == true)
            {
                if(this.streamUrl.Text != "")
                {
                    this.liveClass.DeviceClose();
                    this.liveClass = null;
                }
            }
        }

        private void InputDeviceDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.videoSettingsEnabledButton.IsChecked = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        //Populate video sources combo box 
        private void InitializeVideoInputSources()
        {
            try
            {
                liveClass = new MFLiveClass();
                int deviceCount;
                liveClass.DeviceGetCount(eMFDeviceType.eMFDT_Video, out deviceCount);

                if (deviceCount > 0)
                {
                    for (int i = 0; i < deviceCount; i++)
                    {
                        string deviceName;
                        int busy;
                        liveClass.DeviceGetByIndex(eMFDeviceType.eMFDT_Video, i, out deviceName, out busy);
                        this.videoInputComboBox.Items.Add(deviceName);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        //After video input selection, populate other combo boxes
        private void videoInputComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            liveClass.DeviceSet(eMFDeviceType.eMFDT_Video, videoInputComboBox.SelectedIndex, "");
            populateVideoLineComboBox();
            populateVideoFormatsComboBox();
            populateAudioInternalComboBox();
            populateAudioExternalComboBox();
            populateAudioFormatsComboBox();
        }

        //Populate Line-in combo box
        private void populateVideoLineComboBox()
        {
            this.videoLineComboBox.Items.Clear();
            string current;
            string options;
            string help;
            int selected = -1;
            int lineCount;
            int node;

            this.liveClass.PropsGet("line-in", out current);

            this.liveClass.PropsOptionGetCount("line-in", out lineCount);
            for(int i = 0; i<lineCount; i++)
            {
                liveClass.PropsOptionGetByIndex("line-in", i, out options, out help);
                this.videoLineComboBox.Items.Add(help);
                if(current == options)
                {
                    selected = i;
                }

                if (selected >= 0)
                    this.videoLineComboBox.SelectedIndex = selected;
                else
                    this.videoLineComboBox.SelectedIndex = 0;
            }
        }

        //Populate Video Formats Combo Box
        private void populateVideoFormatsComboBox()
        {
            this.videoFormatComboBox.Items.Clear();
            int formatsCount;

            this.liveClass.FormatVideoGetCount(eMFormatType.eMFT_Input, out formatsCount);

            for (int i = 0; i < formatsCount; i++)
            {
                M_VID_PROPS mVidProps;
                string formatName;
                this.liveClass.FormatVideoGetByIndex(eMFormatType.eMFT_Input, i, out mVidProps, out formatName);
                this.videoFormatComboBox.Items.Add(formatName);
            }

            string formName;
            M_VID_PROPS videoProps;
            int currentFormat;
            liveClass.FormatVideoGet(eMFormatType.eMFT_Input, out videoProps, out currentFormat, out formName);

            this.videoFormatComboBox.SelectedIndex = currentFormat >= 0 ? currentFormat : 0;
        }
        //Populate Audio Internal Combo Box
        private void populateAudioInternalComboBox()
        {
            this.audioInternalComboBox.Items.Clear();
            int audioDeviceCount;
            liveClass.DeviceGetCount(eMFDeviceType.eMFDT_Audio, out audioDeviceCount);

            for(int i = 0; i < audioDeviceCount; i++)
            {
                string strName;
                int pbBusy;
                liveClass.DeviceGetByIndex(eMFDeviceType.eMFDT_Audio, i, out strName, out pbBusy);
                this.audioInternalComboBox.Items.Add(strName);


            }
            string name = "";
            int nIndex = 0;
            try
            {
                liveClass.DeviceGet(eMFDeviceType.eMFDT_Audio, out nIndex, out name);
                this.audioInternalComboBox.SelectedIndex = nIndex >= 0 && nIndex < this.audioInternalComboBox.Items.Count - 1 ? nIndex : 0;
            }
            catch
            {
                this.audioInternalComboBox.SelectedIndex = this.audioInternalComboBox.Items.Count > 1 ? this.audioInternalComboBox.Items.Count - 2 : 0;
            }
        }
        //Populate Audio External Combo Box
        private void populateAudioExternalComboBox()
        {
            this.audioExternalComboBox.Items.Clear();
            int audioDeviceCount;
            liveClass.DeviceGetCount(eMFDeviceType.eMFDT_ExtAudio, out audioDeviceCount);

            for (int i = 0; i < audioDeviceCount; i++)
            {
                string strName;
                int pbBusy;
                liveClass.DeviceGetByIndex(eMFDeviceType.eMFDT_ExtAudio, i, out strName, out pbBusy);
                this.audioExternalComboBox.Items.Add(strName);


            }
            string name = "";
            int nIndex = 0;
            try
            {
                liveClass.DeviceGet(eMFDeviceType.eMFDT_ExtAudio, out nIndex, out name);
                this.audioExternalComboBox.SelectedIndex = nIndex >= 0 && nIndex < this.audioExternalComboBox.Items.Count - 1 ? nIndex : 0;
            }
            catch
            {
                this.audioExternalComboBox.SelectedIndex = 0;
            }
        }
        //Populate Audio Formats Combo Box
        private void populateAudioFormatsComboBox()
        {
            this.audioFormatComboBox.Items.Clear();

            int nCount = 0;
            string strName;
            liveClass.FormatAudioGetCount(eMFormatType.eMFT_Input, out nCount);

            M_AUD_PROPS audProps;
            for (int i = 0; i < nCount; i++)
            {
                liveClass.FormatAudioGetByIndex(eMFormatType.eMFT_Input, i, out audProps, out strName);
                this.audioFormatComboBox.Items.Add(strName);
            }

            int nCurrent = 0;
            try
            {
                string strName_;
                M_AUD_PROPS _audProps;
                liveClass.FormatAudioGet(eMFormatType.eMFT_Input, out _audProps, out nCurrent, out strName_);
            }
            catch (System.Exception)
            {
            }
            this.audioFormatComboBox.SelectedIndex = nCurrent >= 0 ? nCurrent : 0;
        }

        //Reaction to selection changed in video line-in combo box
        private void videoLineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.videoLineComboBox.Items.Count > 0)
            {
                this.liveClass.PropsOptionSetByIndex("line-in", this.videoLineComboBox.SelectedIndex);
            }
        }
        //Reaction to selection changed in video format combo box
        private void videoFormatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.videoFormatComboBox.Items.Count > 0)
            {
                M_VID_PROPS vidProps;
                string name;
                liveClass.FormatVideoGetByIndex(eMFormatType.eMFT_Input, this.videoFormatComboBox.SelectedIndex, out vidProps, out name);
                liveClass.FormatVideoSet(eMFormatType.eMFT_Input, vidProps);
            }
        }
        //Reaction to selection changed in Internal Audio Combo Box
        private void audioInternalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                liveClass.DeviceSet(eMFDeviceType.eMFDT_Audio, this.audioInternalComboBox.SelectedIndex, "");
            }
            catch { }
        }
        //Reaction to selection in External Audio Combo Box
        private void audioExternalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                liveClass.DeviceSet(eMFDeviceType.eMFDT_ExtAudio, this.audioExternalComboBox.SelectedIndex, "");
            }
            catch { }
        }
        //Reaction to selection in Audio Format Combo Box
        private void audioFormatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.audioFormatComboBox.Items.Count > 0)
            {
                M_AUD_PROPS audProps;
                string strName;
                liveClass.FormatAudioGetByIndex(eMFormatType.eMFT_Input, this.audioFormatComboBox.SelectedIndex, out audProps, out strName);
                liveClass.FormatAudioSet(eMFormatType.eMFT_Input, audProps);
            }
        }
        //GUI Backend for changing radio button
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            this.videoSettings.Visibility = Visibility.Visible;
            this.audioSettings.Visibility = Visibility.Visible;
            this.streamSettings.Visibility = Visibility.Collapsed;
        }
        //GUI Backend for changing radio button
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            this.videoSettings.Visibility = Visibility.Collapsed;
            this.audioSettings.Visibility = Visibility.Collapsed;
            this.streamSettings.Visibility = Visibility.Visible;
        }

        public MFLiveClass getVideoDevice()
        {
            if(this.streamUrl.Text != "")
            {
                return null;
            }
            return this.liveClass;
        }

        public string getStreamUrl()
        {
            return this.streamUrl.Text;
        }


    }
}
