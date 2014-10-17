using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Sniffer.Sniffs.Veky
{
    public class VekyUserStateDictionary : Dictionary<string, VekyUserState>
    {
        public event EventHandler OnDestruction;

        public VekyUserStateDictionary()
        {

        }

        ~VekyUserStateDictionary()
        {
            if (OnDestruction != null)
            {
                OnDestruction(this, EventArgs.Empty);
            }
        }
    }

    public class VekyUserState
    {
        public BlockingCollection<Post> PreviousPosts;
        public List<Post> _PreviousPosts
        {
            get
            {
                return new List<Post>(PreviousPosts);
            }
            set
            {
                PreviousPosts = new BlockingCollection<Post>();
                foreach (Post post in value)
                {
                    PreviousPosts.Add(post);
                }
            }
        }

        public BlockingCollection<Post> PostsToSend;
        public List<Post> _PostsToSend
        {
            get
            {
                return new List<Post>(PostsToSend);
            }
            set
            {
                PostsToSend = new BlockingCollection<Post>();
                foreach (Post post in value)
                {
                    PostsToSend.Add(post);
                }
            }
        }

        public VekyUserState()
        {
            PreviousPosts = new BlockingCollection<Post>();
            PostsToSend = new BlockingCollection<Post>();
        }
    }
}
