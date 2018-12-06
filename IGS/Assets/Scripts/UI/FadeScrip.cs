using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScrip : MonoBehaviour {
    
    public Color startColor, endColor;
    [SerializeField]
    float fadeSpeed;
    Renderer myRenderer;
	bool firsttime = false;

    // Use this for initialization
    void Start () {
        myRenderer = GetComponent<Renderer>();

        //FadeInCall();
	}
	
	// Update is called once per frame
	void Update () {
		if (firsttime == false)
		{
			firsttime = true;
			FadeInCall();
		}
	}

    [ContextMenu("FadeIn")]
    public void FadeInCall()
    {
        StopCoroutine(FadeIn());
        StartCoroutine(FadeIn());
    }

    [ContextMenu("FadeOut")]
    public void FadeOutCall()
    {
        StopCoroutine(FadeOut());
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        float timer = 0;
        while(timer < 1.2f)
        {
            timer += Time.deltaTime * fadeSpeed;
            myRenderer.material.SetColor("_TintColor", Color.Lerp(startColor, endColor, timer));
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
    }

    IEnumerator FadeOut()
    {
        float timer = 0;
        while (timer < 1.2f)
        {
            timer += Time.deltaTime * fadeSpeed;
            myRenderer.material.SetColor("_TintColor", Color.Lerp(endColor, startColor, timer));
            yield return new WaitForEndOfFrame();
        }
        timer = 0;
        while (timer < 1.2f)
        {
            timer += Time.deltaTime * fadeSpeed;
            myRenderer.material.SetColor("_TintColor", Color.Lerp(startColor, endColor, timer));
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
    }
}