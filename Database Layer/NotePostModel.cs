using System;
using System.Collections.Generic;
using System.Text;

namespace Database_Layer
{
    public class NotePostModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string BGColor { get; set; }
        public bool IsArchive { get; set; }
        public bool IsReminder { get; set; }
        public bool IsPin { get; set; }
        public bool IsTrash { get; set; }
    }
}
