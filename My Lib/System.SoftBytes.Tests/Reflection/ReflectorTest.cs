using System;
using System.Linq.Expressions;
using System.Reflection;
using System.SoftBytes.Reflection;
using Xunit;

namespace System.SoftBytes.Tests.Reflection
{
    public sealed class ReflectorFixture
    {
        [Fact]
        public void ShouldThrowIfNullMethodLambda()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Reflector<TestEntity>.GetMethod((Expression<Action<TestEntity>>)null);
            });
        }

        [Fact]
        public void ShouldThrowIfNullPropertyLambda()
        {
            Assert.Throws<ArgumentNullException>(() =>
           {
               Reflector<TestEntity>.GetProperty((Expression<Func<TestEntity, object>>)null);
           });
        }

        [Fact]
        public void ShouldThrowIfNullFieldLambda()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Reflector<TestEntity>.GetField((Expression<Func<TestEntity, Object>>)null);
            });
        }

        [Fact]
        public void ShouldThrowIfNotMethodLambda()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Reflector<TestEntity>.GetMethod(x => new Object());
            });
        }

        [Fact]
        public void ShouldGetPublicProperty()
        {
            PropertyInfo info = Reflector<TestEntity>.GetProperty(x => x.PublicProperty);
            Assert.True(info == typeof(TestEntity).GetProperty("PublicProperty"));
        }

        [Fact]
        public void ShouldGetPublicVoidMethod()
        {
            MethodInfo info = Reflector<TestEntity>.GetMethod(x => x.PublicVoidMethod());
            Assert.True(info == typeof(TestEntity).GetMethod("PublicVoidMethod"));
        }

        [Fact]
        public void ShouldGetPublicMethodParameterless()
        {
            MethodInfo info = Reflector<TestEntity>.GetMethod(x => x.PublicMethodNoParameters());
            Assert.True(info == typeof(TestEntity).GetMethod("PublicMethodNoParameters"));
        }

        [Fact]
        public void ShouldGetPublicMethodParameters()
        {
            MethodInfo info = Reflector<TestEntity>.GetMethod<String, Int32>(
                (x, y, z) => x.PublicMethodParameters(y, z));
            Assert.True(info == typeof(TestEntity).GetMethod("PublicMethodParameters", 
                new Type[] { typeof(String), typeof(Int32) }));
        }

        [Fact]
        public void ShouldGetNonPublicProperty()
        {
            PropertyInfo info = Reflector<ReflectorFixture>.GetProperty(x => x.NonPublicProperty);
            Assert.True(info == typeof(ReflectorFixture).GetProperty(
                "NonPublicProperty", BindingFlags.Instance | BindingFlags.NonPublic));
        }

        [Fact]
        public void ShouldGetNonPublicField()
        {
            FieldInfo info = Reflector<ReflectorFixture>.GetField(x => x.m_nonPublicField);
            Assert.True(info == typeof(ReflectorFixture).GetField(
                "m_nonPublicField", BindingFlags.Instance | BindingFlags.NonPublic));
        }

        [Fact]
        public void ShouldGetNonPublicMethod()
        {
            MethodInfo info = Reflector<ReflectorFixture>.GetMethod(x => x.NonPublicMethod());
            Assert.True(info == typeof(ReflectorFixture).GetMethod(
                "NonPublicMethod", BindingFlags.Instance | BindingFlags.NonPublic));
        }

        private Boolean m_nonPublicField;

        private Boolean NonPublicProperty
        {
            get { return m_nonPublicField; }
            set { m_nonPublicField = value; }
        }

        private Object NonPublicMethod()
        {
            throw new NotImplementedException();
        }

        #region SUT

        private sealed class TestEntity
        {
            private Int32 m_valueProp;

            public TestEntity(String foo = null, Int32 bar = 0) { }

            public Int32 PublicProperty
            {
                get { return m_valueProp; }
                set { m_valueProp = value; }
            }

            public Boolean PublicMethodNoParameters()
            {
                throw new NotImplementedException();
            }

            public Boolean PublicMethodParameters(String foo, Int32 bar)
            {
                throw new NotImplementedException();
            }

            public void PublicVoidMethod()
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
