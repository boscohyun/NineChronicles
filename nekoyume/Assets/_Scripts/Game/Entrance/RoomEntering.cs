using System.Collections;
using UnityEngine;

namespace Nekoyume.Game.Entrance
{
    public class RoomEntering : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(Act());
        }

        private IEnumerator Act()
        {
            var stage = GetComponent<Stage>();
            var loadingScreen = UI.Widget.Find<UI.LoadingScreen>();
            loadingScreen.Show();

            UI.Widget.Find<UI.Menu>().ShowRoom();

            stage.id = 0;
            stage.LoadBackground("room");

            var objectPool = GetComponent<Util.ObjectPool>();
            var players = stage.GetComponentsInChildren<Character.Player>();
            foreach (var p in players)
            {
                objectPool.Remove<Character.Player>(p.gameObject);
            }
            objectPool.ReleaseAll();

            var boss = stage.GetComponentInChildren<Character.Boss.BossBase>();
            if (boss != null)
            {
                Destroy(boss.gameObject);
            }

            var playerFactory = GetComponent<Factory.PlayerFactory>();
            GameObject player = playerFactory.Create();
            player.transform.position = stage.RoomPosition;

            UI.Widget.Find<UI.Status>().UpdatePlayer(player);

            var cam = Camera.main.gameObject.GetComponent<ActionCamera>();
            var camPos = cam.transform.position;
            camPos.x = 0.0f;
            camPos.y = 0.0f;
            cam.transform.position = camPos;
            cam.target = null;

            yield return new WaitForSeconds(2.0f);
            loadingScreen.Close();
            Destroy(this);
        }
    }
}
