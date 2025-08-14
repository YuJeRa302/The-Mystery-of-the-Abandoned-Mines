public struct M_MoveSpeedReduced
{
    private float _valueSlowed;

    public M_MoveSpeedReduced(float valueSlowed)
    {
        _valueSlowed = valueSlowed;
    }

    public float ValueSlowed => _valueSlowed;
}