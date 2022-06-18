#version 330

uniform sampler2D texture0;

in vec2 texcoord;
in vec4 color;

out vec4 out_color;

void main()
{
    vec4 texel = texture(texture0, texcoord, 0);
    vec4 blended_color = clamp(color + texel, vec4(0.0), vec4(2.0));

    // Map blended colors from 0..63 (0..2), then step down each value by 16.
    // Clamp to (0..31)
    vec4 min_color = vec4(0.0);
    vec4 max_color = vec4((31 * 8.0) / 256.0);
    vec4 step_color = vec4((16 * 8.0) / 256.0);
    vec4 shaded_color = clamp(blended_color - step_color, min_color, max_color);

    out_color = shaded_color;
}
