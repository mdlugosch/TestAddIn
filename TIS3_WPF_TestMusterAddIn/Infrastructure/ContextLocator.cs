using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIS3_WPF_TestMusterAddIn.Infrastructure
{
    public static class ContextLocator
    {
        private static IDictionary<string, WinTIS30db_entwModel.Honorarkraefte.WinTIS30db_entwEntities> contextList = new Dictionary<string, WinTIS30db_entwModel.Honorarkraefte.WinTIS30db_entwEntities>();

        public static void CreateContext(string guid)
        {
            WinTIS30db_entwModel.Honorarkraefte.WinTIS30db_entwEntities context = new WinTIS30db_entwModel.Honorarkraefte.WinTIS30db_entwEntities();
            if (context == null)
            {
                Debug.WriteLine("DB-Context ist null!");
                throw new ArgumentNullException("context");
            }

            if(guid!=null)
            contextList.Add(guid, context);
            else throw new ArgumentNullException("guid");
        }

        public static WinTIS30db_entwModel.Honorarkraefte.WinTIS30db_entwEntities GetContext(string guid)
        {
            if (contextList.ContainsKey(guid))
            {
                return contextList[guid];
            }
            else
            {
                CreateContext(guid);
                return contextList[guid];
            }
        }

    }
}
