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
    /// Interaction logic for LoadHighlightsDialog.xaml
    /// </summary>
    public partial class LoadHighlightsDialog : Window
    {
        public LoadHighlightsDialog()
        {
            InitializeComponent();
            (this.DataContext as LoadHighligtsDialogViewModel).view = this;
        }
    }
}
