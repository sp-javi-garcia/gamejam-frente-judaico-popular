using UnityEngine;
using System.Collections;

public class FadeInDelay : MonoBehaviour
{
    public float FadeDuration = 0.5f;
    public float FadeDelay = 0f;
    CanvasGroup _canvasGroup;
	// Use this for initialization
	void Start ()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
	}

	
	// Update is called once per frame
	void Update ()
    {
        FadeDelay -= Time.deltaTime;
        if (FadeDelay <= 0f)
        {
            float progress = Mathf.Min(1f, (- FadeDelay) / FadeDuration);
            _canvasGroup.alpha = progress;
        }
	}
}
