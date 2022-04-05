using Spv.Generator;
using Stride.Core.Shaders.Ast;
using static Spv.Specification;

namespace Stride.Shaders.Spirv
{
    public interface ValueElement{}

    public class FieldElement : ValueElement
    {
        public string Name;
        public string ValueType;
        public Instruction RawType;
        public bool IsComposite;
        public Instruction ZeroValue;
        
    }
    public class StructElement : ValueElement
    {
        public string Name;
        public Instruction RawType;
        public bool IsComposite;
        public Dictionary<string, ValueElement> Fields = new();
        public StructElement(ShaderModule program, StructType s)
        {

            Name = s.Name.Text;
            RawType = program.TypeStruct(
                false,
                // If field is basic or is struct
                s.Fields.Select(f => {
                    if(program.IsStructType(f.Name.Text))
                    {
                        // Add struct type
                        Fields.Add(f.Name.Text, new FieldElement{Name = f.Name.Text, ValueType = f.Type.Name.Text,RawType = program.Elements[f.Name.Text].RawType});
                        return program.Elements[f.Name.Text].RawType;
                    }
                    else
                    {
                        var tmp = program.GetOrCreateSPVType(f.Type.Name.Text);
                        tmp.Name = f.Name.Text;
                        Fields.Add(f.Name.Text, tmp);
                        return tmp.RawType;
                    }
                }).ToArray()
            );
            
            // Name struct and members
            program.Name(RawType, Name);
            for(int i = 0; i< s.Fields.Count; i++)
            {
                program.MemberName(this.RawType, i, s.Fields[i].Name.Text);
            }
        }        
    }
}