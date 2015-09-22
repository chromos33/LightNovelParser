using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightNovelHTMLParser
{
    public class Webpage
    {
        public string html;
        public string link;
        public int id;
        public List<LightNovel> Lightnovels;
        public Webpage()
        {
            Lightnovels = new List<LightNovel>();
        }
    }
}
