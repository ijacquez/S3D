#version 330 core

uniform mat4 modelview_matrix;
uniform mat4 projection_matrix;

layout(location = 0) in vec3 in_position;
layout(location = 1) in vec2 in_texcoord;
layout(location = 2) in vec3 in_color;

out vec2 texcoord;
out vec4 color;

void main(void)
{
    gl_Position = (projection_matrix * modelview_matrix) * vec4(in_position, 1.0);

    texcoord = in_texcoord;
    color = vec4(in_color, 1.0);
}
