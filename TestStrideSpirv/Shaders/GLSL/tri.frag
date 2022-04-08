#version 450

struct VS_STREAMS
{
    float ScreenPosition;
};

layout(location = 0) out vec4 outColor;

void main() {
    VS_STREAMS dondo;
    dondo.ScreenPosition = 0;
    outColor = vec4(1.0,1.0,1.0,dondo.ScreenPosition);
}