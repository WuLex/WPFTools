using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using System.Linq;
using System.Windows;
using Microsoft.CodeAnalysis.Text;
using System;

namespace CodeRefactoringTool
{
    /// <summary>
    /// MainToolWindow.xaml 的交互逻辑 - 代码重构工具主窗口
    /// </summary>
    public partial class MainToolWindow : Window
    {
        /// <summary>
        /// 初始化窗口
        /// </summary>
        public MainToolWindow()
        {
            InitializeComponent();
            // 设置默认示例代码
            EditorTextBox.Text = GetSampleCode();
        }

        /// <summary>
        /// 生成示例代码用于测试
        /// </summary>
        private string GetSampleCode()
        {
            return @"
public class Person
{
    private string name;
    private int age;

    public void ProcessData(UserData userData)
    {
        Console.WriteLine(userData.ToString());
        var result = userData.Value * 2;
        Console.WriteLine(result);
    }

    public void DisplayInfo()
    {
        Console.WriteLine($""Name: {name}, Age: {age}"");
    }
}";
        }

        /// <summary>
        /// 格式化代码按钮点击事件
        /// 使用Roslyn格式化工具对代码进行格式化
        /// </summary>
        private async void FormatCodeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var code = EditorTextBox.Text;
                if (string.IsNullOrWhiteSpace(code))
                {
                    MessageBox.Show("Please enter some code first.");
                    return;
                }

                var document = CreateDocument(code);
                var formattedDocument = await Formatter.FormatAsync(document);
                var formattedCode = await formattedDocument.GetTextAsync();

                EditorTextBox.Text = formattedCode.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error formatting code: {ex.Message}");
            }
        }

        /// <summary>
        /// 字段封装按钮点击事件
        /// 将选中的字段转换为属性
        /// </summary>
        private async void EncapsulateFieldButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var code = EditorTextBox.Text;
                if (string.IsNullOrWhiteSpace(code))
                {
                    MessageBox.Show("Please enter some code first.");
                    return;
                }

                var document = CreateDocument(code);
                var root = await document.GetSyntaxRootAsync();

                // 获取第一个字段声明
                var field = root.DescendantNodes()
                    .OfType<FieldDeclarationSyntax>()
                    .FirstOrDefault();

                if (field == null)
                {
                    MessageBox.Show("No field found to encapsulate.");
                    return;
                }

                var variable = field.Declaration.Variables.First();

                // 创建属性声明
                var property = SyntaxFactory.PropertyDeclaration(
                        field.Declaration.Type,
                        SyntaxFactory.Identifier(char.ToUpper(variable.Identifier.Text[0]) + variable.Identifier.Text.Substring(1)))
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithBody(SyntaxFactory.Block(
                                SyntaxFactory.ReturnStatement(
                                    SyntaxFactory.IdentifierName(variable.Identifier)))),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithBody(SyntaxFactory.Block(
                                SyntaxFactory.ExpressionStatement(
                                    SyntaxFactory.AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        SyntaxFactory.IdentifierName(variable.Identifier),
                                        SyntaxFactory.IdentifierName("value"))))));

                // 替换节点并格式化
                var newRoot = root.ReplaceNode(field, property);
                var newDocument = document.WithSyntaxRoot(newRoot);
                var formattedDocument = await Formatter.FormatAsync(newDocument);
                var formattedCode = await formattedDocument.GetTextAsync();

                EditorTextBox.Text = formattedCode.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error encapsulating field: {ex.Message}");
            }
        }

        /// <summary>
        /// 提取方法按钮点击事件
        /// 将选中的代码块提取为新方法
        /// </summary>
        private async void ExtractMethodButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var code = EditorTextBox.Text;
                if (string.IsNullOrWhiteSpace(code))
                {
                    MessageBox.Show("Please enter some code first.");
                    return;
                }

                var document = CreateDocument(code);
                var root = await document.GetSyntaxRootAsync();

                // 查找要提取的语句块
                var statements = root.DescendantNodes()
                    .OfType<StatementSyntax>()
                    .Where(s => s is not BlockSyntax)
                    .Take(2)
                    .ToList();

                if (!statements.Any())
                {
                    MessageBox.Show("No suitable statements found to extract.");
                    return;
                }

                // 创建新方法
                var methodName = "ExtractedMethod";
                var method = SyntaxFactory.MethodDeclaration(
                        SyntaxFactory.PredefinedType(
                            SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                        SyntaxFactory.Identifier(methodName))
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword))
                    .WithBody(SyntaxFactory.Block(statements));

                // 创建方法调用
                var methodCall = SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName(methodName)));

                // 明确指定SyntaxNode类型的数组
                SyntaxNode[] newNodes = new SyntaxNode[] { methodCall, method };

                // 替换原始语句
                var newRoot = root.ReplaceNode(statements[0], newNodes);

                var newDocument = document.WithSyntaxRoot(newRoot);
                var formattedDocument = await Formatter.FormatAsync(newDocument);
                var formattedCode = await formattedDocument.GetTextAsync();

                EditorTextBox.Text = formattedCode.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error extracting method: {ex.Message}");
            }
        }
        /// <summary>
        /// 提取接口按钮点击事件
        /// 从现有类中提取接口定义
        /// </summary>
        private async void ExtractInterfaceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var code = EditorTextBox.Text;
                if (string.IsNullOrWhiteSpace(code))
                {
                    MessageBox.Show("Please enter some code first.");
                    return;
                }

                var document = CreateDocument(code);
                var root = await document.GetSyntaxRootAsync();

                var @class = root.DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .FirstOrDefault();

                if (@class == null)
                {
                    MessageBox.Show("No class found to extract interface from.");
                    return;
                }

                // 创建接口名称
                var interfaceName = "I" + @class.Identifier.ValueText;

                // 提取公共方法作为接口成员
                var interfaceMembers = @class.Members
                    .OfType<MethodDeclarationSyntax>()
                    .Where(m => m.Modifiers.Any(SyntaxKind.PublicKeyword))
                    .Select(method =>
                        SyntaxFactory.MethodDeclaration(
                                method.ReturnType,
                                method.Identifier)
                            .WithParameterList(method.ParameterList)
                            .WithSemicolonToken(
                                SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
                    .ToList<MemberDeclarationSyntax>();

                // 创建接口声明
                var @interface = SyntaxFactory.InterfaceDeclaration(interfaceName)
                    .WithModifiers(
                        SyntaxFactory.TokenList(
                            SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                    .WithMembers(SyntaxFactory.List(interfaceMembers));

                // 更新类以实现新接口
                var newClass = @class
                    .AddBaseListTypes(
                        SyntaxFactory.SimpleBaseType(
                            SyntaxFactory.IdentifierName(interfaceName)));

                // 将接口和修改后的类添加到代码中
                var newRoot = root
                    .ReplaceNode(@class, new SyntaxNode[] { @interface, newClass });
                var newDocument = document.WithSyntaxRoot(newRoot);
                var formattedDocument = await Formatter.FormatAsync(newDocument);
                var formattedCode = await formattedDocument.GetTextAsync();

                EditorTextBox.Text = formattedCode.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error extracting interface: {ex.Message}");
            }
        }

        /// <summary>
        /// 从参数提取类按钮点击事件
        /// 将方法参数相关的字段提取为新的类
        /// </summary>
        private async void ExtractClassFromParameterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var code = EditorTextBox.Text;
                if (string.IsNullOrWhiteSpace(code))
                {
                    MessageBox.Show("Please enter some code first.");
                    return;
                }

                var document = CreateDocument(code);
                var root = await document.GetSyntaxRootAsync();

                // 查找第一个参数
                var parameter = root.DescendantNodes()
                    .OfType<ParameterSyntax>()
                    .FirstOrDefault();

                if (parameter == null)
                {
                    MessageBox.Show("No parameter found to extract class from.");
                    return;
                }

                // 创建新类名
                var className = parameter.Identifier.Text + "Class";

                // 收集相关字段
                var fields = root.DescendantNodes()
                    .OfType<FieldDeclarationSyntax>()
                    .Where(field => field.Declaration.Variables
                        .Any(v => v.Identifier.Text.Contains(parameter.Identifier.Text,
                            StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                // 创建属性
                var properties = fields.Select(field =>
                    SyntaxFactory.PropertyDeclaration(
                            field.Declaration.Type,
                            SyntaxFactory.Identifier(char.ToUpper(field.Declaration.Variables.First().Identifier.Text[0]) +
                                                   field.Declaration.Variables.First().Identifier.Text.Substring(1)))
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                        .AddAccessorListAccessors(
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))))
                    .ToList<MemberDeclarationSyntax>();

                // 创建新类
                var newClass = SyntaxFactory.ClassDeclaration(className)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddMembers(properties.ToArray());

                // 更新参数类型
                var newParameter = parameter.WithType(SyntaxFactory.IdentifierName(className));

                // 替换节点
                var newRoot = root
                    .ReplaceNode(parameter, newParameter)
                    .InsertNodesBefore(
                        root.DescendantNodes().OfType<ClassDeclarationSyntax>().First(),
                        new[] { newClass });

                var newDocument = document.WithSyntaxRoot(newRoot);
                var formattedDocument = await Formatter.FormatAsync(newDocument);
                var formattedCode = await formattedDocument.GetTextAsync();

                EditorTextBox.Text = formattedCode.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error extracting class from parameter: {ex.Message}");
            }
        }

        /// <summary>
        /// 创建临时文档用于代码分析和重构
        /// </summary>
        /// <param name="code">要分析的代码文本</param>
        /// <returns>包含代码的Document对象</returns>
        private static Microsoft.CodeAnalysis.Document CreateDocument(string code)
        {
            // 创建源代码文本
            var sourceText = SourceText.From(code);

            // 创建工作区
            var workspace = new AdhocWorkspace();

            // 创建项目ID和版本戳
            var projectId = ProjectId.CreateNewId();
            var versionStamp = VersionStamp.Create();

            // 创建项目信息
            var projectInfo = ProjectInfo.Create(
                projectId,
                versionStamp,
                "TempProject",
                "TempProject",
                LanguageNames.CSharp)
                .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .WithMetadataReferences(new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
                });

            // 添加项目到工作区
            workspace.AddProject(projectInfo);
            var project = workspace.CurrentSolution.GetProject(projectId);

            // 创建文档并返回
            return project.AddDocument("TempFile.cs", sourceText);
        }
    }
}