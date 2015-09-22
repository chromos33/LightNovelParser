using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightNovelHTMLParser
{
    public class Chapter
    {
        private string htmllink;
        public string Link
        {
            get { return htmllink; }
            set { htmllink = value; }
        }
        private string linktitle;
        public string Title
        {
            get { return linktitle; }
            set { linktitle = value; }
        }


        public Chapter(string Link, string Title)
        {
            htmllink = Link;
            linktitle = Title;
        }

    }
}
