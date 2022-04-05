using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Stride.Core;
using Stride.Core.Diagnostics;
using Stride.Core.IO;
using Stride.Core.Serialization.Contents;
using Stride.Core.Storage;
using Stride.Rendering;
using Stride.Graphics;
using Stride.Shaders.Parser;
using Stride.Core.Shaders.Ast;
using Stride.Core.Shaders.Ast.Hlsl;
using Stride.Core.Shaders.Utility;
using Encoding = System.Text.Encoding;
using LoggerResult = Stride.Core.Diagnostics.LoggerResult;

using Stride.Shaders.Compiler;

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
        public override TaskOrResult<EffectBytecodeCompilerResult> Compile(ShaderMixinSource mixinTree, EffectCompilerParameters effectParameters, CompilerParameters compilerParameters)
        {
            throw new NotImplementedException();
        }
        private ShaderMixinParser GetMixinParser()
        {
            lock (shaderMixinParserLock)
            {
                // Generate the AST from the mixin description
                if (shaderMixinParser == null)
                {
                    shaderMixinParser = new ShaderMixinParser(FileProvider);
                    shaderMixinParser.SourceManager.LookupDirectoryList.AddRange(SourceDirectories); // TODO: temp
                    shaderMixinParser.SourceManager.UseFileSystem = UseFileSystem;
                    shaderMixinParser.SourceManager.UrlToFilePath = UrlToFilePath; // TODO: temp
                }
                return shaderMixinParser;
            }
        }
        public override ObjectId GetShaderSourceHash(string type)
        {
            return GetMixinParser().SourceManager.GetShaderSourceHash(type);
        }



    }
}