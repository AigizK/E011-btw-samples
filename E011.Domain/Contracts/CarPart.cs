using System;

namespace E011
{
    [Serializable]
    public sealed class CarPart
    {
        public readonly string Name;
        public readonly int Quantity;

        public CarPart(string name, int quantity)
        {
            Name = name;
            Quantity = quantity;
        }
    } 
}