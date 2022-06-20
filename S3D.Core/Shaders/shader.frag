#version 330

#ifndef M_PI
#define M_PI 3.1415926535897932384626433832795
#endif // !M_PI

#define HAS_FLAGS(x) ((flags & (x)) != 0U)

#define FLAGS_TEXTURED        (1U << 3U)
#define FLAGS_GOURAUD_SHADING (1U << 4U)
#define FLAGS_SELECT          (1U << 31U)

uniform sampler2D texture0;
uniform highp float time;

in vec2 texcoord;
in vec4 gscolor;
// No interpolation. This value will come from a single provoking vertex
flat in vec4 basecolor;
flat in uint flags;

out vec4 out_color;

void gouraud_shading()
{
    vec3 blended_color = clamp(gscolor.rgb + out_color.rgb, vec3(0.0), vec3(2.0));

    // Map blended colors from 0..63 (0..2), then step down each value by 16.
    // Clamp to (0..31)
    vec3 min_color = vec3(0.0);
    vec3 max_color = vec3((31 * 8.0) / 256.0);
    vec3 step_color = vec3((16 * 8.0) / 256.0);

    out_color = vec4(clamp(blended_color - step_color, min_color, max_color), 1.0);
}

void main()
{
    if (HAS_FLAGS(FLAGS_TEXTURED)) {
        out_color = texture(texture0, texcoord, 0);
    } else {
        out_color = basecolor;
    }

    if (HAS_FLAGS(FLAGS_GOURAUD_SHADING)) {
        gouraud_shading();
    }

    if (HAS_FLAGS(FLAGS_SELECT)) {
        float t = mod(time, 1.0);
        float rate = clamp(1.0 - abs(sin((2 * M_PI) * t)), 0.0, 0.333);

        out_color.r += rate;
        out_color.g += rate;
        out_color.b += rate;
    }
}
