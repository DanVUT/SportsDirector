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
using Xceed.Wpf.Toolkit;

namespace MFormat.View
{
    /// <summary>
    /// Interaction logic for HiglightsList.xaml
    /// </summary>
    public partial class HiglightsList : Page
    {
        public HiglightsList(BroadcastManager videoPlayer)
        {
            InitializeComponent();
            (this.DataContext as HighlightsListViewModel).view = this;
            (this.DataContext as HighlightsListViewModel).broadcastPlayer = videoPlayer;
        }

        private void highlightsList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Border border = (Border)VisualTreeHelper.GetChild(highlightsList, 0);
            ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);

            if(e.Delta < 0)
            {
                scrollViewer.LineRight();
                scrollViewer.LineRight();
            }
            if(e.Delta > 0)
            {
                scrollViewer.LineLeft();
                scrollViewer.LineLeft();
            }
        }

        private void UpDownPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            IntegerUpDown upDown = sender as IntegerUpDown;

            if(e.Delta > 0)
            {
                upDown.Value++;
            }
            if(e.Delta < 0)
            {
                if(upDown.Value - 1 > upDown.Minimum)
                {
                    upDown.Value--;
                }
            }
        }
    }
}
