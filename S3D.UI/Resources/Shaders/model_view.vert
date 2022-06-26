#version 330 core

uniform mat4 modelview_matrix;
uniform mat4 projection_matrix;

in vec3 in_position;
in vec2 in_texcoord;
in vec3 in_gscolor;
in vec3 in_basecolor;
in uint in_flags;

out vec2 texcoord;
out vec4 gscolor;
flat out vec4 basecolor;
flat out uint flags;

void main(void)
{
    gl_Position = (projection_matrix * modelview_matrix) * vec4(in_position, 1.0);

    texcoord = in_texcoord;
    gscolor = vec4(in_gscolor, 1.0);
    basecolor = vec4(in_basecolor, 1.0);
    flags = in_flags;
}
