using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Slider slider;

    public void setMaxHealth(int mHealth)
    {
        slider.maxValue = mHealth;
        slider.value = mHealth;
    }

    public void setHealth(int health)
    {
        slider.value = Mathf.Max(0, health);
    }

    public void Hide()
    {
        slider.gameObject.SetActive(false);
    }
}
