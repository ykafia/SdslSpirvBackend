using Stride.Core.Shaders;
using Stride.Core;
using Stride.Core.Shaders.Ast;
using Stride.Core.Shaders.Ast.Stride;
using Stride.Shaders.Parser;
using Stride.Shaders.Parser.Mixins;

namespace Stride.Shaders.Spirv
{
    public class Compiler
    {
        ShaderMixinSource shader;
        ShaderMixinParser parser;
        public Compiler(ShaderMixinSource mixin, ShaderMixinParser parser)
        {
            this.shader = mixin;
            this.parser = parser;
        }

        public ShaderModule Compile(StageEntryPoint entry, ShaderMacro[] macro)
        {
            var shaderClass = parser.Parse(shader,macro).Shader;
            if(shaderClass is null) throw new Exception("No shader class found");
            var sm = new ShaderModule(shaderClass);        
            sm.Construct(entry);
            return sm;    
        }
    }
}