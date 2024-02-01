using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DarkNaku.Stat
{
    [System.Serializable]
    public partial class CharacterStats
    {
        public IReadOnlyDictionary<string, Stat> All => _stats;
        
        public UnityEvent<CharacterStats, Stat> OnChangeStat { get; } = new();

        private Dictionary<string, Stat> _stats;
        
        public Stat this[string statName]
        {
            get
            {
                if (_stats.ContainsKey(statName))
                {
                    return _stats[statName];
                }
                else
                {
                    Debug.LogErrorFormat("[CharacterStats] Can't found stat - {0}", statName);
                    return null;
                }
            }
        }

        public CharacterStats()
        {
            _stats = new Dictionary<string, Stat>();
        }

        public bool Contains(string statName) => _stats.ContainsKey(statName);

        public void Add(string statName, float initialValue)
        {
            if (_stats.ContainsKey(statName))
            {
                Debug.LogErrorFormat("[CharacterStats] Add : Already added - {0}", statName);
            }
            else
            {
                var stat = new Stat(initialValue, statName);

                stat.OnChangeValue.AddListener(OnChangeValue);

                _stats.Add(statName, stat);
            }
        }

        public void AddModifier(string statName, Modifier modifier)
        {
            if (_stats.ContainsKey(statName))
            {
                _stats[statName].AddModifier(modifier);
            }
            else
            {
                Debug.LogErrorFormat("[CharacterStats] AddModifier : Can't found stat - {0}", statName);
            }
        }

        public void RemoveModifier(string statName, Modifier modifier)
        {
            if (_stats.ContainsKey(statName))
            {
                _stats[statName].RemoveModifier(modifier);
            }
            else
            {
                Debug.LogErrorFormat("[CharacterStats] RemoveModifier : Can't found stat - {0}", statName);
            }
        }
        
        public void RemoveModifierFromSource(object source)
        {
            foreach (var stat in _stats.Values)
            {
                stat.RemoveModifiersFromSource(source);
            }
        }

        public void Log(string title)
        {
            var info = $"[CharacterStats  -  {title}]";
            
            foreach (var stat in _stats.Values)
            {
                info += $"\n{stat.Name} = Value : {stat.Value}, Value Without Temporary : {stat.PermanentValue}";
            }
            
            Debug.Log(info);
        }

        private void OnChangeValue(Stat stat)
        {
            OnChangeStat.Invoke(this, stat);
        }
    }
}