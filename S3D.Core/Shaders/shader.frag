#version 330

uniform sampler2D texture0;

in vec2 texcoord;
in vec4 color;

out vec4 out_color;

void main()
{
    vec4 color2 = color;

    out_color = color2 * texture(texture0, texcoord);
}
