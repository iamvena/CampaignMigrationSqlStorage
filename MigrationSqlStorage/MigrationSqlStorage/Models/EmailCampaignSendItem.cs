using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationSqlStorage.Models
{
    public class EmailCampaignSendItem
    {
        public ObjectId Id { set; get; }
        public Guid Uid { set; get; }
        public int? CustomerId { set; get; }
        public int CampaignId { set; get; }
        public DateTime DateCreated { set; get; }
        public DateTime? DateLastQueued { set; get; }
        public DateTime? DateProcessed { set; get; }
        public CampaignItemStatus CampaignItemStatus { set; get; }
        public string Subject { set; get; }
        public string From { set; get; }
        public string To { set; get; }
        public string BodyHtml { set; get; }
        public string BodyText { set; get; }
        public string Message { set; get; }
        public Guid? SenderId { set; get; }
        public int? StoreId { set; get; }
    }

    public enum CampaignItemStatus
    {
        Sent = 1,
        Duplicate = 2,
        Error = 3,
        Waiting = 50,
        InvalidEmail = 60,
        LowSender = 70,
        BounceEmail = 80
    }

    public enum CampaignTypeEnum
    {
        Campaign = 1,
        PriceChangeAutoMailer = 2,
        ShowroomVisitAutoMailer = 3
    }
}
