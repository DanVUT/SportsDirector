using GalaSoft.MvvmLight.Command;
using MFormat.Model;
using MFormat.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MFormat.ViewModel
{
    public class MediaListViewModel
    {
        //Reference to view object
        public MediaList view;
        //Class representing item on MediaList
        public class MediaListItem
        {
            private string name;
            public string Name
            {
                get { return this.name; }
                set { this.name = value; }
            }

            private BitmapImage imageData;
            public BitmapImage ImageData
            {
                get { return this.imageData; }
                set { this.imageData = value; }
            }

        }

        //Video player instance
        public VideoPlayer videoPlayer;

       private string extention;
        public Boolean video;
        //Current media folder path
        string directoryPath = Settings.Instance.MediaFolder;
        //Command reacting for list view selection
        public RelayCommand<MediaListItem> BroadcastMediaCommand { get; set; }
        //Command reacting for ViewLoaded event
        public RelayCommand ViewLoadedCommand { get; set; }

        //Constructor connecting commands and subscribing to change media folder event
        public MediaListViewModel()
        {
            BroadcastMediaCommand = new RelayCommand<MediaListItem>((item) => BroadcastMedia(item));
            ViewLoadedCommand = new RelayCommand(() => InitializeListBox());
            Settings.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "MediaFolder")
                {
                    InitializeListBox();
                }
            };
        }
        //Initialisation of media listbox from mediafolder
        private void InitializeListBox()
        {
            
            Thread thread = new Thread(() => {
                this.directoryPath = Settings.Instance.MediaFolder;
                Application.Current.Dispatcher.Invoke(() => this.view.mediaList.Items.Clear());
                if (this.directoryPath != null)
                {
                  
                        extention = ".jpg,.jpeg,.gif,.png,.bmp,.jpe,.jpeg";
                    
                 //  extention = ".mov,.avi,.mp4,.ts";
                    
                    var files = Directory.GetFiles(this.directoryPath).Where(s => extention.Contains(Path.GetExtension(s)));
                    foreach (string file in files)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MediaListItem item = new MediaListItem();

                           
                                if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".gif") || file.EndsWith(".jpeg") || file.EndsWith(".bmp"))
                                {
                                    item.ImageData = new BitmapImage(new Uri(file));
                                }
                                else
                                {
                                    item.ImageData = new BitmapImage(new Uri(@"pack://application:,,,/Static/video.jpg"));
                                }
                                item.Name = file.Substring(file.LastIndexOf('\\') + 1);
                                view.mediaList.Items.Add(item);
                            
                        });
                    }
                }
            });

            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        //Changes played media in videoplayer
        public void BroadcastMedia(MediaListItem item)
        {
            this.videoPlayer.ChangeMedia(this.directoryPath + @"\" +(item.Name));
            this.videoPlayer.StartMediaSourcePlayback();
            this.view.mediaList.UnselectAll();
        }
    }


}
