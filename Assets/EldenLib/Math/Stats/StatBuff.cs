using UnityEngine;

namespace EldenLib.Math.Stats
{
    [CreateAssetMenu(fileName = "StatBuff", menuName = "Scriptable Objects/Buff")]
    public class StatBuff : ScriptableObject
    {
        public enum BuffTypeEnum
        {
            Add,
            Multiply,
        }
        
        public CharacterStats.BuffableStats Stat;
        public BuffTypeEnum BuffType;
        public float Amount;
    }
}