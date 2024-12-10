using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<WeaponData> _paladinWeapons;
    [SerializeField] private List<WeaponData> _barbarianWeapons;
    [SerializeField] private List<WeaponData> _warlockWeapons;

    public List<WeaponData> WarlockWeapons => _warlockWeapons;
    public List<WeaponData> BarbarianWeapons => _barbarianWeapons;
    public List<WeaponData> PaladinWeapons => _paladinWeapons;
}