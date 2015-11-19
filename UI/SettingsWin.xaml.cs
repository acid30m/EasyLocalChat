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

namespace WpfApplication1.UI
{
    /// <summary>
    /// Interaction logic for SettingsWin.xaml
    /// </summary>
    public partial class SettingsWin : Window
    {
        public ChatWin parent;

        bool flag = false;

        public SettingsWin()
        {
            InitializeComponent();
            flag = true;
        }

        private void Sliders_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!flag)
            {
                return;
            }
            Resources.Remove(typeof(Label));
            Style style = new Style
            {
                TargetType = typeof(Label)
            };
            style.Setters.Add(new Setter(Label.BackgroundProperty, new SolidColorBrush(Color.FromArgb((byte)(backAChanel.Value), (byte)(backRChanel.Value), (byte)(backGChanel.Value), (byte)(backBChanel.Value)))));
            style.Setters.Add(new Setter(Label.ForegroundProperty, new SolidColorBrush(Color.FromArgb((byte)(forgAChanel.Value), (byte)(forgRChanel.Value), (byte)(forgGChanel.Value), (byte)(forgBChanel.Value)))));
            Resources.Add(typeof(Label), style);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            parent.background = new SolidColorBrush(Color.FromArgb((byte)(backAChanel.Value), (byte)(backRChanel.Value), (byte)(backGChanel.Value), (byte)(backBChanel.Value)));
            parent.foreground = new SolidColorBrush(Color.FromArgb((byte)(forgAChanel.Value), (byte)(forgRChanel.Value), (byte)(forgGChanel.Value), (byte)(forgBChanel.Value)));
            parent.SetStyles();
            this.Close();
        }

        
    }
}
