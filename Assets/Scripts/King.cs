using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class King : NetworkBehaviour
    {
        [SerializeField]
        private Team team = Team.Red;

        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Ball") || other.gameObject.CompareTag("Lava"))
            {
                GameManager.Instance.KingHit(team);
            }
        }
    }
}
