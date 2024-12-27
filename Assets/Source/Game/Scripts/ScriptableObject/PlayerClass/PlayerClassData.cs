using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "New Class", menuName = "Create Player Class", order = 51)]
public class PlayerClassData : ScriptableObject
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _translationName;
    [SerializeField] private string _translationDescription;
    [SerializeField] private TypePlayerClass _className;
    [SerializeField] private AnimatorController _animatorController;

    public string TranslationDescription => _translationDescription;
    public string TranslationName => _translationName;
    public Sprite Icon => _icon;
    public AnimatorController AnimatorController => _animatorController;
    public TypePlayerClass TypePlayerClass => _className;
}