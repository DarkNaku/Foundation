namespace DarkNaku.Stat
{
    public partial class ModifierType : Enumeration
    {
        public static ModifierType Sum = new(1, nameof(Sum));
        public static ModifierType PercentAdd = new(2, nameof(PercentAdd));
        public static ModifierType PercentMultiply = new(3, nameof(PercentMultiply));

        public ModifierType(int id, string name) : base(id, name)
        {
        }
    }

    public class Modifier
    {
        public ModifierType Type { get; }
        public float Value { get; }
        public bool IsTemporary { get; }
        public object Source { get; }
        public string ID { get; }

        public Modifier(ModifierType type, float value, bool isTemporary = false, object source = null, string id = "")
        {
            Type = type;
            Value = value;
            IsTemporary = isTemporary;
            Source = source;
            ID = id;
        }
    }
}