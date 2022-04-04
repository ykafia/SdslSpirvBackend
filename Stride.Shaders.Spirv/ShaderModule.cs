using Spv;
using Spv.Generator;
using static Spv.Specification;
using Stride.Core.Shaders.Ast;
using Stride.Core.Shaders.Ast.Stride;
using StrideVariable = Stride.Core.Shaders.Ast.Variable;
using Stride.Core.Shaders.Ast.Hlsl;
using Stride.Shaders.Spirv.Abstraction;

namespace Stride.Shaders.Spirv
{
    public partial class ShaderModule : Module
    {
        Shader program;
        (Instruction PtrType,Instruction RawType) InputType;
        (Instruction PtrType,Instruction RawType) OutputType;
        Instruction Streams;
        Dictionary<string, StructElement> Elements = new();
        private readonly List<string> dataNames = new();
        private IEnumerable<string> declaredTypes;
        
        

        public ShaderModule(Shader program) : base(Specification.Version)
        {
            this.program = program;
            foreach(string s in new string[]{"VS","GS","PS","HS","DS","CS"})
                foreach(string s2 in new string[]{"INPUT","OUPUT","STREAM"})
                    dataNames.Add(s+"_"+s2);
        }

        public ShaderModule Construct(StageEntryPoint entry)
        {
            AddCapability(Capability.Shader);
            SetMemoryModel(AddressingModel.Logical, MemoryModel.Simple);
            var structTypes = program.Declarations.Where(x => x is StructType type && !dataNames.Contains(type.Name.Text)).Cast<StructType>();
            declaredTypes = structTypes.Select(x => x.Name.Text);
            // Generate declared types
            List<string> waitList = new();
            while(waitList != null)
            {
                var tmpList = new List<string>();
                foreach(StructType p in structTypes)
                {

                    if(!Elements.ContainsKey(p.Name.Text))
                    {
                        var fieldTypes = p.Fields.Select(x => x.Name.Text);
                        if(fieldTypes.All(x => !declaredTypes.Contains(x)))
                        {
                            // Create StructType
                        }
                        else
                        {
                            var structFields = fieldTypes.Where(declaredTypes.Contains);
                            if(structFields.All(Elements.ContainsKey))
                            {
                                // Create StructType
                            }
                            else
                            {
                                // tmpList.Add()
                            }
                        }
                    }
                    // If type is declared but not created
                    //      If type contains no declared types fields
                    //          Create type
                    //      Else
                    //          If type contains declared fields and not created
                    //              
                    // Name(GetOrCreateStructType(p), p.Name);
                    
                }
                if(tmpList.Count == 0)
                    waitList = null;
            }
            foreach(
                StructType p in program.Declarations.Where(
                    x => x is StructType type && dataNames.Contains(type.Name.Text)
                ))
            {
                Name(GetOrCreateStructType(p), p.Name);
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
        public Instruction GetOrCreateStructType(StructType s)
        {   
            var t = TypeStruct(true,s.Fields.Select(f => GetOrCreateSPVType(f.Type.Name)).ToArray());
            Name(t,s.Name.Text);
            for(int i =0; i< s.Fields.Count; i++)
            {
                MemberName(t,i,s.Fields[i].Name.Text);
            }
            return t;
        }
        public Instruction GetOrCreateSPVType(string name)
        {
            // TODO : Create type instruction SPV
            var t = SDSLTypeToSPVType(name);
            return t;
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
                        // Function(TypePointer(StorageClass.Generic,vType),FunctionControlMask.MaskNone,
                        // Store(variable,new Instruction(Op.OpConstant));
                    }
                }
                // new Instruction(Op.OpPtrCastToGeneric)

                
            }
        }
    }
}