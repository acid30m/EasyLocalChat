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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        BLL.BLL BL = new BLL.BLL();

         List<string> connectionStr; //List for saving/loading connection settings

         public Window1()
        {
            // Try to load saved connecting data
            connectionStr = new List<string>();
            try
            {   
                using (StreamReader sr = new StreamReader("Connection.txt"))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    connectionStr.Add(line);
                }
            }
            catch (Exception e)
            {
                this.Title = "Connection";
            };
            
            InitializeComponent();

            if (connectionStr.Count() < 1) //If connection settings file does not exist
            {
                return;
            }
            else if (connectionStr[0] == "WinAuth")
            {
                SNameTB.Text = connectionStr[1];
                DBNameTB.Text = connectionStr[2];
                WinAuthCkB.IsChecked = true;
            }
            else
            {
                SNameTB.Text = connectionStr[0];
                DBNameTB.Text = connectionStr[1];
                UserNameTB.Text = connectionStr[2];
                PasswordTB.Password = connectionStr[3];
            }

        }

        //Cancel button is closing window
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //If all settings are filled, confirm button send connection data to login window's BLL 
        //and writes data to file if needed  
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string connection;
            connectionStr = new List<string>(); 
            if (SNameTB.Text == "")
            {
                this.Title = "Server name field is required!";
                return;
            }
            if (DBNameTB.Text == "")
            {
                this.Title = "DB name field is required!";
                return;
            }
            if (WinAuthCkB.IsChecked == false)
            {
                if (UserNameTB.Text == "")
                {
                    this.Title = "User name field is required!";
                    return;
                }
                if (PasswordTB.Password == "")
                {
                    this.Title = "Password field is required!";
                    return;
                }
                connectionStr.Add(SNameTB.Text);
                connectionStr.Add(DBNameTB.Text);
                connectionStr.Add(UserNameTB.Text);
                connectionStr.Add(PasswordTB.Password);
                connection = string.Format(@"Server={0}; Database={1}; User Id={2};Password={3};", 
                    SNameTB.Text, DBNameTB.Text, UserNameTB.Text, PasswordTB.Password);
            }
            else
            {
                connectionStr.Add(SNameTB.Text);
                connectionStr.Add(DBNameTB.Text);
                connection = string.Format(@"Data Source={0};Integrated Security=SSPI;Database={1};", SNameTB.Text, DBNameTB.Text);
            }


            if (RememberCkB.IsChecked.Value)
            {

                using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(@"Connection.txt"))
                {
                    if (WinAuthCkB.IsChecked.Value)
                    {
                        file.WriteLine("WinAuth");
                        foreach (string line in connectionStr)
                        {
                            file.WriteLine(line);
                        }
                    }
                    else
                    {                        
                        foreach (string line in connectionStr)
                        {
                            file.WriteLine(line);
                        }
                    }
                    
                }
            }

            if (BL.CheckConnection(connection))
            {
                LoginWin lgWin = new LoginWin();
                lgWin.Show();
               
                this.Close();
            }
            else
            {
                this.Title = "Failed to connect to server!";
            }
            
        }

        //Disabling or enabling user name and password fields when Windows Authentification is checked/uncheked
        private void WinAuthCkB_Checked(object sender, RoutedEventArgs e)
        {
            
            UserNameTB.IsEnabled = false;
            PasswordTB.IsEnabled = false;
                       
        }

        private void WinAuthCkB_Unchecked(object sender, RoutedEventArgs e)
        {
            UserNameTB.IsEnabled = true;
            PasswordTB.IsEnabled = true;
        }
    }
}
