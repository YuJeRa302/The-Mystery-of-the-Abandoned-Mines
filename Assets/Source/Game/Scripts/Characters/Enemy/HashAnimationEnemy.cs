public class HashAnimationEnemy
{
    private string _moveAnimation = "Move";
    private string _attackAnimation = "Attack";
    private string _additionalAttackAnimation = "AdditionalAttack";
    private string _takeDamageAnimation = "TakeDamage";
    private string _winDanceAnimation = "Win";

    public string MoveAnimation => _moveAnimation;
    public string AttackAnimation => _attackAnimation;
    public string AdditionalAttackAnimation => _additionalAttackAnimation;
    public string TakeDamageAnimation => _takeDamageAnimation;
    public string WinDanceAnimation => _winDanceAnimation;
}