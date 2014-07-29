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
using FormModeller;

namespace FormDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FormModel form;
        FormToolBox toolbox;
        public MainWindow()
        {
            InitializeComponent();
            toolbox = new FormToolBox();
            form = new FormModel();
            toolbox.Show();
        }



        private void getValuesFromFormToolBox()
        { 
        
        
        }
    }
}
