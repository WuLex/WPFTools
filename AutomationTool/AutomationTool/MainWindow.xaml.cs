using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using WindowsInput;
using MessageBox = System.Windows.Forms.MessageBox;

namespace AutomationTool
{
    public partial class MainWindow : System.Windows.Window
    {
      
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void OpenFileManager_Click(object sender, RoutedEventArgs e)
        {
            await OpenFileManagerAsync();
        }

        //private async void CopyAndPaste_Click(object sender, RoutedEventArgs e)
        //{
        //    await CopyAndPasteAsync();
        //}

        private async Task OpenFileManagerAsync()
        {
            string folderPath = FolderPathTextBox.Text;
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                MessageBox.Show("请选择文件夹路径。");
                return;
            }

            Process.Start("explorer.exe", folderPath);
            await Task.Delay(2000); // 等待文件资源管理器打开
        }



        private void CopyAndPasteFilesWithKeyboard()
        {
            string sourceFolderPath = FolderPathTextBox.Text;
            string targetFolderPath = TargetFolderPathTextBox.Text;
            string fileName = FileNameTextBox.Text;

            if (string.IsNullOrWhiteSpace(sourceFolderPath) || string.IsNullOrWhiteSpace(targetFolderPath) || string.IsNullOrWhiteSpace(fileName))
            {
                MessageBox.Show("请填写文件夹路径、目标文件夹路径和文件名称。");
                return;
            }

            try
            {
                // 打开文件资源管理器并选择文件
                Process.Start("explorer.exe", sourceFolderPath);
                System.Threading.Thread.Sleep(2000); // 等待文件资源管理器打开

                // 模拟Ctrl+A（全选）操作
                var inputSimulator = new InputSimulator();
                inputSimulator.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.VK_A);

                // 模拟Ctrl+C（复制）操作
                inputSimulator.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.VK_C);

                // 打开目标文件夹
                Process.Start("explorer.exe", targetFolderPath);
                System.Threading.Thread.Sleep(2000); // 等待目标文件夹打开

                // 模拟Ctrl+V（粘贴）操作
                inputSimulator.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.VK_V);

                MessageBox.Show("文件复制成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误：{ex.Message}");
            }
        }

        private void CopyAndPaste_Click(object sender, RoutedEventArgs e)
        {
            CopyAndPasteFilesWithKeyboard();
        }

        private void SelectSourceFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // 将选择的文件夹路径显示在 WPF TextBox 中
                    FolderPathTextBox.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void SelectTargetFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    TargetFolderPathTextBox.Text = folderDialog.SelectedPath;
                }
            }
        }
    }
}
