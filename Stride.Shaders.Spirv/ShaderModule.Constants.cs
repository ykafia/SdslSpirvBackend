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
        private Instruction ConstantOf(string type, params float[] values)
        {
            return type switch
            {
                var t when t.StartsWith("byte") => ConstantOf(TypeInt(8,0),values),
                var t when t.StartsWith("sbyte") => ConstantOf(TypeInt(32,1),values),
                
                var t when t.StartsWith("ushort") => ConstantOf(TypeInt(16,0),values),
                var t when t.StartsWith("short") => ConstantOf(TypeInt(16,1),values),

                var t when t.StartsWith("uint") => ConstantOf(TypeInt(32,1),values),
                var t when t.StartsWith("int") => ConstantOf(TypeInt(32,1),values),

                var t when t.StartsWith("ulong") => ConstantOf(TypeInt(32,1),values),
                var t when t.StartsWith("long") => ConstantOf(TypeInt(32,1),values),
                
                var t when t.StartsWith("half") => ConstantOf(TypeFloat(16),values),
                var t when t.StartsWith("float") => ConstantOf(TypeFloat(32), values),
                var t when t.StartsWith("double") => ConstantOf(TypeFloat(32), values),

                
                _ => throw new NotImplementedException()
            };
        }
        private Instruction ConstantOf(Instruction type, params float[] values)
        {
            if(values.Length == 1)
            {
                return Constant(type,values[0]);
            }
            else
                return ConstantComposite(
                    TypeVector(type,values.Length),
                    values.Select(x => ConstantOf(type,x)).ToArray()
                );
        }
        private Instruction ConstantOf(params byte[] values)
        {
            Instruction result;
            
            if(values.Length == 1)
            {
                result = Constant(TypeInt(8,0),(int)values[0]);
            }
            else 
            {
                result = ConstantComposite(TypeVector(TypeInt(8,0),values.Length), values.Select(x => ConstantOf(x)).ToArray());
            }
            return result;
        }
        private Instruction ConstantOf(params ushort[] values)
        {
            Instruction result;
            
            if(values.Length == 1)
            {
                result = Constant(TypeInt(16,0),(int)values[0]);
            }
            else 
            {
                result = ConstantComposite(TypeVector(TypeInt(16,0),values.Length), values.Select(x => ConstantOf(x)).ToArray());
            }
            return result;
        }
        private Instruction ConstantOf(params uint[] values)
        {
            Instruction result;
            
            if(values.Length == 1)
            {
                result = Constant(TypeInt(32,0),values[0]);
            }
            else 
            {
                result = ConstantComposite(TypeVector(TypeInt(32,0),values.Length), values.Select(x => ConstantOf(x)).ToArray());
            }
            return result;
        }
        private Instruction ConstantOf(params ulong[] values)
        {
            Instruction result;
            
            if(values.Length == 1)
            {
                result = Constant(TypeInt(64,0),values[0]);
            }
            else 
            {
                result = ConstantComposite(TypeVector(TypeInt(64,0),values.Length), values.Select(x => ConstantOf(x)).ToArray());
            }
            return result;
        }
        private Instruction ConstantOf(params sbyte[] values)
        {
            Instruction result;
            
            if(values.Length == 1)
            {
                result = Constant(TypeInt(8,1),values[0]);
            }
            else 
            {
                result = ConstantComposite(TypeVector(TypeInt(8,1),values.Length), values.Select(x => ConstantOf(x)).ToArray());
            }
            return result;
        }
        private Instruction ConstantOf(params short[] values)
        {
            Instruction result;
            
            if(values.Length == 1)
            {
                result = Constant(TypeInt(16,1),values[0]);
            }
            else 
            {
                result = ConstantComposite(TypeVector(TypeInt(16,1),values.Length), values.Select(x => ConstantOf(x)).ToArray());
            }
            return result;
        }
        private Instruction ConstantOf(params int[] values)
        {
            Instruction result;
            
            if(values.Length == 1)
            {
                result = Constant(TypeInt(32,1),values[0]);
            }
            else 
            {
                result = ConstantComposite(TypeVector(TypeInt(32,1),values.Length), values.Select(x => ConstantOf(x)).ToArray());
            }
            return result;
        }
        private Instruction ConstantOf(params long[] values)
        {
            Instruction result;
            
            if(values.Length == 1)
            {
                result = Constant(TypeInt(64,1),values[0]);
            }
            else 
            {
                result = ConstantComposite(TypeVector(TypeInt(64,1),values.Length), values.Select(x => ConstantOf(x)).ToArray());
            }
            return result;
        }
        private Instruction ConstantOf(params Half[] values)
        {
            Instruction result;
            
            if(values.Length == 1)
            {
                result = Constant(TypeFloat(16),(float)values[0]);
            }
            else 
            {
                result = ConstantComposite(TypeVector(TypeFloat(32),values.Length), values.Select(x => ConstantOf(x)).ToArray());
            }
            return result;
        }
        private Instruction ConstantOf(params float[] values)
        {
            Instruction result;
            
            if(values.Length == 1)
            {
                result = Constant(TypeFloat(32),values[0]);
            }
            else 
            {
                result = ConstantComposite(TypeVector(TypeFloat(32),values.Length), values.Select(x => ConstantOf(x)).ToArray());
            }
            return result;
        }
    }
}