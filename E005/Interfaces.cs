using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace E005_testing_use_cases
{
    public interface ISampleMessage { }

    public interface ICommand : ISampleMessage { }

    public interface IEvent : ISampleMessage { }

    public interface ICommand<out TIdentity> : ICommand
        where TIdentity : IIdentity
    {
        TIdentity Id { get; }
    }

    public interface IApplicationService
    {
        void Execute(object command);
    }


    public interface IEvent<out TIdentity> : IEvent
        where TIdentity : IIdentity
    {
        TIdentity Id { get; }
    }

    public interface IEventStore
    {
        EventStream LoadEventStream(IIdentity id);
        void AppendEventsToStream(IIdentity id, long version, ICollection<IEvent> events);
    }

    public sealed class EventStream
    {
        public long StreamVersion;
        public List<IEvent> Events = new List<IEvent>();
    }

    [Serializable]
    public class DomainError : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DomainError() { }
        public DomainError(string message) : base(message) { }
        public DomainError(string format, params object[] args) : base(string.Format(format, args)) { }

        /// <summary>
        /// Creates domain error exception with a string name, that is easily identifiable in the tests
        /// </summary>
        /// <param name="name">The name to be used to identify this exception in tests.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static DomainError Named(string name, string format, params object[] args)
        {
            var message = "[" + name + "] " + string.Format(format, args);
            return new DomainError(message)
            {
                Name = name
            };
        }

        public string Name { get; private set; }

        public DomainError(string message, Exception inner) : base(message, inner) { }

        protected DomainError(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }
    }
}
