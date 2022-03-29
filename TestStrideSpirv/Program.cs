// See https://aka.ms/new-console-template for more information
using TestStrideSpirv;
using Stride.Shaders.Spirv;
using Stride.Core.Shaders;
using Stride.Core.Shaders.Parser;
using Stride.Core.Shaders.Grammar.Stride;
using SPIRVCross;
using static SPIRVCross.SPIRV;
using System.Linq;
using Stride.Core.Shaders.Ast.Stride;

Console.WriteLine("Hello, World!");

var grammar = ShaderParser.GetGrammar<StrideGrammar>();

var parser = ShaderParser.GetParser<StrideGrammar>();

var sdslPath = "./Shaders/SDSL/TestVertexStreamFull.sdsl";
var shaderText = File.ReadAllText(sdslPath);

var shader = parser.Parse(shaderText, sdslPath).Shader;
Console.WriteLine(shader.Declarations.Where(x => x is ShaderClassType).Count());
Console.WriteLine(shader);

// var vertPath = @"C:\Users\youness_kafia\Documents\GitHub\SdslSpirvBackend\TestStrideSpirv\Shaders\GLSL\tri.vert.spv";

// System.IO.File.ReadAllBytes(vertPath).ToGlsl();