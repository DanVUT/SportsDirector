using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MFormat.Model;
using MFormat.View;
using MFORMATSLib;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MFormat.ViewModel
{
    //Class representing the DataContext for MainView. This DataContext is used also by FullscreenPreview window
    public partial class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        //Video player intance
        private BroadcastManager broadcastPlayer;
        //Fullscreen video player
        private FullscreenPreview fullscreenPreview;
        //Reference on MainView (For UI references)
        public MainView view;
        //Media list view
        private ImageList imageList;

        private VideoList videoList;
        //Highlights list view
        private HiglightsList higlightsList;
        //D3DImage instance. It is used as image source to render video frames on WPF Image element
        private D3DImage broadcastPreviewSource;
        public D3DImage BroadcastPreviewSource { get { return broadcastPreviewSource; } set { this.broadcastPreviewSource = value; } }
        //D3DImage instance. It is used as image source to render video frames on WPF Image element
        private D3DImage livePreviewSource;
        public D3DImage LivePreviewSource { get { return this.livePreviewSource; } set { this.livePreviewSource = value; } }
        public bool ShowRecordingActive { get; set; } = false;
        public bool ShowStartRecording { get; set; } = true;
        public bool ShowStopRecording { get; set; } = false;
        public bool EnableActionButtons { get; set; } = false;
        //Time when recording started
        public DateTime RecordingStartTime { get; set; }


        //Constructor just initializes commands
        public MainViewModel()
        {
            InitializeCommands();
        }
        //Constructor after view is loaded
        private void InitializeViewModel()
        {
            if (!App.IsDesignMode)
            {
                //Creates new FullscreenPreview window
                this.fullscreenPreview = new FullscreenPreview() { DataContext = this };
                this.fullscreenPreview.Show();
                //Creates instance of video player
                this.broadcastPlayer = new BroadcastManager(out this.livePreviewSource, out this.broadcastPreviewSource, 1);
                //Subscribes to broadcastPlayer event when frame is loaded in PreviewSource
                this.broadcastPlayer.PropertyChanged += VideoPlayer_RenderNewFrame;
                
                //Prepares pages and sets videoList as default page
                this.imageList = new ImageList(this.broadcastPlayer);
                this.videoList = new VideoList(this.broadcastPlayer);
                this.higlightsList = new HiglightsList(this.broadcastPlayer);
                
                this.view.Navigation.Content = this.videoList;
            }
        }
        //This method is used as event handler for closing main window. It destroys videoplayer and closes fullscreen preview
        public void CloseWindow()
        {
            this.broadcastPlayer.Destroy();
            fullscreenPreview.Close();
        }


        public event PropertyChangedEventHandler PropertyChanged; // default implementation of INotifyPropertyChanged interface

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        //Reaction to event when VideoPlayer renders new frame
        private void VideoPlayer_RenderNewFrame(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BroadcastPreviewSource")
            {
                OnPropertyChanged("BroadcastPreviewSource");
            }
            if (e.PropertyName == "LivePreviewSource")
            {
                OnPropertyChanged("LivePreviewSource");
            }
        }


    }
}