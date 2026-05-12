shader_type canvas_item;

void fragment() {
    float x = UV.x - 0.5;
    float y = 1.0 - UV.y;
    
    if(abs(x) > (0.5 - y * 0.5)) discard;
    
    COLOR = vec4(1.0, 1.0, 1.0, COLOR.a);
}