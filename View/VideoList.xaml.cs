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
    /// Interakční logika pro VideoList.xaml
    /// </summary>
    public partial class VideoList : Page
    {
        public VideoList(BroadcastManager videoPlayer)
        {
            InitializeComponent();

            ((VideoListViewModel)this.DataContext).broadcastPlayer = videoPlayer;

            ((VideoListViewModel)this.DataContext).view = this;
        }
    }
   
}
