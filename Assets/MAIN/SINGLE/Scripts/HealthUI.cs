using System.Collections;
using System.Collections.Generic;
using Wand;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public float changeValueSpeed;
    private float currentStep;
    private float targetValue;

    public Image healthBar;

    public Image vulnerabilityIce;
    public Image vulnerabilityFire;
    public Image vulnerabilityElectro;
    public Image vulnerabilityKinetic;

    bool inAnimation;

    public void UpdateValue(float value)
    {
        value = Mathf.Clamp(value, 0, 1);
        targetValue = value;
        currentStep = 0;
        inAnimation = true;
    }

    private void Update()
    {
        if (inAnimation)
        {
            currentStep += Time.deltaTime * changeValueSpeed;
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, targetValue, currentStep);
            if (currentStep >= 1)
            {
                currentStep = 0;
                inAnimation = false;
            }

            if (healthBar.fillAmount == 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void VulnerabilityChange(MagicType type)
    {
        vulnerabilityIce.gameObject.SetActive(false);
        vulnerabilityFire.gameObject.SetActive(false);
        vulnerabilityElectro.gameObject.SetActive(false);
        vulnerabilityKinetic.gameObject.SetActive(false);

        switch (type) 
        {
            case MagicType.Fire:
                vulnerabilityFire.gameObject.SetActive(true);
                break;
            case MagicType.Ice:
                vulnerabilityIce.gameObject.SetActive(true);
                break;
            case MagicType.Electro:
                vulnerabilityElectro.gameObject.SetActive(true);
                break;
            case MagicType.Kinetic:
                vulnerabilityKinetic.gameObject.SetActive(true);
                break;
        }

    }
}
