using System.ComponentModel;
using System.Reflection;
using CodeMap.ReferenceData;
using Xunit;

namespace CodeMap.Tests.ReferenceData
{
    public class EventReferenceTests : MemberReferenceTests
    {
        [Fact]
        public void CreateReferenceFromEventInfo()
        {
            var eventReference = (EventReference)Factory.Create(_GetEventInfo());
            var visitor = new MemberReferenceVisitorMock<EventReference>(eventReference);

            Assert.Equal("PropertyChanged", eventReference.Name);
            Assert.True(eventReference.DeclaringType == typeof(INotifyPropertyChanged));
            Assert.True(eventReference == _GetEventInfo());

            eventReference.Accept(visitor);
            Assert.Equal(1, visitor.VisitCount);

            EventInfo _GetEventInfo()
                => typeof(INotifyPropertyChanged).GetEvent(nameof(INotifyPropertyChanged.PropertyChanged));
        }
    }
}