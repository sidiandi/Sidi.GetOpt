using System.Threading;
using System.Threading.Tasks;

namespace Sidi.GetOpt.Test
{
    class TestAsyncApp
    {
        [Usage("Other command")]
        public async Task<string> TestAsyncWithStringResult()
        {
            Thread.Sleep(50);
            TestAsyncWasCalled = true;
            return "hello";
        }

        [Usage("Command to test correct invocation of async methods")]
        public async Task<int> TestAsyncWithIntResult()
        {
            Thread.Sleep(50);
            TestAsyncWasCalled = true;
            return 123;
        }

        public bool TestAsyncWasCalled { get; private set; }
    }
}
