using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndingSceneManager : MonoBehaviour
{
    public RawImage fadeImage;
    public float fadeDuration = 2f;

    private Color startColor;
    private Color endColor = Color.clear;
    private float elapsedTime = 0f;

    private void Start()
    {
        startColor = fadeImage.color;
        fadeImage.color = startColor;
        StartCoroutine(StartFadeIn());
    }

    private IEnumerator StartFadeIn()
    {
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            float alpha = Mathf.Lerp(startColor.a, endColor.a, t);

            fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        // 엔딩 씬 시작 후 추가적인 처리를 할 수 있습니다.
    }
}
