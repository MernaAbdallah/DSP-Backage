using DSPAlgorithms.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPAlgorithms.Algorithms
{
    public class DCT : Algorithm
    {
        public Signal InputSignal { get; set; }
        public Signal OutputSignal { get; set; }

        public override void Run()
        {
            List<float> outputsignal = new List<float>();
            float count = InputSignal.Samples.Count;
            float sqrtvalue = (float)Math.Sqrt(2.0 / count);

            for (int i = 0; i < count; i++)
            {
                float sum = 0;
                for (int j = 0; j < count; j++)
                {
                    sum += InputSignal.Samples[j] * (float)Math.Cos((2 * j - 1) * (2 * i - 1) * Math.PI / (4.0 * count));
                }
                float r = sqrtvalue * sum;
                outputsignal.Add(r);
            }

            OutputSignal = new Signal(outputsignal, false);
        }
    }
}
