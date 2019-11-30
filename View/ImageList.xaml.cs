using MFormat.Model;
using MFormat.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MFormat.View
{
    /// <summary>
    /// Interaction logic for MediaList.xaml
    /// </summary>
    public partial class ImageList : Page
    {
        public ImageList(BroadcastManager videoPlayer)
        {
            InitializeComponent();

            ((ImageListViewModel)this.DataContext).broadcastPlayer = videoPlayer;
           
            ((ImageListViewModel)this.DataContext).view = this;

        }
       

        private void mediaList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Border border = (Border)VisualTreeHelper.GetChild(mediaList, 0);
            ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);

            if (e.Delta < 0)
            {
                scrollViewer.LineRight();
                scrollViewer.LineRight();
            }
            if (e.Delta > 0)
            {
                scrollViewer.LineLeft();
                scrollViewer.LineLeft();
            }
        }
    }
}
