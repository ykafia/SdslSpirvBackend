using Spv;
using Spv.Generator;
using static Spv.Specification;
using Stride.Core.Shaders.Ast;
using Stride.Core.Shaders.Ast.Stride;
using StrideVariable = Stride.Core.Shaders.Ast.Variable;
using Stride.Core.Shaders.Ast.Hlsl;

namespace Stride.Shaders.Spirv
{
    public partial class ShaderModule : Module
    {
        Shader program;
        public StructElement InputType;
        public StructElement OutputType;
        Instruction Streams;
        public Dictionary<string, StructElement> Elements = new();
        // public Dictionary<string, Instruction> ZeroValues = new();
        
        

        public ShaderModule(Shader program) : base(Specification.Version)
        {
            this.program = program;
        }

        public byte[] Construct(ShaderStage stage, string entryPoint)
        {
            AddCapability(Capability.Shader);
            SetMemoryModel(AddressingModel.Logical, MemoryModel.Simple);
            // var structTypes = program.Declarations.Where(x => x is StructType type && !dataNames.Contains(type.Name.Text)).Cast<StructType>();
            // Generate declared types
            
            foreach(StructType p in program.Declarations.Where(x => x is StructType))
            {
                Elements[p.Name.Text] = new StructElement(this,p);
            }
            
            
            

            // Input handling
            Instruction inputv = Variable(TypePointer(StorageClass.Input, InputType.RawType,true), StorageClass.Input);
            Instruction outputv = Variable(TypePointer(StorageClass.Output, OutputType.RawType,true), StorageClass.Output);

            
            Instruction vec4Var = Variable(TypePointer(StorageClass.Function,TypeVector(TypeFloat(32),4),true), StorageClass.Output);
            
            Name(inputv, "input");
            Name(outputv, "output");
            AddGlobalVariable(inputv);
            AddGlobalVariable(outputv);
            AddLocalVariable(vec4Var);

            // Generate Main method

            MethodDefinition mainMethod = (MethodDefinition)program.Declarations.First(x => x is MethodDefinition m && m.Name.Text == entryPoint);
            Instruction voidType = TypeVoid();
            Instruction mainFunctionType = TypeFunction(voidType, true);
            Instruction mainFunction = Function(voidType, FunctionControlMask.MaskNone, mainFunctionType);
            AddLabel(Label());
            Instruction tmpIn = Load(InputType.RawType,inputv);
            Dictionary<string,Instruction> variables = new();
            // foreach(var s in mainMethod.Body.Statements)
            // {
            //     ConvertStatement(s, ref variables);
            // }
            var chains = new Dictionary<string,AccessChainData>();
            var streamsType = Elements["VS_STREAMS"];
            streamsType.GetAllAccessChains(ref chains, new List<int>(),"myVar");
            var tptr = TypePointer(StorageClass.Function,streamsType.RawType);
            var vstreams = Variable(tptr,StorageClass.Function);
            AddLocalVariable(vstreams);

            var zero = ConstantOf(0);
            var zero4 = ConstantOf("float4",0f,0,1,0);
            var ptrFloatType = TypePointer(StorageClass.Function, TypeFloat(32));
            var access = AccessChain(ptrFloatType,vstreams, zero);

            var ptr = Load(TypeFloat(32),access);
            var cst = ConstantOf(0f);
            Store(ptr,cst);
            Store(vec4Var,zero4);
            

            Return();
            FunctionEnd();
            AddEntryPoint(ExecutionModel.Vertex, mainFunction, "main", inputv, outputv);
            AddExecutionMode(mainFunction, ExecutionMode.OriginUpperLeft);
            return Generate();
        }
        public IValueElement GetOrCreateSPVType(string name)
        {
            // TODO : Create type instruction SPV
            if(IsStructType(name))
                return Elements[name];
            else
                return SDSLTypeToSPVType(name);
        }

        public bool IsStructType(string name) => program.Declarations.Any(x => x is StructType s && s.Name.Text == name); 
        public StructType GetStructType(string name) => (StructType)program.Declarations.First(x => x is StructType st && st.Name.Text == name);
        private FieldElement SDSLTypeToSPVType(string name)
        {
            return name switch 
            {
                "half" => new FieldElement{ValueType = name, RawType = TypeFloat(16)},
                "double" => new FieldElement{ValueType = name, RawType = TypeFloat(64)},
                "ushort" => new FieldElement{ValueType = name, RawType = TypeInt(16,0)},
                "uint" => new FieldElement{ValueType = name, RawType = TypeInt(32,0)},
                "ulong" => new FieldElement{ValueType = name, RawType = TypeInt(64,0)},
                "short" => new FieldElement{ValueType = name, RawType = TypeInt(16,1)},
                "int" => new FieldElement{ValueType = name, RawType = TypeInt(32,1)},
                "long" => new FieldElement{ValueType = name, RawType = TypeInt(64,1)},
                "byte" => new FieldElement{ValueType = name, RawType = TypeInt(8,0)},
                "sbyte" =>  new FieldElement{ValueType = name, RawType = TypeInt(8,1)},
                "float" => new FieldElement{ValueType = name, RawType = TypeFloat(32)},
                "float2" => new FieldElement{ValueType = name, RawType = TypeVector(TypeFloat(32),2)},
                "float3" => new FieldElement{ValueType = name, RawType = TypeVector(TypeFloat(32),3)},
                "float4" => new FieldElement{ValueType = name, RawType = TypeVector(TypeFloat(32),4)},
                _ => throw new Exception("Type not found")
            };
        }        
    }
}