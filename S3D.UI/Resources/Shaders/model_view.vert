#version 330 core

uniform vec2 window_size;
uniform mat4 modelview_matrix;
uniform mat4 projection_matrix;

in vec3 in_position;
in vec2 in_texcoord;
in vec3 in_gscolor;
in vec3 in_basecolor;
in uint in_flags;

out vec4 clipcoord;
out vec2 texcoord;
out vec4 gscolor;
flat out vec4 basecolor;
flat out uint flags;

void main(void)
{
    mat4 mvp = projection_matrix * modelview_matrix;
    vec4 position = mvp * vec4(in_position, 1.0);

    clipcoord = position;
    texcoord = in_texcoord;
    gscolor = vec4(in_gscolor, 1.0);
    basecolor = vec4(in_basecolor, 1.0);
    flags = in_flags;

    gl_Position = position;
}
