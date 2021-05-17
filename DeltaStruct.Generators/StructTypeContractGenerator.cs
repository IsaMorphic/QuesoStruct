using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace DeltaStruct.Generators
{
    [Generator]
    public class StructTypeContractGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var syntax = context.SyntaxReceiver as MarkedClassSyntaxReceiver;
            foreach (var classDef in syntax.Classes)
            {
                var className = classDef.Identifier.ValueText;
                context.AddSource($"{className}_serializer", GenerateSerializerSource(classDef));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new MarkedClassSyntaxReceiver());
        }

        private string GenerateSerializerSource(ClassDeclarationSyntax classDef)
        {
            var className = classDef.Identifier.ValueText;

            var text = new StringBuilder();

            text.Append($"using DeltaStruct;");
            text.Append($"using System;");
            text.Append($"using System.IO;");

            text.Append($"namespace {(classDef.Parent as NamespaceDeclarationSyntax).Name} {{");
            text.Append($"public class G_{className}_Serializer : ISerializer<{className}> {{");

            text.Append($"static G_{className}_Serializer() {{");
            text.Append($"Serializers.Register<{className}, G_{className}_Serializer>();");

            text.Append($"}} public {className} ReadFromStream(Stream stream) {{");

            text.Append($"var inst = new {className}();");
            text.Append($"var buffer = new byte[8];");

            foreach (var member in classDef.Members)
            {
                if (member is PropertyDeclarationSyntax prop &&
                    prop.Modifiers.All(m => m.ValueText == "public") &&
                    prop.AccessorList.Accessors.Any(a => a.Keyword.ValueText == "set") &&
                    prop.AttributeLists.Single().Attributes
                    .Any(a => a.Name.ToString() == "StructMember"))
                {
                    var propName = prop.Identifier.ValueText;
                    var typeName = prop.Type.ToString();

                    switch (typeName)
                    {
                        case "byte":
                            text.Append($"inst.{propName} = stream.ReadByte();");
                            break;
                        case "sbyte":
                            text.Append($"inst.{propName} = unchecked((sbyte)stream.ReadByte());");
                            break;

                        case "ushort":
                            text.Append("stream.Read(buffer, 0, sizeof(ushort))");
                            text.Append($"inst.{propName} = BitConverter.ToUInt16(buffer, 0);");
                            break;
                        case "short":
                            text.Append("stream.Read(buffer, 0, sizeof(short))");
                            text.Append($"inst.{propName} = BitConverter.ToInt16(buffer, 0);");
                            break;

                        case "uint":
                            text.Append("stream.Read(buffer, 0, sizeof(uint))");
                            text.Append($"inst.{propName} = BitConverter.ToUInt32(buffer, 0);");
                            break;
                        case "int":
                            text.Append("stream.Read(buffer, 0, sizeof(int))");
                            text.Append($"inst.{propName} = BitConverter.ToInt32(buffer, 0);");
                            break;

                        case "ulong":
                            text.Append("stream.Read(buffer, 0, sizeof(ulong))");
                            text.Append($"inst.{propName} = BitConverter.ToUInt64(buffer, 0);");
                            break;
                        case "long":
                            text.Append("stream.Read(buffer, 0, sizeof(long))");
                            text.Append($"inst.{propName} = BitConverter.ToInt64(buffer, 0);");
                            break;

                        case "float":
                            text.Append("stream.Read(buffer, 0, sizeof(float))");
                            text.Append($"inst.{propName} = BitConverter.ToSingle(buffer, 0);");
                            break;
                        case "double":
                            text.Append("stream.Read(buffer, 0, sizeof(double))");
                            text.Append($"inst.{propName} = BitConverter.ToDouble(buffer, 0);");
                            break;
                    }
                }
            }

            text.Append($"}} }} }}");

            return text.ToString();
        }

        private class MarkedClassSyntaxReceiver : ISyntaxReceiver
        {
            public HashSet<ClassDeclarationSyntax> Classes { get; }

            public MarkedClassSyntaxReceiver()
            {
                Classes = new HashSet<ClassDeclarationSyntax>();
            }

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax cds &&
                   (cds.AttributeLists.SingleOrDefault()?.Attributes
                   .Any(attr => attr.Name.ToString() == "StructType") ?? false))
                {
                    Classes.Add(cds);
                }
            }
        }
    }
}
