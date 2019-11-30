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
    public class SettingsDialogViewModel
    {
        //SettingsDialog View for UI elements reference
        public SettingsDialog view;

        public string VideosFolder { get; set; }
        public string ImagesFolder { get; set; }
        public string OverlaysFolder { get; set; }
        public int ShortBefore { get; set; }
        public int ShortAfter { get; set; }
        public int MediumBefore { get; set; }
        public int MediumAfter { get; set; }
        public int LongBefore { get; set; }
        public int LongAfter { get; set; }

        //Command for opening folder browser for Media Folder
        public RelayCommand BrowseVideoFolderCommand { get; set; }
        //Command for opening folder browser fo Overlay Folder 
        public RelayCommand BrowseOverlayFolderCommand { get; set; }

        public RelayCommand BrowseImageFolderCommand { get; set; }
        //Command for saving settings 
        public RelayCommand SaveSettingsCommand { get; set; }
        //Command for initializating values on window opening
        //Constructor, just creating relations between methods and commands
        public SettingsDialogViewModel()
        {
            BrowseImageFolderCommand = new RelayCommand(() => BrowseImageFolder());
            BrowseVideoFolderCommand = new RelayCommand(() => BrowseVideoFolder());
            BrowseOverlayFolderCommand = new RelayCommand(() => BrowseOverlayFolder());
            SaveSettingsCommand = new RelayCommand(() => SaveSettings());
            InitializeTextboxValues();
        }

        //Method initializating text boxes based on values from Settings class
        public void InitializeTextboxValues()
        {
            if (Settings.Instance.OverlayFolder != null) {
                this.OverlaysFolder = Settings.Instance.OverlayFolder;
            }
            if (Settings.Instance.VideosFolder != null)
            {
                this.VideosFolder = Settings.Instance.VideosFolder;
            }
            if (Settings.Instance.ImagesFolder != null)
            {
                this.ImagesFolder = Settings.Instance.ImagesFolder;
            }

            this.ShortBefore = Actions.Instance.ShortBefore;
            this.ShortAfter = Actions.Instance.ShortAfter;
            this.MediumBefore = Actions.Instance.MediumBefore;
            this.MediumAfter = Actions.Instance.MediumAfter;
            this.LongBefore = Actions.Instance.LongBefore;
            this.LongAfter = Actions.Instance.LongAfter;
        }

        //Method opening folder browser and getting new media folder value
        public void BrowseImageFolder()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if(folderDialog.ShowDialog() == DialogResult.OK)
            {
                this.ImagesFolder = folderDialog.SelectedPath;
            }
        }

        public void BrowseVideoFolder()
        {
            FolderBrowserDialog videoDialog = new FolderBrowserDialog();
            if (videoDialog.ShowDialog() == DialogResult.OK)
            {
                this.VideosFolder = videoDialog.SelectedPath;
            }
        }
        //Method opening folder browser and getting new overlay folder value
        public void BrowseOverlayFolder()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
               this.OverlaysFolder = folderDialog.SelectedPath;
            }
        }
        //Method setting new folder values to Settings instance and saving them to XML 
        public void SaveSettings()
        {
            if(VideosFolder != "")
            {
                Settings.Instance.VideosFolder = this.VideosFolder;
            }

            if (ImagesFolder != "")
            {
                Settings.Instance.ImagesFolder = this.ImagesFolder;
            }
            if (OverlaysFolder != "")
            {
                Settings.Instance.OverlayFolder = this.OverlaysFolder;
            }
            Actions.Instance.ShortBefore = this.ShortBefore;
            Actions.Instance.ShortAfter = this.ShortAfter;

            Actions.Instance.MediumBefore = this.MediumBefore;
            Actions.Instance.MediumAfter = this.MediumAfter;

            Actions.Instance.LongBefore = this.LongBefore;
            Actions.Instance.LongAfter = this.LongAfter;
            
            Settings.Instance.SaveSettings();
            Actions.Instance.SaveActionsSettings();
            this.view.Close();
        }
    }
}
