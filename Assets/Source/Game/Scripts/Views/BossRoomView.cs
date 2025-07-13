using UnityEngine;

namespace Assets.Source.Game.Scripts.Views
{
    public class BossRoomView : RoomView
    {
        [SerializeField] private Transform _bossSpawnPoint;

        public void UnlockBossRoom(bool state)
        {
            if (state == false)
                return;

            base.UnlockRoom();
        }
    }
}