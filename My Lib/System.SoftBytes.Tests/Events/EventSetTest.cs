using System;
using System.Threading;
using System.Threading.Tasks;
using System.SoftBytes.Events;
using Xunit;

namespace System.SoftBytes.Tests.Events
{
    public sealed class EventSetTest
    {
        // Define a private instance field that references a collection.
        // The collection manages a set of Event/Delegate pairs.
        // NOTE: The EventSet type is not part of the FCL, it is my own type.
        private readonly EventSet m_eventSet = new EventSet();

        [Fact]
        public void FooEvent()
        {
            Boolean called = false;
            ThreadPool.QueueUserWorkItem(delegate
            {
                Foo += new EventHandler<FooEventArgs>(
                    (sender, e) => { called = true; });
                RaiseFoo();
            }, null);
            SpinWait.SpinUntil(() => called == true);
            Assert.True(called);
        }

        [Fact]
        public void BarEvent()
        {
            Boolean called = false;
            new Task(() =>
            {
                Bar += new EventHandler<BarEventArgs>(
                    (sender, e) => { called = true; });
                RaiseBar();
            }).Start();
            SpinWait.SpinUntil(() => called == true);
            Assert.True(called);
        }

        #region Code to support the Foo event (repeat this pattern for additional events)
        // Define the members necessary for the Foo event.
        // 2a. Construct a static, read-only object to identify this event.
        // Each object has its own hash code for looking up this
        // event’s delegate linked list in the object’s collection.
        private static readonly EventKey s_fooEventKey = new EventKey();

        // 2d. Define the event’s accessor methods that add/remove the
        // delegate from the collection.
        public event EventHandler<FooEventArgs> Foo
        {
            add { m_eventSet.Add(s_fooEventKey, value); }
            remove { m_eventSet.Remove(s_fooEventKey, value); }
        }

        // 2e. Define the protected, virtual On method for this event.
        private void OnFoo(FooEventArgs e)
        {
            m_eventSet.Raise(s_fooEventKey, this, e);
        }

        // 2f. Define the method that translates input to this event.
        public void RaiseFoo()
        {
            OnFoo(new FooEventArgs());
        }
        #endregion

        #region Code to support the Bar event
        // 3. Define the members necessary for the Bar event.
        // 3a. Construct a static, read-only object to identify this event.
        // Each object has its own hash code for looking up this
        // event’s delegate linked list in the object’s collection.
        private static readonly EventKey s_barEventKey = new EventKey();

        // 3d. Define the event’s accessor methods that add/remove the
        // delegate from the collection.
        public event EventHandler<BarEventArgs> Bar
        {
            add { m_eventSet.Add(s_barEventKey, value); }
            remove { m_eventSet.Remove(s_barEventKey, value); }
        }

        // 3e. Define the protected, virtual On method for this event.
        private void OnBar(BarEventArgs e)
        {
            m_eventSet.Raise(s_barEventKey, this, e);
        }

        // 3f. Define the method that translates input to this event.
        public void RaiseBar()
        {
            OnBar(new BarEventArgs());
        }
        #endregion

        // Define the EventArgs-derived type for this event.
        public class FooEventArgs : EventArgs { }

        // Define the EventArgs-derived type for this event.
        public class BarEventArgs : EventArgs { }
    }
}
