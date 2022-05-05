using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandyControl.Controls;

namespace TodoNotes.Data
{
    public class User
    {
        public string Id;
        public string Name;
        public int Level;

        public User(string id)
        {
            Id = id;
        }
    }
}
