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

namespace APKRepackageSDKTool
{
    /// <summary>
    /// ChannelEditorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChannelEditorWindow : Window
    {
        public ChannelEditorWindow()
        {
            InitializeComponent();

            Grid_root.DataContext = EditorData.CurrentChannel;

            PasswordBox_keyStore.Password = EditorData.CurrentChannel.KeyStorePassWord;
            PasswordBox_alias.Password = EditorData.CurrentChannel.KeyStoreAliasPassWord;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "KeyStore Files (*.keystore)|*.keystore"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                this.TextBox_keyStore.Text = openFileDialog.FileName;
                EditorData.CurrentChannel.keyStorePath = openFileDialog.FileName;
            }
        }

        private void Button_Click_SelectIcon(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "KeyStore Files (*.png)|*.png"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                this.TextBox_Icon.Text = openFileDialog.FileName;
                Image_icon.Source = new BitmapImage(new Uri("pack://" + openFileDialog.FileName));
                EditorData.CurrentChannel.AppIcon = openFileDialog.FileName;
            }
        }

        private void Button_Click_SelectBanner(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "KeyStore Files (*.png)|*.png"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                this.TextBox_Banner.Text = openFileDialog.FileName;
                Image_Banner.Source = new BitmapImage(new Uri("pack://" + openFileDialog.FileName));
                EditorData.CurrentChannel.AppBanner = openFileDialog.FileName;
            }
        }

        private void Button_ClickSave(object sender, RoutedEventArgs e)
        {
            EditorData.CurrentChannel.KeyStorePassWord = PasswordBox_keyStore.Password;
            EditorData.CurrentChannel.KeyStoreAliasPassWord = PasswordBox_alias.Password;

            EditorData.CurrentGameChannelList = EditorData.CurrentGameChannelList;
            MessageBox.Show("保存成功");
        }
    }
}