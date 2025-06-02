using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using sigma_api.Classes;
using sigma_api.Models;
using sigma_api.ModelView;

namespace sigma_api.Controllers
{
    public class ChatController : Controller
    {
        private cls_public cp = new cls_public();

        public chatUserInfoResponseModelView response_users;
        public chatConversationInfoResponseModelView response_conversations;

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Users")]
        public string GetUsers([FromQuery] ListRequest request)
        {
            if (request.page_number < 1)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "شماره صفحه وارد شده یافت نشد");
            }
            ListResponse list = new ListResponse();
            List<chatUserInfoResponseModel> user = new List<chatUserInfoResponseModel>();
            int type = HttpContext.Items["UserType"].ToInt();
            int Pages = 1;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("Type",type),
                new SqlParameter("UserId",DBNull.Value)
            };
            DataTable dt_users = cp.select_row("API_ChatGetInfoUsers", parameters);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
            }
            if (dt_users.Rows.Count > 0)
            {
                Pages = dt_users.Rows.Count / 15;
                if (Pages < 1)
                    Pages = 1;
                if (dt_users.Rows.Count > 15)
                {
                    dt_users = dt_users.AsEnumerable().Skip((request.page_number - 1) * 15).CopyToDataTable();
                    dt_users = dt_users.AsEnumerable().Take(15).CopyToDataTable();
                }
                foreach (DataRow row in dt_users.Rows)
                {
                    user.Add(new chatUserInfoResponseModel()
                    {
                        id = row["us_id"].ToInt(),
                        Name = row["FullName"].ToString(),
                        phone = row["phone"].ToString(),
                        email = row["email"].ToString(),
                        avatar = row["avatar"].ToString(),
                        bio = row["us_bio"].ToString(),
                        unReadCount = row["us_unreadCount"].ToInt(),
                        lastseen = string.IsNullOrEmpty(row["us_lastSeen"].ToString()) ? null : Convert.ToDateTime(row["us_lastSeen"].ToString()),
                        statusName = row["StatusName"].ToString(),
                        status = row["us_status"].ToInt(),
                    });
                }
                list = new ListResponse()
                {
                    Pages = Pages,
                    Page_row = request.page_number,
                    List = user
                };
            }

            response_users = new chatUserInfoResponseModelView(user.Count <= 0 ? null : list);
            //API_ChatGetInfoUsers
            return JsonConvert.SerializeObject(response_users);
        }
        
        
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Users/{UserId?}")]
        public string GetUsers(int? UserId)
        {
            if (UserId == null)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "UserId ارائه نشده است.");
            }

            chatUserInfoResponseModel user = new chatUserInfoResponseModel();
            int type = HttpContext.Items["UserType"].ToInt();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("UserId",UserId)
            };
            DataTable dt_users = cp.select_row("API_ChatGetInfoUser", parameters);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
            }
            if (dt_users.Rows.Count > 0)
            {
                foreach (DataRow row in dt_users.Rows)
                {
                    user = new chatUserInfoResponseModel()
                    {
                        id = row["us_id"].ToInt(),
                        Name = row["FullName"].ToString(),
                        phone = row["phone"].ToString(),
                        email = row["email"].ToString(),
                        avatar = row["avatar"].ToString(),
                        bio = row["us_bio"].ToString(),
                        unReadCount = row["us_unreadCount"].ToInt(),
                        lastseen = string.IsNullOrEmpty(row["us_lastSeen"].ToString()) ? null : Convert.ToDateTime(row["us_lastSeen"].ToString()),
                        statusName = row["StatusName"].ToString(),
                        status = row["us_status"].ToInt(),
                    };
                }
            }

            response_users = new chatUserInfoResponseModelView(user);
            //API_ChatGetInfoUsers
            return JsonConvert.SerializeObject(response_users);
        }
        
        [Authorize]
        [HttpPut]
        [Route("api/[controller]/Users/{UserId}/Status")]

        public string UserCheangeStatus(int? UserId, [FromBody] chatUserCheangeStatusModelView request)
        {
            if (UserId == null)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "UserId ارائه نشده است.");
            }

            if (request.status == null || request.status < 0)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "Status نامشخص میباشد.");
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("UserId",UserId),
                new SqlParameter("Status",request.status),
            };
            bool update = cp.update("API_ChatCheangStatusUser", parameters);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
            }
            if (update)
            {
                return messagebase.messages(true, ErrorCode.Created.GetHashCode(), "");
            }
            else
            {
                return messagebase.messages(false, ErrorCode.Conflict.GetHashCode(), "");
            }
        }
        
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Conversations")]

        public string GetConversations()
        {
            int UserId = HttpContext.Items["ChatId"].ToInt();
            List<chatConversationInfoResponseModel> conversations = new List<chatConversationInfoResponseModel>(); 
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("UserId",UserId),
            };
            DataTable dt_conversations = cp.select_row("API_ChatGetConversations", parameters);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
            }
            if (dt_conversations.Rows.Count > 0)
            {
                foreach (DataRow row in dt_conversations.Rows)
                {
                    conversations.Add(new chatConversationInfoResponseModel()
                    {
                        id = row["con_id"].ToInt(),
                        sender = UserId,
                        receiver = row["receiver"].ToInt(),
                        lastMessage = row["lastMessage"].ToString(),
                        unReadCount = row["unReadCount"].ToInt(),
                        atCreate = string.IsNullOrEmpty(row["con_atCreate"].ToString()) ? null : Convert.ToDateTime(row["con_atCreate"].ToString()),
                        atUpdate = string.IsNullOrEmpty(row["atUpdate"].ToString()) ? null : Convert.ToDateTime(row["atUpdate"].ToString()),
                        status = row["con_status"].ToBoolean(),
                    });
                }
            }

            response_conversations = new chatConversationInfoResponseModelView(conversations);
            return JsonConvert.SerializeObject(response_conversations);
        }
        
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Conversations/{ConversationId}")]

        public string GetConversations(int? ConversationId)
        {
            if (ConversationId == null)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "ConversationId ارائه نشده است.");
            }
            chatConversationInfoResponseModel conversations = new chatConversationInfoResponseModel(); 
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("Id",ConversationId),
            };
            DataTable dt_conversation = cp.select_row("API_ChatGetConversation", parameters);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
            }
            if (dt_conversation.Rows.Count > 0)
            {
                foreach (DataRow row in dt_conversation.Rows)
                {
                    conversations = new chatConversationInfoResponseModel()
                    {
                        id = row["con_id"].ToInt(),
                        sender = row["sender"].ToInt(),
                        receiver = row["receiver"].ToInt(),
                        lastMessage = row["lastMessage"].ToString(),
                        unReadCount = row["unReadCount"].ToInt(),
                        atCreate = string.IsNullOrEmpty(row["con_atCreate"].ToString()) ? null : Convert.ToDateTime(row["con_atCreate"].ToString()),
                        atUpdate = string.IsNullOrEmpty(row["atUpdate"].ToString()) ? null : Convert.ToDateTime(row["atUpdate"].ToString()),
                        status = row["con_status"].ToBoolean(),
                    };
                }
            }

            response_conversations = new chatConversationInfoResponseModelView(conversations);
            return JsonConvert.SerializeObject(response_conversations);
        }
    }
}
