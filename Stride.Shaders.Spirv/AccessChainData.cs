namespace Stride.Shaders.Spirv
{
    public struct AccessChainData
    {
        public IEnumerable<int> Indices;
        public string StructPath;
        public FieldElement Element;
    }
}