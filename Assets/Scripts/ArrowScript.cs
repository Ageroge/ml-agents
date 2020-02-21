using System;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    public event Action<ArrowScript> OnArrowHit = delegate { };

    bool HasHit = false;

    void OnCollisionEnter(Collision collided)
    {
        if (HasHit)
        {
            Debug.LogWarning($"Arrow hit multiple times");
            return;
        }

        var swordsman = collided.gameObject.GetComponent<SwordsmanScript>();
        if (swordsman)
        {
            HasHit = true;

            swordsman.OnHitByArrow();
            OnArrowHit(this);

            Destroy(gameObject);
        }
    }
}
