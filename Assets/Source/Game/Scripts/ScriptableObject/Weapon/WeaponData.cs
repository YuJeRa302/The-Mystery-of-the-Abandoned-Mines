using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Create Weapon/Weapon", order = 51)]
public class WeaponData : ScriptableObject
{
    [SerializeField] private TypePlayerClass _targetClass;
    [SerializeField] private WeponPrefab _weaponPrefab;
    [SerializeField] private int _bonusDamage;
    [SerializeField] private int _bonusArmor;

    public TypePlayerClass TargetClass => _targetClass;
    public WeponPrefab WeaponPrefab => _weaponPrefab;
    public int BonusDamage => _bonusDamage;
    public int BonusArmor => _bonusArmor;
}