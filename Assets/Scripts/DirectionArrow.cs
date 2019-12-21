using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionArrow : MonoBehaviour
{
    private void Start()
    {
        SetSpriteColor();
    }

    void Update()
    {
        SetSpriteColor();
    }

    private void SetSpriteColor()
    {
        var energyCoeff = transform.parent.GetComponent<Minion>().Energy.EnergyCoeff;
        float red = 1 - energyCoeff;
        float green = 1 - energyCoeff;
        float blue = 1 - energyCoeff;
        float alpha = energyCoeff;

        Color spriteColor = new Color(red, green, blue, alpha);
        GetComponent<SpriteRenderer>().color = spriteColor;
    }
}
