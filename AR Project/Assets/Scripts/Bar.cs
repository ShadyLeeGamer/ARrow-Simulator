using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetMaxValue(float maxValue)
    {
        SetValue(slider.maxValue = maxValue);
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }
}