using System;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    public event Action Attacked;
    public event Action AdditionalAttacked;

    public void TryAttackPlayer()
    {
        Attacked?.Invoke();
    }

    public void TryAdditionAtacked()
    {
        AdditionalAttacked?.Invoke();
    }
}