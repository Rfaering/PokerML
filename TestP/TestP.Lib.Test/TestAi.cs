using System;
using System.Collections.Generic;
using System.Text;
using TestP.Ai;
using Xunit;
using Xunit.Abstractions;

namespace TestP.Lib.Test
{
    public class TestAi
    {

        public TestAi(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        public ITestOutputHelper OutputHelper { get; }

        [Fact]
        public void Test()
        {
            var runTest = new AiTest();
            runTest.Train();
            // OutputHelper.WriteLine(result);
        }
    }
}
