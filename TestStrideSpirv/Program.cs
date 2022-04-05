// See https://aka.ms/new-console-template for more information
using TestStrideSpirv;
using Stride.Shaders.Spirv;
using Stride.Core.Shaders;
using Stride.Core.Shaders.Parser;
using Stride.Core.Shaders.Grammar.Stride;
using SPIRVCross;
using static SPIRVCross.SPIRV;
using System.Linq;
using Stride.Core.Storage;
using Stride.Core.IO;
using Stride.Shaders;
using Stride.Shaders.Parser;
using Stride.Shaders.Parser.Mixins;
using System.Reflection;

Console.WriteLine("Hello, World!");

var assetPath = "./";
var mixinName = "TestVertexStreamFull";

var fs = VirtualFileSystem.MountFileSystem(assetPath, "./Shaders/SDSL/");

var db = new ObjectDatabase(assetPath,VirtualFileSystem.ApplicationDatabaseIndexName);
var dbfp = new DatabaseFileProvider(db);

var search = await VirtualFileSystem.ListFiles(assetPath,"*.sdsl",VirtualSearchOption.TopDirectoryOnly);
var shaderMixinParser = new ShaderMixinParser(dbfp);
shaderMixinParser.SourceManager.LookupDirectoryList.Add(assetPath);
foreach(var s in search)
    shaderMixinParser.SourceManager.AddShaderSource(Path.GetFileNameWithoutExtension(s), new StreamReader(VirtualFileSystem.OpenStream(s, VirtualFileMode.Open,VirtualFileAccess.Read)).ReadToEnd(),s);

var source = new ShaderMixinSource();
source.Mixins.Add(new ShaderClassSource(mixinName));
var compiler = new Compiler(source,shaderMixinParser);
shaderMixinParser.ParseA
ShaderModule sm = compiler.Compile(StageEntryPoint.VSMain,new Stride.Shaders.ShaderMacro[]{});

sm.Generate().ToGlsl();

// new TestModule().Construct().Generate().ToGlsl();


// var vertPath = @".\Shaders\GLSL\tri.frag.spv";

// System.IO.File.ReadAllBytes(vertPath).ToJson();