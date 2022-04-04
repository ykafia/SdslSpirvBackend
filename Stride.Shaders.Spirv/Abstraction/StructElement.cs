using Spv.Generator;
using Stride.Core.Shaders.Ast;
using static Spv.Specification;

namespace Stride.Shaders.Spirv.Abstraction
{
    public class ValueElement
    {
        public string Name;
        public string Type;
        public Instruction RawType;
        public bool IsComposite;
        public Instruction Zero;
        
    }
    public class StructElement
    {
        public string Name;
        public string Type;
        public Instruction RawType;
        public bool IsComposite;
        public List<ValueElement> Fields;

        public StructElement(){}
        public StructElement(ShaderModule program, StructType s)
        {
            // var t = program.TypeStruct(true,s.Fields.Select(f => program.GetOrCreateSPVType(f.Type.Name)).ToArray());
            // Name(t,s.Name.Text);
            // for(int i =0; i< s.Fields.Count; i++)
            // {
            //     MemberName(t,i,s.Fields[i].Name.Text);
            // }
            // return t;
        }

        public void Construct()
        {
            // OpName %MyStruct "MyStruct"
            // OpMemberName %MyStruct 0 "toto"
            // OpMemberName %MyStruct 1 "dodo"
            // %MyStruct = OpTypeStruct %v3float %v2float
            
        }
        public List<Instruction> Instantiate(Module program, params Instruction[] values)
        {
            // %_ptr_Function_MyStruct = OpTypePointer Function %MyStruct
            // %dondo = OpVariable %_ptr_Function_MyStruct Function
            // %17 = OpAccessChain %_ptr_Function_v3float %dondo %int_0
            //     OpStore %17 %15
            // %22 = OpAccessChain %_ptr_Function_v3float %dondo %int_0
            var ptr = program.TypePointer(StorageClass.Function, RawType);
            var dondo = program.Variable(ptr, StorageClass.Function);
            // var _17 = program.AccessChain(Fields.First(x => x.Name == ))
            return null;
        }
        public Instruction Instantiate(Module program, string name)
        {
            // %_ptr_Function_MyStruct = OpTypePointer Function %MyStruct
            // %dondo = OpVariable %_ptr_Function_MyStruct Function
            // %17 = OpAccessChain %_ptr_Function_v3float %dondo %int_0
            //     OpStore %17 %15
            // %22 = OpAccessChain %_ptr_Function_v3float %dondo %int_0
            var ptr = program.TypePointer(StorageClass.Function, RawType);
            program.Variable(ptr, StorageClass.Function);
            
            // var _17 = program.AccessChain()
            return null;
        } 
        
    }
}