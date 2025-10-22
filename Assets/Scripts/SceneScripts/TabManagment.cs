using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TabManagment : MonoBehaviour
{
    // Variables
    public TMPro.TMP_Text textMeshPro;
    public string nColour = "<color=green>";
    public string eColour = "</color>";


    // methods
    void OnEnable()
    {
        ChangeTxtColour();
    }
    public void ChangeTxtColour()
    {
        textMeshPro.text = nColour + textMeshPro.text + eColour;
    }
}
