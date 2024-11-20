using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;


namespace CodeRefactoringTool
{
    /// <summary>
    /// 代码重构工具的主窗口类
    /// 提供基本的代码重构功能，包括提取方法、内联方法和重命名变量
    /// </summary>
    public partial class RefactorWindow : Window
    {
        // 存储当前编辑的代码文本
        private string code;
        private bool isLineNumbersVisible = true;
        private bool isMinimapVisible = false;
        private bool hasUnsavedChanges = false;

        /// <summary>
        /// 主窗口构造函数，初始化界面组件
        /// </summary>
        public RefactorWindow()
        {
            InitializeComponent();
            InitializeEditor();
        }
        /// <summary>
        /// 初始化编辑器设置
        /// </summary>
        private void InitializeEditor()
        {
            // 绑定文本改变事件
            CodeTextBox.TextChanged += (s, e) =>
            {
                hasUnsavedChanges = true;
                UpdateStatusBar();
            };

            // 绑定光标位置改变事件
            CodeTextBox.SelectionChanged += (s, e) => UpdateStatusBar();

            // 初始化状态栏
            UpdateStatusBar();
        }

        /// <summary>
        /// 更新状态栏信息
        /// </summary>
        private void UpdateStatusBar()
        {
            // 获取当前行和列
            int line = GetCurrentLine();
            int column = GetCurrentColumn();
            LineColumnStatus.Text = $"Line: {line} Col: {column}";

            // 更新文件类型
            FileTypeStatus.Text = "C#";
        }

        /// <summary>
        /// 获取当前行号
        /// </summary>
        private int GetCurrentLine()
        {
            int caretIndex = CodeTextBox.CaretIndex;
            var text = CodeTextBox.Text.Substring(0, caretIndex);
            return text.Split('\n').Length;
        }

        /// <summary>
        /// 获取当前列号
        /// </summary>
        private int GetCurrentColumn()
        {
            int caretIndex = CodeTextBox.CaretIndex;
            var text = CodeTextBox.Text.Substring(0, caretIndex);
            var lines = text.Split('\n');
            if (lines.Length > 0)
                return lines[lines.Length - 1].Length + 1;
            return 1;
        }

        /// <summary>
        /// 新建文件
        /// </summary>
        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (hasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    "Do you want to save changes?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Save_Click(sender, e);
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            CodeTextBox.Clear();
            code = string.Empty;
            hasUnsavedChanges = false;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (hasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    "Do you want to save changes?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Save_Click(sender, e);
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "C# Files (*.cs)|*.cs|All Files (*.*)|*.*",
                Title = "Open Code File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                    {
                        CodeTextBox.Text = reader.ReadToEnd();
                        code = CodeTextBox.Text;
                        hasUnsavedChanges = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening file: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "C# Files (*.cs)|*.cs|All Files (*.*)|*.*",
                Title = "Save Code File"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        writer.Write(CodeTextBox.Text);
                        hasUnsavedChanges = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// 退出应用程序
        /// </summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (hasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    "Do you want to save changes before exiting?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Save_Click(sender, e);
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            Application.Current.Shutdown();
        }

        /// <summary>
        /// 切换行号显示
        /// </summary>
        private void ToggleLineNumbers_Click(object sender, RoutedEventArgs e)
        {
            isLineNumbersVisible = !isLineNumbersVisible;
            // 这里需要实现行号显示的具体逻辑
            // 可以使用自定义的TextBox控件或者其他控件来实现行号显示
            MessageBox.Show("Line numbers feature will be implemented in future updates.",
                "Feature Not Available", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 切换小地图显示
        /// </summary>
        private void ToggleMinimap_Click(object sender, RoutedEventArgs e)
        {
            isMinimapVisible = !isMinimapVisible;
            // 这里需要实现代码小地图的具体逻辑
            // 可以使用缩放的TextBox或者自定义控件来实现小地图功能
            MessageBox.Show("Minimap feature will be implemented in future updates.",
                "Feature Not Available", MessageBoxButton.OK, MessageBoxImage.Information);
        }



        ///// <summary>
        ///// 打开文件对话框，读取选中的代码文件
        ///// </summary>
        //private void Open_Click(object sender, RoutedEventArgs e)
        //{
        //    Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
        //    {
        //        Filter = "C# Files (*.cs)|*.cs|All Files (*.*)|*.*", // 设置文件过滤器
        //        Title = "Open Code File"  // 设置对话框标题
        //    };

        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        try
        //        {
        //            using (StreamReader reader = new StreamReader(openFileDialog.FileName))
        //            {
        //                CodeTextBox.Text = reader.ReadToEnd();
        //                code = CodeTextBox.Text; // 更新code变量
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 保存文件对话框，将当前代码保存到文件
        ///// </summary>
        //private void Save_Click(object sender, RoutedEventArgs e)
        //{
        //    Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
        //    {
        //        Filter = "C# Files (*.cs)|*.cs|All Files (*.*)|*.*",
        //        Title = "Save Code File"
        //    };

        //    if (saveFileDialog.ShowDialog() == true)
        //    {
        //        try
        //        {
        //            using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
        //            {
        //                writer.Write(CodeTextBox.Text);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //}

        /// <summary>
        /// 提取方法重构：将选中的代码块提取为一个新方法
        /// </summary>
        private void ExtractMethod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 确保有选中的文本
                if (string.IsNullOrEmpty(CodeTextBox.SelectedText))
                {
                    MessageBox.Show("Please select the code to extract.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 解析代码为语法树
                SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
                CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

                // 获取选中的代码范围
                var selectedCode = CodeTextBox.SelectedText;
                var methodName = "ExtractedMethod"; // 可以通过对话框让用户输入方法名

                // 创建新的方法语法
                var methodDeclaration = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    SyntaxFactory.Identifier(methodName))
                    .WithModifiers(
                        SyntaxFactory.TokenList(
                            SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
                    .WithBody(
                        SyntaxFactory.Block(
                            SyntaxFactory.ParseStatement(selectedCode)));

                // 将新方法插入到第一个类声明中
                var firstClass = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
                var newRoot = root.ReplaceNode(
                    firstClass,
                    firstClass.AddMembers(methodDeclaration));

                // 用方法调用替换选中的代码
                var methodCall = $"{methodName}();";
                CodeTextBox.Text = CodeTextBox.Text.Replace(selectedCode, methodCall);
                code = CodeTextBox.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during method extraction: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 内联方法重构：将方法调用替换为方法体
        /// </summary>
        private void InlineMethod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 解析代码为语法树
                SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
                CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

                // 获取光标位置的方法调用
                var methodInvocation = root.DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .FirstOrDefault();

                if (methodInvocation == null)
                {
                    MessageBox.Show("No method call found at cursor position.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 查找对应的方法声明
                var methodName = methodInvocation.Expression.ToString();
                var methodDeclaration = root.DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .FirstOrDefault(m => m.Identifier.ValueText == methodName);

                if (methodDeclaration != null)
                {
                    // 获取方法体
                    var methodBody = methodDeclaration.Body.ToString();
                    // 替换方法调用为方法体
                    CodeTextBox.Text = CodeTextBox.Text.Replace(
                        methodInvocation.ToString() + ";",
                        methodBody);
                    code = CodeTextBox.Text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during method inlining: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 重命名变量重构：将选中的变量重命名为新名称
        /// </summary>
        private void RenameVariable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 获取选中的变量名
                var selectedText = CodeTextBox.SelectedText;
                if (string.IsNullOrEmpty(selectedText))
                {
                    MessageBox.Show("Please select the variable to rename.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 弹出输入对话框获取新名称
                var dialog = new Window
                {
                    Title = "Rename Variable",
                    Width = 300,
                    Height = 150,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                var stackPanel = new System.Windows.Controls.StackPanel { Margin = new Thickness(10) };
                var textBox = new System.Windows.Controls.TextBox { Margin = new Thickness(0, 5, 0, 5) };
                var button = new System.Windows.Controls.Button { Content = "OK" };

                stackPanel.Children.Add(new System.Windows.Controls.TextBlock { Text = "New variable name:" });
                stackPanel.Children.Add(textBox);
                stackPanel.Children.Add(button);

                dialog.Content = stackPanel;
                button.Click += (s, args) => { dialog.DialogResult = true; dialog.Close(); };

                if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(textBox.Text))
                {
                    // 解析代码为语法树
                    SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
                    CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

                    // 查找所有匹配的标识符
                    var identifiers = root.DescendantTokens()
                        .Where(t => t.IsKind(SyntaxKind.IdentifierToken) && t.ValueText == selectedText);

                    // 替换所有匹配的标识符
                    foreach (var identifier in identifiers.Reverse())
                    {
                        CodeTextBox.Text = CodeTextBox.Text.Remove(identifier.Span.Start, identifier.Span.Length)
                            .Insert(identifier.Span.Start, textBox.Text);
                    }

                    code = CodeTextBox.Text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during variable renaming: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 更新存储的代码文本
        /// </summary>
        private void Refactor_Click(object sender, RoutedEventArgs e)
        {
            code = CodeTextBox.Text;
        }
    }
}