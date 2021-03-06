﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace APKRepackageSDKTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        const int c_totalStep = 6;
        int currentTotalStep = 6;

        RepackageManager repackageManager = new RepackageManager();
        bool isBuilding = false;

        string apkPath;
        string keyStorePath;
        string exportPath;

        List<ChannelInfo> currentGameChannelList = new List<ChannelInfo>();

        public List<ChannelInfo> CurrentGameChannelList
        {
            get => currentGameChannelList;
            set
            {
                currentGameChannelList = value;
                ListBox_channel.ItemsSource = currentGameChannelList;
             }
        }

        public MainWindow()
        {
            InitializeComponent();

            apkPath = RecordManager.GetRecord(EditorData.c_ConfigRecord, "apkPath", "null");
            keyStorePath = RecordManager.GetRecord(EditorData.c_ConfigRecord, "keyStorePath", "null");
            exportPath = RecordManager.GetRecord(EditorData.c_ConfigRecord, "exportPath", "null");
            
            UpdateContent();

            //展示到游戏选择界面上
            ComboBox_gameList.ItemsSource = EditorData.GameList;
            ComboBox_gameList.SelectedIndex = RecordManager.GetRecord(EditorData.c_ConfigRecord, "index", -1);

            //再显示该游戏的所有渠道
            UpdateChannel();
        }

        #region 点击事件

        private void Button_ClickSelectAPK(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("点击了click事件");

            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "APK Files (*.apk)|*.apk"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                this.Text_APKPath.Text = openFileDialog.FileName;
                apkPath = openFileDialog.FileName;
                RecordManager.SaveRecord(EditorData.c_ConfigRecord, "apkPath", apkPath);
            }
        }

        private void Button_ClickSelectExportPath(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog m_Dialog = new FolderBrowserDialog();
            DialogResult result = m_Dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string m_Dir = m_Dialog.SelectedPath.Trim();
            this.Text_APKExportPath.Text = m_Dir;
            exportPath = m_Dir;
            RecordManager.SaveRecord(EditorData.c_ConfigRecord, "exportPath", exportPath);
        }

        private void Button_ClickSelectSDKLibPath(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog m_Dialog = new FolderBrowserDialog();
            DialogResult result = m_Dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string m_Dir = m_Dialog.SelectedPath.Trim();
            this.Text_SDKLibPath.Text = m_Dir;
            EditorData.SdkLibPath = m_Dir;
        }

        /// <summary>
        /// 点击重打包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ClicRepackageAPK(object sender, RoutedEventArgs e)
        {
            if(!isBuilding)
            {
                //获取ChannelInfo
                List<ChannelInfo> list = new List<ChannelInfo>();
                for (int i = 0; i < EditorData.CurrentGameChannelList.Count; i++)
                {
                    if (EditorData.CurrentGameChannelList[i].isChecked)
                    {
                        list.Add( EditorData.CurrentGameChannelList[i]);
                    }
                }

                if (list.Count == 0)
                {
                    System.Windows.MessageBox.Show("没有渠道被选中");
                    return;
                }

                RepackageInfo ri = new RepackageInfo();
                ri.apkPath = apkPath;
                ri.exportPath = exportPath;

                repackageManager.Repackage(ri, list, RepackCallBack, RepackCallBackError);
                isBuilding = true;
                UpdateRepackButton();

                currentTotalStep = list.Count * c_totalStep;
            }
            else
            {
                repackageManager.CancelRepack();
                isBuilding = false;
                UpdateRepackButton();

                progress = 0;
                Text_progressName.Content = "已取消";
            }
        }

        private void Button_ClickClean(object sender, RoutedEventArgs e)
        {
            line = 0;
            output = "";
            Text_output.Text = "";
        }

        private void Button_ClickEditorGame(object sender, RoutedEventArgs e)
        {
            GameSetting gs = new GameSetting();
            gs.ShowDialog();
        }

        private void Button_ClickDeleteChannel(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("确定要移除这个渠道吗？\n所有填写的配置都会丢失", "警告", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;

                for (int i = 0; i < EditorData.CurrentGameChannelList.Count; i++)
                {
                    ChannelInfo info = EditorData.CurrentGameChannelList[i];

                    if (info.Name == (string)btn.Tag)
                    {
                        EditorData.CurrentGameChannelList.RemoveAt(i);
                        EditorData.CurrentGameChannelList = EditorData.CurrentGameChannelList;
                        return;
                    }
                }
            }

        }

        private void Button_ClickAddChannel(object sender, RoutedEventArgs e)
        {
            if (Text_AddChannel.Text != "")
            {
                ChannelInfo ci3 = new ChannelInfo();
                ci3.Name = Text_AddChannel.Text;

                EditorData.CurrentGameChannelList.Add(ci3);
                EditorData.CurrentGameChannelList = EditorData.CurrentGameChannelList;

                Text_AddChannel.Text = "";
            }
        }

        private void CheckBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.CheckBox cb = sender as System.Windows.Controls.CheckBox;

            EditorData.SetCurrentChannel((string)cb.Tag);

            ChannelEditorWindow cew = new ChannelEditorWindow();
            cew.ShowDialog();
        }

        private void Button_ClickEditor(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button cb = sender as System.Windows.Controls.Button;

            EditorData.SetCurrentChannel((string)cb.Tag);

            ChannelEditorWindow cew = new ChannelEditorWindow();
            cew.ShowDialog();
        }

        #endregion

        #region 界面改变事件

        private void ComboBox_gameList_Selected(object sender, RoutedEventArgs e)
        {
            RecordManager.SaveRecord(EditorData.c_ConfigRecord, "index", ComboBox_gameList.SelectedIndex);
            UpdateChannel();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EditorData.CurrentGameChannelList = EditorData.CurrentGameChannelList;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            EditorData.CurrentGameChannelList = EditorData.CurrentGameChannelList;
        }

        void UpdateRepackButton()
        {
            if(isBuilding)
            {
                Button_Repack.Content = "取消";
            }
            else
            {
                Button_Repack.Content = "重打包";
            }
        }

        #endregion

        #region 事件接收

        int line = 0;
        string output = "";

        float progress = 0;
        string content;

        void RepackCallBack(float progress, string content, string output)
        {
            this.progress = progress;
            this.content = content;

            line++;
            this.output += "["+line+"]"+ output+"\n";

            Action ac = new Action(UpdateContent);
            Dispatcher.BeginInvoke(ac);
        }

        void RepackCallBackError(float progress, string content, string output)
        {
            this.progress = progress;
            this.content = "打包异常";

            line++;
            this.output += "<Error>[" + line + "]" + output + "\n";
            isBuilding = false;
            progress = 0;

            Action ac = new Action(UpdateContent);
            Dispatcher.BeginInvoke(ac);

            repackageManager.CancelRepack();
        }

        #endregion

        #region Update

        void UpdateContent()
        {
            Progress_repackage.Maximum = currentTotalStep;
            Progress_repackage.Value = progress;

            Text_output.Text = output;
            Text_output.ScrollToEnd();

            Text_progressName.Content = content;

            Text_APKExportPath.Text = exportPath;
            Text_APKPath.Text = apkPath;
            Text_SDKLibPath.Text = EditorData.SdkLibPath;

            if(progress == currentTotalStep)
            {
                isBuilding = false;
            }

            UpdateRepackButton();
        }

        void UpdateChannel()
        {
            if(ComboBox_gameList.SelectedIndex != -1
                && ComboBox_gameList.SelectedIndex < EditorData.GameList.Count
                )
            {
                EditorData.SetChannelList(EditorData.GameList[ComboBox_gameList.SelectedIndex].GameName);
                ListBox_channel.ItemsSource = EditorData.CurrentGameChannelList;
            }
            else
            {
                ListBox_channel.ItemsSource = null;
            }
        }



        #endregion


    }
}
