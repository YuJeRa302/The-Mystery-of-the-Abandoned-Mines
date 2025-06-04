using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class BossRoomView : RoomView
    {
        [SerializeField] private Transform _bossSpawnPoint;

        private void OnEnable()
        {
            LockRoom();
            //base.UnlockRoom();
            //SetRoomStatus();
        }

        public void UnlockBossRoom(bool state)
        {
            if (state == false)
                return;

            base.UnlockRoom();
        }
    }
}