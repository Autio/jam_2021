using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Staminabar : MonoBehaviour
{
    [SerializeField]
    private Image foregroundImage;
    [SerializeField] 
    private float updateSpeedSeconds = 0.5f;

    private void Start()
    {
        // Add delegate - but this needs to be defined in whatever contains the enemy health
        GetComponentInParent<CharacterBase>().OnStaminaPctChanged += HandleStaminaChanged;
    }

    private void HandleStaminaChanged(float pct)
    {
        Debug.Log("Stamina: " + pct);
        StartCoroutine(ChangeToPct(pct));
    }

    private IEnumerator ChangeToPct(float pct){
        float preChangePct = foregroundImage.fillAmount;
        // Make the health bar visible if it was previously full 
        if(preChangePct == 1f)
        {
            ToggleStaminabarCanvas();
        }

        float elapsed = 0f;

        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(preChangePct, pct, elapsed / updateSpeedSeconds);

            yield return null;
        }

        // If the health goes back to 1, then hide it
        if(elapsed > updateSpeedSeconds && pct >= 1)
        {
            ToggleStaminabarCanvas();
        }
        foregroundImage.fillAmount = pct;
    }

    // Toggle the canvas
    // Don't show full health bars
    void ToggleStaminabarCanvas()
    {
        GetComponent<Canvas>().enabled = !GetComponent<Canvas>().enabled;
    }

    // Update is called once per frame
    void Update()
    {
        // Look at the camera
        transform.LookAt(transform.position - Camera.main.transform.position);

    }
}
