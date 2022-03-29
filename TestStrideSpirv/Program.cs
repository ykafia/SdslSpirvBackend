// See https://aka.ms/new-console-template for more information
using Stride.Shaders.Spirv;
using Stride.Core.Shaders;
using Stride.Core.Shaders.Parser;
using Stride.Core.Shaders.Grammar.Stride;
using SPIRVCross;
using static SPIRVCross.SPIRV;

Console.WriteLine("Hello, World!");

var grammar = ShaderParser.GetGrammar<StrideGrammar>();

var parser = ShaderParser.GetParser<StrideGrammar>();

// var shaderText = File.ReadAllText("./Test.sdsl");

// var shader = parser.Parse(shaderText, "./Test.sdsl").Shader;
// Console.WriteLine(shader);

var vertPath = @"C:\Users\youness_kafia\Documents\GitHub\SdslSpirvBackend\TestStrideSpirv\Shaders\GLSL\tri.vert.spv";

var bytecode = System.IO.File.ReadAllBytes(vertPath);

unsafe
{
    string GetString(byte* ptr)
    {
        int length = 0;
        while (length < 4096 && ptr[length] != 0)
            length++;
        // Decode UTF-8 bytes to string.
        return System.Text.Encoding.UTF8.GetString(ptr, length);
    }
    SpvId* spirv = null;
    fixed (byte* ptr = bytecode)
        spirv = (SpvId*)ptr;
    
    uint word_count = (uint)bytecode.Length / 4;

    spvc_context context = default;
    spvc_parsed_ir ir;
    spvc_compiler compiler_glsl;
    spvc_compiler_options options;
    spvc_resources resources;
    spvc_reflected_resource* list = default;
    nuint count = default;
    spvc_error_callback error_callback = default;

    // Create context.
    spvc_context_create(&context);

    // Set debug callback.
    spvc_context_set_error_callback(context, error_callback, null);

    // Parse the SPIR-V.
    spvc_context_parse_spirv(context, spirv, word_count, &ir);

    // Hand it off to a compiler instance and give it ownership of the IR.
    spvc_context_create_compiler(context, spvc_backend.Glsl, ir, spvc_capture_mode.TakeOwnership, &compiler_glsl);

    // Do some basic reflection.
    spvc_compiler_create_shader_resources(compiler_glsl, &resources);
    spvc_resources_get_resource_list_for_type(resources, spvc_resource_type.UniformBuffer, (spvc_reflected_resource*)&list, &count);

    for (uint i = 0; i < count; i++)
    {
        Console.WriteLine("ID: {0}, BaseTypeID: {1}, TypeID: {2}, Name: {3}", list[i].id, list[i].base_type_id, list[i].type_id, GetString(list[i].name));

        uint set = spvc_compiler_get_decoration(compiler_glsl, (SpvId)list[i].id, SpvDecoration.SpvDecorationDescriptorSet);
        Console.WriteLine($"Set: {set}");

        uint binding = spvc_compiler_get_decoration(compiler_glsl, (SpvId)list[i].id, SpvDecoration.SpvDecorationBinding);
        Console.WriteLine($"Binding: {binding}");

        Console.WriteLine("=========");
    }
    Console.WriteLine("\n \n");

    // Modify options.
    spvc_compiler_create_compiler_options(compiler_glsl, &options);
    spvc_compiler_options_set_uint(options, spvc_compiler_option.GlslVersion, 450);
    spvc_compiler_options_set_bool(options, spvc_compiler_option.GlslEs, false);
    spvc_compiler_install_compiler_options(compiler_glsl, options);

    byte* result = default;
    spvc_compiler_compile(compiler_glsl, (byte*)&result);
    Console.WriteLine("Cross-compiled source: {0}", GetString(result));

    // Frees all memory we allocated so far.
    spvc_context_destroy(context);
}

