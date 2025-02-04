public class HashAnimation
{
    private string _moveAnimation = "Move";
    private string _attackAnimation = "Attack";
    private string _additionalAttackAnimation = "AdditionalAttack";
    private string _specialAttackAnimation = "SpecialAttack";
    private string _takeDamageAnimation = "TakeDamage";
    private string _winDanceAnimation = "Win";
    private string _idleAnimation = "Idle";

    public string MoveAnimation => _moveAnimation;
    public string AttackAnimation => _attackAnimation;
    public string AdditionalAttackAnimation => _additionalAttackAnimation;
    public string SpecialAttackAnimation => _specialAttackAnimation;
    public string TakeDamageAnimation => _takeDamageAnimation;
    public string WinDanceAnimation => _winDanceAnimation;
    public string IdleAnimation => _idleAnimation;
}