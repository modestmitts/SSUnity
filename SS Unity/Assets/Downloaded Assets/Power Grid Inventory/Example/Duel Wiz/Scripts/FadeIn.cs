using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour 
{
    CanvasRenderer ImageRenderer;
    
	void Start()
    {
        ImageRenderer = GetComponent<CanvasRenderer>();
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(2.0f);

        GetComponent<Image>().CrossFadeAlpha(0, 3.0f, false);
    }

    void Update()
    {
        
        if (ImageRenderer.GetAlpha() <= 0.001f) GameObject.Destroy(this.gameObject);
    }
    
}
