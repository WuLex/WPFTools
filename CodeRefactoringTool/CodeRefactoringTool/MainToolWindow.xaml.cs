using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.CSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using System.IO;
using System.Windows;
//using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;

namespace CodeRefactoringTool
{
    /// <summary>
    /// MainRWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainToolWindow : Window
    {
        private async void FormatCodeButton_Click(object sender, RoutedEventArgs e)
        {
            var code = EditorTextBox.Text;

            var document = CreateDocument(code);
            var formattedDocument = await Formatter.FormatAsync(document);
            var formattedCode = await formattedDocument.GetTextAsync();

            EditorTextBox.Text = formattedCode.ToString();
        }

        private async void EncapsulateFieldButton_Click(object sender, RoutedEventArgs e)
        {
            var code = EditorTextBox.Text;

            var document = CreateDocument(code);
            var root = await document.GetSyntaxRootAsync();

            var field = root.DescendantNodes().OfType<FieldDeclarationSyntax>().FirstOrDefault();

            if (field == null)
            {
                MessageBox.Show("No field found.");
                return;
            }

            var variable = field.Declaration.Variables.First();

            var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(variable.Identifier.Text), variable.Identifier.Text)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
                .AddAccessorListAccessors(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithBody(SyntaxFactory.Block(
                            SyntaxFactory.ExpressionStatement(
                                SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                    SyntaxFactory.IdentifierName(variable.Identifier),
                                    SyntaxFactory.IdentifierName("value")))))
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            var newRoot = root.ReplaceNode(field, property);
            var newDocument = document.WithSyntaxRoot(newRoot);

            var formattedDocument = await Formatter.FormatAsync(newDocument);
            var formattedCode = await formattedDocument.GetTextAsync();

            EditorTextBox.Text = formattedCode.ToString();
        }

        private async void ExtractMethodButton_Click(object sender, RoutedEventArgs e)
        {
            var code = EditorTextBox.Text;

            var document = CreateDocument(code);
            var root = await document.GetSyntaxRootAsync();

            var statement = root.DescendantNodes().OfType<StatementSyntax>().FirstOrDefault();

            if (statement == null)
            {
                MessageBox.Show("No statement found.");
                return;
            }

            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "NewMethod")
                .WithBody(SyntaxFactory.Block(statement));

            var newRoot = root.ReplaceNode(statement, method);
            var newDocument = document.WithSyntaxRoot(newRoot);

            var formattedDocument = await Formatter.FormatAsync(newDocument);
            var formattedCode = await formattedDocument.GetTextAsync();

            EditorTextBox.Text = formattedCode.ToString();
        }

        private async void ExtractInterfaceButton_Click(object sender, RoutedEventArgs e)
        {
            var code = EditorTextBox.Text;

            var document = CreateDocument(code);
            var root = await document.GetSyntaxRootAsync();

            var @class = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

            if (@class==null) 
            { 
                MessageBox.Show("No class found.");
                return;
            }

            var interfaceName = @class.Identifier.ValueText + "Interface";

            var interfaceMembers = @class.Members
                .OfType<MethodDeclarationSyntax>()
                .Select(method =>
                    SyntaxFactory.MethodDeclaration(
                        method.ReturnType,
                        method.Identifier)
                    .WithParameterList(method.ParameterList)
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
                .ToList<MemberDeclarationSyntax>();

            var @interface = SyntaxFactory.InterfaceDeclaration(interfaceName)
                .WithMembers(SyntaxFactory.List(interfaceMembers))
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));

            var newRoot = root.ReplaceNode(@class, @interface);
            var newDocument = document.WithSyntaxRoot(newRoot);

            var formattedDocument = await Formatter.FormatAsync(newDocument);
            var formattedCode = await formattedDocument.GetTextAsync();

            EditorTextBox.Text = formattedCode.ToString();
        }

        private async void ExtractClassFromParameterButton_Click(object sender, RoutedEventArgs e)
        {
            var code = EditorTextBox.Text;

            var document = CreateDocument(code);
            var root = await document.GetSyntaxRootAsync();

            var parameter = root.DescendantNodes().OfType<ParameterSyntax>().FirstOrDefault();

            if (parameter == null)
            {
                MessageBox.Show("No parameter found.");
                return;
            }

            var className = parameter.Type.ToString();

            var classMembers = root.DescendantNodes()
                .OfType<FieldDeclarationSyntax>()
                .Where(field => field.Declaration.Variables.Any(variable => variable.Identifier.Text == parameter.Identifier.Text))
                .ToList<MemberDeclarationSyntax>();

            var @class = SyntaxFactory.ClassDeclaration(className)
                .WithMembers(SyntaxFactory.List(classMembers))
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));

            var newRoot = root.ReplaceNode(parameter, SyntaxFactory.Parameter(parameter.Identifier).WithType(SyntaxFactory.ParseTypeName(className)));
            newRoot = newRoot.ReplaceNode(newRoot.DescendantNodes().OfType<FieldDeclarationSyntax>().First(), @class);

            var newDocument = document.WithSyntaxRoot(newRoot);

            var formattedDocument = await Formatter.FormatAsync(newDocument);
            var formattedCode = await formattedDocument.GetTextAsync();

            EditorTextBox.Text = formattedCode.ToString();
        }

        //private static Microsoft.CodeAnalysis.Document CreateDocument111(string code)
        //{
        //    var syntaxTree = CSharpSyntaxTree.ParseText(code);
        //    var compilation = CSharpCompilation.Create("Temp")
        //        .AddReferences(
        //            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        //            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
        //            MetadataReference.CreateFromFile(typeof(SyntaxFactory).Assembly.Location),
        //            MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly.Location),
        //            MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location))
        //        .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
        //        .AddSyntaxTrees(syntaxTree);


        //    var parseOptions = ((CSharpCompilationOptions)compilation.Options).ParseOptions;
        //    return compilation.GetSemanticModel(syntaxTree).SyntaxTree.GetRoot()
        //        .SyntaxTree.WithRootAndOptions(compilation.SyntaxTrees[0].GetRoot(), parseOptions)
        //        .WithFilePath(System.IO.Path.GetRandomFileName()).WithChangedText(syntaxTree.GetText());
        //}
        //private static Microsoft.CodeAnalysis.Document CreateDocument11(string code)
        //{
        //    var tree = SyntaxFactory.ParseSyntaxTree(code, new CSharpParseOptions());

        //    var syntaxTree = CSharpSyntaxTree.ParseText(code);
        //    var compilation = CSharpCompilation.Create("Temp")
        //        .AddReferences(
        //            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        //            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
        //            MetadataReference.CreateFromFile(typeof(SyntaxFactory).Assembly.Location),
        //            MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly.Location),
        //            MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location))
        //        .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
        //        .AddSyntaxTrees(syntaxTree);

        //    var root = syntaxTree.GetCompilationUnitRoot();
        //    var options = compilation.SyntaxTrees[0].Options;
        //    var newTree = syntaxTree.WithRootAndOptions(root, options);

        //    Microsoft.CodeAnalysis.Document document = new Microsoft.CodeAnalysis.Document();
        //    var document =new Microsoft.CodeAnalysis.Document(newTree).FromSyntaxTree(newTree, path: "Temp");
        //}
        public static Microsoft.CodeAnalysis.Document CreateDocument(string code)
        {
            string fileName = "TempFile.cs";
            SourceText sourceText = SourceText.From(code);

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceText, path: fileName);
            string projectName = "TempProject";
            string documentName = "TempDocument";

            DocumentId documentId = DocumentId.CreateNewId(ProjectId.CreateNewId(projectName), debugName: documentName);

            VersionStamp versionStamp = VersionStamp.Create();
            DocumentInfo documentInfo = DocumentInfo.Create(
                documentId,
                documentName,
                loader: TextLoader.From(TextAndVersion.Create(sourceText, versionStamp)),
                filePath: fileName);

            AdhocWorkspace workspace = new AdhocWorkspace();
            ProjectId projectId = ProjectId.CreateNewId(projectName);
            ProjectInfo projectInfo = ProjectInfo.Create(projectId, version: versionStamp, name: projectName, assemblyName: projectName, language: LanguageNames.CSharp);

            workspace.AddProject(projectInfo);
            Project project = workspace.CurrentSolution.GetProject(projectId)!;

            //project = project.AddDocument(documentInfo).GetProject(projectId)!;
            //Microsoft.CodeAnalysis.Document document = project.GetDocument(documentId)!;
            Microsoft.CodeAnalysis.Document document = project.AddDocument(documentName, sourceText);
            return document;
        }


    }
}
