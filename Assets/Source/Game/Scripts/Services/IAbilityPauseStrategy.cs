namespace Assets.Source.Game.Scripts.Services
{
    public interface IAbilityPauseStrategy
    {
        public void PausedGame(bool state);
        public void ResumedGame(bool state);
    }
}