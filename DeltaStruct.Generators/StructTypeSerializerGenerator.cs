using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeltaStruct.Generators
{
    [Generator]
    public class StructTypeSerializerGenerator : ISourceGenerator
    {
        private static readonly Dictionary<string, string> Types =
            new Dictionary<string, string>()
            {
                { "ushort", "UInt16" },
                { "short", "Int16" },
                { "uint", "UInt32" },
                { "int", "Int32" },
                { "ulong", "UInt64" },
                { "long", "Int64" },
                { "float", "Single" },
                { "double", "Double" },
            };

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

            // Namespace declaration
            text.Append($"namespace {(classDef.Parent as NamespaceDeclarationSyntax).Name} {{");

            // Inject serializer registration code
            text.Append($"public partial class {className} : IStructInstance {{");
            text.Append($"public static void Init() {{ if(!Serializers.Has<{className}>()) {{");
            text.Append($"Serializers.Register<{className}, G_{className}_Serializer>(); }} }}");

            // Inject properties/fields
            text.Append($"private readonly Context context;");

            text.Append($"public long? Offset {{ get; set; }}");
            text.Append($"public IStructInstance Parent {{ get; set; }}");

            text.Append($"public HashSet<IStructReference> References {{ get; }}");

            // Inject constructor
            text.Append($"public {className}(Context context) {{");
            text.Append($"this.context = context; Offset = context.Stream.Position; Parent = context.Current; References = new HashSet<IStructReference>();");
            text.Append($"}} public {className}() {{ }}");

            // Declare serializer class
            text.Append($"}} public class G_{className}_Serializer : ISerializer<{className}> {{");

            // Implement Read
            text.Append($"public {className} Read(Context context) {{");
            text.Append($"var stream = context.Stream; var buffer = new byte[8].AsSpan();");
            text.Append($"var inst = new {className}(context); context.Current = inst;");

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
                            text.Append($"var v_{propName} = stream.ReadByte();");
                            text.Append($"if(v_{propName} < 0) throw new EndOfStreamException(\"Failed to read {typeName} {className}.{propName} from stream!\");");
                            text.Append($"inst.{propName} = (byte)v_{propName}");
                            break;
                        case "sbyte":
                            text.Append($"var v_{propName} = stream.ReadByte();");
                            text.Append($"if(v_{propName} < 0) throw new EndOfStreamException(\"Failed to read {typeName} {className}.{propName} from stream!\");");
                            text.Append($"inst.{propName} = unchecked((sbyte)v_{propName});");
                            break;

                        case "ushort":
                        case "short":
                        case "uint":
                        case "int":
                        case "ulong":
                        case "long":
                        case "float":
                        case "double":
                            text.Append($"var s_{propName} = buffer[0..sizeof({typeName})];");
                            text.Append($"if (context.Endianess != Context.SystemEndianess) s_{propName}.Reverse();");
                            text.Append($"var v_{propName} = stream.Read(s_{propName});");
                            text.Append($"if(v_{propName} <= 0) throw new EndOfStreamException(\"Failed to read {typeName} {className}.{propName} from stream!\");");
                            text.Append($"inst.{propName} = BitConverter.To{Types[typeName]}(s_{propName});");
                            break;

                        default:
                            text.Append($"{typeName}.Init(); inst.{propName} = Serializers.Get<{typeName}>().Read(context);");
                            break;
                    }
                }
            }

            var refInfo = classDef.BaseList.Types
                .SingleOrDefault(type => type.Type is GenericNameSyntax g && g.Identifier.ValueText == "IStructReference")
                .Type as GenericNameSyntax;

            if (refInfo != null)
            {
                var refTypeName = refInfo.TypeArgumentList.Arguments.Single().ToString();
                text.Append($"var posBefore = stream.Position; stream.Seek(inst.OffsetValue, SeekOrigin.Begin);");
                text.Append($"{refTypeName}.Init(); inst.Instance = Serializers.Get<{refTypeName}>().Read(context);");
                text.Append($"inst.Instance.References.Add(this); stream.Seek(posBefore, SeekOrigin.Begin);");
            }

            text.Append($"context.Instances.Add(inst); return inst; }}");
            text.Append($"public void Write({className} inst, Context context) {{");
            text.Append($"var stream = context.Stream; var buffer = new byte[8].AsSpan();");

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
                            text.Append($"stream.WriteByte(inst.{propName});");
                            break;
                        case "sbyte":
                            text.Append($"stream.WriteByte(unchecked((byte)inst.{propName}));");
                            break;

                        case "ushort":
                        case "short":
                        case "uint":
                        case "int":
                        case "ulong":
                        case "long":
                        case "float":
                        case "double":
                            text.Append($"BitConverter.TryWriteBytes(buffer, inst.{propName});");
                            text.Append($"var s_{propName} = buffer[0..sizeof({typeName})];");
                            text.Append($"if (context.Endianess != Context.SystemEndianess) s_{propName}.Reverse();");
                            text.Append($"stream.Write(s_{propName});");
                            break;

                        default:
                            text.Append($"{typeName}.Init(); Serializers.Get<{typeName}>().Write(inst.{propName}, context);");
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
