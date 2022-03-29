using Spv;
using Spv.Generator;
using static Spv.Specification;
using Stride.Core.Shaders.Ast;

namespace Stride.Shaders.Spirv
{
    public class ShaderModule : Module
    {
        Shader program;
        public ShaderModule(Shader program) : base(Specification.Version)
        {
            this.program = program;
        }

        public void Construct()
        {
            
        }
    }
}