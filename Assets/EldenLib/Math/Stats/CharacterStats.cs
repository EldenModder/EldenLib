using System;
using System.Collections.Generic;
using UnityEngine;

namespace EldenLib.Math.Stats
{
    [CreateAssetMenu(fileName = "CharacterStats", menuName = "Scriptable Objects/Stats")]
    public class CharacterStats : ScriptableObject
    {
        public enum BuffableStats
        {
            MaxHealth,
            Defense,
            Attack,
        }
        
        public event Action OnHealthDepleted;
        public event Action<int, int> OnHealthChanged;

        public int BaseMaxHealth;
        public int BaseDefense;
        public int BaseAttack;

        private int _exp;
        public int EXP
        {
            get => _exp;
            set
            {
                int oldLevel = Level;
                _exp = value;
                if (oldLevel != Level)
                    RecalculateStats();
            }
        }

        public int Level => Mathf.Max(1, (int)Mathf.Round(Mathf.Sqrt(_exp / 100.0f)));
        
        public int CurrentMaxHealth { get; set; }
        public int CurrentDefense { get; set; }
        public int CurrentAttack  { get; set; }

        private int _health;
        public int Health
        {
            get => _health;
            set
            {
                _health = Mathf.Clamp(value, 0, CurrentMaxHealth);
                OnHealthChanged?.Invoke(_health, CurrentMaxHealth);
                if (_health <= 0) OnHealthDepleted?.Invoke();
            }
        }
        
        private List<StatBuff> statBuffs = new();
        public Dictionary<BuffableStats, AnimationCurve> StatCurves = new();

        public void Init() => SetupStats();

        private void SetupStats()
        {
            RecalculateStats();
            Health = CurrentMaxHealth;
        }

        public void AddBuff(StatBuff buff)
        {
            statBuffs.Add(buff);
            RecalculateStats();
        }

        public void RemoveStats(StatBuff buff)
        {
            statBuffs.Remove(buff);
            RecalculateStats();
        }

        public void RecalculateStats()
        {
            var multipliers = new Dictionary<BuffableStats, float>();
            var addends = new Dictionary<BuffableStats, float>();

            foreach (var buff in statBuffs)
            {
                if (buff.BuffType == StatBuff.BuffTypeEnum.Add)
                {
                    if (!addends.ContainsKey((buff.Stat))) addends[buff.Stat] = 0f;
                    addends[buff.Stat] += buff.Amount;
                }
                else
                {
                    if (!multipliers.ContainsKey(buff.Stat)) multipliers[buff.Stat] = 1f;
                    multipliers[buff.Stat] += buff.Amount;

                    if (multipliers[buff.Stat] < 0f)
                        multipliers[buff.Stat] = 0f;
                }
            }

            float samplePos = (Level / 100f) - 0.01f;
            
            CurrentMaxHealth = Mathf.RoundToInt(BaseMaxHealth * GetCurve(BuffableStats.MaxHealth).Evaluate(samplePos));
            CurrentDefense = Mathf.RoundToInt(BaseDefense * GetCurve(BuffableStats.Defense).Evaluate(samplePos));
            CurrentAttack = Mathf.RoundToInt(BaseAttack * GetCurve(BuffableStats.Attack).Evaluate(samplePos));

            var currentMaxHealth = CurrentMaxHealth;
            var currentDefense = CurrentDefense;
            var currentAttack = CurrentAttack;
            ApplyModifiers(ref currentMaxHealth, BuffableStats.MaxHealth, multipliers, addends);
            ApplyModifiers(ref currentDefense, BuffableStats.Defense, multipliers, addends);
            ApplyModifiers(ref currentAttack, BuffableStats.Attack, multipliers, addends);
        }
        
        private void ApplyModifiers(
            ref int stat,
            BuffableStats statType,
            Dictionary<BuffableStats, float> multipliers,
            Dictionary<BuffableStats, float> addends)
        {
            if (multipliers.TryGetValue(statType, out float mul))
                stat = Mathf.RoundToInt(stat * mul);
            if (addends.TryGetValue(statType, out float add))
                stat += Mathf.RoundToInt(add);
        }
        
        private AnimationCurve GetCurve(BuffableStats stat) => StatCurves.TryGetValue(stat, out var curve) ? curve : AnimationCurve.Linear(0, 0, 1, 1);
    }
}