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
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.IO;



namespace WpfApplication1.UI
{
    /// <summary>
    /// Interaction logic for ChatWin.xaml
    /// </summary>
    public partial class ChatWin : Window
    {

        public int userId;

       
        private List<TabItem> tabList = new List<TabItem>();

        private List<int> tabMsgCount = new List<int>();

        private System.Windows.Threading.DispatcherTimer timer;

        public Brush foreground;
        public Brush background;

        bool flag = true;

        private BLL.BLL BL = new BLL.BLL();

        public ChatWin()
        {
            InitializeComponent();

            List<string> style = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader("StyleConfig.txt"))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                        style.Add(line);
                }
            }
            catch (Exception e)
            {
                return;
            };
            if (style.Count == 2)
            {
                background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(style[0]));
                foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(style[1]));
                SetStyles();
            }
            

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += dispatcherTimer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            AddNewTab("General",false);
            BL.GrantAccessToTalk(userId, BL.GetTalkIdByName("General"));
            ChatTabCtrl.SelectedIndex = 0;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Refresh();
        }

        private void AddNewTab(string name, bool IsPersonalChat)
        {
            
            TabItem tb = new TabItem();
            DockPanel head = new DockPanel();
            head.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch; 
            head.VerticalAlignment = System.Windows.VerticalAlignment.Stretch; 
            head.Children.Add(new TextBlock( new Run(name)));

            Button btnClose = new Button();
            btnClose.Content = new TextBlock(new Bold(new Run("X")));
            btnClose.Click += btnClose_Click;
            btnClose.Margin = new Thickness(10,0,0,0);
            head.Children.Add(btnClose);

            StackPanel content = new StackPanel();

            ScrollViewer sw = new ScrollViewer();
            sw.Height = 206;
            

            TextBlock chatOutput = new TextBlock();
            chatOutput.TextWrapping = TextWrapping.Wrap;
            chatOutput.Text = "";
            
            sw.Content = chatOutput;

            content.Children.Add(sw);

            Label lb = new Label();
            lb.Visibility = Visibility.Hidden;
            if (!IsPersonalChat)
            {
                lb.Content = name;                
            }
            else
            {
                int res = BL.CheckIfPersonalChatExists(name, BL.GetUserNickById(userId));
                if (res == 0)
                {
                    BL.CreatePersonalChat(name, BL.GetUserNickById(userId));
                    lb.Content = string.Format("pm_{0}:{1}", name, BL.GetUserNickById(userId));
                }
                else
                {
                    lb.Content = BL.GetTalkNameById(res);
                }
                

            }
            
            content.Children.Add(lb);

            TextBox chatInput = new TextBox();
            chatInput.TextWrapping = TextWrapping.Wrap;
            chatInput.Height = 23;
            chatInput.KeyUp += inputChatTB_KeyUp;
            

            content.Children.Add(chatInput);
            
            tb.Content = content;
            tb.Header = head;

            tabMsgCount.Add(0);
            tabList.Add(tb); 

            ChatTabCtrl.Items.Add(tb);
            
        }  



        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            int idx = ChatTabCtrl.SelectedIndex;
            ChatTabCtrl.Items.RemoveAt(idx);
            tabList.RemoveAt(idx);
            tabMsgCount.RemoveAt(idx);
        }

        private void Refresh()
        {
            if (ChatTabCtrl.Items.Count == 0 || ChatTabCtrl.SelectedItem == null)
            {
                return;
            }
            
            int idx = ChatTabCtrl.SelectedIndex;
            TabItem ti = tabList[idx];
            string talkName = ((Label)(((StackPanel)ti.Content).Children[1])).Content.ToString();
            if (idx == ChatTabCtrl.Items.Count - 1)
            {
                flag = true;
            }
            if (BL.CheckTalkForNewMsgs(talkName) || flag)
            {
                flag = false;
                if (tabMsgCount[idx] != BL.GetTalkMsgCountByName(talkName))
                {                
                    ((TextBlock)((ScrollViewer)((StackPanel)(ti.Content)).Children[0]).Content).Text = "";
                    foreach (string message in BL.GetAllTalkMsgsByName(talkName))
                    {

                        ((TextBlock)((ScrollViewer)((StackPanel)(ti.Content)).Children[0]).Content).Text += string.Format("\n{0}", message);                    
                    }
                    ((ScrollViewer)((StackPanel)(ti.Content)).Children[0]).ScrollToBottom();
                }
                tabMsgCount[idx] = BL.GetTalkMsgCountByName(talkName);
                BL.ResetNewMsgsStatus(talkName);
            }
            if (BL.CheckTalkForNewJoins(talkName))
            {
                List<string> requests = BL.GetInvitationRequests(talkName);
                if (requests.Count == 0)
                {
                    BL.ResetNewInvitesStatus(talkName);
                }
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                foreach(string nick in requests)
                {
                    if (nick != BL.GetUserNickById(userId))
                    {
                        Button btn = new Button();
                        btn.Content = nick;
                        btn.Click += btnInvite_Click;
                        btn.Width = 80;
                        sp.Children.Add(btn);
                    }
                }

                if (((StackPanel)(ti.Content)).Children.Count == 4)
                {
                    ((StackPanel)(ti.Content)).Children.RemoveAt(3);
                    ((StackPanel)(ti.Content)).Children.Add(sp);
                }
                else
                {
                    ((StackPanel)(ti.Content)).Children.Add(sp);
                }
            }
        }

        private void btnInvite_Click(object sender, RoutedEventArgs e)
        {
            int idx = ChatTabCtrl.SelectedIndex;
            TabItem ti = tabList[idx];
            string talkName = ((Label)(((StackPanel)ti.Content).Children[1])).Content.ToString();
            BL.Invite(((Button)sender).Content.ToString(), talkName);
            StackPanel sp = (StackPanel)(((Button)sender).Parent);
            int count = 0;
            for (int i = 0; i < sp.Children.Count; i++)
            {
                if (sp.Children[i] == (Button)sender)
                {
                    count = i;
                }
            }
            sp.Children.RemoveAt(count);
        }

        private void inputChatTB_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                int idx = ChatTabCtrl.SelectedIndex;
                string talkName = ((Label)(((StackPanel)tabList[idx].Content).Children[1])).Content.ToString();
                BL.SendMessage(talkName,userId,((TextBox)sender).Text);
                ((TextBox)sender).Text = "";
            }
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BL.LogOut(userId);
            LoginWin lw = new LoginWin();
            lw.Show();                       
        }

        private void Expander_Expanded_1(object sender, RoutedEventArgs e)
        {
            StackPanel content = new StackPanel();

            List<string> openedTabs = new List<string>();
            foreach(TabItem ti in tabList)
            {
                openedTabs.Add(((Label)(((StackPanel)ti.Content).Children[1])).Content.ToString());
            }

            foreach (string name in BL.GetUsersOnlineExceptCurrent(userId))
            {
                if (!( openedTabs.Contains(string.Format("pm_{0}:{1}", BL.GetUserNickById(userId), name))  ||
                     openedTabs.Contains(string.Format("pm_{1}:{0}", BL.GetUserNickById(userId), name))))
                {
                    Button btn = new Button();
                    btn.Content = name;
                    btn.Click += btnOpen_Click;
                    content.Children.Add(btn);
                }
            }
            ((Expander)sender).Content = content;
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            AddNewTab(((Button)sender).Content.ToString(),true);
            StackPanel sp = (StackPanel)(((Button)sender).Parent);
            int count = 0;
            for (int i = 0; i < sp.Children.Count; i++)
            {
                if (sp.Children[i] == (Button)sender)
                {
                    count = i;
                }
            }
            sp.Children.RemoveAt(count);
        }

        private void Expander_Expanded_2(object sender, RoutedEventArgs e)
        {
            StackPanel content = new StackPanel();
            List<string> openedTabs = new List<string>();
            foreach (TabItem ti in tabList)
            {
                openedTabs.Add(((Label)(((StackPanel)ti.Content).Children[1])).Content.ToString());
            }
            foreach (string name in BL.GetAllGroupTalksName())
            {
                if (!(openedTabs.Contains(name)) && BL.CheckAccessToTalk(userId, name))
                {
                    Button btn = new Button();
                    btn.Content = name;
                    btn.Click += btnOpenGroup_Click;
                    content.Children.Add(btn);
                }
            }
            StackPanel spNewRoom = new StackPanel();
            spNewRoom.Orientation = Orientation.Horizontal;

            TextBox newRoomTb = new TextBox();
            newRoomTb.ToolTip = "Name of room to create/join";
            newRoomTb.Width = 70;

            Button btnCreate = new Button();
            btnCreate.Content = "Ok";
            btnCreate.Width = 30;
            btnCreate.Click += btnNewGroup_Click;

            spNewRoom.Children.Add(newRoomTb);
            spNewRoom.Children.Add(btnCreate);

            content.Children.Add(spNewRoom);

            ((Expander)sender).Content = content;
        }

        private void btnOpenGroup_Click(object sender, RoutedEventArgs e)
        {
            AddNewTab(((Button)sender).Content.ToString(), false);
            StackPanel sp = (StackPanel)(((Button)sender).Parent);
            int count = 0;
            for (int i = 0; i < sp.Children.Count; i++)
            {
                if (sp.Children[i] == (Button)sender)
                {
                    count = i;
                }
            }
            sp.Children.RemoveAt(count);
        }

        private void btnNewGroup_Click(object sender, RoutedEventArgs e)
        {
            StackPanel sp = (StackPanel)(((Button)sender).Parent);
            string name = ((TextBox)(sp.Children[0])).Text;
            if(BL.CheckIfGroupChatExists(name) == 0)
            {
                BL.CreateGroupChat(name,userId);
                AddNewTab(name, false);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show(string.Format("Do you want to send invitation reques to join {0} room?",name), "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    BL.CreateConTalk(userId,BL.GetTalkIdByName(name));
                    MessageBox.Show(string.Format("When you will be allowed to join {0} room, you will see it in your room list!", name), "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }           
                        
        }


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SettingsWin sw = new SettingsWin();
            sw.Show();
            sw.parent = this;                       
        }

        public void SetStyles()
        {
            if (background == null || foreground == null)
            {
                return;
            }
            Resources.Remove(typeof(Button));
            Resources.Remove(typeof(Expander));
            Resources.Remove(typeof(TabControl));
            Resources.Remove(typeof(StackPanel));
            Resources.Remove(typeof(Grid));
            Resources.Remove(typeof(DockPanel));
            
            

            Style style = new Style
            {
                TargetType = typeof(Button)
            };
            style.Setters.Add(new Setter(Button.BackgroundProperty, background));
            style.Setters.Add(new Setter(Button.ForegroundProperty, foreground));
            Resources.Add(typeof(Button), style);
            style = new Style
            {
                TargetType = typeof(Expander)
            };
            style.Setters.Add(new Setter(Expander.BackgroundProperty, background));
            style.Setters.Add(new Setter(Expander.ForegroundProperty, foreground));
            Resources.Add(typeof(Expander), style);
            style = new Style
            {
                TargetType = typeof(TabControl)
            };
            style.Setters.Add(new Setter(TabControl.BackgroundProperty, background));
            style.Setters.Add(new Setter(TabControl.ForegroundProperty, foreground));
            Resources.Add(typeof(TabControl), style);
            style = new Style
            {
                TargetType = typeof(StackPanel)
            };
            style.Setters.Add(new Setter(StackPanel.BackgroundProperty, background));
            Resources.Add(typeof(StackPanel), style);
            style = new Style
            {
                TargetType = typeof(Grid)
            };
            style.Setters.Add(new Setter(Grid.BackgroundProperty, background));
            Resources.Add(typeof(Grid), style);
            style = new Style
            {
                TargetType = typeof(DockPanel)
            };
            style.Setters.Add(new Setter(DockPanel.BackgroundProperty, background));
            Resources.Add(typeof(DockPanel), style); 
            
            

            using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(@"StyleConfig.txt"))
            {
                SolidColorBrush S = background as SolidColorBrush;
                file.WriteLine(S.Color.ToString());
                S = foreground as SolidColorBrush;
                file.WriteLine(S.Color.ToString());
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {           
            this.Close(); 
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format("This project was created by Oleksandr Sinkevych\nAll questions, suggestions and remarks you can send to kramer.istep@gmail.com\nAll right reserved(c)2015")
                                        , "Easy Local Chat", MessageBoxButton.OK, MessageBoxImage.Information);
        }

       
    }
}
