using DpvLab.XunitMtpBadDisposables.SomeLibrary;
using Xunit;

namespace DpvLab.XunitMtpBadDisposables.UnitTests
{
    public sealed class SomeClassTest
    {
        [Theory]
        [ClassData(typeof(SomeClassTestData))]
        public void TestWithTheory(SomeRandomDisposable thingThatWillFailInRunner)
        {
            // this one crashes the test runner due to the classdata instance.
            // which is written to throw an exception in dispose
            var instance = new SomeClass(42, thingThatWillFailInRunner);
            Assert.NotNull(instance);
        }

        [Fact]
        public void TestWithFact()
        {
            // this one won't crash the test runner
            var instance = new SomeClass(42, new SomeRandomDisposable());
            Assert.NotNull(instance);
        }

        
        public sealed class SomeClassTestData : TheoryData<SomeRandomDisposable>
        {
            public SomeClassTestData()
            {
                Add(new SomeRandomDisposable());
            }
        }
    }
}
