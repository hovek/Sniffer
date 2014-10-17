using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Sniffer.Sniffs.Njuskalo
{
    public class UserStateDictionary : Dictionary<string, UserState>
    {
        public event EventHandler OnDestruction;

        public UserStateDictionary()
        {

        }

        ~UserStateDictionary()
        {
            if (OnDestruction != null)
            {
                OnDestruction(this, EventArgs.Empty);
            }
        }
    }

    public class UserState
    {
        public BlockingCollection<Ad> PreviousElements;
        public List<Ad> _PreviousElements
        {
            get
            {
                return new List<Ad>(PreviousElements);
            }
            set
            {
                PreviousElements = new BlockingCollection<Ad>();
                foreach (Ad element in value)
                {
                    PreviousElements.Add(element);
                }
            }
        }

        public BlockingCollection<Ad> ElementsToSend;
        public List<Ad> _ElementToSend
        {
            get
            {
                return new List<Ad>(ElementsToSend);
            }
            set
            {
                ElementsToSend = new BlockingCollection<Ad>();
                foreach (Ad element in value)
                {
                    ElementsToSend.Add(element);
                }
            }
        }

        public UserState()
        {
            PreviousElements = new BlockingCollection<Ad>();
            ElementsToSend = new BlockingCollection<Ad>();
        }
    }
}
