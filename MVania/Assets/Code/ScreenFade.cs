using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] Image _image;
   

    public void FadeOut(float fadeTime, float waitTostart = 0.0f)
    {
        StartCoroutine(FadeOutCo(fadeTime,waitTostart));
    }

    public void FadeIn(float fadeTime, float waitTostart = 0.0f)
    {
        StartCoroutine(FadeInCo(fadeTime,waitTostart));
        
    }

    IEnumerator FadeOutCo(float time, float waitTostart)
    {

        yield return new WaitForSeconds(waitTostart);

        float fraction = 0;
        while (fraction < 1)
        {
            fraction += Time.deltaTime / time;
            _image.color = new Color(0, 0, 0, fraction);
            yield return null;
        }

    }

    IEnumerator FadeInCo(float time, float waitTostart)
    {

        yield return new WaitForSeconds(waitTostart);

        float fraction = 0;
        while (fraction < 1)
        {
            fraction += Time.deltaTime / time;
            _image.color = new Color(0, 0, 0, 1 - fraction);
            yield return null;
        }

    }

}
