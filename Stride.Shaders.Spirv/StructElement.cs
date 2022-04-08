using Spv.Generator;
using Stride.Core.Shaders.Ast;
using static Spv.Specification;

namespace Stride.Shaders.Spirv
{
    public interface IValueElement{}

    public class FieldElement : IValueElement
    {
        public string Name;
        public string ValueType;
        public int Index;
        public Instruction RawType;
        public bool IsComposite;
        public Instruction ZeroValue;
    }
    public class StructElement : IValueElement
    {
        public string Name;
        public string ValueType;
        public int Index;
        public Instruction RawType;
        public bool IsComposite;
        public Dictionary<string, IValueElement> Fields = new();
        public StructElement(){}
        public StructElement(ShaderModule program, StructType s)
        {
            ValueType = s.Name.Text;
            var fields = 
                s.Fields.Select((f,i) => {
                    if(program.IsStructType(f.Type.Name.Text))
                    {
                        // Add struct type
                        if(!program.Elements.ContainsKey(f.Type.Name.Text))
                        {
                            program.Elements[f.Type.Name.Text] = new StructElement(program, program.GetStructType(f.Type.Name.Text));   
                        }
                        Fields.Add(f.Name.Text, program.Elements[f.Type.Name.Text]);
                        return program.Elements[f.Type.Name.Text].RawType;
                        
                    }
                    else
                    {
                        FieldElement tmp = program.GetOrCreateSPVType(f.Type.Name.Text) as FieldElement;
                        tmp.Name = f.Name.Text;
                        tmp.Index = i;
                        Fields.Add(f.Name.Text, tmp);
                        return tmp.RawType;
                    }
                }).ToArray();
            RawType = program.TypeStruct(
                true,
                fields
            );
            
            // Name struct and members
            program.Name(RawType, ValueType);
            for(int i = 0; i< s.Fields.Count; i++)
            {
                program.MemberName(this.RawType, i, s.Fields[i].Name.Text);
            }
            if(s.Name.Text.Contains("S_INPUT"))
                program.InputType = this;
            else if(s.Name.Text.Contains("S_OUTPUT"))
                program.OutputType = this;
        }
        public void GetAllAccessChains(ref Dictionary<string,AccessChainData> accessChains, IEnumerable<int> indices, string parentName = "")
        {
            var name = parentName + ".";
            foreach(var f in Fields)
            {
                if(f.Value is FieldElement fe)
                {
                    var a = new AccessChainData
                    {
                        Indices = indices.Append(fe.Index),
                        StructPath = name + fe.Name,
                        Element = fe
                    };
                    accessChains.Add(a.StructPath ?? "", a);
                }
                else if(f.Value is StructElement se)
                {
                    se.GetAllAccessChains(ref accessChains, indices.Append(se.Index), name + f.Key);
                }
            }
        }
    }
}