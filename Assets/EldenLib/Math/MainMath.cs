using UnityEngine;

namespace EldenLib.Math
{
    public class MainMath : MonoBehaviour
    {
        public static MainMath Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("More than one MainMath found!");
                return;
            }

            Instance = this;
        }

        public int CalculateDamage(float baseDamage, float attackPower, float defense)
        {
            var totalDamage = baseDamage * (attackPower / (attackPower + defense));
            return Mathf.RoundToInt(totalDamage);
        }

        public int AdvanceCalculateDamage(float ATK, float HP, float DEF)
        {
            var totalDamage = Mathf.Sqrt(ATK) //reduce impact of big damage value
                * Mathf.Log(HP + 1f) //slow scaling
                / (1f + Mathf.Log(DEF + 1f)); //DEF reduce damage with diminishing returns
            return Mathf.RoundToInt(totalDamage);
        }
    }
}
