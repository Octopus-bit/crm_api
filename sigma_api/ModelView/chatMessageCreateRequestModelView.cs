using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.ModelView
{
    public class chatMessageCreateRequestModelView
    {
        public int ConversationId { get; set; }
        public bool HasFile { get; set; } = false;
        public string Text { get; set; }
        public string FormName { get; set; } = null;
        public int FormId { get; set; } = 0;
    }
}
