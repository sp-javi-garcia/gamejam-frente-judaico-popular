var scrollSpeed :  float = .5;
var offset : float;

function Update (){
	offset += (Time.deltaTime*scrollSpeed)/10.0;
	renderer.material.SetTextureOffset ("_MainTex", Vector2(-offset, 0));
	}