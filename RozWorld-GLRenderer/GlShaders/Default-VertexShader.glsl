#version 330 core

layout(location = 0) in vec2 VertexPosition;
layout(location = 1) in vec2 VertexUV;

out vec2 UV;

uniform vec2 CanvasResolution;
uniform vec2 UvMapResolution;


void main()
{
    gl_Position.xy = vec2(VertexPosition.x / CanvasResolution.x, VertexPosition.y / CanvasResolution.y);
    gl_Position.z = 1.0;
    gl_Position.w = 1.0;
    
    UV = vec2(VertexUV.x / UvMapResolution.x, VertexUV.y / UvMapResolution.y);
}