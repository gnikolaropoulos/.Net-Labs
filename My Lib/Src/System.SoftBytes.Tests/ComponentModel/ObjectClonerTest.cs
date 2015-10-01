using System;
using System.SoftBytes.ComponentModel;
using Xunit;

namespace System.SoftBytes.Tests.ComponentModel
{
    public sealed class ObjectClonerTest
    {
        [Fact]
        public void CanCloneUsingCodeGen()
        {
            Person person = new Person("Jeff", true);
            Person personClone = person.MakeClone(CloneOption.UseCodeGen);

            Boolean sameObjects = Object.ReferenceEquals(person, personClone);
            Assert.False(sameObjects);
        }

        [Fact]
        public void CanCloneUsingSerialization()
        {
			Person person = new Person("Jeff", true);
            Person personClone = person.MakeClone(CloneOption.UseSerialization);

            Boolean sameObjects = Object.ReferenceEquals(person, personClone);
            Assert.False(sameObjects);
        }

        #region SUT

        [Serializable]
        private sealed class Person
        {
            private Boolean m_male;

            public Person() { }

            public Person(String name, Boolean male)
            {
                Name = name;
                m_male = male;
            }

            public String Name { get; set; }

            public Decimal Age { get; set; }

            public override String ToString()
            {
                return String.Format("Name={0}, Male={1}", Name, m_male);
            }
        }

        #endregion
    }
}
