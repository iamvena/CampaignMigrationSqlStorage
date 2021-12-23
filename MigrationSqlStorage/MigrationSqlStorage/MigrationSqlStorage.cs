using MacHaik.Data;
using MigrationSqlStorage.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationSqlStorage
{
    public class MigrationSqlStorage
    {
        private readonly CampaignSendItemContentSqlHelper _contentSqlHelper;

        public MigrationSqlStorage()
        {
            _contentSqlHelper = new CampaignSendItemContentSqlHelper();
        }

        public void Execute()
        {
            //Console.WriteLine("Retrieving stores...");

            //var stores = GetAllActiveStores();

            Console.WriteLine("Enter storeId...");

            int.TryParse(Console.ReadLine(), out int storeId);

            Console.WriteLine($"Inputted storeId - {storeId} ");

            Console.WriteLine($"Retrieve campaign headers by storeId - {storeId} ");

            var lastHeaderId = 0;

            var any = true;

            var totalHeaderIds = 0;

            var headerIds = new List<int>();

            while (any)
            {
                headerIds = GetTop500ActiveCampaignsByStore(storeId, lastHeaderId);

                if (!headerIds.Any()) break;

                totalHeaderIds += headerIds.Count();

                foreach (var headerId in headerIds)
                {
                    MigrateMongoDbRecord(headerId, storeId);
                }

                lastHeaderId = headerIds.OrderByDescending(d => d).FirstOrDefault();

                using (var context = new MacHaikCrmEntities())
                {
                    any = context.Email_Campaign_Header
                        .Any(r => r.Id > lastHeaderId);
                }

                Console.WriteLine($"Total updated records : {totalHeaderIds} ," +
                    $"StoreId : {storeId} ");
            }

            Console.WriteLine($"Finished update. " +
                    $"HeaderIdCount : {totalHeaderIds} ," +
                    $"StoreId : {storeId} ");
        }

        private void MigrateMongoDbRecord(int campaignId, int storeId)
        {
            var page = 1;

            var rpp = 500;

            var totalRecords = 0;

            var sendItems = GetTop500MongoDbCampaign(page, rpp, campaignId);

            if (!sendItems.Any())
            {
                Console.WriteLine($"No mongodb campaign record to retrieve. " +
                    $"HeaderId : {campaignId} ");

                return;
            }

            while (sendItems.Any())
            {
                totalRecords += sendItems.Count();

                foreach (var item in sendItems)
                {
                    MigrateItem(item, storeId);

                    Console.WriteLine($"Done. " +
                        $"Migrated Uid - {item.Uid} , " +
                        $"CampaignId - {item.CampaignId} ");
                }

                page++;

                sendItems = GetTop500MongoDbCampaign(page, rpp, campaignId);

                Console.WriteLine($"Total migrated from mongodb records : {totalRecords} ," +
                    $"StoreId : {storeId} ," +
                    $"CampaignId : {campaignId} ");
            }

            Console.WriteLine($"Finished migrate. " +
                   $"HeaderId : {campaignId} ," +
                   $"StoreId : {storeId} ," +
                   $"Count: {totalRecords} ");
        }

        private void MigrateItem(EmailCampaignSendItem item, int storeId)
        {
            var uid = item.Uid;

            using (var context = new MacHaikCrmEntities())
            {
                var emailCampaignSqlStorage = context.Email_Campaign_SendItems.FirstOrDefault(d => d.Uid == uid);

                if (emailCampaignSqlStorage == null)
                {
                    emailCampaignSqlStorage = new Email_Campaign_SendItem
                    {
                        CampaignId = item.CampaignId,
                        Uid = uid,
                        CustomerId = item.CustomerId,
                        DateCreated = item.DateCreated,
                        DateProcessed = item.DateProcessed,
                        DateLastQueued = item.DateLastQueued,
                        SendStatusId = (int)item.CampaignItemStatus,
                        Subject = item.Subject,
                        To = item.To,
                        From = item.From,
                        Message = item.Message,
                        SenderId = item.SenderId,
                        StoreId = storeId
                    };

                    context.Email_Campaign_SendItems.Add(emailCampaignSqlStorage);
                }

                var campaignContent = _contentSqlHelper.GetContentByRefUid(uid);

                if (!campaignContent.HasValue)
                {
                    _contentSqlHelper.Add(new Email_Campaign_SendItemContent
                    {
                        BodyText = item.BodyText,
                        ContentHtml = item.BodyHtml,
                        CampaignType = (int)CampaignTypeEnum.Campaign,
                        DateCreated = DateTime.Now,
                        RefUid = uid
                    });
                }

                context.SaveChanges();
            }
        }

        private IEnumerable<EmailCampaignSendItem> GetTop500MongoDbCampaign(int page, int rpp, int campaignId)
        {
            var q = DefaultConnectionProvider.GetInstance()
                .GetCollection<EmailCampaignSendItem>(nameof(EmailCampaignSendItem))
                .AsQueryable();

            var skip = ((page - 1) * rpp);

            var sendItems = q.Where(d => d.CampaignId == campaignId)
                .Skip(skip)
                .Take(rpp)
                .ToList();

            return sendItems;
        }

        private List<StoreModel> GetAllActiveStores()
        {
            using (var context = new MacHaikCrmEntities())
            {
                var stores = context.StorePreferences
                        .Where(c => c.Name == "Store.Settings.Active" &&
                               c.Value == "1")
                        .Select(c => new StoreModel
                        {
                            StoreId = c.StoreId.Value,
                            StoreName = c.Store.Name
                        }).ToList();

                return stores;
            }
        }

        private List<int> GetTop500ActiveCampaignsByStore(int storeId, int lastHeaderId)
        {
            var campaignHeaders = new List<int>();

            using (var context = new MacHaikCrmEntities())
            {
                if (lastHeaderId == 0)
                {
                    campaignHeaders = context.Email_Campaign_Header
                       .Where(c => c.StoreId == storeId &&
                              c.IsHidden != true)
                       .OrderBy(r => r.Id)
                       .Take(500)
                       .Select(c => c.Id)
                       .ToList();
                }
                else
                {
                    campaignHeaders = context.Email_Campaign_Header
                       .Where(c => c.StoreId == storeId &&
                              c.IsHidden != true)
                       .OrderBy(r => r.Id)
                       .Where(d => d.Id > lastHeaderId)
                       .Take(500)
                       .Select(c => c.Id)
                       .ToList();
                }
            }

            return campaignHeaders;
        }
    }
}
