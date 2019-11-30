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
using System.Windows.Shapes;

namespace MFormat.View
{
    /// <summary>
    /// Interaction logic for RecordingSettingsDialog.xaml
    /// </summary>
    public partial class RecordingSettingsDialog : Window
    {
        public RecordingSettingsDialog(BroadcastManager broadcastPlayer)
        {
            InitializeComponent();
            (this.DataContext as RecordingSettingsDialogViewModel).view = this;
            (this.DataContext as RecordingSettingsDialogViewModel).broadcastPlayer = broadcastPlayer;
        }
    }
}
