using System;
using System.Linq;
using Xunit;

namespace IDisposableSourceGenerator.Test
{
    public class UnitTest1
    {
        [Fact]
        public void DisposeItem()
        {
            var d = new DisposableObject();

            d.IsDisposed.IsFalse();
            using (var reader = new MyDisposer(d)) { }
            d.IsDisposed.IsTrue();
        }

        [Fact]
        public void DisposeItems()
        {
            var ds = Enumerable.Range(0, 10).Select(_ => new DisposableObject()).ToArray();

            ds.Select(x => x.IsDisposed).All(x => !x).IsTrue();
            using (var reader = new MyDisposer(ds)) { }
            ds.Select(x => x.IsDisposed).All(x => x).IsTrue();
        }

    }
}
