using CodeAtWork.DAL;
using CodeAtWork.Models;
using CodeAtWork.Models.Session;
using CodeAtWork.Models.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CodeAtWork.BL
{
    public class CodeAtWorkAdminBL
    {
        CodeAtWorkAdminDAL dal;

        public CodeAtWorkAdminBL()
        {
            dal = new CodeAtWorkAdminDAL();
        }

        public HtmlString GetTopics()
        {
            var topics = dal.GetTopics();

            return ConvertTopicsToSelectOptions(topics);

        }

        internal dynamic GetUsers(bool isActive, string filterBy)
        {
            return ConvertToTableHtml(dal.GetUsersList(isActive, filterBy));
        }

        internal HtmlString GetUsernameList()
        {
            return ConvertToDropDownOpt(dal.GetUsersList());
        }

        private HtmlString ConvertTopicsToSelectOptions(List<InterestCatergoryTopic> topics)
        {
            string result = "";
            topics.ForEach(t =>
            {
                result += $"<option value=\"{t.InterestCategoryTopicId}\">{t.Name}</option>";
            });

            return new HtmlString(result);
        }

        internal void DeactivateAccounts(IEnumerable<int> userIds, bool toDeactive)
        {
            dal.DeactivateAccounts(userIds, toDeactive);
        }

        public Guid SaveNewVid(CreateVid vid)
        {
            return dal.SaveNewVideo(vid);
        }

        public void GetDetailsAndDownload(IEnumerable<int> UserIds, bool includeInProgress, bool includeCompleted)
        {
            var users = dal.GetUsersList(UserIds);

            List<UserVideoLog> usersVidLog = new List<UserVideoLog>();
            List<VideoRepository> vidsNameAndId = new List<VideoRepository>();

            if (includeInProgress || includeCompleted)
            {
                usersVidLog = dal.GetVideoLog(UserIds);

                if (usersVidLog.Any())
                    vidsNameAndId = dal.GetVideoIdAndName(usersVidLog.Select(z => z.VideoId).Distinct().ToList());
            }

            var filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exports", "UserReport.csv");
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                var headers = "Username, Full Name, Created On, Last Login";
                if (includeInProgress)
                {
                    headers += ", Videos - In Progress";
                }
                if (includeCompleted)
                {
                    headers += ", Videos - Completed";
                }

                writer.WriteLine(headers);

                users.ForEach(u =>
                {
                    Queue<Guid> userCompletedVideos = new Queue<Guid>(); //might help to change to queue
                    List<Guid> userInProgressVideos = new List<Guid>();

                    if (includeCompleted)
                    {
                        usersVidLog.Where(z => z.AppUserId == u.AppUserId && z.IsFinished)?.ToList().ForEach(v => {
                            if (!userCompletedVideos.Contains(v.VideoId))
                                userCompletedVideos.Enqueue(v.VideoId);
                                });
                    }

                    if (includeInProgress)
                    {
                        usersVidLog.Where(z => z.AppUserId == u.AppUserId && !z.IsFinished)?.ToList().GroupBy(z => z.VideoId).ToList().ForEach(z =>
                        {
                            userInProgressVideos.Add(z.OrderBy(vl => vl.LastModifiedTimestamp).First().VideoId);
                        });

                    }

                    var firstLine = $"{u.Username}, {u.DisplayName}, {u.CreatedOn:dddd; dd MMMM yyyy HH:mm:ss}, {u.LastLogin:dddd; dd MMMM yyyy HH:mm:ss}";
                    List<string> subSequentLines = new List<string>();
                    if (includeInProgress)
                    {
                        var firstRow = true;
                        userInProgressVideos.ForEach(PV =>
                        {
                            var vidName = FormatVidName(PV);

                            if (firstRow)
                            {
                                firstLine += $", {vidName}";
                                firstRow = false;
                            }
                            else
                            {
                                subSequentLines.Add($",,,,{vidName}");
                            }
                        });
                    }

                    if (includeCompleted && userCompletedVideos.Any())
                    {
                        var commaCount = firstLine.Count(z => z == ',');

                        if (commaCount == 3 && includeInProgress)
                            firstLine += ",";
                        var vidId = userCompletedVideos.Dequeue();
                        var vidName = FormatVidName(vidId);

                        firstLine += $", {vidName}";


                        for (int i = 0; i< subSequentLines.Count; i++)
                        {
                            if (userCompletedVideos.Any())
                            {
                                commaCount = subSequentLines[i].Count(z => z == ',');
                                if (commaCount == 3 && includeInProgress)
                                    subSequentLines[i] += ",";
                                vidId = userCompletedVideos.Dequeue();
                                vidName = FormatVidName(vidId);

                                subSequentLines[i] += $", {vidName}";
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (userCompletedVideos.Any())
                        {
                            var newLine = ",,,,";
                            if (includeInProgress)
                                newLine += ",";
                            do
                            {
                                vidId = userCompletedVideos.Dequeue();
                                vidName = FormatVidName(vidId);

                                subSequentLines.Add(newLine + vidName);
                            } while (userCompletedVideos.Any());

                        }
                    }
                    writer.WriteLine(firstLine);
                    subSequentLines.ForEach(z => writer.WriteLine(z));
                });
            }

            string FormatVidName (Guid VidId) {
                var vidName = vidsNameAndId.Single(z => z.VideoId == VidId).VideoDescription;

                if (vidName.Contains("\""))
                {
                    vidName = vidName.Replace("\"", "\"\"");
                }

                if (vidName.Contains(","))
                {
                    vidName = String.Format("\"{0}\"", vidName);
                }

                if (vidName.Contains(Environment.NewLine))
                {
                    vidName = String.Format("\"{0}\"", vidName);
                }

                return vidName;
            }
        }


        public HtmlString ConvertToDropDownOpt(List<FullUserDetail> UC)
        {
            string result = $"<a> <input type=\"checkbox\" id=\"AllUserCheckBox\"  onchange=\"ToggleAllUsers()\"> All Users </a>";

            UC.ForEach(u =>
            {
                result += $"<a id=\"UserRow_{u.AppUserId}\"> <input type=\"checkbox\" Id=\"UserCheckBox_{u.AppUserId}\"  class=\"UserCheckBox\"  onchange=\"thisRowSelected()\"> {u.Username} ({u.DisplayName})</a>";
            });

            return new HtmlString(result);
        }
        public HtmlString ConvertToTableHtml(List<FullUserDetail> UC)
        {
            string result = "";
            UC.ForEach(u =>
            {
                result += $"<tr id=\"channelRow_{u.AppUserId}\">" +
                $"<td><input type=\"checkbox\" onchange=\"softDeleteRow(this, {u.AppUserId})\" /></td>" +
                $"<td >{u.FirstName}</td> " +
               $"<td>({u.LastName}) Videos</td> " +
              $"<td>({u.Email}) Paths</td> " +
                $"<td>By {u.Company}</td> " +
                "</tr> ";
            });

            return new HtmlString(result);
        }
    }
}