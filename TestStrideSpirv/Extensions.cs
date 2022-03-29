namespace TestStrideSpirv
{
    public static class Extension
    {
        public static void ToHlsl(this byte[] code)
        {
            ShaderConverter.ToHlsl(code);
        }
        public static void ToGlsl(this byte[] code)
        {
            ShaderConverter.ToGlsl(code);
        }
    }
}