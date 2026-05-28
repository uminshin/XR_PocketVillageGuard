using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    private int defensePower;
    private float duration;
    private float radius;

    private PlayerWeaponController owner;

    public void Init(int defensePower, float duration, float radius, PlayerWeaponController owner)
    {
        this.defensePower = defensePower;
        this.duration = duration;
        this.radius = radius;
        this.owner = owner;

        transform.localScale = Vector3.one * radius;

        Destroy(gameObject, duration);
    }

    private void OnDestroy()
    {
        if (owner != null)
        {
            owner.EndDefense();
        }
    }

    public int GetDefensePower()
    {
        return defensePower;
    }
}
