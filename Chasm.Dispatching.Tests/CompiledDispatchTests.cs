using System.Collections.Generic;
using Xunit;

namespace Chasm.Dispatching.Tests
{
    public partial class CompiledDispatchTests
    {
        private static readonly List<string> outlet = [];

        [Fact]
        public void BasicUsage()
        {
            outlet.Clear();
            CompiledDispatch<int> dispatch = new();
            Assert.Equal((0, false), (dispatch.Count, dispatch.IsCompiled));

            // Add a lambda
            dispatch.Add(x => outlet.Add($"Lambda: {x}"));
            Assert.Equal((1, false), (dispatch.Count, dispatch.IsCompiled));

            // Add a local function
            List<string> localOutlet = outlet;
            void LocalFunc(int x) => localOutlet.Add($"Local: {x}");
            dispatch.Add(LocalFunc);
            Assert.Equal((2, false), (dispatch.Count, dispatch.IsCompiled));

            // Add an instance method
            dispatch.Add(InstanceMethod);
            Assert.Equal((3, false), (dispatch.Count, dispatch.IsCompiled));

            // Add a static method
            dispatch.Add(StaticMethod);
            Assert.Equal((4, false), (dispatch.Count, dispatch.IsCompiled));

            // Dispatch without compiling
            dispatch.Dispatch(355, false);
            Assert.Equal((4, false), (dispatch.Count, dispatch.IsCompiled));

            Assert.Equal(["Lambda: 355", "Local: 355", "Instance: 355", "Static: 355"], outlet);
            outlet.Clear();

            // Dispatch with compiling
            dispatch.Dispatch(73);
            Assert.Equal((4, true), (dispatch.Count, dispatch.IsCompiled));

            Assert.Equal(["Lambda: 73", "Local: 73", "Instance: 73", "Static: 73"], outlet);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void InstanceMethod(int x) => outlet.Add($"Instance: {x}");
        private static void StaticMethod(int x) => outlet.Add($"Static: {x}");

    }
}
