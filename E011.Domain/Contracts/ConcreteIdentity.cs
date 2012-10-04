using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace E011
{
    [Serializable]
    public sealed class FactoryId : AbstractIdentity<long>
    {
        public const string TagValue = "factory";

        public FactoryId(long id)
        {
            Contract.Requires(id > 0);
            Id = id;
        }

        public override string GetTag()
        {
            return TagValue;
        }


        [DataMember(Order = 1)]
        public override long Id { get; protected set; }

        public FactoryId() { }

        public static FactoryId ForTest = new FactoryId(long.MaxValue);
    }
}