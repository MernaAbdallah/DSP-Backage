using DSPAlgorithms.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace DSPAlgorithms.Algorithms

{
    public class PracticalTask2 : Algorithm
    {
        public String SignalPath { get; set; }
        public float Fs { get; set; }
        public float miniF { get; set; }
        public float maxF { get; set; }
        public float newFs { get; set; }
        public int L { get; set; } //upsampling factor
        public int M { get; set; } //downsampling factor
        public Signal OutputFreqDomainSignal { get; set; }

        public override void Run()
        {
            Signal signal = new Signal(new List<float>(), new List<int>(), false);
            OutputFreqDomainSignal = new Signal(new List<float>(), false);
            Signal InputSignal = LoadSignal(SignalPath);

            //FIR        
            FIR f = new FIR();
            f.InputTimeDomainSignal = InputSignal;
            f.InputFilterType = FILTER_TYPES.BAND_PASS;
            f.InputFS = Fs;
            f.InputF1 = miniF;
            f.InputF2 = maxF;
            f.InputStopBandAttenuation = 50;
            f.InputTransitionBand = 500;
            f.Run();
            signal = f.OutputYn;
            using (StreamWriter w = new StreamWriter("C:\\Users\\asus\\Desktop\\FIR.ds"))
            {
                w.WriteLine("0"); //freq or time //flag_freq_or_time : if freq print 1 else print 0
                w.WriteLine("0");// perodic or non  //periodic_or_not : if preodic print 1 else print 0
                w.WriteLine(f.OutputYn.Samples.Count().ToString());
                for (int i = 0; i < f.OutputYn.Samples.Count(); i++)
                {

                    w.WriteLine(f.OutputYn.SamplesIndices[i].ToString() + " " + f.OutputYn.Samples[i].ToString());

                }
            }

            //==================================================================================================================================//

            //SAMPLING
            if (newFs >= 2 * maxF)
            {
                Sampling s = new Sampling();
                s.InputSignal = f.OutputYn;
                s.M = M;
                s.L = L;
                s.Run();
                using (StreamWriter w = new StreamWriter("C:\\Users\\asus\\Desktop\\sampling.ds"))
                {
                    w.WriteLine("0");
                    w.WriteLine("0");
                    w.WriteLine(s.OutputSignal.Samples.Count().ToString());
                    for (int i = 0; i < s.OutputSignal.Samples.Count(); i++)
                    {

                        w.WriteLine(s.OutputSignal.SamplesIndices[i].ToString() + " " + s.OutputSignal.Samples[i].ToString());

                    }
                }
                signal = s.OutputSignal;

            }
            else
                Console.WriteLine("newFs is not valid");


           //=====================================================================================================================================================//

            //REMOVE DC_COMPONENT
            DC_Component dc = new DC_Component();
            dc.InputSignal = signal;
            dc.Run();
            using (StreamWriter w = new StreamWriter("C:\\Users\\asus\\Desktop\\DC_component.ds"))
            {
                w.WriteLine("0");
                w.WriteLine("0");
                w.WriteLine(dc.OutputSignal.Samples.Count().ToString());
                for (int i = 0; i < dc.OutputSignal.Samples.Count(); i++)
                {

                    w.WriteLine(dc.OutputSignal.SamplesIndices[i].ToString() + " " + dc.OutputSignal.Samples[i].ToString());

                }
            }


         //===============================================================================================================================================//
            //NORMALIZE
            Normalizer n = new Normalizer();
            n.InputSignal = dc.OutputSignal;
            n.InputMinRange = -1;
            n.InputMaxRange = 1;
            n.Run();
            using (StreamWriter w = new StreamWriter("C:\\Users\\asus\\Desktop\\Normalize.ds"))
            {
                w.WriteLine("0");
                w.WriteLine("0");
                w.WriteLine(n.OutputNormalizedSignal.Samples.Count().ToString());
                for (int i = 0; i < n.OutputNormalizedSignal.Samples.Count(); i++)
                {

                    w.WriteLine(n.OutputNormalizedSignal.SamplesIndices[i].ToString() + " " + n.OutputNormalizedSignal.Samples[i].ToString());

                }
            }
 

            //=================================================================================================================================================//

            //DFT
            DiscreteFourierTransform dft = new DiscreteFourierTransform();
            dft.InputTimeDomainSignal = n.OutputNormalizedSignal;
            dft.InputSamplingFrequency = Fs;

            dft.Run();
          OutputFreqDomainSignal = dft.OutputFreqDomainSignal;
           for (int i = 0; i < OutputFreqDomainSignal.Frequencies.Count(); i++) {
                OutputFreqDomainSignal.Frequencies[i] = (float)Math.Round((double)dft.OutputFreqDomainSignal.Frequencies[i], 1);
            }
               
            using (StreamWriter w = new StreamWriter("C:\\Users\\asus\\Desktop\\DFTpractical2.ds"))
            {
                w.WriteLine("1");
                w.WriteLine("0");
                w.WriteLine(dft.OutputFreqDomainSignal.Frequencies.Count().ToString());
                for (int i = 0; i < OutputFreqDomainSignal.Frequencies.Count(); i++)
                {

                    w.WriteLine(OutputFreqDomainSignal.Frequencies[i].ToString() + " " + OutputFreqDomainSignal.FrequenciesAmplitudes[i].ToString() + " " + OutputFreqDomainSignal.FrequenciesPhaseShifts[i].ToString());

                }
            }
        }

        public Signal LoadSignal(string filePath)
        {
            Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var sr = new StreamReader(stream);

            var sigType = byte.Parse(sr.ReadLine());
            var isPeriodic = byte.Parse(sr.ReadLine());
            long N1 = long.Parse(sr.ReadLine());

            List<float> SigSamples = new List<float>(unchecked((int)N1));
            List<int> SigIndices = new List<int>(unchecked((int)N1));
            List<float> SigFreq = new List<float>(unchecked((int)N1));
            List<float> SigFreqAmp = new List<float>(unchecked((int)N1));
            List<float> SigPhaseShift = new List<float>(unchecked((int)N1));

            if (sigType == 1)
            {
                SigSamples = null;
                SigIndices = null;
            }

            for (int i = 0; i < N1; i++)
            {
                if (sigType == 0 || sigType == 2)
                {
                    var timeIndex_SampleAmplitude = sr.ReadLine().Split();
                    SigIndices.Add(int.Parse(timeIndex_SampleAmplitude[0]));
                    SigSamples.Add(float.Parse(timeIndex_SampleAmplitude[1]));
                }
                else
                {
                    var Freq_Amp_PhaseShift = sr.ReadLine().Split();
                    SigFreq.Add(float.Parse(Freq_Amp_PhaseShift[0]));
                    SigFreqAmp.Add(float.Parse(Freq_Amp_PhaseShift[1]));
                    SigPhaseShift.Add(float.Parse(Freq_Amp_PhaseShift[2]));
                }
            }

            if (!sr.EndOfStream)
            {
                long N2 = long.Parse(sr.ReadLine());

                for (int i = 0; i < N2; i++)
                {
                    var Freq_Amp_PhaseShift = sr.ReadLine().Split();
                    SigFreq.Add(float.Parse(Freq_Amp_PhaseShift[0]));
                    SigFreqAmp.Add(float.Parse(Freq_Amp_PhaseShift[1]));
                    SigPhaseShift.Add(float.Parse(Freq_Amp_PhaseShift[2]));
                }
            }

            stream.Close();
            return new Signal(SigSamples, SigIndices, isPeriodic == 1, SigFreq, SigFreqAmp, SigPhaseShift);
        }
    }
}