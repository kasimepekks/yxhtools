using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YXH_Tools_Files.Tools_List
{
    public static class Mileage
    {
        /// <summary>
        /// 利用速度和时间来计算一个csv文件的里程
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static double ReturnSumDistanceperFile(List<double> speed, List<double> time)
        {
            double sumdistance=0;
            for (int i = 0; i < speed.Count - 1; i++)
            {

               var singledistance = (speed[i] + speed[i + 1]) * (time[i + 1] - time[i]) / 2 / 3.6;
                sumdistance += singledistance;
                
            }
            return sumdistance;
        }
        /// <summary>
        /// 按照里程数来划分文件
        /// </summary>
        /// <param name="list">每个文件的里程列表</param>
        /// <param name="permileage">基准里程值</param>
        /// <returns></returns>
        public static List<List<int>> GetIndexRanges(List<double> list, int permileage)
        {
            var result = new List<List<int>>();
            var startIndex = 0;
            var sum = 0.0;
            for (var i = 0; i < list.Count; i++)
            {
                sum += list[i];
                if (sum >= permileage)
                {
                    result.Add(new List<int> { startIndex, i });
                    startIndex = i + 1;
                    sum = 0;
                }
            }
            if (startIndex < list.Count)
            {
                //这个时候sum已经是求和完最后一个数了
                if (sum >= (permileage/2))//大于5就把剩余的数再分成一组
                {
                    result.Add(new List<int> { startIndex, list.Count - 1 });
                }
                else
                {
                    //小于5就把上一组result里的第二数字改为原数组的最后一个数的下标
                    if (startIndex == 0)//startIndex == 0说明所有文件加起来的总里程还小于5，那就只能分1组
                    {
                        result.Add(new List<int> { startIndex, list.Count - 1 });
                    }
                    else//如果startIndex不等于0则说明至少已经分了一组了，也就是说所有文件的总里程已经超过10了
                    {
                        result[result.Count - 1][1] = list.Count - 1;
                    }
                  
                }
            }
            return result;
        }
    }
}
