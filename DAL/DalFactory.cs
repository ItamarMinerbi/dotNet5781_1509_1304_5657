using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi
{
    public enum Options { Object, Xml }
    public static class DalFactory
    {
        public static IDAL GetDal(Options type)
        {
            switch (type)
            {
                case Options.Object:
                    return DAL.DAL.Instance;
                case Options.Xml:
                    return DAL.DalXml.Instance;
                default:
                    return null;
            }
        }
    }
}
