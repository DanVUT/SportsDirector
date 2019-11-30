using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MFormat.Model;
using MFormat.View;
using MFORMATSLib;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Action = MFormat.Model.Action;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace MFormat.ViewModel
{
    //Partial class to MainViewModel containing Commands declarations and their implementations
    public partial class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public RelayCommand OpenStreamCommand { get; private set; }
        public RelayCommand<MouseButtonEventArgs> LeftClickButtonCommand { get; set; }
        public RelayCommand OpenOverlayCommand { get; set; }
        public RelayCommand InitializeViewModelCommand { get; set; }
        public RelayCommand OpenSettingsDialogCommand { get; set; }
        public RelayCommand OpenRecordingSettingsDialogCommand { get; set; }
        public RelayCommand StartRecordingCommand { get; set; }
        public RelayCommand StopRecordingCommand { get; set; }
        public RelayCommand ShortActionCommand { get; set; }
        public RelayCommand MediumActionCommand { get; set; }
        public RelayCommand LongActionCommand { get; set; }
        public RelayCommand OpenImagesCommand { get; set; }

        public RelayCommand OpenVideoCommand { get; set; }

        public RelayCommand OpenHighlightsCommand { get; set; }
        public RelayCommand SwitchToLiveCommand { get; set; }
        public RelayCommand SaveHighlightsCommand { get; set; }
        public RelayCommand LoadHighlightsCommand { get; set; }
        private void InitializeCommands()
        {
            OpenStreamCommand = new RelayCommand(() => GetStream());
            InitializeViewModelCommand = new RelayCommand(() => InitializeViewModel());
            OpenSettingsDialogCommand = new RelayCommand(() => OpenSettingsDialog());
            OpenRecordingSettingsDialogCommand = new RelayCommand(() => OpenRecordingSettingsDialog());
            StartRecordingCommand = new RelayCommand(() => StartRecording());
            StopRecordingCommand = new RelayCommand(() => StopRecording());
            ShortActionCommand = new RelayCommand(() => ShortAction());
            MediumActionCommand = new RelayCommand(() => MediumAction());
            LongActionCommand = new RelayCommand(() => LongAction());
            OpenImagesCommand = new RelayCommand(() => OpenImageView());

            OpenVideoCommand = new RelayCommand(() => OpenVideoView());

            OpenHighlightsCommand = new RelayCommand(() => OpenHighlightsView());
            SwitchToLiveCommand = new RelayCommand(() => this.broadcastPlayer.StopMediaSourcePlayback());
            SaveHighlightsCommand = new RelayCommand(() => SaveHighlights());
            LoadHighlightsCommand = new RelayCommand(() => new LoadHighlightsDialog().ShowDialog());
            //LeftClickButtonCommand = new RelayCommand<MouseButtonEventArgs>((e) => PlaceOverlay(e));
            //OpenOverlayCommand = new RelayCommand(() => LoadOverlay());
        }

        //public void LoadOverlay()
        //{
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        this.editedOverlay = new OverlayObject();
        //        this.editedOverlay.isText = false;
        //        this.editedOverlay.overlayFile = openFileDialog.FileName;
        //        this.newOverlay = true;
        //    }
        //}

        //public void PlaceOverlay(MouseButtonEventArgs e)
        //{
        //    if (this.newOverlay)
        //    {
        //        if (!this.editedOverlay.isText)
        //        {
        //            Image overlay = Image.FromFile(this.editedOverlay.overlayFile);
        //            this.editedOverlay.x = (e.GetPosition(relativeTo: view.preview).X * ((double)this.mediaWidth / this.view.preview.ActualWidth)) - overlay.Width / 2;
        //            this.editedOverlay.y = (e.GetPosition(relativeTo: view.preview).Y * ((double)this.mediaHeight / this.view.preview.ActualHeight)) - overlay.Height / 2;
        //        }
        //        else
        //        {
        //            this.editedOverlay.x = e.GetPosition(relativeTo: view.preview).X;
        //            this.editedOverlay.y = e.GetPosition(relativeTo: view.preview).Y;
        //        }
        //        this.overlaysList.Add(this.editedOverlay);
        //    }
        //}

        //Opens Dialog for getting Input Device/Stream URL and changes input device / stream url in videoplayer
        public void GetStream()
        {
            Thread thread = new Thread(() => {
                InputDeviceDialog deviceDialog = new InputDeviceDialog();

                if (deviceDialog.ShowDialog() == true)
                {
                    if (deviceDialog.getVideoDevice() != null)
                    {
                        this.broadcastPlayer.ChangeDevice(deviceDialog.getVideoDevice());
                        this.broadcastPlayer.StartLiveSourcePlayback();
                    }
                    else
                    {
                        this.broadcastPlayer.ChangeStream(deviceDialog.getStreamUrl());
                        this.broadcastPlayer.StartLiveSourcePlayback();
                    }
                }
            });

            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        //Opens settings dialog
        public void OpenSettingsDialog()
        {
            Thread thread = new Thread(() => {
                SettingsDialog settingsDialog = new SettingsDialog();
                settingsDialog.ShowDialog();
            });
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        //Opens recording settings dialog
        public void OpenRecordingSettingsDialog()
        {
            Thread thread = new Thread(() => { 
                RecordingSettingsDialog recordingDialog = new RecordingSettingsDialog(this.broadcastPlayer);
                recordingDialog.ShowDialog();
            });
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        //Start recording
        public void StartRecording()
        {
            if (!this.broadcastPlayer.IsStreaming)
            {
                MessageBox.Show("Open Stream/Video Device Input first");
                return;
            }
            Thread thread = new Thread(() => {
                if (this.broadcastPlayer.recorder == null)
                {
                    if (!(new RecordingSettingsDialog(this.broadcastPlayer).ShowDialog()) == true)
                    {
                        return;
                    }
                }
                this.broadcastPlayer.StartRecording();
                this.RecordingStartTime = DateTime.Now;
                this.ShowRecordingActive = true;
                this.ShowStopRecording = true;
                this.ShowStartRecording = false;
                this.EnableActionButtons = true;

                Application.Current.Dispatcher.Invoke(() => {
                    OnPropertyChanged("ShowRecordingActive");
                    OnPropertyChanged("ShowStartRecording");
                    OnPropertyChanged("ShowStopRecording");
                    OnPropertyChanged("EnableActionButtons");
                });
            });

            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
        //Stop recording
        public void StopRecording()
        {
            this.broadcastPlayer.StopRecording();
            this.ShowStartRecording = true;
            this.ShowStopRecording = false;
            this.ShowRecordingActive = false;
            this.EnableActionButtons = false;
            OnPropertyChanged("ShowRecordingActive");
            OnPropertyChanged("ShowStartRecording");
            OnPropertyChanged("ShowStopRecording");
            OnPropertyChanged("EnableActionButtons");
        }

        //Create ShortAction
        public void ShortAction()
        {
            Actions.Instance.AddShortAction(this.RecordingStartTime, this.broadcastPlayer.GetCurrentlyRecordedMedia());
        }
        //Create ShortMedium
        public void MediumAction()
        {
            Actions.Instance.AddMediumAction(this.RecordingStartTime, this.broadcastPlayer.GetCurrentlyRecordedMedia());
        }
        //Create LongAction
        public void LongAction()
        {
            Actions.Instance.AddLongAction(this.RecordingStartTime, this.broadcastPlayer.GetCurrentlyRecordedMedia());
        }
        //Opens media view
        public void OpenImageView()
        {
            this.view.Navigation.Content = this.imageList;
        }

        public void OpenVideoView()
        {
            this.view.Navigation.Content = this.videoList;
        }



        //Opens highlights view
        public void OpenHighlightsView()
        {
            this.view.Navigation.Content = this.higlightsList;
        }

        public void SaveHighlights()
        {
            Thread thread = new Thread(() => {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "XML-File | *.xml";

                if(dialog.ShowDialog() == DialogResult.OK) {
                    Actions.Instance.SaveHighlights(dialog.FileName);
                }
            });
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}
