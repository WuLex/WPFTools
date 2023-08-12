using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Windows;

namespace CodeRefactoringTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private string code;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                {
                    CodeTextBox.Text = reader.ReadToEnd();
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    writer.Write(CodeTextBox.Text);
                }
            }
        }

        private void ExtractMethod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
                CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
                // Extract method code refactoring algorithm
                CodeTextBox.Text = root.ToFullString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InlineMethod_Click(object sender, RoutedEventArgs e)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            // Inline method code refactoring algorithm
            CodeTextBox.Text = root.ToFullString();
        }

        private void RenameVariable_Click(object sender, RoutedEventArgs e)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            // Rename variable code refactoring algorithm
            CodeTextBox.Text = root.ToFullString();
        }

        private void Refactor_Click(object sender, RoutedEventArgs e)
        {
            code = CodeTextBox.Text;
        }
    }
}