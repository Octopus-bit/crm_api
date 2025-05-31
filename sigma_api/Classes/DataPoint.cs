using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Classes
{
    public class DataPoint
    {
        public int CST { get; set; }
        public float CLV { get; set; }
        public float Ave { get; set; }
        public int Cluster { get; set; }
        public float L { get; set; }
        public float R { get; set; }
        public float F { get; set; }
        public decimal M { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string TitleCity { get; set; }
        public string TitleOstan { get; set; }
        public DataPoint(int Cluster, float CLV, int CST, float Ave,
            decimal M, float F, float R, float L, string FullName, string Code, string TitleOstan, string TitleCity)
        {
            CST = CST;
            CLV = CLV;
            Cluster = Cluster;
            FullName = FullName;
            Code = Code;
            M = M;
            F = F;
            R = R;
            L = L;
            TitleOstan = TitleOstan;
            TitleCity = TitleCity;
        }

        public DataPoint()
        {

        }

        public override string ToString()
        {
            return string.Format("{{{0},{1}}}", Cluster.ToString("f" + 1), CST.ToString("f" + 1), CLV.ToString("f" + 1));
        }
    }

}
