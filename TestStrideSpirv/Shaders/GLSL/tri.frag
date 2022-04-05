#version 450

struct MyStruct
{
    vec3 toto;
    vec2 dodo;
};
struct MyStruct2
{
    MyStruct ref1;
};

layout(location = 0) out vec4 outColor;

void main() {
    MyStruct2 dondo;
    outColor = vec4(1.0,dondo.ref1.toto);
}