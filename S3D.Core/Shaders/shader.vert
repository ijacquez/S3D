#version 330 core

uniform mat4 modelview_matrix;
uniform mat4 projection_matrix;

in vec3 in_position;
in vec2 in_texcoord;

out vec2 texcoord;

void main(void)
{
    texcoord = in_texcoord;

    gl_Position = (projection_matrix * modelview_matrix) * vec4(in_position, 1.0);
}
