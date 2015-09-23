using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightNovelHTMLParser
{
    public class Chapter
    {
        public bool downloadscheduled = false;
        private string htmllink;
        public string file;
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
        public override string ToString()
        {
            return Title;
        }
        //html is the content that lates goes into epub
        public string html;
    }
}
