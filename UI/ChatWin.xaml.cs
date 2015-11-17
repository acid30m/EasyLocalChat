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
    /// Interaction logic for ChatWin.xaml
    /// </summary>
    public partial class ChatWin : Window
    {

        public int userId;

        private BLL.BLL BL = new BLL.BLL();

        public ChatWin()
        {
            InitializeComponent();
        }
    }
}
