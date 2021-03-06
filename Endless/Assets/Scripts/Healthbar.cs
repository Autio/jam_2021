using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{

    [SerializeField]
    private Image foregroundImage;
    [SerializeField] 
    private float updateSpeedSeconds = 0.5f;

    private void Start()
    {
        // Add delegate - but this needs to be defined in whatever contains the enemy health
        GetComponentInParent<TempHealth>().OnHealthPctChanged += HandleHealthChanged;
    }

    private void HandleHealthChanged(float pct)
    {
        StartCoroutine(ChangeToPct(pct));
    }

    private IEnumerator ChangeToPct(float pct){
        float preChangePct = foregroundImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(preChangePct, pct, elapsed / updateSpeedSeconds);
            yield return null;
        }

        foregroundImage.fillAmount = pct;
    }

    // Toggle the canvas
    // Don't show full health bars
    void ToggleHealthbarCanvas()
    {
        GetComponent<Canvas>().enabled = !GetComponent<Canvas>().enabled;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
