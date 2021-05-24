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
                var className = string.Join(".", classDef
                .AncestorsAndSelf().Reverse().Select(node =>
                (node as ClassDeclarationSyntax)?.Identifier.ValueText ??
                (node as NamespaceDeclarationSyntax)?.Name.ToString()
                ).Where(id => id != null));

                context.AddSource($"{className}_serializer", GenerateSerializerSource(classDef));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new MarkedClassSyntaxReceiver());
        }

        private bool IsStructMember(MemberDeclarationSyntax member)
        {
            return member is PropertyDeclarationSyntax prop &&
                    prop.Modifiers.All(m => m.ValueText == "public") &&
                    (prop.AccessorList?.Accessors.Any(a => a.Keyword.ValueText == "set") ?? false) &&
                    (prop.AttributeLists.SingleOrDefault()?.Attributes
                    .Any(a => a.Name.ToString() == "StructMember") ?? false);
        }

        private string GenerateSerializerSource(ClassDeclarationSyntax classDef)
        {
            var className = classDef.Identifier.ValueText;
            var namespaceName = (classDef.Ancestors()
                    .First(a => a is NamespaceDeclarationSyntax)
                    as NamespaceDeclarationSyntax).Name;

            var classParams = classDef.TypeParameterList?.ToString();
            var fullClassName = className + classParams;

            var classConstr = classDef.ConstraintClauses.ToString();

            GenericNameSyntax refInfo = null;
            string refTypeName = null;

            if (classDef.BaseList != null)
            {
                refInfo = classDef.BaseList?.Types
                .Select(bt => bt.Type as GenericNameSyntax)
                .Where(g => g != null)
                .Single(g => g.Identifier.ValueText == "IStructReference");

                if (refInfo != null)
                {
                    refTypeName = refInfo.TypeArgumentList.Arguments.Single().ToString();
                }
            }

            var text = new StringBuilder();

            text.Append($"using DeltaStruct;");
            text.Append($"using DeltaStruct.Types;");
            text.Append($"using DeltaStruct.Types.Pointers;");
            text.Append($"using System;");
            text.Append($"using System.Collections.Generic;");
            text.Append($"using System.IO;");

            // Namespace declaration
            text.Append($"namespace {namespaceName} {{");

            var parentClasses = classDef
                .Ancestors().Reverse().Select(node =>
                (node as ClassDeclarationSyntax)?.Identifier.ValueText
                ).Where(id => id != null);

            foreach (var parent in parentClasses)
            {
                text.Append($"public partial class {parent} {{");
            }

            // Inject serializer registration code
            if (refInfo == null)
            {
                text.Append($"public partial class {fullClassName} : IStructInstance {classConstr} {{");
            }
            else 
            {
                text.Append($"public partial class {fullClassName} : {refInfo}, IStructInstance {classConstr} {{");
            }
            text.Append($"public static void Init() {{ if(!Serializers.Has<{fullClassName}>()) {{");
            text.Append($"Serializers.Register<{fullClassName}, Serializer>(); }} }}");

            // Inject properties/fields
            text.Append($"private readonly Context context;");

            text.Append($"public long? Offset {{ get; set; }}");
            text.Append($"public IStructInstance Parent {{ get; set; }}");

            text.Append($"public HashSet<IStructReference> References {{ get; }}");

            if (refTypeName != null)
            {
                text.Append($"public {refTypeName} Instance {{ get => instance; set {{ instance = value; Update(); }} }} private {refTypeName} instance;");
            }

            // Inject constructor
            text.Append($"public {className}(Context context) {{");
            text.Append($"this.context = context; Offset = context.Stream.Position; Parent = context.Current; References = new HashSet<IStructReference>();");
            text.Append($"}} public {className}() {{ }}");

            // Declare serializer class
            text.Append($"public class Serializer : ISerializer<{fullClassName}> {{");

            // Implement Read
            text.Append($"public {fullClassName} Read(Context context) {{");
            text.Append($"var stream = context.Stream; var buffer = new byte[8].AsSpan();");
            text.Append($"var inst = new {fullClassName}(context);");

            foreach (var member in classDef.Members)
            {
                if (IsStructMember(member))
                {
                    var prop = member as PropertyDeclarationSyntax;
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
                            text.Append($"context.Current = inst; inst.{propName} = Serializers.Get<{typeName}>().Read(context);");
                            break;
                    }
                }
            }

            if (refTypeName != null)
            {
                text.Append($"var posBefore = stream.Position; stream.Seek(inst.OffsetValue.Value, SeekOrigin.Begin);");
                text.Append($"inst.Instance = Serializers.Get<{refTypeName}>().Read(context);");
                text.Append($"inst.Instance.References.Add(inst); stream.Seek(posBefore, SeekOrigin.Begin);");
            }

            text.Append($"context.Instances.Add(inst); inst.OnAfterRead(); return inst; }}");
            text.Append($"public void Write({fullClassName} inst, Context context) {{");
            text.Append($"inst.OnBeforeWrite(context); var stream = context.Stream; var buffer = new byte[8].AsSpan();");

            foreach (var member in classDef.Members)
            {
                if (IsStructMember(member))
                {
                    var prop = member as PropertyDeclarationSyntax;
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
                            text.Append($"Serializers.Get<{typeName}>().Write(inst.{propName}, context);");
                            break;
                    }
                }
            }

            text.Append($"}} }} }} }}");

            foreach (var parent in parentClasses)
            {
                text.Append($"}} ");
            }

            return text.ToString();
        }

        public class MarkedClassSyntaxReceiver : ISyntaxReceiver
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
