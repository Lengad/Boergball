using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class TeamPlayer : NetworkBehaviour
    {
        public Team Team;

        public static TeamPlayer CreateComponent(GameObject gameObject, Team team)
        {
            TeamPlayer tp = gameObject.AddComponent<TeamPlayer>();
            tp.Team = team;
            return tp;
        }
    }
}
