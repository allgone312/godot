shader_type canvas_item;

uniform float lerp_persent : hint_range(0.0, 1.0);

void fragment() {
	vec4 texture_color = texture(TEXTURE,UV);
	vec4 final_color = mix(texture_color,vec4(1.0,1.0,1.0,texture_color.a),lerp_persent);
	COLOR = final_color;
}
//vec4 表示RGBA。texture_color取的是像素图像（也就是这个人物），不取会对整个方框内都进行渲染
//vec4(1.0)表示全白色，mix(渲染对象,目标渲染效果,混合百分比)
//COLOR这类属性要全大写

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
