using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Stride.Core.IO;
using Stride.Core.Storage;
using Stride.Shaders.Parser;

using Stride.Shaders.Compiler;
using TestStrideSpirv;

namespace Stride.Shaders.Spirv
{

    public class SPVEffectCompiler : EffectCompilerBase
    {
        public override IVirtualFileProvider FileProvider { get; set; }
        private static readonly Object WriterLock = new Object();
        private readonly object shaderMixinParserLock = new object();
        private ShaderMixinParser shaderMixinParser;
        public List<string> SourceDirectories { get; private set; }
        public Dictionary<string, string> UrlToFilePath { get; private set; }
        public bool UseFileSystem { get; set; }

        public SPVEffectCompiler(IVirtualFileProvider fileProvider)
        {
            FileProvider = fileProvider;
            SourceDirectories = new List<string>();
            UrlToFilePath = new Dictionary<string, string>();
            UseFileSystem = true;
        }

        private ShaderMixinParser GetMixinParser()
        {
            var search = new string[]{"./TestVertexStreamFull.sdsl"};
            lock (shaderMixinParserLock)
            {
                // Generate the AST from the mixin description
                if (shaderMixinParser == null)
                {
                    shaderMixinParser = new ShaderMixinParser(FileProvider);
                    shaderMixinParser.SourceManager.LookupDirectoryList.AddRange(SourceDirectories); // TODO: temp
                    shaderMixinParser.SourceManager.UseFileSystem = UseFileSystem;
                    shaderMixinParser.SourceManager.UrlToFilePath = UrlToFilePath; // TODO: temp
                    shaderMixinParser.SourceManager.LookupDirectoryList.Add("./");
                    foreach (var s in search)
                        shaderMixinParser.SourceManager.AddShaderSource(Path.GetFileNameWithoutExtension(s), new StreamReader(VirtualFileSystem.OpenStream(s, VirtualFileMode.Open, VirtualFileAccess.Read)).ReadToEnd(), s);
                }
                return shaderMixinParser;
            }
        }

        public override ObjectId GetShaderSourceHash(string type)
        {
            return GetMixinParser().SourceManager.GetShaderSourceHash(type);
        }

        public override TaskOrResult<EffectBytecodeCompilerResult> Compile(ShaderMixinSource mixinTree, EffectCompilerParameters effectParameters, CompilerParameters compilerParameters)
        {
            var shaderMixinSource = mixinTree;
            var fullEffectName = mixinTree.Name;

            // Make a copy of shaderMixinSource. Use deep clone since shaderMixinSource can be altered during compilation (e.g. macros)
            var shaderMixinSourceCopy = new ShaderMixinSource();
            shaderMixinSourceCopy.DeepCloneFrom(shaderMixinSource);
            shaderMixinSource = shaderMixinSourceCopy;
            shaderMixinSource.AddMacro("class", "shader");

            var parsingResult = GetMixinParser().Parse(shaderMixinSource, shaderMixinSource.Macros.ToArray());
            foreach(var stageBinding in parsingResult.EntryPoints)
            {
                var spvModule = 
                    new ShaderModule(parsingResult.Shader)
                    .Construct(stageBinding.Key,stageBinding.Value);
                File.WriteAllBytes("./test.spv",spvModule);
                Console.WriteLine(spvModule.ToGlsl());
            }
            var result = new EffectBytecodeCompilerResult();
            // var bc = new ShaderBytecode(ObjectId.New(), spvModule.Generate());
            return result;
        }
    }
}