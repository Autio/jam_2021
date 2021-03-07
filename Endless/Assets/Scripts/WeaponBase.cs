using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour, IWeaponController
{
    public virtual void TrySwing()
    {
        throw new System.NotImplementedException();
    }
}
