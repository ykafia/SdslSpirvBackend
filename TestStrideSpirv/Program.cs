// See https://aka.ms/new-console-template for more information
using TestStrideSpirv;
using Stride.Shaders.Spirv;
using Stride.Core.Shaders;
using Stride.Core.Shaders.Parser;
using Stride.Core.Shaders.Grammar.Stride;
using SPIRVCross;
using static SPIRVCross.SPIRV;

Console.WriteLine("Hello, World!");

var grammar = ShaderParser.GetGrammar<StrideGrammar>();

var parser = ShaderParser.GetParser<StrideGrammar>();

// var shaderText = File.ReadAllText("./Test.sdsl");

// var shader = parser.Parse(shaderText, "./Test.sdsl").Shader;
// Console.WriteLine(shader);

var vertPath = @"C:\Users\youness_kafia\Documents\GitHub\SdslSpirvBackend\TestStrideSpirv\Shaders\GLSL\tri.vert.spv";

System.IO.File.ReadAllBytes(vertPath).ToGlsl();