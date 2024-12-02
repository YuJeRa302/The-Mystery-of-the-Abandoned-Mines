using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "New Class", menuName = "Create Player Class", order = 51)]
public class PlayerClassData : ScriptableObject
{
    [SerializeField] private TypePlayerClass _className;
    [SerializeField] private AnimatorController _animatorController;

    public AnimatorController AnimatorController => _animatorController;
    public TypePlayerClass ClassName => _className;
}