using UnityEngine;

namespace Shooter.Agents {
    public enum Team {
        None,
        CounterTerrorist,
        Terrorist
    }

    public static class TeamExtensions {
        public static Color GetColor(this Team team) => team switch {
            Team.CounterTerrorist => new Color(0.55f, 0.61f, 0.65f, 1f),
            Team.Terrorist => new Color(0.8f, 0.75f, 0.55f, 1f),
            _ => Color.white
        };

        public static bool IsHostile(this Team team, Team other) {
            if (team != other) return true;
            return team == Team.None;
        }
    }
}