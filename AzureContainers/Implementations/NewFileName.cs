using System;

namespace RerouteBlobs.Implementations
{
    public class NewFileName
    {
        public string ApplicantId { get; }
        public string ProofType { get; }
        public string Date { get; }
        public string Extension { get; }
        public Guid Guid { get; }
        public string FileName { get; }


        public NewFileName(string applicantId, string proofType, string date, string extension)
        {
            ApplicantId = applicantId;
            ProofType = proofType;
            Date = date;
            Extension = extension;
            Guid = Guid.NewGuid();
            FileName = $"{ApplicantId}-{ProofType}-{Date}-{Guid}.{Extension}";
        }
    }
}