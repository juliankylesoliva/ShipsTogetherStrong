using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllySoundList", menuName = "ScriptableObjects/AllySoundList", order = 2)]
public class AllySoundList : ScriptableObject
{
    public AudioClip[] soundEffects;
}
