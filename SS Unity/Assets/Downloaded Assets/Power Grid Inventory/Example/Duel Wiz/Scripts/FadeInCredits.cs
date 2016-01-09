using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeInCredits : MonoBehaviour
{
    public float FadeDelay = 5.0f;
    
    void OnEnable()
    {
        GetComponent<Text>().CrossFadeAlpha(0.0f, 0.0f, true);
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(FadeDelay);

        GetComponent<Text>().CrossFadeAlpha(1.0f, 3.0f, true);
    }

}
