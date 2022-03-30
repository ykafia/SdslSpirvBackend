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
        Dictionary<string, Instruction> InputRegister = new();
        Dictionary<string, Instruction> OutputRegister = new();
        Dictionary<string, Instruction> StreamsRegister = new();
        
        

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
                if(p.Name.Text.Contains("INPUT"))
                    GenerateInputs(p);
                if(p.Name.Text.Contains("OUTPUT"))
                    GenerateOutputs(p);
                if(p.Name.Text.Contains("STREAMS"))
                    GenerateStream(p);
            }

            // TODO: add void function
        }
        public void GenerateInputs(StructType s)
        {
            s.Fields.ForEach(
                f =>
                {
                    InputRegister.Add(f.Name,TypePointer(StorageClass.Input,GetOrCreateSPVType(f.Type.Name)));
                }
            );
        }
        public void GenerateOutputs(StructType s)
        {
            s.Fields.ForEach(
                f =>
                {
                    OutputRegister.Add(f.Name,TypePointer(StorageClass.Output,GetOrCreateSPVType(f.Type.Name)));
                }
            );
        }
        public void GenerateStream(StructType s)
        {
            s.Fields.ForEach(
                f =>
                {
                    StreamsRegister.Add(f.Name,TypePointer(StorageClass.Output,GetOrCreateSPVType(f.Type.Name)));
                }
            );
        }
        public Instruction GetOrCreateSPVType(string t)
        {
            if(TypeRegister.ContainsKey(t))
            {
                return TypeRegister[t];
            }
            // TODO : Create type instruction SPV
            TypeRegister[t] = SDSLTypeToSPVType(t);
            return TypeRegister[t];
        }

        private Instruction SDSLTypeToSPVType(string name)
        {
            return name switch 
            {
                "half" => TypeFloat(16),
                "float" => TypeFloat(32),
                "double" => TypeFloat(64),
                "ushort" => TypeInt(16,0),
                "uint" => TypeInt(32,0),
                "ulong" => TypeInt(64,0),
                "short" => TypeInt(16,1),
                "int" => TypeInt(32,1),
                "long" => TypeInt(64,1),
                "byte" => TypeInt(0,1),
                "sbyte" => TypeInt(8,1),
                "float2" => TypeVector(GetOrCreateSPVType("float"),2),
                "float3" => TypeVector(GetOrCreateSPVType("float"),3),
                "float4" => TypeVector(GetOrCreateSPVType("float"),4),
                _ => throw new Exception("Type not found")
            };
        }
    }
}