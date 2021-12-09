using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationSqlStorage.Models
{
    public class Email_Campaign_SendItemContent
    {
        public int Id { set; get; }
        public Guid RefUid { set; get; }
        public string ContentHtml { set; get; }
        public string BodyText { set; get; }
        public DateTime DateCreated { set; get; }
        public int CampaignType { set; get; }
        public byte[] Version { set; get; }
    }
}
