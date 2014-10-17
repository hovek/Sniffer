using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Sniffer.Sniffs.Deki
{
    public class DekiUserStateDictionary : Dictionary<string, DekiUserState>
    {
        public event EventHandler OnDestruction;

        public DekiUserStateDictionary()
        {

        }

        ~DekiUserStateDictionary()
        {
            if (OnDestruction != null)
            {
                OnDestruction(this, EventArgs.Empty);
            }
        }
    }

    public class DekiUserState
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

        public DekiUserState()
        {
            PreviousPosts = new BlockingCollection<Post>();
            PostsToSend = new BlockingCollection<Post>();
        }
    }
}
