using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : MonoBehaviour
{
    private void OnEnable()
    {
        Destroy(this.gameObject, 0.15f);
    }
}
