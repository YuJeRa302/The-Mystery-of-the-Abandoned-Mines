using Assets.Source.Game.Scripts.Items;
using Assets.Source.Game.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Services
{
    [Serializable]
    public class WeaponService
    {
        [SerializeField] private int _currentWeaponId;
        [SerializeField] private List<WeaponState> _weaponStates = new();

        public int CurrentWeaponId => _currentWeaponId;
        public List<WeaponState> WeaponStates => _weaponStates;

        public void SetWeaponId(int id) => _currentWeaponId = id;

        public void SetWeaponStates(WeaponState[] weaponStates)
        {
            for (int index = 0; index < weaponStates.Length; index++)
            {
                _weaponStates.Add(weaponStates[index]);
            }
        }

        public void UnlockWeaponByData(WeaponData weaponData)
        {
            if (_weaponStates == null)
                return;

            bool found = false;

            for (int i = 0; i < _weaponStates.Count; i++)
            {
                WeaponState weaponState = _weaponStates[i];

                if (weaponState.Id == weaponData.Id)
                {
                    weaponState.UnlockWeapon();
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                WeaponState newWeaponState = InitWeaponState(weaponData);
                _weaponStates.Add(new(newWeaponState.Id, newWeaponState.IsEquip, newWeaponState.IsUnlock == true));
            }
        }

        public WeaponState GetWeaponStateByData(WeaponData weaponData)
        {
            WeaponState weaponState = FindWeaponState(weaponData.Id);

            if (weaponState == null)
                weaponState = InitWeaponState(weaponData);

            return weaponState;
        }

        private WeaponState FindWeaponState(int id)
        {
            if (_weaponStates != null)
            {
                foreach (WeaponState weaponState in _weaponStates)
                {
                    if (weaponState.Id == id)
                        return weaponState;
                }
            }

            return null;
        }

        private WeaponState InitWeaponState(WeaponData weaponData)
        {
            WeaponState weaponState = new(weaponData.Id, false, false);
            _weaponStates.Add(weaponState);
            return weaponState;
        }
    }
}