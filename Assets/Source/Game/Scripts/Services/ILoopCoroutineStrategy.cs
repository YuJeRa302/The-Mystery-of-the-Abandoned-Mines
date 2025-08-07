namespace Assets.Source.Game.Scripts.Services
{
    public interface ILoopCoroutineStrategy
    {
        public void StartCoroutine();
        public void StopCoroutine();
    }
}