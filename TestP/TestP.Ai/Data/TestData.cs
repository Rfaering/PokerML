using System;
using System.Collections.Generic;

namespace TestP.Ai.Data
{
    public class TestData
    {
        public ICollection<Sample> Samples { get; set; }

        public TestData()
        {
            var random = new Random();

            for (int i = 0; i < 1000; i++)
            {
                var sample = new Sample()
                {
                    A = random.Next(-10, 10),
                    B = random.Next(-10, 10)
                };

                if(sample.A > 8 && sample.B > 5)
                {
                    sample.C = 10;
                } else
                {
                    sample.C = 0;
                }

                Samples.Add(sample);
            }
        }
    }
}
