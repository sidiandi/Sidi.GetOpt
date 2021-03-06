﻿using System.Threading;
using System.Threading.Tasks;

namespace Sidi.GetOpt.Test
{
    #pragma warning disable CS1998
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

        public const string anErrorOccured = "An error occured.";

        [Usage("Command to test correct exception handling")]
        public async Task<int> TestAsyncWithException()
        {
            TestAsyncWasCalled = true;
            throw new System.Exception(anErrorOccured);
        }

        public bool TestAsyncWasCalled { get; private set; }
    }
}
