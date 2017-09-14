using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.SEC.Api
{
    public class SubmissionFolderInfo
    {
        public SubmissionFolderInfo(string name, DateTime lastModifed)
        {
            Name = name;
            LastModified = lastModifed;
        }

        public string Name
        {
            get;
            set;
        }

        public DateTime LastModified
        {
            get;
            set;
        }
    }

    public class Submissions
    {
        public Submissions(string cik = null)
        {
            Folders = new List<SubmissionFolderInfo>();
            TimeStamp = DateTime.UtcNow;
            CIK = cik;
        }

        public string CIK
        {
            get;
            set;
        }

        public DateTime TimeStamp
        {
            get;
            set;
        }


        public List<SubmissionFolderInfo> Folders
        {
            get;
            set;
        }
    }

    public class SubmissionFileInfo
    {
        public SubmissionFileInfo(string name, DateTime lastModified, UInt32 size = 0)
        {
            Name = name;
            LastModifed = lastModified;
            Size = size;
        }

        public string Name
        {
            get;
            set;
        }

        public DateTime LastModifed
        {
            get;
            set;
        }

        public UInt32 Size
        {
            get;
            set;
        }

    }

    public class Submission
    {
        public Submission(string name, DateTime submitted)
        {
            Name = name;
            Submitted = submitted;
            Files = new List<SubmissionFileInfo>();
        }

        public string Name
        {
            get;
            set;
        }

        public DateTime Submitted
        {
            get;
            set;
        }

        public List<SubmissionFileInfo> Files
        {
            get;
            set;
        }
    }

    public class SubmissionFile
    {
        public SubmissionFile(string name)
        {
            Name = name;
            Content = new List<byte>();
        }

        public string Name
        {
            get;
            set;
        }

        public List<byte> Content
        {
            get;
            set;
        }
    }
}
