using Xunit;

namespace VCSJones.FiddlerCert.UnitTests
{
    public class IoCTests
    {
        [Fact]
        public void ShouldResolveSimpleObject()
        {
            var ioc = new IoC();
            ioc.Register<ITest, Test>();
            ioc.Register<IBar, Bar>();
            var resolve = ioc.Resolve<ITest>();
            var bar = ioc.Resolve<IBar>();
            Assert.IsType<Test>(resolve);
            Assert.IsType<Bar>(bar);
            var secondResolve = ioc.Resolve<ITest>();
            Assert.Same(resolve, secondResolve);
            Assert.Same(bar, resolve.Foo);
        }

        [Fact]
        public void ShouldSupportChildContainers()
        {
            var parent = new IoC();
            parent.Register<IBar, Bar>();
            Assert.IsType<Bar>(parent.Resolve<IBar>());
            using (var child = parent.Child())
            {
                child.Register<IBar, Bar2>();
                Assert.IsType<Bar2>(child.Resolve<IBar>());
                Assert.IsType<Bar>(parent.Resolve<IBar>());
            }
            Assert.IsType<Bar>(parent.Resolve<IBar>());

        }

        private interface IBar
        {

        }

        private class Bar : IBar
        {

        }

        private class Bar2 : IBar
        {

        }

        private interface ITest
        {
            IBar Foo { get; }
        }

        private class Test : ITest
        {
            public IBar Foo { get; }

            public Test(IBar test)
            {
                Foo = test;
            }
        }


    }

}
