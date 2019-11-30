using GalaSoft.MvvmLight.Command;
using MFormat.Model;
using MFormat.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MFormat.ViewModel
{
    public class ImageListViewModel
    {
        //Reference to view object
        public ImageList view;
        //Class representing item on MediaList
        public class ImageListItem
        {
            private int time;
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

            public int PlaybackLength {

                get { return this.time; }

                set { this.time = value; }
            }


        }


        //Video player instance
        public BroadcastManager broadcastPlayer;

        //Current media folder path
        string directoryPath = Settings.Instance.VideosFolder;
        //Command reacting for list view selection
        public RelayCommand<ImageListItem> BroadcastImageCommand { get; set; }
        //Command reacting for ViewLoaded event
        public ObservableCollection<ImageListItem> ImagesList { get; set; } = new ObservableCollection<ImageListItem>();

        //Constructor connecting commands and subscribing to change media folder event
        public ImageListViewModel()
        {
            BroadcastImageCommand = new RelayCommand<ImageListItem>((item) => BroadcastImage(item));
            Settings.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "ImagesFolder")
                {
                    LoadImagesFromFolder();
                }
            };
            LoadImagesFromFolder();
        }
        //Initialisation of media listbox from mediafolder
        private void LoadImagesFromFolder()
        {
            Task thread = new Task(() => {
                this.directoryPath = Settings.Instance.ImagesFolder;
                Application.Current.Dispatcher.Invoke(() => this.ImagesList.Clear());
                if (this.directoryPath != null)
                {
                    var extentions = ".jpg,.jpeg,.gif,.png,.bmp,.jpe,.jpeg";
                    
                    var files = Directory.GetFiles(this.directoryPath).Where(s => extentions.Contains(Path.GetExtension(s).ToLower()));

                    foreach (string file in files)
                    {
                        BitmapImage thumbnail = new BitmapImage(new Uri(file));
                        thumbnail.Freeze();
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ImageListItem item = new ImageListItem();
                            item.ImageData = thumbnail;
                            item.Name = file.Substring(file.LastIndexOf('\\') + 1);
                            this.ImagesList.Add(item);
                        });
                    }
                }
            });
            thread.Start();
        }


        //Changes played media in videoplayer
        public void BroadcastImage(ImageListItem item)
        {
          
            this.broadcastPlayer.ChangeMedia(this.directoryPath + @"\" +(item.Name),true);
            this.broadcastPlayer.StartMediaSourcePlayback(item.PlaybackLength);
            this.view.mediaList.UnselectAll();
            
        }
    }


}
