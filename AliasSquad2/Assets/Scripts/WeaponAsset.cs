using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Weapon")]
public class WeaponAsset : ScriptableObject {

    public AnimationCurve x,y;

    public int damage;

    public int magazine, ammo;

    public float delayTime;

    public AudioClip soundEffect;

}
