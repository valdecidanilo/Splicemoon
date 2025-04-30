using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    [SerializeField] private Image expBarImage;
    [SerializeField] private int maxExp = 100;
    [SerializeField] private int currentExp;
    [SerializeField] private float lerpDuration = 0.5f;

    private Coroutine currentLerpCoroutine;

    public void SetExp(int exp)
    {
        currentExp = Mathf.Clamp(exp, 0, maxExp);
        UpdateHealthBar();
    }

    public void SetMaxExp(int expMax) => maxExp = expMax;

    private void UpdateHealthBar()
    {
        if (currentLerpCoroutine != null)
            StopCoroutine(currentLerpCoroutine);
        
        currentLerpCoroutine = StartCoroutine(LerpExpBar());
    }

    private IEnumerator LerpExpBar()
    {
        var elapsedTime = 0f;
        var startFillAmount = expBarImage.fillAmount;
        var targetFillAmount = (float)currentExp / maxExp;

        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            var t = Mathf.Clamp01(elapsedTime / lerpDuration);
            
            expBarImage.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, t);
            
            yield return null;
        }
        expBarImage.fillAmount = targetFillAmount;
        
        currentLerpCoroutine = null;
    }
}
