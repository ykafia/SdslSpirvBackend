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
        (Instruction PtrType,Instruction RawType) InputType;
        (Instruction PtrType,Instruction RawType) OutputType;
        Instruction Streams;
        public Dictionary<string, StructElement> Elements = new();
        
        

        public ShaderModule(Shader program) : base(Specification.Version)
        {
            this.program = program;
        }

        public ShaderModule Construct(StageEntryPoint entry)
        {
            AddCapability(Capability.Shader);
            SetMemoryModel(AddressingModel.Logical, MemoryModel.Simple);
            // var structTypes = program.Declarations.Where(x => x is StructType type && !dataNames.Contains(type.Name.Text)).Cast<StructType>();
            // Generate declared types
            
            foreach(StructType p in program.Declarations.Where(x => x is StructType))
            {
                if(new List<string>{"_STREAMS","_INPUT","_OUTPUT"}.All(x => !p.Name.Text.EndsWith(x)))
                {
                    Elements[p.Name.Text] = new StructElement(this,p);
                }
            }
            
            
            

            // Input handling

            Instruction inputv = Variable(InputType.PtrType, StorageClass.Input);
            Instruction outputv = Variable(OutputType.PtrType, StorageClass.Output);

            Name(inputv, "inputTest");
            Name(outputv, "outputColor");
            AddGlobalVariable(inputv);
            AddGlobalVariable(outputv);

            // Generate Main method

            MethodDefinition mainMethod = (MethodDefinition)program.Declarations.First(x => x is MethodDefinition);
            var mainFunctionType = TypeFunction(TypeVoid(),true);
            var mainFunction = Function(TypeVoid(),FunctionControlMask.MaskNone,mainFunctionType);
            AddLabel(Label());
            Instruction tmpIn = Load(InputType.RawType,inputv);
            Dictionary<string,Instruction> variables = new();
            foreach(var s in mainMethod.Body.Statements)
            {
                ConvertStatement(s, ref variables);
            }
            Return();
            FunctionEnd();
            AddEntryPoint(ExecutionModel.Vertex, mainFunction, "main", inputv, outputv);
            AddExecutionMode(mainFunction, ExecutionMode.OriginUpperLeft);
            return this;
            // TODO: add void function
        }
        public FieldElement GetOrCreateSPVType(string name)
        {
            // TODO : Create type instruction SPV
            var t = SDSLTypeToSPVType(name);
            return t;
        }

        public bool IsStructType(string name) => Elements.ContainsKey(name); 

        private FieldElement SDSLTypeToSPVType(string name)
        {
            return name switch 
            {
                "half" => new FieldElement{ValueType = name, RawType = TypeFloat(16), ZeroValue = Constant(TypeFloat(16),0.0)},
                "double" => new FieldElement{ValueType = name, RawType = TypeFloat(64), ZeroValue = Constant(TypeFloat(64),0.0)},
                "ushort" => new FieldElement{ValueType = name, RawType = TypeInt(16,0), ZeroValue = Constant(TypeInt(16,0),0.0)},
                "uint" => new FieldElement{ValueType = name, RawType = TypeInt(32,0), ZeroValue = Constant(TypeInt(32,0),0.0)},
                "ulong" => new FieldElement{ValueType = name, RawType = TypeInt(64,0), ZeroValue = Constant(TypeInt(64,0),0.0)},
                "short" => new FieldElement{ValueType = name, RawType = TypeInt(16,1), ZeroValue = Constant(TypeInt(16,1),0.0)},
                "int" => new FieldElement{ValueType = name, RawType = TypeInt(32,1), ZeroValue = Constant(TypeInt(32,1),0.0)},
                "long" => new FieldElement{ValueType = name, RawType = TypeInt(64,1), ZeroValue = Constant(TypeInt(64,1),0.0)},
                "byte" => new FieldElement{ValueType = name, RawType = TypeInt(8,0), ZeroValue = Constant(TypeInt(8,0),0.0)},
                "sbyte" =>  new FieldElement{ValueType = name, RawType = TypeInt(8,1), ZeroValue = Constant(TypeInt(8,1),0.0)},
                "float" => new FieldElement{ValueType = name, RawType = TypeFloat(32), ZeroValue = Constant(TypeFloat(32),0.0)},
                "float2" => new FieldElement{ValueType = name, RawType = TypeVector(TypeFloat(32),2), ZeroValue = Constant(TypeVector(TypeFloat(32),2),0.0)},
                "float3" => new FieldElement{ValueType = name, RawType = TypeVector(TypeFloat(32),3), ZeroValue = Constant(TypeVector(TypeFloat(32),3),0.0)},
                "float4" => new FieldElement{ValueType = name, RawType = TypeVector(TypeFloat(32),3), ZeroValue = Constant(TypeVector(TypeFloat(32),4),0.0)},
                _ => throw new Exception("Type not found")
            };
        }
        private void ConvertStatement(Statement s, ref Dictionary<string,Instruction> variables)
        {
            if(s is DeclarationStatement ds)
            {
                var pvar = ((Variable)ds.Content);
                var vType = GetOrCreateSPVType(pvar.Type.Name.Text);
                var variable = Variable(vType.RawType,StorageClass.Generic);
                Name(variable, pvar.Name.Text);


                Instruction declaration;
                Expression expr = ((Variable)ds.Content).InitialValue;
                if(expr is CastExpression cexp)
                {
                    // if(cexp.Target. is Literal l)
                    if(cexp.From is LiteralExpression l && (int)l.Literal.Value == 0)
                    {
                        // declaration = ConstantComposite(vType, );
                        variables.Add(pvar.Name.Text,variable);
                        AddLocalVariable(variable);
                        // Function(TypePointer(StorageClass.Generic,vType),FunctionControlMask.MaskNone,
                        // Store(variable,new Instruction(Op.OpConstant));
                    }
                }
                // new Instruction(Op.OpPtrCastToGeneric)

                
            }
        }
    }
}