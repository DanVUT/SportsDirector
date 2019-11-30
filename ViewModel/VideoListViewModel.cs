using GalaSoft.MvvmLight.Command;
using MFormat.Model;
using MFormat.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MFormat.ViewModel
{
    public class VideoListViewModel
    {

        public VideoList view;
        public class VideoListItem
        {
            private string name;
            public string Name
            {
                get { return this.name; }
                set { this.name = value; }
            }

            private BitmapSource imageData;
            public BitmapSource ImageData
            {
                get { return this.imageData; }
                set { this.imageData = value; }
            }

        }

        //Command reacting for list view selection
        public RelayCommand<VideoListItem> BroadcastVideoCommand { get; set; }

        //Video player instance
        public BroadcastManager broadcastPlayer;
        //ListView items 
        public ObservableCollection<VideoListItem> VideosList { get; set; } = new ObservableCollection<VideoListItem>();
        //Current media folder path
        string directoryPath = Settings.Instance.VideosFolder;
        

        //Constructor connecting commands and subscribing to change media folder event
        public VideoListViewModel()
        {
            BroadcastVideoCommand = new RelayCommand<VideoListItem>((item) => BroadcastVideo(item));
            Settings.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "VideosFolder")
                {
                    LoadVideosFromFolder();
                }
            };
            LoadVideosFromFolder();
        }

        public void LoadVideosFromFolder()
        {
            Task thread = new Task(() =>
            {
                this.directoryPath = Settings.Instance.VideosFolder;
                Application.Current.Dispatcher.Invoke(() => this.VideosList.Clear());
                if (this.directoryPath != null)
                {

                    var extentions = ".mov,.avi,.mp4,.ts";

                    var files = Directory.GetFiles(this.directoryPath).Where(s => extentions.Contains(Path.GetExtension(s).ToLower()));
                    foreach (string file in files)
                    {
                        BitmapSource thumbnail = Thumbnailer.Instance.GetThumbnail(file);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            VideoListItem item = new VideoListItem();

                            item.ImageData = thumbnail;
                            item.Name = file.Substring(file.LastIndexOf('\\') + 1);
                            this.VideosList.Add(item);
                        });
                    }
                }
            });
            thread.Start();
        }


        //Changes played media in videoplayer
        public void BroadcastVideo(VideoListItem item)
        {
            this.broadcastPlayer.ChangeMedia(this.directoryPath + @"\" + (item.Name));
            this.broadcastPlayer.StartMediaSourcePlayback();
            this.view.videoList.UnselectAll();
        }
    }


}
