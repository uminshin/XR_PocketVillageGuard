using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShield : MonoBehaviour
{
    private float duration;
    private PlayerWeaponController owner;

    public void Init(float duration, PlayerWeaponController owner)
    {
        this.duration = duration;
        this.owner = owner;

        Destroy(gameObject, duration);
    }

    private void OnDestroy()
    {
        if (owner != null)
        {
            owner.EndDefense();
        }
    }
}
