using Dapper;
using MigrationSqlStorage.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationSqlStorage
{
    public class CampaignSendItemContentSqlHelper
    {
        public void Add(Email_Campaign_SendItemContent entity)
        {
            var insertQuery = @"INSERT INTO dbo.Email_Campaign_SendItemContent 
                                (
                                     RefUid,
                                     ContentHtml,
                                     BodyText,
                                     DateCreated,
                                     CampaignType
                                )
                                VALUES 
                                (
                                    @RefUid,
                                    @ContentHtml,
                                    @BodyText,
                                    @DateCreated,
                                    @CampaignType
                                 )";

            var parameters = new DynamicParameters();

            parameters.Add(nameof(Email_Campaign_SendItemContent.RefUid), entity.RefUid, DbType.Guid);
            parameters.Add(nameof(Email_Campaign_SendItemContent.ContentHtml), entity.ContentHtml, DbType.String);
            parameters.Add(nameof(Email_Campaign_SendItemContent.BodyText), entity.BodyText, DbType.String);
            parameters.Add(nameof(Email_Campaign_SendItemContent.DateCreated), entity.DateCreated, DbType.DateTime);
            parameters.Add(nameof(Email_Campaign_SendItemContent.CampaignType), entity.CampaignType, DbType.Int32);

            using (var con = CrmCampaignSqlConnection())
            {
                con.Execute(insertQuery, parameters);
            }
        }

        public void UpdateContent(Guid refUid, string bodyHtml, string bodyText)
        {
            var updateQuery = @"UPDATE dbo.Email_Campaign_SendItemContent 
                                SET ContentHtml = @contentHtml , BodyText = @bodyText
                                WHERE RefUid = @refUid";

            var parameters = new DynamicParameters();

            parameters.Add(nameof(Email_Campaign_SendItemContent.RefUid), refUid, DbType.Guid);
            parameters.Add(nameof(Email_Campaign_SendItemContent.ContentHtml), bodyHtml, DbType.String);
            parameters.Add(nameof(Email_Campaign_SendItemContent.BodyText), bodyText, DbType.String);

            using (var con = CrmCampaignSqlConnection())
            {
                con.Execute(updateQuery, parameters);
            }
        }

        public Guid? GetContentByRefUid(Guid refUid)
        {
            var query = @"SELECT 
                            RefUid, 
                         FROM dbo.Email_Campaign_SendItemContent 
                         WHERE RefUid = @refUid";

            using (var con = CrmCampaignSqlConnection())
            {
                var data = con.Query<Guid?>(query, new
                {
                    refUid = refUid
                }).FirstOrDefault();

                return data;
            }
        }

        public IDbConnection CrmCampaignSqlConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MacHaikCampaignDbContext"];

            return new SqlConnection(connectionString.ConnectionString);
        }
    }
}
