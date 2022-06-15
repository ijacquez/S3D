#version 330 core

uniform mat4 modelview_matrix;
uniform mat4 projection_matrix;

in vec3 in_position;

void main(void)
{
    mat4 mvp = projection_matrix * modelview_matrix;

    gl_Position = mvp * vec4(in_position, 1.0);
}
