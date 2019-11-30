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
using Xceed.Wpf.Toolkit;

namespace MFormat.View
{
    /// <summary>
    /// Interaction logic for SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : Window
    {
        public SettingsDialog()
        {
            InitializeComponent();
            Closed += (s, e) => this.DataContext = null;
            ((SettingsDialogViewModel)this.DataContext).view = this;
        }

        private void IntegerUpDown_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            IntegerUpDown upDown = sender as IntegerUpDown;
            if(e.Delta > 0)
            {
                upDown.Value++;
            } else
            {
                upDown.Value--;
            }
        }

        private void IntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            IntegerUpDown upDown = sender as IntegerUpDown;

            if(upDown.Value < upDown.Minimum)
            {
                upDown.Value = upDown.Minimum;
            }
        }
    }
}
