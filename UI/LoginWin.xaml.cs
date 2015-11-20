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

using System.IO;

namespace WpfApplication1.UI
{
    /// <summary>
    /// Interaction logic for LoginWin.xaml
    /// </summary>
    public partial class LoginWin : Window
    {
        private BLL.BLL BL = new BLL.BLL();

        private List<string> loginData = new List<string>();

        public LoginWin()
        {
            BL.CheckDBTables();
            InitializeComponent();
            LoadData();
            if (loginData.Count() != 0)
            {
                SignNickTB.Text = loginData[0];
                SignPasswordTB.Password = loginData[1];

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (BL.CreateUser(RegNickTB.Text, RegPasswordTB.Password))
            {
                if (RegRemCkB.IsChecked.Value)
                {
                    SaveData(RegNickTB.Text, RegPasswordTB.Password);
                }
                ChatWin chWin = new ChatWin();
                chWin.Show();
                chWin.userId = BL.GetUserIdByNick(RegNickTB.Text);
                this.Close();
            }
            else
            {
                this.Title = "Nick name is occupied";
            }            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (!BL.CheckUserNick(SignNickTB.Text))
            {
                this.Title = "Wrong nick name";
                return;
            }
            if (BL.CheckUserPass(SignNickTB.Text, SignPasswordTB.Password))
            {
                this.Title = "Success";

                if (SignRemCkB.IsChecked.Value)
                {
                    SaveData(SignNickTB.Text, SignPasswordTB.Password);
                }
                ChatWin chWin = new ChatWin();
                chWin.Show();
                chWin.userId = BL.GetUserIdByNick(SignNickTB.Text);
                this.Close();
                
                return;
            }
            else
            {
                this.Title = "Wrong password";
                return;
            }

        }

        private void LoadData()
        {
            loginData = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader("LoginConfig.txt"))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                        loginData.Add(line);
                }
            }
            catch (Exception e)
            {
                return;
            };
            
        }
       
        private void SaveData(string nick, string password)
        {
            using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(@"LoginConfig.txt"))
            {
                file.WriteLine(nick);
                file.WriteLine(password); 
            }
        }

        
    }
}
