﻿using System;
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


namespace WpfApplication1.UI
{
    /// <summary>
    /// Interaction logic for ChatWin.xaml
    /// </summary>
    public partial class ChatWin : Window
    {

        public int userId;

        private Dictionary<string, int> tabs =
        new Dictionary<string, int>();

        private BLL.BLL BL = new BLL.BLL();

        public ChatWin()
        {
            InitializeComponent();
            AddNewTab("General",false);
            ChatTabCtrl.SelectedIndex = 0;
        }

        private void AddNewTab(string name, bool IsPersonalChat)
        {
            
            TabItem tb = new TabItem();
            StackPanel head = new StackPanel();
            head.Orientation = Orientation.Horizontal;
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
                tabs.Add(name, 0);
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
                tabs.Add(lb.Content.ToString(), 0);

            }
            
            content.Children.Add(lb);

            TextBox chatInput = new TextBox();
            chatInput.TextWrapping = TextWrapping.Wrap;
            chatInput.Height = 23;
            chatInput.KeyUp += inputChatTB_KeyUp;
            

            content.Children.Add(chatInput);

            Button btnRefreh = new Button();
            btnRefreh.Width = 50;
            btnRefreh.Height = 30;
            btnRefreh.Content = "Refresh";
            btnRefreh.Click += btnRefresh_Click;

            content.Children.Add(btnRefreh);

            tb.Content = content;
            tb.Header = head;
            ChatTabCtrl.Items.Add(tb);

            
        }  



        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            
            TabItem ti = (TabItem)((StackPanel)((Button)sender).Parent).Parent;
            int cntr = 0;
            for (int i = 0; i < ((TabControl)(ti.Parent)).Items.Count; i++)
            {
                if (((TabControl)(ti.Parent)).Items[i] == ti)
                {
                    StackPanel sp = (StackPanel)ti.Header;
                    TextBlock tb = (TextBlock)sp.Children[0];
                    tabs.Remove(tb.Text);
                    cntr = i;
                }

            }
            TextBlock name = (TextBlock)((StackPanel)(ti.Header)).Children[0];
            tabs.Remove(name.Text);
            ((TabControl)(ti.Parent)).Items.RemoveAt(cntr);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            string name = ((Label)((StackPanel)((Button)sender).Parent).Children[1]).Content.ToString();

            //Don't touch - Magic!!!
            if (tabs[name] != BL.GetTalkMsgCountByName(name))
            {
                ((TextBlock)((ScrollViewer)((StackPanel)((Button)sender).Parent).Children[0]).Content).Text = "";
                foreach (string message in BL.GetAllTalkMsgsByName(name))
                {
                    
                    ((TextBlock)((ScrollViewer)((StackPanel)((Button)sender).Parent).Children[0]).Content).Text += string.Format("\n{0}", message);                    
                }
            }
            tabs[name] = BL.GetTalkMsgCountByName(name);            
        }

        private void inputChatTB_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string talkName = ((Label)((StackPanel)((TextBox)sender).Parent).Children[1]).Content.ToString();
                BL.SendMessage(talkName,userId,((TextBox)sender).Text);
                ((TextBox)sender).Text = "";
            }
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Expander_Expanded_1(object sender, RoutedEventArgs e)
        {
            StackPanel content = new StackPanel();
            List<string> openedTabs = new List<string>(tabs.Keys);
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




       
    }
}
