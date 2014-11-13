using FormModeller;
using System.Windows;

namespace FormDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FormModel form;
        private FormToolBox toolbox;

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