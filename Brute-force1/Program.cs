﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetOneHopNode;
using System.Collections;
using System.IO;

namespace Brute_force1
{

    class Program
    {
        static void Main(string[] args)
        {
            KeyValuePair<string, UInt64> node1;
            KeyValuePair<string, UInt64> node2;
            //    ///一跳测试
            //    ///id--id
            //node1 = new KeyValuePair<string, UInt64>("Id", 2140251882);
            //node2 = new KeyValuePair<string, UInt64>("Id", 2143554828);
            //Solution.solve(node1, node2);
            /////id--AA.AuId
            //node1 = new KeyValuePair<string, UInt64>("Id", 2140251882);
            //node2 = new KeyValuePair<string, UInt64>("AA.AuId", 2145115012);
            //Solution.solve(node1, node2);
            /////AA.AuId--id
            //node1 = new KeyValuePair<string, UInt64>("AA.AuId", 2145115012);
            //node2 = new KeyValuePair<string, UInt64>("Id", 2140251882);
            //Solution.solve(node1, node2);
            /////AA.AuId--AA.AuId
            //node1 = new KeyValuePair<string, UInt64>("AA.AuId", 2145115012);
            //node2 = new KeyValuePair<string, UInt64>("AA.AuId", 2145115015);
            //Solution.solve(node1, node2);
            ///id to id  1970381522  to 2162351023  大家算出来多少？ 36
            ///[id, AA.AuId]=[2273736245,2094437628]有多少对啊？ 41  19
            ///大家看看这组数据有多少边，我这里出来2584条，感觉有点虚：2126125555，2153635508
            ///两跳测试
            ///id--id
            node1 = new KeyValuePair<string, UInt64>("Id", 2126125555);
            node2 = new KeyValuePair<string, UInt64>("Id", 2060367530);

            List<List<UInt64>> pathList= Solution.solve(node1, node2);
            ///三跳测试
            ///id--id
            //node1 = new KeyValuePair<string, UInt64>("Id", 2094437628);
            //node2 = new KeyValuePair<string, UInt64>("Id", 2088397685);
            //Solution.solve(node1, node2);
            Console.ReadLine();
        }
    }
    public class Solution
    {
        private class SortedSetComparer : IComparer<KeyValuePair<string, UInt64>>
        {
            public int Compare(KeyValuePair<string, UInt64> x, KeyValuePair<string, UInt64> y)
            {
                if (x.Key!=y.Key)
                    return x.Key.CompareTo(y.Key);
                else
                    return x.Value.CompareTo(y.Value);
            }
        }
        public static int pairCompare(KeyValuePair<string, UInt64> o1, KeyValuePair<string, UInt64> o2)
        {
                return (int)(o1.Value - o2.Value);
        }

        public static SortedSet<KeyValuePair<string, UInt64>> GetOneHopNode(KeyValuePair<string, UInt64> nodeid)
        {
            //通过id获取one-hop节点集合
            SortedSet<KeyValuePair<string, UInt64>> retval;
            /*
             * 这里是获取id 的过程，基本上需要两步，第一步通过id获取该id的属性，
             * 第二步通过属性获取有此属性的一跳节点id集合
             */
            /*
            *  获取1-Hop的node list
            *  by Esdreal
            */
            GetOneHopNodeClass getOneHop = new GetOneHopNodeClass();
            retval = getOneHop.getNode(nodeid);
            return retval;
        }

        public static Dictionary<KeyValuePair<string, UInt64>, SortedSet<KeyValuePair<string, UInt64>>> GetTwoHopNode(SortedSet<KeyValuePair<string, UInt64>> hop1set)
        {
            Dictionary<KeyValuePair<string, UInt64>, SortedSet<KeyValuePair<string, UInt64>>> dic = new Dictionary<KeyValuePair<string, ulong>, SortedSet<KeyValuePair<string, ulong>>>();
            //通过id获取one-hop节点集合
            long start, end;
            foreach (KeyValuePair<string, UInt64> nodeid in hop1set)
            {
                ///Console.WriteLine("find:{0}:{1}", nodeid.Key, nodeid);
                start = DateTime.Now.Ticks;
                GetOneHopNodeClass getOneHop = new GetOneHopNodeClass();
                SortedSet<KeyValuePair<string, UInt64>> tmp = getOneHop.getNode(nodeid);
                end = DateTime.Now.Ticks;
                /// Console.WriteLine("{0}:获取hop1set花费时间", (end - start) / 1000);
                dic.Add(nodeid, tmp);
            }
            return dic;
        }
        /// <summary>
        /// 并行获取2-hopNodeLIST
        /// </summary>
        /// <param name="hop1set"></param>
        /// <returns></returns>
        public static Dictionary<KeyValuePair<string, UInt64>, SortedSet<KeyValuePair<string, UInt64>>> GetTwoHopNodeAsync(SortedSet<KeyValuePair<string, UInt64>> hop1set)
        {
            Dictionary<KeyValuePair<string, UInt64>, SortedSet<KeyValuePair<string, UInt64>>> dic = new Dictionary<KeyValuePair<string, ulong>, SortedSet<KeyValuePair<string, ulong>>>();
            //通过id获取one-hop节点集合
            long start, end;
            //
            List<Task> taskList = new List<Task>();
            //
            GetOneHopNodeClass getOneHop = new GetOneHopNodeClass();
            foreach (KeyValuePair<string, UInt64> nodeid in hop1set)
            {
                Task t=Task.Run(()=>{
                    //Console.WriteLine("tofind:{0}", nodeid);
                    //start = DateTime.Now.Ticks;
                    SortedSet<KeyValuePair<string, UInt64>> tmp = getOneHop.getNode(nodeid);
                    //end = DateTime.Now.Ticks;
                    //Console.WriteLine("{0}花费时间:{1},set大小:{2}", nodeid,(end - start) / 10000000, tmp.Count());
                    //if(nodeid.Key== "AA.AuId")
                    //    foreach (KeyValuePair<string, UInt64> tmpnode in tmp)
                    //        Console.WriteLine(tmpnode);
                    dic.Add(nodeid, tmp); });
                taskList.Add(t);
            }
            
            Task.WaitAll(taskList.ToArray());
            return dic;
        }
        public static void testGetTwoHopNodeAsync()
        {
            SortedSet<KeyValuePair<string, UInt64>> hop2set = new SortedSet<KeyValuePair<string, ulong>>(new SortedSetComparer());
            hop2set.Add(new KeyValuePair<string, ulong>("F.FId", 124657808));

            Dictionary<KeyValuePair<string, UInt64>, SortedSet<KeyValuePair<string, UInt64>>> hop3res = GetTwoHopNodeAsync(hop2set);
            
        }
        //public static void testGetThreeHopNodeAsync()
        //{
        //    KeyValuePair<string, UInt64> node2 = new KeyValuePair<string, ulong>("AA.AuId", 2273736245);
        //    SortedSet<KeyValuePair<string, UInt64>> hop2set = new SortedSet<KeyValuePair<string, ulong>>(new SortedSetComparer());     
        //    hop2set.Add(new KeyValuePair<string, ulong>("Id", 2094437628));
        //    hop2set.Add(new KeyValuePair<string, ulong>("Id", 2088397685));
        //    hop2set.Add(new KeyValuePair<string, ulong>("Id", 2054283902));
        //    SortedSet<KeyValuePair<string, UInt64>> hop3res = GetThreeHopNodeAsync(hop2set, node2);
        //    foreach (KeyValuePair<string, UInt64> lastnode in hop3res)
        //    {
        //        Console.WriteLine(lastnode);
        //    }
        //}
        public static SortedSet<KeyValuePair<string, UInt64>> GetThreeHopNodeAsync(SortedSet<KeyValuePair<string, UInt64>> hop2set, KeyValuePair<string, UInt64>dstNode)
        {
            SortedSet<KeyValuePair<string, UInt64>> res = new SortedSet<KeyValuePair<string, ulong>>(new SortedSetComparer());
            //通过id获取one-hop节点集合
            long start, end;
            //
            List<Task> taskList = new List<Task>();
            //
            GetOneHopNodeClass getOneHop = new GetOneHopNodeClass();
            foreach (KeyValuePair<string, UInt64> nodeid in hop2set)
            {
                if (dstNode.Key == "Id")
                {
                    if (nodeid.Key == "AA.AfId")
                        continue;
                }
                if (dstNode.Key == "AA.AuId")
                {
                    if (nodeid.Key != "Id"&& nodeid.Key != "AA.AfId")
                        continue;
                }
                Task t = Task.Run(() => {
                    //  Console.WriteLine("find:{0}:{1}", nodeid.Key, nodeid);
                    //  start = DateTime.Now.Ticks;
                    if(getOneHop.checkNodeWithCondition(nodeid, dstNode))
                    {
                        res.Add(nodeid);
                    }
                    //   end = DateTime.Now.Ticks;
                    //  Console.WriteLine("{0}:获取hop1set花费时间,{1},set大小:{2}", (end - start) / 1000000,nodeid, tmp.Count());
                    
                });
                taskList.Add(t);
            }

            Task.WaitAll(taskList.ToArray());
            return res;
        }
       
        /// <summary>
        /// </summary>
        /// <param name="node1">节点1</param>
        /// <param name="node2">节点2</param>
       
        public static List<List<UInt64>> solve(KeyValuePair<string, UInt64> node1, KeyValuePair<string, UInt64> node2)
        {
            List<List<UInt64>> ans=new List<List<UInt64>>();
            long count = 0;
            long start, end,start_;
            int chushu = 10000000;
            start_ = DateTime.Now.Ticks;
            GetOneHopNodeClass getOneHop = new GetOneHopNodeClass();
            SortedSet<KeyValuePair<string, UInt64>> lastset = getOneHop.getLastNode(node2);
            Console.WriteLine("lastset大小：{0}", lastset.Count);
            //foreach(KeyValuePair<string, UInt64> tmp in lastset)
            //    Console.WriteLine(tmp);
            ///step1:获取node1和node2是否存在一跳关系
            start = DateTime.Now.Ticks;
            SortedSet<KeyValuePair<string, UInt64>> hop1set = GetOneHopNode(node1);
            Console.WriteLine("hop1set大小：{0}", hop1set.Count);
            end = DateTime.Now.Ticks;
            if (lastset.Contains<KeyValuePair<string, UInt64>>(node1) == true)
            {
                //存在one-hop
                Console.WriteLine("{0}:存在one-hop", count++);
                Console.WriteLine("[{0},{1}]", node1, node2);
                //StreamWriter sw = new StreamWriter("1.txt", true);
                //sw.WriteLine("[{0},{1}]", node1, node2);
                //sw.Flush();
                //sw.Close();
                ans.Add(new List<ulong>() { node1.Value, node2.Value });
            }
            end = DateTime.Now.Ticks;
            Console.WriteLine("一跳全部花费{0}", (end - start) / chushu);
            ///step2:获取两跳关系
            /// 方式2：根据node1的一跳集合得到node1的两跳集合，看是否包含node2
            start = DateTime.Now.Ticks;
            Dictionary<KeyValuePair<string, UInt64>, SortedSet<KeyValuePair<string, UInt64>>> hop2dic = GetTwoHopNodeAsync(hop1set);
            end = DateTime.Now.Ticks;
            Console.WriteLine("二跳查询花费{0}", (end - start) / chushu);
            SortedSet<KeyValuePair<string, UInt64>> hop2set = new SortedSet<KeyValuePair<string, ulong>>(new SortedSetComparer());
            foreach (KeyValuePair<KeyValuePair<string, UInt64>, SortedSet<KeyValuePair<string, UInt64>>> kv in hop2dic)
            {
                foreach (KeyValuePair<string, UInt64> tmppair in kv.Value)
                    hop2set.Add(tmppair);
                if (kv.Value.Contains(node2))
                {
                    Console.WriteLine("{0}:存在two-hop", count++);
                    Console.WriteLine("[{0},{1},{2}]", node1, kv.Key, node2);
                    ans.Add(new List<ulong> { node1.Value, kv.Key.Value, node2.Value });
                }
            }
            end = DateTime.Now.Ticks;
            Console.WriteLine("二跳全部花费{0}:set大小：{1}", (end - start) / chushu, hop2set.ToList().Capacity);
            //step3:获取三跳关系
            //方式1：从hop2set进行搜索，看是否包含node2
            //start = DateTime.Now.Ticks;
            //SortedSet<KeyValuePair<string, UInt64>> hop3res = GetThreeHopNodeAsync(hop2set, node2);
            //end = DateTime.Now.Ticks;
            //Console.WriteLine("三跳查询花费{0}", (end - start) / chushu);
            //foreach (KeyValuePair<string, UInt64> lastnode in hop3res)
            //{
            //    foreach (KeyValuePair<KeyValuePair<string, UInt64>, SortedSet<KeyValuePair<string, UInt64>>> kv in hop2dic)
            //    {
            //        if (kv.Value.Contains(lastnode))
            //        {
            //            Console.WriteLine("{0}:存在three-hop", count++);
            //            Console.WriteLine("[{0},{1},{2},{3}]", node1, kv.Key, lastnode, node2);
            //        }
            //    }
            //}
            //end = DateTime.Now.Ticks;
            //Console.WriteLine("三跳全部花费{0}", (end - start) / chushu);
            ///方式2：查找hop2set和lastset的交集
            start = DateTime.Now.Ticks;
            int i = 0, j = 0;
            foreach (KeyValuePair<KeyValuePair<string, UInt64>, SortedSet<KeyValuePair<string, UInt64>>> kv in hop2dic)
            {
                foreach (KeyValuePair<string, UInt64>lastNode in lastset)
                {
                   
                    if (kv.Value.Contains(lastNode))
                    {
                        Console.WriteLine("{0}:存在three-hop", count++);
                        Console.WriteLine("[{0},{1},{2},{3}]", node1, kv.Key, lastNode, node2);
                        ans.Add(new List<ulong>() { node1.Value, kv.Key.Value, lastNode.Value, node2.Value });
                    }
                }
               
            }
            end = DateTime.Now.Ticks;
            Console.WriteLine("三跳全部花费{0}", (end - start) / chushu);
            Console.WriteLine("总时间{0}", (end - start_) / chushu);
            return ans;
        }
    }
}
