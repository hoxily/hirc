using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hirc
{
    class MessageReceiveEvent : EventArgs
    {
        private string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }
        }
        public MessageReceiveEvent(string message)
        {
            _Message = message;
        }
    }
}
