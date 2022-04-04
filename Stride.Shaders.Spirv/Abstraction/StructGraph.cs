using Spv.Generator;
using Stride.Core.Shaders.Ast;
using static Spv.Specification;

namespace Stride.Shaders.Spirv.Abstraction
{
    public class StructNode
    {
        public string Name;
        public int Depth;
    }
    public class StructGraph
    {
        public Dictionary<string,StructElement> Created = new();
        public Dictionary<string, StructNode> Redirection = new();
        public Dictionary<string,List<StructNode>> AdjacencyChild = new();
        public Dictionary<string,List<StructNode>> AdjacencyParent = new();
        

        public void AddChild(string a, string b)
        {
            if(AdjacencyChild.TryGetValue(a, out var values))
            {
                Redirection[b] = new StructNode{Name = b, Depth = Redirection[a].Depth};
                values.Add(Redirection[b]);
            }
            else
            {
                Redirection.Add(a,new StructNode{Name = a, Depth = 0});
                Redirection.Add(b,new StructNode{Name = b, Depth = 1});
                AdjacencyChild[a] = new List<StructNode>{Redirection[b]};
            }
            AdjacencyChild.Add(b,new List<StructNode>());
            // AddParent(b,a);
        }
        // public void AddParent(string a, string b)
        // {
        //     if(AdjacencyParent.TryGetValue(a, out var values))
        //         values.Add(b);
        //     else
        //         AdjacencyParent[a] = new List<string>{b};
        // }
        public List<string> GetLeaves()
        {
            var result = new List<string>();

            foreach(var k in AdjacencyChild)
            {
                if(k.Value.Count == 0)
                    result.Add(k.Key);
            }

            return result;
        }
        public void Create()
        {
            
        }
        public List<StructNode> CreateChain(string name)
        {
            return null;
        }
    }
}