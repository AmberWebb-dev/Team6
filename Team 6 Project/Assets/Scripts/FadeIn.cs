using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    [SerializeField] private float timeUntilFadeIn;
    [SerializeField] private float fadeInLength;
    [SerializeField] private TMPro.TMP_Text textRenderer;
    [SerializeField] private Image imageRenderer;
    [SerializeField] private MeshRenderer meshRenderer;

    bool fadingIn = false;

    private void Start()
    {
        if (textRenderer != null) { textRenderer.alpha = 0.0f; }
        if (imageRenderer != null) { ChangeImageAlpha(0.0f); }

        StartCoroutine(TimeUntilFadeIn());
    }

    private void Update()
    {
        if (fadingIn)
        {
            if (textRenderer != null) { textRenderer.alpha += (1 / fadeInLength) * Time.deltaTime; }
            if (imageRenderer != null) { ChangeImageAlpha(GetImageAlpha() + (1 / fadeInLength) * Time.deltaTime); }
        }
    }

    private IEnumerator TimeUntilFadeIn()
    {
        yield return new WaitForSeconds(timeUntilFadeIn);
        fadingIn = true;
    }

    private void ChangeImageAlpha(float alpha)
    {
        Color oldColor = imageRenderer.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
        imageRenderer.color = newColor;
    }

    private float GetImageAlpha()
    {
        return imageRenderer.color.a;
    }
}
