using Spv;
using Spv.Generator;
using static Spv.Specification;
using Stride.Core.Shaders.Ast;
using Stride.Core.Shaders.Ast.Stride;
using StrideVariable = Stride.Core.Shaders.Ast.Variable;


namespace Stride.Shaders.Spirv
{
    public class ShaderModule : Module
    {
        Shader program;
        Dictionary<string,Instruction> TypeRegister = new();

        public ShaderModule(Shader program) : base(Specification.Version)
        {
            this.program = program;
        }

        public void Construct(StageEntryPoint entry)
        {
            AddCapability(Capability.Shader);
            SetMemoryModel(AddressingModel.Logical, MemoryModel.Simple);
            foreach(StructType p in program.Declarations.Where(x => x is StructType))
            {
                Console.WriteLine(p);
            }

            // TODO: add void function
        }
        public void GenerateInputs(StructType s)
        {
            s.Fields.ForEach(
                f =>
                {
                    f.Type.Name
                }
            );
        }
        public Instruction GetOrCreateSPVType(TypeBase t)
        {
            if(TypeRegister.ContainsKey(t.Name))
            {
                return TypeRegister[t.Name];
            }
            // TODO : Create type instruction SPV
            return null;
        }

        private static void SDSLTypeToSPVType()
        {

        }
    }
}