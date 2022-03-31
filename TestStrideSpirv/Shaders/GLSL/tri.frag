#version 450

struct MyStruct
{
    vec3 toto;
    vec2 dodo;
};

layout(location = 0) out vec4 outColor;

void main() {
    MyStruct dondo;
    dondo.toto = vec3(3,3,3);
    outColor = vec4(1.0, dondo.toto);
}