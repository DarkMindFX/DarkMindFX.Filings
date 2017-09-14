using ServiceStack;
using ServiceStack.Text;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace DMFX.SEC.Api
{
    public class SECApi
    {
        private static string BaseURL = "https://www.sec.gov/";

        #region Json responses

        [DataContract]
        class ArchivesEdgarDirectoryItem
        {
            [DataMember(Name="name")]
            public string name
            {
                get;
                set;
            }

            [DataMember(Name="last-modified")]
            public string last_modified
            {
                get;
                set;
            }

            [DataMember(Name="type")]
            public string type
            {
                get;
                set;
            }

            [DataMember(Name="size")]
            public string size
            {
                get;
                set;
            }
        }

        [DataContract]
        class ArchivesEdgarDirectory
        {
            [DataMember(Name="item")]
            public List<ArchivesEdgarDirectoryItem> item
            {
                get;
                set;
            }

            [DataMember(Name="name")]
            public string name
            {
                get;
                set;
            }

            [DataMember(Name="parent-dir")]
            public string parent_dir
            {
                get;
                set;
            }
        }

        [DataContract]
        class ArchivesEdgarDataCIKResponse
        {
            [DataMember(Name="directory")]
            public ArchivesEdgarDirectory directory
            {
                get; set;
            }
        }

        [DataContract]
        class ArchivesEdgarDataCIKSubmissionResponse
        {
            [DataMember(Name = "directory")]
            public ArchivesEdgarDirectory directory
            {
                get; set;
            }
        }
        #endregion


        // Call: /Archives/edgar/data/<CIK>
        public Submissions ArchivesEdgarDataCIK(string cik)
        {
            string Command = "/Archives/edgar/data/{0}/index.json";

            Submissions submissions = null;

            using (var client = new JsonServiceClient(BaseURL))
            {
                string request = string.Format(Command, cik);
                ArchivesEdgarDataCIKResponse model = client.Get<ArchivesEdgarDataCIKResponse>(request);

                submissions = Convert(model);
                if (submissions != null)
                {
                    submissions.CIK = cik;
                    submissions.TimeStamp = DateTime.UtcNow;
                }
            }

            return submissions;
        }

        // Call: /Archives/edgar/data/<CIK>/<Submission Access Number>
        public Submission ArchivesEdgarDataCIKSubmission(string cik, string accessNumber)
        {
            string Command = "/Archives/edgar/data/{0}/{1}/index.json";

            Submission submission = null;

            using (var client = new JsonServiceClient(BaseURL))
            {
                string request = string.Format(Command, cik, accessNumber);
                ArchivesEdgarDataCIKSubmissionResponse model = client.Get<ArchivesEdgarDataCIKSubmissionResponse>(request);

                submission = Convert(model);
                submission.Name = accessNumber;
            }

            return submission;
        }

        // Call: /Archives/edgar/data/<CIK>/<Submission Access Number>/<File Name>
        public SubmissionFile ArchivesEdgarDataCIKSubmissionFile(string cik, string accessNumber, string fileName)
        {
            string Command = "/Archives/edgar/data/{0}/{1}/{2}";

            SubmissionFile submission = null;

            using (var client = new JsonServiceClient(BaseURL))
            {
                string request = string.Format(Command, cik, accessNumber, fileName);
                byte[] fileContent = client.Get<byte[]>(request);

                submission = Convert(fileName, fileContent);

            }

            return submission;
        }

        #region Support methods
        private Submissions Convert(ArchivesEdgarDataCIKResponse model)
        {
            Submissions submissions = new Submissions();

            foreach (var item in model.directory.item)
            {
                SubmissionFolderInfo folder = new SubmissionFolderInfo(item.name, !string.IsNullOrEmpty(item.last_modified) ? DateTime.Parse(item.last_modified) : DateTime.MinValue);
                submissions.Folders.Add(folder);
            }

            return submissions;
        }

        private Submission Convert(ArchivesEdgarDataCIKSubmissionResponse model)
        {
            Submission submission = new Submission(model.directory.name, DateTime.MinValue);

            foreach (var item in model.directory.item)
            {
                SubmissionFileInfo folder = new SubmissionFileInfo(item.name, !string.IsNullOrEmpty(item.last_modified) ? DateTime.Parse(item.last_modified) : DateTime.MinValue, !string.IsNullOrEmpty(item.size) ? UInt32.Parse(item.size) : 0);
                submission.Files.Add(folder);
            }

            return submission;
        }

        private SubmissionFile Convert(string fileName, byte[] fileContent)
        {
            SubmissionFile file = new SubmissionFile(fileName);

            file.Content.AddRange(fileContent);

            return file;
        }
        #endregion

    }
}
