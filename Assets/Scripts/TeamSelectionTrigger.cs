using UnityEngine;

namespace Assets.Scripts
{
    public class TeamSelectionTrigger : MonoBehaviour
    {
        [SerializeField]
        private Team team = Team.Red;

        void OnTriggerEnter(Collider collider)
        {
            TeamPlayer.CreateComponent(collider.gameObject, team);
        }
    }
}
