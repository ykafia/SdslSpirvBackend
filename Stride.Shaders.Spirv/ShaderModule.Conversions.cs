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
                var pvar = (StrideVariable)ds.Content;
                var vType = GetOrCreateSPVType(pvar.Type.Name.Text);
                if(vType is StructElement st)
                {
                    Expression expr = ((StrideVariable)ds.Content).InitialValue;
                    if(expr is CastExpression cexp && Elements.ContainsKey(cexp.Target.Name.Text))
                    {
                        var variable = Variable(st.RawType,StorageClass.Function);
                        Name(variable, pvar.Name.Text);
                        // if(cexp.Target. is Literal l)
                        if(cexp.From is LiteralExpression l && (int)l.Literal.Value == 0)
                        {
                            var stType = Elements[cexp.Target.Name.Text];
                            var chains = new Dictionary<string,AccessChainData>();
                            stType.GetAllAccessChains(ref chains,new List<int>(),pvar.Name.Text);
                            // var streamsType = Elements["VS_STREAMS"];
                            // streamsType.GetAllAccessChains(ref chains, new List<int>(),"myVar");
                            var tptr = TypePointer(StorageClass.Function,stType.RawType);
                            var variablePointer = Variable(tptr,StorageClass.Function);
                            Name(variablePointer,pvar.Name.Text);
                            AddLocalVariable(variablePointer);
                            foreach(var chain in chains.Values)
                            {
                                var typePointer = TypePointer(StorageClass.Function, chain.Element.RawType);
                                var access = AccessChain(typePointer, variablePointer, ConstantOf(chain.Indices.ToArray()));
                                Store(access,ZeroOf(chain.Element.ValueType));
                            }
                            // foreach(var accessChain in Elements[cexp.Target.Name.Text].)
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Something");
                    var rawType = ((FieldElement)vType).RawType;
                    var variable = Variable(rawType,StorageClass.Function);
                    Name(variable, pvar.Name.Text);
                    var content = (StrideVariable)ds.Content;
                    if(content.InitialValue is MethodInvocationExpression mie)
                    {
                        // TODO : manage expressions
                        var args = mie.Arguments;
                    }
                    // Store(variable, ConstantOf(((StrideVariable)ds.Content).InitialValue)
                }
                // new Instruction(Op.OpPtrCastToGeneric)

                
            }
        }
    }
}