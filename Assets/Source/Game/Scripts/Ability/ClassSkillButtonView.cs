using Assets.Source.Game.Scripts;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ClassSkillButtonView : AbilityView
{
    [SerializeField] private Button _button;

    public event Action AbilityUsed;

    private void Awake()
    {
        _button.onClick.AddListener(OnAbilityUse);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnAbilityUse);
    }

    public void SetInerectableButton(bool state)
    {
        _button.interactable = state;
    }

    private void OnAbilityUse()
    {
        AbilityUsed?.Invoke();
    }
}