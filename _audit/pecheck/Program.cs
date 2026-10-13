using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

if (args.Length < 1) { Console.WriteLine("usage: pecheck <dll>"); return 1; }
string path = args[0];
using var fs = File.OpenRead(path);
using var pe = new PEReader(fs);
var mr = pe.GetMetadataReader();

bool IsPublic(MethodAttributes a) => (a & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
bool IsPublicField(FieldAttributes a) => (a & FieldAttributes.FieldAccessMask) == FieldAttributes.Public;

int mAll = 0, mReal = 0, acc = 0, ctors = 0;
int fieldsAll = 0, fieldsEnumLiteral = 0, fieldsNonEnum = 0;
int props = 0, events = 0, enumTypes = 0, enumMembers = 0;

foreach (var th in mr.TypeDefinitions)
{
    var t = mr.GetTypeDefinition(th);
    var vis = t.Attributes & TypeAttributes.VisibilityMask;
    if (vis != TypeAttributes.Public && vis != TypeAttributes.NestedPublic) continue;

    // enum detection: base type System.Enum
    bool isEnum = false;
    if (!t.BaseType.IsNil)
    {
        var bt = t.BaseType;
        if (bt.Kind == HandleKind.TypeReference)
        {
            var tr = mr.GetTypeReference((TypeReferenceHandle)bt);
            if (mr.GetString(tr.Name) == "Enum") isEnum = true;
        }
    }
    if (isEnum) enumTypes++;

    foreach (var mh in t.GetMethods())
    {
        var m = mr.GetMethodDefinition(mh);
        if (!IsPublic(m.Attributes)) continue;
        string n = mr.GetString(m.Name);
        if (n == ".ctor" || n == ".cctor") { ctors++; mAll++; continue; }
        if (n.StartsWith("get_") || n.StartsWith("set_") || n.StartsWith("add_") || n.StartsWith("remove_")) acc++;
        else mReal++;
        mAll++;
    }
    foreach (var fh in t.GetFields())
    {
        var f = mr.GetFieldDefinition(fh);
        if (!IsPublicField(f.Attributes)) continue;
        fieldsAll++;
        bool isStatic = (f.Attributes & FieldAttributes.Static) != 0;
        bool isLiteral = (f.Attributes & FieldAttributes.Literal) != 0;
        if (isEnum && isStatic && isLiteral) { fieldsEnumLiteral++; enumMembers++; fieldsEnumLiteral += 0; }
        else fieldsNonEnum++;
    }
    foreach (var ph in t.GetProperties())
    {
        var p = mr.GetPropertyDefinition(ph);
        var a = p.GetAccessors();
        if ((!a.Getter.IsNil && IsPublic(mr.GetMethodDefinition(a.Getter).Attributes))
         || (!a.Setter.IsNil && IsPublic(mr.GetMethodDefinition(a.Setter).Attributes))) props++;
    }
    foreach (var eh in t.GetEvents())
    {
        var e = mr.GetEventDefinition(eh);
        var a = e.GetAccessors();
        if ((!a.Adder.IsNil && IsPublic(mr.GetMethodDefinition(a.Adder).Attributes))
         || (!a.Remover.IsNil && IsPublic(mr.GetMethodDefinition(a.Remover).Attributes))) events++;
    }
}

Console.WriteLine($"enumTypes={enumTypes} enumMembers(literals)={enumMembers}");
Console.WriteLine($"methodsAll(incl acc+ctors)={mAll}  methodsReal={mReal}  accessors={acc}  ctors={ctors}");
Console.WriteLine($"fieldsAll={fieldsAll}  fieldsEnumLiteral={fieldsEnumLiteral}  fieldsNonEnum={fieldsNonEnum}");
Console.WriteLine($"props={props}  events={events}");
Console.WriteLine("-- candidate 'member' totals --");
Console.WriteLine($"mAll+props+fieldsAll+events        = {mAll + props + fieldsAll + events}");
Console.WriteLine($"mAll+props+fieldsNonEnum+events    = {mAll + props + fieldsNonEnum + events}");
Console.WriteLine($"mReal+props+fieldsAll+events       = {mReal + props + fieldsAll + events}");
Console.WriteLine($"mReal+props+fieldsNonEnum+events   = {mReal + props + fieldsNonEnum + events}");
Console.WriteLine($"mAll+props+fieldsAll               = {mAll + props + fieldsAll}");
Console.WriteLine($"mAll+props+fieldsNonEnum           = {mAll + props + fieldsNonEnum}");
return 0;
