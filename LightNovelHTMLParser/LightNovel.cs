using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightNovelHTMLParser
{
    public class LightNovel
    {
        private string htmllink;
        public string Link {
            get { return htmllink; }
            set { htmllink = value; }
        }
        private string linktitle;
        public string Title
        {
            get { return linktitle; }
            set { linktitle = value; }
        }

        private List<Chapter> chapters;

        public void addChapter(Chapter flup)
        {
            chapters.Add(flup);
        }
        public List<Chapter> Chapter
        {
            get { return chapters; }
        }

        public LightNovel(string Link,string Title)
        {
            chapters = new List<Chapter>();
            htmllink = Link;
            linktitle = Title;
        }
        public override string ToString()
        {
            return Title;
        }
    }
}
