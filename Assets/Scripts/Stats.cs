using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/New Stats")]
public class Stats : SerializedScriptableObject
{
    public int Speed = 3;
    public int Health = 3;
    public int Stamina = 3;
    public int Defense = 3;

    public int Might = 3;
    public int Knowledge = 3;
    public int Willpower = 3;
    public int Awareness = 3;
}
