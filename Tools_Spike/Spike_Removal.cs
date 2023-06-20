using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YXH_Tools_Files.Tools_Spike
{
    public static class Spike_Removal
    {
        /// <summary>
        /// 先每meanblocklength个点算均值
        /// </summary>
        /// <param name="list">数据源</param>
        /// <param name="meanblocklength">均值点数</param>
        /// <returns></returns>
        public static List<double> meanSignalCal(List<double> list,int meanblocklength)
        {
            List<double> M = new List<double>();
            for (int i = 0; i < list.Count; i++)
            {
                var half = meanblocklength / 2;
                double t = 0;
                int lessthan = 0;
                for (int j = i-half; j <= i+half; j++)
                {
                    
                    if ((j) < 0)
                    {
                        //t += list[0];
                        t += 0;
                        lessthan++;
                    }
                    else
                    {
                        if (j >= list.Count)
                        {
                            //t += list[list.Count - 1];
                            t += 0;
                            lessthan++;
                        }
                        else
                        {
                            t += list[j];
                        }
                    }
                   
                }
                M.Add(t / (meanblocklength- lessthan));
            }
            return M;
        }

        /// <summary>
        /// 创建一个新的rough数据（原数据-均值数据）
        /// </summary>
        /// <param name="orignal">原数据</param>
        /// <param name="smooth">均值数据</param>
        /// <returns></returns>
        public static List<double> createRoughSignal(List<double> orignal, List<double> smooth)
        {
            List<double> newlist = new List<double>();
            for (int i = 0; i < orignal.Count; i++)
            {
                newlist.Add(orignal[i] - smooth[i]);
            }
            return newlist;
        }
        /// <summary>
        /// 计算MRi的值
        /// </summary>
        /// <param name="rough">rough数据</param>
        /// <param name="varianceblocklength">方差点数</param>
        /// <returns></returns>
        public static List<double> MRiCal(List<double> rough, int varianceblocklength)
        {
            List<double> MR = new List<double>();
            for (int i = 0; i < rough.Count; i++)
            {
                var half = varianceblocklength / 2;
                double t = 0;
                int lessthan = 0;
                for (int j = i - half; j <= i + half; j++)
                {
                    if ((j) < 0)
                    {
                        //t += rough[0];
                        t += 0;
                        lessthan++;
                    }
                    else
                    {
                        if (j >= rough.Count)
                        {
                            //t += rough[rough.Count - 1];
                            t += 0;
                            lessthan++;
                        }
                        else
                        {
                            t += rough[j];
                        }
                    }

                }
                MR.Add(t / (varianceblocklength - lessthan));
            }
            return MR;
        }
        /// <summary>
        /// 计算方差信号
        /// </summary>
        /// <param name="R">rough数据</param>
        /// <param name="MR">MR数据</param>
        /// <param name="varianceblocklength">方差点数</param>
        /// <returns></returns>
        public static List<double> varianceSignalCal(List<double> R, List<double> MR, int varianceblocklength)
        {
            List<double> V = new List<double>();
            for (int i = 0; i < R.Count; i++)
            {
                var half = varianceblocklength / 2;
                double t = 0;
                int lessthan = 0;
                for (int j = i - half; j <= i + half; j++)
                {
                   
                    if ((j) < 0)
                    {
                        //t += Math.Pow((R[0] - MR[0]), 2);
                        t += 0;
                        lessthan++;
                    }
                    else
                    {
                        if (j >= R.Count)
                        {
                            //t += Math.Pow((R[R.Count - 1] - MR[MR.Count - 1]), 2);
                            t += 0;
                            lessthan++;
                        }
                        else
                        {
                            t += Math.Pow(R[j] - MR[j],2);
                        }
                    }

                }
                V.Add(t /( varianceblocklength-1 - lessthan));
            }
            return V;
        }
        /// <summary>
        /// 检测spike点，大于最大方差百分比的即为spike
        /// </summary>
        /// <param name="variance"></param>
        /// <param name="percentage">最大方差的百分比</param>
        /// <returns></returns>
        public static List<int> detectSpikes(List<double> variance,double percentage)
        {
           var threshold= variance.Max()*percentage;
            List<int> spikesindex = new List<int>();
            for (int i = 0; i < variance.Count; i++)
            {
                if (variance[i] > threshold)
                {
                    spikesindex.Add(i);
                }
            }
            return spikesindex; 
        }

        /// <summary>
        /// 线性插值
        /// </summary>
        /// <param name="value1">插值起始点</param>
        /// <param name="value2">插值终止点</param>
        /// <param name="numberOfInterpolations">插值数量</param>
        /// <returns></returns>
        public static double[] linearInterpolation(double value1, double value2, int numberOfInterpolations)
        {
            double[] interpolations = new double[numberOfInterpolations ];
            double step = (value2 - value1) / (numberOfInterpolations + 1);

            //interpolations[0] = value1;
            //interpolations[numberOfInterpolations + 1] = value2;

            for (int i = 0; i < numberOfInterpolations; i++)
            {
                interpolations[i] = value1 + (step * i);
            }

            return interpolations;
        }
        /// <summary>
        /// 将输入的List<int>数据中连续的数字分组输出
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static List<List<int>> GroupConsecutiveNumbers(List<int> numbers)
        {
            List<List<int>> result = new List<List<int>>();

            if (numbers.Count == 0)
            {
                return result;
            }
            //创建了一个名为currentGroup的List<int>类型变量，并将其中的第一个元素设置为numbers列表中的第一个元素。
            List<int> currentGroup = new List<int>() { numbers[0] };
            //这部分代码使用循环遍历numbers列表中的元素。在每次循环中，它检查当前元素是否与前一个元素相差1，
            //如果是，则将当前元素添加到currentGroup中。如果不是，则说明当前元素与前一个元素不连续，
            //需要将currentGroup添加到结果列表result中，并重新创建一个新的currentGroup来存储下一个连续的数字。
            for (int i = 1; i < numbers.Count; i++)
            {
                if (Math.Abs(numbers[i] - numbers[i - 1] )<=8)
                {
                    currentGroup.Add(numbers[i]);
                    
                }
                else
                {
                    result.Add(currentGroup);
                    currentGroup = new List<int>() { numbers[i] };
                }
            }

            result.Add(currentGroup);

            return result;
        }
        /// <summary>
        /// 把每组的不连续的数字改为连续的数字
        /// </summary>
        /// <param name="groups"></param>
        public static List<List<int>> MergeConsecutiveGroups(List<List<int>> groups)
        {
            foreach (var g in groups)
            {
                for (int i = 0; i < g.Count; i++)
                {
                    if (g[g.Count - 1] - g[0] + 1 != g.Count)
                    {
                       var cv= Enumerable.Range(g[0], g[g.Count - 1] - g[0] + 1).ToArray();
                        g.Clear();
                        g.AddRange(cv);
                    }
                }
            }
            return groups;
            
        }

        public static List<List<double>> getInterceptvalue(List<double> original, List<List<int>> index)
        {
            List<List<double>> nospike =new List<List<double>>();
            for (int i = 0; i < index.Count; i++)
            {
                var interceptcount = index[i].Count;//插值的数量
                //不考虑原数据第一个数或者最后一个数为spike的情况
                if (index[i][0]> 0 && index[i][index[i].Count - 1]+1< original.Count)
                {
                    List<double> list=new List<double>();
                    var firstdata = original[index[i][0] - 1];//拿到插值的起点数
                    var lastdata = original[index[i][index[i].Count - 1] + 1];//拿到插值的终点数
                    
                    var newdata=linearInterpolation(firstdata,lastdata, interceptcount);
                    foreach (var item in newdata)
                    {
                        list.Add(item);
                    }
                    nospike.Add(list);
                }
                else
                {
                    //如果是第一个或者最后一个为spike则原始数据不动
                    List<double> list = new List<double>();
                    for (int j = 0; j < interceptcount; j++)
                    {
                        list.Add(original[j]);
                    }
                    nospike.Add(list);
                }
            }
            return nospike;
        }

        /// <summary>
        /// 源数据根据spike的index改为插值
        /// </summary>
        /// <param name="original"></param>
        /// <param name="interceptvalue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static List<double> removeSpike( List<double> original, List<List<double>> interceptvalue, List<List<int>> index)
        {
            for (int i = 0; i < index.Count; i++)
            {
                for (int j = 0; j < index[i].Count; j++)
                {
                 
                    original[index[i][j]] = interceptvalue[i][j];
                }

            }
            return original;
        }

    }



}
