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
        Dictionary<string,Instruction> TypeRegister = new();
        (Instruction PtrType,Instruction RawType) InputType;
        (Instruction PtrType,Instruction RawType) OutputType;
        Instruction Streams;
        
        

        public ShaderModule(Shader program) : base(Specification.Version)
        {
            this.program = program;
        }

        public ShaderModule Construct(StageEntryPoint entry)
        {
            AddCapability(Capability.Shader);
            SetMemoryModel(AddressingModel.Logical, MemoryModel.Simple);
            TypeRegister["void"] = TypeVoid();
            // Generate declared types
            foreach(StructType p in program.Declarations.Where(x => x is StructType))
            {
                if(p.Name.Text.Contains("INPUT"))
                {
                    InputType = (TypePointer(StorageClass.Input, GetOrCreateStructType(p)),GetOrCreateStructType(p));
                    Name(InputType.RawType, p.Name);
                }
                else if(p.Name.Text.Contains("OUTPUT"))
                {
                    OutputType = (TypePointer(StorageClass.Output, GetOrCreateStructType(p)),GetOrCreateStructType(p));
                    Name(OutputType.RawType, p.Name);
                }
                else if(p.Name.Text.Contains("STREAMS"))
                {
                    Streams = GetOrCreateStructType(p);
                    Name(Streams,p.Name);
                }
                else
                {
                    Name(GetOrCreateStructType(p), p.Name);
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
            var mainFunctionType = TypeFunction(TypeRegister["void"],true);
            var mainFunction = Function(TypeRegister["void"],FunctionControlMask.MaskNone,mainFunctionType);
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
        public Instruction GetOrCreateStructType(StructType s)
        {   
            
            if(!TypeRegister.ContainsKey(s.Name.Text))
            {
                TypeRegister.Add(s.Name.Text, TypeStruct(true,s.Fields.Select(f => GetOrCreateSPVType(f.Type.Name)).ToArray()));
                for(int i =0; i< s.Fields.Count; i++)
                {
                    MemberName(TypeRegister[s.Name.Text],i,s.Fields[i].Name.Text);
                }
            }
            return TypeRegister[s.Name.Text];
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
        private void ConvertStatement(Statement s, ref Dictionary<string,Instruction> variables)
        {
            if(s is DeclarationStatement ds)
            {
                var pvar = ((Variable)ds.Content);
                var vType = GetOrCreateSPVType(pvar.Type.Name.Text);
                var variable = Variable(vType,StorageClass.Generic);
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
                        // Store(variable,new Instruction(Op.OpConstant));
                    }
                }
                // new Instruction(Op.OpPtrCastToGeneric)

                
            }
        }
    }
}