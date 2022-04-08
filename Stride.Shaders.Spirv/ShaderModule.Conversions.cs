using Spv;
using Spv.Generator;
using static Spv.Specification;
using Stride.Core.Shaders.Ast;
using Stride.Core.Shaders.Ast.Stride;
using StrideVariable = Stride.Core.Shaders.Ast.Variable;
using Stride.Core.Shaders.Ast.Hlsl;

namespace Stride.Shaders.Spirv
{
    public partial class ShaderModule
    {
        private void ConvertStatement(Statement s, ref Dictionary<string,Instruction> variables)
        {
            if(s is DeclarationStatement ds)
            {
                var pvar = ((Variable)ds.Content);
                var vType = GetOrCreateSPVType(pvar.Type.Name.Text);
                if(vType is StructElement st)
                {
                    Expression expr = ((Variable)ds.Content).InitialValue;
                    if(expr is CastExpression cexp && Elements.ContainsKey(cexp.Target.Name.Text))
                    {
                        var variable = Variable(st.RawType,StorageClass.Generic);
                        Name(variable, pvar.Name.Text);
                        // if(cexp.Target. is Literal l)
                        if(cexp.From is LiteralExpression l && (int)l.Literal.Value == 0)
                        {
                            variables.Add(pvar.Name.Text,variable);
                            AddLocalVariable(variable);
                            var chains = new Dictionary<string,AccessChainData>();
                            Elements[cexp.Target.Name.Text].GetAllAccessChains(ref chains,new List<int>(),pvar.Name.Text);
                            foreach(var chain in chains.Values)
                            {
                                var typePointer = TypePointer(StorageClass.Function, chain.Element.RawType);
                                var access = AccessChain(typePointer, st.RawType, chain.Indices.Select(x => Constant(TypeInt(32,1),x)).ToArray());
                                Store(access,chain.Element.ZeroValue);
                            }
                            // foreach(var accessChain in Elements[cexp.Target.Name.Text].)
                        }
                    }
                }
                // new Instruction(Op.OpPtrCastToGeneric)

                
            }
        }
    }
}