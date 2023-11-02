using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class Adder : Algorithm
    {
        public List<Signal> InputSignals { get; set; }
        public Signal OutputSignal { get; set; }

        public override void Run()
        {
            List<float> output_signals = new List<float>();
            for (int i = 0; i < InputSignals[1].Samples.Count; i++)
            {
                float x = InputSignals[0].Samples[i] + InputSignals[1].Samples[i];
                output_signals.Add(x);
            }
            OutputSignal = new Signal(output_signals, false);
        }
    }
}