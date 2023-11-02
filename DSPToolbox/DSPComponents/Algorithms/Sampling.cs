using DSPAlgorithms.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPAlgorithms.Algorithms
{
    public class Sampling : Algorithm
    {
        public int L { get; set; } //upsampling factor
        public int M { get; set; } //downsampling factor
        public Signal InputSignal { get; set; }
        public Signal OutputSignal { get; set; }




        public override void Run()
        {

            

            //DOWN
            if (L == 0 && M != 0) 
            {

                FIR fir = new FIR();
                fir.InputTimeDomainSignal = InputSignal;
                fir.InputFilterType = FILTER_TYPES.LOW;
                fir.InputFS = 8000;
                fir.InputStopBandAttenuation = 50;
                fir.InputCutOffFrequency = 1500;
                fir.InputTransitionBand = 500;

                fir.Run();


                List<float> output = new List<float>();

                /*According to the number of m, 
                 * take a value from the sample and
                 * sign values with the number of m-1 from the samples
                 */

                for (int i = 0; i < fir.OutputYn.Samples.Count; i += M)
                {
                    output.Add(fir.OutputYn.Samples[i]);
                }
                OutputSignal = new Signal(output, false);


            }


            //UP sampling
            if (L != 0 && M == 0) 
            {

                List<float> output = new List<float>();

                for (int i = 0; i < InputSignal.Samples.Count; i++)
                {
                    output.Add(InputSignal.Samples[i]);
                    // Put 0 by the number of (L-1) between each value in samples
                    if(i < InputSignal.Samples.Count - 1)
                    {
                        for (int k = 0; k < L - 1; k++)
                        {
                            output.Add(0);
                        }
                    }

                }

                Signal mySignal = new Signal(output, false);

                FIR fir = new FIR();
                fir.InputTimeDomainSignal = mySignal;
                fir.InputFilterType = FILTER_TYPES.LOW;
                fir.InputFS = 8000;
                fir.InputStopBandAttenuation = 50;
                fir.InputCutOffFrequency = 1500;
                fir.InputTransitionBand = 500;

                fir.Run();

                OutputSignal = fir.OutputYn;
               

            }


            //UP smapling && down sampling
            if (L > 0 && M > 0) 
            {

                List<float> output = new List<float>();

                for (int i = 0; i < InputSignal.Samples.Count; i++)
                {
                    output.Add(InputSignal.Samples[i]);
                    // Put 0 by the number of (L-1) between each value in samples
                    if (i < InputSignal.Samples.Count - 1)
                    {
                        for (int k = 0; k < L - 1; k++)
                        {
                            output.Add(0);
                        }
                    }

                }

                Signal mySignal = new Signal(output, false);

                FIR fir = new FIR();
                fir.InputTimeDomainSignal = mySignal;
                fir.InputFilterType = FILTER_TYPES.LOW;
                fir.InputFS = 8000;
                fir.InputStopBandAttenuation = 50;
                fir.InputCutOffFrequency = 1500;
                fir.InputTransitionBand = 500;

                fir.Run();
                OutputSignal = fir.OutputYn;

                List<float> output2 = new List<float>();

                for (int i = 0; i < OutputSignal.Samples.Count; i += M)
                {
                    output2.Add(fir.OutputYn.Samples[i]);
                }
                OutputSignal = new Signal(output2, false);


            }

            //case 4
            if(L==0 && M == 0)
            {
                return;
            }
        }
    }

}