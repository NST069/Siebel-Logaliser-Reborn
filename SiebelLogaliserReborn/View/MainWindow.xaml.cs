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

namespace SiebelLogaliserReborn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Model.SQLLine sqlLine = (sender as Button).DataContext as Model.SQLLine;
            View.QueryPopup q = new View.QueryPopup(sqlLine.SQL+"\n=====\nBinds:\n"+sqlLine.BindVar);/////
            q.Show();
        }
    }
}
