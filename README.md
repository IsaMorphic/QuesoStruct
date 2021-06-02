# QuesoStruct 🧀

Boilerplate is out, and *QuesoStruct* is in! ¿Porque queso? ¡¿Porque no?! Me gusta queso 😋 

**QuesoStruct is a pure C# .NET based source generation package** that allows its users to **instantly write binary serialization code** for **both reading and writing** scenarios, complete **with built-in support for data structures** common to binary file formats including:

* **All C# .NET number primitives** (except `decimal`!) in **both little and big endian** byte orders
* **Null terminating strings** (with **any encoding** you choose via built in `System.Text.Encoding`s)

* **Sequential/contiguous structure arrays** (with **customizable termination behavior** i.e conditional per item, specific number of items, or until EOI)
* **Singly/doubly linked lists** (implemented abstractly to **support any kind of built-in or user created pointer type**)
* **Structured data substreams** (to wrap/expose certain sections of input data as a .NET `Stream`)

and an easy way to **markup your classes and code** to make the magic happen!

# ¿Que es eso? (What's the big idea?)

To best demonstrate the power of QuesoStruct, let's use a brief concrete code example to show you just how much you'll gain in both productivity and code quality by using the library! 

Suppose you want to write a collection of binary formatted data structures to a file on the disk. 

Each instance in the collection is laid out according to the following class definition:

```csharp
public class MyStruct 
{ 
    public uint Id { get; set; }
    
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}
```

Now lets see a before and after!

### Before using QuesoStruct (reading from a file): 

```csharp
/* below methods are defined somewhere in MyStruct class... */
public static MyStruct ReadInstance(BinaryReader reader) 
{
    var myInstance = new MyStruct
    {
        Id = reader.ReadUInt32(),
        
        X = reader.ReadSingle(),
        Y = reader.ReadSingle(),
        Z = reader.ReadSingle(),
    };
    
    return myInstance;
}

public static MyStruct[] ReadCollection(BinaryReader reader)
{
    var collection = new List<MyStruct>();
    
    while(true) 
    { 
    	try
	    {
            collection.Add(ReadInstance(reader));
    	} catch(EndOfStreamException) {
            break;
        }
    }
    
    return collection.ToArray();
}

/* ...then used wherever else to retrieve the instances from a Stream wrapped in a BinaryReader. */
using var file = File.OpenRead("my-file.bin");
using var reader = new BinaryReader(file);

var instances = MyStruct.ReadCollection(reader);
foreach(var instance in instances)
{
    Console.WriteLine($"Hello world from MyStruct with ID: {instance.Id}!");
}
```

### After using QuesoStruct (same example): 

`MyStruct` class code is slightly modified with annotations for the source generator:

```csharp
using QuesoStruct;

[StructType]
public partial class MyStruct /* class is now marked as partial to insert QuesoStruct goodies! */
{ 
    [StructMember]
    public uint Id { get; set; }
    
    [StructMember]
    public float X { get; set; }
    
    [StructMember]
    public float Y { get; set; }
    
    [StructMember]
    public float Z { get; set; }
    
/* free to define other members (can be public, too) without the annotation so that they are not read or written. */
}
```

And then we just use the generated code! Note how now, the verbose `BinaryReader` boilerplate is nowhere to be seen! It has instead been replaced by very minimal API boilerplate for abstractly accessing and configuring the generated code for our use case.

```csharp
using QuesoStruct;
using QuesoStruct.Types.Collections;
...
/* in Main method of Program.cs or something */
using var file = File.OpenRead("my-file.bin");

// Minimal boilerplate for accessing DeltaStruct API:
// 1. access generated code
var serializer = Serializers.Get<Collection<MyStruct>>();

// 2. wrap stream, configuring endianess and string encoding
var context = new Context(file, Context.SystemEndianess, Encoding.ASCII);

// 3. execute generated code!
var instances = serializer.Read(context);

// 4. inside "instances" is now a QuesoStruct Collection<MyStruct>. Use it like a IList<MyStruct>!!!
foreach(var instance in instances)
{
    Console.WriteLine($"Hello world from MyStruct with ID: {instance.Id}!");
}
```

In the "before example", imagine having to write code like that for several other classes, whose members might be many times more numerous! *It'd be a spaghetti code nightmare!!* **QuesoStruct saves you the trouble so you can focus exclusively on your schema and processing.**

