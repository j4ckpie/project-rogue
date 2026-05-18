shader_type canvas_item;

uniform bool flip = false;
uniform vec4 triangle_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);

void fragment() {
    float x = UV.x - 0.5;
    float y = flip ? UV.y : 1.0 - UV.y;
    
    if(abs(x) > (0.5 - y * 0.5)) discard;
    
    COLOR = vec4(triangle_color.rgb, COLOR.a);
}