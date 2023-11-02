using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class FIR : Algorithm
    {
        public Signal InputTimeDomainSignal { get; set; }
        public FILTER_TYPES InputFilterType { get; set; }
        public float InputFS { get; set; }
        public float? InputCutOffFrequency { get; set; }
        public float? InputF1 { get; set; }
        public float? InputF2 { get; set; }
        public float InputStopBandAttenuation { get; set; }
        public float InputTransitionBand { get; set; }
        public Signal OutputHn { get; set; }
        public Signal OutputYn { get; set; }

        public override void Run()
        {
            OutputHn = new Signal(new List<float>(), new List<int>(), false);
            float TransitionBandNormalized = InputTransitionBand / InputFS;
            int N;
            int n ;

            float TransitionWidth = 0.0f;
            string windowType = "";

            if (InputStopBandAttenuation <= 21)
            {
                TransitionWidth = 0.9f;
                windowType = "rect";
            }
            else if (InputStopBandAttenuation > 21 && InputStopBandAttenuation <= 44)
            {
                TransitionWidth = 3.1f;
                windowType = "hanning";
            }
            else if (InputStopBandAttenuation > 44 && InputStopBandAttenuation <= 53)
            {
                TransitionWidth = 3.3f;
                windowType = "hamming";
            }
            else if (InputStopBandAttenuation > 53 && InputStopBandAttenuation <= 74)
            {
                TransitionWidth = 5.5f;
                windowType = "blackman";
            }

            //TransitionWidth of window func = 3.3/N
            //we need N 
            //so 3.3/N = TransitionBandNormalized
            //so N = 3.3 / TransitionBandNormalized


            N = (int)Math.Ceiling(TransitionWidth / TransitionBandNormalized); // N = 53
            if (N % 2 == 0)
                //next odd
                N++;

            n = -N / 2; // -26 < n < 26
            for (int i = 0; i < N; i++)
            {
                OutputHn.SamplesIndices.Add(n);
                n++;
            }
           

          

            if (InputFilterType == FILTER_TYPES.LOW)
            {
                float FcDash = (float)InputCutOffFrequency + (InputTransitionBand / 2);

                float Wc = FcDash / InputFS;

                for (int i = 0; i < N; i++)
                {
                       int x =OutputHn.SamplesIndices[i];

                        if (OutputHn.SamplesIndices[i] == 0)
                        {
                            float hn = 2 * Wc;
                            float window = select_window_method(windowType, x, N);
                            OutputHn.Samples.Add(hn * window);
                        }
                        else
                        {
                           float hn = (float)(2 * Wc * Math.Sin((float)(2 * Math.PI * Wc * x)) / (float)(2 * Math.PI * Wc * x));
                            float window = select_window_method(windowType, x, N);
                            OutputHn.Samples.Add(hn * window);
                        }
                    
                }
            }

             if (InputFilterType == FILTER_TYPES.HIGH)
            {
                float FcDash = (float)InputCutOffFrequency + (InputTransitionBand / 2);
                float Wc = FcDash / InputFS;
                for (int i = 0; i < N; i++)
                {
                    int x = OutputHn.SamplesIndices[i];
                    if (OutputHn.SamplesIndices[i] == 0)
                    {
                        float hn = 1- ( 2 * Wc);
                        float window = select_window_method(windowType, x, N);
                        OutputHn.Samples.Add(hn * window);
                    }
                    else
                    {
                        float hn = - (float)(2 * Wc * Math.Sin((float)(2 * Math.PI * Wc * x)) / (float)(2 * Math.PI * Wc * x));
                        float window = select_window_method(windowType, x, N);
                        OutputHn.Samples.Add(hn * window);
                    }

                }
            }

            if (InputFilterType == FILTER_TYPES.BAND_PASS)
            {
              
                float Wc1 =  (((float)InputF1 - (InputTransitionBand / 2)) / InputFS);
                float Wc2 = (((float)InputF2 + (InputTransitionBand / 2)) / InputFS);

                for (int i = 0; i < N; i++)
                {
                    int x = OutputHn.SamplesIndices[i];

                    if (OutputHn.SamplesIndices[i] == 0)
                    {
                        float hn = 2 * (Wc2 - Wc1);
                        float window = (select_window_method(windowType, x, N));
                        OutputHn.Samples.Add(hn * window);
                    }
                    else
                    {

                        float hn = (float)(2 * Wc2 * Math.Sin((float)(2 * Math.PI * Wc2 * x)) / (float)(2 * Math.PI * Wc2 * x)) - (float)(2 * Wc1 * Math.Sin((float)(2 * Math.PI * Wc1 * x)) / (float)(2 * Math.PI * Wc1 * x));
                        float window =( select_window_method(windowType, x, N));
                        OutputHn.Samples.Add(hn * window);
                    }

                }
            }


            if (InputFilterType == FILTER_TYPES.BAND_STOP)
            {

                float Wc1 = (((float)InputF1 - (InputTransitionBand / 2)) / InputFS);
                float Wc2 = (((float)InputF2 + (InputTransitionBand / 2)) / InputFS);

                for (int i = 0; i < N; i++)
                {
                    int x = OutputHn.SamplesIndices[i];

                    if (OutputHn.SamplesIndices[i] == 0)
                    {
                     
                       float hn = 1- (2 * (Wc2 - Wc1) );
                        float window =( select_window_method(windowType, x, N));
                        OutputHn.Samples.Add(hn * window);
                    }
                    else
                    {            

                        float hn =  - (float)(2 * Wc2 * Math.Sin((float)(2 * Math.PI * Wc2 * x)) / (float)(2 * Math.PI * Wc2 * x)) + (float)(2 * Wc1 * Math.Sin((float)(2 * Math.PI * Wc1 * x)) / (float)(2 * Math.PI * Wc1 * x));
                        float window = (select_window_method(windowType, x, N));
                        OutputHn.Samples.Add(hn * window);
                    }

                }
            }





            DirectConvolution c = new DirectConvolution();
            c.InputSignal1 = InputTimeDomainSignal;
            c.InputSignal2 = OutputHn;
            c.Run();
            OutputYn = c.OutputConvolvedSignal;
        }


        

        public float select_window_method(String windowType, int n, int N)
        {
            float result = 0.0f;
            if (windowType == "rect")
            {
                result = 1;
            }
            else if (windowType == "hanning")
            {
                result = (float)0.5 + (float)(0.5 * Math.Cos((2 * Math.PI * n) / N));
            }
            else if (windowType == "hamming")
            {
                result = (float)0.54 + (float)(0.46 * Math.Cos((2 * Math.PI * n) / N));
            }
            else if (windowType == "blackman")
            {
                result = (float)(0.42 + (0.5 * Math.Cos((2 * Math.PI * n) / (N - 1))) + (0.08 * Math.Cos((4 * Math.PI * n) / (N - 1))));
            }

            return result;
        }
    }
}
