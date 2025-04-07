using UnityEngine;

[System.Serializable]
public class DamageSupportivePatametr
{
    [SerializeField] private float _value;
    [SerializeField] private TypeSupportivePatametr _supportivePatametr;

    public DamageSupportivePatametr(float value, TypeSupportivePatametr supportivePatametr)
    {
        _value = value;
        _supportivePatametr = supportivePatametr;
    }

    public float Value => _value;
    public TypeSupportivePatametr SupportivePatametr => _supportivePatametr;

    public void ChaneValue(float value)
    {
        _value = value;
    }
}