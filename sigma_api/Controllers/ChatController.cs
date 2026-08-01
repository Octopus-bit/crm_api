using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        public chatUserSResponseModelview response_users2;
        public chatConversationInfoResponseModelView response_conversations;
        public chatConversationCreatedResponseModelView response_createConversations;
        public chatMessageCreateResponseModelView response_createMessage;
        public chatMessagesResponseModelView response_messages;

        private readonly IWebHostEnvironment _env;
        public ChatController(IWebHostEnvironment env)
        {
            _env = env;
        }
        public string ImageConverter(byte[] file, string filename)
        {
            string folderPath = Path.Combine(_env.WebRootPath == null ? _env.ContentRootPath : _env.WebRootPath, "images-user");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, filename);
            string imageUrl = null;
            if (file != null)
                if (file.Length > 0)
                {

                    if (!System.IO.File.Exists(filePath))
                    {
                        System.IO.File.WriteAllBytes(filePath, file);
                    }

                    imageUrl = $"{Request.Scheme}://{Request.Host}/images-user/{filename}";
                }

            return imageUrl;
        }
        public string OptimizedImageConverter(byte[] file, string filename, string folderPath, string baseImageUrl)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return null;

                string filePath = Path.Combine(folderPath, filename);

                // Only write file if it doesn't exist
                if (!System.IO.File.Exists(filePath))
                {
                    System.IO.File.WriteAllBytes(filePath, file);
                }

                return baseImageUrl + filename;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        //[Authorize]
        //[HttpGet]
        //[Route("api/[controller]/GetUsersChat")]
        //public string GetUsersChat([FromQuery] ListRequest request)
        //{
        //    if (request.page_number < 1)
        //    {
        //        return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "شماره صفحه وارد شده یافت نشد");
        //    }
        //    ListResponse list = new ListResponse();
        //    List<chatUserInfoResponseModel> user = new List<chatUserInfoResponseModel>();
        //    //int type = HttpContext.Items["UserType"].ToInt();
        //    int Pages = 1;
        //    //SqlParameter[] parameters = new SqlParameter[]
        //    //{
        //    //    new SqlParameter("Type",type),
        //    //    new SqlParameter("UserId",DBNull.Value)
        //    //};
        //    DataTable dt_users = cp.select_command(@" SELECT USR_id, USR_pic, Name, Phone FROM     
        //    PRG_Users where USR_type=1");
        //    if (cls_public.sqlError.success == false)
        //    {
        //        return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
        //    }
        //    if (dt_users.Rows.Count > 0)
        //    {
        //        Pages = 1;// dt_users.Rows.Count / 15;
        //                  //if (Pages < 1)
        //                  //    Pages = 1;
        //                  //if (dt_users.Rows.Count > 15)
        //                  //{
        //                  //    dt_users = dt_users.AsEnumerable().Skip((request.page_number - 1) * 15).CopyToDataTable();
        //                  //    dt_users = dt_users.AsEnumerable().Take(15).CopyToDataTable();
        //                  //}
        //        bool hasImage = dt_users.Columns.Contains("Image");

        //        foreach (DataRow row in dt_users.Rows)
        //        {
        //            user.Add(new chatUserSResponseModel()
        //            {
        //                id = row["us_id"] != DBNull.Value ? Convert.ToInt32(row["USR_id"]) : 0,
        //                Name = row["FullName"] != DBNull.Value ? row["Name"].ToString() : string.Empty,
        //                phone = row["phone"] != DBNull.Value ? row["Phone"].ToString() : string.Empty,
        //                Image = hasImage ? OptimizedImageConverter(string.IsNullOrEmpty(row["USR_pic"].ToString()) ? null : (byte[])row["Image"], row["IS_id"].ToInt() + ".jpg", imageFolderPath, baseImageUrl) : null,
        //                //avatar = row["avatar"] != DBNull.Value ? ImageConverter((byte[])row["avatar"], HttpContext.Items["ChatId"].ToString() + ".jpg") : string.Empty,
        //                //bio = row["us_bio"] != DBNull.Value ? row["us_bio"].ToString() : string.Empty,
        //                //unReadCount = row["us_unreadCount"] != DBNull.Value ? Convert.ToInt32(row["us_unreadCount"]) : 0,
        //                //lastseen = row["us_lastSeen"] != DBNull.Value && !string.IsNullOrEmpty(row["us_lastSeen"].ToString()) ? Convert.ToDateTime(row["us_lastSeen"]) : (DateTime?)null,
        //                //statusName = row["StatusName"] != DBNull.Value ? row["StatusName"].ToString() : string.Empty,
        //                //status = row["us_status"] != DBNull.Value ? Convert.ToInt32(row["us_status"]) : 0

        //            });
        //        }
        //        list = new ListResponse()
        //        {
        //            Pages = Pages,
        //            Page_row = request.page_number,
        //            List = user
        //        };
        //    }

        //    response_users = new chatUserInfoResponseModelView(user.Count <= 0 ? null : list);
        //    //API_ChatGetInfoUsers
        //    return JsonConvert.SerializeObject(response_users);
        //}
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
            //List<chatUserSResponseModel> user = new List<chatUserSResponseModel>();
            int type = HttpContext.Items["UserType"].ToInt();
            int Pages = 1;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("Type",type),
                new SqlParameter("UserId",DBNull.Value)
            };
            DataTable dt_users = cp.select_row("API_ChatGetInfoUsers", parameters);
//            DataTable dt_users = cp.select_command(@"select USR_id ,USR_pic,Name + ' _ ' +isnull( Ch_Name,'') FullName
//,Phone from PRG_Users join PRG_AffiliationChart on AFC_usr_id=USR_id
//            join PRG_Chart on Ch_id=AFC_Post_id where USR_type=1");
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
            }
            if (dt_users.Rows.Count > 0)
            {
                //Pages = dt_users.Rows.Count / 15;
                //if (Pages < 1)بconv
                //    Pages = 1;
                //if (dt_users.Rows.Count > 15)
                //{
                //    dt_users = dt_users.AsEnumerable().Skip((request.page_number - 1) * 15).CopyToDataTable();
                //    dt_users = dt_users.AsEnumerable().Take(15).CopyToDataTable();
                //}
                foreach (DataRow row in dt_users.Rows)
                {
                    user.Add(new chatUserInfoResponseModel()
                    {
                        id = row["us_id"] != DBNull.Value ? Convert.ToInt32(row["us_id"]) : 0,
                        Name = row["FullName"] != DBNull.Value ? row["FullName"].ToString() : string.Empty,
                        phone = row["phone"] != DBNull.Value ? row["Phone"].ToString() : string.Empty,
                        Image = row["avatar"] != DBNull.Value ? ImageConverter((byte[])row["avatar"], HttpContext.Items["ChatId"].ToString() + ".jpg") : string.Empty,
                        bio = row["us_bio"] != DBNull.Value ? row["us_bio"].ToString() : string.Empty,
                        unReadCount = row["us_unreadCount"] != DBNull.Value ? Convert.ToInt32(row["us_unreadCount"]) : 0,
                        lastseen = row["us_lastSeen"] != DBNull.Value && !string.IsNullOrEmpty(row["us_lastSeen"].ToString()) ? Convert.ToDateTime(row["us_lastSeen"]) : (DateTime?)null,
                        statusName = row["StatusName"] != DBNull.Value ? row["StatusName"].ToString() : string.Empty,
                        status = row["us_status"] != DBNull.Value ? Convert.ToInt32(row["us_status"]) : 0,
                        email = row["email"] != DBNull.Value ? row["email"].ToString() : string.Empty,
                    });
                }
                list = new ListResponse()
                {
                    Pages = 1,
                    Page_row = 1,
                    List = user
                };
            }

            response_users = new chatUserInfoResponseModelView(user.Count <= 0 ? null : list);
            //API_ChatGetInfoUsers
            return JsonConvert.SerializeObject(response_users);
        }


        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Users/{UserId}")]
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
                        Image = ImageConverter(string.IsNullOrEmpty(row["avatar"].ToString()) ? null : (byte[])row["avatar"], UserId + ".jpg"),
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
        [HttpGet]
        [Route("api/[controller]/get-User-info")]
        public string GetUserInfo()
        {
            chatUserInfoResponseModel user = new chatUserInfoResponseModel();
            int type = HttpContext.Items["UserType"].ToInt();
            int UserId = HttpContext.Items["ChatId"].ToInt();

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
                        Image = ImageConverter(string.IsNullOrEmpty(row["avatar"].ToString()) ? null : (byte[])row["avatar"], UserId + ".jpg"),
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
        [HttpPost]
        [Route("api/[controller]/Conversations")]

        public string GetConversations(int Receiver)
        {
            int UserId = HttpContext.Items["ChatId"].ToInt();
            int UserId3 = HttpContext.Items["Userid"].ToInt();
            //if (request.Receiver == null || request.Receiver < 0)
            //{
            //    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "Receiver نامشخص میباشد.");
            //}
            //else if (request.Receiver == UserId)
            //{
            //    return messagebase.messages(false, ErrorCode.Conflict.GetHashCode(), "ایجاد گفتگو با حساب کاربری خود امکان پذیر نمیباشد");
            //}
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("UserId",UserId),
                new SqlParameter("ReceiverId",Receiver),
            };
            int con_id = cp.insert_getid("API_ChatCreateConversation", parameters).ToInt();
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
            }
            if (con_id > 0)
            {
                response_createConversations =
                    new chatConversationCreatedResponseModelView(con_id);
            }
            else
            {
                if (cls_public.sqlError.success == false)
                {
                    return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
                }
                else
                {
                    if (cls_public.sqlError.message != null)
                    {
                        return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), cls_public.sqlError.message);
                    }
                    else if (con_id == 0)
                    {
                        return messagebase.messages(false, ErrorCode.Duplicate.GetHashCode(),
                            "گفتگوی مورد نظر قبلا ایجاد شده است");
                    }
                    else
                    {
                        return messagebase.messages(false, ErrorCode.Conflict.GetHashCode(),
                                "ایجاد گفتگو با مشکل مواجه شد");
                    }
                }

            }

            return JsonConvert.SerializeObject(response_createConversations);
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

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Messages/{ConversationId}")]

        public string GetMessages(int? ConversationId)
        {
            if (ConversationId == null)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "ConversationId ارائه نشده است.");
            }
            int UserId = HttpContext.Items["ChatId"].ToInt();
            List<chatMessagesResponseModel> messages = new List<chatMessagesResponseModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("UserId",UserId),
                new SqlParameter("conId",ConversationId),
            };
            DataTable dt_messages = cp.select_row("API_ChatGetMessages", parameters);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
            }
            if (dt_messages.Rows.Count > 0)
            {
                foreach (DataRow row in dt_messages.Rows)
                {
                    messages.Add(new chatMessagesResponseModel()
                    {
                        id = row["mes_id"].ToInt(),
                        Sender = row["sender"].ToInt() == 0 ? null : row["sender"].ToInt(),
                        Receiver = row["receiver"].ToInt() == 0 ? null : row["receiver"].ToInt(),
                        Text = row["mes_text"].ToString(),
                        atCreate = string.IsNullOrEmpty(row["mes_atCreate"].ToString()) ? null : Convert.ToDateTime(row["mes_atCreate"].ToString()),
                        atSeen = string.IsNullOrEmpty(row["mes_atSeen"].ToString()) ? null : Convert.ToDateTime(row["mes_atSeen"].ToString()),
                        File = row["file"].ToString(),
                        FileName = row["fileName"].ToString(),
                        IsRead = row["isRead"].ToBoolean(),
                        HasFile = row["mes_type"].ToBoolean(),
                        ReplyTo = row["mes_ReplyTo"].ToInt(),
                    });
                }
            }

            response_messages = new chatMessagesResponseModelView(messages);
            return JsonConvert.SerializeObject(response_messages);
        }


        [Authorize]
        [HttpPut]
        [Route("api/[controller]/Messages/{messageid}/read")]

        public string ReadMessages(int? messageid)
        {
            if (messageid == null)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "messageid ارائه نشده است.");
            }
            List<chatMessagesResponseModel> messages = new List<chatMessagesResponseModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("MessageId",messageid),
            };
            bool update = cp.update("API_ChatSeenMessage", parameters);
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
        [HttpPost]
        [Route("api/[controller]/Messages")]

        public string SendMessages([FromBody] chatMessageCreateRequestModelView request)
        {
            try
            {
                if (request.ConversationId == null || request.ConversationId <= 0)
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "ConversationId ارائه نشده است.");
                }
                if (request.HasFile.ToBoolean() == false & string.IsNullOrEmpty(request.Text))
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "HasFile ارائه نشده است.");
                }
                if (request.FormId < 0)
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "FormId ارائه نشده است.");
                }
                if (request.CycleUser < 0)
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "CycleUser ارائه نشده است.");
                }
            }
            catch
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "Method not allowed");
            }
            int UserId = HttpContext.Items["ChatId"].ToInt();
            List<chatMessagesResponseModel> messages = new List<chatMessagesResponseModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ConId", request.ConversationId),
                new SqlParameter("@UserId", UserId),
                new SqlParameter("@Type", request.HasFile.ToInt()),
                new SqlParameter("@Text", string.IsNullOrEmpty(request.Text) ? DBNull.Value : request.Text),
                new SqlParameter("@FormName", string.IsNullOrEmpty(request.FormName) ? DBNull.Value : request.FormName),
                new SqlParameter("@FormId", request.FormId),
                new SqlParameter("@CycleUser", request.CycleUser),
            };
            int mes_id = cp.insert_getid("API_ChatCreateMessage", parameters).ToInt();
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
            }
            if (mes_id > 0)
            {
                response_createMessage =
                    new chatMessageCreateResponseModelView(mes_id);
            }
            else
            {
                if (cls_public.sqlError.success == false)
                {
                    response_createMessage =
                        new chatMessageCreateResponseModelView(ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
                }
                else
                {
                    if (cls_public.sqlError.message != null)
                    {
                        response_createMessage =
                            new chatMessageCreateResponseModelView(ErrorCode.DBO_Error.GetHashCode(), cls_public.sqlError.message);
                    }
                    else
                    {
                        response_createMessage =
                            new chatMessageCreateResponseModelView(ErrorCode.Conflict.GetHashCode(),
                            "ارسال پیام با مشکل مواجه شد");
                    }
                }

            }

            return JsonConvert.SerializeObject(response_createMessage);

        }


        [Authorize]
        [HttpPost]
        [Route("api/[controller]/Upload")]

        public string UploadFile([FromForm] chatUploadFileRequestModelView request)
        {
            try
            {

                if (request.MessageId == null || request.MessageId <= 0)
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "MessageId ارائه نشده است.");
                }
                if (request.File == null)
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "file ارائه نشده است.");
                }
                if (string.IsNullOrEmpty(request.FileName))
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "filename ارائه نشده است.");
                }

            }
            catch
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "Method not allowed");
            }
            using var memoryStream = new MemoryStream();
            request.File.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("file",fileBytes),
                new SqlParameter("filename",request.FileName),
                new SqlParameter("mes_id",request.MessageId),
            };
            bool insert = cp.insert("API_ChatUploadFile", parameters);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
            }
            if (insert)
            {
                return messagebase.messages(true, ErrorCode.Created.GetHashCode(), "آپلود فایل با موفقیت انجام شد");
            }
            else
            {
                if (cls_public.sqlError.success == false)
                {
                    return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
                }
                else
                {
                    if (cls_public.sqlError.message != null)
                    {
                        return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), cls_public.sqlError.message);
                    }
                }
            }
            return messagebase.messages(false, ErrorCode.Conflict.GetHashCode(), "آپلود فایل با خطا مواجه شد");
        }



        [HttpGet]
        [Route("api/[controller]/Upload/{messageid}/download")]

        public IActionResult DownloadFile(int? messageid)
        {
            if (messageid == null || messageid < 0)
            {
                return NotFound(messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "MessageId ارائه نشده است."));
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("mes_id",messageid),
            };
            DataTable dt_file = cp.select_row("API_ChatGetFileUploaded", parameters);
            if (cls_public.sqlError.success == false)
            {
                return NotFound(messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message));
            }
            if (dt_file.Rows.Count > 0)
            {
                byte[] file = (byte[])dt_file.Rows[0]["file"];
                string filename = dt_file.Rows[0]["filename"].ToString();
                var extension = Path.GetExtension(filename).ToLowerInvariant();

                var contactType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".pdf" => "application/pdf",
                    ".txt" => "text/plain",
                    ".doc" or ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".xls" or ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    ".zip" => "application/zip",
                    _ => "application/octet-stream",
                };
                return File(file, contactType, filename);
            }
            else
            {
                return NotFound(messagebase.messages(false, ErrorCode.Conflict.GetHashCode(), "فایل موجود نیست"));
            }
        }
    }
}
