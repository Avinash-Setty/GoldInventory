using System;

namespace GoldInventory.Core
{
    public class Condition
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public ConditionTypes ConditionType { get; set; }
    }
}