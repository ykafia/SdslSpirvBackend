using Spv;
using Spv.Generator;
using static Spv.Specification;
using Stride.Core.Shaders.Ast;
using Stride.Core.Shaders.Ast.Stride;


namespace Stride.Shaders.Spirv
{
    public class ShaderModule : Module
    {
        ShaderClassType program;
        public ShaderModule(ShaderClassType program) : base(Specification.Version)
        {
            this.program = program;
        }

        public void Construct(StageEntryPoint entry)
        {
            
        }
    }
}