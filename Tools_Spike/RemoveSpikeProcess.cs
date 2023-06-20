using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YXH_Tools_Files.Tools_Spike
{
    public static class RemoveSpikeProcess
    {
        public static List<double> completeSpikeRemovalProcess(List<double> signal, int blockLengthMean, int blockLengthVariance, double threshold)
        {
            var smooth = Spike_Removal.meanSignalCal(signal, blockLengthMean);
            var roughr = Spike_Removal.createRoughSignal(signal, smooth);
            var mrr = Spike_Removal.MRiCal(roughr, blockLengthVariance);
            var variance = Spike_Removal.varianceSignalCal(roughr, mrr, blockLengthVariance);
            var spikeindex = Spike_Removal.detectSpikes(variance, threshold);
            var groupspikeindex = Spike_Removal.GroupConsecutiveNumbers(spikeindex);
            var mergegroup = Spike_Removal.MergeConsecutiveGroups(groupspikeindex);
            var interceptvalue = Spike_Removal.getInterceptvalue(signal, mergegroup);
            var rmdata = Spike_Removal.removeSpike(signal, interceptvalue, mergegroup);
            return rmdata;
        }


    }
}
